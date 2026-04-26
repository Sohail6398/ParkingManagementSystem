using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager; 
        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        public async Task<ServiceResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            var response = new ServiceResponse<LoginResponse>();
            if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                response.StatusCode = 401;
                response.Message = "Invalid request.";
                response.Success = false;
                return response;
            }
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.StatusCode = 401;
                response.Message = "Invalid Email or Password.";
                response.Success = false;
                return response;
            }
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
        }
    }
}
