namespace MerchantSafeMVC.Models
{
    public class PaymentExtraInfo
    {
        public string SettleId { get; set; } = string.Empty;
        public string TrxDate { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public string HostMsg { get; set; } = string.Empty;
        public string NumCode { get; set; } = string.Empty;
    }
}
