namespace WebApplication1.Models
{
    public class MerchantSafeRequestViewModel
    {
        public string SafeKey { get; set; } = string.Empty;
        public CardInfoViewModel? CardInfo { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency { get; set; }
        public string? Description { get; set; }
        public int? Priority { get; set; }
        public int? AccountClosureDay { get; set; }
    }
}
