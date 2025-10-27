using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Data.SqlClient;
using POS.Models;
using MensajesLibrary;

namespace POS.Control
{
    public partial class CanjePavos : Telerik.WinControls.UI.RadForm
    {
        string cliente;
        string producto;
        string nombrecliente;
        string nombreproducto;
        string codigocanje;
        string cedulacanje;
        string nombrecanje;
        Factura _factura;


        
        private bool _validando; // evita reentradas

        public CanjePavos()
        {
            InitializeComponent();
            // Bloquear letras al tipear
            txtCedula.KeyPress += TxtCedula_KeyPress;

            // Limpiar pegados (Ctrl+V, arrastres, etc.) y cortar a 10
            txtCedula.TextChanging += TxtCedula_TextChanging;

            // Capturar Enter en ambos campos
            txtCodCanje.KeyDown += OnEnterPressed;
            txtProducto.KeyDown += OnEnterPressed;

            // Recomendado: asegurarte que no sean Multiline
            txtCodCanje.Multiline = false;
            txtProducto.Multiline = false;
        }

        public CanjePavos(Factura factura) : this()
        {
            _factura = factura ?? throw new ArgumentNullException(nameof(factura));
            txtCedula.Focus();
        }

        private void CanjePavos_Load(object sender, EventArgs e)
        {

        }

        private void TxtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void TxtCedula_TextChanging(object sender, Telerik.WinControls.TextChangingEventArgs e)
        {
            var digits = new string(e.NewValue.Where(char.IsDigit).Take(10).ToArray());
            if (digits != e.NewValue)
            {
                e.Cancel = true;                 // cancela el cambio inválido
                txtCedula.Text = digits;         // aplica solo dígitos
                txtCedula.SelectionStart = txtCedula.Text.Length;
            }
        }

        private void OnEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter || e.KeyCode == Keys.Tab) return;

            e.SuppressKeyPress = true; // evita el “beep”
            e.Handled = true;
            
            // Si estoy en Código y el producto está vacío, pasar el foco al producto
            if (sender == txtCodCanje && string.IsNullOrWhiteSpace(txtProducto.Text) && !string.IsNullOrWhiteSpace(txtCodCanje.Text))
            {
                txtProducto.Focus();
                
            }

            // Si estoy en Producto y el código está vacío, pasar el foco al código
            if (sender == txtProducto && string.IsNullOrWhiteSpace(txtCodCanje.Text))
            {
                if (string.IsNullOrWhiteSpace(txtCedula.Text))
                {
                    Control.Common.General.GetMensajeToList(702);
                    txtProducto.Text = "";

                    lblItem.Text = "";

                    txtCedula.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    Control.Common.General.GetMensajeToList(703);
                    txtProducto.Text = "";

                    lblItem.Text = "";

                    txtNombre.Focus();
                    return;
                }
                Control.Common.General.GetMensajeToList(696);
                
                txtProducto.Text = "";
                
                lblItem.Text = "";
               
                txtCodCanje.Focus();
                return;
            }

