using System;
using System.Threading.Tasks;
using QLKHO_PhanVanHoang.Models;

namespace QLKHO_PhanVanHoang.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Warehouse> Warehouses { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Inventory> Inventories { get; }
        IGenericRepository<StockCard> StockCards { get; }
        
        IGenericRepository<ReceivingVoucher> ReceivingVouchers { get; }
        IGenericRepository<ReceivingVoucherDetail> ReceivingVoucherDetails { get; }
        IGenericRepository<DeliveryVoucher> DeliveryVouchers { get; }
        IGenericRepository<DeliveryVoucherDetail> DeliveryVoucherDetails { get; }
        IGenericRepository<TransferVoucher> TransferVouchers { get; }
        IGenericRepository<TransferVoucherDetail> TransferVoucherDetails { get; }
        
        IGenericRepository<CountingSheet> CountingSheets { get; }
        IGenericRepository<CountingSheetDetail> CountingSheetDetails { get; }
        IGenericRepository<InventoryAdjustment> InventoryAdjustments { get; }
        
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<SystemUser> SystemUsers { get; }
        IGenericRepository<AuditLog> AuditLogs { get; }

        Task<int> CompleteAsync();
        
        // Transaction management
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
