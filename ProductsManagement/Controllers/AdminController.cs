using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductsManagement.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet("/admin/login")]
        public IActionResult Login()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("/admin/dashboard")]
        //[AllowAnonymous]
        public IActionResult Dashboard()
        {
            return View();
        }

    }
}
