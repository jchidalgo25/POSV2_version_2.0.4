using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Mensajes
    {

        public int id { get; set; }
        public string opcion { get; set; }
        public string descripcion { get; set; }
        public string titulo_mensaje { get; set; }
        public string tipo_mensaje { get; set; }
        public int tiempo_espera { get; set; }
        public bool activo { get; set; }
        public DateTime fecha_creacion { get; set; }
        public DateTime fecha_modificacion { get; set; }

        public MensajesLibrary.MsgBoxCtrl.MessageType TipoMensaje { get; set; }
        public Liris_MenssageDLL.MsgBoxCtrl_v2.MessageType TipoMensaje2 { get; set; }


    }

    public class ParametrosMensajes {
        public string codigo { get; set; }
        public string valor { get; set; }
    }
}
