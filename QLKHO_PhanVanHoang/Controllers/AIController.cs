using QLKHO_PhanVanHoang.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLKHO_PhanVanHoang.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QLKHO_PhanVanHoang.Data;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;

namespace QLKHO_PhanVanHoang.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AIController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = string.Empty;
        }

        public class TranslateAuditRequest
        {
            public string Action { get; set; } = string.Empty;
            public string EntityName { get; set; } = string.Empty;
            public string? OldValues { get; set; }
            public string? NewValues { get; set; }
        }

        [HttpPost("translate-audit")]
        public async Task<IActionResult> TranslateAudit([FromBody] TranslateAuditRequest request)
        {
            try
            {
                var apiKey = _configuration["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return StatusCode(500, ApiResponse<object>.FailureResult("API Key của Gemini chưa được cấu hình."));
                }

                var sbContext = new StringBuilder();
                sbContext.AppendLine("Bạn là một chuyên gia quản trị hệ thống WMS (Kho hàng).");
                sbContext.AppendLine("Người dùng vừa thực hiện một hành động làm thay đổi cơ sở dữ liệu. Dưới đây là dữ liệu log dưới dạng JSON. Hãy dịch sự thay đổi này sang một đoạn văn mô tả tiếng Việt thật ngắn gọn, chuyên nghiệp, tự nhiên và dễ hiểu cho con người. Không giải thích dài dòng, chỉ đi thẳng vào vấn đề. Nếu có JSON thì nói rõ trường nào thay đổi thành gì.");
                sbContext.AppendLine($"Hành động: {request.Action}");
                sbContext.AppendLine($"Bảng dữ liệu: {request.EntityName}");
                sbContext.AppendLine($"Giá trị cũ: {request.OldValues ?? "Không có"}");
                sbContext.AppendLine($"Giá trị mới: {request.NewValues ?? "Không có"}");

                using var client = new HttpClient();
                var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = sbContext.ToString() }
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errBody = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, ApiResponse<object>.FailureResult($"Gemini API Error: {errBody}"));
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                
                using var doc = JsonDocument.Parse(responseBody);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return Ok(ApiResponse<object>.SuccessResult(new { translation = text?.Trim() }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailureResult($"Lỗi dịch AI: {ex.Message}"));
            }
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest(ApiResponse<object>.FailureResult("Tin nhắn không được để trống."));
            }

            try
            {
                var isAdmin = User.IsInRole(AppRoles.Admin);
                var permissions = User.Claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToHashSet();
                bool HasPerm(string perm) => isAdmin || permissions.Contains(perm);

                // 1. Phân hệ Danh mục & Cơ sở hạ tầng
                var products = HasPerm("PRODUCT_VIEW") ? await _context.Products
                    .Include(p => p.Category)
                    .Where(p => !p.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync() : new List<Product>();

                var warehouses = HasPerm("WAREHOUSE_VIEW") ? await _context.Warehouses
                    .Where(w => !w.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync() : new List<Warehouse>();

                var suppliers = HasPerm("MASTER_DATA_VIEW") ? await _context.Suppliers
                    .Where(s => !s.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync() : new List<Supplier>();

                var customers = HasPerm("MASTER_DATA_VIEW") ? await _context.Customers
                    .Where(c => !c.IsDeleted)
                    .AsNoTracking()
                    .ToListAsync() : new List<Customer>();

                // 2. Phân hệ Tồn kho theo lô chi tiết
                var detailedInventories = (HasPerm("REPORT_VIEW") || HasPerm("STOCK_CARD_VIEW")) ? await _context.Inventories
                    .Include(i => i.Product)
                    .Include(i => i.Warehouse)
                    .Where(i => !i.IsDeleted && i.QuantityOnHand > 0)
                    .OrderBy(i => i.ExpiryDate)
                    .AsNoTracking()
                    .ToListAsync() : new List<Inventory>();

                var groupQty = detailedInventories
                    .GroupBy(i => i.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.QuantityOnHand));

                // 3. Phân hệ Vận hành & Giao dịch
                var stockCards = HasPerm("STOCK_CARD_VIEW") ? await _context.StockCards
                    .Include(s => s.Product)
                    .Include(s => s.Warehouse)
                    .Where(s => !s.IsDeleted)
                    .OrderByDescending(s => s.TransactionDate)
                    .Take(10)
                    .AsNoTracking()
                    .ToListAsync() : new List<StockCard>();

                var inboundVouchers = HasPerm("INBOUND_VIEW") ? await _context.ReceivingVouchers
                    .Include(v => v.Supplier)
                    .Where(v => !v.IsDeleted)
                    .OrderByDescending(v => v.CreatedAt)
                    .Take(10)
                    .AsNoTracking()
                    .ToListAsync() : new List<ReceivingVoucher>();

                var outboundVouchers = HasPerm("OUTBOUND_VIEW") ? await _context.DeliveryVouchers
                    .Include(v => v.Customer)
                    .Where(v => !v.IsDeleted)
                    .OrderByDescending(v => v.CreatedAt)
                    .Take(10)
                    .AsNoTracking()
                    .ToListAsync() : new List<DeliveryVoucher>();

                var transfers = HasPerm("TRANSFER_VIEW") ? await _context.TransferVouchers
                    .Include(t => t.FromWarehouse)
                    .Include(t => t.ToWarehouse)
                    .Where(t => !t.IsDeleted)
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync() : new List<TransferVoucher>();

                // 4. Phân hệ Kiểm kê & Nhật ký hệ thống
                var countings = HasPerm("COUNTING_VIEW") ? await _context.CountingSheets
                    .Where(c => !c.IsDeleted)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync() : new List<CountingSheet>();

                var auditLogs = HasPerm("SYSTEM_LOGS") ? await _context.AuditLogs
                    .OrderByDescending(l => l.ChangedAt)
                    .Take(10)
                    .AsNoTracking()
                    .ToListAsync() : new List<AuditLog>();

                // 5. Biên soạn văn bản ngữ cảnh toàn diện
                var sbContext = new StringBuilder();
                sbContext.AppendLine("Bạn là Trợ lý Quản lý Kho thông minh WMS (AI Advisor) được phát triển bởi Phan Văn Hoàng.");
                sbContext.AppendLine($"Ngày hiện tại của hệ thống: {DateTime.Now:dd/MM/yyyy HH:mm}");
                sbContext.AppendLine("Dưới đây là DỮ LIỆU TOÀN DIỆN VỀ HOẠT ĐỘNG KHO được trích xuất trực tiếp từ database của hệ thống:");
                sbContext.AppendLine();

                // --- 5.1. DANH SÁCH SẢN PHẨM & GIÁ CẢ ---
                sbContext.AppendLine("--- DANH SÁCH SẢN PHẨM & GIÁ CẢ ---");
                if (HasPerm("PRODUCT_VIEW"))
                {
                    var alerts = new List<string>();
                    foreach (var p in products)
                    {
                        decimal qty = groupQty.ContainsKey(p.Id) ? groupQty[p.Id] : 0;
                        sbContext.AppendLine($"- SKU: {p.SkuCode} | Tên: {p.Name} | Danh mục: {p.Category?.Name ?? "N/A"} | Tồn: {qty} {p.Unit} | Giá vốn: {p.CostPrice:N0} VNĐ | Giá bán: {p.SellingPrice:N0} VNĐ | Ngưỡng an toàn: {p.MinStockLevel} {p.Unit}");
                        
                        if (qty < p.MinStockLevel)
                        {
                            alerts.Add($"+ SKU: {p.SkuCode} | Tên: {p.Name} | Thiếu hụt: {p.MinStockLevel - qty} {p.Unit} (Tồn {qty}/{p.MinStockLevel} an toàn)");
                        }
                    }
                    sbContext.AppendLine();

                    // --- 5.2. CẢNH BÁO THIẾU HỤT ---
                    sbContext.AppendLine("--- CẢNH BÁO THIẾU HỤT TỒN KHO ---");
                    if (alerts.Any())
                    {
                        foreach (var a in alerts)
                        {
                            sbContext.AppendLine(a);
                        }
                    }
                    else
                    {
                        sbContext.AppendLine("Không có sản phẩm nào dưới ngưỡng tồn kho an toàn.");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền truy cập dữ liệu Sản phẩm.");
                }
                sbContext.AppendLine();

                // --- 5.3. CHI TIẾT CÁC LÔ HÀNG ĐANG LƯU KHO & HẠN SỬ DỤNG ---
                sbContext.AppendLine("--- CHI TIẾT CÁC LÔ HÀNG ĐANG LƯU KHO & HẠN SỬ DỤNG ---");
                if (HasPerm("REPORT_VIEW") || HasPerm("STOCK_CARD_VIEW"))
                {
                    if (detailedInventories.Any())
                    {
                        foreach (var inv in detailedInventories)
                        {
                            var expiryStr = inv.ExpiryDate.HasValue ? inv.ExpiryDate.Value.ToString("dd/MM/yyyy") : "Không quản lý hạn";
                            sbContext.AppendLine($"- SP: {inv.Product?.Name} ({inv.Product?.SkuCode}) | Số lô: {inv.LotNumber ?? "N/A"} | Hạn SD: {expiryStr} | Số lượng: {inv.QuantityOnHand} {inv.Product?.Unit} | Vị trí: {inv.LocationInWarehouse ?? "N/A"} | Kho: {inv.Warehouse?.Name}");
                        }
                    }
                    else
                    {
                        sbContext.AppendLine("Không có lô hàng nào đang lưu trữ trong hệ thống.");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem chi tiết tồn kho.");
                }
                sbContext.AppendLine();

                // --- 5.4. THÔNG TIN CƠ SỞ HẠ TẦNG KHO ---
                sbContext.AppendLine("--- THÔNG TIN CƠ SỞ HẠ TẦNG KHO ---");
                if (HasPerm("WAREHOUSE_VIEW"))
                {
                    foreach (var w in warehouses)
                    {
                        sbContext.AppendLine($"- Kho: {w.Name} | Vị trí: {w.Location ?? "N/A"} | SĐT: {w.PhoneNumber ?? "N/A"}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem cơ sở hạ tầng kho.");
                }
                sbContext.AppendLine();

                // --- 5.5. DANH SÁCH ĐỐI TÁC (NHÀ CUNG CẤP & KHÁCH HÀNG) ---
                sbContext.AppendLine("--- DANH SÁCH NHÀ CUNG CẤP ---");
                if (HasPerm("MASTER_DATA_VIEW"))
                {
                    foreach (var s in suppliers)
                    {
                        sbContext.AppendLine($"- NCC: {s.Name} ({s.Code ?? "N/A"}) | Người liên hệ: {s.ContactPerson ?? "N/A"} | Email: {s.Email ?? "N/A"} | SĐT: {s.PhoneNumber ?? "N/A"} | Địa chỉ: {s.Address ?? "N/A"}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem nhà cung cấp.");
                }
                sbContext.AppendLine();

                sbContext.AppendLine("--- DANH SÁCH KHÁCH HÀNG ---");
                if (HasPerm("MASTER_DATA_VIEW"))
                {
                    foreach (var c in customers)
                    {
                        sbContext.AppendLine($"- KH: {c.Name} ({c.Code ?? "N/A"}) | Người liên hệ: {c.ContactPerson ?? "N/A"} | Email: {c.Email ?? "N/A"} | SĐT: {c.PhoneNumber ?? "N/A"} | Địa chỉ: {c.Address ?? "N/A"}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem khách hàng.");
                }
                sbContext.AppendLine();

                // --- 5.6. VẬN HÀNH & GIAO DỊCH GẦN ĐÂY ---
                sbContext.AppendLine("--- 10 BIẾN ĐỘNG THẺ KHO GẦN ĐÂY NHẤT ---");
                if (HasPerm("STOCK_CARD_VIEW"))
                {
                    foreach (var s in stockCards)
                    {
                        sbContext.AppendLine($"- Ngày: {s.TransactionDate:dd/MM/yyyy HH:mm} | SP: {s.Product?.Name} ({s.Product?.SkuCode}) | Kho: {s.Warehouse?.Name} | Loại GD: {s.TransactionType} | Số lượng thay đổi: {s.ChangeQuantity} (Tồn trước: {s.BeforeQuantity} -> Tồn sau: {s.AfterQuantity})");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem biến động thẻ kho.");
                }
                sbContext.AppendLine();

                sbContext.AppendLine("--- 10 PHIẾU NHẬP KHO GẦN ĐÂY NHẤT ---");
                if (HasPerm("INBOUND_VIEW"))
                {
                    foreach (var iv in inboundVouchers)
                    {
                        sbContext.AppendLine($"- Phiếu nhập: {iv.Code} | NCC: {iv.Supplier?.Name} | Ngày lập: {iv.CreatedAt:dd/MM/yyyy} | Trạng thái: {iv.Status}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem phiếu nhập kho.");
                }
                sbContext.AppendLine();

                sbContext.AppendLine("--- 10 PHIẾU XUẤT KHO GẦN ĐÂY NHẤT ---");
                if (HasPerm("OUTBOUND_VIEW"))
                {
                    foreach (var ov in outboundVouchers)
                    {
                        sbContext.AppendLine($"- Phiếu xuất: {ov.Code} | KH: {ov.Customer?.Name} | Ngày lập: {ov.CreatedAt:dd/MM/yyyy} | Trạng thái: {ov.Status}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem phiếu xuất kho.");
                }
                sbContext.AppendLine();

                sbContext.AppendLine("--- 5 PHIẾU ĐIỀU CHUYỂN KHO GẦN ĐÂY NHẤT ---");
                if (HasPerm("TRANSFER_VIEW"))
                {
                    foreach (var t in transfers)
                    {
                        sbContext.AppendLine($"- Phiếu điều chuyển: {t.Code} | Từ kho: {t.FromWarehouse?.Name} -> Đến kho: {t.ToWarehouse?.Name} | Ngày lập: {t.CreatedAt:dd/MM/yyyy} | Trạng thái: {t.Status}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem phiếu điều chuyển.");
                }
                sbContext.AppendLine();

                // --- 5.7. KIỂM KÊ & AN NINH HỆ THỐNG ---
                sbContext.AppendLine("--- 5 PHIẾU KIỂM KÊ GẦN ĐÂY NHẤT ---");
                if (HasPerm("COUNTING_VIEW"))
                {
                    foreach (var cs in countings)
                    {
                        sbContext.AppendLine($"- Phiếu kiểm kê: {cs.Code} | Ngày lập: {cs.CreatedAt:dd/MM/yyyy} | Trạng thái: {cs.Status}");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem phiếu kiểm kê.");
                }
                sbContext.AppendLine();

                sbContext.AppendLine("--- 10 NHẬT KÝ HOẠT ĐỘNG HỆ THỐNG GẦN ĐÂY NHẤT ---");
                if (HasPerm("SYSTEM_LOGS"))
                {
                    foreach (var l in auditLogs)
                    {
                        sbContext.AppendLine($"- Ngày: {l.ChangedAt:dd/MM/yyyy HH:mm} | Nhân viên: {l.ChangedBy} | Hành động: {l.Action} | Bảng: {l.EntityName} (ID: {l.EntityId})");
                    }
                }
                else
                {
                    sbContext.AppendLine("Người dùng không có quyền xem nhật ký hệ thống.");
                }
                sbContext.AppendLine();

                // --- 5.8. YÊU CẦU ĐỐI VỚI BẠN (AI ADVISOR) ---
                sbContext.AppendLine("--- YÊU CẦU ĐỐI VỚI BẠN (AI ADVISOR) ---");
                sbContext.AppendLine("1. Nếu ở mục dữ liệu nào có thông báo \"Người dùng không có quyền...\", hãy từ chối lịch sự khi người dùng hỏi về mục dữ liệu đó (vd: Bạn không được cấp quyền xem dữ liệu này).");
                sbContext.AppendLine("2. Hãy trả lời bằng tiếng Việt lịch sự, chuyên nghiệp, mang phong cách của một chuyên gia phân tích chuỗi cung ứng thực thụ.");
                sbContext.AppendLine("3. Khuyến khích sử dụng định dạng Markdown sạch sẽ (in đậm, danh sách gạch đầu dòng, bảng biểu) để câu trả lời hiển thị chuyên nghiệp nhất.");
                sbContext.AppendLine("4. Nếu người dùng hỏi các câu hỏi tính toán như tổng giá trị tồn kho của sản phẩm, tổng giá trị kho hàng, hãy tự động nhân số lượng tồn chi tiết (QuantityOnHand) với giá vốn (CostPrice) của từng sản phẩm đó và tính tổng tiền cụ thể (nếu có quyền xem sản phẩm).");
                sbContext.AppendLine("5. Nếu người dùng hỏi về các lô hàng cận hạn, hãy đối chiếu hạn sử dụng (ExpiryDate) với ngày giờ hệ thống hiện tại được ghi nhận ở trên để tìm ra các lô hàng cận hạn (ví dụ: còn dưới 30 ngày) hoặc đã quá hạn và cảnh báo.");
                sbContext.AppendLine("6. Nếu người dùng hỏi điều gì không có trong dữ liệu kho, hãy trả lời khéo léo và gợi ý các chủ đề có thể giải quyết.");
                sbContext.AppendLine();
                sbContext.AppendLine($"Câu hỏi của nhân viên: {request.Message}");

                // 3. Gọi Gemini API
                var apiKey = _configuration["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    return StatusCode(500, ApiResponse<object>.FailureResult("API Key của Gemini chưa được cấu hình."));
                }

                using var client = new HttpClient();
                var endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = sbContext.ToString() }
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(endpoint, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[AI] Gemini API Error: Code={response.StatusCode}, Body={errBody}");
                    return StatusCode((int)response.StatusCode, ApiResponse<object>.FailureResult($"Gemini API Error: {errBody}"));
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                
                // Parse response để lấy phần text trả về
                using var doc = JsonDocument.Parse(responseBody);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return Ok(ApiResponse<object>.SuccessResult(new { response = text }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.FailureResult($"Lỗi xử lý AI: {ex.Message}"));
            }
        }
    }
}


