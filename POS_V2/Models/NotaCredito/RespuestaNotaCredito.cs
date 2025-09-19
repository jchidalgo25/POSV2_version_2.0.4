using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.NotaCredito
{
    class RespuestaNotaCredito
    {
        public int CodigoRespuesta { get; set; }
        public string MensajeRespuesta { get; set; }
        public Exception exception { get; set; }
        public string StackTrace { get; set; }
        public string ticketNotaCredit { get; set; }
        public string tickeGiftCard { get; set; }
    }
}
