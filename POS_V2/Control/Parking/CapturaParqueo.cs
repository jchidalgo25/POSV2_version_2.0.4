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
using System.IO;

namespace POS.Control.Parking
{
    public partial class CapturaParqueo : Telerik.WinControls.UI.RadForm
    {
        public CapturaParqueo()
        {
            InitializeComponent();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    this.Dispose();
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private Models.Parking.clsParking _objParking;

        public Models.Parking.clsParking ObjParking { get { return _objParking; } }

        private bool _noTieneTicket = false;
        public bool NoTieneTicket { get { return _noTieneTicket; } }

        private string _itemIdParqueo;
        private string _itemIdParqueoSinCompra;
        private string _pathIngreso;
        private string _pathSalida;
        private string _pathHorario;
        private TimeSpan _horarioDesde;
        private TimeSpan _horarioHasta;
        private int _tiempoLibreMax = 0;
        private int _tiempoLibreMax2 = 0;
        private int _tiempoDefaultGracia = 0;
        private int _tiempoFraccion = 0;
        private int _tiempoFraccionSinCompra = 0;
        private decimal _tiempoLibreMinCompra = 0M;
        private decimal _tiempoLibreMinCompra2 = 0M;
        private string _WSIngreso;
        private string _WSSalida;

        private void CargarParametros()
        {
            bool parametrosCargados = true;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //------------------------PATH INGRESO--------------------------------------------------------
                    var param = db.core_parametro.Where(x => x.identificador == "PARKING_PATHINGRESO" && x.valor == Common.GlobalParameters.Establecimiento).FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_PATHINGRESO' en core_parametro");
                    }

                    if (string.IsNullOrEmpty(param.parametro2))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_PATHINGRESO' en core_parametro está vacío y no contiene la ruta de ficheros de ingresos");
                    }

                    if (param.parametro2 == "API")
                    {
                        if (string.IsNullOrEmpty(param.documento))
                        {
                            throw new Exception("Campo 'documento' de registro 'PARKING_PATHSALIDA' en core_parametro está vacío y no contiene la url del web service ficheros de ingresos");
                        }
                        _WSIngreso = param.documento;
                    }

                    _pathIngreso = param.parametro2;

                    //------------------------PATH SALIDAS--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_PATHSALIDA" && x.valor == Common.GlobalParameters.Establecimiento).FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_PATHSALIDA' en core_parametro");
                    }

