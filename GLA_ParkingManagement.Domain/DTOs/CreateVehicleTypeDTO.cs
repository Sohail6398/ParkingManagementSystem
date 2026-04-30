using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.DTOs
{
    public class CreateVehicleTypeDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vehicle type name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(1, 10000, ErrorMessage = "Rate must be greater than 0")]
        public decimal HourlyRate { get; set; }
    }
}
