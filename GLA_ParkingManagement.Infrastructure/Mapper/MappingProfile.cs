using AutoMapper;
using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Domain.DTOs;
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
        }
    }
}
