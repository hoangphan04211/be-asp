using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IInboundService
    {
        Task ApproveReceivingVoucherAsync(int voucherId);
    }
}
