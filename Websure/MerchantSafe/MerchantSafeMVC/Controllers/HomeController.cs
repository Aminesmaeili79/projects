using Microsoft.AspNetCore.Mvc;

namespace MerchantSafeMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Redirect("MerchantSafe/");
        }
    }
}
