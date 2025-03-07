using Microsoft.AspNetCore.Mvc;
using PaymentAutomationAPI.Interfaces;
using PaymentAutomationAPI.Models;
using System.Threading.Tasks;

namespace PaymentAutomationMVC.Controllers
{
    public class CardController : Controller
    {
        private readonly IMerchantSafeService _merchantSafeService;

        public CardController(IMerchantSafeService merchantSafeService)
        {
            _merchantSafeService = merchantSafeService;
        }

        public async Task<IActionResult> Index()
        {
            var safeKey = "your-safe-key";
            var response = await _merchantSafeService.GetCards(safeKey);
            return View(response);
        }

        // Other actions using _merchantSafeService
    }
}