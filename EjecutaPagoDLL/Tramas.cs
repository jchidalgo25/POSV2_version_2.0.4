using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjecutaPagoDLL
{
    public class Tramas
    {
        #region Lectura Tarjeta
        public struct LecturaTarjeta
        {

            const string tipoMensaje = "LT";

            public string DevuelveTrama
            {
                get
                {
                    return tipoMensaje;
                }
            }
        }

        #endregion

        #region ProcesoControl
        public struct ProcesoControl
        {
            const string tipoMensaje = "PC";
            public string _numLoteDatafast;
            public string _secuenciaDatafast;
            public string _numLoteMedianet;
            public string _secuenciaMedianet;
            public string _MIDDatafast;
            public string _TIDDatafast;
            public string _MIDMedianet;
            public string _TIDMedianet;
            public string _CID;
            public string _CID_DATAFAST;

            public string _talla;
            public string _redActiva;
            private string _filler23;
            private string _filler12;
            public string DevuelveTramaMedianet
            {
                get
                {
                    _filler23 = string.Empty;
                    _filler12 = string.Empty;

                    _numLoteMedianet = string.IsNullOrEmpty(_numLoteMedianet) ? "" : _numLoteMedianet;
                    _secuenciaMedianet = string.IsNullOrEmpty(_secuenciaMedianet) ? "" : _secuenciaMedianet;

                    _numLoteMedianet = (_numLoteMedianet.Trim()).PadLeft(6, ' ');
                    _secuenciaMedianet = (_secuenciaMedianet.Trim()).PadLeft(6, ' ');

                    return tipoMensaje
                                + _numLoteMedianet
                                + _secuenciaMedianet
                                + (_filler12).PadRight(12, ' ')
                                + (_MIDMedianet).PadRight(15, ' ')
                                + (_TIDMedianet).PadRight(8, ' ')
                                + (_filler23).PadRight(23, ' ')
                                + (_CID).PadRight(15, ' ')
                                + (_redActiva).PadRight(1, '2')
                                ;
                }
            }

            public string DevuelveTramaMultiRed
            {
                get
                {

                    _numLoteDatafast = string.IsNullOrWhiteSpace(_numLoteDatafast) ? "" : _numLoteDatafast;
                    _secuenciaDatafast = string.IsNullOrWhiteSpace(_secuenciaDatafast) ? "" : _secuenciaDatafast;
                    _numLoteMedianet = string.IsNullOrWhiteSpace(_numLoteMedianet) ? "" : _numLoteMedianet;
                    _secuenciaMedianet = string.IsNullOrWhiteSpace(_secuenciaMedianet) ? "" : _secuenciaMedianet;
                    _MIDDatafast = string.IsNullOrWhiteSpace(_MIDDatafast) ? "" : _MIDDatafast;
                    _TIDDatafast = string.IsNullOrWhiteSpace(_TIDDatafast) ? "" : _TIDDatafast;
                    _MIDMedianet = string.IsNullOrWhiteSpace(_MIDMedianet) ? "" : _MIDMedianet;
                    _TIDMedianet = string.IsNullOrWhiteSpace(_TIDMedianet) ? "" : _TIDMedianet;
                    _CID = string.IsNullOrWhiteSpace(_CID) ? "" : _CID;


                    return (tipoMensaje.Trim()).PadLeft(2, ' ')
                                + (_numLoteDatafast.Trim()).PadLeft(6, '0')
                                + (_secuenciaDatafast.Trim()).PadLeft(6, '0')
                                + (_numLoteMedianet.Trim()).PadLeft(6, '0')
                                + (_secuenciaMedianet).PadLeft(6, '0')
                                + (_MIDDatafast).PadRight(15, ' ')
                                + (_TIDDatafast).PadRight(8, ' ')
                                + (_MIDMedianet).PadRight(15, ' ')
                                + (_TIDMedianet).PadRight(8, ' ')
                                + (_CID).PadRight(15, ' ');


                }
            }
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
            public string DevuelveTramaPP
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
                    //_mesGracia = "00";

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
        }



        #endregion
    }
}
