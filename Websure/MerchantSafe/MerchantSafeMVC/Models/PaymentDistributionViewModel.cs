namespace MerchantSafeMVC.Models
{
    public class PaymentDistributionViewModel
    {
        public decimal TotalAmount { get; set; }
        public int NumberOfMonths { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public List<MonthlyPayment> MonthlyPayments { get; set; } = new();
    }
}
