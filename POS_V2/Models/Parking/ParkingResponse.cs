using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Parking
{
    public class ParkingResponse
    {
        
            public string _id { get; set; }
            public string _rev { get; set; }
            public string address { get; set; }
            public IList<Bitacora_list> bitacora_list { get; set; }
            public DateTime date_entry { get; set; }
            public DateTime date_out { get; set; }
            public string email { get; set; }
            public string full_name { get; set; }
            public string identification { get; set; }
            public IList<string> local_path { get; set; }
            public string phone { get; set; }
            public string status { get; set; }
            public double time_entry { get; set; }
            public double time_out { get; set; }
            public double total_sec { get; set; }
            public string try_out { get; set; }
            public string url { get; set; }
            public IList<string> w3_path { get; set; }
        
    }

    public class Bitacora_list
    {
        public DateTime fecha { get; set; }
        public string info { get; set; }

    }

   
}
