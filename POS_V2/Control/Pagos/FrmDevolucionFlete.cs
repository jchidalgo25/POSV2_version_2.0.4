using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Models;
using Telerik.WinControls;
using System.Diagnostics;
using System.IO;
using Telerik.WinControls.UI;
using POS.Control.Fingerprint;
using System.Data.SqlClient;

namespace POS.Control.Pagos
{
    /// <summary>
    /// Formulario de Devolución de Retenciones Manuales - Fisicas.
    /// Desarrollado por: Erick Velasco 
    /// Fecha : 2019-10-17
    /// </summary>
    public partial class FrmDevolucionFlete : Telerik.WinControls.UI.RadForm
    {        
        private pos_customer CustomerPOS { get; set; }
        System.Diagnostics.Process virtualKeyboard = new System.Diagnostics.Process();
        IEnumerable<TblFlete> LstMotorizados;
        private VerificationForm Verifier;
        private DialogResult verificador;
        private FleteMotorizado _fleteMotorizado =  new FleteMotorizado();
        AppData Data;
        private string gConceptoFlete = string.Empty;
        private string conceptofleteDefault = "Servicio Flete a Domicilio";

        public FrmDevolucionFlete()
        {
            InitializeComponent();
        }

        private void FrmDevolucionFlete_Load(object sender, EventArgs e)
        {
            gConceptoFlete = conceptofleteDefault;
            CargarParametros();
        }

        private void btnKbd_Click(object sender, EventArgs e)
        {
            Control.Common.General.TecladoPantalla();
        }
        

     /*   private void btnKbd_Click(object sender, EventArgs e)
        {
            string windir = Environment.GetEnvironmentVariable("WINDIR");
            string osk = null;
            if (osk == null)
            {
                osk = "C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
                if (!File.Exists(osk))
                {
                    osk = null;
                }
            }

            if (osk == null)
            {
                osk = Path.Combine(Path.Combine(windir, "SysWOW64"), "osk.exe");
                if (!File.Exists(osk))
                {
                    osk = null;
                }
            }

            if (osk == null)
            {
                osk = Path.Combine(Path.Combine(windir, "system32"), "osk.exe");
                if (!File.Exists(osk))
                {
                    osk = null;
                }
            }

            if (osk == null)
            {
                osk = "osk.exe";
            }
            virtualKeyboard = System.Diagnostics.Process.Start(osk); // open

        }*/

        private void FrmDevolucionFlete_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "FrmRetencionFisicaDev", "FrmRetencionFisicaDev_FormClosing", "Ha ocurrido una excepción al Cerrar el Formulario, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }



        #region Métodos
        private void CargarParametros()
        {
            bool parametrosCargados = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var recibo = db.core_recibo.Where(x => x.identificador == "RECIBO_FLETE_PEDIDODOMICILIO").FirstOrDefault();
                    if (recibo == null)
                    {
                        throw new Exception("No hay recibo 'RECIBO_FLETE_PEDIDODOMICILIO' en core_recibo");
                    }
                    _fleteMotorizado.Recibo = recibo.cuerpo;

                    var reciboCOPIA = db.core_recibo.Where(x => x.identificador == "RECIBO_FLETE_PEDIDODOMICILIO_COPIA").FirstOrDefault();
                    if (reciboCOPIA == null)
                    {
                        throw new Exception("No hay recibo 'RECIBO_FLETE_PEDIDODOMICILIOCOPIA' en core_recibo");
                    }
                    _fleteMotorizado.ReciboCOPIA = reciboCOPIA.cuerpo;

                    if(db.core_parametro.Any(x => x.identificador == "CONCEPTO_FLETEDOMICILIO"))
                    {
                        gConceptoFlete = db.core_parametro.Where(x => x.identificador == "CONCEPTO_FLETEDOMICILIO").FirstOrDefault().valor;
                    }
                    else
                    {
                        throw new Exception("No se encontró el parametro 'CONCEPTO_FLETEDOMICILIO'");
                    }

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmRetencionElectronica", "CargarParametros", "Imposible terminar de cargar parámetros en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                parametrosCargados = false;
            }

