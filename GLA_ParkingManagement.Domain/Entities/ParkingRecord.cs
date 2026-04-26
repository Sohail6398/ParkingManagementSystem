using GLA_ParkingManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.Entities
{
    public class ParkingRecord
    {
        public int Id { get; set; }

        public string VehicleNumber { get; set; }

        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; }

        public int SlotId { get; set; }
        public ParkingSlot Slot { get; set; }

        public DateTime EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }

        public double DurationHours { get; set; }
        public decimal TotalAmount { get; set; }

        public ParkingStatus Status { get; set; } // Active / Completed

        public string UserId { get; set; } // FK to AspNetUsers
    }
}
