using Microsoft.EntityFrameworkCore;
using RetailPosERP.API.Data;
using RetailPosERP.API.Models;
using RetailPosERP.API.Repositories.Interfaces;

namespace RetailPosERP.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db) => _db = db;

        public async Task<IEnumerable<Product>> GetAllAsync() {
            return await _db.Products.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id) { 
            return await _db.Products.FindAsync(id);
        }

        public async Task<Product?> GetByBarcodeAsync(string barcode) { 
            return await _db.Products.FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.ProductCode))
            {
                var count = await _db.Products.CountAsync();
                product.ProductCode = $"PRD-{(count + 1):D4}";
            }
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateStockAsync(int productId, int quantity)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return null;

            product.StockQuantity += quantity;   // negative delta for sale
            if (product.StockQuantity < 0)
                throw new InvalidOperationException($"Insufficient stock for product {product.Name}. Available: {product.StockQuantity - quantity}");

            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return product;
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _db.Products.AnyAsync(p => p.Id == id);
    }
}
