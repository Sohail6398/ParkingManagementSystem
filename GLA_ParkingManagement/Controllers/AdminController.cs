using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLA_ParkingManagement.Controllers
{
    [Authorize(Roles = CommonProperties.AdminUser)]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
