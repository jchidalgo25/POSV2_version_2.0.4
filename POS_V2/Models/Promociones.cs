using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    public class Promocion
    {
        string _establecimiento;

        public string Establecimiento
        {
            get { return _establecimiento; }
            set { _establecimiento = value; }
        }

        int _tipo;

        public int Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        String _descripcion;

        public String Descripcion
        {
            get { return _descripcion; }
            set { _descripcion = value; }
        }

        DateTime _fecha_desde;

        public DateTime FechaDesde
        {
            get { return _fecha_desde; }
            set { _fecha_desde = value; }
        }

        DateTime _fecha_hasta;

        public DateTime FechaHasta
        {
            get { return _fecha_hasta; }
            set { _fecha_hasta = value; }
        }

        int _estado;

        public int Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        Int64 _rec_id;

        public Int64 RecId
        {
            get { return _rec_id; }
            set { _rec_id = value; }
        }

        decimal _maxCantidadDscto = 100000;

        public decimal MaxCantidadDscto
        {
            get { return _maxCantidadDscto; }
            set { _maxCantidadDscto = value; }
        }

        bool _esRestrictiva = false;

        public bool EsRestrictiva
        {
            get { return _esRestrictiva; }
            set { _esRestrictiva = value; }
        }



        List<Producto> _list_productos;

        public List<Producto> ListProductos
        {
            get { return _list_productos; }
            set { _list_productos = value; }
        }


        bool _GeneralPromo = false;
        public bool GeneralPromo
        {
            get { return _GeneralPromo; }
            set { _GeneralPromo = value; }
        }

        public System.Linq.IQueryable<vw_DescuentosCabeceraAX> getCabeceraPorDiaHoy(POSEntities db, string _establecimiento)
        {
            DateTime today = DateTime.Now;
            string diaHoy = ((int)today.DayOfWeek).ToString();
            string almacenTodos = "TODOS";

            // Convertir a mayúsculas
            string establecimientoUpper = string.IsNullOrEmpty(_establecimiento) ? almacenTodos : _establecimiento.ToUpper();
            string almacenTodosUpper = string.IsNullOrEmpty(almacenTodos) ? almacenTodos : almacenTodos.ToUpper();


            try
            {

                
               
                if (diaHoy == "1")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.LUNES == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.LUNES == 1 && z.ESTADO == 1));
                if (diaHoy == "2")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.MARTES == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.MARTES == 1 && z.ESTADO == 1));
                if (diaHoy == "3")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.MIERCOLES == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.MIERCOLES == 1 && z.ESTADO == 1));
                if (diaHoy == "4")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.JUEVES == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.JUEVES == 1 && z.ESTADO == 1));
                if (diaHoy == "5")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.VIERNES == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.VIERNES == 1 && z.ESTADO == 1));
                if (diaHoy == "6")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.SABADO == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.SABADO == 1 && z.ESTADO == 1));
                if (diaHoy == "0" || diaHoy == "7")
                    return db.vw_DescuentosCabeceraAX.Where(z =>
                        (z.ALMACEN.ToUpper() == establecimientoUpper && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.DOMINGO == 1 && z.ESTADO == 1)
                        || (z.ALMACEN.ToUpper() == almacenTodosUpper
                        && z.FECHADESDE <= today && z.FECHAHASTA >= today && z.DOMINGO == 1 && z.ESTADO == 1));
                return null;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Promociones", "getCabeceraPorDiaHoy", $"error {ex.Message}");
                return null;
            }


        }

    }
}
