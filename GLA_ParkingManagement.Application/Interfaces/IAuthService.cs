using GLA_ParkingManagement.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest loginRequest);
    }
}
