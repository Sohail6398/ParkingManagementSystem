using AutoMapper;
using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Domain.DTOs;
using GLA_ParkingManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Infrastructure.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser, RegisterUser>().ReverseMap();
            CreateMap<AppUser, UserDTO>().ReverseMap();
            CreateMap<VehicleTypeDTO, VehicleType>().ReverseMap();
            CreateMap<CreateParkingSlotDTO, ParkingSlot>().ReverseMap();
            CreateMap<ParkingSlotDTO, ParkingSlot>().ReverseMap();
        }
    }
}
