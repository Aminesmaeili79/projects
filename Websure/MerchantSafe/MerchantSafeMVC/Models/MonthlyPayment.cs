namespace MerchantSafeMVC.Models
{
    public class MonthlyPayment
    {
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsEdited { get; set; }
    }
}
