using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.Services.Interfaces;

namespace RetailPosERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ISyncService _syncService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(ISyncService syncService, ILogger<SyncController> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Main sync endpoint – receives batch of POS sales and saves to central DB.
        /// Idempotent: duplicate UniqueSaleId records are silently skipped.
        /// </summary>
        [HttpPost("sync-sales")]
        public async Task<IActionResult> SyncSales([FromBody] SyncSalesRequest request)
        {
            if (request.Sales == null || request.Sales.Count == 0)
                return BadRequest(new { message = "No sales provided for sync." });

            _logger.LogInformation("Sync request received: {Count} sales", request.Sales.Count);

            var result = await _syncService.SyncSalesAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Marks all local Pending/Failed sales as Synced (simulates sync with retry logic).
        /// </summary>
        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerSync()
        {
            var result = await _syncService.TriggerLocalSyncAsync();
            return Ok(result);
        }
    }
}
