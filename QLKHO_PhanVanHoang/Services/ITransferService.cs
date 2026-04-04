using System.Threading.Tasks;
using QLKHO_PhanVanHoang.DTOs;

namespace QLKHO_PhanVanHoang.Services
{
    public interface ITransferService
    {
        Task CreateTransferVoucherAsync(CreateTransferDto dto);
        Task ApproveTransferVoucherAsync(int voucherId);
    }
}
