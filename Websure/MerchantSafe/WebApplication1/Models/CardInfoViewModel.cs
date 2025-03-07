namespace WebApplication1.Models
{
    public class CardInfoViewModel
    {
        public string Number { get; set; } = string.Empty;
        public string Expires { get; set; } = string.Empty;
        public string? Cvv { get; set; }
        public string? CardOwner { get; set; }
    }
}
