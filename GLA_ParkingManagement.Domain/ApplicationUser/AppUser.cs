using GLA_ParkingManagement.Domain.Entities;
using GLA_ParkingManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.ApplicationUser
{
    public class AppUser:IdentityUser
    {
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public GenderType Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation Property
        public ICollection<ParkingRecord> ParkingRecords { get; set; }
    }
}
