using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RetailPosERP.API.DTOs.Request;
using RetailPosERP.API.Services.Interfaces;

namespace RetailPosERP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
            => _saleService = saleService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var sales = await _saleService.GetAllSalesAsync();
            return Ok(sales);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sale = await _saleService.GetSaleByIdAsync(id);
            return sale is null ? NotFound(new { message = $"Sale {id} not found." }) : Ok(sale);
        }

        [HttpGet("unsynced")]
        public async Task<IActionResult> GetUnsynced()
        {
            var sales = await _saleService.GetUnsyncedSalesAsync();
            return Ok(sales);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleRequest request)
        {
            var sale = await _saleService.CreateSaleAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = sale.Id }, sale);
        }
    }
}
