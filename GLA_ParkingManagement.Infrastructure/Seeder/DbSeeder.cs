using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Domain.Enums;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Infrastructure.Seeder
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userManager = service.GetRequiredService<UserManager<AppUser>>();
            var roleManager = service.GetRequiredService<RoleManager<IdentityRole>>();
            // 1. Seed Roles
            if (!await roleManager.RoleExistsAsync(CommonProperties.AdminUser))
                await roleManager.CreateAsync(new IdentityRole(CommonProperties.AdminUser));
            // 2. Seed Admin User
            var adminEmail = "admin@gmail.com";

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                var adminUser = new AppUser()
                {
                    UserName = "admin6398",
                    NormalizedUserName = "ADMIN6398",
                    Email = "admin@gmail.com",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                    FirstName = "Zishan",
                    LastName = "Qureshi",
                    Gender = GenderType.Male,
                    SecurityStamp = "d3b07384-d9a1-4c3b-9c2e-123456789abc",
                    ConcurrencyStamp = "a7f5f354-8b2a-4c6d-9f3a-abcdef123456",
                    PasswordHash = "Admin@123!"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, CommonProperties.AdminUser);
                }
            }
        }
    }
}