                    if (string.IsNullOrEmpty(param.parametro2))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_PATHSALIDA' en core_parametro está vacío y no contiene la ruta de ficheros de salidas");
                    }

                    if (param.parametro2 =="API")
                    {
                        if (string.IsNullOrEmpty(param.documento))
                        {
                            throw new Exception("Campo 'documento' de registro 'PARKING_PATHSALIDA' en core_parametro está vacío y no contiene la url del web service ficheros de salidas");
                        }
                        _WSSalida = param.documento ;
                    }

                    _pathSalida = param.parametro2;

                    //------------------------PATH HORARIO--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_PATHHORARIO" && x.valor == Common.GlobalParameters.Establecimiento).FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_PATHHORARIO' en core_parametro");
                    }

                    if (string.IsNullOrEmpty(param.parametro2))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_PATHHORARIO' en core_parametro está vacío y no contiene la ruta de ficheros de salidas");
                    }

                    _pathHorario = param.parametro2;

                    //------------------------TIEMPO GRACIA POR COMPRA--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_MAXGRACIACOMPRA" 
                                                         && 
                                                         (x.documento.Contains(Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_MAXGRACIACOMPRA' en core_parametro para este establecimiento");
                    }

                    if (!int.TryParse(param.valor, out _tiempoLibreMax))
                    {
                        throw new Exception("Campo 'valor' de registro 'PARKING_MAXGRACIACOMPRA' en core_parametro es incorrecto y no se puede parsear a entero");
                    }

                    if (_tiempoLibreMax < 0)
                    {
                        throw new Exception("Parametro 'PARKING_MAXGRACIACOMPRA' tiene configurado un valor incorrecto en columna 'valor'. Valor debe ser mayor o igual a 0");
                    }

                    if (!decimal.TryParse(param.parametro2, out _tiempoLibreMinCompra))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_MAXGRACIACOMPRA' en core_parametro es incorrecto y no se puede parsear a decimal");
                    }

                    if (_tiempoLibreMinCompra < 0)
                    {
                        throw new Exception("Parametro 'PARKING_MAXGRACIACOMPRA' tiene configurado un valor incorrecto en columna 'parametro2'. Valor debe ser mayor o igual a 0");
                    }


                    //------------------------TIEMPO GRACIA POR COMPRA CONF 2--------------------------------------------------------
                    if (_pathSalida == "API")
                    {
                        param = db.core_parametro.Where(x => x.identificador == "PARKING_MAXGRACIACOMPRA2"
                                                             &&
                                                             (x.documento.Contains(Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                       )
                                                 .FirstOrDefault();
                        if (param == null)
                        {
                            throw new Exception("No hay parametro 'PARKING_MAXGRACIACOMPRA2' en core_parametro para este establecimiento");
                        }

                        if (!int.TryParse(param.valor, out _tiempoLibreMax2))
                        {
                            throw new Exception("Campo 'valor' de registro 'PARKING_MAXGRACIACOMPRA2' en core_parametro es incorrecto y no se puede parsear a entero");
                        }

                        if (_tiempoLibreMax2 < 0)
                        {
                            throw new Exception("Parametro 'PARKING_MAXGRACIACOMPRA2' tiene configurado un valor incorrecto en columna 'valor'. Valor debe ser mayor o igual a 0");
                        }

                        if (!decimal.TryParse(param.parametro2, out _tiempoLibreMinCompra2))
                        {
                            throw new Exception("Campo 'parametro2' de registro 'PARKING_MAXGRACIACOMPRA2' en core_parametro es incorrecto y no se puede parsear a decimal");
                        }

                        if (_tiempoLibreMinCompra2 < 0)
                        {
                            throw new Exception("Parametro 'PARKING_MAXGRACIACOMPRA2' tiene configurado un valor incorrecto en columna 'parametro2'. Valor debe ser mayor o igual a 0");
                        }
                    }

                    //------------------------MINUTOS FRACCION CON COMPRA--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_MINUTOSFRACCION_CONCOMPRA"
                                                         &&
                                                         (x.documento.Contains(Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_MINUTOSFRACCION_CONCOMPRA' en core_parametro para este establecimiento");
                    }

                    if (!int.TryParse(param.valor, out _tiempoFraccion))
                    {
                        throw new Exception("Campo 'valor' de registro 'PARKING_MINUTOSFRACCION_CONCOMPRA' en core_parametro es incorrecto y no se puede parsear a entero");
                    }

                    if (_tiempoFraccion <= 0)
                    {
                        throw new Exception("Parametro 'PARKING_MINUTOSFRACCION_CONCOMPRA' tiene configurado un valor incorrecto en columna 'valor'. Valor debe ser mayor a 0");
                    }

                    if (string.IsNullOrWhiteSpace(param.parametro2))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_MINUTOSFRACCION_CONCOMPRA' en core_parametro está vacío y no contiene el ITEMID del parqueo");
                    }
                    _itemIdParqueo = param.parametro2;

                    //------------------------MINUTOS FRACCION SIN COMPRA--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_MINUTOSFRACCION_SINCOMPRA"
                                                         &&
                                                         (x.documento.Contains(Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_MINUTOSFRACCION_SINCOMPRA' en core_parametro para este establecimiento");
                    }

                    if (!int.TryParse(param.valor, out _tiempoFraccionSinCompra))
                    {
                        throw new Exception("Campo 'valor' de registro 'PARKING_MINUTOSFRACCION_SINCOMPRA' en core_parametro es incorrecto y no se puede parsear a entero");
                    }

                    if (_tiempoFraccionSinCompra <= 0)
                    {
                        throw new Exception("Parametro 'PARKING_MINUTOSFRACCION_SINCOMPRA' tiene configurado un valor incorrecto en columna 'valor'. Valor debe ser mayor a 0");
                    }

                    if (string.IsNullOrWhiteSpace(param.parametro2))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_MINUTOSFRACCION_SINCOMPRA' en core_parametro está vacío y no contiene el ITEMID del parqueo");
                    }
                    _itemIdParqueoSinCompra = param.parametro2;

                    //------------------------TIEMPO GRACIA DEFAULT--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_MINUTOSGRACIA"
                                                         &&
                                                         (x.documento.Contains(Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_MINUTOSGRACIA' en core_parametro para este establecimiento");
                    }

                    if (!int.TryParse(param.valor, out _tiempoDefaultGracia))
                    {
                        throw new Exception("Campo 'valor' de registro 'PARKING_MINUTOSGRACIA' en core_parametro es incorrecto y no se puede parsear a entero");
                    }

                    if (_tiempoDefaultGracia < 0)
                    {
                        throw new Exception("Parametro 'PARKING_MINUTOSGRACIA' tiene configurado un valor incorrecto en columna 'valor'. Valor debe ser mayor o igual a 0");
                    }

                    //------------------------HORARIO--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "PARKING_HORARIO"
                                                         &&
                                                         (x.documento.Contains(Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'PARKING_HORARIO' en core_parametro para este establecimiento");
                    }

                    if (!TimeSpan.TryParse(param.valor, out _horarioDesde))
                    {
                        throw new Exception("Campo 'valor' de registro 'PARKING_HORARIO' en core_parametro es incorrecto y no se puede parsear a timespan");
                    }

                    if (!TimeSpan.TryParse(param.parametro2, out _horarioHasta))
                    {
                        throw new Exception("Campo 'parametro2' de registro 'PARKING_HORARIO' en core_parametro es incorrecto y no se puede parsear a timespan");
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "POS.Control.Parking.GiftcardSale", "CargarParametros", "Imposible terminar de cargar parámetros en este momento, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                parametrosCargados = false;
            }

            if (!parametrosCargados)
            {
                //Control.Common.WinForm.ShowMessage("No se pudieron cargar todos los parámetros necesarios para el formulario en este momento, esto pudo deberse a una breve interrupción en la comunicación. Vuélvalo a intentar en unos momentos");
                Control.Common.General.GetMensajeToList(518);
                this.Close();
            }
        }

        private void btnValidar_Click(object sender, EventArgs e)
        {
            Validar();
        }

        private void btnConfirmar_Click(object sender, EventArgs e)
        {
            Confirmar();
        }

        private void Confirmar()
        {
            string msg = string.Empty;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            try
            {
                if (_objParking != null)
                {
                    if (_objParking.FechaIngreso > DateTime.Now)
                    {
                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[Codigo]", valor = _objParking.Codigo });
                        parametros.Add(new ParametrosMensajes() { codigo = "[FechaIngreso]", valor = _objParking.FechaIngreso.ToString("dddd, dd MMMM yyyy H:mm") });
                        Control.Common.General.GetMensajeToList(519, parametros);


                        msg = "Se está intentando confirmar un parqueo '" + _objParking.Codigo
                            + "' que tiene hora de entrada '" + _objParking.FechaIngreso.ToString("dddd, dd MMMM yyyy H:mm")
                            + "' superior a la hora actual";

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaParqueo", "Confirmar", msg);
                        //Control.Common.WinForm.ShowMessage(msg);

                        return;
                    }
                    _objParking.DebeEnlazarFactura = chkEnlazarFactura.Checked;
                    if (_objParking.DebeEnlazarFactura)
                    {
                        _objParking.FacPtoEmision = txtPtoEmisionEnlazar.Text.Trim();
                        int nroFactura = 0;
                        int.TryParse(txtNroFacturaEnlazar.Text, out nroFactura);
                        if (nroFactura == 0)
                        {
                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[NroFacturaEnlazar]", valor = txtNroFacturaEnlazar.Text });                           
                            Control.Common.General.GetMensajeToList(520, parametros);

                            
                            msg = "Se está intentando confirmar enlazando un nro de factura con valor incorrecto '" + txtNroFacturaEnlazar.Text + "'";
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaParqueo", "Confirmar", msg);

                            //Control.Common.WinForm.ShowMessage(msg);

                            return;
                        }
                        _objParking.FacNumero = nroFactura;
                        using (POSEntities db = new POSEntities())
                        {
                            var factura = db.core_factura.Where(x => x.establecimiento == Common.GlobalParameters.Establecimiento && x.punto_emision == _objParking.FacPtoEmision && x.numero == _objParking.FacNumero).FirstOrDefault();
                            if (factura == null)
                            {
                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[NroFacturaEnlazar]", valor = txtNroFacturaEnlazar.Text });
                                Control.Common.General.GetMensajeToList(600, parametros);

                                msg = "Se está intentando enlazar el parqueo con una factura que no existe '" + lblFacEnlazar.Text + "'";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaParqueo", "Confirmar", msg);

                                //Control.Common.WinForm.ShowMessage(msg);
                                return;
                            }
                            //comentario de Prueba Publicar --Borrar

                            if (factura.fecha_creacion < _objParking.FechaIngreso)
                            {
                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[FechaIngreso]", valor = _objParking.FechaIngreso.ToString("dddd, dd MMMM yyyy H:mm")  });
                                parametros.Add(new ParametrosMensajes() { codigo = "[fecha_creacion]", valor = factura.fecha_creacion.ToString("dddd, dd MMMM yyyy H:mm") });
                                Control.Common.General.GetMensajeToList(521, parametros);

                                msg = "Se está intentando enlazar el parqueo con una factura que se creó antes de su ingreso. Ingreso Parqueo: '" + _objParking.FechaIngreso.ToString("dddd, dd MMMM yyyy H:mm") + "'. Creacion Factura: '" + factura.fecha_creacion.ToString("dddd, dd MMMM yyyy H:mm") + "'";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaParqueo", "Confirmar", msg);
                                
                                //Control.Common.WinForm.ShowMessage(msg);
                                return;
                            }

                            var tieneParqueo = db.core_facturadetalle.Any(x => x.factura_id == factura.id && Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(i => i.Equals(x.item_id)));
                            if (tieneParqueo)
                            {
                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[lblFacEnlazar]", valor = lblFacEnlazar.Text });
                                Control.Common.General.GetMensajeToList(522, parametros);

                                msg = "Se está intentando enlazar el parqueo con una factura que ya tiene item de parqueo '" + lblFacEnlazar.Text + "'";
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaParqueo", "Confirmar", msg);

                                //Control.Common.WinForm.ShowMessage(msg);

                                return;
                            }

                            _objParking.FacFechaCreacion = factura.fecha_creacion;
                            _objParking.FacValorTotal = factura.total;
                            _objParking.FacEnlaceId = factura.id;
                        }
                    }
                    else
                    {
                        _objParking.FacFechaCreacion = new DateTime();
                        _objParking.FacPtoEmision = string.Empty;
                        _objParking.FacNumero = 0;
                    }
                    _objParking.EstaConfirmado = true;
                    this.Close();
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "CapturaParqueo", "Confirmar", "Se está intentando confirmar el código que escribieron en el formulario '" + txtCodigo.Text + "' antes de realizar el paso de validación.");
                    //Control.Common.WinForm.ShowMessage("Validar el código '" + txtCodigo.Text + "' antes de confirmar");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[txtCodigo]", valor = txtCodigo.Text });
                    Control.Common.General.GetMensajeToList(523, parametros);

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "CapturaParqueo", "Confirmar", "Ocurrió un incidente mientras se confirmaba parqueo del código '" + txtCodigo.Text + "'. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No fue posible confirmar el código '" + txtCodigo.Text + "'. Por favor inténtelo nuevamente");
                parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[txtCodigo]", valor = lblFacEnlazar.Text });
                Control.Common.General.GetMensajeToList(524, parametros);

            }
        }

        private void Validar()
        {
            try
            {
                if (_pathIngreso == "API")
                {
                    WebMethods wbser = new WebMethods();
                    var res=wbser.ObtenerTicketParqueo(txtCodigo.Text, _WSIngreso);
                    DateTime date ;
                    if(res==null )
                    {
                        _objParking = null;
                        btnConfirmar.Enabled = false;
                        lblValidacion.Text = "Código no existe, verifique y vuélvalo a intentar";
                    }
                    else if(res.status == null)
                    {
                        _objParking = null;
                        btnConfirmar.Enabled = false;
                        lblValidacion.Text = "El ticket es invalido";
                    }
                    else
                    {
                        if (res.status == "paid")
                        {
                            _objParking = null;
                            btnConfirmar.Enabled = false;
                            lblValidacion.Text = "El ticket esta pagado";
                        }
                        else if (res.status == "usado")
                        {
                            _objParking = null;
                            btnConfirmar.Enabled = false;
                            lblValidacion.Text = "El ticket esta usado, carro salió";
                        }
                        else if (res.date_entry.Date<DateTime.Now.Date )
                        {
                            _objParking = null;
                            btnConfirmar.Enabled = false;
                            lblValidacion.Text = "El ticket esta vencido";
                        }
                        else
                        {

                            lblValidacion.Text = "Hora ingreso: " + res.date_entry;
                            _objParking = new Models.Parking.clsParking
                            {
                                Codigo = txtCodigo.Text,
                                DebeEnlazarFactura = chkEnlazarFactura.Checked,
                                FacPtoEmision = txtPtoEmisionEnlazar.Text,
                                MinutosFraccion = _tiempoFraccion,
                                MinutosGracia = _tiempoDefaultGracia,
                                FechaIngreso = res.date_entry, // DateTime.Now.Date.Add(TimeSpan.Parse(res.date_entry)),
                                MinutosLibreMaxPorCompra = _tiempoLibreMax,
                                MinutosLibreMaxPorCompra2 = _tiempoLibreMax2,
                                ValorMinCompraParaMinutosLibre = _tiempoLibreMinCompra,
                                ValorMinCompraParaMinutosLibre2 = _tiempoLibreMinCompra2,
                                ItemIdParqueo = _itemIdParqueo,
                                ItemIdParqueoSinCompra = _itemIdParqueoSinCompra,
                                MinutosFraccionSinCompra = _tiempoFraccionSinCompra,
                                PathIngreso = _pathIngreso,
                                PathSalida = _pathSalida,
                                PathHorario = _pathHorario
                            };
                            int facNro = 0;
                            int.TryParse(txtNroFacturaEnlazar.Text, out facNro);
                            _objParking.FacNumero = facNro;

                            btnConfirmar.Enabled = true;
                            btnConfirmar.Focus();
                        }
                    }
                }
                else
                {
                    
                    txtCodigo.Text = txtCodigo.Text.Trim().Substring(0, 12);

                    var path = Path.Combine(_pathIngreso, DateTime.Now.Date.ToString("yyyyMMdd"), txtCodigo.Text + ".txt");
                    if (File.Exists(path))
                    {
                        var pathSalida = Path.Combine(_pathSalida, DateTime.Now.Date.ToString("yyyyMMdd"), txtCodigo.Text + ".txt");
                        if (File.Exists(pathSalida))
                        {
                            var content = File.ReadAllText(path);
                            lblValidacion.Text = "Hora ingreso: " + content;
                            _objParking = new Models.Parking.clsParking
                            {
                                Codigo = txtCodigo.Text,
                                DebeEnlazarFactura = chkEnlazarFactura.Checked,
                                FacPtoEmision = txtPtoEmisionEnlazar.Text,
                                MinutosFraccion = _tiempoFraccion,
                                MinutosGracia = _tiempoDefaultGracia,
                                FechaIngreso = DateTime.Now.Date.Add(TimeSpan.Parse(content)),
                                MinutosLibreMaxPorCompra = _tiempoLibreMax,
                                MinutosLibreMaxPorCompra2 = _tiempoLibreMax2,
                                ValorMinCompraParaMinutosLibre = _tiempoLibreMinCompra,
                                ValorMinCompraParaMinutosLibre2 = _tiempoLibreMinCompra2,
                                ItemIdParqueo = _itemIdParqueo,
                                ItemIdParqueoSinCompra = _itemIdParqueoSinCompra,
                                MinutosFraccionSinCompra = _tiempoFraccionSinCompra,
                                PathIngreso = _pathIngreso,
                                PathSalida = _pathSalida,
                                PathHorario = _pathHorario
                            };
                            int facNro = 0;
                            int.TryParse(txtNroFacturaEnlazar.Text, out facNro);
                            _objParking.FacNumero = facNro;

                            btnConfirmar.Enabled = true;
                            btnConfirmar.Focus();
                        }
                        else
                        {
                            _objParking = null;
                            btnConfirmar.Enabled = false;
                            lblValidacion.Text = "Vehículo ya salió del estacionamiento";
                        }
                    }
                    else
                    {
                        _objParking = null;
                        btnConfirmar.Enabled = false;
                        lblValidacion.Text = "Código no existe, verifique y vuélvalo a intentar";
                    }
                }
            }
            catch (Exception ex)
            {
                _objParking = null;

                List<ParametrosMensajes>  parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[txtCodigo]", valor = lblFacEnlazar.Text });
                Control.Common.General.GetMensajeToList(525, parametros);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "CapturaParqueo", "Validar", "Ocurrió un incidente mientras se validaba el código '" + txtCodigo.Text + "'. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //Control.Common.WinForm.ShowMessage("No fue posible validar el código '" + txtCodigo.Text + "'. Por favor inténtelo nuevamente");
            }
        }

        private void CapturaParqueo_Load(object sender, EventArgs e)
        {

            lblFacEnlazarAyuda.Text = "F-" + Common.GlobalParameters.Establecimiento.PadLeft(3, '0') + "-";
            ActualizarFacEnlazar();

            lblValidacion.Text = string.Empty;
            CargarParametros();
        }

        private void chkEnlazarFactura_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEnlazarFactura.Checked)
            {
                if (_objParking != null)
                {
                    boxEnlazarFactura.Visible = true;
                    chkEnlazarFactura.Visible = false;
                }
                else
                {
                    chkEnlazarFactura.Checked = false;
                    //Control.Common.WinForm.ShowMessage("Antes debe haber cargado un código de parqueo válido");
                    Control.Common.General.GetMensajeToList(526);
                }
            }
        }

        private void lklCancelar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            boxEnlazarFactura.Visible = false;
            chkEnlazarFactura.Checked = false;
            chkEnlazarFactura.Visible = true;
        }

        private void txtPtoEmisionEnlazar_TextChanged(object sender, EventArgs e)
        {
            ActualizarFacEnlazar();
        }

        private void txtNroFacturaEnlazar_TextChanged(object sender, EventArgs e)
        {
            ActualizarFacEnlazar();
        }

        private void ActualizarFacEnlazar()
        {
            lblFacEnlazar.Text = lblFacEnlazarAyuda.Text + txtPtoEmisionEnlazar.Text.PadLeft(3, '0') + "-" + txtNroFacturaEnlazar.Text.PadLeft(9, '0');
        }

        private void btnNoTicket_Click(object sender, EventArgs e)
        {
            _noTieneTicket = true;
            _objParking = null;
            this.Close();
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                return;
                //if (txtCodigo.Text.Trim().Length > 12)
                //{
                //    txtCodigo.Text = txtCodigo.Text.Trim().Substring(0, 12);
                //}
                //Validar();
            }
        }
    }
}
