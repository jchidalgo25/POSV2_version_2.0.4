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

namespace POS.Control.Pagos
{
    public partial class CalculoPago : Telerik.WinControls.UI.RadForm
    {

        Factura _factura;
        private Timer _autoCloseTimer;

        public CalculoPago(Factura factura)
        {
            InitializeComponent();
            this._factura = factura;

        }

  
        private void AutoCloseTimer_Tick(object sender, EventArgs e)
        {
            _autoCloseTimer.Stop(); // Detener el timer
            this.Close(); // Cerrar el formulario
        }

        private void CalculoPago_Load(object sender, EventArgs e)
        {
            List<ItemMostrar> list = new List<ItemMostrar>();

            list.Add(new ItemMostrar() { Descripcion = "Subtotal", Porcentaje = "", Valor = _factura.getSubTotalSinDescuento() });
            list.Add(new ItemMostrar() { Descripcion = "Descuento", Porcentaje = "", Valor = _factura.GetDescuentos() });
            foreach (var d in _factura.Descuentos2)
            {
                list.Add(new ItemMostrar() { Descripcion = "Descuento por " + d.Tipo, Porcentaje = d.Porcentaje.ToString(), Valor = d.Valor });
            }
            list.Add(new ItemMostrar() { Descripcion = "I.V.A", Porcentaje = "", Valor = _factura.getIVA() });

            decimal total = _factura.GetTotal();

            if (_factura.aplicaBeneficioDevolucionIVA)
            {
                decimal valorIVA = _factura.getIVA();
                decimal montoIvaDevolver = _factura.montoIvaDevolver;               

                list.Add(new ItemMostrar()
                {
                    Descripcion = "Devolución IVA",
                    Porcentaje = "",
                    Valor = montoIvaDevolver
                });
            }


            list.Add(new ItemMostrar() { Descripcion = "TOTAL", Porcentaje = "", Valor = total });
            gridPagos.DataSource = list;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridPagos_CellFormatting(object sender, Telerik.WinControls.UI.CellFormattingEventArgs e)
        {
            var new_font = new Font("Segoe UI", 14);
            e.CellElement.Font = new_font;
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;

                case Keys.Enter:
                    this.Close();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }
    }

    public class ItemMostrar
    {
        string _descripcion;

        public string Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }
        string _porcentaje;

        public string Porcentaje
        {
            get { return _porcentaje; }
            set { _porcentaje = value; }
        }
        decimal _valor;

        public decimal Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }
    }
}
