namespace PaymentAutomationMVC.Models
{
    public class PaymentViewModel
    {
        public string SafeKey { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public string Description { get; set; }
    }
}
