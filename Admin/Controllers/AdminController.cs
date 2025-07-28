using Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using ProductsManagement.Models;
using System.Diagnostics;

namespace Admin.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ApiCalls _apiCall;
        public AdminController(ILogger<AdminController> logger, ApiCalls apiCall)
        {
            _logger = logger;
            _apiCall = apiCall;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginDto)
        {
            ApiResponse<Tuple<string, string>> result = await _apiCall.LoginAsync(loginDto);
            var data = result.Data;
            if(result.Success == false || data == null)
            {
                ViewData["ErrorMessage"] = result.Message ?? "Login failed.";
                return View(loginDto);
            }

            if (data.Item2.Equals("admin"))
            {
                // Login success - save token in TempData, Session, or Cookie
                HttpContext.Session.SetString("token", data.Item1);
                HttpContext.Session.SetString("role", data.Item2);

                return RedirectToAction("Dashboard", "Admin"); // Change to your actual page
            }
            ViewData["ErrorMessage"] = data.Item2 ?? "Invalid login.";
            return View(loginDto);
        }
        public async Task<IActionResult> Dashboard()
        {
            return View();
        }

    }
}
