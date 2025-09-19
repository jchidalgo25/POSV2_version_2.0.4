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
    public partial class EmuladorPOSVOUCHER : Form
    {
        public EmuladorPOSVOUCHER()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                Tramas.ProcesaPago trama = new Tramas.ProcesaPago();
                Tramas.RespuestaProcesoPago resptrama = new Tramas.RespuestaProcesoPago();
                resptrama.ObtieneDato = richTextBox1.Text;

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
                    pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumo; //2 ;
                else
                    pos_voucher.TIPOCONSUMO = ((pos_tarjeta_tipopago)cmbBancoTarjeta.SelectedValue).tipoConsumoData; //2 ;
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
                pos_voucher.PUNTOEMISION = _factura.Establecimiento + _factura.PtoEmision;
                pos_voucher.PROCESADO = false;
                pos_voucher.GRUPOTAR = resptrama.nomGruTar;
                pos_voucher.AUTORIZADOR = int.Parse(resptrama.codigoRed);
                pos_voucher.LOTE = resptrama.numerolote;
                pos_voucher.FACTURA = _factura.getNumeroFactura();
                //ML [16/01/2018]: Nuevos campos
                pos_voucher.ARQC = resptrama.ARQC;
                pos_voucher.AIDEMV = resptrama.AIDEMV;
                pos_voucher.EMV = resptrama.idEMV;
                pos_voucher.TC = resptrama.tipoCritoyValorEMV;
                pos_voucher.PUBLICIDAD = resptrama.mensajePremioPublicidad;
                pos_voucher.TIPOTRANSACCION = trama.TipoTransaccion;
                pos_voucher.BANCOADQUIRIENTE = resptrama.nomBancoAdq;
                pos_voucher.TARJETAHABIENTE = resptrama.nombreTarjetaHabiente;
                pos_voucher.MID = trama.MID;
                pos_voucher.TID = trama.TID;
                pos_voucher.VENCTAR = resptrama.codigoRed == "02" ? resptrama.fechaVencTar.Substring(0, 2) + "/" + resptrama.fechaVencTar.Substring(2, 2) : "XX/XX";
                pos_voucher.ANULAUTORIZACION = trama.numAutorizacion;
                pos_voucher.TIPOBANCOTARJETA = cmbBancoTarjeta.Text;
            }
            catch (Exception ex)
            {
                ex = ex;
            }

            */
        }


    }
}
