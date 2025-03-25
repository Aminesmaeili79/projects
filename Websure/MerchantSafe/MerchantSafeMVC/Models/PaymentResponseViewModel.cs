namespace MerchantSafeMVC.Models
{
    public class PaymentResponseViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string AuthCode { get; set; } = string.Empty;
        public string HostRefNum { get; set; } = string.Empty;
        public string ProcReturnCode { get; set; } = string.Empty;
        public string TransId { get; set; } = string.Empty;
        public string ErrMsg { get; set; } = string.Empty;
        public PaymentExtraInfo Extra { get; set; } = new PaymentExtraInfo();
    }
}
