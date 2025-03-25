namespace MerchantSafeAPI.Models
{
    public class CardInfo
    {
        public required string Number { get; set; }
        public required string Expires { get; set; }
        public string? Cvv { get; set; }
        public string? CardOwner { get; set; }
    }
}
