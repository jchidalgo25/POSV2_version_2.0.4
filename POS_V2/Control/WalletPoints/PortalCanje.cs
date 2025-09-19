using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;

namespace POS.Control.WalletPoints
{
    public partial class PortalCanje : Form
    {
        private string IdentificacionCliente { get; set; }
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();

        public PortalCanje(string identificacion)
        {
            InitializeComponent();
            IdentificacionCliente = identificacion;
        }

        private void PortalCanje_Load(object sender, EventArgs e)
        {
            NavegarPortal();
        }

        private void NavegarPortal()
        {
            try
            {
                DateTime fecha = DateTime.Now;
                string tipoIdentificacion = GetTypeClient();

                if (tipoIdentificacion != string.Empty)
                {
                    bool clienteTieneMonedero = false;
                    string urlPortal = string.Empty;
                    using (POSEntities db = new POSEntities())
                    {
                        clienteTieneMonedero = db.TblPuntosCab.Any(x => x.AccountNum == IdentificacionCliente 
                                                                        && 
                                                                        x.Estado == 1);

                        urlPortal = db.core_parametro.Where(x => x.identificador == "WALLETPOINTS_URLPORTALCANJE").FirstOrDefault().valor;
                    }

                    if (string.IsNullOrWhiteSpace(urlPortal)) throw new Exception("No se pudo obtener url valida de WALLETPOINTS_URLPORTALCANJE en core_parametro, compruebe que parametro exista y tenga una url valida en campo valor");

                    if (clienteTieneMonedero)
                    {
                        /*
                            1.	Cedula Cajero
                            2.	Establecimiento
                            3.	Punto Emisión
                            4.	Token de Sesión (FECHAHORSEGUNDO)
                            5.	Tipo de identificación  del cliente (C, R,P)
                            6.	Identificación Cliente
                            7.	Tipo integración enviar siempre “01”
                            8.	Hashing de la trama

                            La identificación del cliente y del cajero  debe venir con un 
                            máximo de 13 caracteres, si es cedula debe venir con 10 dígitos 
                            más 3 espacios a la derecha
                            */
                        string postData = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                Control.Common.GlobalParameters.UserObj.username.PadRight(13, ' '),
                                Control.Common.GlobalParameters.Establecimiento,
                                Control.Common.GlobalParameters.PuntoEmision,
                                fecha.Year.ToString() + fecha.Month.ToString().PadLeft(2, '0') + fecha.Day.ToString().PadLeft(2, '0') + fecha.Hour.ToString().PadLeft(2, '0') + fecha.Minute.ToString().PadLeft(2, '0') + fecha.Second.ToString().PadLeft(2, '0'),
                                tipoIdentificacion,
                                IdentificacionCliente.PadRight(13, ' '),
                                "01"
                            );

                        string hashCode = ObtenerHashing(postData);
                        hashCode = ToHexString(hashCode);
                        postData += hashCode;
                        System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                        byte[] bytes = encoding.GetBytes("DATOS=" + postData);
                        webBrowser1.Navigate(urlPortal, string.Empty, bytes, "Content-Type: application/x-www-form-urlencoded");

                        this.Opacity = 100;
                    }
                    else
                    {
                        this.Close();
                        //MessageBox.Show("Cliente no tiene monedero de puntos");
                        Control.Common.General.GetMensajeToList(558);

                    }
                }
            }
            catch (Exception ex)
            {
                this.Close();
                //MessageBox.Show("No fue posible abrir Portal de Canje en este momento, esto puede deberse a una breve interrupcion en la comunicacion. Por favor inténtelo en unos momentos");
                Control.Common.General.GetMensajeToList(559);

            }

        }

        public static string ToHexString(string str)
        {
            var sb = new StringBuilder();

            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
        }

        public string ObtenerHashing(string message)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] messageBytes = encoding.GetBytes(message);
            SHA256Managed sha256 = new SHA256Managed();
            return Convert.ToBase64String(sha256.ComputeHash(messageBytes));
        }

        private string GetTypeClient()
        {
            if (ValidarIdentificador.ValidarCedula(IdentificacionCliente))
            {
                return "C";
            }
            else if (ValidarIdentificador.ValidarRUCNatural(IdentificacionCliente))
            {
                return "R";
            }
            else if (ValidarIdentificador.ValidarPasaporte(IdentificacionCliente))
            {
                return "P";
            }
            else
            {
                //MessageBox.Show(this, "Identificación de cliente debe ser de persona natural");
                Control.Common.General.GetMensajeToList(560);

                return string.Empty;
            }
        }


        private void btnKbd_Click(object sender, EventArgs e)
        {

            Control.Common.General.TecladoPantalla();
            
        }

    
        private void GarancheckForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("tabtip");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }
            procs = Process.GetProcessesByName("osk");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
