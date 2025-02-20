namespace PaymentAutomation.Models
{
    public class MerchantSafeConfig
    {
        public required string ClientId { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ApiUrl { get; set; }
    }
}
