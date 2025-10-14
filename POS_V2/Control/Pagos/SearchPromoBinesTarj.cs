using POS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control;
namespace POS.Control.Clientes
{
    public partial class SearchPromoBinesTarj : Form
    {
        public string code;
        System.Windows.Forms.Control focused;        

        public core_descuento SelectedDescuento { get; set; }
        public pos_customer SelectedCustomer { get; set; }
        public string NumeroPedidoDomicilio { get; set; }        
        private string _numTarjeta;
        private string _tarjetaHabiente;
        private string _numbineTarjeta;


        public string BinTarjetaPromo {
            get { return _numbineTarjeta; }
            set { _numbineTarjeta = value; }
        }
        public decimal DescuentoPromTarjetaBines { get; set; }
        public bool AplicaPromoTarjBines { get; set; }
        string Cliente { get; set; }

        public SearchPromoBinesTarj()
        {
            Init();
        }

        public SearchPromoBinesTarj(string _clienteAccountNum)
        {
            Cliente = _clienteAccountNum;
            Init();
        }

        public SearchPromoBinesTarj(pos_customer previousSelectedCustomer)
        {
            SelectedCustomer = previousSelectedCustomer;
            Init();
        }

        private void Init()
        {
            InitializeComponent();            
            focused = (System.Windows.Forms.Control)txtNumTarjeta;
        }

     

        private void txtProduct_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
           // BuscaPedidoDomicilio();
            //SearchProductList();
        }

