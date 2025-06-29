using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration.FileExtensions;
//using Microsoft.Extensions.Configuration.Json;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BusinessObjects.Entities
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
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
        public DbSet<Item> Items { get; set; }
        public DbSet<InvoiceItem> InvoiceItem { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<ProductWarehouse> ProductWarehouse { get; set; }


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

            // Thiết lập quan hệ cho ProductWarehouse
            modelBuilder.Entity<ProductWarehouse>()
      .HasKey(pw => new { pw.ProductId, pw.WarehouseId });

            modelBuilder.Entity<ProductWarehouse>()
                .HasOne(pw => pw.Product)
                .WithMany(p => p.ProductWarehouses)
                .HasForeignKey(pw => pw.ProductId);

            modelBuilder.Entity<ProductWarehouse>()
                .HasOne(pw => pw.Warehouse)
                .WithMany(w => w.ProductWarehouses)
                .HasForeignKey(pw => pw.WarehouseId);
        }
    }
}
