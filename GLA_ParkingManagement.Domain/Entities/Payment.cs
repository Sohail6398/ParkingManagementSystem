using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLA_ParkingManagement.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int ParkingRecordId { get; set; }
        public ParkingRecord ParkingRecord { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        public string PaymentMethod { get; set; } // Cash, Online
    }
}
