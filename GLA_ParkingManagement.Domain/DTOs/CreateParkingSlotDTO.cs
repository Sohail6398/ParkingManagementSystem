using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.DTOs
{
    public class CreateParkingSlotDTO
    {
        public int Id { get; set; }

        [Required]
        public string SlotNumber { get; set; }

        [Required]
        public int VehicleTypeId { get; set; }
    }
}
