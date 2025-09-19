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

namespace POS.UnitTests
{
    public partial class DummyForm : Form
    {
        public DummyForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestPadLeftVoucher();
            //EncryptPwd();
            //OpenDBContextWithDecryptedPassword();
        }

        private void OpenDBContextWithDecryptedPassword()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var cantidad = db.core_parametro.ToList().Count();

                    MessageBox.Show("Cantidad parametros: " + cantidad.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void EncryptPwd()
        {
            var decrypter = new Control.Common.Encriptador();
            textBox1.Text = decrypter.Encriptar("a17472Ol0");
        }

        private void TestTexto()
        {
            MessageBox.Show("Un texto" + "||" + ("01" == "02" ? "MEDIANET" : "DATAFAST"));
        }

        private void IntentaParsearFecha()
        {
            try
            {
                string formatoFechaCorta = "yyyy/MM/dd";
                string texto = DateTime.Parse(textBox1.Text.Trim()).ToString(formatoFechaCorta);
                MessageBox.Show(texto);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No");
            }
        }

        //private void CompruebaValidacionAcumulaPtosBines()
        //{
        //    try
        //    {
        //        Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva = true;

        //        var _factura = new POS.Models.Factura();
        //        POS.Control.POS.init(ref _factura);
        //        core_tarjetacreditointerno tarjeta = null;
        //        core_tarjetacreditointerno tarjetaAdicional = null;
        //        _factura.ClienteIdentificacion = "0926596578";
        //        var cliente_actual = POS.Control.Cliente.getCliente(_factura.ClienteIdentificacion, out tarjeta, out tarjetaAdicional);
        //        _factura.User = new User { isSuperUser = true, nombres = "test", username = "1234", result = true };

        //        string bin_descripcion = "";

        //        using (POSEntities db = new POSEntities())
        //        {
        //            var ctb = db.core_tarjetacredito_bin.Where(x => x.bin == textBox1.Text.Trim()).FirstOrDefault();

        //            if (ctb != null)
        //            {
        //                bin_descripcion = ctb.bin_descripcion;
        //            }
        //            else
        //            {
        //                MessageBox.Show(this, "Tarjeta no se encuentra en listado de bines");
        //                return;
        //            }
        //        }

        //        _factura.AgregarPagoTarjetaCredito(1, "Banco", "Nombre", "Marca", "MEDIANET", bin_descripcion);
                
        //        //Acumular puntos
        //        var acumuladorPuntos = new Control.WalletPoints.ClsAcumulacion();
        //        acumuladorPuntos.Acumular(ref _factura);

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Yuca");
        //    }
        //}

        private void TestPadLeftVoucher()
        {
            try
            {
                decimal base12 = Decimal.Parse(textBox1.Text.Trim());
                base12 = Decimal.Round(base12 * 1, 2, MidpointRounding.AwayFromZero);

                var a = (base12).ToString("N2").Replace(".", "").PadLeft(12, '0');
                var b = a.PadLeft(13, '0');

                MessageBox.Show(string.Format("Pad12: {0}({1}); Pad13: {2}({3})", a, a.Length.ToString(), b, b.Length.ToString()));
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo");
            }
        }
    }
}
