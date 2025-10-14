using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Mail
{
    public class MailModel
    {
        //Titulo
        public string Subject { get; set; }

        //Desde
        public string From { get; set; }

        //Destino
        public string To { get; set; }

        //Con copia
        public string CC { get; set; }

        //Copia oculta
        public string CCO { get; set; }
    }
}
