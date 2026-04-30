using GLA_ParkingManagement.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Application.Interfaces
{
    public interface IParkingService
    {
        // Customer
        Task<List<ParkingSlotDTO>> GetAvailableSlots(int vehicleTypeId);
        Task<ServiceResponse<string>> BookParking(BookParkingRequest request, string userId);
        Task<List<ParkingHistoryDTO>> GetUserParkingHistory(string userId);

        // Admin
        Task<ServiceResponse<string>> ConfirmParking(int recordId);
        Task<ServiceResponse<string>> CompleteParking(int recordId);
        Task<List<ParkingRecordDTO>> GetAllParkingRecords();
        Task<List<PendingParkingDTO>> GetPendingRequests();

        // Vehicle Type (Admin)
        Task<ServiceResponse<string>> CreateVehicleType(CreateVehicleTypeDTO request);
        Task<List<VehicleTypeDTO>> GetVehicleTypes();
    }
}
