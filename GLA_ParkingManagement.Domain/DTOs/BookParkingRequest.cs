using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.DTOs
{
    public class BookParkingRequest
    {
        public string VehicleNumber { get; set; }
        public int VehicleTypeId { get; set; }
        public int SlotId { get; set; }
    }
}
