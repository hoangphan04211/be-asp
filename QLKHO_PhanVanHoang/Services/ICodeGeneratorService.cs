using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateProductCodeAsync();
        Task<string> GenerateSupplierCodeAsync();
        Task<string> GenerateCustomerCodeAsync();
        Task<string> GenerateReceivingCodeAsync();
        Task<string> GenerateDeliveryCodeAsync();
        Task<string> GenerateTransferCodeAsync();
        Task<string> GenerateCountingCodeAsync();
    }
}
