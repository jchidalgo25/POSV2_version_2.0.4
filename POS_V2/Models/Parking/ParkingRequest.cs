using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Parking
{
    public class ParkingRequest
    {
        public string url { get; set; }
        public string full_name { get; set; }
        public string identification { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string email { get; set; }
        public decimal total_value { get; set; }
    }

   
}
