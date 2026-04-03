using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IOutboundService
    {
        Task ApproveDeliveryVoucherAsync(int voucherId);
    }
}
