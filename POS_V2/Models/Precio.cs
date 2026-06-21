using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class ListaPrecio
    {
        public List<Precio> Precios { get; set; }
    }

    public class Precio
    {   
        private string _establecimiento;
        private string _ITEMID;
        private string _ITEMNAME;
        private string _NAMEALIAS;
        private string _ITEMBARCODE;
        private string _UNITID;
        private decimal _PRICE;
        private decimal _TAXVALUE;
        private decimal _PrecioBase;
        private decimal _pvp;
        private decimal _PorcDescuento;
        private decimal _valorDescto;
        private decimal _ValorImpto;
        private decimal _pvpImpto;
        private string _ITEMGROUPID;
        private string _Grupo;
        private string _SubGrupo;
        private string _Variedad;
        private string _categoria;

        public string Establecimiento
        {
            get { return _establecimiento; }
            set { _establecimiento = value; }
        }

        public string ITEMID
        {
            get { return _ITEMID; }
            set { _ITEMID = value; }
        }

        public string ITEMNAME
        {
            get { return _ITEMNAME; }
            set { _ITEMNAME = value; }
        }
        public string NAMEALIAS {
            get { return _NAMEALIAS; }
            set { _NAMEALIAS = value; }
        }
        public string ITEMBARCODE
        {
            get { return _ITEMBARCODE; }
            set { _ITEMBARCODE  = value; }
        }
        public string UNITID
        {
            get { return _UNITID; }
            set { _UNITID = value; }
        }
        public decimal PRICE
        {
            get { return _PRICE; }
            set { _PRICE = value; }
        }
        
        public decimal TAXVALUE
        {
            get { return _TAXVALUE; }
            set { _TAXVALUE = value; }
        }

        public decimal PrecioBase
        {
            get { return _PrecioBase; }
            set { _PrecioBase = value; }
        }

        public decimal pvp
        {
            get { return _pvp; }
            set { _pvp = value; }
        }

        public decimal PorcDescuento
        {
            get { return _PorcDescuento; }
            set { _PorcDescuento = value; }
        }

        public decimal valorDescto
        {
            get { return _valorDescto; }
            set { _valorDescto = value; }
        }

        public decimal ValorImpto
        {
            get { return _ValorImpto; }
            set { _ValorImpto  = value; }
        }

        public decimal pvpImpto
        {
            get { return _pvpImpto; }
            set { _pvpImpto = value; }
        }
        
        public string ITEMGROUPID
        {
            get { return _ITEMGROUPID; }
            set { _ITEMGROUPID = value; }
        }

        public string Grupo
        {
            get { return _Grupo; }
            set { _Grupo = value; }
        }

        public string SubGrupo
        {
            get { return _SubGrupo; }
            set { _SubGrupo = value; }
        }

        
        public string Variedad
        {
            get { return _Variedad; }
            set { _Variedad = value; }
        }

        public string categoria
        {
            get { return _categoria; }
            set { _categoria = value; }
        }


    }



}
