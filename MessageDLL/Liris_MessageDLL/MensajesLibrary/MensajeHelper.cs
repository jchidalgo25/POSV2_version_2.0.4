using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MensajesLibrary
{
    public static class MensajeHelper
    {
        /// <summary>
        /// Muestra un mensaje de confirmación y devuelve un valor booleano según la respuesta del usuario.
        /// </summary>
        /// <param name="titulo">El título del mensaje</param>
        /// <param name="mensaje">El contenido del mensaje</param>
        /// <returns>true si el usuario presiona "Sí", false si presiona "No"</returns>
        //public static bool ConfirmarAccion(string titulo, string mensaje)
        //{

        //    //DialogResult resultado = MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        //    //return resultado == DialogResult.Yes;


        //}

        /// <summary>
        /// Muestra un mensaje informativo y devuelve la respuesta del usuario (Aceptar/Cancelar).
        /// </summary>
        /// <param name="titulo">El título del mensaje</param>
        /// <param name="mensaje">El contenido del mensaje</param>
        /// <returns>Devuelve el valor de DialogResult (OK o Cancel)</returns>
        public static DialogResult MostrarInformacion(string titulo, string mensaje)
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Muestra un mensaje de advertencia personalizado y devuelve la respuesta del usuario.
        /// </summary>
        /// <param name="titulo">El título del mensaje</param>
        /// <param name="mensaje">El contenido del mensaje</param>
        /// <returns>Devuelve DialogResult (Retry, Ignore, Abort)</returns>
        public static DialogResult MostrarAdvertencia(string titulo, string mensaje)
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
        }
    }
}

