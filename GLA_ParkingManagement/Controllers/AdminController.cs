using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace GLA_ParkingManagement.Controllers
{
    [Authorize(Roles = CommonProperties.AdminUser)]
    public class AdminController : Controller
    {
        private IAuthService _authService;
        public AdminController(IAuthService authService)
        {
            this._authService = authService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AllUsers()
        {
            return View();
        }
        public async Task<IActionResult> GetAllUsers()
        {
            var resp = await _authService.GetAllUsers();
            return Json(resp);
        }
    }
}
