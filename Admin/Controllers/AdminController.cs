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
        public IActionResult Dashboard()
        {
            //List<ProductSummaryDto> products = _apiCall.GetProducts();
            return View();
        }

    }
}
