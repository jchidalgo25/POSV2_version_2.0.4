using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS.Control.Common;

namespace POS.Control.PINPAD
{
    public class Tramas // : INotifyPropertyChanged
    {
        
        #region ConsultaTarjeta
        public struct ConsultaTarjeta
        {
            const string _tipoMensaje = "CT";
            string _tipotran;
            string _codRed;
            string _codDiferido;
            string _plazoDiferido;
            string _mesGracia;
            const string _filler = " ";
            string _montoTotalTransaccion;
            string _montoBaseGravaIVa;
            string _montoBaseNoGravaIVa;
            string _impuestoIvaTransaccion;
            string _impuestoServicioTransaccion;
            string _popinaTransaccion;
            string _montoFijo;
            string _secuencialTransaccion;
            string _horaTransccion;
            string _fechaTransaccion;
            string _numAutorizacion;
            string _MID;
            string _TID;
            string _CID;
            string _filler2;
            string _OTT;
            string _OTTProveedor;
            string _Factura;
            string _PushBanco;
            string _PushCodigo;
            string _FillerFin;
            string _IdVendedor;
            string _facturaComprobante;

            public string TipoMensaje
            {
                get { return string.IsNullOrEmpty(_tipoMensaje) ? "" : _tipoMensaje; }
                //set { _tipoMensaje = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string TipoTransaccion
            {
                get { return _tipotran; }
                set { _tipotran = value; _filler2 = " "; }
            }

            public string codRed
            {
                get { return string.IsNullOrEmpty(_codRed) ? "" : _codRed; }
                set { _codRed = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string codDiferido
            {
                get { return string.IsNullOrEmpty(_codDiferido) ? "" : _codDiferido; }
                set { _codDiferido = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string plazoDiferido
            {
                get { return string.IsNullOrEmpty(_plazoDiferido) ? "" : _plazoDiferido; }
                set { _plazoDiferido = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string mesGracia
            {
                get { return string.IsNullOrEmpty(_mesGracia) ? "" : _mesGracia; }
                set { _mesGracia = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoTotalTransaccion
            {
                get { return string.IsNullOrEmpty(_montoTotalTransaccion) ? "" : _montoTotalTransaccion; }
                set { _montoTotalTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoBaseGravaIVa
            {
                get { return string.IsNullOrEmpty(_montoBaseGravaIVa) ? "" : _montoBaseGravaIVa; }
                set { _montoBaseGravaIVa = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoBaseNoGravaIVa
            {
                get { return string.IsNullOrEmpty(_montoBaseNoGravaIVa) ? "" : _montoBaseNoGravaIVa; }
                set { _montoBaseNoGravaIVa = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string impuestoIvaTransaccion
            {
                get { return string.IsNullOrEmpty(_impuestoIvaTransaccion) ? "" : _impuestoIvaTransaccion; }
                set { _impuestoIvaTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string impuestoServicioTransaccion
            {
                get { return string.IsNullOrEmpty(_impuestoServicioTransaccion) ? "" : _impuestoServicioTransaccion; }
                set { _impuestoServicioTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string popinaTransaccion
            {
                get { return string.IsNullOrEmpty(_popinaTransaccion) ? "" : _popinaTransaccion; }
                set { _popinaTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoFijo
            {
                get { return string.IsNullOrEmpty(_montoFijo) ? "" : _montoFijo; }
                set { _montoFijo = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string secuencialTransaccion
            {
                get { return string.IsNullOrEmpty(_secuencialTransaccion) ? "" : _secuencialTransaccion; }
                set { _secuencialTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }


            public string horaTransccion
            {
                get { return string.IsNullOrEmpty(_horaTransccion) ? "" : _horaTransccion; }
                set { _horaTransccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string fechaTransaccion
            {
                get { return string.IsNullOrEmpty(_fechaTransaccion) ? "" : _fechaTransaccion; }
                set { _fechaTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string numAutorizacion
            {
                get { return string.IsNullOrEmpty(_numAutorizacion) ? "" : _numAutorizacion; }
                set { _numAutorizacion = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string MID
            {
                get { return string.IsNullOrEmpty(_MID) ? "" : _MID; }
                set { _MID = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string TID
            {
                get { return string.IsNullOrEmpty(_TID) ? "" : _TID; }
                set { _TID = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string CID
            {
                get { return string.IsNullOrEmpty(_CID) ? "" : _CID; }
                set { _CID = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string OTT
            {
                get { return string.IsNullOrEmpty(_OTT) ? "" : _OTT; }
                set { _OTT = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string OTTProveedor
            {
                get { return string.IsNullOrEmpty(_OTTProveedor) ? "" : _OTTProveedor; }
                set { _OTTProveedor = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string Factura
            {
                get { return string.IsNullOrEmpty(_Factura) ? "" : _Factura; }
                set { _Factura = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string PushBanco
            {
                get { return string.IsNullOrEmpty(_PushBanco) ? "" : _PushBanco; }
                set { _PushBanco = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string PushCodigo
            {
                get { return string.IsNullOrEmpty(_PushCodigo) ? "" : _PushCodigo; }
                set { _PushCodigo = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string FillerFin
            {
                get { return string.IsNullOrEmpty(_FillerFin) ? "" : _FillerFin; }
                set { _FillerFin = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string IdVendedor
            {
                get { return string.IsNullOrEmpty(_IdVendedor) ? "" : _IdVendedor; }
                set { _IdVendedor = string.IsNullOrEmpty(value) ? "" : value; }
            }


            public string FacturaComprobante
            {
                get { return string.IsNullOrEmpty(_facturaComprobante) ? "" : _facturaComprobante; }
                set { _facturaComprobante = string.IsNullOrEmpty(value) ? "" : value; }
            }

       
            public string DevuelveTramaCT
            {

                get
                {
                    string _trama = string.Empty;

                    _OTT = string.IsNullOrEmpty(_OTT) ? "" : _OTT;
                    _facturaComprobante = string.IsNullOrEmpty(_facturaComprobante) ? "" : _facturaComprobante;
                    _PushCodigo = string.IsNullOrEmpty(_PushCodigo) ? "" : _PushCodigo;
                    _PushBanco = string.IsNullOrEmpty(_PushBanco) ? "" : _PushBanco;
                    _IdVendedor = string.IsNullOrEmpty(_IdVendedor) ? "" : _IdVendedor;
                    //_plazoDiferido = (_plazoDiferido).PadLeft(2, '0') == "00" ? "  " : (_plazoDiferido).PadLeft(2, '0');
                    //_mesGracia = (_mesGracia).PadLeft(2, '0') == "00" ? "  " : (_mesGracia).PadLeft(2, '0');
                    _montoFijo = _montoFijo == "" ? "  " : (_montoFijo).PadLeft(12, ' ');

                    _plazoDiferido = string.IsNullOrEmpty(_plazoDiferido) ? " " : _plazoDiferido;
                    _mesGracia = string.IsNullOrEmpty(_mesGracia) ? " " : _mesGracia;
                    //_mesGracia = "00";

                    _trama = string.Empty;
                    _trama = string.Concat(_trama, _tipoMensaje);
                    _trama = string.Concat(_trama, (_tipotran).Substring(0, 2));
                    _trama = string.Concat(_trama, (_codRed).PadRight(1, '0'));
                    _trama = string.Concat(_trama, (_codDiferido).PadRight(2, '0'));
                    _trama = string.Concat(_trama, (_plazoDiferido).PadLeft(2, ' '));
                    _trama = string.Concat(_trama, (_mesGracia).PadRight(2, ' '));
                    _trama = string.Concat(_trama, _filler);
                    _trama = string.Concat(_trama, (_montoTotalTransaccion).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_montoBaseGravaIVa).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_montoBaseNoGravaIVa).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_impuestoIvaTransaccion).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_impuestoServicioTransaccion).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_popinaTransaccion).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_montoFijo).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_secuencialTransaccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_horaTransccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_fechaTransaccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_numAutorizacion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_MID).PadRight(15, ' '));
                    _trama = string.Concat(_trama, (_TID).PadRight(8, ' '));
                    _trama = string.Concat(_trama, (_CID).PadRight(15, ' '));
                    _trama = string.Concat(_trama, (_filler2).PadRight(20, ' '));
                    //_trama = string.Concat(_trama, (_filler2).PadRight(1, ' '));
                    return _trama;
                }
            }

        }
        public struct RespuestaConsultaTarjeta
        {
            string _obtieneDato;
            string _codigoRespuesta;
            string _codigoRed;
            string _codAutorizador;
            string _mensajeRespuesta;
            string _secuencialtransaccion;
            string _numerolote;
            string _horaTrans;
            string _fechaTrans;
            string _numAut;
            string _termId;
            string _merchantId;
            string _valInteres;
            string _mensajePremioPublicidad;//imprimir al final del voucher
            string _codBancoAdq;
            string _nomBancoAdq;
            string _nomGruTar;
            string _modoLectura;
            string _nombreTarjetaHabiente;
            string _montoFijo;
            string _idEMV;//imprimir al final del voucher
            string _AIDEMV;//imprimir al final del voucher
            string _tipoCritoyValorEMV;//imprimir al final del voucher
            string _verificacionPin;//imprimir al final del voucher
            string _ARQC;
            string _numTarTuncate;
            string _fechaVencTar;
            string _numtarEncrip;
            string _TVR;//imprimir al final del voucher
            string _TSI;//imprimir al final del voucher

            public string ObtieneDato
            {
                set
                {
                    _obtieneDato = value.PadRight(450, ' ');
                    _codigoRespuesta = (_obtieneDato).Substring(2, 2);
                    _codigoRed = (_obtieneDato).Substring(4, 2);
                    _codAutorizador = (_obtieneDato).Substring(6, 2);
                    _mensajeRespuesta = (_obtieneDato).Substring(8, 20);
                    _secuencialtransaccion = (_obtieneDato).Substring(28, 6);
                    _numerolote = (_obtieneDato).Substring(34, 6);
                    _horaTrans = (_obtieneDato).Substring(40, 6);
                    _fechaTrans = (_obtieneDato).Substring(46, 8);
                    _numAut = (_obtieneDato).Substring(54, 6);
                    _termId = (_obtieneDato).Substring(60, 8);
                    _merchantId = (_obtieneDato).Substring(68, 15);
                    _valInteres = (_obtieneDato).Substring(83, 12);
                    _mensajePremioPublicidad = (_obtieneDato).Substring(95, 80);//imprimir al final del voucher
                    _codBancoAdq = (_obtieneDato).Substring(175, 3);
                    _nomBancoAdq = (_obtieneDato).Substring(178, 30);
                    _nomGruTar = (_obtieneDato).Substring(208, 25);
                    _modoLectura = (_obtieneDato).Substring(233, 2);
                    _nombreTarjetaHabiente = (_obtieneDato).Substring(235, 40);
                    _montoFijo = (_obtieneDato).Substring(275, 12);
                    _idEMV = (_obtieneDato).Substring(287, 20);//imprimir al final del voucher
                    _AIDEMV = (_obtieneDato).Substring(307, 20);//imprimir al final del voucher
                    _tipoCritoyValorEMV = (_obtieneDato).Substring(327, 22);//imprimir al final del voucher
                    _verificacionPin = (_obtieneDato).Substring(349, 15);//imprimir al final del voucher
                    _ARQC = (_obtieneDato).Substring(364, 16);

                    _numTarTuncate = (_obtieneDato).Substring(380, 25);
                    _fechaVencTar = (_obtieneDato).Substring(405, 4);
                    _numtarEncrip = (_obtieneDato).Substring(409, 40);

                    //Nueva trama cuando se procese por TCP IP.   JM  25-08-2020
                    if (Control.Common.GlobalParameters.EstTcpIpPinpad)
                    {
                        _TVR = (_obtieneDato).Substring(380, 10);
                        _TSI = (_obtieneDato).Substring(390, 4);
                        _numTarTuncate = (_obtieneDato).Substring(394, 25);
                        _fechaVencTar = (_obtieneDato).Substring(419, 4);

                        int leerTramaRes = 40;
                        if (_obtieneDato.Length > 480)
                        {
                            leerTramaRes = leerTramaRes + 24;
                        }
                        _numtarEncrip = (_obtieneDato).Substring(423, leerTramaRes);
                    }
                    /*
                    var charArr = _numTarTuncate.Trim().ToArray();
                    var tarj_mask = "";
                    for (int i = 0; i < charArr.Length; i++)
                    {
                        tarj_mask += (i<6 || charArr.Length-3 <= i) ? charArr[i].ToString() : "X";
                    }
                    _numTarTuncate = tarj_mask;
                    */
                }
            }

            public void SetValuesTest(string codigoRespuesta,
                                      string codigoRed,
                                      string codigoAutorizador,
                                      string msjRespuesta,
                                      string secuencialTransaccion,
                                      string numeroLote,
                                      string fechaTrans,
                                      string horaTrans,
                                      string numAut,
                                      string termId,
                                      string merchantId,
                                      string valInteres,
                                      string msjPremioPublicidad,
                                      string codBancoAdq,
                                      string nomBancoAdq,
                                      string nomGruTar,
                                      string modoLectura,
                                      string nombreTarjetaHabiente,
                                      string montoFijo,
                                      string idEMV,
                                      string aidEMV,
                                      string tipoCritoyValorEMV,
                                      string verificacionPin,
                                      string arqc,
                                      string numTarTuncate,
                                      string fechaVencTar,
                                      string numtarEncrip)
            {
                _obtieneDato = "";
                _codigoRespuesta = codigoRespuesta;
                _codigoRed = codigoRed;
                _codAutorizador = codigoAutorizador;
                _mensajeRespuesta = msjRespuesta;
                _secuencialtransaccion = secuencialTransaccion;
                _numerolote = numeroLote;
                _horaTrans = horaTrans;
                _fechaTrans = fechaTrans;
                _numAut = numAut;
                _termId = termId;
                _merchantId = merchantId;
                _valInteres = valInteres;
                _mensajePremioPublicidad = msjPremioPublicidad;
                _codBancoAdq = codBancoAdq;
                _nomBancoAdq = nomBancoAdq;
                _nomGruTar = nomGruTar;
                _modoLectura = modoLectura;
                _nombreTarjetaHabiente = nombreTarjetaHabiente;
                _montoFijo = montoFijo;
                _idEMV = idEMV;
                _AIDEMV = aidEMV;
                _tipoCritoyValorEMV = tipoCritoyValorEMV;
                _verificacionPin = verificacionPin;
                _ARQC = arqc;
                _numTarTuncate = numTarTuncate;
                _fechaVencTar = fechaVencTar;
                _numtarEncrip = numtarEncrip;
            }


            public string codigoRespuesta { get { return _codigoRespuesta; } }
            public string codigoRed { get { return _codigoRed; } }
            public string codAutorizador { get { return _codAutorizador; } }
            public string mensajeRespuesta { get { return _mensajeRespuesta; } }
            public string secuencialtransaccion { get { return _secuencialtransaccion; } }
            public string numerolote { get { return _numerolote; } }
            public string horaTrans { get { return _horaTrans; } }
            public string fechaTrans { get { return _fechaTrans; } }
            public string numAut { get { return _numAut; } }
            public string termId { get { return _termId; } }
            public string merchantId { get { return _merchantId; } }
            public string valInteres { get { return _valInteres; } }
            public string mensajePremioPublicidad { get { return _mensajePremioPublicidad; } }//imprimir al final del voucher
            public string codBancoAdq { get { return _codBancoAdq; } }
            public string nomBancoAdq { get { return _nomBancoAdq; } }
            public string nomGruTar { get { return _nomGruTar; } }
            public string modoLectura { get { return _modoLectura; } }
            public string nombreTarjetaHabiente { get { return _nombreTarjetaHabiente; } }
            public string montoFijo { get { return _montoFijo; } }
            public string idEMV { get { return _idEMV; } }//imprimir al final del voucher
            public string AIDEMV { get { return _AIDEMV; } }//imprimir al final del voucher
            public string tipoCritoyValorEMV { get { return _tipoCritoyValorEMV; } }//imprimir al final del voucher
            public string verificacionPin { get { return _verificacionPin; } }//imprimir al final del voucher
            public string ARQC { get { return _ARQC; } }
            public string numTarTuncate { get { return _numTarTuncate; } }
            public string fechaVencTar { get { return _fechaVencTar; } }
            public string numtarEncrip { get { return _numtarEncrip; } }
            public string TVR { get { return _TVR; } }
            public string TSI { get { return _TSI; } }

        }
        #endregion

        #region ProcesaPago    
        public struct ProcesaPago
        {

            string _tipoMensaje;
            string _tipotran;
            string _codRed;
            string _codDiferido;
            string _plazoDiferido;
            string _mesGracia;
            const string _filler = " ";
            string _montoTotalTransaccion;
            string _montoBaseGravaIVa;
            string _montoBaseNoGravaIVa;
            string _impuestoIvaTransaccion;
            string _impuestoServicioTransaccion;
            string _popinaTransaccion;
            string _montoFijo;
            string _secuencialTransaccion;
            string _horaTransccion;
            string _fechaTransaccion;
            string _numAutorizacion;
            string _MID;
            string _TID;
            string _CID;
            string _filler2;
            string _OTT;
            string _OTTProveedor;
            string _Factura;
            string _PushBanco;
            string _PushCodigo;
            string _FillerFin;
            string _IdVendedor;
            string _facturaComprobante;

            public string TipoMensaje
            {
                get { return string.IsNullOrEmpty(_tipoMensaje) ? "" : _tipoMensaje; }
                set { _tipoMensaje = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string Filler2
            {
                get { return string.IsNullOrEmpty(_filler2) ? "" : _filler2; }
                set { _filler2 = string.IsNullOrEmpty(value) ? "" : value; }
            }
            

            public string TipoTransaccion
            {
                get { return _tipotran; }
                // set { _tipotran = value; _tipotran = " "; }
                set { _tipotran = string.IsNullOrEmpty(value) ? "" : value; }

            }

            public string codRed
            {
                get { return string.IsNullOrEmpty(_codRed) ? "" : _codRed; }
                set { _codRed = string.IsNullOrEmpty(value) ? "" : value; }
                 
            }

            public string codDiferido
            {
                get { return string.IsNullOrEmpty(_codDiferido) ? "" : _codDiferido; }
                set { _codDiferido = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string plazoDiferido
            {
                get { return string.IsNullOrEmpty(_plazoDiferido) ? "" : _plazoDiferido; }
                set { _plazoDiferido = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string mesGracia
            {
                get { return string.IsNullOrEmpty(_mesGracia) ? "" : _mesGracia; }
                set { _mesGracia = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoTotalTransaccion
            {
                get { return string.IsNullOrEmpty(_montoTotalTransaccion) ? "" : _montoTotalTransaccion; }
                set { _montoTotalTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoBaseGravaIVa
            {
                get { return string.IsNullOrEmpty(_montoBaseGravaIVa) ? "" : _montoBaseGravaIVa; }
                set { _montoBaseGravaIVa = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoBaseNoGravaIVa
            {
                get { return string.IsNullOrEmpty(_montoBaseNoGravaIVa) ? "" : _montoBaseNoGravaIVa; }
                set { _montoBaseNoGravaIVa = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string impuestoIvaTransaccion
            {
                get { return string.IsNullOrEmpty(_impuestoIvaTransaccion) ? "" : _impuestoIvaTransaccion; }
                set { _impuestoIvaTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string impuestoServicioTransaccion
            {
                get { return string.IsNullOrEmpty(_impuestoServicioTransaccion) ? "" : _impuestoServicioTransaccion; }
                set { _impuestoServicioTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string popinaTransaccion
            {
                get { return string.IsNullOrEmpty(_popinaTransaccion) ? "" : _popinaTransaccion; }
                set { _popinaTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string montoFijo
            {
                get { return string.IsNullOrEmpty(_montoFijo) ? "" : _montoFijo; }
                set { _montoFijo = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string secuencialTransaccion
            {
                get { return string.IsNullOrEmpty(_secuencialTransaccion) ? "" : _secuencialTransaccion; }
                set { _secuencialTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string horaTransccion
            {
                get { return string.IsNullOrEmpty(_horaTransccion) ? "" : _horaTransccion; }
                set { _horaTransccion = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string fechaTransaccion
            {
                get { return string.IsNullOrEmpty(_fechaTransaccion) ? "" : _fechaTransaccion; }
                set { _fechaTransaccion = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string numAutorizacion
            {
                get { return string.IsNullOrEmpty(_numAutorizacion) ? "" : _numAutorizacion; }
                set { _numAutorizacion = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string MID
            {
                get { return string.IsNullOrEmpty(_MID) ? "" : _MID; }
                set { _MID = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string TID
            {
                get { return string.IsNullOrEmpty(_TID) ? "" : _TID; }
                set { _TID = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string CID
            {
                get { return string.IsNullOrEmpty(_CID) ? "" : _CID; }
                set { _CID  = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string OTT
            {
                get { return string.IsNullOrEmpty(_OTT) ? "" : _OTT; }
                set { _OTT = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string OTTProveedor
            {
                get { return string.IsNullOrEmpty(_OTTProveedor) ? "" : _OTTProveedor; }
                set { _OTTProveedor = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string Factura
            {
                get { return string.IsNullOrEmpty(_Factura) ? "" : _Factura; }
                set { _Factura = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string PushBanco
            {
                get { return string.IsNullOrEmpty(_PushBanco) ? "" : _PushBanco; }
                set { _PushBanco = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string PushCodigo
            {
                get { return string.IsNullOrEmpty(_PushCodigo) ? "" : _PushCodigo; }
                set { _PushCodigo = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string FillerFin
            {
                get { return string.IsNullOrEmpty(_FillerFin) ? "" : _FillerFin; }
                set { _FillerFin = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string IdVendedor
            {
                get { return string.IsNullOrEmpty(_IdVendedor) ? "" : _IdVendedor; }
                set { _IdVendedor = string.IsNullOrEmpty(value) ? "" : value; }
            }

            
            public string FacturaComprobante
            {
                get { return string.IsNullOrEmpty(_facturaComprobante) ? "" : _facturaComprobante; }
                set { _facturaComprobante = string.IsNullOrEmpty(value) ? "" : value; }
            }

            public string DevuelveTramaPPMultiRedv2
            {
                get {
                    string _trama = string.Empty;

                    return _trama;
                }

            }


            public string DevuelveTramaPPMultiRed
            {

                get
                {
                    string _trama = string.Empty;

                    _OTT = string.IsNullOrEmpty(_OTT) ? "" : _OTT;
                    _facturaComprobante = string.IsNullOrEmpty(_facturaComprobante) ? "" : _facturaComprobante;
                    _PushCodigo = string.IsNullOrEmpty(_PushCodigo) ? "" : _PushCodigo;
                    _PushBanco = string.IsNullOrEmpty(_PushBanco) ? "" : _PushBanco;
                    _IdVendedor = string.IsNullOrEmpty(_IdVendedor) ? "" : _IdVendedor;
                    _montoFijo = string.IsNullOrEmpty(_montoFijo) ? "" : _montoFijo;
                    _codRed = string.IsNullOrEmpty(_codRed) ? "" : _codRed;
                    _codDiferido = string.IsNullOrEmpty(_codDiferido) ? "" : _codDiferido;
                    _plazoDiferido = string.IsNullOrEmpty(_plazoDiferido) ? "" : _plazoDiferido;
                    _mesGracia = string.IsNullOrEmpty(_mesGracia) ? "" : _mesGracia;
                    _montoTotalTransaccion = string.IsNullOrEmpty(_montoTotalTransaccion) ? "" : _montoTotalTransaccion;
                    _montoBaseGravaIVa = string.IsNullOrEmpty(_montoBaseGravaIVa) ? "" : _montoBaseGravaIVa;
                    _montoBaseNoGravaIVa = string.IsNullOrEmpty(_montoBaseNoGravaIVa) ? "" : _montoBaseNoGravaIVa;
                    _impuestoIvaTransaccion = string.IsNullOrEmpty(_impuestoIvaTransaccion) ? "" : _impuestoIvaTransaccion;
                    _impuestoServicioTransaccion = string.IsNullOrEmpty(_impuestoServicioTransaccion) ? "" : _impuestoServicioTransaccion;
                    _popinaTransaccion = string.IsNullOrEmpty(_popinaTransaccion) ? "" : _popinaTransaccion;
                    _montoFijo = string.IsNullOrEmpty(_montoFijo) ? "" : _montoFijo;
                    _secuencialTransaccion = string.IsNullOrEmpty(_secuencialTransaccion) ? "" : _secuencialTransaccion;
                    _horaTransccion = string.IsNullOrEmpty(_horaTransccion) ? "" : _horaTransccion;
                    _fechaTransaccion = string.IsNullOrEmpty(_fechaTransaccion) ? "" : _fechaTransaccion;
                    _numAutorizacion = string.IsNullOrEmpty(_numAutorizacion) ? "" : _numAutorizacion;
                    _MID = string.IsNullOrEmpty(_MID) ? "" : _MID;
                    _TID = string.IsNullOrEmpty(_TID) ? "" : _TID;
                    _CID = string.IsNullOrEmpty(_CID) ? "" : _CID;
                    _OTTProveedor = string.IsNullOrEmpty(_OTTProveedor) ? "" : _OTTProveedor;
                    _filler2 = string.IsNullOrEmpty(_filler2) ? "" : (_filler2).PadLeft(20, ' ');

                    _montoFijo = _montoFijo == "" ? "  " : (_montoFijo).PadLeft(12, ' ');
                    _plazoDiferido = string.IsNullOrEmpty(_plazoDiferido) ? " " : _plazoDiferido;
                    _mesGracia = string.IsNullOrEmpty(_mesGracia) ? " " : _mesGracia;


                    _trama = string.Empty;
                    _trama = string.Concat(_trama, _tipoMensaje);
                    _trama = string.Concat(_trama, (_tipotran).Substring(0, 2));
                    _trama = string.Concat(_trama, (_codRed).PadRight(1, '0'));
                    _trama = string.Concat(_trama, (_codDiferido).PadRight(2, '0'));
                    _trama = string.Concat(_trama, (_plazoDiferido.Trim()).PadLeft(2, ' '));
                    _trama = string.Concat(_trama, (_mesGracia.Trim()).PadRight(2, ' '));
                    _trama = string.Concat(_trama, _filler);
                    _trama = string.Concat(_trama, (_montoTotalTransaccion).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_montoBaseGravaIVa).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_montoBaseNoGravaIVa).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_impuestoIvaTransaccion).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_impuestoServicioTransaccion).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_popinaTransaccion).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_montoFijo).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_secuencialTransaccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_horaTransccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_fechaTransaccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_numAutorizacion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_MID).PadRight(15, ' '));
                    _trama = string.Concat(_trama, (_TID).PadRight(8, ' '));
                    _trama = string.Concat(_trama, (_CID).PadRight(15, ' '));
                    _trama = string.Concat(_trama, (_filler2).PadRight(20, ' '));
                    return _trama;
                }
            }

            public string DevuelveTramaLT
            {
                get
                {
                    string _trama = string.Empty;
                    _trama = string.Empty;
                    _trama = string.Concat(_trama, _tipoMensaje);

                    return _trama;
                }
            }

            public string DevuelveTramaCT
            {
                get
                {
                    string _trama = string.Empty;

                    _trama = string.Empty;
                    _trama = string.Concat(_trama, _tipoMensaje);

                    return _trama;
                }
            }

            public string DevuelveTrama
            {

                get
                {
                    string _trama = string.Empty;

                    _OTT = string.IsNullOrEmpty(_OTT) ? "" : _OTT;
                    _facturaComprobante = string.IsNullOrEmpty(_facturaComprobante) ? "" : _facturaComprobante;
                    _PushCodigo = string.IsNullOrEmpty(_PushCodigo) ? "" : _PushCodigo;
                    _PushBanco = string.IsNullOrEmpty(_PushBanco) ? "" : _PushBanco;
                    _IdVendedor = string.IsNullOrEmpty(_IdVendedor) ? "" : _IdVendedor;
                    //_plazoDiferido = (_plazoDiferido).PadLeft(2, '0') == "00" ? "  " : (_plazoDiferido).PadLeft(2, '0');
                    //_mesGracia = (_mesGracia).PadLeft(2, '0') == "00" ? "  " : (_mesGracia).PadLeft(2, '0');
                    _montoFijo = _montoFijo == "" ? "  " : (_montoFijo).PadLeft(12, ' ');

                    _plazoDiferido = string.IsNullOrEmpty(_plazoDiferido) ? " " : _plazoDiferido;
                    _mesGracia = string.IsNullOrEmpty(_mesGracia) ? " " : _mesGracia;
                    //_mesGracia = "00";

                    _trama = string.Empty;
                    _trama = string.Concat(_trama, _tipoMensaje);
                    _trama = string.Concat(_trama, (_tipotran).Substring(0, 2));
                    _trama = string.Concat(_trama, (_codRed).PadRight(1, '0'));
                    _trama = string.Concat(_trama, (_codDiferido).PadRight(2, '0'));
                    _trama = string.Concat(_trama, (_plazoDiferido).PadLeft(2, ' '));
                    _trama = string.Concat(_trama, (_mesGracia).PadRight(2, ' '));
                    _trama = string.Concat(_trama, _filler);
                    _trama = string.Concat(_trama, (_montoTotalTransaccion).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_montoBaseGravaIVa).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_montoBaseNoGravaIVa).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_impuestoIvaTransaccion).PadLeft(12, '0'));
                    _trama = string.Concat(_trama, (_impuestoServicioTransaccion).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_popinaTransaccion).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_montoFijo).PadLeft(12, ' '));
                    _trama = string.Concat(_trama, (_secuencialTransaccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_horaTransccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_fechaTransaccion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_numAutorizacion).PadRight(6, ' '));
                    _trama = string.Concat(_trama, (_MID).PadRight(15, ' '));
                    _trama = string.Concat(_trama, (_TID).PadRight(8, ' '));
                    _trama = string.Concat(_trama, (_CID).PadRight(15, ' '));
                    _trama = string.Concat(_trama, (Filler2).PadRight(20, ' '));
                    //_trama = string.Concat(_trama, (_filler2).PadRight(1, ' '));
                    return _trama;
                }
            }
        }
        public struct RespuestaProcesoPago
        {
            string _obtieneDato;
            string _codigoRespuesta;
            string _codigoRed;
            string _codAutorizador;
            string _mensajeRespuesta;
            string _secuencialtransaccion;
            string _numerolote;
            string _horaTrans;
            string _fechaTrans;
            string _numAut;
            string _termId;
            string _merchantId;
            string _valInteres;
            string _mensajePremioPublicidad;//imprimir al final del voucher
            string _codBancoAdq;
            string _nomBancoAdq;
            string _nomGruTar;
            string _modoLectura;
            string _nombreTarjetaHabiente;
            string _montoFijo;
            string _idEMV;//imprimir al final del voucher
            string _AIDEMV;//imprimir al final del voucher
            string _tipoCritoyValorEMV;//imprimir al final del voucher
            string _verificacionPin;//imprimir al final del voucher
            string _ARQC;
            string _numTarTuncate;
            string _numBin;
            string _fechaVencTar;
            string _numtarEncrip;
            string _TVR;//imprimir al final del voucher
            string _TSI;//imprimir al final del voucher

            public string ObtieneDato
            {
                set
                {
                    _obtieneDato = value.PadRight(450,' ');
                    _codigoRespuesta = (_obtieneDato).Substring(2, 2);
                    _codigoRed = (_obtieneDato).Substring(4, 2);
                    _codAutorizador = (_obtieneDato).Substring(6, 2);
                    _mensajeRespuesta = (_obtieneDato).Substring(8, 20);
                    _secuencialtransaccion = (_obtieneDato).Substring(28, 6);
                    _numerolote = (_obtieneDato).Substring(34, 6);
                    _horaTrans = (_obtieneDato).Substring(40, 6);
                    _fechaTrans = (_obtieneDato).Substring(46, 8);
                    _numAut = (_obtieneDato).Substring(54, 6);
                    _termId = (_obtieneDato).Substring(60, 8);
                    _merchantId = (_obtieneDato).Substring(68, 15);
                    _valInteres = (_obtieneDato).Substring(83, 12);
                    _mensajePremioPublicidad = (_obtieneDato).Substring(95, 80);//imprimir al final del voucher
                    _codBancoAdq = (_obtieneDato).Substring(175, 3);
                    _nomBancoAdq = (_obtieneDato).Substring(178, 30);
                    _nomGruTar = (_obtieneDato).Substring(208, 25);
                    _modoLectura = (_obtieneDato).Substring(233, 2);
                    _nombreTarjetaHabiente = (_obtieneDato).Substring(235, 40);
                    _montoFijo = (_obtieneDato).Substring(275, 12);
                    _idEMV = (_obtieneDato).Substring(287, 20);//imprimir al final del voucher
                    _AIDEMV = (_obtieneDato).Substring(307, 20);//imprimir al final del voucher
                    _tipoCritoyValorEMV = (_obtieneDato).Substring(327, 22);//imprimir al final del voucher
                    _verificacionPin = (_obtieneDato).Substring(349, 15);//imprimir al final del voucher
                    _ARQC = (_obtieneDato).Substring(364, 16);

                    _numTarTuncate = (_obtieneDato).Substring(380, 25);
                    _fechaVencTar = (_obtieneDato).Substring(405, 4);
                    _numtarEncrip = (_obtieneDato).Substring(409, 40);

                    //Nueva trama cuando se procese por TCP IP.   JM  25-08-2020
                    if (Control.Common.GlobalParameters.EstTcpIpPinpad)
                    { 
                        _TVR = (_obtieneDato).Substring(380, 10);
                        _TSI = (_obtieneDato).Substring(390, 4);
                        _numTarTuncate = (_obtieneDato).Substring(394, 25);
                        _fechaVencTar = (_obtieneDato).Substring(419, 4);

                        int leerTramaRes = 40;
                        if (_obtieneDato.Length > 480)
                        {
                            leerTramaRes = leerTramaRes + 24;
                        }
                        _numtarEncrip = (_obtieneDato).Substring(423, leerTramaRes);
                    }
                    /*
                    var charArr = _numTarTuncate.Trim().ToArray();
                    var tarj_mask = "";
                    for (int i = 0; i < charArr.Length; i++)
                    {
                        tarj_mask += (i<6 || charArr.Length-3 <= i) ? charArr[i].ToString() : "X";
                    }
                    _numTarTuncate = tarj_mask;
                    */
                }
            }

            public void SetValuesTest(string codigoRespuesta, 
                                      string codigoRed,
                                      string codigoAutorizador,
                                      string msjRespuesta,
                                      string secuencialTransaccion,
                                      string numeroLote,
                                      string fechaTrans,
                                      string horaTrans,
                                      string numAut,
                                      string termId,
                                      string merchantId,
                                      string valInteres,
                                      string msjPremioPublicidad,
                                      string codBancoAdq,
                                      string nomBancoAdq,
                                      string nomGruTar,
                                      string modoLectura,
                                      string nombreTarjetaHabiente,
                                      string montoFijo,
                                      string idEMV,
                                      string aidEMV,
                                      string tipoCritoyValorEMV,
                                      string verificacionPin,
                                      string arqc,
                                      string numTarTuncate,
                                      string fechaVencTar,
                                      string numtarEncrip)
            {
                _obtieneDato = "";
                _codigoRespuesta = codigoRespuesta;
                _codigoRed = codigoRed;
                _codAutorizador = codigoAutorizador;
                _mensajeRespuesta = msjRespuesta;
                _secuencialtransaccion = secuencialTransaccion;
                _numerolote = numeroLote;
                _horaTrans = horaTrans;
                _fechaTrans = fechaTrans;
                _numAut = numAut;
                _termId = termId;
                _merchantId = merchantId;
                _valInteres = valInteres;
                _mensajePremioPublicidad = msjPremioPublicidad;
                _codBancoAdq = codBancoAdq;
                _nomBancoAdq = nomBancoAdq;
                _nomGruTar = nomGruTar;
                _modoLectura = modoLectura;
                _nombreTarjetaHabiente = nombreTarjetaHabiente;
                _montoFijo = montoFijo;
                _idEMV = idEMV;
                _AIDEMV = aidEMV;
                _tipoCritoyValorEMV = tipoCritoyValorEMV;
                _verificacionPin = verificacionPin;
                _ARQC = arqc;
                _numTarTuncate = numTarTuncate;
                _fechaVencTar = fechaVencTar;
                _numtarEncrip = numtarEncrip;
            }


            public string codigoRespuesta { get { return _codigoRespuesta; } }
            public string codigoRed { get { return _codigoRed; } }
            public string codAutorizador { get { return _codAutorizador; } }
            public string mensajeRespuesta { get { return _mensajeRespuesta; } }
            public string secuencialtransaccion { get { return _secuencialtransaccion; } }
            public string numerolote { get { return _numerolote; } }
            public string horaTrans { get { return _horaTrans; } }
            public string fechaTrans { get { return _fechaTrans; } }
            public string numAut { get { return _numAut; } }
            public string termId { get { return _termId; } }
            public string merchantId { get { return _merchantId; } }
            public string valInteres { get { return _valInteres; } }
            public string mensajePremioPublicidad { get { return _mensajePremioPublicidad; } }//imprimir al final del voucher
            public string codBancoAdq { get { return _codBancoAdq; } }
            public string nomBancoAdq { get { return _nomBancoAdq; } }
            public string nomGruTar { get { return _nomGruTar; } }
            public string modoLectura { get { return _modoLectura; } }
            public string nombreTarjetaHabiente { get { return _nombreTarjetaHabiente; } }
            public string montoFijo { get { return _montoFijo; } }
            public string idEMV { get { return _idEMV; } }//imprimir al final del voucher
            public string AIDEMV { get { return _AIDEMV; } }//imprimir al final del voucher
            public string tipoCritoyValorEMV { get { return _tipoCritoyValorEMV; } }//imprimir al final del voucher
            public string verificacionPin { get { return _verificacionPin; } }//imprimir al final del voucher
            public string ARQC { get { return _ARQC; } }
            public string numTarTuncate { get { return _numTarTuncate; } }
            public string fechaVencTar { get { return _fechaVencTar; } }
            public string numtarEncrip { get { return _numtarEncrip; } }
            public string TVR { get { return _TVR; } }
            public string TSI { get { return _TSI; } }

        }
        #endregion


        #region ProcesoControl
        public struct ProcesoControl
        {
            const string tipo = "PC";
            public string _numLoteDatafast { get; set; }
            public string _secuenciaDatafast { get; set; }
            public string _numLoteMedianet { get; set; }
            public string _secuenciaMedianet { get; set; }
            public string _MIDDatafast { get; set; }
            public string _TIDDatafast { get; set; }
            public string _MIDMedianet { get; set; }
            public string _TIDMedianet { get; set; }
            public string _CID { get; set; }
            public string _talla;
            public string _redActiva;
            private string _filler23;
            private string _filler12;

            public string DevuelveTramaPCMultiRed
            {
                get
                {
                    _filler23 = string.Empty;
                    _filler12 = string.Empty;

                    _numLoteDatafast = string.IsNullOrWhiteSpace(_numLoteDatafast) ? "" : _numLoteDatafast;
                    _secuenciaDatafast = string.IsNullOrWhiteSpace(_secuenciaDatafast) ? "" : _secuenciaDatafast;
                    _numLoteMedianet = string.IsNullOrWhiteSpace(_numLoteMedianet) ? "" : _numLoteMedianet;
                    _secuenciaMedianet = string.IsNullOrWhiteSpace(_secuenciaMedianet) ? "" : _secuenciaMedianet;
                    _MIDDatafast = string.IsNullOrWhiteSpace(_MIDDatafast) ? "" : _MIDDatafast;
                    _TIDDatafast = string.IsNullOrWhiteSpace(_TIDDatafast) ? "" : _TIDDatafast;
                    _MIDMedianet = string.IsNullOrWhiteSpace(_MIDMedianet) ? "" : _MIDMedianet;
                    _TIDMedianet = string.IsNullOrWhiteSpace(_TIDMedianet) ? "" : _TIDMedianet;
                    _CID = string.IsNullOrWhiteSpace(_CID) ? "" : _CID;


                    string TramaReturn = string.Empty;
                    TramaReturn = string.Concat(TramaReturn, tipo);
                    TramaReturn = string.Concat(TramaReturn, (_numLoteDatafast.Trim()).PadLeft(6, '0'));
                    TramaReturn = string.Concat(TramaReturn, (_secuenciaDatafast.Trim()).PadLeft(6, '0'));
                    TramaReturn = string.Concat(TramaReturn, (_numLoteMedianet.Trim()).PadLeft(6, '0'));
                    TramaReturn = string.Concat(TramaReturn, (_secuenciaMedianet.Trim()).PadLeft(6, '0'));
                    TramaReturn = string.Concat(TramaReturn, (_MIDDatafast).PadRight(15, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_TIDDatafast).PadRight(8, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_MIDMedianet).PadRight(15, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_TIDMedianet).PadRight(8, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_CID).PadRight(15, ' '));
                    return TramaReturn;

                    


                }
            }

            public string DevuelveTramaPC
            {
                get
                {

                    _filler23 = string.Empty;
                    _filler12 = string.Empty;

                    _numLoteMedianet = string.IsNullOrEmpty(_numLoteMedianet) ? "" : _numLoteMedianet;
                    _secuenciaMedianet = string.IsNullOrEmpty(_secuenciaMedianet) ? "" : _secuenciaMedianet;
                    _TIDMedianet = string.IsNullOrEmpty(_TIDMedianet) ? "" : _TIDMedianet;

                    _numLoteMedianet = string.Empty;
                    _secuenciaMedianet = string.Empty;
                    _CID = string.Empty;

                    _numLoteMedianet = (_numLoteMedianet.Trim()).PadLeft(6, ' ');
                    _secuenciaMedianet = (_secuenciaMedianet.Trim()).PadLeft(6, ' ');
                    _TIDMedianet = string.IsNullOrEmpty(_TIDMedianet) ? "" : _TIDMedianet;

                    _redActiva = "2";

                    string TramaReturn = string.Empty;
                    TramaReturn = string.Concat(TramaReturn, tipo);
                    TramaReturn = string.Concat(TramaReturn, (_numLoteMedianet.Trim()).PadLeft(6, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_secuenciaMedianet.Trim()).PadLeft(6, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_filler12).PadRight(12, ' ') );
                    TramaReturn = string.Concat(TramaReturn, (_MIDMedianet).PadRight(15, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_TIDMedianet).PadRight(8, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_filler23).PadRight(23, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_CID).PadRight(15, ' '));
                    TramaReturn = string.Concat(TramaReturn, (_redActiva).PadRight(1, '2'));

                    return TramaReturn;

                    //return tipo
                    //        + _numLoteMedianet
                    //        + _secuenciaMedianet
                    //        + (_filler12).PadRight(12, ' ')
                    //        + (_MIDMedianet).PadRight(15, ' ')
                    //        + (_TIDMedianet).PadRight(8, ' ')
                    //        + (_filler23).PadRight(23, ' ')
                    //        + (_CID).PadRight(15, ' ')
                    //        + (_redActiva).PadRight(1, '2')
                    //        ;

                    //return tipo
                    //        + (_numLoteDatafast.Trim()).PadLeft(6, '0')
                    //        + (_secuenciaDatafast.Trim()).PadLeft(6, '0')
                    //        + (_numLoteMedianet.Trim()).PadLeft(6, '0')
                    //        + (_secuenciaMedianet).PadLeft(6, '0')
                    //        + (_MIDDatafast).PadRight(15, ' ')
                    //        + (_TIDDatafast).PadRight(8, ' ')
                    //        + (_MIDMedianet).PadRight(15, ' ')
                    //        + (_TIDMedianet).PadRight(8, ' ')
                    //        + (_CID).PadRight(15, ' ')
                    //        + (_redActiva).PadRight(1, '2')
                    //        ;
                }
            }


        }

        #endregion


        #region Configuración PinPad    
        public struct ConfiguraPinPad
        {
            const string tipo = "CP";

            string _IpPinPadMedianet;
            string _MascaraPinPadMediaNet;
            string _PuertaEnlacePinPadMedianet;
            string _PuertoEscuchaMedianet;
            string _IpPinPadpDataFast;
            string _MascaraPinPadDataFast;
            string _PuertaEnlacePinPadDataFast;
            string _PuertoEscuchaDataFast;
            string _IpConfigPrincipalMedianet;
            string _PuertoConfigPrincipalMedianet;
            string _IpConfigAlternoMedianet;
            string _PuertoConfigAlternoMedianet;
            string _IpConfigPrincipalDataFast;
            string _PuertoConfigPrincipalDataFast;
            string _IpConfigAlternoDataFast;
            string _PuertoConfigAlternoDataFast;
            string _tipoMensaje;
            string _CodigoRespuesta;
            string _MensajeRepsuesta;
            string _obtieneDato;

            public string IpPinPadMedianet { get { return _IpPinPadMedianet; } }
            public string MascaraPinPadMediaNet { get { return _MascaraPinPadMediaNet; } }
            public string PuertaEnlacePinPadMedianet { get { return _PuertaEnlacePinPadMedianet; } }
            public string PuertoEscuchaMedianet { get { return _PuertoEscuchaMedianet; } }
            public string IpPinPadpDataFast { get { return _IpPinPadpDataFast; } }
            public string MascaraPinPadDataFast { get { return _MascaraPinPadDataFast; } }
            public string PuertaEnlacePinPadDataFast { get { return _PuertaEnlacePinPadDataFast; } }
            public string PuertoEscuchaDataFast { get { return _PuertoEscuchaDataFast; } }
            public string IpConfigPrincipalMedianet { get { return _IpConfigPrincipalMedianet; } }
            public string PuertoConfigPrincipalMedianet { get { return _PuertoConfigPrincipalMedianet; } }
            public string IpConfigAlternoMedianet { get { return _IpConfigAlternoMedianet; } }
            public string PuertoConfigAlternoMedianet { get { return _PuertoConfigAlternoMedianet; } }
            public string IpConfigPrincipalDataFast { get { return _IpConfigPrincipalDataFast; } }
            public string PuertoConfigPrincipalDataFast { get { return _PuertoConfigPrincipalDataFast; } }
            public string IpConfigAlternoDataFast { get { return _IpConfigAlternoDataFast; } }
            public string PuertoConfigAlternoDataFast { get { return _PuertoConfigAlternoDataFast; } }

            
            public string TipoMensaje
            {
                get { return string.IsNullOrEmpty(_tipoMensaje) ? "" : _tipoMensaje; }
                //set { _tipoMensaje = string.IsNullOrEmpty(value) ? "" : value; }
            }
            public string CodigoRespuesta
            {
                get { return _CodigoRespuesta; }
                set { _CodigoRespuesta = value; _CodigoRespuesta = " "; }
            }            
            public string MensajeRespuesta
            {
                get { return _MensajeRepsuesta; }
                set { _MensajeRepsuesta = value; _MensajeRepsuesta = " "; }
            }
           
            public string RespuestaTramaCP
            {
                set
                {
                    _obtieneDato = value.PadRight(450, ' ');
                    _CodigoRespuesta = (_obtieneDato).Substring(2, 2);
                    _MensajeRepsuesta = (_obtieneDato).Substring(4, 20);


                }
                    
            }
            public string DevuelveTramaConfig {
                get
                {
                    string Trama = string.Empty;                   
                    string IpPinPadMedianet = (_IpPinPadMedianet.ToString()).PadRight(15, ' ');
                    string MascaraPinPadDataFast = (_MascaraPinPadDataFast.ToString()).PadRight(15, ' ');
                    string PuertaEnlacePinPadMedianet = (_PuertaEnlacePinPadMedianet.ToString()).PadRight(15, ' ');
                    string PuertoEscuchaMedianet  = (_PuertoEscuchaMedianet.ToString()).PadRight(6, '0');

                    string PuertaEnlacePinPadDataFast = (_PuertaEnlacePinPadDataFast.ToString()).PadRight(15, ' ');
                    string PuertoEscucha = (_PuertoEscuchaMedianet.ToString()).PadRight(15, ' ');

                    string IpPrincipaMedianet = (_IpConfigPrincipalMedianet.ToString()).PadRight(15, ' ');
                    string puertoPrincipalMedianet = (_PuertoConfigPrincipalMedianet.ToString()).PadRight(6, ' ');

                    string IpAlternoMedianet = (_IpConfigAlternoMedianet.ToString()).PadRight(15, ' ');
                    string puertoAlternoMedianet = (_PuertoConfigAlternoMedianet.ToString()).PadRight(6, ' ');

                    string IpConfigPrincipalDataFast = (_IpConfigPrincipalDataFast.ToString()).PadRight(15, ' ');
                    string PuertoConfigPrincipalDataFast  = (_PuertoConfigPrincipalDataFast.ToString()).PadRight(6, ' ');

                    string IpConfigAlternoDataFast  = (_IpConfigAlternoDataFast.ToString()).PadRight(15, ' ');
                    string PuertoConfigAlternoDataFast = (_PuertoConfigAlternoDataFast.ToString()).PadRight(6, ' ');


                    Trama = string.Concat(Trama, tipo);
                    Trama = string.Concat(Trama, IpPinPadMedianet);
                    Trama = string.Concat(Trama, MascaraPinPadDataFast);
                    Trama = string.Concat(Trama, PuertaEnlacePinPadMedianet);
                    Trama = string.Concat(Trama, IpPrincipaMedianet);
                    Trama = string.Concat(Trama, PuertoEscuchaMedianet);
                    Trama = string.Concat(Trama, IpConfigPrincipalDataFast);
                    Trama = string.Concat(Trama, PuertoConfigPrincipalDataFast);
                    Trama = string.Concat(Trama, PuertoEscuchaMedianet);

                    return Trama;


                }
            }
        }


        #endregion


        #region Devolución / Anulación 

        #endregion
    }
}
