using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.DTOs
{
    public class ParkingSlotDTO
    {
        public int Id { get; set; }
        public string SlotNumber { get; set; }
        public int VehicleTypeId { get; set; }
        public string VehicleTypeName { get; set; }
        public bool IsOccupied { get; set; }
    }
}
