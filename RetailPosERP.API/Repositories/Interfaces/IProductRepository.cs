using RetailPosERP.API.Models;

namespace RetailPosERP.API.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product?> GetByBarcodeAsync(string barcode);
        Task<Product> CreateAsync(Product product);
        Task<Product?> UpdateStockAsync(int productId, int quantity);
        Task<bool> ExistsAsync(int id);
    }
}
