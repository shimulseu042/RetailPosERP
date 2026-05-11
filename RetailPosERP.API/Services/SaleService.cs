using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.DTOs.Response;
using RetailPosERP.API.Enums;
using RetailPosERP.API.Models;
using RetailPosERP.API.Repositories.Interfaces;
using RetailPosERP.API.Services.Interfaces;

namespace RetailPosERP.API.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepo;
        private readonly IProductRepository _productRepo;
        private readonly ILogger<SaleService> _logger;

        public SaleService(
            ISaleRepository saleRepo,
            IProductRepository productRepo,
            ILogger<SaleService> logger)
        {
            _saleRepo = saleRepo;
            _productRepo = productRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<SaleResponse>> GetAllSalesAsync()
        {
            var sales = await _saleRepo.GetAllAsync();
            return sales.Select(MapToResponse);
        }

        public async Task<SaleResponse?> GetSaleByIdAsync(int id)
        {
            var sale = await _saleRepo.GetByIdAsync(id);
            return sale is null ? null : MapToResponse(sale);
        }

        public async Task<IEnumerable<SaleResponse>> GetUnsyncedSalesAsync()
        {
            var sales = await _saleRepo.GetUnsyncedAsync();
            return sales.Select(MapToResponse);
        }

        public async Task<SaleResponse> CreateSaleAsync(SaleRequest request)
        {
            if (request.Items == null || request.Items.Count == 0)
                throw new ArgumentException("Sale must have at least one item.");

            if (!string.IsNullOrEmpty(request.UniqueSaleId))
            {
                var existing = await _saleRepo.GetByUniqueSaleIdAsync(request.UniqueSaleId);
                if (existing != null)
                {
                    _logger.LogWarning("Duplicate sale detected: {UniqueSaleId}", request.UniqueSaleId);
                    return MapToResponse(existing);
                }
            }

            decimal total = 0;
            var saleItems = new List<SaleItem>();

            foreach (var itemReq in request.Items)
            {
                var product = await _productRepo.GetByIdAsync(itemReq.ProductId)
                              ?? throw new KeyNotFoundException($"Product {itemReq.ProductId} not found.");

                if (product.StockQuantity < itemReq.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}");

                saleItems.Add(new SaleItem
                {
                    ProductId = itemReq.ProductId,
                    Quantity = itemReq.Quantity,
                    UnitPrice = itemReq.UnitPrice > 0 ? itemReq.UnitPrice : product.Price
                });

                total += saleItems.Last().Quantity * saleItems.Last().UnitPrice;
            }

            foreach (var item in saleItems)
                await _productRepo.UpdateStockAsync(item.ProductId, -item.Quantity);

            var sale = new Sale
            {
                UniqueSaleId = request.UniqueSaleId ?? string.Empty,
                StoreCode = request.StoreCode,
                TotalAmount = total,
                SaleTimestamp = request.SaleTimestamp,
                Status = SaleStatus.Pending,
                SaleItems = saleItems
            };

            var created = await _saleRepo.CreateAsync(sale);
            _logger.LogInformation("Sale created: {UniqueSaleId} | Total: {Total}", created.UniqueSaleId, total);
            return MapToResponse(created);
        }

        private static SaleResponse MapToResponse(Sale s) => new()
        {
            Id = s.Id,
            UniqueSaleId = s.UniqueSaleId,
            StoreCode = s.StoreCode,
            TotalAmount = s.TotalAmount,
            Status = s.Status.ToString(),
            SaleTimestamp = s.SaleTimestamp,
            SyncedAt = s.SyncedAt,
            Items = s.SaleItems.Select(si => new SaleItemResponse
            {
                ProductId = si.ProductId,
                ProductName = si.Product?.Name ?? string.Empty,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                LineTotal = si.LineTotal
            }).ToList()
        };
    }
}
