using Microsoft.AspNetCore.Mvc;

namespace TestWeb.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Message = "Все пропало";
            return View();
        }
    }
}