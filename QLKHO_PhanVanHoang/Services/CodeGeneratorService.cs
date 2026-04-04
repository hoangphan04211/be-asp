using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLKHO_PhanVanHoang.Data;

namespace QLKHO_PhanVanHoang.Services
{
    public class CodeGeneratorService : ICodeGeneratorService
    {
        private readonly ApplicationDbContext _context;

        public CodeGeneratorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateProductCodeAsync()
        {
            var today = DateTime.Today.ToString("yyyyMMdd");
            int count = await _context.Products.IgnoreQueryFilters().CountAsync();
            return $"SP-{count + 1:D5}";
        }

        public async Task<string> GenerateSupplierCodeAsync()
        {
            int count = await _context.Suppliers.IgnoreQueryFilters().CountAsync();
            return $"NCC-{count + 1:D3}";
        }

        public async Task<string> GenerateCustomerCodeAsync()
        {
            int count = await _context.Customers.IgnoreQueryFilters().CountAsync();
            return $"KH-{count + 1:D3}";
        }

        public async Task<string> GenerateReceivingCodeAsync()
        {
            return await GenerateVoucherCodeAsync("PN");
        }

        public async Task<string> GenerateDeliveryCodeAsync()
        {
            return await GenerateVoucherCodeAsync("PX");
        }

        public async Task<string> GenerateTransferCodeAsync()
        {
            return await GenerateVoucherCodeAsync("DC");
        }

        public async Task<string> GenerateCountingCodeAsync()
        {
            return await GenerateVoucherCodeAsync("KK");
        }

        private async Task<string> GenerateVoucherCodeAsync(string prefix)
        {
            var todayStr = DateTime.Today.ToString("yyyyMMdd");
            var prefixWithDate = $"{prefix}-{todayStr}-";

            // Find max sequence for today
            int sequence = 1;
            
            // Simple logic: count for the day (we can refine this by looking at substring)
            // But to be precise, we check existing codes
            var latestCode = "";
            switch (prefix)
            {
                case "PN": 
                    latestCode = (await _context.ReceivingVouchers.Where(v => v.Code.StartsWith(prefixWithDate)).OrderByDescending(v => v.Code).FirstOrDefaultAsync())?.Code;
                    break;
                case "PX":
                    latestCode = (await _context.DeliveryVouchers.Where(v => v.Code.StartsWith(prefixWithDate)).OrderByDescending(v => v.Code).FirstOrDefaultAsync())?.Code;
                    break;
                case "DC":
                    latestCode = (await _context.TransferVouchers.Where(v => v.Code.StartsWith(prefixWithDate)).OrderByDescending(v => v.Code).FirstOrDefaultAsync())?.Code;
                    break;
                case "KK":
                    latestCode = (await _context.CountingSheets.Where(v => v.Code.StartsWith(prefixWithDate)).OrderByDescending(v => v.Code).FirstOrDefaultAsync())?.Code;
                    break;
            }

            if (!string.IsNullOrEmpty(latestCode))
            {
                var parts = latestCode.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                {
                    sequence = lastSeq + 1;
                }
            }

            return $"{prefixWithDate}{sequence:D3}";
        }
    }
}
