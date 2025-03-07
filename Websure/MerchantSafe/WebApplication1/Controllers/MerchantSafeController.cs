using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class MerchantSafeController : Controller
    {
        public IActionResult Index()
        {
            return View("AddCard");
        }

        [HttpGet]
        public IActionResult AddCard()
        {
            return View();
        }
    }
}
