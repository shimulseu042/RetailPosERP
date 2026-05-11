using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.DTOs.Response;

namespace RetailPosERP.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync();
        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse?> GetProductByBarcodeAsync(string barcode);
        Task<ProductResponse> CreateProductAsync(ProductRequest request);
    }
}