        public void EjecutaConsulta()
        {

            string CodigoRed;
            string codigoBin;
            int indice = 0;
            string numtarjetaTodo = string.Empty;
            bool separadorValidoNumeroTarjeta = true;

            try
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.SearchPromoBinesTarj", "txtNumTarjeta_KeyPress", "ejecuta metodo para consultar de promociones BINES" );
                numtarjetaTodo = txtNumTarjeta.Text;

                this._tarjetaHabiente = string.Empty;
                this._numTarjeta = string.Empty;

                var numeroTarjeta = txtNumTarjeta.Text.Trim().Replace("%B", "");
                numeroTarjeta = numeroTarjeta.Replace("\t", string.Empty);

                if (numeroTarjeta.Length >= 8)
                {
                    //' '
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.SearchPromoBinesTarj", "txtNumTarjeta_KeyPress", "numero de tarjeta leida: " + numeroTarjeta);
                    indice = numeroTarjeta.IndexOf('^');
                    if (indice < 0)
                    {
                        indice = numeroTarjeta.IndexOf('&');
                        if (indice < 0) //Validación para Tarjetas ALIA - CUOTAFACIL
                        {
                            indice = numeroTarjeta.IndexOf('=');//Tarjetas ALIA - CUOTAFACIL.
                        }

                    }
                    //Si no encuentra caracter separador, no seguir con la rutina y enviar mensaje.
                    if (indice < 0)
                    {
                        separadorValidoNumeroTarjeta = false;
                    }
                    //Validación de que tenga el separador para leer el número de Bin de la tarjeta. Si no existe validación de caracter para obtener el IndexOf, no puede leer el bin.
                    if (separadorValidoNumeroTarjeta)
                    {

                        this._numTarjeta = this.enmascararTarjeta(numeroTarjeta.Substring(0, indice));

                        indice += 1;
                        for (int i = indice; i < numeroTarjeta.Length; i++)
                        {
                            if (numeroTarjeta[i] == '-')
                            {
                                break;
                            }
                            if (numeroTarjeta[i] == '^')
                            {
                                break;
                            }
                            if (numeroTarjeta[i] == '&')
                            {
                                break;
                            }
                            if (numeroTarjeta[i] == '=')
                            {
                                break;
                            }
                            if (numeroTarjeta[i] == '=')
                            {
                                break;
                            }
                            this._tarjetaHabiente += numeroTarjeta[i];
                        }

                        if (ConsultaPromoTarjetaBines(_numTarjeta))
                        {
                            ConsultaTarjetaBines(_numTarjeta, numtarjetaTodo);
                        }


                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.SearchPromoBinesTarj", "txtNumTarjeta_KeyPress", "No pudo encontrar el caracter separador para obtener numero de Tarjeta '" + numeroTarjeta + "'");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.SearchPromoBinesTarj", "txtNumTarjeta_KeyPress", "obtener el caracter que utiliza la tarjeta de credito y agregar a la validación para obtener el indice. (Ejemplo: indice = numeroTarjeta.IndexOf('&');) ");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.SearchPromoBinesTarj", "txtNumTarjeta_KeyPress", "Numero Tarjeta: " + numtarjetaTodo);

                        if (ConsultaPromoTarjetaBines(_numTarjeta))
                        {
                            ConsultaTarjetaBines(_numTarjeta, numtarjetaTodo);
                        }
                        //txtNoTarjeta.Clear();

                    }

                    //txtNoTarjeta.SelectAll();
                }
            }
            catch (Exception exkp)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.SearchPromoBinesTarj", "txtNumTarjeta_KeyPress", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);
                //txtNoTarjeta.Clear();

            }
        }

        private void txtNumTarjeta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                EjecutaConsulta();

            }
        }

        private void ConsultaTarjetaBines(string _numTarjeta, string numtarjetaTodo)
        {
            string CodigoRed;
            string codigoBin;
            int indice = 0;
            // string numtarjetaTodo = string.Empty;
            bool separadorValidoNumeroTarjeta = true;
            try
            {
                codigoBin = _numTarjeta.Substring(0, 6);
                using (var db = new POSEntities())
                {


                    var core_tarjetacredito_bin = db.core_tarjetacredito_bin.Where(x => x.bin == codigoBin).FirstOrDefault();
                    if (core_tarjetacredito_bin != null)
                    {

                        
                        CodigoRed = core_tarjetacredito_bin.bin_red == "2" ? "Medianet" : "Datafast";
                        lblMsjPromocion.Text = "Tarjeta Aplica promoción del 5% Descuento";
                        btnAceptar.Visible = true;
                        btnExit.Visible = false;
                        btnAceptar.Left = 200;
                        btnAceptar.Top = 170;
                        //btnAceptar.Focus();
                        BinTarjetaPromo = codigoBin;

                        Destacar("Aplica promoción del "+ DescuentoPromTarjetaBines.ToString() + "% Dscto");
                        btnAceptar.Focus();

                    }


                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "el bin de tarjeta '" + _numTarjeta + "' no fué encontrado: bin(" + codigoBin + ")");
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.BasePagos", "txtNoTarjeta_KeyPress", "Numero Tarjeta: " + numtarjetaTodo);                       
                       
                        //RecargaCombosPagoManual();
                        
                        //txtNoTarjeta.Clear();
                        MessageBox.Show("Tarjeta no pudo ser leida correctamente, favor pase de nuevo la tarjeta por el lector, si no lee, seleccione la tarjeta y tipo del listado", "Notificación Pago Manual", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.SearchPromoBinesTarj", "ConsultaTarjetaBines", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void Destacar(String Texto, int tipo = 0)
        {
            //El 1 < 2 determina el grado de parpadeo.
            if (tipo == 0)
            {
                for (int i = 0; i < 8; i++)
                {

                    //Ocultar el mensaje.
                    System.Threading.Thread.Sleep(130);
                    lblMsjPromocion.Text = string.Empty;
                    lblMsjPromocion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
                    Application.DoEvents();
                    //Fin de para ocultar el mensaje.

                    //Mostrar el mensaje.
                    System.Threading.Thread.Sleep(130);
                    lblMsjPromocion.Text = Texto;
                    Application.DoEvents();
                    //Fin de para mostrar el mensaje.

                }//Fin del for.    
            }
            if (tipo == 1)
            {
                for (int i = 0; i < 3; i++)
                {

                    //Ocultar el mensaje.
                    System.Threading.Thread.Sleep(100);
                    lblMsjPromocion.Text = string.Empty;
                    lblMsjPromocion.ForeColor = Color.Red;
                    Application.DoEvents();
                    //Fin de para ocultar el mensaje.

                    //Mostrar el mensaje.
                    System.Threading.Thread.Sleep(100);
                    lblMsjPromocion.Text = Texto;
                    Application.DoEvents();
                    //Fin de para mostrar el mensaje.

                }//Fin del for.    
            }
        }//Fin del metodo Destacar.

        private bool ConsultaPromoTarjetaBines(string _numTarjeta)
        {
            string codigoBin = string.Empty;
            decimal descuentoPromoTarjeta = 0;
            AplicaPromoTarjBines = false;
            try
            {
                codigoBin = _numTarjeta.Substring(0, 6);

                using (var db = new POSEntities())
                {

                    var core_descuento = db.core_descuento.Where(x => x.tipo_descuento == "bines_tarjetas" 
                    && x.parametro == Control.Common.GlobalParameters.Establecimiento 
                    && x.parametro2 == codigoBin && x.rango_fecha 
                    && (x.fecha_desde <= DateTime.Now && x.fecha_hasta >= DateTime.Now)).FirstOrDefault();
                    if (core_descuento != null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "POS.Control.Pagos.SearchPromoBinesTarj", "ConsultaTarjetaBines", " APLICA PROMO TARJETA BINES :  " + core_descuento.valor.ToString() + "  bin: " + codigoBin + " #TARJETA: " + _numTarjeta);
                        descuentoPromoTarjeta = core_descuento.valor;
                        DescuentoPromTarjetaBines = descuentoPromoTarjeta;
                        if (descuentoPromoTarjeta > 0)
                        {
                            AplicaPromoTarjBines = true;
                        }
                    }

                    if (!AplicaPromoTarjBines)
                    {
                        Destacar("No se encontró promoción activa con esta tarjeta.",1);
                        btnExit.Focus();
                    }
                    
                }
            }
            catch (Exception ex)
            {

                AplicaPromoTarjBines = false; ;
            }

            return AplicaPromoTarjBines;
        }
        private string enmascararTarjeta(string numTarjeta)
        {
            string numeroTarjeta = "";
            try
            {


                for (int i = 0; i < numTarjeta.Length; i++)
                {
                    if (i >= 6 && i < numTarjeta.Length - 3)
                    {
                        numeroTarjeta += 'X';
                    }
                    else
                    {
                        numeroTarjeta += numTarjeta[i];
                    }

                }
            }
            catch (Exception exkp)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.BasePagos", "enmascararTarjeta", "Se presentaron novedades durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exkp), "StackTrace: " + exkp.StackTrace);
            }

            return numeroTarjeta;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnKb_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();
        }

        private void SearchPromoBinesTarj_Load(object sender, EventArgs e)
        {
            AplicaPromoTarjBines = false;
            txtNumTarjeta.Focus();
        }

        private void txtNumTarjeta_Enter(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtNumTarjeta.Text))
            {
                txtNumTarjeta.Focus();
            }
        }

    

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            EjecutaConsulta();
                //this.Close();
        }


        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void txtNumTarjeta_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
