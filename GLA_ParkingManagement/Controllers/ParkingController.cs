using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Domain.DTOs;
using GLA_ParkingManagement.Domain.Entities;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.AppConfig;
using System.Security.Claims;

namespace GLA_ParkingManagement.Controllers
{
    [Authorize(Roles = CommonProperties.AdminUser + "," + CommonProperties.CustomerUser)]
    public class ParkingController : Controller
    {
        private IParkingService _parkingService;
        public ParkingController(IParkingService parkingService)
        {
            this._parkingService = parkingService;
        }

        [Authorize(Roles =CommonProperties.AdminUser)]
        [HttpGet]
        public IActionResult VehicleTypes()
        {
            return View();
        }

        public IActionResult CheckParkingSlots()
        {
            return View();
        }

        [Authorize(Roles = CommonProperties.AdminUser)]
        [HttpPost]
        public async Task<IActionResult> AddUpdateVehicleType(CreateVehicleTypeDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            ServiceResponse<string> response;

            if (model.Id == 0)
            {
                response = await _parkingService.CreateVehicleType(model);
            }
            else
            {
                response = await _parkingService.UpdateVehicleTypeBy(model);
            }

            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(new
            {
                success = true,
                message = response.Message
            });
        }
        public async Task<IActionResult> GetAllVehicleTypes()
        {
            var resp = await _parkingService.GetVehicleTypes();
            return Json(new { data = resp });
        }

        [Authorize(Roles = CommonProperties.AdminUser)]
        public IActionResult Slots()
        {
            return View();
        }

        [Authorize(Roles = CommonProperties.AdminUser)]
        public async Task<IActionResult> GetAllSlots()
        {
            var res = await _parkingService.GetAllSlots();
            return Json(new { data = res.Data });
        }

        [HttpPost]
        [Authorize(Roles = CommonProperties.AdminUser)]
        public async Task<IActionResult> AddUpdateSlot(CreateParkingSlotDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            ServiceResponse<string> response;

            if (model.Id == 0)
                response = await _parkingService.CreateSlot(model);
            else
                response = await _parkingService.UpdateSlot(model);

            return Json(response);
        }

        [HttpDelete]
        [Authorize(Roles = CommonProperties.AdminUser)]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var res = await _parkingService.DeleteSlot(id);
            return Json(res);
        }

        public async Task<IActionResult> GetAvailableSlots(int vehicleTypeId)
        {
            var slots = await _parkingService.GetAvailableSlots(vehicleTypeId);
            return Json(slots);
        }
        [HttpPost]
        public async Task<IActionResult> BookParking(BookParkingRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not logged in" });

            var result = await _parkingService.BookParking(request, userId);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        public async Task<IActionResult> GetBookingHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not logged in" });

            var result = await _parkingService.GetUserParkingHistory(userId);

            return Json(new
            {
                success = result.Success,
                data = result.Data
            });
        }

        public IActionResult BookingHistory()
        {
            return View();
        }

        [Authorize(Roles = CommonProperties.AdminUser)]
        [HttpGet]
        public async Task<IActionResult> GetAllParkingRecords()
        {
            var data = await _parkingService.GetAllParkingRecords();

            return Json(new
            {
                data = data
            });
        }

        public IActionResult PandingParkings()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingRequests()
        {
            var data = await _parkingService.GetPendingRequests();

            return Json(new
            {
                data = data
            });
        }

        [HttpPost]
        [Authorize(Roles = CommonProperties.AdminUser)]
        public async Task<IActionResult> ApproveParking(int id)
        {
            var result = await _parkingService.ApproveParking(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }


        [HttpPost]
        [Authorize(Roles = CommonProperties.AdminUser)]
        public async Task<IActionResult> RejectParking(int id)
        {
            var result = await _parkingService.RejectParking(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        [Authorize(Roles = CommonProperties.AdminUser)]
        [HttpPost]
        public async Task<IActionResult> CompleteParking(int id)
        {
            var result = await _parkingService.CompleteParking(id);

            return Json(new
            {
                success = result.Success,
                message = result.Message
            });
        }

        public IActionResult AllParkingRecords()
        {
            return View();
        }
    }
}
