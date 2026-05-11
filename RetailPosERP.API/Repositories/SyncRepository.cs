using Microsoft.EntityFrameworkCore;
using RetailPosERP.API.Data;
using RetailPosERP.API.Enums;
using RetailPosERP.API.Models;
using RetailPosERP.API.Repositories.Interfaces;

namespace RetailPosERP.API.Repositories
{
    public class SyncRepository : ISyncRepository
    {
        private readonly AppDbContext _db;

        public SyncRepository(AppDbContext db) => _db = db;

        public async Task<(Sale sale, bool isNew)> UpsertSaleAsync(Sale sale, IEnumerable<SaleItem> items)
        {
            var existing = await _db.Sales
                .FirstOrDefaultAsync(s => s.UniqueSaleId == sale.UniqueSaleId);

            if (existing != null)
                return (existing, false);

            sale.Status = SaleStatus.Synced;
            sale.SyncedAt = DateTime.UtcNow;
            _db.Sales.Add(sale);
            await _db.SaveChangesAsync();

            foreach (var item in items)
            {
                item.SaleId = sale.Id;
                _db.SaleItems.Add(item);

                var product = await _db.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.StockQuantity -= item.Quantity;
                    if (product.StockQuantity < 0) product.StockQuantity = 0;
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();
            return (sale, true);
        }
    }
}
