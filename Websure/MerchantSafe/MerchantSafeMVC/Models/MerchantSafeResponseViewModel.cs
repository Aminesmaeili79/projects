namespace MerchantSafeMVC.Models
{
    public class MerchantSafeResponseViewModel
    {
        public string? OrderId { get; set; }
        public string? GroupId { get; set; }
        public string? Response { get; set; }
        public string? AuthCode { get; set; }
        public string? HostRefNum { get; set; }
        public string? ProcReturnCode { get; set; }
        public string? TransId { get; set; }  // This matches what's used in the view
        public string? ErrMsg { get; set; }
        public string? ErrorMessage { get; set; }
        public string? RawResponse { get; set; }
        public ExtraResponseInfo? Extra { get; set; }
    }
}