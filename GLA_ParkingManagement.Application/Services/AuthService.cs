using AutoMapper;
using GLA_ParkingManagement.Application.Interfaces;
using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Domain.DTOs;
using GLA_ParkingManagement.Infrastructure.Database;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ParkingManagementDbContext _context;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IMapper mapper,
            RoleManager<IdentityRole> roleManager,
            ParkingManagementDbContext context
            )
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._mapper = mapper;
            this._roleManager = roleManager;   
            this._context = context;
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
            if (!user.IsActive)
            {
                response.StatusCode = Convert.ToInt32(HttpStatusCode.Forbidden);
                response.Message = "Account has deactivated, Please contact to admin,";
                response.Success = false;
                return response;
            }
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, false);
            if (!result.Succeeded)
            {
                response.StatusCode = 401;
                response.Message = "Invalid Email or Password.";
                response.Success = false;
                return response;
            }
            var userRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            // update the response
            response.StatusCode = 200;
            response.Message = "Logged in successfully";
            response.Success = true;
            response.Data = new LoginResponse()
            {
                Email = user.Email!,
                userId = user.Id,
                username = user.UserName!,
                FullName = user.FirstName+" "+user.LastName,
                Role = userRole!
            };
            return response;
        }

        public async Task<ServiceResponse<string>> RegisterUserAsync(RegisterUser request)
        {
            var response = new ServiceResponse<string>();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //Check existing user
                var existUser = await _userManager.FindByEmailAsync(request.Email);
                if (existUser != null)
                {
                    response.Success = false;
                    response.Message = "Email already exists.";
                    response.StatusCode = 400;
                    return response;
                }

                // Map DTO → Entity
                var appUser = _mapper.Map<AppUser>(request);
                appUser.UserName = request.Email.Split("@")[0];

                //Create user
                var userResult = await _userManager.CreateAsync(appUser, request.Password);

                if (!userResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = string.Join(", ", userResult.Errors.Select(e => e.Description));
                    response.StatusCode = 400;
                    return response;
                }

                // Assign role
                var roleResult = await _userManager.AddToRoleAsync(appUser, CommonProperties.CustomerUser);

                if (!roleResult.Succeeded)
                {
                    // Rollback user creation if role fails
                    await transaction.RollbackAsync();

                    response.Success = false;
                    response.Message = "User created but role assignment failed.";
                    response.StatusCode = 500;
                    return response;
                }

                // Commit transaction
                await transaction.CommitAsync();

                response.Success = true;
                response.Message = "User registered successfully.";
                response.StatusCode = 200;
                response.Data = appUser.Id;

                return response;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                response.Success = false;
                response.StatusCode = 500;
                response.Message = "Something went wrong.";
                return response;
            }
        }
    }
}
