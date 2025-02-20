using PaymentAutomation.Models;

namespace PaymentAutomation.Interfaces
{
    public interface IMerchantSafeService
    {
        Task<MerchantSafeResponse> AddCard(MerchantSafeRequest request);
        Task<MerchantSafeResponse> UpdateCard(MerchantSafeRequest request);
        Task<MerchantSafeResponse> GetCards(string safeKey);
        Task<MerchantSafeResponse> DisableCards(string safeKey);
        Task<MerchantSafeResponse> ProcessPayment(MerchantSafeRequest request);
    }
}
