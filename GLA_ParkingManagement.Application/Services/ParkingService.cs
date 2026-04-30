using AutoMapper;
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
using static System.Reflection.Metadata.BlobBuilder;

namespace GLA_ParkingManagement.Application.Services
{
    public class ParkingService : IParkingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ParkingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            var slots = await _unitOfWork
            .GetRepository<ParkingSlot>()
            .GetAllAsync(
                x => x.VehicleTypeId == vehicleTypeId,
                include: q => q.Include(s => s.VehicleType)
            );
            return slots.Select(s => new ParkingSlotDTO
            {
                Id = s.Id,
                SlotNumber = s.SlotNumber,
                VehicleTypeId = s.VehicleTypeId,
                VehicleTypeName = s.VehicleType != null ? s.VehicleType.Name : "",
                IsOccupied = s.IsOccupied
            }).ToList();
        }

        public async Task<ServiceResponse<List<ParkingHistoryDTO>>> GetUserParkingHistory(string userId)
        {
            var response = new ServiceResponse<List<ParkingHistoryDTO>>();

            try
            {
                var records = await _unitOfWork.GetRepository<ParkingRecord>()
                    .GetAllAsync(
                        x => x.UserId == userId,
                        include: q => q
                            .Include(r => r.Slot)
                            .Include(r => r.VehicleType)
                    );

                var result = records
                    .OrderByDescending(x => x.EntryTime)
                    .Select(r => new ParkingHistoryDTO
                    {
                        Id = r.Id,
                        VehicleNumber = r.VehicleNumber,
                        VehicleType = r.VehicleType.Name,
                        SlotNumber = r.Slot.SlotNumber,
                        EntryTime = r.EntryTime,
                        ExitTime = r.ExitTime,
                        TotalAmount = r.TotalAmount,
                        Status = r.Status.ToString()
                    }).ToList();

                response.Success = true;
                response.Data = result;
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "Failed to fetch history";
            }

            return response;
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
        public async Task<VehicleTypeDTO> GetVehicleTypeById(int id)
        {
            var vehicleType = await _unitOfWork.GetRepository<VehicleType>().GetByIdAsync(id);
            return _mapper.Map<VehicleTypeDTO>(vehicleType);
        }
        public async Task<ServiceResponse<string>> UpdateVehicleTypeBy(CreateVehicleTypeDTO request)
        {
            var response = new ServiceResponse<string>();
            var vehicleRepo = _unitOfWork.GetRepository<VehicleType>();
            var vehicleType = await vehicleRepo.GetByIdAsync(request.Id);
            if (vehicleType == null)
            {
                response.StatusCode = 404;
                response.Success = false;
                response.Message = "Vehicle type not found.";
                return response;
            }
            vehicleType.Name = request.Name;
            vehicleType.HourlyRate = request.HourlyRate;
            vehicleRepo.Update(vehicleType);
            await _unitOfWork.SaveAsync();

            response.StatusCode = 201;
            response.Success = true;
            response.Message = "Vehicle updated sucessfully";
            return response;
        }
        public async Task<List<PendingParkingDTO>> GetPendingRequests()
        {
            var records = await _unitOfWork.GetRepository<ParkingRecord>()
                                .GetAllAsync(
                                    x => x.Status == ParkingStatus.Pending, //  filter FIRST
                                    include: q => q
                                        .Include(r => r.Slot)
                                        .Include(r => r.VehicleType)
                                );

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

        public async Task<ServiceResponse<string>> CreateSlot(CreateParkingSlotDTO model)
        {
            var response = new ServiceResponse<string>();

            var exists = await _unitOfWork.GetRepository<ParkingSlot>()
                .AnyAsync(x => x.SlotNumber == model.SlotNumber);

            if (exists)
            {
                response.Success = false;
                response.Message = "Slot already exists";
                return response;
            }

            var slot = new ParkingSlot
            {
                SlotNumber = model.SlotNumber,
                VehicleTypeId = model.VehicleTypeId,
                IsOccupied = false
            };

            await _unitOfWork.GetRepository<ParkingSlot>().AddAsync(slot);
            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Slot created successfully";
            return response;
        }
        public async Task<ServiceResponse<string>> UpdateSlot(CreateParkingSlotDTO model)
        {
            var response = new ServiceResponse<string>();

            var slot = await _unitOfWork.GetRepository<ParkingSlot>()
                .GetByIdAsync(model.Id);

            if (slot == null)
            {
                response.Success = false;
                response.Message = "Slot not found";
                return response;
            }

            slot.SlotNumber = model.SlotNumber;
            slot.VehicleTypeId = model.VehicleTypeId;

            _unitOfWork.GetRepository<ParkingSlot>().Update(slot);
            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Slot updated successfully";
            return response;
        }
        public async Task<ServiceResponse<string>> DeleteSlot(int id)
        {
            var response = new ServiceResponse<string>();

            var slot = await _unitOfWork.GetRepository<ParkingSlot>()
                .GetByIdAsync(id);

            if (slot == null)
            {
                response.Success = false;
                response.Message = "Slot not found";
                return response;
            }

            _unitOfWork.GetRepository<ParkingSlot>().Delete(slot);
            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Slot deleted successfully";
            return response;
        }
        public async Task<ServiceResponse<List<ParkingSlotDTO>>> GetAllSlots()
        {
            var response = new ServiceResponse<List<ParkingSlotDTO>>();
            var slotList = await _unitOfWork.GetRepository<ParkingSlot>().GetAllAsync(include: x => x.Include(s => s.VehicleType));
            if (slotList.Count() == 0)
            {
                response.Success = false;
                response.Message = "Emplty List";
                response.StatusCode = 404;
                return response;
            }
            var result = slotList.Select(s => new ParkingSlotDTO
            {
                Id = s.Id,
                SlotNumber = s.SlotNumber,
                VehicleTypeId = s.VehicleTypeId,
                VehicleTypeName = s.VehicleType.Name,
                IsOccupied = s.IsOccupied
            }).ToList();
            response.Success = true;
            response.Message = string.Empty;
            response.StatusCode = 200;
            response.Data = result;
            return response;
        }

        public async Task<ServiceResponse<string>> ApproveParking(int id)
        {
            var response = new ServiceResponse<string>();

            var record = await _unitOfWork.GetRepository<ParkingRecord>()
                .GetByIdAsync(id);

            if (record == null)
            {
                response.Success = false;
                response.Message = "Record not found";
                return response;
            }

            if (record.Status != ParkingStatus.Pending)
            {
                response.Success = false;
                response.Message = "Already processed";
                return response;
            }

            var slot = await _unitOfWork.GetRepository<ParkingSlot>()
                .GetByIdAsync(record.SlotId);

            if (slot == null || slot.IsOccupied)
            {
                response.Success = false;
                response.Message = "Slot not available";
                return response;
            }

            record.Status = ParkingStatus.Confirmed;
            slot.IsOccupied = true;

            _unitOfWork.GetRepository<ParkingRecord>().Update(record);
            _unitOfWork.GetRepository<ParkingSlot>().Update(slot);

            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Parking approved";

            return response;
        }

        public async Task<ServiceResponse<string>> RejectParking(int id)
        {
            var response = new ServiceResponse<string>();

            var record = await _unitOfWork.GetRepository<ParkingRecord>()
                .GetByIdAsync(id);

            if (record == null)
            {
                response.Success = false;
                response.Message = "Record not found";
                return response;
            }

            if (record.Status != ParkingStatus.Pending)
            {
                response.Success = false;
                response.Message = "Already processed";
                return response;
            }

            record.Status = ParkingStatus.Rejected; // or Rejected (better if enum updated)

            _unitOfWork.GetRepository<ParkingRecord>().Update(record);
            await _unitOfWork.SaveAsync();

            response.Success = true;
            response.Message = "Parking rejected";

            return response;
        }
    }
}
