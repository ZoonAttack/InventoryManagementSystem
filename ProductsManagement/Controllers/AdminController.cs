using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsManagement.DTOs;
using ProductsManagement.Models;

namespace ProductsManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly APICalls _apiCall;

        public AdminController(APICalls apiCall)
        {
            _apiCall = apiCall;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginUserDto());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginDto)
        {
            var result = await _apiCall.LoginAsync(loginDto);

            if (result.Item2 == "admin")
            {
                // Login success - save token in TempData, Session, or Cookie
                TempData["token"] = result.Item1;
                TempData["role"] = result.Item2;

                return RedirectToAction("Dashboard", "Admin"); // Change to your actual page
            }

            ViewData["ErrorMessage"] = result.Item2 ?? "Invalid login.";
            return View(loginDto);
        }
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }
    }
}
