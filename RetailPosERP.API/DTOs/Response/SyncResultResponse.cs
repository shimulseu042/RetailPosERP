namespace RetailPosERP.API.DTOs.Response
{
    public class SyncResultResponse
    {
        public int TotalReceived { get; set; }
        public int NewlySynced { get; set; }
        public int Duplicates { get; set; }
        public int Failed { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
