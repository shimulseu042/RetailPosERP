using RetailPosERP.API.Models;

namespace RetailPosERP.API.Repositories.Interfaces
{
    public interface ISyncRepository
    {
        Task<(Sale sale, bool isNew)> UpsertSaleAsync(Sale sale, IEnumerable<SaleItem> items);
    }
}
