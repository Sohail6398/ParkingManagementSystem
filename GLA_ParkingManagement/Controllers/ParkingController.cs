using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GLA_ParkingManagement.Controllers
{
    [Authorize(Roles = CommonProperties.AdminUser + "," + CommonProperties.CustomerUser)]
    public class ParkingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
