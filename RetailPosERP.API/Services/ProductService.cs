using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.DTOs.Response;
using RetailPosERP.API.Models;
using RetailPosERP.API.Repositories.Interfaces;
using RetailPosERP.API.Services.Interfaces;
using System.Collections.Concurrent;

namespace RetailPosERP.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepo, ILogger<ProductService> logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync()
        {
            var products = await _productRepo.GetAllAsync();
            return products.Select(MapToResponse);
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            return product is null ? null : MapToResponse(product);
        }

        public async Task<ProductResponse?> GetProductByBarcodeAsync(string barcode)
        {
            var product = await _productRepo.GetByBarcodeAsync(barcode);
            return product is null ? null : MapToResponse(product);
        }

        public async Task<ProductResponse> CreateProductAsync(ProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Product name is required.");

            if (request.Price <= 0)
                throw new ArgumentException("Price must be greater than zero.");

            var product = new Product
            {
                Name = request.Name.Trim(),
                Barcode = request.Barcode.Trim(),
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _productRepo.CreateAsync(product);
            _logger.LogInformation("Product created: {ProductCode} - {Name}", created.ProductCode, created.Name);
            return MapToResponse(created);
        }

        private static ProductResponse MapToResponse(Product p) => new()
        {
            Id = p.Id,
            ProductCode = p.ProductCode,
            Name = p.Name,
            Barcode = p.Barcode,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive,
            CreatedAt = p.CreatedAt
        };
    }
}
