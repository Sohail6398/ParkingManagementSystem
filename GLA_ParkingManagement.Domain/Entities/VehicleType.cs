using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.Entities
{
    public class VehicleType
    {
        public int Id { get; set; }
        public string Name { get; set; } // Car, Bike, Truck
        public decimal HourlyRate { get; set; }
        public ICollection<ParkingSlot> ParkingSlots { get; set; }
    }
}
