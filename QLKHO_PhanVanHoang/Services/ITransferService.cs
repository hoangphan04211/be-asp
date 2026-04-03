using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface ITransferService
    {
        Task CreateTransferVoucherAsync(int fromWarehouseId, int toWarehouseId, string code, string? notes);
        Task ApproveTransferVoucherAsync(int voucherId);
    }
}
