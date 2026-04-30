using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Domain.DTOs;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GLA_ParkingManagement.Controllers
{
    public class AccountController : Controller
    {
        private IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            this._authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser request)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please fix the validation errors.";
                return View(request);
            }

            var result = await _authService.RegisterUserAsync(request);

            if (!result.Success)
            {
                TempData["error"] = result.Message;
                return View(request);
            }

            TempData["success"] = result.Message;
            return RedirectToAction("Login", "Account");
            }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var result = await _authService.LoginAsync(request);
            if(!result.Success)
            {
                TempData["errorMessage"] = result.Message;
                return View(request);
            }
            // Add the session.
            HttpContext.Session.SetString(CommonProperties.UserId, result.Data!.userId);
            HttpContext.Session.SetString(CommonProperties.Role, result.Data!.Role);

            return result.Data.Role switch
            {
                CommonProperties.AdminUser => RedirectToAction("Dashboard", "Admin"),
                CommonProperties.CustomerUser => RedirectToAction("Index", "Home"),
                _ => RedirectToAction("Index", "Home")
            };
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            TempData["successMessage"] = "Logged out successfully";

            return RedirectToAction("Login", "Account");
        }
    }
}
