namespace MerchantSafeAPI.Models
{
    public class MerchantSafeRequest
    {
        public required string SafeKey { get; set; }
        public CardInfo? CardInfo { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int? AccountClosureDay { get; set; }
    }
}
