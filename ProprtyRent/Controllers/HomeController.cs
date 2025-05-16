using Microsoft.AspNetCore.Mvc;

namespace PropertyRent.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Landing()
        {
            return View();
        }
    }
}
