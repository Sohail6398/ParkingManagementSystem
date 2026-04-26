using GLA_ParkingManagement.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace GLA_ParkingManagement.Controllers
{
    public class AccountController : Controller
    {
        private 
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
        }
    }
}
