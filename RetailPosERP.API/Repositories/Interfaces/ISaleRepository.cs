using RetailPosERP.API.Models;

namespace RetailPosERP.API.Repositories.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task<Sale?> GetByUniqueSaleIdAsync(string uniqueSaleId);
        Task<IEnumerable<Sale>> GetUnsyncedAsync();
        Task<Sale> CreateAsync(Sale sale);
        Task<Sale?> MarkAsSyncedAsync(int saleId);
        Task<Sale?> MarkAsFailedAsync(int saleId, string error);
        Task<bool> ExistsByUniqueSaleIdAsync(string uniqueSaleId);
    }
}
