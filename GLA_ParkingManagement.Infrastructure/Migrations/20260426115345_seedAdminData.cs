using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GLA_ParkingManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedAdminData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "Email", "EmailConfirmed", "FirstName", "Gender", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5b7e1d9c-3a2f-4c8b-9e6d-1f0a2b3c4d55", 0, null, "a7f5f354-8b2a-4c6d-9f3a-abcdef123456", new DateTime(2026, 4, 26, 11, 53, 44, 945, DateTimeKind.Utc).AddTicks(5079), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@gmail.com", true, "Zishan", 0, true, "Qureshi", false, null, "ADMIN@GMAIL.COM", "ADMIN6398", "Admin@123!", null, false, "d3b07384-d9a1-4c3b-9c2e-123456789abc", false, "admin6398" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "8f3a2c6e-1b4a-4f9c-9c3e-2a7d5e8b1f01", "5b7e1d9c-3a2f-4c8b-9e6d-1f0a2b3c4d55" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "8f3a2c6e-1b4a-4f9c-9c3e-2a7d5e8b1f01", "5b7e1d9c-3a2f-4c8b-9e6d-1f0a2b3c4d55" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5b7e1d9c-3a2f-4c8b-9e6d-1f0a2b3c4d55");
        }
    }
}
