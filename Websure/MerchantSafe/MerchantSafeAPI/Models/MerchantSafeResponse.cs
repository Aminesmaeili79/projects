namespace MerchantSafeAPI.Models
{
    public class MerchantSafeResponse
    {
        public string? ProcReturnCode { get; set; }
        public string? Response { get; set; }
        public string? ErrorMessage { get; set; }
        public string? TransactionId { get; set; }
        public string RawResponse { get; set; }
    }
}
