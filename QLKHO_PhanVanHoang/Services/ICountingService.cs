using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface ICountingService
    {
        Task ApproveCountingSheetAsync(int countingSheetId);
    }
}
