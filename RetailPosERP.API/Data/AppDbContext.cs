using Microsoft.EntityFrameworkCore;
using RetailPosERP.API.Models;

namespace RetailPosERP.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Sale> Sales => Set<Sale>();
        public DbSet<SaleItem> SaleItems => Set<SaleItem>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(e =>
            {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name).IsRequired().HasMaxLength(200);
                e.Property(p => p.Barcode).HasMaxLength(100);
                e.Property(p => p.ProductCode).HasMaxLength(50);
                e.Property(p => p.Price).HasColumnType("decimal(18,2)");
                e.HasIndex(p => p.Barcode);
                e.HasIndex(p => p.ProductCode).IsUnique();
            });

            modelBuilder.Entity<Sale>(e =>
            {
                e.HasKey(s => s.Id);
                // idempotency key
                e.HasIndex(s => s.UniqueSaleId).IsUnique();
                e.Property(s => s.UniqueSaleId).IsRequired().HasMaxLength(100);
                e.Property(s => s.StoreCode).HasMaxLength(50);
                e.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
                e.Property(s => s.Status).HasConversion<string>();   // store as "Pending"/"Synced"
            });

            modelBuilder.Entity<SaleItem>(e =>
            {
                e.HasKey(si => si.Id);
                e.Property(si => si.UnitPrice).HasColumnType("decimal(18,2)");
                e.Ignore(si => si.LineTotal);

                e.HasOne(si => si.Sale)
                 .WithMany(s => s.SaleItems)
                 .HasForeignKey(si => si.SaleId);

                e.HasOne(si => si.Product)
                 .WithMany(p => p.SaleItems)
                 .HasForeignKey(si => si.ProductId);
            });

            modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, ProductCode = "PRD-001", Name = "T-Shirt (M)", Barcode = "8901234567890", Price = 499.00m, StockQuantity = 80 },
            new Product { Id = 2, ProductCode = "PRD-002", Name = "Jeans (32)", Barcode = "8901234567891", Price = 1299.00m, StockQuantity = 50 },
            new Product { Id = 3, ProductCode = "PRD-003", Name = "Polo Shirt (L)", Barcode = "8901234567892", Price = 799.00m, StockQuantity = 70 },
            new Product { Id = 4, ProductCode = "PRD-004", Name = "Jacket (XL)", Barcode = "8901234567893", Price = 2499.00m, StockQuantity = 20 }
        );
        }
    }
}
