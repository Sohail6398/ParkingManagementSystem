using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.DTOs
{
    public class VehicleTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal HourlyRate { get; set; }
    }
}
