using System.Threading.Tasks;

namespace QLKHO_PhanVanHoang.Services
{
    public interface IInventoryService
    {
        Task IncreaseInventoryAsync(int productId, int warehouseId, string? lotNumber, decimal quantity, decimal costPrice, string referenceCode);
        Task DecreaseInventoryAsync(int productId, int warehouseId, string? lotNumber, decimal quantity, string referenceCode);
    }
}
