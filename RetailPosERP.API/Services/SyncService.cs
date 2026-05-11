using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.DTOs.Response;
using RetailPosERP.API.Models;
using RetailPosERP.API.Repositories.Interfaces;
using RetailPosERP.API.Services.Interfaces;

namespace RetailPosERP.API.Services
{
    public class SyncService : ISyncService
    {
        private readonly ISyncRepository _syncRepo;
        private readonly ISaleRepository _saleRepo;
        private readonly ILogger<SyncService> _logger;

        public SyncService(
            ISyncRepository syncRepo,
            ISaleRepository saleRepo,
            ILogger<SyncService> logger)
        {
            _syncRepo = syncRepo;
            _saleRepo = saleRepo;
            _logger = logger;
        }
        public async Task<SyncResultResponse> SyncSalesAsync(SyncSalesRequest request)
        {
            var result = new SyncResultResponse
            {
                TotalReceived = request.Sales.Count
            };

            foreach (var saleDto in request.Sales)
            {
                try
                {
                    if (string.IsNullOrEmpty(saleDto.UniqueSaleId))
                    {
                        result.Failed++;
                        result.Errors.Add("Sale skipped: missing UniqueSaleId");
                        continue;
                    }

                    var sale = new Sale
                    {
                        UniqueSaleId = saleDto.UniqueSaleId,
                        StoreCode = saleDto.StoreCode,
                        TotalAmount = saleDto.TotalAmount,
                        SaleTimestamp = saleDto.SaleTimestamp,
                    };

                    var items = saleDto.Items.Select(i => new SaleItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList();

                    var (_, isNew) = await _syncRepo.UpsertSaleAsync(sale, items);

                    if (isNew)
                        result.NewlySynced++;
                    else
                        result.Duplicates++;

                    _logger.LogInformation(
                        "Sync: {UniqueSaleId} | New={IsNew}", saleDto.UniqueSaleId, isNew);
                }
                catch (Exception ex)
                {
                    result.Failed++;
                    result.Errors.Add($"{saleDto.UniqueSaleId}: {ex.Message}");
                    _logger.LogError(ex, "Sync failed for sale {UniqueSaleId}", saleDto.UniqueSaleId);
                }
            }

            return result;
        }
        public async Task<SyncResultResponse> TriggerLocalSyncAsync()
        {
            var unsynced = (await _saleRepo.GetUnsyncedAsync()).ToList();
            var result = new SyncResultResponse { TotalReceived = unsynced.Count };

            foreach (var sale in unsynced)
            {
                try
                {
                    await _saleRepo.MarkAsSyncedAsync(sale.Id);
                    result.NewlySynced++;
                }
                catch (Exception ex)
                {
                    await _saleRepo.MarkAsFailedAsync(sale.Id, ex.Message);
                    result.Failed++;
                    result.Errors.Add($"{sale.UniqueSaleId}: {ex.Message}");
                }
            }

            return result;
        }
    }
}
