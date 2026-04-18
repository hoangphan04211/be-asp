using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using QLKHO_PhanVanHoang.Models;
using QLKHO_PhanVanHoang.Models.Common;

namespace QLKHO_PhanVanHoang.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Warehouse> Warehouses { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Inventory> Inventories { get; set; } = null!;
        public DbSet<ReceivingVoucher> ReceivingVouchers { get; set; } = null!;
        public DbSet<ReceivingVoucherDetail> ReceivingVoucherDetails { get; set; } = null!;
        public DbSet<CountingSheet> CountingSheets { get; set; } = null!;
        public DbSet<CountingSheetDetail> CountingSheetDetails { get; set; } = null!;
        public DbSet<InventoryAdjustment> InventoryAdjustments { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<DeliveryVoucher> DeliveryVouchers { get; set; } = null!;
        public DbSet<DeliveryVoucherDetail> DeliveryVoucherDetails { get; set; } = null!;
        public DbSet<TransferVoucher> TransferVouchers { get; set; } = null!;
        public DbSet<TransferVoucherDetail> TransferVoucherDetails { get; set; } = null!;
        public DbSet<StockCard> StockCards { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<SystemUser> SystemUsers { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<Permission> Permissions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Manual configuration for indexes
            modelBuilder.Entity<Product>().HasIndex(p => p.SkuCode).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Warehouse>().HasIndex(w => w.Name).IsUnique();
            modelBuilder.Entity<Supplier>().HasIndex(s => s.Code).IsUnique();
            modelBuilder.Entity<Inventory>().HasIndex(i => new { i.ProductId, i.WarehouseId, i.LotNumber }).IsUnique();
            modelBuilder.Entity<ReceivingVoucher>().HasIndex(r => r.Code).IsUnique();
            modelBuilder.Entity<CountingSheet>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<Customer>().HasIndex(c => c.Code).IsUnique();
            modelBuilder.Entity<DeliveryVoucher>().HasIndex(d => d.Code).IsUnique();
            modelBuilder.Entity<TransferVoucher>().HasIndex(t => t.Code).IsUnique();
            modelBuilder.Entity<SystemUser>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
            modelBuilder.Entity<Permission>().HasIndex(p => p.Code).IsUnique();

            // Many-to-Many Permission-Role
            modelBuilder.Entity<Role>()
                .HasMany(p => p.Permissions)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermissions",
                    j => j.HasOne<Permission>().WithMany().HasForeignKey("PermissionId"),
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId");
                        j.HasData(
                            // Gán toàn bộ quyền cho Admin (Id=1)
                            new { RoleId = 1, PermissionId = 1 },
                            new { RoleId = 1, PermissionId = 2 },
                            new { RoleId = 1, PermissionId = 3 },
                            new { RoleId = 1, PermissionId = 4 },
                            new { RoleId = 1, PermissionId = 5 },
                            new { RoleId = 1, PermissionId = 6 },
                            new { RoleId = 1, PermissionId = 7 },
                            new { RoleId = 1, PermissionId = 8 },
                            new { RoleId = 1, PermissionId = 9 },
                            new { RoleId = 1, PermissionId = 10 },
                            new { RoleId = 1, PermissionId = 11 },
                            new { RoleId = 1, PermissionId = 12 },
                            new { RoleId = 1, PermissionId = 13 },
                            new { RoleId = 1, PermissionId = 14 },
                            new { RoleId = 1, PermissionId = 15 },
                            new { RoleId = 1, PermissionId = 16 },
                            new { RoleId = 1, PermissionId = 17 },
                            new { RoleId = 1, PermissionId = 18 },
                            new { RoleId = 1, PermissionId = 19 },
                            new { RoleId = 1, PermissionId = 20 }
                        );
                    });

            // Seed Permissions
            modelBuilder.Entity<Permission>().HasData(
                // Master Data (1-5)
                new Permission { Id = 1, Code = "PRODUCT_VIEW", Name = "Xem sản phẩm", Group = "Sản phẩm", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 2, Code = "PRODUCT_EDIT", Name = "Sửa/Xóa sản phẩm", Group = "Sản phẩm", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 3, Code = "MASTER_DATA_VIEW", Name = "Xem danh mục chung", Group = "Danh mục", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 4, Code = "MASTER_DATA_EDIT", Name = "Quản lý danh mục chung", Group = "Danh mục", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 5, Code = "WAREHOUSE_VIEW", Name = "Xem kho hàng", Group = "Danh mục", CreatedBy = "system", CreatedAt = DateTime.Now },
                
                // Operations (6-15)
                new Permission { Id = 6, Code = "INBOUND_VIEW", Name = "Xem phiếu nhập", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 7, Code = "INBOUND_CREATE", Name = "Tạo phiếu nhập", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 8, Code = "INBOUND_APPROVE", Name = "Duyệt phiếu nhập", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 9, Code = "OUTBOUND_VIEW", Name = "Xem phiếu xuất", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 10, Code = "OUTBOUND_CREATE", Name = "Tạo phiếu xuất", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 11, Code = "OUTBOUND_APPROVE", Name = "Duyệt phiếu xuất", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 12, Code = "TRANSFER_VIEW", Name = "Xem điều chuyển", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 13, Code = "TRANSFER_CREATE", Name = "Tạo điều chuyển", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 14, Code = "COUNTING_VIEW", Name = "Xem kiểm kê", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 15, Code = "COUNTING_APPROVE", Name = "Duyệt kiểm kê", Group = "Vận hành", CreatedBy = "system", CreatedAt = DateTime.Now },

                // Reports (16-17)
                new Permission { Id = 16, Code = "REPORT_VIEW", Name = "Xem báo cáo tổn kho", Group = "Báo cáo", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 17, Code = "STOCK_CARD_VIEW", Name = "Xem thẻ kho", Group = "Báo cáo", CreatedBy = "system", CreatedAt = DateTime.Now },

                // System (18-20)
                new Permission { Id = 18, Code = "USER_MANAGEMENT", Name = "Quản lý nhân viên", Group = "Hệ thống", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 19, Code = "SYSTEM_LOGS", Name = "Xem nhật ký hệ thống", Group = "Hệ thống", CreatedBy = "system", CreatedAt = DateTime.Now },
                new Permission { Id = 20, Code = "SYSTEM_TRASH", Name = "Quản lý thùng rác", Group = "Hệ thống", CreatedBy = "system", CreatedAt = DateTime.Now }
            );

            // Automatic Global Query Filter (Soft Delete) & RowVersion
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    // Global Soft Delete Filter
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(CreateSoftDeleteFilter(entityType.ClrType));
                    
                    // RowVersion
                    modelBuilder.Entity(entityType.ClrType).Property("RowVersion").IsRowVersion();
                }
            }

            // Relationships (keep existing)
            modelBuilder.Entity<Product>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Inventory>().HasOne(i => i.Product).WithMany(p => p.Inventories).HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Inventory>().HasOne(i => i.Warehouse).WithMany(w => w.Inventories).HasForeignKey(i => i.WarehouseId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ReceivingVoucherDetail>().HasOne(d => d.ReceivingVoucher).WithMany(r => r.Details).HasForeignKey(d => d.ReceivingVoucherId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<CountingSheetDetail>().HasOne(d => d.CountingSheet).WithMany(c => c.Details).HasForeignKey(d => d.CountingSheetId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<DeliveryVoucherDetail>().HasOne(d => d.DeliveryVoucher).WithMany(v => v.Details).HasForeignKey(d => d.DeliveryVoucherId).OnDelete(DeleteBehavior.Cascade);
            
            // Fix circular cascade for TransferVouchers
            modelBuilder.Entity<TransferVoucher>().HasOne(t => t.FromWarehouse).WithMany().HasForeignKey(t => t.FromWarehouseId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<TransferVoucher>().HasOne(t => t.ToWarehouse).WithMany().HasForeignKey(t => t.ToWarehouseId).OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<TransferVoucherDetail>().HasOne(d => d.TransferVoucher).WithMany(t => t.Details).HasForeignKey(d => d.TransferVoucherId).OnDelete(DeleteBehavior.Cascade);

            // Seed Rules (Mã nguồn đã có sẵn)
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "Toàn quyền hệ thống", CreatedAt = new DateTime(2026, 4, 1), CreatedBy = "system", IsDeleted = false },
                new Role { Id = 2, Name = "WarehouseManager", Description = "Quản lý kho và duyệt phiếu", CreatedAt = new DateTime(2026, 4, 1), CreatedBy = "system", IsDeleted = false },
                new Role { Id = 3, Name = "Employee", Description = "Nhân viên kho thực thi", CreatedAt = new DateTime(2026, 4, 1), CreatedBy = "system", IsDeleted = false }
            );
        }

        private static System.Linq.Expressions.LambdaExpression CreateSoftDeleteFilter(Type type)
        {
            var parameter = System.Linq.Expressions.Expression.Parameter(type, "e");
            var property = System.Linq.Expressions.Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var notDeleted = System.Linq.Expressions.Expression.Not(property);
            return System.Linq.Expressions.Expression.Lambda(notDeleted, parameter);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "system";
            var auditEntries = OnBeforeSaveChanges(currentUser);
            
            var result = await base.SaveChangesAsync(cancellationToken);
            
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges(string userId)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    EntityName = entry.Entity.GetType().Name,
                    ChangedBy = userId
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue!;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = "Create";
                            if (property.CurrentValue != null) auditEntry.NewValues[propertyName] = property.CurrentValue;
                            entry.Entity.CreatedAt = DateTime.Now;
                            entry.Entity.CreatedBy = userId;
                            break;

                        case EntityState.Deleted:
                            if (entry.Entity.IsDeleted)
                            {
                                // If already soft-deleted, allow hard delete
                                auditEntry.AuditType = "HardDelete";
                            }
                            else
                            {
                                // Convert to Soft Delete
                                entry.State = EntityState.Modified;
                                entry.Entity.IsDeleted = true;
                                entry.Entity.UpdatedAt = DateTime.Now;
                                entry.Entity.UpdatedBy = userId;
                                auditEntry.AuditType = "Delete";
                            }
                            if (property.OriginalValue != null) auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.AuditType = "Update";
                                auditEntry.OldValues[propertyName] = property.OriginalValue!;
                                auditEntry.NewValues[propertyName] = property.CurrentValue!;
                                entry.Entity.UpdatedAt = DateTime.Now;
                                entry.Entity.UpdatedBy = userId;
                            }
                            break;
                    }
                }
            }
            return auditEntries;
        }

        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
            return base.SaveChangesAsync();
        }
    }

    // Helper class for Audit Entries
    internal class AuditEntry
    {
        public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry) => Entry = entry;
        public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
        public string EntityName { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public string AuditType { get; set; } = string.Empty;
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();

        public AuditLog ToAudit()
        {
            var audit = new AuditLog
            {
                EntityName = EntityName,
                EntityId = JsonSerializer.Serialize(KeyValues),
                Action = AuditType,
                OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
                NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
                ChangedBy = ChangedBy,
                ChangedAt = DateTime.Now
            };
            return audit;
        }
    }
}