namespace RetailPosERP.API.DTOs.Response
{
    public class SaleResponse
    {
        public int Id { get; set; }
        public string UniqueSaleId { get; set; } = string.Empty;
        public string StoreCode { get; set; } = string.Empty;
        public DateTime SaleTimestamp { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? SyncedAt { get; set; }
        public List<SaleItemResponse> Items { get; set; } = new();
    }

    public class SaleItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
