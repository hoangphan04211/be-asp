using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Helpers;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Repositories;

namespace QLKHO_PhanVanHoang.Services
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _unitOfWork;

        private static readonly Dictionary<string, string> _entityNames = new()
        {
            { "Product", "Sản phẩm" },
            { "Category", "Danh mục" },
            { "Warehouse", "Kho hàng" },
            { "Supplier", "Nhà cung cấp" },
            { "Customer", "Khách hàng" },
            { "SystemUser", "Người dùng" },
            { "ReceivingVoucher", "Phiếu nhập kho" },
            { "DeliveryVoucher", "Phiếu xuất kho" },
            { "TransferVoucher", "Phiếu chuyển kho" },
            { "CountingSheet", "Phiếu kiểm kê" },
            { "Inventory", "Tồn kho" },
            { "InventoryAdjustment", "Cân đối kho" }
        };

        private static readonly Dictionary<string, string> _propertyNames = new()
        {
            { "Name", "Tên" },
            { "SkuCode", "Mã SKU" },
            { "QuantityOnHand", "Số lượng tồn" },
            { "CostPrice", "Giá vốn" },
            { "SellingPrice", "Giá bán" },
            { "Status", "Trạng thái" },
            { "Description", "Mô tả" },
            { "Address", "Địa chỉ" },
            { "PhoneNumber", "Số điện thoại" },
            { "Email", "Email" },
            { "FullName", "Họ tên" },
            { "IsActive", "Đang hoạt động" },
            { "RoleId", "ID Vai trò" },
            { "Note", "Ghi chú" },
            { "Notes", "Ghi chú" }
        };

        public AuditService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<AuditLogDto>> GetPagedLogsAsync(AuditLogParams @params)
        {
            var pagedLogs = await _unitOfWork.AuditLogs.GetPagedAsync(
                @params.PageNumber,
                @params.PageSize,
                a => (string.IsNullOrEmpty(@params.EntityName) || a.EntityName == @params.EntityName) &&
                     (string.IsNullOrEmpty(@params.Action) || a.Action == @params.Action),
                q => q.OrderByDescending(a => a.ChangedAt));

            var dtos = pagedLogs.Items.Select(log => new AuditLogDto
            {
                Id = log.Id,
                EntityName = _entityNames.GetValueOrDefault(log.EntityName, log.EntityName),
                Action = log.Action,
                ChangedBy = log.ChangedBy,
                ChangedAt = log.ChangedAt,
                OldValues = log.OldValues,
                NewValues = log.NewValues,
                FriendlyDescription = GenerateFriendlyDescription(log)
            }).ToList();

            return new PagedResult<AuditLogDto>
            {
                Items = dtos,
                TotalCount = pagedLogs.TotalCount,
                TotalPages = pagedLogs.TotalPages,
                PageNumber = pagedLogs.PageNumber,
                PageSize = pagedLogs.PageSize
            };
        }

        private string GenerateFriendlyDescription(AuditLog log)
        {
            string vietnameseEntity = _entityNames.GetValueOrDefault(log.EntityName, log.EntityName);
            string actionVerb = log.Action switch
            {
                "Create" => "đã tạo mới",
                "Update" => "đã cập nhật",
                "Delete" => "đã xóa",
                "HardDelete" => "đã xóa vĩnh viễn",
                _ => $"đã thực hiện {log.Action}"
            };

            string description = $"{log.ChangedBy} {actionVerb} {vietnameseEntity} (ID: {log.EntityId})";

            if (log.Action.ToUpper() == "UPDATE" && !string.IsNullOrEmpty(log.NewValues))
            {
                try
                {
                    var oldValues = string.IsNullOrEmpty(log.OldValues) ? new Dictionary<string, object>() : JsonSerializer.Deserialize<Dictionary<string, object>>(log.OldValues);
                    var newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(log.NewValues);

                    if (newValues != null && oldValues != null)
                    {
                        var changes = new List<string>();
                        foreach (var kvp in newValues)
                        {
                            if (oldValues.TryGetValue(kvp.Key, out var oldValue))
                            {
                                if (!Equals(oldValue?.ToString(), kvp.Value?.ToString()))
                                {
                                    string propName = _propertyNames.GetValueOrDefault(kvp.Key, kvp.Key);
                                    changes.Add($"{propName} từ '{oldValue}' sang '{kvp.Value}'");
                                }
                            }
                        }

                        if (changes.Count > 0)
                        {
                            description += ": " + string.Join(", ", changes);
                        }
                    }
                }
                catch
                {
                    // Fallback if JSON parsing fails
                }
            }

            return description;
        }
    }
}
