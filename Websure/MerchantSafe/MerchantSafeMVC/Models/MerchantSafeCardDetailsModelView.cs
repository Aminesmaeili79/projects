namespace MerchantSafeMVC.Models
{
    public class MerchantSafeCardDetailsViewModel
    {
        public string ProcReturnCode { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string SafeKeyLastModified { get; set; } = string.Empty;
        public string Pan { get; set; } = string.Empty;
        public string PanStatus { get; set; } = string.Empty;
        public string IndexAccountClosure { get; set; } = string.Empty;
        public string PanExpiry { get; set; } = string.Empty;
        public string PanOwner { get; set; } = string.Empty;
        public string NumberOfPans { get; set; } = string.Empty;
        public string IndexOrder { get; set; } = string.Empty;
        public string PanDescription { get; set; } = string.Empty;
    }
}