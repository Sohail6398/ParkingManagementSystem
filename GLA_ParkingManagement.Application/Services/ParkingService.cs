using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Domain.DTOs;
using GLA_ParkingManagement.Domain.Entities;
using GLA_ParkingManagement.Domain.Enums;
using GLA_ParkingManagement.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Application.Services
{
    public class ParkingService : IParkingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ParkingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Method to book the parking according avalable slots.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ServiceResponse<string>> BookParking(BookParkingRequest request, string userId)
        {
            var response = new ServiceResponse<string>();

            // Check if user already has active/confirmed parking
            var hasActive = await _unitOfWork.GetRepository<ParkingRecord>()
                .AnyAsync(x => x.UserId == userId && x.Status == ParkingStatus.Confirmed);

            if (hasActive)
            {
                response.Success = false;
                response.Message = "You already have an active parking.";
                return response;
            }

            //Get slot
            var slot = await _unitOfWork.GetRepository<ParkingSlot>()
                .GetByIdAsync(request.SlotId);

            if (slot == null || slot.IsOccupied)
            {
                response.Success = false;
                response.Message = "Slot is not available";
                return response;
            }

            // Validate vehicle type
            if (slot.VehicleTypeId != request.VehicleTypeId)
            {
                response.Success = false;
                response.Message = "Invalid slot for selected vehicle type";
                return response;
            }

            // Create record (Pending)
            var record = new ParkingRecord
            {
                VehicleNumber = request.VehicleNumber,
                VehicleTypeId = request.VehicleTypeId,
                SlotId = request.SlotId,
                EntryTime = DateTime.Now,
                Status = ParkingStatus.Pending,
                UserId = userId
            };

            await _unitOfWork.GetRepository<ParkingRecord>().AddAsync(record);

            // DO NOT occupy slot yet

            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Parking request submitted. Waiting for admin approval.";
            return response;
        }

        public async Task<ServiceResponse<string>> CompleteParking(int recordId)
        {
            var response = new ServiceResponse<string>();

            var record = await _unitOfWork.GetRepository<ParkingRecord>()
                .GetByIdAsync(recordId);

            if (record == null)
            {
                response.Success = false;
                response.Message = "Record not found";
                return response;
            }

            record.ExitTime = DateTime.Now;

            var duration = (record.ExitTime.Value - record.EntryTime).TotalHours;
            record.DurationHours = Math.Round(duration, 2);

            var vehicleType = await _unitOfWork.GetRepository<VehicleType>()
                .GetByIdAsync(record.VehicleTypeId);

            record.TotalAmount = (decimal)record.DurationHours * vehicleType!.HourlyRate;
            record.Status = ParkingStatus.Completed;

            // Free slot
            var slot = await _unitOfWork.GetRepository<ParkingSlot>()
                .GetByIdAsync(record.SlotId);

            slot.IsOccupied = false;

            _unitOfWork.GetRepository<ParkingRecord>().Update(record);
            _unitOfWork.GetRepository<ParkingSlot>().Update(slot);

            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Parking completed";
            return response;
        }

        public async Task<ServiceResponse<string>> ConfirmParking(int recordId)
        {
            var response = new ServiceResponse<string>();

            var record = await _unitOfWork.GetRepository<ParkingRecord>()
                .GetByIdAsync(recordId);

            if (record == null)
            {
                response.Success = false;
                response.Message = "Record not found";
                return response;
            }

            var slot = await _unitOfWork.GetRepository<ParkingSlot>()
                .GetByIdAsync(record.SlotId);

            if (slot.IsOccupied)
            {
                response.Success = false;
                response.Message = "Slot already occupied";
                return response;
            }

            record.Status = ParkingStatus.Confirmed;
            slot.IsOccupied = true;

            _unitOfWork.GetRepository<ParkingRecord>().Update(record);
            _unitOfWork.GetRepository<ParkingSlot>().Update(slot);

            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Parking confirmed";
            return response;
        }

        public async Task<ServiceResponse<string>> CreateVehicleType(CreateVehicleTypeDTO request)
        {
            var response = new ServiceResponse<string>();

            var entity = new VehicleType
            {
                Name = request.Name,
                HourlyRate = request.HourlyRate
            };

            await _unitOfWork.GetRepository<VehicleType>().AddAsync(entity);
            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Vehicle type created";
            return response;
        }

        public async Task<List<ParkingRecordDTO>> GetAllParkingRecords()
        {
            var records = await _unitOfWork.GetRepository<ParkingRecord>().GetAllAsync();

            return records.Select(r => new ParkingRecordDTO
            {
                Id = r.Id,
                VehicleNumber = r.VehicleNumber,
                EntryTime = r.EntryTime,
                ExitTime = r.ExitTime,
                TotalAmount = r.TotalAmount,
                Status = r.Status.ToString()
            }).ToList();
        }

        /// <summary>
        /// Get the available slots.
        /// </summary>
        /// <param name="vehicleTypeId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<ParkingSlotDTO>> GetAvailableSlots(int vehicleTypeId)
        {
            var slots = await _unitOfWork.GetRepository<ParkingSlot>()
            .FindAsync(x => x.VehicleTypeId == vehicleTypeId && !x.IsOccupied);
            return slots.Select(s => new ParkingSlotDTO
            {
                Id = s.Id,
                SlotNumber = s.SlotNumber,
                IsOccupied = s.IsOccupied
            }).ToList();
        }

        public async Task<List<ParkingHistoryDTO>> GetUserParkingHistory(string userId)
        {
            var records = await _unitOfWork.GetRepository<ParkingRecord>()
            .FindAsync(x => x.UserId == userId);

            return records.Select(r => new ParkingHistoryDTO
            {
                VehicleNumber = r.VehicleNumber,
                SlotNumber = r.Slot.SlotNumber,
                EntryTime = r.EntryTime,
                ExitTime = r.ExitTime,
                TotalAmount = r.TotalAmount,
                Status = r.Status.ToString()
            }).ToList();
        }

        public async Task<List<VehicleTypeDTO>> GetVehicleTypes()
        {
            var types = await _unitOfWork.GetRepository<VehicleType>().GetAllAsync();

            return types.Select(x => new VehicleTypeDTO
            {
                Id = x.Id,
                Name = x.Name,
                HourlyRate = x.HourlyRate
            }).ToList();
        }

        public async Task<List<PendingParkingDTO>> GetPendingRequests()
        {
            var records = await _unitOfWork.GetRepository<ParkingRecord>()
                .GetAllAsync( x => x
                    .Include(r => r.Slot)
                    .Include(r => r.VehicleType));

            return records
                .Where(x => x.Status == ParkingStatus.Pending)
                .Select(r => new PendingParkingDTO
                {
                    Id = r.Id,
                    VehicleNumber = r.VehicleNumber,
                    VehicleType = r.VehicleType.Name,
                    SlotNumber = r.Slot.SlotNumber,
                    EntryTime = r.EntryTime,
                    UserName = r.UserId 
                }).ToList();
        }
    }
}
