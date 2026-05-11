using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.DTOs.Response;

namespace RetailPosERP.API.Services.Interfaces
{
    public interface ISyncService
    {
        Task<SyncResultResponse> SyncSalesAsync(SyncSalesRequest request);
        Task<SyncResultResponse> TriggerLocalSyncAsync();
    }
}