            TryValidateNow();
        }

        private void TryValidateNow()
        {
            if (_validando) return;
            
            var cod = txtCodCanje.Text?.Trim();
            var prod = txtProducto.Text?.Trim();
            //if (string.IsNullOrWhiteSpace(cod) || string.IsNullOrWhiteSpace(prod)) return;

            try
            {
                _validando = true;
                // Reutiliza tu lógica existente del botón:
                btnValidar_Click(btnValidar, EventArgs.Empty);
                // o: btnValidar.PerformClick();
            }
            finally
            {
                _validando = false;
            }
        }

    private void btnValidar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtCedula.Text))
            {
                Control.Common.General.GetMensajeToList(702);
                txtCodCanje.Text = "";
                txtCedula.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                Control.Common.General.GetMensajeToList(703);
                txtCodCanje.Text = "";
                txtNombre.Focus();
                return;
            }
            //else
            //{
            

            if (validarCodigo(txtCodCanje.Text, txtProducto.Text) && !string.IsNullOrWhiteSpace(txtProducto.Text))
                {
                    //pedir pesaje de item
                    string EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode;
                    var balanzaForm = new Control.Peso.TomaPesoUI(lblItem.Text, _factura);
                    balanzaForm._estab = _factura.Establecimiento;
                    balanzaForm._modeloBalanza = _factura.ModeloBalanza;
                    balanzaForm.StartPosition = FormStartPosition.CenterParent;
                    balanzaForm.ShowDialog(this);

                    //if (!balanzaForm.correcto)
                    //{
                    //    MessageBox.Show("No se capturó el peso. Intenta nuevamente.");
                    //    return;
                    //}

                    decimal pesoLb = balanzaForm.Peso;

                    decimal pesoKg = (pesoLb * 0.453592M);

                    lblPeso.Text = pesoKg.ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";

                //si peso es correcto o menor entrega item y crea ov y/o remision

                //si peso es mayor mensaje de error
                    this.codigocanje = txtCodCanje.Text;
                this.cedulacanje = txtCedula.Text;
                this.nombrecanje = txtNombre.Text;
                txtCedula.Text = "";
                txtNombre.Text = "";
                txtCodCanje.Text = "";
                txtProducto.Text = "";
                lblCliente.Text = "";
                lblItem.Text = "";
                lblNombre.Text = "";
                lblPeso.Text = "";

                
                if (this.validarPeso(this.producto, pesoKg))
                    {
                    
                    this.crearOV_AddItemAX(EstablecimientoAxCode, this.codigocanje, pesoKg);
                    }
                    
                    
                //}
            }
        }

        private bool validarCodigo(string codigo, string producto="")
        {

            bool result = false;
            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                SqlConnection conn = new SqlConnection(cadenaCon);
                try
                {
                    string Query = "Exec dbo.[spValidaCanjePavo] '" + codigo + "', '" + producto + "'";

                    conn.Open();
                    SqlCommand select = new SqlCommand(Query, conn);
                    IAsyncResult iar = select.BeginExecuteReader();
                    SqlDataReader dr = (SqlDataReader)select.EndExecuteReader(iar);
                    while (dr.Read())
                    {

                        int msg = dr.IsDBNull(0) ? 694 : dr.GetInt32(0);
                        cliente = dr.IsDBNull(1) ? null : dr.GetString(1);
                        this.producto = dr.IsDBNull(2) ? null : dr.GetString(2);
                        nombrecliente = dr.IsDBNull(3) ? null : dr.GetString(3);
                        nombreproducto = dr.IsDBNull(4) ? null : dr.GetString(4);


                        if (msg != 0)
                        {
                            if (msg == 694)
                                txtCodCanje.Focus();
                            Control.Common.General.GetMensajeToList(msg);
                            txtCodCanje.Text = "";
                            txtProducto.Text = "";
                            lblCliente.Text = "";
                            lblItem.Text = "";
                            lblNombre.Text = "";
                            
                            return false;
                        }

                        lblCliente.Text = cliente;
                        lblNombre.Text = nombrecliente.ToUpper();
                        lblItem.Text = nombreproducto.ToUpper();

                    }


                    result = true;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    conn.Close();
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Control/CanjePavos", "Validar", "No se pudo validar codigo de canje de Pavo, a continuacion el detalle de la excepcion - " + ex.Message);
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "Control/CanjePavos", "Validar", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
            }

            return result;
        }
        
        private bool validarPeso(string producto, decimal pesoKg)
            {
                string cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                
                if (string.IsNullOrWhiteSpace(cadenaCon))
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "Control/CanjePavos",
                        "ValidarPeso",
                        "No hay parámetro 'CON_SERVER_PUNTOS' para este local o está vacío.");
                    return false;
                }

                try
                {
                    using (var conn = new SqlConnection(cadenaCon))
                    using (var cmd = new SqlCommand("dbo.spValidarPesoItem", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // @ItemId nvarchar(20)
                        cmd.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20)
                           .Value = (object)producto ?? string.Empty;

                        // @PesoKg decimal(9,3)  -> usa decimal en .NET y fija precision/escala
                        var pPeso = cmd.Parameters.Add("@PesoKg", SqlDbType.Decimal).Value = pesoKg;

                        // OUTPUTS
                        var pValido = cmd.Parameters.Add("@Valido", SqlDbType.Bit);
                        pValido.Direction = ParameterDirection.Output;

                        var pDif = cmd.Parameters.Add("@DiferenciaKg", SqlDbType.Decimal);
                        pDif.Precision = 9;
                        pDif.Scale = 3;
                        pDif.Direction = ParameterDirection.Output;

                        var pMsg = cmd.Parameters.Add("@Mensaje", SqlDbType.NVarChar, 200);
                        pMsg.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        bool valido = (pValido.Value != DBNull.Value) && Convert.ToBoolean(pValido.Value);
                        decimal diferenciaKg = (pDif.Value == DBNull.Value) ? 0m : (decimal)pDif.Value;
                        string mensaje = pMsg.Value?.ToString() ?? string.Empty;

                        // valido == false  -> excede máximo (bloquear)
                        if(!valido)
                        {
                            var parametros = new List<ParametrosMensajes>
                            {
                                new ParametrosMensajes { codigo = "[DIFERENCIA]", valor = diferenciaKg.ToString("N2") }
                            };
                            Control.Common.General.GetMensajeToList(690, parametros);
                            txtCodCanje.Text = "";
                            txtProducto.Text = "";
                            lblCliente.Text = "";
                            lblItem.Text = "";
                            lblNombre.Text = "";
                            lblPeso.Text = "";

                        //MessageBox.Show("El Pavo excede el peso permitido en "+ diferenciaKg+" Kg");
                        return false;

                        }
                        else
                        {
                        // valido == true && diferenciaKg > 0 -> bajo mínimo permitido
                            if (diferenciaKg > 0)
                            {
                                var parametros = new List<ParametrosMensajes>
                                {
                                    new ParametrosMensajes { codigo = "[DIFERENCIA]", valor = diferenciaKg.ToString("N2") }
                                };
                                var result = Control.Common.General.GetMensajeToList(691, parametros);

                                if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                                {
                                    Control.Common.General.GetMensajeToList(692);
                                    txtCedula.Text = "";
                                    txtNombre.Text = "";
                                    txtCodCanje.Text = "";
                                    txtProducto.Text = "";
                                    lblCliente.Text = "";
                                    lblItem.Text = "";
                                    lblNombre.Text = "";
                                    lblPeso.Text = "";
                                txtCedula.Focus();
                            }
                                else
                                {
                                    Control.Common.General.GetMensajeToList(700);
                                    txtCodCanje.Text = "";
                                    txtProducto.Text = "";
                                    lblCliente.Text = "";
                                    lblItem.Text = "";
                                    lblNombre.Text = "";
                                    lblPeso.Text = "";
                                return false;
                                }

                            }
                            else
                            {
                                Control.Common.General.GetMensajeToList(692);
                     

                        }
                        txtCedula.Text = "";
                        txtNombre.Text = "";
                        txtCodCanje.Text = "";
                        txtProducto.Text = "";
                        lblCliente.Text = "";
                        lblItem.Text = "";
                        lblNombre.Text = "";
                        lblPeso.Text = "";
                        txtCedula.Focus();
                        return true;
                        
                        }

                    // valido == true && diferenciaKg == 0 -> dentro de rango
                    Control.Common.Logger.LogMessage(
                            Control.Common.Enum.LogTypes.Info,
                            "Control/CanjePavos",
                            "ValidarPeso",
                            $"Valido={valido}, DiferenciaKg={diferenciaKg}, Mensaje='{mensaje}'");
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        "Control/CanjePavos",
                        "ValidarPeso",
                        "No se pudo validar peso de Pavo. Detalle: " + ex.Message);
                return false;
                }
            }

        private bool crearOV_AddItemAX(string inventlocation, string barcode, decimal pesoKg)
            {
                string cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                
                if (string.IsNullOrWhiteSpace(cadenaCon))
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "Control/CapturaCupon",
                        "Validar",
                        "No hay parametro 'CON_SERVER_PUNTOS' para este local o está vacío ");
                    return false;
                }

                try
                {
                    using (var conn = new SqlConnection(cadenaCon))
                    using (var cmd = new SqlCommand("dbo.spCanjeaPavo", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        // Opcional: ajustar timeout si el WS de AX puede tardar
                        // cmd.CommandTimeout = 60;

                        // @inventlocation varchar(15)
                        cmd.Parameters.Add("@inventlocation", SqlDbType.VarChar, 15)
                           .Value = (object)inventlocation ?? string.Empty;

                        // @barCode varchar(100)
                        cmd.Parameters.Add("@barCode", SqlDbType.VarChar, 100)
                           .Value = (object)barcode ?? string.Empty;

                        cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 100)
                           .Value = (object)cedulacanje ?? string.Empty;
                        cmd.Parameters.Add("@nombre", SqlDbType.VarChar, 100)
                               .Value = (object)nombrecanje ?? string.Empty;

                    // @qty decimal  -> usa decimal con escala (ej. 3) para no perder decimales
                    var pQty = cmd.Parameters.Add("@qty", SqlDbType.Decimal);
                        pQty.Precision = 18;          // o la que uses en SQL
                        pQty.Scale = 2;               // 2 decimales
                        //pQty.Value = decimal.Round(pesoLb, 2, MidpointRounding.AwayFromZero);
                        pQty.Value = decimal.Round(pesoKg, 2, MidpointRounding.AwayFromZero);



                    // @Respuesta varchar(2000) OUTPUT
                    var pOut = cmd.Parameters.Add("@Respuesta", SqlDbType.VarChar, 2000);
                        pOut.Direction = ParameterDirection.Output;

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        string respuesta = pOut.Value?.ToString()?.Trim() ?? string.Empty;

                        bool ok = string.Equals(respuesta, "OK", StringComparison.OrdinalIgnoreCase);
                        if (!ok)
                        {
                        // Loguea el detalle devuelto por el SP (errores del WS, etc.)
                        Control.Common.General.GetMensajeToList(699);
                        txtCedula.Text = "";
                        txtNombre.Text = "";
                        txtCodCanje.Text = "";
                        txtProducto.Text = "";
                        lblCliente.Text = "";
                        lblItem.Text = "";
                        lblNombre.Text = "";
                        lblPeso.Text = "";
                        Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                "Control/CanjePavos",
                                "crearOV_AddItemAX",
                                $"Respuesta SP spCanjeaPavo: '{respuesta}'");
                        return false;
                        }

                    Control.Common.General.GetMensajeToList(698);
                    
                    return ok;
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Error,
                        "Control/CanjePavos",
                        "crearOV_AddItemAX",
                        "No se pudo ejecutar spCanjeaPavo. Detalle: " + ex.Message);
                    return false;
                }
            }


}
}
