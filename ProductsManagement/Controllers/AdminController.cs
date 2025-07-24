using Microsoft.AspNetCore.Mvc;

namespace ProductsManagement.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
