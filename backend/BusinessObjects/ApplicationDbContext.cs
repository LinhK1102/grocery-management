using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.FileExtensions;
//using Microsoft.Extensions.Configuration.Json;
using System.IO;
using Microsoft.Extensions.Configuration; // chứa SetBasePath

namespace BusinessObjects
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<RetailOutlet> RetailOutlets { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        // Nếu bạn cần thiết lập quan hệ nâng cao, override OnModelCreating ở đây
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Khóa chính cho OrderDetail
            modelBuilder.Entity<OrderDetail>()
                        .HasKey(od => od.OrderDetailId);

            // Thiết lập quan hệ cho OrderDetail
            modelBuilder.Entity<OrderDetail>()
                        .HasOne(od => od.Order)
                        .WithMany(o => o.OrderDetails)
                        .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                        .HasOne(od => od.Product)
                        .WithMany(p => p.OrderDetails)
                        .HasForeignKey(od => od.ProductId);

            // Thiết lập quan hệ nullable cho Order
            modelBuilder.Entity<Order>()
                        .HasOne(o => o.RetailOutlet)
                        .WithMany(ro => ro.Orders)
                        .HasForeignKey(o => o.OutletId)
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                        .HasOne(o => o.Warehouse)
                        .WithMany(w => w.Orders)
                        .HasForeignKey(o => o.WarehouseId)
                        .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
