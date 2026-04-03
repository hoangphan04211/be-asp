using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using QLKHO_PhanVanHoang.Data;
using QLKHO_PhanVanHoang.Models;

namespace QLKHO_PhanVanHoang.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _currentTransaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            
            Products = new GenericRepository<Product>(_context);
            Categories = new GenericRepository<Category>(_context);
            Warehouses = new GenericRepository<Warehouse>(_context);
            Suppliers = new GenericRepository<Supplier>(_context);
            Customers = new GenericRepository<Customer>(_context);
            Inventories = new GenericRepository<Inventory>(_context);
            StockCards = new GenericRepository<StockCard>(_context);
            
            ReceivingVouchers = new GenericRepository<ReceivingVoucher>(_context);
            ReceivingVoucherDetails = new GenericRepository<ReceivingVoucherDetail>(_context);
            DeliveryVouchers = new GenericRepository<DeliveryVoucher>(_context);
            DeliveryVoucherDetails = new GenericRepository<DeliveryVoucherDetail>(_context);
            TransferVouchers = new GenericRepository<TransferVoucher>(_context);
            TransferVoucherDetails = new GenericRepository<TransferVoucherDetail>(_context);
            
            CountingSheets = new GenericRepository<CountingSheet>(_context);
            CountingSheetDetails = new GenericRepository<CountingSheetDetail>(_context);
            InventoryAdjustments = new GenericRepository<InventoryAdjustment>(_context);
            
            Roles = new GenericRepository<Role>(_context);
            SystemUsers = new GenericRepository<SystemUser>(_context);
            AuditLogs = new GenericRepository<AuditLog>(_context);
        }

        public IGenericRepository<Product> Products { get; private set; }
        public IGenericRepository<Category> Categories { get; private set; }
        public IGenericRepository<Warehouse> Warehouses { get; private set; }
        public IGenericRepository<Supplier> Suppliers { get; private set; }
        public IGenericRepository<Customer> Customers { get; private set; }
        public IGenericRepository<Inventory> Inventories { get; private set; }
        public IGenericRepository<StockCard> StockCards { get; private set; }
        
        public IGenericRepository<ReceivingVoucher> ReceivingVouchers { get; private set; }
        public IGenericRepository<ReceivingVoucherDetail> ReceivingVoucherDetails { get; private set; }
        public IGenericRepository<DeliveryVoucher> DeliveryVouchers { get; private set; }
        public IGenericRepository<DeliveryVoucherDetail> DeliveryVoucherDetails { get; private set; }
        public IGenericRepository<TransferVoucher> TransferVouchers { get; private set; }
        public IGenericRepository<TransferVoucherDetail> TransferVoucherDetails { get; private set; }
        
        public IGenericRepository<CountingSheet> CountingSheets { get; private set; }
        public IGenericRepository<CountingSheetDetail> CountingSheetDetails { get; private set; }
        public IGenericRepository<InventoryAdjustment> InventoryAdjustments { get; private set; }
        
        public IGenericRepository<Role> Roles { get; private set; }
        public IGenericRepository<SystemUser> SystemUsers { get; private set; }
        public IGenericRepository<AuditLog> AuditLogs { get; private set; }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync();
                }
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
