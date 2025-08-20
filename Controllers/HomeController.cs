using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Dashbaord()
        {
            return View();
        }
    }
}
