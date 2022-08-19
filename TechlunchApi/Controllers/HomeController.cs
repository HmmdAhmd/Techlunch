using Microsoft.AspNetCore.Mvc;

namespace TechlunchApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}