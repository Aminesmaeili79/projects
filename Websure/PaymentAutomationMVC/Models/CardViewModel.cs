namespace PaymentAutomationMVC.Models
{
    public class CardViewModel
    {
        public string SafeKey { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string Cvv { get; set; }
        public string CardOwner { get; set; }
        public string Description { get; set; }
        public int? Priority { get; set; }
        public int? AccountClosureDay { get; set; }
    }
}
