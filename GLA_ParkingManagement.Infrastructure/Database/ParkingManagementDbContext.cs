using GLA_ParkingManagement.Domain.ApplicationUser;
using GLA_ParkingManagement.Domain.Entities;
using GLA_ParkingManagement.Domain.Enums;
using GLA_ParkingManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Infrastructure.Database
{
    public class ParkingManagementDbContext : IdentityDbContext<AppUser>
    {
        public ParkingManagementDbContext(DbContextOptions<ParkingManagementDbContext> options) : base(options) { }

        //public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<VehicleType> VehicleTypes { get; set; }
        public DbSet<ParkingSlot> ParkingSlots { get; set; }
        public DbSet<ParkingRecord> ParkingRecords { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            string adminRoleId = "8f3a2c6e-1b4a-4f9c-9c3e-2a7d5e8b1f01";
            string customerRoleId = "c2d9a7f4-6e3b-4a91-b8d2-5f0c9e7a2b12";
            string adminUserId = "5b7e1d9c-3a2f-4c8b-9e6d-1f0a2b3c4d55";

            base.OnModelCreating(builder);

            // Unique Slot Number
            builder.Entity<ParkingSlot>()
                .HasIndex(x => x.SlotNumber)
                .IsUnique();

            // Relationships
            builder.Entity<ParkingRecord>()
                .HasOne(p => p.Slot)
                .WithMany(s => s.ParkingRecords)
                .HasForeignKey(p => p.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.ParkingRecord)
                .WithMany()
                .HasForeignKey(p => p.ParkingRecordId);

            // Seed Vehicle Types
            //builder.Entity<VehicleType>().HasData(
            //    new VehicleType { Id = 1, Name = "Car", HourlyRate = 50 },
            //    new VehicleType { Id = 2, Name = "Bike", HourlyRate = 20 },
            //    new VehicleType { Id = 3, Name = "Truck", HourlyRate = 100 }
            //);

            // 🌱 Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = CommonProperties.AdminUser,
                    NormalizedName = CommonProperties.AdminUser.ToUpper(),
                    ConcurrencyStamp = adminRoleId
                },
                new IdentityRole
                {
                    Id = customerRoleId,
                    Name = CommonProperties.CustomerUser,
                    NormalizedName = CommonProperties.CustomerUser.ToUpper(),
                    ConcurrencyStamp = customerRoleId
                }
            );

            //builder.Entity<AppUser>().HasData(new
            //AppUser
            //{
            //    Id = adminUserId,
            //    UserName = "admin@parking.com",
            //    NormalizedUserName = "ADMIN@PARKING.COM",
            //    Email = "admin@gmail.com",
            //    NormalizedEmail = "ADMIN@PARKING.COM",
            //    EmailConfirmed = true,
            //    FirstName = "Zishan",
            //    LastName = "Qureshi",
            //    Gender = GenderType.Male,
            //    PasswordHash = "Admin@123",
            //    SecurityStamp = "d3b07384-d9a1-4c3b-9c2e-123456789abc",
            //    ConcurrencyStamp = "a7f5f354-8b2a-4c6d-9f3a-abcdef123456"
            //});

            //// 🔗 Assign Admin Role to Admin User
            //builder.Entity<IdentityUserRole<string>>().HasData(
            //    new IdentityUserRole<string>
            //    {
            //        UserId = adminUserId,
            //        RoleId = adminRoleId
            //    }
            //);

            builder.Entity<VehicleType>()
                .Property(v => v.HourlyRate)
                .HasPrecision(10, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            builder.Entity<ParkingRecord>()
                .Property(p => p.TotalAmount)
                .HasPrecision(10, 2);
        }
    }
}
