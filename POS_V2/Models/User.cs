using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class User
    {
        public bool result { get; set; }
        public string username { get; set; }
        public string nombres { get; set; }
        public string mensaje { get; set; }
        public bool isSuperUser { get; set; }
    }
}
