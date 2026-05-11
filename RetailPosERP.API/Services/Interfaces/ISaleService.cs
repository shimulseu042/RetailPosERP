using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.DTOs.Response;

namespace RetailPosERP.API.Services.Interfaces
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleResponse>> GetAllSalesAsync();
        Task<SaleResponse?> GetSaleByIdAsync(int id);
        Task<IEnumerable<SaleResponse>> GetUnsyncedSalesAsync();
        Task<SaleResponse> CreateSaleAsync(SaleRequest request);
    }
}
