using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.finger
{
    class respuestaFinger
    {
        public int CodError { get; set; }
        public string MsjError { get; set; }
        public DPFP.Gui.EventHandlerStatus Status { get; set; }

    }
}
