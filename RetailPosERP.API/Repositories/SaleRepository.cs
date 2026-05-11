using Microsoft.EntityFrameworkCore;
using RetailPosERP.API.Data;
using RetailPosERP.API.Enums;
using RetailPosERP.API.Models;
using RetailPosERP.API.Repositories.Interfaces;

namespace RetailPosERP.API.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _db;

        public SaleRepository(AppDbContext db) => _db = db;

        public async Task<Sale> CreateAsync(Sale sale)
        {
            if (string.IsNullOrEmpty(sale.UniqueSaleId))
                sale.UniqueSaleId = $"SALE-{sale.StoreCode}-{Guid.NewGuid():N}".ToUpper();

            _db.Sales.Add(sale);
            await _db.SaveChangesAsync();
            return sale;
        }

        public async Task<bool> ExistsByUniqueSaleIdAsync(string uniqueSaleId)
        {
            return await _db.Sales.AnyAsync(s => s.UniqueSaleId == uniqueSaleId);
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            return await _db.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .OrderByDescending(s => s.SaleTimestamp)
            .ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _db.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Sale?> GetByUniqueSaleIdAsync(string uniqueSaleId)
        {
            return await _db.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.UniqueSaleId == uniqueSaleId);
        }

        public async Task<IEnumerable<Sale>> GetUnsyncedAsync()
        {
            return await _db.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Product)
            .Where(s => s.Status == SaleStatus.Pending || s.Status == SaleStatus.Failed)
            .ToListAsync();
        }

        public async Task<Sale?> MarkAsFailedAsync(int saleId, string error)
        {
            var sale = await _db.Sales.FindAsync(saleId);
            if (sale == null) return null;

            sale.Status = SaleStatus.Failed;
            sale.LastSyncError = error;
            sale.SyncAttempts++;
            await _db.SaveChangesAsync();
            return sale;
        }

        public async Task<Sale?> MarkAsSyncedAsync(int saleId)
        {
            var sale = await _db.Sales.FindAsync(saleId);
            if (sale == null) return null;

            sale.Status = SaleStatus.Synced;
            sale.SyncedAt = DateTime.UtcNow;
            sale.SyncAttempts++;
            await _db.SaveChangesAsync();
            return sale;
        }
    }
}
