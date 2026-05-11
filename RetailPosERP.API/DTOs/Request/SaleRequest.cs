namespace RetailPosERP.API.DTOs.Request
{
    public class SaleRequest
    {
        public string? UniqueSaleId { get; set; }
        public string StoreCode { get; set; } = "STORE-01";
        public DateTime SaleTimestamp { get; set; } = DateTime.UtcNow;
        public List<SaleItemRequest> Items { get; set; } = new();
    }

    public class SaleItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
