using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.DTOs
{
    public class ParkingRecordDTO
    {
        public int Id { get; set; }

        public string VehicleNumber { get; set; }

        public string VehicleType { get; set; }   // Name (Car, Bike)

        public string SlotNumber { get; set; }

        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public double DurationHours { get; set; }
        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public string UserId { get; set; }
    }
}
