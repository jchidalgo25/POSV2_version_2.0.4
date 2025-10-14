using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.Parking
{
    public class clsParking
    {
        private string _codigo;
        public string Codigo
        {
            get { return _codigo; }
            set { _codigo = value; }
        }

        private string _itemIdParqueo;
        public string ItemIdParqueo
        {
            get { return _itemIdParqueo; }
            set { _itemIdParqueo = value; }
        }

        private string _itemIdParqueoSinCompra;
        public string ItemIdParqueoSinCompra
        {
            get { return _itemIdParqueoSinCompra; }
            set { _itemIdParqueoSinCompra = value; }
        }

        private bool _estaConfirmado = false;
        public bool EstaConfirmado
        {
            get { return _estaConfirmado; }
            set { _estaConfirmado = value; }
        }

        private bool _debeEnlazarFactura = false;
        public bool DebeEnlazarFactura
        {
            get { return _debeEnlazarFactura; }
            set { _debeEnlazarFactura = value; }
        }

        private DateTime _fechaIngreso;
        public DateTime FechaIngreso
        {
            get { return _fechaIngreso; }
            set { _fechaIngreso = value; }
        }

        private string _facPtoEmision;
        public string FacPtoEmision
        {
            get { return _facPtoEmision; }
            set { _facPtoEmision = value; }
        }

        private int _facNumero;
        public int FacNumero
        {
            get { return _facNumero; }
            set { _facNumero = value; }
        }

        private int _facEnlaceId;
        public int FacEnlaceId
        {
            get { return _facEnlaceId; }
            set { _facEnlaceId = value; }
        }

        private DateTime _facFechaCreacion;
        public DateTime FacFechaCreacion
        {
            get { return _facFechaCreacion; }
            set { _facFechaCreacion = value; }
        }

        private decimal _facValorTotal;
        public decimal FacValorTotal
        {
            get { return _facValorTotal; }
            set { _facValorTotal = value; }
        }

        private string _pathIngreso;
        public string PathIngreso
        {
            get { return _pathIngreso; }
            set { _pathIngreso = value; }
        }

        private string _pathSalida;
        public string PathSalida
        {
            get { return _pathSalida; }
            set { _pathSalida = value; }
        }

        private string _pathHorario;
        public string PathHorario
        {
            get { return _pathHorario; }
            set { _pathHorario = value; }
        }

        private int _minutosFraccion;
        public int MinutosFraccion
        {
            get { return _minutosFraccion; }
            set { _minutosFraccion = value; }
        }

        private int _minutosFraccionSinCompra;
        public int MinutosFraccionSinCompra
        {
            get { return _minutosFraccionSinCompra; }
            set { _minutosFraccionSinCompra = value; }
        }

        private int _minutosGracia;
        public int MinutosGracia
        {
            get { return _minutosGracia; }
            set { _minutosGracia = value; }
        }

        private int _minutosLibreMaxPorCompra;
        public int MinutosLibreMaxPorCompra
        {
            get { return _minutosLibreMaxPorCompra; }
            set { _minutosLibreMaxPorCompra = value; }
        }

        private int _minutosLibreMaxPorCompra2;
        public int MinutosLibreMaxPorCompra2
        {
            get { return _minutosLibreMaxPorCompra2; }
            set { _minutosLibreMaxPorCompra2 = value; }
        }

        private decimal _valorMinCompraParaMinutosLibre;
        public decimal ValorMinCompraParaMinutosLibre
        {
            get { return _valorMinCompraParaMinutosLibre; }
            set { _valorMinCompraParaMinutosLibre = value; }
        }

        private decimal _valorMinCompraParaMinutosLibre2;
        public decimal ValorMinCompraParaMinutosLibre2
        {
            get { return _valorMinCompraParaMinutosLibre2; }
            set { _valorMinCompraParaMinutosLibre2 = value; }
        }

        private bool _tieneTiempoGraciaPorCompra = false;
        public bool TieneTiempoGraciaPorCompra
        {
            get { return _tieneTiempoGraciaPorCompra; }
            set { _tieneTiempoGraciaPorCompra = value; }
        }

        private int _totalFracciones = 0;
        public int TotalFracciones
        {
            get { return _totalFracciones; }
            set { _totalFracciones = value; }
        }

        private bool _debeRealizarAvisoTiempoGracia= true;
        public bool DebeRealizarAvisoTiempoGracia
        {
            get { return _debeRealizarAvisoTiempoGracia; }
            set { _debeRealizarAvisoTiempoGracia = value; }
        }

        private Control.Common.Enum.ParkingItemType _tipoItem = 0;
        public Control.Common.Enum.ParkingItemType TipoItem
        {
            get { return _tipoItem; }
            set { _tipoItem = value; }
        }

        public Models.Parking.clsParking Clone()
        {
            var cloned = (Models.Parking.clsParking)this.MemberwiseClone();

            return cloned;
        }
    }
}
