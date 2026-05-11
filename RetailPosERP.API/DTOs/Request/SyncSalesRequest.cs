namespace RetailPosERP.API.DTOs.Request
{
    public class SyncSalesRequest
    {
        public List<SyncSaleItem> Sales { get; set; } = new();
    }

    public class SyncSaleItem
    {
        public string? UniqueSaleId { get; set; }
        public string StoreCode { get; set; } = string.Empty;
        public DateTime SaleTimestamp { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public List<SyncSaleItemDetail> Items { get; set; } = new();
    }

    public class SyncSaleItemDetail
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