            if (!parametrosCargados)
            {
                //Control.Common.WinForm.ShowMessage("No se pudieron cargar todos los parámetros necesarios para el formulario en este momento, esto pudo deberse a una breve interrupción en la comunicación. Vuélvalo a intentar en unos momentos");
                Control.Common.General.GetMensajeToList(441);
                this.Close();
            }
        }

        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();

            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;               

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                GetDatosFlete();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmDevolucionFlete", "btnBuscar_Click", "No se pudo obtener la consulta en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar datos de Fletes de Pedidos a Domicilio Completados, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(442);
            }
        }

        private DataTable crearTabla()
        {
            DataTable dt = new DataTable();
            DataColumn column;
            try
            {
               

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "OrdenApp";
                column.ReadOnly = true;
                //column.Unique = true;
                // Add the Column to the DataColumnCollection.
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Codigo Rider";
                column.ReadOnly = true;
                //column.Unique = true;
                // Add the Column to the DataColumnCollection.
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Nombre Rider";
                column.ReadOnly = true;
                //column.Unique = true;
                // Add the Column to the DataColumnCollection.
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.Decimal");
                column.ColumnName = "total";
                column.ReadOnly = true;
               // column.Unique = true;
                // Add the Column to the DataColumnCollection.
                dt.Columns.Add(column);

                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = "Estado";
                column.ReadOnly = true;
                // column.Unique = true;
                // Add the Column to the DataColumnCollection.
                dt.Columns.Add(column);

                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        private void GetDatosFlete()
        {


            DataTable dt = crearTabla();
            DataRow row;
            string EstadoEtiqueta = string.Empty;
            try
            {
                MasterTemplate.Rows.Clear();
                using (POSEntities db = new POSEntities())
                {
                    if (db.TblFlete.Any(x => x.IdMotorizado == txtCodigoMotorizado.Text && x.Estado ==0))
                    {
                        LstMotorizados = new List<TblFlete>();

                        LstMotorizados = db.TblFlete.Where(x => x.IdMotorizado == txtCodigoMotorizado.Text && x.Estado == 0 && x.Establecimiento == Control.Common.GlobalParameters.Establecimiento).ToList();
                        lblTotalMotorizado.Text ="$ "+ LstMotorizados.Sum(x=>x.Total).ToString();

                        foreach (TblFlete flete in LstMotorizados)
                        {
                            if(flete.Estado ==0)
                            {
                                EstadoEtiqueta = "Pendiente de Pago";
                            }

                            if (flete.Estado == 1)
                            {
                                EstadoEtiqueta = "Liquidado Motorizado";
                            }

                            Telerik.WinControls.UI.GridViewDataRowInfo dataRowInfo = new Telerik.WinControls.UI.GridViewDataRowInfo(this.MasterTemplate.MasterView);
                            dataRowInfo.Cells[0].Value = flete.OrdenId;
                            dataRowInfo.Cells[1].Value = flete.IdMotorizado;
                            dataRowInfo.Cells[2].Value = flete.NombreMotorizado;
                            dataRowInfo.Cells[3].Value = flete.Total;
                            dataRowInfo.Cells[4].Value = EstadoEtiqueta;
                            dataRowInfo.Cells[5].Value = flete.FechaOrdenCompletado;

                            MasterTemplate.Rows.Add(dataRowInfo);

                           
                           
                        }
                        /*
                        MasterTemplate.MasterTemplate.DataSource = motorizados.ToList();
*/
                        this.MasterTemplate.MasterTemplate.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                        this.MasterTemplate.Columns[1].BestFit();
                        this.MasterTemplate.Columns[5].BestFit();

                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage("No se han encontrado registros con el criterio ingresado.");
                        Control.Common.General.GetMensajeToList(443);

                    }
                }

            }
            catch (Exception ex)
            {


                throw;
            }
        }

        private void txtCodigoMotorizado_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    GetDatosFlete();
                }              
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmDevolucionFlete", "txtCodigoMotorizado_KeyPress", "No se pudo obtener la consulta en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No pudimos consultar datos de Fletes de Pedidos a Domicilio Completados, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(444);
            }
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            bool _utilizaHuella = false;
            try
            {
                if (!ValidaProveedor(txtCodigoMotorizado.Text))
                    return;

                using (var db = new POSEntities())
                {
                    if (db.core_parametro.Any(x => x.identificador == "FINGERPRINT" && x.parametro2 == Control.Common.GlobalParameters.Establecimiento))
                    {
                        if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == Control.Common.GlobalParameters.Establecimiento).First().valor == "TRUE")
                        {
                            Factura Motorizado = new Factura();
                            Motorizado.User = new User { username= txtCodigoMotorizado.Text } ;
                            Verifier = new VerificationForm(Control.Common.GlobalParameters.DataForFingerprint, Motorizado);
                            Verifier.Tag = "usr";
                            verificador = Verifier.ShowDialog();
                            if (verificador == DialogResult.OK)
                            {
                                _utilizaHuella = true;
                                //Valida si tiene efectivo disponible para el pago de los fletes.
                                if (TieneEfectivoCaja())
                                {
                                    DevolverDinero();
                                }
                            }
                        }
                        else
                        {
                            //DevolverDinero();
                            if (TieneEfectivoCaja())
                            {
                                DevolverDinero();
                            }
                        }
                    }
                        
                }

               // DevolverDinero();
            }
            catch (Exception ex)
            {

        
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmDevolucionFlete", "txtCodigoMotorizado_KeyPress", "No se pudo obtener la consulta en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No se pudo completar la transacción, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(445);

            }
        }

        public bool TieneEfectivoCaja()
        {
            decimal TotalEfectivoCaja = 0;
            decimal TotalPagadoMotorizado = 0;
            decimal TotalFletexLiquidar = 0;
            decimal DineroDisponibleEfectivo = 0;           
            try
            {
                string establecimiento = Control.Common.GlobalParameters.Establecimiento;
                string ptoEmision = Control.Common.GlobalParameters.PuntoEmision;
                string usuario = Control.Common.GlobalParameters.Usuario;
                string IdCajal = Program.ID_Caja_POS;
                DateTime FechaIni;
                DateTime FechaFin;
                DateTime Hoy;
                Hoy = DateTime.Now;
                bool valida = false;

                FechaIni = new DateTime(Hoy.Year, Hoy.Month, Hoy.Day, 0, 0, 0);
                FechaFin = new DateTime(Hoy.Year, Hoy.Month, Hoy.Day, 23, 59, 59);
                TotalFletexLiquidar = LstMotorizados.Sum(x => x.Total);            
                using (var db = new POSEntities())
                {                   
                    if(db.core_parametro.Any(x=>x.identificador == "VALIDAEFECTIVOCAJAFLETE"))
                    {
                        var q = db.core_parametro.Where(x => x.identificador == "VALIDAEFECTIVOCAJAFLETE" && x.parametro2.Contains(establecimiento)).FirstOrDefault();
                        if (q != null)
                        {
                            if (string.IsNullOrEmpty(q.valor))//.ToUpper() =="FALSE")
                            {
                                return true;
                            }
                        
                            if (q.valor.ToUpper() =="FALSE")
                            {
                                return true;
                            }
                        }
                        else { return true; }
                    }

                    if(Program.ID_Caja_POS== "CAJA_ABIERTA_MANUAL")
                    {
                        if (db.core_facturapago.Any(x => x.tipo_id == "EFECTIVO" && x.core_factura.establecimiento == establecimiento && x.core_factura.punto_emision == ptoEmision && x.core_factura.usuario == usuario && (x.core_factura.fecha_creacion>= FechaIni && x.core_factura.fecha_creacion < FechaFin))) //x.core_factura.msgError == Program.ID_Caja_POS))
                        {
                            TotalEfectivoCaja = db.core_facturapago.Where(x => x.tipo_id == "EFECTIVO" && x.core_factura.establecimiento == establecimiento && x.core_factura.punto_emision == ptoEmision && x.core_factura.usuario == usuario && (x.core_factura.fecha_creacion >= FechaIni && x.core_factura.fecha_creacion < FechaFin)).Sum(y => y.valor);
                        }

                        //Obtengo el total de los pagado en esa caja por concepto de flete Motorizado. Estado >0 ya que puede 
                        if (db.TblFlete.Any(x => x.Establecimiento == establecimiento && x.Punto_emision== ptoEmision && x.IdCaja == Program.ID_Caja_POS && x.Estado > 0 && x.Usuario == usuario && (x.FechaModificacion >= FechaIni && x.FechaModificacion <= FechaFin)))
                        {
                            TotalPagadoMotorizado = db.TblFlete.Where(x => x.Establecimiento == establecimiento && x.Punto_emision == ptoEmision && x.IdCaja == Program.ID_Caja_POS && x.Estado > 0 && x.Usuario == usuario && (x.FechaModificacion >= FechaIni && x.FechaModificacion <= FechaFin)).Sum(y => y.Total);
                        }  
                        
                    }
                    else
                    {

                        if (db.core_facturapago.Any(x => x.tipo_id == "EFECTIVO" && x.core_factura.msgError == IdCajal)) //x.core_factura.msgError == Program.ID_Caja_POS))
                        {
                            TotalEfectivoCaja = db.core_facturapago.Where(x => x.tipo_id == "EFECTIVO" && x.core_factura.msgError == IdCajal).Sum(y => y.valor);
                        }

                        //Obtengo el total de los pagado en esa caja por concepto de flete Motorizado. Estado >0 ya que puede 
                        if (db.TblFlete.Any(x => x.IdCaja == IdCajal && x.Estado > 0 ))
                        {
                            TotalPagadoMotorizado = db.TblFlete.Where(x => x.IdCaja == Program.ID_Caja_POS && x.Estado > 0 ).Sum(y => y.Total);
                        }                       
                    }  
                    

                }  
                
                    DineroDisponibleEfectivo = TotalEfectivoCaja - TotalPagadoMotorizado;      
                if (DineroDisponibleEfectivo>0  )
                {
                    if (DineroDisponibleEfectivo >= TotalFletexLiquidar)
                    {
                        return true;
                    }
                    else
                    {
                        //Control.Common.WinForm.ShowMessage("No tiene Suficiente Dinero en Efectivo en la Caja para realizar el desembolso de Flete Motorizado.");
                        Control.Common.General.GetMensajeToList(446);

                        return false;
                    }
                }
                else
                {
                    //Control.Common.WinForm.ShowMessage("No tiene Efectivo Disponible en la Caja para realizar el desembolso de Flete Motorizado.");
                    Control.Common.General.GetMensajeToList(447);
                    return false;
                }

            }
            catch (Exception ex)
            {                
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Pagos.FrmDevolucionFlete", "txtCodigoMotorizado_KeyPress", "No se pudo obtener la consulta en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No se pudo completar la transacción, inténtelo nuevamente en unos momentos");
                Control.Common.General.GetMensajeToList(448);
                return false;
            }
        }
        private bool DevolverDinero()
        {
            bool _retorno = true;
            TblFlete fleteMotorizado1 = new TblFlete();
            Decimal totalFlete = 0;

            try
            {
                using (var db = new POSEntities())
                {
                    using (System.Data.Entity.DbContextTransaction dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Reemplazo de Ordenes de Fletes del Motorizado. 
                            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaOrden>(.*)\</plantillaOrden>");                            
                            //regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");                            
                            string concepto= string.Empty;
                            decimal valorflete = 0;
                            concepto = gConceptoFlete;
                            //Reemplazo de productos                   
                            StringBuilder Orders = new StringBuilder();
                            /*foreach (var pago in this.Pagos)
                            {
                                //pagos.AppendLine(pago.Descripcion + " : " + String.Format("{0,10:0.00}", pago.Valor.ToString("N2")));
                                pagos.AppendLine(pago.Descripcion + Convert.ToChar(9) + ":" + Control.Common.StringHelper.DevolverConPadding(pago.Valor.ToString("N2"), 65));
                            }*/

                            foreach (TblFlete fleteMotorizado in LstMotorizados)
                            {

                                TblFlete fleteUpd = db.TblFlete.Where(x => x.Id == fleteMotorizado.Id).FirstOrDefault();
                                fleteUpd.Estado = 1; //Liquidado -Pagado al motorizado.
                                fleteUpd.Usuario = Control.Common.GlobalParameters.UserObj.username;
                                fleteUpd.Punto_emision = Control.Common.GlobalParameters.PuntoEmision;
                                fleteUpd.IdCaja = Program.ID_Caja_POS;
                                fleteUpd.FechaModificacion = DateTime.Now;
                                fleteUpd.IpProceso = Control.Common.GlobalParameters.IpMaquina;
                                db.SaveChanges();

                                totalFlete += Convert.ToDecimal(fleteUpd.Total);
                                fleteMotorizado1 = fleteMotorizado;

                                valorflete = (decimal)fleteMotorizado.Total;
                                //Orders.AppendLine(item.Nombre.PadRight(40, ' '));
                                //items.AppendLine(asterisk.PadRight(8, ' ') + item.Cantidad.ToString("N2").PadLeft(10, ' ') + "     " + item.Pvp.ToString("N2").PadLeft(5, ' ') + "     " + item.SubtotalSinDescuento.ToString("N2").PadLeft(5, ' '));

                                int PadRightConcepto = concepto.Length + 10;
                                int PadRightIdFlete = fleteMotorizado.OrdenId.ToString().Length + 15;
                                int PadRightValorFlete = valorflete.ToString("N2").Length + 5;

                                string OrderLine = fleteMotorizado.OrdenId.ToString().PadRight(PadRightIdFlete, ' ') +
                                concepto.PadRight(PadRightConcepto, ' ') +
                                valorflete.ToString("N2").PadRight(PadRightValorFlete, ' ')
                                ;

                                Orders.AppendLine(OrderLine);
                                //Orders.AppendLine(Control.Common.StringHelper.DevolverConPadding(fleteMotorizado.OrdenId.ToString(), 10,8, false) + Control.Common.StringHelper.DevolverConPadding(concepto, 50) + Control.Common.StringHelper.DevolverConPadding(valorflete.ToString("N2"), 12) );

                            }


                            dbContextTransaction.Commit();
                            //Control.Common.WinForm.ShowMessage("Devolución realizada exitosamente.");
                            Control.Common.General.GetMensajeToList(452);


                            _fleteMotorizado.CajeroNombre = Control.Common.GlobalParameters.UserObj.nombres;
                            _fleteMotorizado.CajeroCedula = Control.Common.GlobalParameters.UserObj.username;
                            _fleteMotorizado.MotorizadoCodigo = fleteMotorizado1.IdMotorizado;
                            _fleteMotorizado.MotorizadoNombre = fleteMotorizado1.NombreMotorizado;
                            _fleteMotorizado.Total = totalFlete.ToString();
                            _fleteMotorizado.Fecha = DateTime.Now.ToString(); 
                            
                            
                            _fleteMotorizado.Recibo = regex.Replace(_fleteMotorizado.Recibo, Orders.ToString());
                            _fleteMotorizado.Recibo = _fleteMotorizado.Recibo.Replace("<plantillaOrden>", "").Replace("</plantillaOrden>", "");
                            //_fleteMotorizado.Recibo = _fleteMotorizado.Recibo.Replace("<plantillaPago>", "").Replace("</plantillaPago>", "");
                            _fleteMotorizado.ImprimirRecibo();

                            _fleteMotorizado.Recibo = _fleteMotorizado.ReciboCOPIA;
                            _fleteMotorizado.Recibo = regex.Replace(_fleteMotorizado.Recibo, Orders.ToString());
                            _fleteMotorizado.Recibo = _fleteMotorizado.Recibo.Replace("<plantillaOrden>", "").Replace("</plantillaOrden>", "");
                            _fleteMotorizado.ImprimirRecibo();

                            this.Close();


                            /*
                            db.SaveChanges();
                            _objRetEletronica.ImprimirRecibo();

                            dbContextTransaction.Commit();
                            Control.Common.WinForm.ShowMessage("Devolución realizada exitosamente.");

                            this.Close();
                            */
                        }
                        catch (Exception ex)
                        {
                            string msj = ex.ToString();

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Devolución Flete Pedido a Domicilio", "Grabar", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                            var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                            Properties.Settings.Default.MAILERROR_FROM,
                            Properties.Settings.Default.MAILERROR_ALIAS,
                            Properties.Settings.Default.MAILERROR_DESTINO,
                            Properties.Settings.Default.MAILERROR_CC,
                            Properties.Settings.Default.MAILERROR_MOTIVO,
                            String.Format("Establecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroId: {4} \n\nDatos Excepcion ------------\nClass: {5} \nMethod: {6} \nMessage: {7} \nStackTrace: {8}",
                                          Control.Common.GlobalParameters.Establecimiento,
                                          Control.Common.GlobalParameters.PuntoEmision,
                                          Control.Common.GlobalParameters.IpMaquina,
                                          Control.Common.GlobalParameters.UserObj.nombres,
                                          Control.Common.GlobalParameters.UserObj.username,
                                          "POS.Control.Pagos.FrmDevolucionFlete",
                                          "DevolverDinero",
                                          Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                          ex.StackTrace),
                            false,
                            String.Empty);

                            if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Devolución Flete Pedido a Domicilio", "DevolverDinero", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                            }
                            dbContextTransaction.Rollback();

                            Control.Common.General.GetMensajeToList(449);
                            //Control.Common.WinForm.ShowMessage("No se pudo grabar la transacción en este momento, esto puede deberse a una breve interrupción en la comunicación, inténtelo nuevamente en unos momentos");
                        }
                    }
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            return _retorno;
        }


        private bool ValidaProveedor(string motorizado)
        {
            string codproveedor = "";
            SqlConnection conexion = new SqlConnection(Properties.Settings.Default.CONECTA_AX);
            string Query = null;
            SqlCommand comando = default(SqlCommand);
            using (conexion)
            {
                try
                {
                    conexion.Open();
                    Query = "select top 1 ruc,recid from LSTFLETEMOTORIZADO where motorizadoid = '" + motorizado  +"' and activo = 1";
                    comando = new SqlCommand(Query, conexion);
                    SqlDataReader dr = comando.ExecuteReader();
                    if (!dr.HasRows)
                    {
                        //Control.Common.WinForm.ShowMessage("No existe asociación del motorizado con el proveedor");
                        Control.Common.General.GetMensajeToList(450);
                        return false;
                    }
                    else
                    {
                        dr.Read();
                        codproveedor = dr.GetValue(0).ToString();
                    }

                    dr.Close();

                    Query = "select top 1 cod_proveedor from TBL_BLOQUEO_PROVEEDOR where cod_proveedor = '" + codproveedor + "' and (blockorden = 1 or blockfactura = 1 or blockpago = 1 or blockdiarios = 1)";
                    comando = new SqlCommand(Query, conexion);
                    dr = comando.ExecuteReader();
                    if (dr.HasRows)
                    {
                        //Control.Common.WinForm.ShowMessage("Proveedor se encuentra bloqueado,  debe gestionar con la administración");
                        Control.Common.General.GetMensajeToList(451);
                        return false; 
                    }
                    dr.Close();
                    conexion.Close();
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidaProveedor", "Ocurrio una novedad durante la consulta de tabla LstFleteMotorizado. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);                     
                }
                return true;
            }
        }

    }    
}
