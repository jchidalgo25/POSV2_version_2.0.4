using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using POS.Control.PINPAD;
using POS.Models;

namespace POS.UnitTests
{
    public partial class TarjetaPinpadTest : Form
    {
        public TarjetaPinpadTest()
        {
            InitializeComponent();
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            try
            {
                string IPTransaction = "";
                Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                Factura _factura = new Factura() { Establecimiento = txtEstablecimiento.Text.Trim(), PtoEmision = txtPtoEmision.Text.Trim() };

                using (POSEntities pos = new POSEntities())
                {
                    if (true)
                    {
                        var ctb = pos.core_tarjetacredito_bin.Where(x => x.bin == textBox1.Text.Trim().Substring(0, 6)).FirstOrDefault();
                        if (ctb != null)
                        {
                            trama.codRed = ctb.bin_red;
                            if (ctb.bin_red == "2")
                            {
                                IPTransaction = pos.core_parametro.Where(x => x.identificador == "PINPAD_IP_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha;
                                trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                                                                                                                                                                                              //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                            }
                            else
                            {
                                IPTransaction = pos.core_parametro.Where(x => x.identificador == "PINPAD_IP_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                                                                                                                                                                                                       //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
                            }

                        }
                        else
                        {
                            MessageBox.Show(this, "Error en tarjeta");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "Error en PINPAD");
                        return;
                    }
                }

                MessageBox.Show("Terminado sin inconvenientes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                Factura _factura = new Factura() { Establecimiento = txtEstablecimiento.Text.Trim(), PtoEmision = txtPtoEmision.Text.Trim() };

                using (POSEntities pos = new POSEntities())
                {

                    if (true)
                    {
                        var ctb = pos.core_tarjetacredito_bin.Where(x => x.bin == textBox1.Text.Trim().Substring(0, 6)).FirstOrDefault();
                        if (ctb != null)
                        {
                            trama.codRed = ctb.bin_red;
                            if (ctb.bin_red == "2")
                            {
                                trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_MEDIANET" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID;//8 identificador del termninal asignado a la caja
                                //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).IdPago;
                            }
                            else
                            {
                                trama.MID = pos.core_parametro.Where(x => x.identificador == "PINPAD_MID_DATAFAST" && x.valor == _factura.Establecimiento).FirstOrDefault().parametro2;//15 codigo asignado x red blancos a la derecha
                                trama.TID = pos.core_puntoemision.Where(x => x.establecimiento_id == _factura.Establecimiento && x.punto_emision == _factura.PtoEmision).FirstOrDefault().TID_DATAFAST;//8 identificador del termninal asignado a la caja
                                //trama.codDiferido = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData;
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
                }

                MessageBox.Show("Terminado sin inconvenientes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
