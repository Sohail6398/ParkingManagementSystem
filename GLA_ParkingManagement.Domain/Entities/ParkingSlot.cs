using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.Entities
{
    public class ParkingSlot
    {
        public int Id { get; set; }
        public string SlotNumber { get; set; }

        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; }

        public bool IsOccupied { get; set; }

        public ICollection<ParkingRecord> ParkingRecords { get; set; }
    }
}
