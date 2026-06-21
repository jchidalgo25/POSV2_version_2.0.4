using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using POS.Models;
using System.Runtime.InteropServices;
using Telerik.WinControls.UI;
using Trx.Messaging;
using POS.Control.PINPAD;
using System.Globalization;
using POS.Control.CajaPinpad.Modelo;


//using System.Data.Entity.Core.Objects;

namespace POS.Control.Pagos
{

    public partial class CreditoPavos : Form
    {
        int PUERTOCOM;
        string TextoPP ="";
        int EsManual;
        System.Windows.Forms.Control focused;
        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        public string Establecimiento;
        public string PtoEmision;
        public string Cajero;
        private DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();
        //KeyboardControl kbd ;

        public CreditoPavos()
        {
            InitializeComponent();
            btnAbonar.Enabled = false;
            cmbEleccion.Visible = false;
            EsManual = 0;

        }
        
        public void setCedula(string id)
        {
            txtIdentificacion.Text = id;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            BuscaPagosPaviPlan();
        }

        private void BuscaPagosPaviPlan()
        {
            valor = 0;
            abono = 0;

            POSEntities db = new POSEntities();

            if (txtIdentificacion.Text == "" || txtIdentificacion.Text.Length < 7)
            {
                //MessageBox.Show(this,"No ha ingresado una identificación o la longitud de identificación es incorrecta.");  

                Control.Common.General.GetMensajeToList(404);

            }
            else
            {  
                if (db.vw_CreditoLocalesCabecera.FirstOrDefault(x => x.CLIENTE == txtIdentificacion.Text && x.ESTADO != 2 && x.ENTREGADO != 1 && x.ANULADO == 0) == null)
                {
                    //MessageBox.Show(this,"El cliente no posee un PaviPLAN. Por favor, verifique la identificación.");
                    Control.Common.General.GetMensajeToList(405);
                }
                else
                {
                    try
                    {
                        var query = (from tran in db.vw_CreditoLocalesPagos
                                     where tran.CLIENTE == txtIdentificacion.Text && tran.CREATEDDATETIME.Year== DateTime.Now.Year
                                     orderby tran.SALDO descending
                                     select tran).ToList();

                        var qname = (from n in db.vw_CreditoLocalesCabecera
                                     where n.CLIENTE == txtIdentificacion.Text && n.ESTADO!=2 && n.ENTREGADO!=1 && n.ANULADO==0
                                     select n).FirstOrDefault();
                        
                        lblNombreCliente.Text = qname.CLIENTENOMBRE;
                        dgvConsultaSaldo.AutoGenerateColumns = false;
                        dgvConsultaSaldo.DataSource = query;
                        lblSaldo.Text = (Math.Round(qname.TOTAL - qname.ABONADO, 2)).ToString();
                        btnAbonar.Enabled = true;
                        valor = Math.Round(qname.TOTAL , 2);
                        abono = Math.Round(qname.ABONADO, 2);

                        SqlConnection conexion = new SqlConnection(Properties.Settings.Default.CONECTA_AX);
                        string Query = null;
                        SqlCommand comando = default(SqlCommand);
                        escomida = 0;
                        using (conexion)
                        {
                            conexion.Open();
                            Query = "Select C.Cliente,C.RecId,D.ItemId from TBLPAVIPLANCABECERA C inner join TBLPAVIPLANDETALLE D on C.Recid = D.RecRefid ";
                            Query += "where D.ItemId in ('PT-CP-020928','PT-CP-020929','PT-CP-020930','PT-CP-020931','PT-CP-020932','PT-CP-020933','PT-CP-020934') ";
                            Query += "and C.Cliente = '"+ txtIdentificacion.Text +"' and C.estado != 2 and C.entregado != 1 and C.anulado = 0 ";
                            comando = new SqlCommand(Query, conexion);
                            SqlDataReader dr = comando.ExecuteReader();
                            if (dr.HasRows)
                            {
                                escomida = 1;
                            }
                        }

                        if (abono==0 && escomida==0)
                        {                          
                            cmbFormaPago.SelectedIndex = 0;
                            cmbFormaPago.Enabled = false;
                        }
                        else
                        {
                            cmbFormaPago.Enabled = true;
                        }
                        if ((qname.TOTAL - qname.ABONADO) == 0)
                        {
                            //MessageBox.Show(this,"El cliente ya canceló su PaviPLAN");
                            Control.Common.General.GetMensajeToList(406);
                            btnAbonar.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {


                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[exception]", valor = ex.InnerException.Message.ToString() });
                        Control.Common.General.GetMensajeToList(407, parametros);

                        //MessageBox.Show(this,ex.InnerException.Message.ToString());

                    }
                }
            }
        }

        private void txtCantidadAbonar_KeyPress(object sender, KeyPressEventArgs e)
        {   
            //Only numbers
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            //Only decimal point
            if ((e.KeyChar == '.') && ((sender as RadTextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }       

        private void btnAbonar_Click(object sender, EventArgs e)
        {
            if (abono == 0 && escomida == 0)
            {
                if (decimal.Parse(txtCantidadAbonar.Text) < Decimal.Round((valor * 0.10M), 2))
                {
                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[minAbono]", valor = Decimal.Round((valor * 0.10M), 2).ToString() });
                    Control.Common.General.GetMensajeToList(407, parametros);

                    //MessageBox.Show(this,string.Format("El valor minimo a abonar es de : "+ Decimal.Round((valor * 0.10M),2)));
                    return;
                }
            }

            string Texto = "";
            POSEntities db = new POSEntities();

            var query = (from tran in db.core_establecimiento
                        where tran.establecimiento == Establecimiento
                        select tran).FirstOrDefault();            
            
            if (txtCantidadAbonar.Text != "" && txtCantidadAbonar.TextLength >= 1 && decimal.Parse(txtCantidadAbonar.Text) >= 0.01M)
            {
              
                if (cmbFormaPago.SelectedItem != null)
                {
                    switch(cmbFormaPago.SelectedItem.ToString())
                    {
                        case "Tarjeta Crédito":
                            if ((db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == Establecimiento).First().valor == "TRUE") && EsManual == 0)
                            {
                                TextoPP = string.Empty;

                                if (Control.Common.GlobalParameters.PINPAD_MULTIRED)
                                {
                                    ProcesaPinpadMultiRed();
                                }
                                else
                                {
                                    ProcesaPinpad();
                                }

                                if (!TextoPP.Contains("||"))
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CreditoPavos", "btnAbonar_Click", "Ocurrió una novedad durante el cobro pinpad por lo que se manda a finalizar el método, la variable 'TextoPP' contiene: " + (string.IsNullOrWhiteSpace(Texto) ? "No tiene valor" : TextoPP));

                                    //MessageBox.Show(this, "Abono por pinpad no pudo ser finalizado", "Paviplan");
                                    Control.Common.General.GetMensajeToList(417);
                                    return;
                                }
                                Texto = TextoPP;
                               

                            }
                            else
                            {
                                var objTar = cmbEleccion.SelectedValue as core_tarjetacredito;
                                Texto = objTar.nombre + "||" + cmbTipoPagoTarjeta.SelectedItem.ToString();
                            }

                            
                            break;
                        case "Cheque":
                            var objChe = cmbEleccion.SelectedValue as core_banco;
                            Texto = objChe.nombre+"-" + txtCuenta.Text +"-"+ txtNumCheque.Text;
                            break;
                    }
                    
//                    bool reg = RegistraPagoPaviPlan(txtIdentificacion.Text, query.almacen.ToString(), decimal.Parse(txtCantidadAbonar.Text), cmbFormaPago.SelectedItem.ToString(), cmbFormaPago.SelectedItem != "Efectivo" ? cmbEleccion.SelectedItem.ToString() : "Efectivo", Cajero);
                    bool reg = RegistraPagoPaviPlan(txtIdentificacion.Text, query.almacen.ToString(), decimal.Parse(txtCantidadAbonar.Text), cmbFormaPago.SelectedItem.ToString(), cmbFormaPago.SelectedItem.ToString() != "Efectivo" ? Texto : "Efectivo", Cajero);
                    //_factura.agregarPagoTarjetaCredito(valor, obj.core_banco.nombre, obj.nombre, obj.tipo, cmbTipoPagoTarjeta.SelectedItem.ToString());
                    System.Threading.Thread.Sleep(2000);
                    string errorString = string.Empty;
                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

                    if (reg)
                    {
                        try
                        {
                            btnAbonar.Enabled = false;
                            txtCantidadAbonar.Clear();
                        }
                        catch (Exception ex)
                        {
                            //MessageBox.Show(this,ex.Message.ToString(),"objetos");
                            errorString = ex.Message.ToString();

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = errorString });
                            Control.Common.General.GetMensajeToList(418, parametros);

                        }

                        try
                        {
                            BuscaPagosPaviPlan();                          
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(this,ex.Message.ToString(),"buscapaviplan");

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = errorString });
                            Control.Common.General.GetMensajeToList(419, parametros);

                        }
                        try
                        {
                            
                            Imprimir();                        
                        }
                        catch (Exception ex)
                        {

                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = errorString });
                            Control.Common.General.GetMensajeToList(420, parametros);

                            //MessageBox.Show(this,ex.Message.ToString(),"imprimir");
                        }
                    }
                    else
                    {
                        //MessageBox.Show(this,"Existió un problema en el pago. Por favor intente de nuevo.");
                        Control.Common.General.GetMensajeToList(421);

                    }
                }
                else
                {
                    //MessageBox.Show(this,"Tiene que elegir una forma de pago!");
                    Control.Common.General.GetMensajeToList(422);

                }         
            }
            else
            {
                //MessageBox.Show(this,"Tiene que ingresar una cantidad válida!");
                Control.Common.General.GetMensajeToList(423);
            }
        }

        public static bool RegistraPagoPaviPlan(string identificacion, string almacen, decimal valor, string forma_pago, string detalle,string cajero)
        {
            bool pago = false;
            List<ParametrosMensajes>  parametros = new List<ParametrosMensajes>();

            using (var servicio = new CreateCustomerLirisProduccion.Service1Client())
            {
                try
                {
                    pago = servicio.RegistraPagoPAVIPLAN(identificacion, almacen, float.Parse(valor.ToString()), forma_pago, detalle, cajero + "|" + Program.ID_Caja_POS);

                    if (pago == true)
                    {
                        //MessageBox.Show("El pago se realizó correctamente.","PaviPLAN",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        Control.Common.General.GetMensajeToList(424);
                    }
                    else
                    {
                        //MessageBox.Show("Existió un error generando el pago, por favor intente nuevamente");
                        Control.Common.General.GetMensajeToList(425);
                    }
                }
                catch (Exception ex)
                {
                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.Message.ToString() });
                    Control.Common.General.GetMensajeToList(426, parametros);
                    //MessageBox.Show("Error: " + ex.Message.ToString());                    
                }
                
                servicio.Close();
            }

            return pago;
        }

        private void addCL(DSS.Controles.Impresion.DSSPrint printer, StringBuilder s, string text)
        {
            s.AppendLine(text);
            if (printer.PrinterSettings.PrinterName.Contains("Generic"))
                s.Append("\n");
        }        

        private void Imprimir()
        {
            cmbFormaPago.Enabled = true;
            POSEntities db = new POSEntities();

            var query = (from tran in db.vw_CreditoLocalesPagos
                         where tran.CLIENTE == txtIdentificacion.Text && tran.CREATEDDATETIME.Year>2015
                         orderby tran.SALDO ascending
                         select tran).FirstOrDefault();

            var qname = (from n in db.vw_CreditoLocalesCabecera
                         where n.CLIENTE == txtIdentificacion.Text && n.FECHACONTRATO.Year>2015
                         select n).FirstOrDefault();

            var msjini = (from m in db.vw_CreditoLocalesMensajes
                          where m.TIPOMENSAJE == 0
                          select m).FirstOrDefault();

            var msjfin = (from o in db.vw_CreditoLocalesMensajes
                          where o.TIPOMENSAJE == 1
                          select o).FirstOrDefault();

            var msjrnd = (from r in db.vw_CreditoLocalesMensajes
                          where r.TIPOMENSAJE != 1 && r.TIPOMENSAJE != 0
                          orderby Guid.NewGuid()
                          select r).FirstOrDefault();

            printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 8, FontStyle.Bold);
            btnAbonar.Enabled = true;

            StringBuilder lineas_impresion = new StringBuilder();

            addCL(printer, lineas_impresion, " ");               
         
            addCL(printer, lineas_impresion, "            LIRIS S.A.");
            addCL(printer, lineas_impresion, "        RUC 0990865477001");
            addCL(printer, lineas_impresion, "      CONTRIBUYENTE ESPECIAL");
            addCL(printer, lineas_impresion, "=======================================");
            addCL(printer, lineas_impresion, "          RECIBO DE ABONO");
            addCL(printer, lineas_impresion, "=======================================");
            addCL(printer, lineas_impresion, "SUCURSAL: " + query.NOMBRELOCAL);
            addCL(printer, lineas_impresion, "FECHA   : " + query.FECHA.ToShortDateString());
            addCL(printer, lineas_impresion, " ");
            addCL(printer, lineas_impresion, "CLIENTE: " + qname.CLIENTENOMBRE);
            addCL(printer, lineas_impresion, "ABONO  : " + Math.Round(query.MONTO,2));
            addCL(printer, lineas_impresion, "SALDO  : " + Math.Round(query.SALDO,2));            
            addCL(printer, lineas_impresion, "=======================================");
            addCL(printer, lineas_impresion, "             MOVIMIENTOS");
            addCL(printer, lineas_impresion, "=======================================");
            addCL(printer, lineas_impresion, "   FECHA        ABONO         SALDO");
            foreach (var d in db.vw_CreditoLocalesPagos.Where(x=> x.CLIENTE == txtIdentificacion.Text).OrderByDescending(x=> x.SALDO))
            { 
                addCL(printer, lineas_impresion, String.Format("{0,-9} {1,10} {2,13}" , d.FECHA.ToShortDateString() , Math.Round(d.MONTO,2) , Math.Round(d.SALDO,2)));
            }
            addCL(printer, lineas_impresion, "=======================================");
            
            if (db.vw_CreditoLocalesCabecera.FirstOrDefault(x => x.ESTADO != 2 && x.CLIENTE == txtIdentificacion.Text) != null)
            {
                addCL(printer, lineas_impresion, "     " + msjrnd.MENSAJE);
            }

            printer.TextToPrint = lineas_impresion.ToString();
            printer.Print();
            if (db.vw_CreditoLocalesPagos.FirstOrDefault(x => x.SALDO == 0 && x.CLIENTE == txtIdentificacion.Text) != null)
            {
                decimal total_giftcard = 0M;
                //total_giftcard = Math.Round(qname.TOTAL * 0.1M, 2);
                total_giftcard = -1M;

                addCL(printer, lineas_impresion, "      Puedes retirar tu Pavo en");
                addCL(printer, lineas_impresion, "         Local : " + qname.NOMBRELOCAL);
                addCL(printer, lineas_impresion, "         El día: " + qname.FECHAENTREGA.ToShortDateString());
                addCL(printer, lineas_impresion, "=======================================");
                if (escomida == 0)
                {
                    addCL(printer, lineas_impresion, "<footer>" + " *** TARJETA DESCUENTO ***" + "</footer>");
                    addCL(printer, lineas_impresion, " ");
                    addCL(printer, lineas_impresion, "    Has ganado en tu proxima compra: ");
                    addCL(printer, lineas_impresion, "              10% DESCUENTO ");
                    //addCL(printer, lineas_impresion, " ");
                    //addCL(printer, lineas_impresion, "<footer>" + "      $" + total_giftcard + ""+"</footer>");
                    addCL(printer, lineas_impresion, " ");
                    addCL(printer, lineas_impresion, " Válidez del ticket: 6 meses ");
                    addCL(printer, lineas_impresion, " despues de la creacion");

                    decimal monto_giftcard;
                    monto_giftcard = -1M;
                    //monto_giftcard = Math.Round(qname.TOTAL * 0.1M, 2);

                    //Models.SP_GENERABARCODE_Result res;
                    //res = db.SP_GENERABARCODE(monto_giftcard).ToList().FirstOrDefault().ToString();

                    var res = db.SP_GENERABARCODE(monto_giftcard).ToList().FirstOrDefault();
                    string CODIGO = res.CODIGO;

                    string coded;
                    Barcode bc = new Barcode();
                    //coded = bc.encodeString(res.CODIGO);
                    coded = bc.encodeString(CODIGO);

                    addCL(printer, lineas_impresion, "<barcode>" + "           " + coded + "</barcode>");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "     ");
                    addCL(printer, lineas_impresion, "     ");
                    //addCL(printer, lineas_impresion, "            " + res.CODIGO);
                    addCL(printer, lineas_impresion, "            " + CODIGO);

                }
                addCL(printer, lineas_impresion, "     ");
                addCL(printer, lineas_impresion, " " + msjfin.MENSAJE);
                
                btnAbonar.Enabled = false;
            }

            addCL(printer, lineas_impresion, "     ");
            addCL(printer, lineas_impresion, "       Más Fresco, Más Cerca!");           
            printer.TextToPrint = lineas_impresion.ToString();
            printer.Print();        
        }
        
        private void txtIdentificacion_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Only numbers & Alpha
            if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) &&  !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter) {
                BuscaPagosPaviPlan();
            }
            
        }

        private void txtIdentificacion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnBuscar.PerformClick();
            }
        }

        private void txtCantidadAbonar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAbonar.PerformClick();
            }
        }

        private void cmbFormaPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnPagoManual.Visible = false;
            using (var db = new POSEntities())
            {
                if (cmbFormaPago.SelectedItem.ToString() == "Efectivo")
                {
                    lblEleccion.Visible = false;
                    cmbEleccion.Visible = false;
                    lblTipoPagoTarjeta.Visible = false;
                    cmbTipoPagoTarjeta.Visible = false;
                    pnl1.Visible = false;
                }
                if (cmbFormaPago.SelectedItem.ToString() == "Tarjeta Crédito")
                {

                    PUERTOCOM = db.core_puntoemision.Where(x => x.establecimiento_id == Establecimiento).FirstOrDefault().puerto_pinpad;
                    if ((db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == Establecimiento).First().valor == "TRUE"))
                    {
                        lblEleccion.Text = "";
                        lblTipoPagoTarjeta.Text = "";
                        cmbEleccion.Visible = false;
                        lblTipoPagoTarjeta.Visible = true;
                        cmbTipoPagoTarjeta.Visible = false;
                        pnl1.Visible = false;
                        btnPagoManual.Visible = true;
                        
                    }
                    else
                    {
                        btnPagoManual.Visible = false;
                        
                        lblEleccion.Text = "Seleccione Tarjeta:";
                        cmbEleccion.Visible = true;
                        lblTipoPagoTarjeta.Visible = true;
                        cmbTipoPagoTarjeta.Visible = true;
                        pnl1.Visible = false;
                        cmbEleccion.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
                        cmbEleccion.DisplayMember = "nombre_completo";
                    }



                }
                if (cmbFormaPago.SelectedItem.ToString() == "Cheque")
                {
                    lblEleccion.Text = "Seleccione Banco:";
                    cmbEleccion.Visible = true;
                    pnl1.Visible = true;
                    cmbEleccion.DataSource = db.core_banco.ToList();
                    cmbEleccion.DisplayMember = "nombre";
                    lblTipoPagoTarjeta.Visible = false;
                    cmbTipoPagoTarjeta.Visible = false;
                    /*
                    List<core_banco> bc = (from r in db.core_banco
                                                    orderby r.nombre
                                                    select r).ToList();

                    cmbEleccion.Items.Clear();
                    lblEleccion.Text = "Seleccione Banco:";
                    cmbEleccion.Visible = true;

                    for (int x = 0; x < bc.Count; x++)
                    {
                        cmbEleccion.Items.Add(bc[x].nombre);
                    }        */

                }
            }
        }
        private decimal valor = 0M;
        private decimal abono = 0M;
        private int     escomida=0;
        private void cmbEleccion_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbFormaPago.SelectedItem.ToString() == "Tarjeta Crédito")
            {
                btnPagoManual.Visible = true;
                var obj = cmbEleccion.SelectedValue as core_tarjetacredito;

                if (obj.operador == 1)
                    cmbTipoPagoTarjeta.SelectedIndex = 1;
                else
                    cmbTipoPagoTarjeta.SelectedIndex = 0;
            }
            else
            {
                btnPagoManual.Visible = false;
            }

        }

        private void btnKbd_Click(object sender, EventArgs e)
        {

            Control.Common.General.TecladoPantalla();

        }

        private void txtIdentificacion_Leave(object sender, EventArgs e)
        {
            focused = (System.Windows.Forms.Control)sender;
            //kbd.Close();
            //  AbreTeclado(sender,e);
        }

        private void ProcesaPinpad()
        {

            var pos = new POSEntities();

            if (pos.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == Establecimiento).First().valor == "TRUE")
            {
                ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
                int timeOutLT = 65000;
                int.TryParse(pos.core_parametro.Where(x => x.identificador == "LT_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutLT);

                string resultadolectura = envio.SendRequestPinpad("", PUERTOCOM, timeOutLT, "LT", "", 1);
                //   string resultadolectura = "LT0000475398XXXXXX7010         2111DA86EFF99EEF04E955AB5E81F6DF5C9B07F38CDDLECTURA OK          ";

                Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
                string valpag = Decimal.Round(Decimal.Parse(txtCantidadAbonar.Text)*1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";
                int leerTramaRes = 75;

                if (resultadolectura.Length > 98)
                {
                    leerTramaRes = leerTramaRes + 24;
                }

                if (resultadolectura.Substring(leerTramaRes, 10) == "LECTURA OK")
                {
                    var ctb = pos.core_tarjetacredito_bin.Where(x => x.bin == resultadolectura.Substring(6, 6)).FirstOrDefault();
                    if (ctb != null)
                    {
                        trama.codRed = ctb.bin_red;
                        if (ctb.bin_red == "2")
                        {
                            
                            trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == Establecimiento && x.punto_emision == PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                            trama.codDiferido = "00";//corriente  ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                        }
                        else
                        {
                            trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                            trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == Establecimiento && x.punto_emision == PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                            trama.codDiferido = "00";// ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Tarjeta no se encuentra en listado de bines");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(this, "Error en PINPAD");
                    return;
                }

                trama.TipoTransaccion = "01";// ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                trama.plazoDiferido = "0"; //cmbDiferido.Text;
                trama.mesGracia = "0";// cmbMesesGracia.Text;
                //filler 1spc


                //  MessageBox.Show(this,"Verificar el calculo para  armar los totales");
                //var porc_pago = 100;
                var base0 = decimal.Parse(txtCantidadAbonar.Text);
                //var base12 = 0;
                trama.montoTotalTransaccion = valpag;//12N 10N2D
                trama.montoBaseGravaIVa = decimal.Round((0), 2, MidpointRounding.AwayFromZero).ToString().Replace(".", "").PadLeft(12, '0'); //12N 10N2D
                trama.montoBaseNoGravaIVa = valpag;//12N 10N2D
                trama.impuestoIvaTransaccion = decimal.Round((0), 2, MidpointRounding.AwayFromZero).ToString().Replace(".", "").PadLeft(12, '0');//12N 10N2D              
                

                trama.impuestoServicioTransaccion = "";//12N 10N2D
                trama.popinaTransaccion = "";//12N 10N2D
                trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                trama.CID = Establecimiento + PtoEmision;//15 identificador de la caja 


                if (trama.TipoTransaccion == "03")
                {
                    //trama.numAutorizacion = txtnumAut.Text;//6N  solo anulaciones envia autorizacion compra original / resto blancos
                    //trama.secuencialTransaccion = txtSecuencial.Text.PadLeft(6, '0');//6N  -- Anulaciones enviar Secuencial / resto en cero
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CreditoPavos", "ProcesaPinpad", "Enviando requerimiento PINPAD, trama: " + trama.DevuelveTrama);

                envio = new ClsEnviaPinPadGeneral();
                int timeOutCP = 45000;
                int.TryParse(pos.core_parametro.Where(x => x.identificador == "CP_PINPAD_MEDIANET_TIMEOUT").FirstOrDefault().valor, out timeOutCP);
                //var pinpadResponse = envio.Envio_requerimientoPinpad("", PUERTOCOM, 45000, trama.DevuelveTrama, "", 1);
                var pinpadResponse = envio.SendRequestPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);
                resptrama.ObtieneDato = pinpadResponse;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CreditoPavos", "ProcesaPinpad", "Respuesta requerimiento PINPAD recibida: " + pinpadResponse);

                if ((resptrama.mensajeRespuesta.Trim() == "AUTORIZACION OK." || resptrama.mensajeRespuesta.Trim() == "APROBADA  TRANS." || resptrama.mensajeRespuesta.Trim() == "APROBADA") && resptrama.numAut.Trim() != "")
                {
                    //MessageBox.Show(this,"impresion de voucher");

                    String tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                    prepararVoucherTarjeta(tipovoucher, resptrama, trama);

                    prepararVoucherTarjeta(tipovoucher, resptrama, trama);

                    POS_VOUCHER pos_voucher = new POS_VOUCHER();
                    pos_voucher.TARJETA = resptrama.numTarTuncate.Trim().PadRight(19, ' ');// ("520081XXXXXX6017   "); //19 ;

                    //revisar
                    string codigoproceso = "000200";
                    //es 003000 cuando es transacciones con tarjeta de crédito, 001000 cuando es transacción de tarjeta de debito cuenta de ahorro y 002000 cuando es transacción de tarjeta de debito cuenta corriente.

                    pos_voucher.CODIGOPROCESO = codigoproceso;// ("000200"); //6 ;
                    //revisar

                    pos_voucher.FECHACONSUMO = resptrama.fechaTrans;// ("20161122"); //8 ;
                    pos_voucher.HORACONSUMO = resptrama.horaTrans;// ("114339"); //6 ;
                    pos_voucher.NUMEROVOUCHER = resptrama.secuencialtransaccion;// ("000002");//6 ;

                    pos_voucher.AUTORIZACION = trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut; //6 ;
                    pos_voucher.ANULADO = trama.TipoTransaccion == "03" ? true : false;

                    pos_voucher.VALORCONSUMO = trama.montoTotalTransaccion.PadLeft(13, '0');// ("0000000001200"); //13 ;
                    pos_voucher.FORMAAUTORIZA = ("1"); //1 ;
                    if (resptrama.codigoRed == "02")
                        pos_voucher.TIPOCONSUMO = "PE";// ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                    else
                        pos_voucher.TIPOCONSUMO = "00";// ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2 ;
                    pos_voucher.PLAZO = trama.plazoDiferido.PadLeft(2, '0');// ("06"); //2 ;

                    pos_voucher.TIPOLECTURA = resptrama.modoLectura.PadLeft(3, '0');// ("005"); //3 ;
                    pos_voucher.TIPOMONEDA = ("840"); //3 ;
                    pos_voucher.VALORIVA = trama.impuestoIvaTransaccion.PadLeft(13, '0');// ("0000000000147"); //13 ;
                    pos_voucher.VALORSERVICIO = ("0000000000000"); //13 ;
                    pos_voucher.VALORPROPINA = trama.popinaTransaccion.PadLeft(13, '0');// ("0000000000000"); //13 ;
                    pos_voucher.VALORINTERES = resptrama.valInteres.Trim().PadLeft(13, '0');// ("0000000000057");  //13 ;
                    pos_voucher.VALORFIJO = resptrama.montoFijo.Trim().PadLeft(13, '0');// ("0000000000000");  //13 ;
                    pos_voucher.TIPOPROMOCION = ("00"); //2 ;
                    pos_voucher.MESESGRACIA = trama.mesGracia.PadLeft(2, '0');//                        ("00");  //2 ;
                    pos_voucher.EMPRESASERVICIO = ("0000");  //4 ;
                    pos_voucher.ESTADOTRX = (pos_voucher.TIPOCONSUMO == "01" ? "O" : "R"); //1 ;
                    pos_voucher.CODIGORESPUESTA = resptrama.codigoRespuesta;// ("00"); //2 ;
                    pos_voucher.TIPODISPOSITIVO = ("2"); //1 ;
                    pos_voucher.ADQUIRENTETARJETA = ("CREDIMATIC01"); //12 ;
                    pos_voucher.ADQUIRENTESERVICIO = ("            ");  //12 ;
                    pos_voucher.MONTOGRAVAIVA = trama.montoBaseGravaIVa.PadLeft(13, '0');// ("0000000001053"); //13 ;
                    pos_voucher.MONTONOGRAVAIVA = trama.montoBaseNoGravaIVa.PadLeft(13, '0');// ("0000000000000");  //13 ;
                    pos_voucher.PUNTOEMISION = Establecimiento + PtoEmision;
                    pos_voucher.PROCESADO = false;
                    pos_voucher.GRUPOTAR = resptrama.nomGruTar;
                    TextoPP = resptrama.nomGruTar + "||" + (resptrama.codigoRed == "02" ? "MEDIANET" : "DATAFAST"); 

                    pos_voucher.AUTORIZADOR = int.Parse(resptrama.codigoRed);
                    pos_voucher.LOTE = resptrama.numerolote;
                    pos_voucher.FACTURA = "PAVIPLAN"; //_factura.getNumeroFactura();

                    bool ExisteVoucher = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == pos_voucher.FECHACONSUMO 
                                            && x.VALORCONSUMO == pos_voucher.VALORCONSUMO 
                                            && x.AUTORIZACION == pos_voucher.AUTORIZACION 
                                            && x.NUMEROVOUCHER == pos_voucher.NUMEROVOUCHER 
                                            && x.FACTURA == pos_voucher.FACTURA);
                    if (!ExisteVoucher)
                    {
                        pos.POS_VOUCHER.Add(pos_voucher);
                    }
                    // pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet += 1;
                    pos.SaveChanges();

                    //Agregar lineas de insert
                    Control.Common.Logger.Agregar_Trace_Voucher(pos_voucher);

                    this.Close();
                }
                else
                {
                    //MessageBox.Show(this, "Error :" + resptrama.mensajeRespuesta);
                    //                    MessageBox.Show(this,"Error :" + txtResult.Text.Substring(8, 16));

                    string msgerror = "Error : " + resptrama.mensajeRespuesta;

                    //Enviar trama de reverso de transaccion tipo 04.   JM  25-08-2020
                    envio = new ClsEnviaPinPadGeneral();
                    trama.TipoTransaccion = "04";
                    pinpadResponse = envio.SendRequestPinpad("", PUERTOCOM, timeOutCP, trama.DevuelveTrama, "", 1);                                       
                    resptrama.ObtieneDato = pinpadResponse;
                    msgerror = msgerror + " \n" + "Reverso : " + resptrama.mensajeRespuesta;
                    MessageBox.Show(this, msgerror);
                }

            }

        }
        private void ProcesaPinpadMultiRed()
        {

            var pos = new POSEntities();
            string bin_descripcion = string.Empty;

            if (pos.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == Establecimiento).First().valor == "TRUE")
            {
                ClsEnviaPinPadGeneral envio = new ClsEnviaPinPadGeneral();
                PinPadRespuesta resultadolectura = new PinPadRespuesta();
                ClsEnviaPinPadGeneral envioGen = new ClsEnviaPinPadGeneral();
                Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
                int timeOutCP = Control.Common.GlobalParameters.ConectContingente.TiempoOutCP;

                Control.Common.General.ValidaContingente();
                string IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                int PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;
                resultadolectura = envioGen.LecturaTarjeta(IPPinPad, PuertoPinPad, 65000, "LT", "", 1);
                string repsuestaPinpad = string.Empty;

                string valpag = Decimal.Round(Decimal.Parse(txtCantidadAbonar.Text) * 1.00M, 2).ToString().Replace(".", "").PadLeft(12, '0'); //"000000000000";
                string autorizador = string.Empty;
                

                if (resultadolectura.CodigoRespuesta == "00")
                {
                    int leerTramaRes = 75;
                    if (resultadolectura.TramaRespuesta.Length > 98)
                    {
                        leerTramaRes = leerTramaRes + 24;
                    }

                    var ConsultaBin = pos.core_tarjetacredito_bin.Where(x => x.bin == resultadolectura.NumBin).FirstOrDefault();
                    if (ConsultaBin != null)
                    {
                        bin_descripcion = ConsultaBin.bin_descripcion;
                        autorizador = Control.Common.GlobalParameters.ConectContingente.Autorizador.ToString();
                        trama.MID = string.Empty;
                        trama.TID = string.Empty;


                        trama.codDiferido = "00";
                        if (!Control.Common.GlobalParameters.ConectContingente.PinPadContingente)
                        {
                            autorizador = Control.Common.GlobalParameters.AutorizadorDefault.ToString();
                            Control.Common.GlobalParameters.ConectContingente.Autorizador = Control.Common.GlobalParameters.AutorizadorDefault;
                        }
                        else
                        {
                            autorizador = "1";
                            Control.Common.GlobalParameters.ConectContingente.Autorizador = 1;
                        }

                        trama.codRed = autorizador;
                        trama.MID = Control.Common.GlobalParameters.MID_DATAFAST;
                        trama.TID = Control.Common.GlobalParameters.TID_DATAFAST;

                        if (Control.Common.GlobalParameters.ConectContingente.Autorizador == 2)
                        {
                            IPPinPad = Control.Common.GlobalParameters.ConectContingente.IpPinPadMEDIANET;
                            PuertoPinPad = Control.Common.GlobalParameters.ConectContingente.PuertoPinPadMEDIANET;

                            trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                            trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;
                        }

                        if (!Control.Common.GlobalParameters.EstTcpIpPinpad)
                        {
                            // ResponseBackground = Common.GlobalParameters.PinpadMsjAutorizadorNoValido;
                            Control.Common.General.GetMensajeToList(411);
                            return;
                        }
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(410);
                    }
                }
                else {
                    
                   
                    repsuestaPinpad = "Error en PINPAD: " + resultadolectura.CodigoRespuesta + " - " + resultadolectura.MensajeRespuesta + "\n\n";
                    repsuestaPinpad = repsuestaPinpad + " IP PinPad: " + IPPinPad + "; PuertoPinPad: " + PuertoPinPad;
                    repsuestaPinpad = repsuestaPinpad + " GlobalParameters.IPPinPad: " + Control.Common.GlobalParameters.IPPinPad + "\n\n";
                    repsuestaPinpad = repsuestaPinpad + " GlobalParameters.PuertoPinPad: " + Control.Common.GlobalParameters.PuertoPinPad + "\n\n";


                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception]", valor = repsuestaPinpad });
                    Control.Common.General.GetMensajeToList(412, parametros);
                    return;
                }

                //string plazoDiferido = "0";
                //string mesGracia = "0";
                var base0 = decimal.Parse(txtCantidadAbonar.Text);

                trama.montoTotalTransaccion = valpag;//12N 10N2D
                trama.montoBaseGravaIVa = decimal.Round((0), 2, MidpointRounding.AwayFromZero).ToString().Replace(".", "").PadLeft(12, '0'); //12N 10N2D
                trama.montoBaseNoGravaIVa = valpag;//12N 10N2D
                trama.impuestoIvaTransaccion = decimal.Round((0), 2, MidpointRounding.AwayFromZero).ToString().Replace(".", "").PadLeft(12, '0');//12N 10N2D              
                trama.impuestoServicioTransaccion = "";//12N 10N2D
                trama.popinaTransaccion = "";//12N 10N2D
                trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                trama.CID = "LIRISCID0" + Establecimiento + PtoEmision;//15 identificador de la caja 

                trama.TipoTransaccion = "01";// ((pos_tarjeta_transaccion)cmbTipoTransaccion.SelectedValue).idTipo;
                trama.plazoDiferido = "0"; //cmbDiferido.Text;
                trama.mesGracia = "0";// cmbMesesGracia.Text;
               
                trama.montoTotalTransaccion = valpag;//12N 10N2D
                trama.montoBaseGravaIVa = decimal.Round((0), 2, MidpointRounding.AwayFromZero).ToString().Replace(".", "").PadLeft(12, '0'); //12N 10N2D
                trama.montoBaseNoGravaIVa = valpag;//12N 10N2D
                trama.impuestoIvaTransaccion = decimal.Round((0), 2, MidpointRounding.AwayFromZero).ToString().Replace(".", "").PadLeft(12, '0');//12N 10N2D              

                trama.impuestoServicioTransaccion = "";//12N 10N2D
                trama.popinaTransaccion = "";//12N 10N2D
                trama.montoFijo = "";//12N 10N2D -- Solo trans anulac gasolineras
                trama.secuencialTransaccion = "";// pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().secuencia_broadnet.ToString();//6N  -- Anulaciones enviar Secuencial / resto en cero
                trama.horaTransccion = DateTime.Now.ToString("HHmmss");//HHMMSS
                trama.fechaTransaccion = DateTime.Now.ToString("yyyyMMdd"); ;//AAAAMMDD
                trama.numAutorizacion = "";//6N  solo anulaciones envia autorizacion compra original / resto blancos
                
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CreditoPavos", "ProcesaPinpadMultiRed", "Enviando requerimiento PINPAD, trama: " + trama.DevuelveTrama);

                var envioGenResponse = new ClsEnviaPinPadGeneral();
                ClsEnviaPinPadGeneral objContingente = new ClsEnviaPinPadGeneral();
                PinPadRespuesta PagoResp = new PinPadRespuesta();
                string strTrama = string.Empty;


                trama.TipoMensaje = "PP";
                strTrama = trama.DevuelveTrama.ToString();
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CreditoPavos", "ProcesaPinpadMultiRed", "Enviando requerimiento PINPAD, trama: " + strTrama);

                /*Se ejecuta el cobro por tarjeta. Se envia parametro*/
                PagoResp = new PinPadRespuesta();
                PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, trama.codRed, strTrama, "", 1, "PP");

                repsuestaPinpad = string.Empty;
                if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                {
                    if (PagoResp.CodigoRespuestaEntidad == "TO" || PagoResp.CodigoRespuestaEntidad == "20" || PagoResp.CodigoRespuestaEntidad == "91" || PagoResp.CodigoRespuestaEntidad == "96" || PagoResp.CodigoRespuestaEntidad == "09")
                    {
                        PagoResp.MensajeRespuesta = "Fuera de Linea / TimeOut ";
                        Control.Common.GlobalParameters.ConectContingente.ValidaRedPinPad = true;
                        DateTime FechaInicioEspera = DateTime.Now;
                        DateTime FechaFinEspera = FechaInicioEspera.AddMinutes(Control.Common.GlobalParameters.ConectContingente.TiempoEsperaContingente);
                        Control.Common.GlobalParameters.ConectContingente.FechaInicioEspera = FechaInicioEspera;
                        Control.Common.GlobalParameters.ConectContingente.FechaFinEspera = FechaFinEspera;
                        Control.Common.GlobalParameters.ConectContingente.PinPadContingente = true;

                        switch (autorizador)
                        {
                            case "1":
                                autorizador = "2";
                                trama.codRed = autorizador;
                                trama.MID = Control.Common.GlobalParameters.MID_MEDIANET;
                                trama.TID = Control.Common.GlobalParameters.TID_MEDIANET;
                                break;
                            case "2":

                                autorizador = "1";
                                trama.codRed = autorizador;
                                trama.MID = Control.Common.GlobalParameters.MID_DATAFAST;
                                trama.TID = Control.Common.GlobalParameters.TID_DATAFAST;
                                break;
                        }

                        Control.Common.GlobalParameters.ConectContingente.CodigoAutorizador = Int32.Parse(autorizador);
                        Control.Common.GlobalParameters.ConectContingente.Autorizador = Int32.Parse(autorizador);

                        PagoResp = new PinPadRespuesta();
                        objContingente = new ClsEnviaPinPadGeneral();

                        string strTramaCont = trama.DevuelveTrama.ToString();

                        PagoResp = new PinPadRespuesta();
                        PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, autorizador, strTramaCont, "", 1, "PP");
                        

                        if (PagoResp.CodigoRespuesta != "00" || PagoResp.CodigoRespuestaEntidad != "00")
                        {
                            repsuestaPinpad = "Error : " + PagoResp.MensajeRespuesta;
                            envioGen = new ClsEnviaPinPadGeneral();

                            trama.TipoTransaccion = "04"; //Reversos de Transacciones de Compras Corrientes y Diferidos
                            strTramaCont = trama.DevuelveTrama.ToString();

                            PagoResp = new PinPadRespuesta();
                            PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, trama.codRed, strTramaCont, "", 1, "PP");

                            resptrama.ObtieneDato = PagoResp.TramaRespuesta;
                            repsuestaPinpad = repsuestaPinpad + " \n" + "Reverso : " + PagoResp.MensajeRespuesta;


                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[exception]", valor = repsuestaPinpad });
                            Control.Common.General.GetMensajeToList(413, parametros);
                            
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "CreditoPavos", "ProcesaPinpadMultiRed", "Reverso de transacción no exitosa, envio: " + trama.DevuelveTrama);
                            Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "CreditoPavos", "ProcesaPinpadMultiRed", "Reverso de transacción no exitosa, respuesta : " + resptrama.mensajeRespuesta);
                            return;
                        }
                    }
                    else
                    {
                        string Respuesta = string.Empty;

                        switch (PagoResp.CodigoRespuestaEntidad)
                        {
                            case "03": Respuesta = "Establecimiento Invalido."; break;
                        }

                        repsuestaPinpad = "Error : " + PagoResp.MensajeRespuesta;
                        envioGen = new ClsEnviaPinPadGeneral();

                        trama.TipoTransaccion = "04"; //Reversos de Transacciones de Compras Corrientes y Diferidos
                        PagoResp = new PinPadRespuesta();
                        PagoResp = envioGenResponse.ObtenerTramaPinPad(IPPinPad, PuertoPinPad, timeOutCP, trama.codRed, strTrama, "", 1, "PP");
                        
                        resptrama.ObtieneDato = PagoResp.TramaRespuesta;
                        repsuestaPinpad = repsuestaPinpad + " \n" + "Reverso : " + PagoResp.MensajeRespuesta;

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[exception]", valor = repsuestaPinpad });
                        Control.Common.General.GetMensajeToList(414, parametros);

                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Reverso de transacción no exitosa, envio: " + trama.DevuelveTrama);
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Reverso de transacción no exitosa, respuesta : " + resptrama.mensajeRespuesta);

                        return;

                    }

                }



                string tipovoucher = Control.Common.GlobalParameters.ComprobanteVoucherTarjetaCredito;
                prepararVoucherTarjeta(tipovoucher, resptrama, trama);

                prepararVoucherTarjeta(tipovoucher, resptrama, trama);


                string RetornaRespuesta = PagoResp.MensajeRespuesta.Trim();
                RepuestaPago Pago = PagoResp.GetDatosPago(PagoResp.TramaRespuesta, autorizador);

                if (Pago.CodRespMsj != "00")
                {

                    repsuestaPinpad = Pago.MsjRespMsjAut;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Error al generar el voucher / " + Pago.MsjRespMsjAut);

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception]", valor = repsuestaPinpad });
                    Control.Common.General.GetMensajeToList(415, parametros);

                    return;
                }

                Pago.Valor = valor;
                
                resptrama.ObtieneDato = PagoResp.TramaRespuesta;
                BasePagos basePagos = new BasePagos();

                RespuestaVoucher objGeneraVoucher = basePagos.GeneraVoucher(Pago, trama, resptrama, bin_descripcion);

                if (objGeneraVoucher.CodigoRespuesta != "0")
                {
                    string msjSeImprimieronVouchers = string.IsNullOrWhiteSpace(objGeneraVoucher.printWarnings) ? " y se imprimieron los recibos," : ", a pesar de que no se pudo imprimir los recibos en el momento,";
                    string msjParaDevteam = "En el POS del siguiente punto de emision, si bien se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers
                                          + " el voucher no pudo ser grabado en nuestra base interna. Generar el registro en POS_VOUCHER inmediatamente"
                                          + " pues puede provocar descuadres en los cierres. Esto pudo deberse a un breve inconveniente,"
                                          + " se recomienda una vez generado el registro, verificar la causa de la novedad";

                    repsuestaPinpad = "Error : " + resptrama.mensajeRespuesta;

                    string TIPOCONSUMO = string.Empty;
                    if (resptrama.codigoRed == "02") 
                        TIPOCONSUMO = "PE";// ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                    else
                        TIPOCONSUMO = "00";// ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2


                    string datosVoucher = "\nTARJETA: " + resptrama.numTarTuncate.Trim().PadRight(19, ' ') +
                                                  "\nCODIGOPROCESO: 000200" +
                                                  "\nFECHACONSUMO: " + resptrama.fechaTrans +
                                                  "\nHORACONSUMO: " + resptrama.horaTrans +
                                                  "\nNUMEROVOUCHER: " + resptrama.secuencialtransaccion +
                                                  "\nAUTORIZACION: " + (trama.TipoTransaccion == "03" ? trama.numAutorizacion : resptrama.numAut) +
                                                  "\nANULADO: " + (trama.TipoTransaccion == "03" ? "1" : "0") +
                                                  "\nVALORCONSUMO: " + trama.montoTotalTransaccion.PadLeft(13, '0') +
                                                  "\nFORMAAUTORIZA: 1" +
                                                  "\nTIPOCONSUMO: " + TIPOCONSUMO +
                                                  "\nPLAZO: " + trama.plazoDiferido.PadLeft(2, '0') +
                                                  "\nTIPOLECTURA: " + resptrama.modoLectura.PadLeft(3, '0') +
                                                  "\nTIPOMONEDA: 840" +
                                                  "\nVALORIVA: " + trama.impuestoIvaTransaccion.PadLeft(13, '0') +
                                                  "\nVALORSERVICIO: 0000000000000" +
                                                  "\nVALORPROPINA: " + trama.popinaTransaccion.PadLeft(13, '0') +
                                                  "\nVALORINTERES: " + resptrama.valInteres.Trim().PadLeft(13, '0') +
                                                  "\nVALORFIJO: " + resptrama.montoFijo.Trim().PadLeft(13, '0') +
                                                  "\nTIPOPROMOCION: 00" +
                                                  "\nMESESGRACIA: " + trama.mesGracia.PadLeft(2, '0') +
                                                  "\nEMPRESASERVICIO: 0000" +
                                                  //"\nESTADOTRX: " + (tipoConsumo == "01" ? "O" : "R") +
                                                  "\nESTADOTRX: " + (objGeneraVoucher.tipoConsumo == "01" ? "O" : "O") +  //JCanarte
                                                  "\nCODIGORESPUESTA: " + resptrama.codigoRespuesta +
                                                  "\nTIPODISPOSITIVO: 2" +
                                                  "\nADQUIRENTETARJETA: CREDIMATIC01" +
                                                  "\nADQUIRENTESERVICIO:             " +
                                                  "\nMONTOGRAVAIVA: " + trama.montoBaseGravaIVa.PadLeft(13, '0') +
                                                  "\nMONTONOGRAVAIVA: " + trama.montoBaseNoGravaIVa.PadLeft(13, '0') +
                                                  "\nPUNTOEMISION: " + Establecimiento + PtoEmision +
                                                  "\nPROCESADO: 0" +
                                                  "\nGRUPOTAR: " + resptrama.nomGruTar +
                                                  "\nAUTORIZADOR: " + resptrama.codigoRed +
                                                  "\nLOTE: " + resptrama.numerolote +
                                                  "\nFACTURA: " + "PAVIPLAN" +
                                                  "\nARQC: " + resptrama.ARQC +
                                                  "\nAIDEMV: " + resptrama.AIDEMV +
                                                  "\nEMV: " + resptrama.idEMV +
                                                  "\nTC: " + resptrama.tipoCritoyValorEMV +
                                                  "\nPUBLICIDAD: " + resptrama.mensajePremioPublicidad +
                                                  "\nTIPOTRANSACCION: " + trama.TipoTransaccion +
                                                  "\nBANCOADQUIRIENTE: " + resptrama.nomBancoAdq +
                                                  "\nTARJETAHABIENTE: " + resptrama.nombreTarjetaHabiente +
                                                  "\nMID: " + resptrama.merchantId + //trama.MID +
                                                  "\nTID: " + trama.TID +
                                                  "\nVENCTAR: " + (resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX") +
                                                  "\nANULAUTORIZACION: " + trama.numAutorizacion.ToString() +
                                                  "\nTIPOBANCOTARJETA: " + " ";

                    datosVoucher += Environment.NewLine + Environment.NewLine + objGeneraVoucher.scriptInsertarVoucher;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "BasePagos", "ProcesaPinpad", "Se realizo correctamente una transaccion pinpad al banco" + msjSeImprimieronVouchers + " pero el voucher no pudo ser grabado en nuestra base interna, " + Environment.NewLine + "a continuacion las excepciones encontradas - " +
                                                              Control.Common.ExceptionHandler.GetExceptionMessages(objGeneraVoucher.exception)
                                                              + Environment.NewLine + "StackTrace:"
                                                              + Environment.NewLine + objGeneraVoucher.StackTrace
                                                              + Environment.NewLine + "Datos Voucher: " + datosVoucher.Replace("\n", Environment.NewLine));


                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[exception]", valor = repsuestaPinpad });
                    Control.Common.General.GetMensajeToList(416, parametros);
                    return;
                }
                Control.Common.Logger.Agregar_Trace_Voucher(objGeneraVoucher.pos_voucher);
            }

        }


        private void prepararVoucherTarjeta(string tipo_voucher, Tramas.RespuestaProcesoPago trama, Tramas.ProcesaPago pp)
        {

            var db = new POSEntities();
            DateTime fecha = DateTime.ParseExact("01-" + trama.fechaVencTar.Substring(2, 2) + "-" + trama.fechaVencTar.Substring(0, 2), "dd-MM-yy", CultureInfo.InvariantCulture);
            string fechaVencTar = Convert.ToString(fecha).Substring(5, 2) + '/' + Convert.ToString(fecha).Substring(0, 4);

            var recipe = new Models.PrinterRecipes.VoucherTarjetaCredito();
            recipe.NomTarjeta = trama.nomGruTar;
            recipe.MID = pp.MID;
            recipe.TID = pp.TID;
            recipe.NumTarjeta = trama.numTarTuncate;
            recipe.NumLote = trama.numerolote;
            recipe.Adquiriente = trama.nomBancoAdq;
            recipe.Aprobacion = pp.TipoTransaccion == "03" ? pp.numAutorizacion : trama.numAut;
            recipe.Secuencial = trama.secuencialtransaccion;
            recipe.NombreTarjetaHabiente = trama.nombreTarjetaHabiente;
            recipe.FechaTrans = trama.fechaTrans.Substring(0, 4) + "/" + trama.fechaTrans.Substring(4, 2) + "/" + trama.fechaTrans.Substring(6, 2);
            recipe.HoraTrans = trama.horaTrans.Substring(0, 2) + ":" + trama.horaTrans.Substring(2, 2) + ":" + trama.horaTrans.Substring(4, 2);
            recipe.VenTarjeta = trama.codigoRed == "02" ? fechaVencTar : "XX/XXXX";
            var valoranulacion = "";
            if (pp.TipoTransaccion == "03")
            {
                valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion).FirstOrDefault().VALORCONSUMO;
                valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00");
            }
            decimal valorinteres = trama.valInteres == "            " ? 0 : Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2));

            recipe.ValorTotal = pp.TipoTransaccion == "03" ? valoranulacion : (Decimal.Parse(pp.montoTotalTransaccion.Substring(0, 10) + "." + pp.montoTotalTransaccion.Substring(10, 2)) + valorinteres).ToString("###,##0.00");
            string modolectura = "";
            switch (trama.modoLectura)
            {
                case "01":
                    modolectura = "Manual";
                    break;
                case "02":
                    modolectura = "Banda";
                    break;
                case "03":
                    modolectura = "Chip";
                    break;
                case "04":
                    modolectura = "Fallback Manual (Chip)";
                    break;
                case "05":
                    modolectura = "Fallback Banda (Chip) ";
                    break;
            }
            recipe.ModoLectura = modolectura;

            if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
            {


                recipe.BaseIva = Decimal.Parse(pp.montoBaseGravaIVa.Substring(0, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString("###,##0.00");
                recipe.BaseSinIva = Decimal.Parse(pp.montoBaseNoGravaIVa.Substring(0, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString("###,##0.00");
                recipe.Subtotal = (Decimal.Parse(pp.montoBaseGravaIVa.Substring(0, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)) + Decimal.Parse(pp.montoBaseNoGravaIVa.Substring(0, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2))).ToString("###,##0.00");
                recipe.Iva = Decimal.Parse(pp.impuestoIvaTransaccion.Substring(0, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString("###,##0.00");
            }
            else
            {
                recipe.BaseIva = "";
                recipe.BaseSinIva = "";
                recipe.Subtotal = "";
                recipe.Iva = "";
            }
            recipe.CodigoRed = trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST";

            string tipodebcred = "";
            if (trama.nomGruTar.Contains("DEBIT"))
            {
                tipodebcred = "<footer>     DEBITO</footer>\n";

                recipe.Pagare = " ";
            }
            else
            {
                tipodebcred = "<footer>     ROTATIVO</footer>\n";
            }
            if (pp.TipoTransaccion != "03")
                recipe.TipoDebCredito = tipodebcred;

            recipe.Intereses = "";
            recipe.Arqc = trama.ARQC;
            recipe.Aidemv = trama.AIDEMV.Trim();
            recipe.Emv = trama.idEMV;
            recipe.Tc = trama.tipoCritoyValorEMV;
            recipe.Publicidad = trama.mensajePremioPublicidad;
            recipe.Factura = "PAVIPLAN " + txtIdentificacion.Text.Trim();

            Control.Common.Printer.ImprimirVoucherTarjetaCredito(tipo_voucher, recipe);

            /*
            var db = new POSEntities();

            if (db.core_recibo.Any(x => x.identificador == tipo_voucher))
            {
                var voucher = db.core_recibo.First(x => x.identificador == tipo_voucher);
                string texto = voucher.cuerpo;

                texto = texto.Replace("<<NOM_TARJETA>>", trama.nomGruTar + "\nCOMERCIO: " + pp.MID + "\nTID: " + pp.TID);
                texto = texto.Replace("<<NUM_TARJETA>>", trama.numTarTuncate);
                texto = texto.Replace("<<NUM_LOTE>>", trama.numerolote);
                texto = texto.Replace("<<ADQUIRIENTE>>", trama.nomBancoAdq);
                texto = texto.Replace("<<APROBACION>>", pp.TipoTransaccion == "03" ? pp.numAutorizacion : trama.numAut);
                texto = texto.Replace("<<SECUENCIAL>>", trama.secuencialtransaccion);
                texto = texto.Replace("<<NOMBRE_TRAJETAHABIENTE>>", trama.nombreTarjetaHabiente);
                texto = texto.Replace("<<FECHA_TRANS>>", trama.fechaTrans.Substring(0, 4) + "/" + trama.fechaTrans.Substring(4, 2) + "/" + trama.fechaTrans.Substring(6, 2));
                texto = texto.Replace("<<HORA_TRANS>>", trama.horaTrans.Substring(0, 2) + ":" + trama.horaTrans.Substring(2, 2) + ":" + trama.horaTrans.Substring(4, 2));
                texto = texto.Replace("<<VENC_TAR>>", trama.codigoRed == "02" ? trama.fechaVencTar.Substring(0, 2) + "/" + trama.fechaVencTar.Substring(2, 2) : "XX/XX");
                var valoranulacion = "";
                if (pp.TipoTransaccion == "03")
                {
                    valoranulacion = db.POS_VOUCHER.Where(x => x.AUTORIZACION == pp.numAutorizacion).FirstOrDefault().VALORCONSUMO;
                    valoranulacion = Decimal.Parse(valoranulacion.Substring(0, 11) + "." + valoranulacion.Substring(11, 2)).ToString("###,##0.00").PadLeft(13, ' ');
                }
                decimal valorinteres = trama.valInteres == "            " ? 0 : Decimal.Parse(trama.valInteres.Substring(0, 10) + "." + trama.valInteres.Substring(10, 2));



                texto = texto.Replace("<<VALOR_TOTAL>>", pp.TipoTransaccion == "03" ? valoranulacion : (Decimal.Parse(pp.montoTotalTransaccion.Substring(0, 10) + "." + pp.montoTotalTransaccion.Substring(10, 2)) + valorinteres).ToString("###,##0.00").PadLeft(13, ' '));
                string modolectura = "";
                switch (trama.modoLectura)
                {
                    case "01":
                        modolectura = "Manual";
                        break;
                    case "02":
                        modolectura = "Banda";
                        break;
                    case "03":
                        modolectura = "Chip";
                        break;
                    case "04":
                        modolectura = "Fallback Manual (Chip)";
                        break;
                    case "05":
                        modolectura = "Fallback Banda (Chip) ";
                        break;
                }
                texto = texto.Replace("<<MODOLECTURA>>", modolectura);

                if (pp.TipoTransaccion != "03" || pp.TipoTransaccion != "04")
                {


                    texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA 12: USD$ " + Decimal.Parse(pp.montoBaseGravaIVa.Substring(0, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString("###,##0.00").PadLeft(13, ' '));
                    texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + Decimal.Parse(pp.montoBaseNoGravaIVa.Substring(0, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString("###,##0.00").PadLeft(13, ' '));
                    texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + (Decimal.Parse(pp.montoBaseGravaIVa.Substring(0, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)) + Decimal.Parse(pp.montoBaseNoGravaIVa.Substring(0, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2))).ToString("###,##0.00").PadLeft(13, ' '));
                    texto = texto.Replace("<<IVA>>", "               IVA 12%: USD$ " + Decimal.Parse(pp.impuestoIvaTransaccion.Substring(0, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString("###,##0.00").PadLeft(13, ' '));

                    //texto = texto.Replace("<<BASEIVA>>", "BASE CONSUMO TARIFA 14: USD$ " + decimal.Round((_factura.getBase12() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                    //texto = texto.Replace("<<BASESIVA>>", " BASE CONSUMO TARIFA 0: USD$ " + decimal.Round((_factura.getBase0() * porc_pago), 2, MidpointRounding.ToEven).ToString("###,##0.00").PadLeft(13, ' '));
                    //texto = texto.Replace("<<SUBTOTAL>>", "     SUBTOTAL CONSUMOS: USD$ " + decimal.Round((_factura.getSubTotal() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                    //texto = texto.Replace("<<IVA>>", "               IVA 14%: USD$ " + decimal.Round((_factura.getIVA() * porc_pago), 2, MidpointRounding.AwayFromZero).ToString("###,##0.00").PadLeft(13, ' '));
                }
                else
                {
                    texto = texto.Replace("<<BASEIVA>>", "");
                    texto = texto.Replace("<<BASESIVA>>", "");
                    texto = texto.Replace("<<SUBTOTAL>>", "");
                    texto = texto.Replace("<<IVA>>", "");

                }
                //texto = texto.Replace("<<BASEIVA>>", decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2) ).ToString());
                //texto = texto.Replace("<<BASESIVA>>", decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString() );
                //texto = texto.Replace("<<SUBTOTAL>>", (Decimal.Parse(decimal.Parse(pp.montoBaseGravaIVa.Substring(1, 10) + "." + pp.montoBaseGravaIVa.Substring(10, 2)).ToString()) +decimal.Parse(decimal.Parse(pp.montoBaseNoGravaIVa.Substring(1, 10) + "." + pp.montoBaseNoGravaIVa.Substring(10, 2)).ToString())).ToString());
                //texto = texto.Replace("<<IVA>>", decimal.Parse(pp.impuestoIvaTransaccion.Substring(1, 10) + "." + pp.impuestoIvaTransaccion.Substring(10, 2)).ToString());
                texto = texto.Replace("<<CODIGORED>>", trama.codigoRed == "02" ? "MEDIANET" : "DATAFAST");


                string tipodebcred = "";
                if (trama.nomGruTar.Contains("DEBIT"))
                {
                    tipodebcred = "<footer>     DEBITO</footer>";

                    texto = texto.Replace("<<PAGARE>>", " ");
                }
                else
                {
                    //  texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO  QUE  EL  PRODUCTO  DE  ESTA \nTRANSACCION NO SERA UTILIZADO EN \nACTIVIDADES DE LAVADO DE DINERO \nY ACTIVO (LEY 108)");
                    texto = texto.Replace("<<PAGARE>>", "DEBO Y PAGARE AL EMISOR INCONDICIONALMENTE\n Y SIN PROTESTO EL TOTAL DE ESTE PAGARE\nMAS LOS INTERESES Y CARGOS POR SERVICIO.\n\nEN CASO DE MORA PAGARE LA TASA\nMAXIMA AUTORIZADA POR EL EMISOR.\n\nDECLARO QUE EL PRODUCTO DE ESTA TRANSACCION \nNO SERA UTILIZADO EN ACTIVIDADES DE LAVADO DE \nACTIVOS, FINANCIAMIENTO DEL TERRORISMO Y OTROS \nDELITOS ");

                   tipodebcred = "<footer>     ROTATIVO</footer>";
                }
                if (pp.TipoTransaccion != "03")
                    texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred);
                // else
                //aqui es para anulaciones
                //texto = texto.Replace("<<TIPODEBCRED>>", tipodebcred + "\nANULACION");
                //texto = texto.Replace("<<TRANSACCION>>", txtSecuencial.Text);

                texto = texto.Replace("<<INTERESES>>", "");
                //texto = texto.Replace("<<>>", trama);

                texto = texto.Replace("<<ARQC>>", "ARQC:      " + trama.ARQC);
                texto = texto.Replace("<<AIDEMV>>", "AID - EMV: " + trama.AIDEMV.Trim());
                texto = texto.Replace("<<EMV>>", trama.idEMV);
                texto = texto.Replace("<<TC>>", "TC:        " + trama.tipoCritoyValorEMV);
                texto = texto.Replace("<<PUBLICIDAD>>", trama.mensajePremioPublicidad);
                 //texto = texto.Replace("", trama.nomGruTar);
                 //texto = texto.Replace("", trama.nomGruTar);
                 //texto = texto.Replace("", trama.nomGruTar);
                 
                printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                printer.TextToPrint = texto;
                printer.Print();
                //2
                //Control.Common.Printer.Imprimir(texto, 3, 10);


            }
            */
        }

        private void btnPagoManual_Click(object sender, EventArgs e)
        {
            EsManual = 1;
            lblEleccion.Text = "Seleccione Tarjeta:";
            cmbEleccion.Visible = true;
            lblTipoPagoTarjeta.Visible = true;
            cmbTipoPagoTarjeta.Visible = true;
            pnl1.Visible = false;
            POSEntities db = new POSEntities();
            cmbEleccion.DataSource = db.core_tarjetacredito.Include("core_banco").ToList();
            cmbEleccion.DisplayMember = "nombre_completo";
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

        private void txtIdentificacion_TextChanged(object sender, EventArgs e)
        {

        }



        //////////////////////////////////////////////////////////
    }
}
