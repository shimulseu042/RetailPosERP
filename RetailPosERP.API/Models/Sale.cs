using RetailPosERP.API.Enums;

namespace RetailPosERP.API.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string UniqueSaleId { get; set; } = string.Empty;

        public string StoreCode { get; set; } = "STORE-01";
        public decimal TotalAmount { get; set; }
        public SaleStatus Status { get; set; } = SaleStatus.Pending;
        public DateTime SaleTimestamp { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SyncedAt { get; set; }
        public int SyncAttempts { get; set; } = 0;
        public string? LastSyncError { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
