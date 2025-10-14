using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Linq;
using POS.Models;
using System.IO;
using System.Data.SqlClient;

namespace POS.Control.WalletPoints 
{

    public partial class CapturaCupon : Telerik.WinControls.UI.RadForm
    {
        private static string _IdClienteApp = string.Empty;
        public static string IdClienteApp
        {
            get { return _IdClienteApp; }
            set { _IdClienteApp = value; }
        }


        public static string ITEMID {get;set;}
        public static string ITEMBARCODE { get;set;}


        public CapturaCupon(string MainClienteApp)
        {
            InitializeComponent();

            IdClienteApp = MainClienteApp;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private ClsCuponApp _cuponApp;

        public ClsCuponApp ObjCuponApp { get { return _cuponApp; } }

        private bool _noTieneTicket = false;
        public bool NoTieneTicket { get { return _noTieneTicket; } }

      

        private void btnValidar_Click(object sender, EventArgs e)
        {
            //ValidarCupon();
            ValidarCuponV2();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
           
            /*Si el porcentaje de descuento es igual a 100, este me genera el consumo de la cuponera*/
            if (_cuponApp.Valor == 100)
            {
                // _cuponApp.IdTblPremio;
                var deta = ConsumoCupon(_cuponApp.IdTblPremio, _cuponApp.Codigo);
                if (deta)
                {
                    Confirmar();
                    _cuponApp = null;

                    MessageBox.Show("El canje del Cupón se ha realizado con éxito", "Canje Cupón");   
                }

                
            }
            else {
                Confirmar();
            }
        }

    
        private void Confirmar()
        {
            string msg = string.Empty;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();



            try
            {
                if (_cuponApp != null)
                {
                    string cadenaCon = "";
                    if (Control.Common.GlobalParameters.ConServerPuntos != "")
                    {
                        cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                        SqlConnection conn = new SqlConnection(cadenaCon);
                        try
                        {
                            string Query = "Exec PtsCliente.spCuponApp  'DET'," + "''," + _cuponApp.IdTblPremio + "";

                            conn.Open();
                            SqlCommand select = new SqlCommand(Query, conn);
                            IAsyncResult iar = select.BeginExecuteReader();
                            SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                            _cuponApp.Lstitem = new List<Items>();
                            while (dr.Read())
                            {

                                Items itm = new Items();
                                itm.ItemId  = dr.GetValue(2).ToString();
                                _cuponApp.Lstitem.Add(itm);
                            }
                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            conn.Close();
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/WalletPoint/CapturaCupon", "Confirmar", "No se pudo consultar  dbo.spConsultaCuponApp  'DET', a continuacion el detalle de la excepcion - " + ex.Message);
                        }
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoint/CapturaCupon", "Confirmar", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                    }

                    _cuponApp.EstaConfirmado = true;
                    this.Close();
                }
                else
                {

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[código]", valor = txtCodigo.Text });
                    Control.Common.General.GetMensajeToList(555, parametros);


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaCupon", "Confirmar", "Se está intentando confirmar el código que escribieron en el formulario '" + txtCodigo.Text + "' antes de realizar el paso de validación.");
                    //Control.Common.WinForm.ShowMessage("Validar el código '" + txtCodigo.Text + "' antes de confirmar");
                    

                }
            }
            catch (Exception ex)
            {
                parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[código]", valor = txtCodigo.Text });
                Control.Common.General.GetMensajeToList(556, parametros);


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "CapturaCupon", "Confirmar", "Ocurrió un incidente mientras se confirmaba cupón app del código '" + txtCodigo.Text + "'. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No fue posible confirmar el código '" + txtCodigo.Text + "'. Por favor inténtelo nuevamente");
            }
        }

     

        private void CapturaParqueo_Load(object sender, EventArgs e)
        {

            //lblFacEnlazarAyuda.Text = "F-" + Common.GlobalParameters.Establecimiento.PadLeft(3, '0') + "-";
            //ActualizarFacEnlazar();

            //lblValidacion.Text = string.Empty;
            //CargarParametros();

            this.txtCodArticulo.Focus();
            this.txtCodigo.ReadOnly = false;
        }
        
        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                lblValidacion.Visible = true;
                //ValidarCupon();
                ValidarCuponV2();

            }
        }

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }


        private void txtCodArticulo_LostFocus(object sender, EventArgs e) {
            string CodArticulo = string.Empty;
            string DscArticulo = string.Empty;
            //string IdClienteApp = string.Empty;

            this.lblDescArticulo.Text = string.Empty;

            CodArticulo = string.IsNullOrEmpty(txtCodArticulo.Text) ? "" : txtCodArticulo.Text;

            ValidaArticulo(CodArticulo);

            txtCodigo.Focus();

        }

        private void txtCodArticulo_KeyPress(object sender, KeyPressEventArgs e)
        {
            string CodArticulo = string.Empty;
            string DscArticulo = string.Empty;
            //string IdClienteApp = string.Empty;

            this.lblDescArticulo.Text = string.Empty;
            
            if (e.KeyChar == (char)Keys.Enter)
            {
                CodArticulo = string.IsNullOrEmpty(txtCodArticulo.Text) ? "": txtCodArticulo.Text;

                ValidaArticulo(CodArticulo);

                txtCodigo.Focus();
            }

        }


        public bool ValidarCuponV2()
        {
            bool result = false;
            string cadenaCon = "";
            DataSet dtsConsulta = new DataSet();

            try
            {
                if (Control.Common.GlobalParameters.ConServerPuntos != "")
                {
                    string Query = string.Empty;

                    cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                    Query = string.Concat(Query, "Exec PtsCliente.spCuponApp 'CAB',  " + "'" + txtCodigo.Text + "', 0");
                    dtsConsulta = Common.General.GetDataSet(Query);

                    string TextCupon = string.Empty;

                    if (dtsConsulta.Tables.Count > 0)
                    {
                        int CodError = 0;
                        string MsjError = string.Empty;

                        if (dtsConsulta.Tables[0].Rows.Count > 0)
                        {

                            CodError = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["CodError"].ToString());

                            if(CodError != 0)
                            {
                                _cuponApp = null;
                                btnConfirmar.Enabled = false;

                                MsjError = dtsConsulta.Tables[0].Rows[0]["MsjError"].ToString();
                                TextCupon = dtsConsulta.Tables[0].Rows[0]["TextCupon"].ToString();

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoint/CapturaCupon", "Validar", MsjError);
                                lblValidacion.Text = TextCupon;

                                result = false;
                                return result;

                            }


                            _cuponApp = new ClsCuponApp();
                            _cuponApp.IdTblPremio = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["IdTblPremio"].ToString());
                            _cuponApp.Descripcion = dtsConsulta.Tables[0].Rows[0]["Descripcion"].ToString();
                            _cuponApp.Procesado = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["Procesado"].ToString());
                            _cuponApp.Codigo = dtsConsulta.Tables[0].Rows[0]["Codigo"].ToString();
                            _cuponApp.Valor = decimal.Parse(dtsConsulta.Tables[0].Rows[0]["Valor"].ToString());
                            _cuponApp.CantAplicar = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["CantAplicar"].ToString());

                            TextCupon = dtsConsulta.Tables[0].Rows[0]["TextCupon"].ToString();
                            lblValidacion.Text = TextCupon;

                            btnConfirmar.Enabled = false;
                   
             
                            if(_cuponApp.Procesado == 0)
                            {
                                btnConfirmar.Enabled = true;
                                result = true;

                            }

                            return result;
                        }
                        
                    }

                    return result;
                }
                else {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoint/CapturaCupon", "Validar", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                    result = false;
                }

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/WalletPoint/CapturaCupon", "Validar", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
            }

            return result;
        }


        public bool ValidarCupon()
        {
            bool result = false;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {
                    string Query = "Exec PtsCliente.spCuponApp 'CAB',  " + "'" + txtCodigo.Text + "',0";

                    conn.Open();
                    SqlCommand select = new SqlCommand(Query, conn);
                    IAsyncResult iar = select.BeginExecuteReader();
                    SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                    while (dr.Read())
                    {

                        _cuponApp = new ClsCuponApp();
                        _cuponApp.IdTblPremio = Int32.Parse(dr.GetValue(0).ToString());
                        _cuponApp.Descripcion = dr.GetValue(1).ToString();
                        _cuponApp.Procesado = Int32.Parse(dr.GetValue(2).ToString());
                        _cuponApp.Codigo = dr.GetValue(3).ToString();
                        _cuponApp.Valor = decimal.Parse(dr.GetValue(4).ToString());
                        _cuponApp.CantAplicar = Int32.Parse(dr.GetValue(5).ToString());

                        if (Int32.Parse(dr.GetValue(2).ToString()) == 1)
                        {
                            _cuponApp = null;
                            btnConfirmar.Enabled = false;
                            lblValidacion.Text = "Cupón de Cartilla Digital, Ya fue canjeado.";
                            result = false;
                        }
                        else
                        {
                            btnConfirmar.Enabled = true;
                            lblValidacion.Text = "";
                            result = true;
                        }

                    }

                    if (_cuponApp.Codigo is null)
                    {
                        _cuponApp = null;
                        btnConfirmar.Enabled = false;
                        lblValidacion.Text = "Código de Cartilla Digital no existe, verifique y vuélvalo a intentar";
                        result = false;
                    }

                    result = true;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/WalletPoint/CapturaCupon", "Validar", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/WalletPoint/CapturaCupon", "Validar", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return result;
        }
        private void ValidaArticulo(string CodArticulo)
        {
            string Query = string.Empty;
            string cadenaCon  = Control.Common.GlobalParameters.ConServerPuntos;
            string ITEMNAME = string.Empty;
            // int IdTblPremio = 0;

            DataSet dtsConsulta = new DataSet();
            string IdTblPremio = _cuponApp == null ? "0" : _cuponApp.Codigo;

            try
            {
                Query = string.Concat(Query, "Exec PtsCliente.spValidaArticuloPremioCupon ", Environment.NewLine);
                Query = string.Concat(Query, " @IdTblPremio = ", IdTblPremio.ToString(), Environment.NewLine);
                Query = string.Concat(Query, " , @IdClienteApp = ", IdClienteApp, Environment.NewLine);
                Query = string.Concat(Query, " , @ArticuloBusq = '", CodArticulo, "'", Environment.NewLine);
                dtsConsulta = Common.General.GetDataSet(Query);

                if (dtsConsulta.Tables.Count > 0) {
                    if (dtsConsulta.Tables[0].Rows.Count > 0) {

                        int CodError = Int32.Parse(dtsConsulta.Tables[0].Rows[0]["CodError"].ToString());
                        string MsjError = dtsConsulta.Tables[0].Rows[0]["MsjError"].ToString();


                        this.lblDescArticulo.Text = string.Empty;
                        if (CodError == 0)
                        {
                            ITEMNAME = dtsConsulta.Tables[0].Rows[0]["ITEMNAME"].ToString();
                            ITEMID = dtsConsulta.Tables[0].Rows[0]["ITEMID"].ToString();
                            ITEMBARCODE = dtsConsulta.Tables[0].Rows[0]["ITEMBARCODE"].ToString();

                            this.lblDescArticulo.Text = ITEMNAME;

                            _cuponApp = new ClsCuponApp();
                            _cuponApp.IdTblPremio = Int32.Parse(dtsConsulta.Tables[1].Rows[0]["IdTblPremio"].ToString());
                            this.txtCodigo.ReadOnly = false;

                        }
                        else {

                            this.lblDescArticulo.Text = MsjError;
                            this.txtCodigo.ReadOnly = true;
                        }

                        //MessageBox.Show(MsjError, "Valida Cupón");
                        return;
                    }
                }


            }
            catch (Exception ex)
            {
               
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/WalletPoint/CapturaCupon", "Validar", "No se pudo consultar tarjeta interna, a continuacion el detalle de la excepcion - " + ex.Message);
            }


        }
        private bool ConsumoCupon(int IdTblPremio, string CodigoCupon )
        {
            bool EjecutaConsumo = false;
            string Query = string.Empty;
            DataSet dtsConsulta = new DataSet();

            try
            {
                
                Query = string.Concat(Query, "Exec PtsCliente.SpConsumoCupon ", Environment.NewLine);
                Query = string.Concat(Query, "  @IdTblPremio = ", IdTblPremio.ToString(), Environment.NewLine);
                Query = string.Concat(Query, "  , @CodArticuloConsumo = '", ITEMID.ToString(), "'", Environment.NewLine);
                Query = string.Concat(Query, "  , @CodigoCupon = '", CodigoCupon.ToString(), "'", Environment.NewLine);
                Query = string.Concat(Query, "  , @Almacen = '", Control.Common.GlobalParameters.EstablecimientoAxCode,  "'", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(Query);

                if (dtsConsulta.Tables.Count > 0) {
                    if (dtsConsulta.Tables[0].Rows.Count > 0) {

                    }
                }

                EjecutaConsumo = true;


            }
            catch (Exception)
            {
                EjecutaConsumo = false;
                throw;
            }

            return EjecutaConsumo;

        }

        private void txtCodArticulo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
