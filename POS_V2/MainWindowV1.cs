using System;
//using System.Reflection;
//using System.Deployment.Application;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using POS.Control;
using POS.Control.Auditor;
using POS.Control.Fingerprint;
using POS.Control.Garancheck;
using POS.Models;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using static POS.Models.ClsMessageQueue;
using System.Messaging;
using System.Net;
using POS.Control.Clientes;
using POS.Control.Pagos;
using POS.Control.ToolBox;
using System.Globalization;
using System.Xml;
using ZXing;
using POS.Control.CajaPinpad.Modelo;
using MensajesLibrary;




namespace POS
{

    public partial class MainWindowV1 : Telerik.WinControls.UI.RadForm
    {
        /* Code to Disable WinKey, Alt+Tab, Ctrl+Esc Starts Here */
        // Structure contain information about low-level keyboard input event 
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        //System level functions to be used for hook and unhook keyboard input  
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        //Declaring Global objects     
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;
        public string AplicaPromoDescuentoProductos = "";
        public static string IPServidorConectado;
        public static int IsServerChanged = 0;
        public static string URLPATHIMGBUSQPROD = "http://reportes.liris.com.ec/ImgAx/100/default.jpg";
        public static bool UtilizaMenuInicialTipoVenta = false;
        public static bool SALTARPRODUCTOETIQUETA = false;
        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling Windows keys 

                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags) || objKeyInfo.key == Keys.Escape && (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    return (IntPtr)1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }

        /* Code to Disable WinKey, Alt+Tab, Ctrl+Esc Ends Here */
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

  
        List<RadButton> lista_botonones = new List<RadButton>();
        pos_customer cliente_actual = null;
        TarjetaRegalo _tarjetaRegalo;
        Factura _factura = null;
        InputBoxDialog _inputform;
        InputBoxDialog _inputFormAuthUser;
        bool usotarjetadscto = false;
        string codigoTarjPromocion = "";
        bool flagPromocionPrimera = false;
        bool flagPromocionAcumulado = false;
        bool flagPromocionDescuento = false;
        bool flagProcesarDsct = false;
        bool esFactura = false;
        bool esGiftCard = false;
        bool esRecarga = false;
        bool flagCarnicero = false;
        System.Windows.Forms.Control focused;
        User _current_user;
        private TomaPeso scanner;
        private OposScanner_CCO.OPOSScanner scannerDL;
        private OposScale_CCO.OPOSScale scannerDLW;
        public static string establecimiento_inicio;
        public string codigocliente;
        public string BotonDirec1;
        public string BotonDirec2;
        public string BotonDirec3;
        public string BotonDirec4;
        public string BotonDirec5;
        bool flagDescuentoBarra = false;
        bool flagCompraGratis = false;
        bool Es2X_CONSULTA_POS = false;
        string NoGrabaTMP = "FALSE";
        string codigoPromocion = "";
        bool usoCodigoPromo = false;
        bool activoCodigoPromo = false;
        bool aplicaDsctoPromoTarjetaBines = false;
        decimal DescuentoPromoTarjBines = 0;
        string binTarjetaPromoDscto = string.Empty;
        bool flagBorrarPago = false;
        bool tieneProductosTmp = false;
        public string codigoclienteAPP;
        MenuInicial MenuIni;
        List<string> ListaCategoriaSaltarItemxEtiqueta = new List<string>();
        bool TieneConectividadPathImagenBusqProd = true;

        UC_Loading UCLoading;
        frmLoading frmLoad;

        string ClienteCompraGratis = "";

        decimal saldoCompraGratis = 0;
        decimal porcenDsctoCompraGratis = 0;
        decimal montoCompraGratis = 0;
        DateTime expiraConsumoCompraGratis;
        DateTime activaConsumoCompraGratis;
        public core_TarjetaDescuento codigoCuponPromocional = new core_TarjetaDescuento();

        List<List<VW_PromoChoose>> promo_list;
        [DllImport("User32.dll")]
        private static extern int FindWindow(string lpClassName,
                                     string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(int hWnd,int nCmdShow);

        public Factura FacturaActual { get { return _factura; } }
        public pos_customer ClienteActual { get { return cliente_actual; } }

        List<Control.WalletPoints.ClsListPoints> listPoints = new List<Control.WalletPoints.ClsListPoints>();
        public static List<Promocion> listPromociones = new List<Promocion>();

        /**
        * Metodo que inicia el Administrador de Tareas, con la ventana oculta             
        */
        static void StartHiddenTaskManager()
        {
            Process p = new Process();
            p.StartInfo.WorkingDirectory =  Environment.GetFolderPath(Environment.SpecialFolder.System);
            p.StartInfo.FileName = "taskmgr.exe";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();
        }
        /**
        * Cierra el administrador de tareas
        */
        static void CloseTaskManager()
        {
            Process[] procs = Process.GetProcessesByName("taskmgr");
            for (int i = 0; i < procs.Length; i++)
            {
                try
                {
                    procs[i].Kill();
                }
                catch
                {
                }
            }
        }

        /**
           * Asegurar que el Task Manager siempre esta abierto y oculto.
           * Si esta cerrado, lo abre de nuevo. Si tiene focus, llama al 
           * metodo para quitarselo, ya que si tiene focus, aun que sea 
           * invisible cuando se presiona 'Supr' o 'Del', puede matar un proceso.
           */
        static void HideTaskManager()
        {
            // while (true)
            {
                try
                {
                    //Obtener la ventana del administrador de tareas
                    //(ingles y español)
                    int taskManager =
                        FindWindow("#32770", "Windows Task Manager");
                    int adminTareas =
                        FindWindow("#32770", "Administrador de tareas de Windows");

                    //Si no existe, crearlo
                    if (taskManager == 0 && adminTareas == 0)
                    {
                        StartHiddenTaskManager();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #region Background ProcesaDisponibilidadImagenProductos

        private string ResponseBackground { get; set; }
        private bool EstadoControles { get; set; }
        private bool DebeCerrarFormBackground { get; set; }
        private Models.PagoPinpad PagoPinpadBackground { get; set; }

        BackgroundWorker bgw1;

        private void ProcesamientoBuscarItemBackGround()
        {

            if (UCLoading == null)
            {
                UCLoading = new UC_Loading();
            }


            UCLoading.Visible = true;
            
            UCLoading.Top = 20;
            UCLoading.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (UCLoading.Width / 2);
            //UCLoading.TopMost = true;

            if (bgw1 == null)
            {
                bgw1 = new BackgroundWorker();
                bgw1.DoWork += new DoWorkEventHandler(bgw1_DoWork);
                bgw1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw1_RunWorkerCompleted);
            }
            bgw1.WorkerReportsProgress = true;
            bgw1.WorkerSupportsCancellation = true;
            bgw1.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }
        void bgw1_DoWork(object sender, DoWorkEventArgs e)
        {
            // ProcesaPinpad();
            TieneConectividadPathImagenBusqProd = ValidaConectividadPathImagenBusqProd();
        }

        void bgw1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //paneLoading.Visible = false;
                UCLoading.Visible = false;
                On_Off_Controles(true);

                /*
                if (!string.IsNullOrWhiteSpace(ResponseBackground)) MessageBox.Show(ResponseBackground, "Pinpad", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                //Agregar pago desde repositorio background si existe
                if (PagoPinpadBackground != null)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "bgw_RunWorkerCompleted", "Cobro tarjeta background agregado a los pagos de la factura '" + _factura.GetNumeroFactura() + "'. Datos cobro [ Valor: " + PagoPinpadBackground.Valor.ToString("N2") + "; Banco: " + PagoPinpadBackground.Banco + "; Nombre: " + PagoPinpadBackground.Nombre + "; Marca: " + PagoPinpadBackground.Marca + "; TipoPOS: " + PagoPinpadBackground.TipoPos + "; BinDescripcion: " + PagoPinpadBackground.BinDescripcion + " ]");
                   
                }
                */

                //Background activara una bandera si se debe cerrar el formulario
                //if (DebeCerrarFormBackground) this.Close();
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "bgw_RunWorkerCompleted", "La solicitud no pudo ser realizada debido a un incidente en los controles. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "Stacktrace " + ex.StackTrace);
                //MessageBox.Show("La solicitud no pudo ser completada. Contacte a administrador");
            }
        }

        private void On_Off_Controles(bool estado)
        {
            this.btnSearchPro.Enabled = estado;
            /*  
              
              EstadoControles = estado;
              txtValor.Enabled = estado;
              cmbBancoTarjeta.Enabled = estado;
              cmbTipoTransaccion.Enabled = estado;
              btn0.Enabled = estado;
              btn1.Enabled = estado;
              btn2.Enabled = estado;
              btn3.Enabled = estado;
              btn4.Enabled = estado;
              btn5.Enabled = estado;
              btn6.Enabled = estado;
              btn7.Enabled = estado;
              btn8.Enabled = estado;
              btn9.Enabled = estado;
              btnBorrar.Enabled = estado;
              btnCancelar.Enabled = estado;
              btnEliminar.Enabled = estado;
              btnPagoManual.Enabled = estado;
              btnEnter.Enabled = estado;
              btnVerificar.Enabled = estado;
              btnPunto.Enabled = estado;
              txtCuenta.Enabled = estado;
              txtNumCheque.Enabled = estado;
              gridPagos.Enabled = estado;*/
        }

        #endregion

        public MainWindowV1(User usuario, AppData data)
        {

            InitializeComponent();
            //CloseTaskManager();
            //StartHiddenTaskManager();
            //HideTaskManager();

            try
            {
                Data = data;                               // Create the application data object
                                                           //            Data.OnChange += delegate { ExchangeData(false); };	// Track data changes to keep the form synchronized

                Verifier = new VerificationForm(Data, _factura);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificationForm", "Ejecuta VerificationForm ");

                _current_user = usuario;
                btnUser.Text = _current_user.nombres;
                lista_botonones.Add(btnFactura);
                lista_botonones.Add(btnGiftback);

                //lista_botonones.Add(btnRecarga);
                //lista_botonones.Add(btnCreditoPavos);

                POSEntities db = new POSEntities();
                //Si parametro descuento PaviPlan esta deshabilitado
                //No mostrar boton superior de PaviPlan
                //bool boolPromoPaviPlan = true;
                //var ActivaPromoPaviPlan = !POS.Control.Common.Promo.EstaActivaPromoPaviPlan();
                //if (ActivaPromoPaviPlan) { boolPromoPaviPlan = false; }
                //btnCreditoPavos.Visible = boolPromoPaviPlan;

                promo_list = new List<List<VW_PromoChoose>>();

                foreach (var b in lista_botonones)
                {
                    b.MouseDown += btn_MouseDown;
                }


            }
            catch (Exception ex)
            {

                // MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS", Titulo, 0, false)
                //MsgBox msgBox = new MsgBox("info", " El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS");

                //DialogResult dg = msgBox.ShowDialog();
                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS");

                Control.Common.General.GetMensajeToList(170);

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MainWindow(Constructor)", "El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se envió a cerrar el aplicativo, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;

                Environment.Exit(1);
            }

            if (Control.Common.GlobalParameters.LogoLocal != null)
                pbLogoPOS.Image = Image.FromFile(Control.Common.GlobalParameters.LogoLocal);
        }


        private void cargaFacturatmp()
        {

            if (NoGrabaTMP == "FALSE" && !Es2X_CONSULTA_POS)
            {
                string nombre_BD = Application.StartupPath + "\\Libs\\POS.db";

                string conexion = "Data Source=" + nombre_BD + ";Version=3;";

                SQLiteConnection sq = new System.Data.SQLite.SQLiteConnection(conexion);
                string comando = "select count(*) from cabecera";
                sq.Open();

                try
                {
                    //inicio de transaccion y borrado de datos anteriores
                    SQLiteCommand cmd;
                    //Codigo para insertar las nuevas lineas
                    cmd = new SQLiteCommand(comando, sq);
                    long retorno = (long)cmd.ExecuteScalar();
                    cmd.Dispose();
                    cmd = null;
                    // MessageBox.Show(this,retorno.ToString());
                    if (retorno != 0)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturaTmp", "Se ha encontrado datos de factura previa en la base temporal, se cargarán los datos a la pantalla");
                        comando = "select cliente_ax from cabecera";
                        cmd = new SQLiteCommand(comando, sq);
                        codigocliente = (string)cmd.ExecuteScalar();
                        usotarjetadscto = false;
                        cambiarCliente(codigocliente);
                        cmd.Dispose();
                        cmd = null;

                    }


                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmp", "No fue posible cargar los datos de Cabecera de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                    string MsjError = string.Concat(comando, "; ", ex.Message);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmp", MsjError);

                    Control.Common.General.GetMensajeToList(138);

                    //Manejo de error
                    //System.Windows.Forms.MessageBox.Show(this, comando);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    //string MsjError = string.Concat(comando, "; ", ex.Message);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, MsjError, "POS ");

                }


                try
                {
                    SQLiteCommand cmd;
                    comando = "select count(*) from detalle";
                    cmd = new SQLiteCommand(comando, sq);
                    long cant = (long)cmd.ExecuteScalar();
                    cmd.Dispose();
                    cmd = null;
                    if (cant != 0)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturaTmp", "Se han encontrado datos de productos de una factura previa en la base temporal, se cargarán los datos a la pantalla");
                        comando = "select item_id,cantidad,unidades,subtotal,descuento,iva,total,unidad,costo,precio from detalle";
                        cmd = new SQLiteCommand(comando, sq);

                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                Producto itm = new Producto();
                                itm.getProducto(rdr.GetString(0) /*item_id*/, _factura, cliente_actual);

                                itm.CantidadINEC = rdr.GetDecimal(1); //cantidad,
                                itm.Cantidad = rdr.GetDecimal(1); //cantidad,
                                itm.Unidades = rdr.GetInt16(2); //unidades,
                                itm.Subtotal = rdr.GetDecimal(3); //subtotal,
                                itm.Descuento = rdr.GetDecimal(4); //itm.DescuentoAX = rdr.GetDecimal(4); //descuento,
                                itm.Iva = rdr.GetDecimal(5); //iva,
                                itm.Unidad = rdr.GetString(7); //unidad,
                                itm.Costo = rdr.GetDecimal(8); //costo,
                                itm.Pvp = rdr.GetDecimal(9); //precio
                                itm.Total = rdr.GetDecimal(6); //total,

                                existeEnListaDescuento(itm.Id); //Verifica si esta en lista de descuentos AX
                                if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))//cliente_actual.CUSTGROUP != "07" && cliente_actual.CUSTGROUP != "09" /*&& cliente_actual.CUSTGROUP != "EM"*/ && cliente_actual.CUSTGROUP != "CE")
                                {
                                    itm.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);
                                }

                                itm.update();
                                _factura.Productos.Add(itm);
                                /*
                                Console.WriteLine(rdr.GetInt32(0) + " "
                                    + rdr.GetString(1) + " " + rdr.GetInt32(2));
                                    */
                            }
                        }

                        PromosPrecioPorCombinacion();
                        PromosDsctoPorSuplemento();

                        promoiva(_factura, null);
                        calcularFactura();
                        cmd.Dispose();
                        cmd = null;

                        RefrescarGridItems();

                        if (Control.Common.GlobalParameters.UserObj == null)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturatmp", "Se detectaron items temporales pero no se pudo levantar la pantalla para la aceptacion del cajero porque el objeto de usuario estaba nulo");
                        }
                        else
                        {
                            if (_factura.Productos.Count > 0 && !Control.Common.GlobalParameters.UserObj.isSuperUser)
                            {
                                Control.Main.TempInvoiceAlert frm = new Control.Main.TempInvoiceAlert();
                                frm.ShowDialog();
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show(this, comando);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmp", "No fue posible cargar los datos de Detalle de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                    Control.Common.General.GetMensajeToList(139);

                    string MsjError = string.Concat(comando, "; ", ex.Message);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmp", MsjError);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, MsjError, "POS");

                }
                //Cerramos conexion

                sq.Close();
                sq.Dispose();
                sq = null;
            }
        }
        private void eliminaFacturatmp()
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (NoGrabaTMP == "FALSE" && !Es2X_CONSULTA_POS)
            {
                string nombre_BD = Application.StartupPath + "\\Libs\\POS.db";
                string conexion = "Data Source=" + nombre_BD + ";Version=3;";

                SQLiteConnection sq = new System.Data.SQLite.SQLiteConnection(conexion);
                string comando = "BEGIN TRANSACTION; delete from cabecera;delete from detalle;delete from pago;COMMIT;VACUUM ; ";
                try
                {

                    sq.Open();
                    //inicio de transaccion y borrado de datos anteriores
                    SQLiteCommand cmd;
                    //Codigo para insertar las nuevas lineas
                    cmd = new SQLiteCommand(comando, sq);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "EliminaFacturaTmp", "No fue posible eliminar los datos de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    //Manejo de error
                    //System.Windows.Forms.MessageBox.Show(this, comando);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    string MsjError = string.Concat(comando, "; ", ex.Message);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, MsjError, "POS");
                    //Control.Common.General.GetMensajeToList("POS", MsjError, "I");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = MsjError });
                    Control.Common.General.GetMensajeToList(586, parametros);



                }

                //Cerramos conexion

                sq.Close();
                sq.Dispose();
                sq = null;
            }
        }
        private void insertaCabecera()
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (NoGrabaTMP == "FALSE" && !Es2X_CONSULTA_POS)
            {
                string nombre_BD = Application.StartupPath + "\\Libs\\POS.db";
                string conexion = "Data Source=" + nombre_BD + ";Version=3;";

                SQLiteConnection sq = new System.Data.SQLite.SQLiteConnection(conexion);
                if (codigocliente == null)
                    codigocliente = txtCedula.Text;

                string comando = "BEGIN TRANSACTION;delete from cabecera;INSERT INTO cabecera(cliente_ax) VALUES ('" + codigocliente + "');COMMIT;  ";
                try
                {

                    sq.Open();
                    //inicio de transaccion y borrado de datos anteriores
                    SQLiteCommand cmd;
                    //Codigo para insertar las nuevas lineas
                    cmd = new SQLiteCommand(comando, sq);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                    // MessageBox.Show(this,retorno.ToString());

                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "InsertaCabecera", "No fue posible cargar los datos de cabecera de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    //Manejo de error
                    //System.Windows.Forms.MessageBox.Show(this, comando);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    string MsjError = string.Concat(comando, "; ", ex.Message);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, MsjError, "POS");
                    //Control.Common.General.GetMensaje("POS", MsjError, "ER");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = MsjError });
                    Control.Common.General.GetMensajeToList(586, parametros);


                }

                //Cerramos conexion

                sq.Close();
                sq.Dispose();
                sq = null;
            }
        }

        private void agregaProductosTmp(string codigo)
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (NoGrabaTMP == "FALSE" && !Es2X_CONSULTA_POS)
            {
                // Ruta compartida por local.  JM 11-09-2019 
                string nombre_BD = Application.StartupPath + "\\Libs\\POS.db";

                string conexion = "Data Source=" + nombre_BD + ";Version=3;";

                SQLiteConnection sq = new System.Data.SQLite.SQLiteConnection(conexion);

                string comando = "BEGIN TRANSACTION;delete from detalle;INSERT INTO detalle(item_id,cantidad,unidades,subtotal,descuento,iva,total,unidad,costo,precio) VALUES ";
                try
                {

                    sq.Open();
                    //inicio de transaccion y borrado de datos anteriores
                    SQLiteCommand cmd;
                    //Codigo para insertar las nuevas lineas
                    string datos = "";
                    foreach (Producto prod in _factura.Productos)
                    {
                        datos = datos + "('" + prod.Id + "'," + prod.Cantidad + "," + prod.Unidades + "," + prod.Subtotal + "," + prod.Descuento + "," + prod.Iva + "," + prod.Total + ",'" + prod.Unidad + "'," + prod.Costo + "," + prod.Pvp + "), ";
                    }
                    if (_factura.Productos.Count > 0)
                    {
                        comando = comando + datos.Substring(0, datos.Length - 2) + ";COMMIT; ";
                    }
                    else
                    {
                        comando = "BEGIN TRANSACTION;delete from detalle; COMMIT;";
                    }
                    cmd = new SQLiteCommand(comando, sq);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;


                    // MessageBox.Show(this,retorno.ToString());

                }
                catch (Exception ex)
                {
                    string MsjError = string.Concat(comando, "; ", ex.Message);
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "AgregaProductoTmp"
                        , "No fue posible agregar productos a la factura temporal, a continuacion las excepciones encontradas - " 
                        + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " 
                        + ex.StackTrace);


                    //Manejo de error
                    //System.Windows.Forms.MessageBox.Show(this, comando);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, MsjError, "POS");
                    //Control.Common.General.GetMensaje("POS", MsjError, "I");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = codigo });
                    Control.Common.General.GetMensajeToList(586, parametros);


                }

                //Cerramos conexion

                sq.Close();
                sq.Dispose();
                sq = null;
            }

        }

        private void txtCedula_KeyPress(object sender, KeyPressEventArgs e)
        {

            //if (e.KeyChar == (char)Keys.Delete || e.KeyChar == (char)Keys.Back)
            //{
            //    txtCedula.Text = "";
            //}
            //if (txtCedula.Text.Length != txtCedula.SelectionStart)
            //{
            //    e.Handled = true;
            //}

            //if (txtCedula.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
            //{
            //    LimpiarClienteCompraGratis();
            //    tempo666.Start();
            //}

            if (InputLanguage.CurrentInputLanguage.Culture.EnglishName.ToUpper() != "ENGLISH (UNITED STATES)")
            {
                foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
                {
                    if (lang.Culture.EnglishName.ToUpper() == "ENGLISH (UNITED STATES)")
                    {
                        InputLanguage.CurrentInputLanguage = lang;
                    }
                }
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "Se define paarametros btnCuponApp.Visible = false");
                btnCuponApp.Visible = false;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "Se define paarametros EsUsoAppMovil = false");
                _factura.EsUsoAppMovil = false;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "Se define paarametros codigoclienteAPP = 0");
                codigoclienteAPP = "0";

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "Se define btnMonedero / btnCuponApp como visible FALSE");
                if (btnMonedero.Visible == true) { btnMonedero.Visible = false; btnCuponApp.Visible = false; }

                string itendifacionCompleta = txtCedula.Text;
                validaClienteSp(itendifacionCompleta);

            }

        }


        private void validaClienteSp(string itendifacionCompleta)
        {
            string identificacion = txtCedula.Text.Trim();
            string EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode;

            try
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "Ejecuta metodo ValidaClienteEmpleado. Identificación: " + txtCedula.Text + " | EstablecimientoAxCode: " + Control.Common.GlobalParameters.EstablecimientoAxCode);
                ClienteEmpleado clteEmpleado = Control.Common.General.ValidaClienteEmpleado(identificacion, EstablecimientoAxCode);

                if (clteEmpleado.CodMensaje != "0")
                {
                    int CodMensaje = Int32.Parse(clteEmpleado.CodMensaje);
                    var result = Control.Common.General.GetMensajeToList(CodMensaje);

                    if (result == MsgBoxCtrl.MessageBoxResult.No || result == MsgBoxCtrl.MessageBoxResult.Cancel)
                    {
                        
                    }

                    if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                    {


                    }

                }

                /*Recuerpo si es Cliente App. */
                if (clteEmpleado.EsClienteApp)
                {
                    LimpiarClienteCompraGratis();
                    tempo666.Start();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "validaClienteSp - El cajero ha pistoleado la Tarjeta Virtual : " + txtCedula.Text);
                    txtCedula.Text = clteEmpleado.Identificacion;
                    codigoclienteAPP = clteEmpleado.CodigoClienteApp;

                    btnCuponApp.Visible = true;
                    lblIdClienteApp.Text = codigoclienteAPP;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "validaClienteSp - EsUsoAppMovil =  " + _factura.EsUsoAppMovil);
                    _factura.EsUsoAppMovil = true;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "validaClienteSp - Antes de validar Parametro Compra Gratis ");
                    if (POS.Control.Common.GlobalParameters.CompraGratis)
                    {
                        ClienteCompraGratis = clteEmpleado.NumeroTarjetaEmpresa;
                        VerificarSaldoCompraGratis();
                    }


                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "validaClienteSp - Antes de validar ValidarMonederoCampania ");
                    usotarjetadscto = false;
                    var val1 = txtCedula.Text.Trim();
                    if (val1.Length > 0)
                    {
                        // Validar si tiene puntos acumulados para activar boton pagar con monedero.
                        ValidarMonederoCampania(clteEmpleado.Identificacion);
                    }

                }

                if (clteEmpleado.EsTarjetaEmpresa || clteEmpleado.EsEmpleadoLiris)
                {
                    // this.btnCreditoInterno.Visible = true;
                    /*Agrega el descuento */
                    _factura.EsEmpleadoLiris = clteEmpleado.EsEmpleadoLiris;
                    _factura.porcDsctoEmpleadoLiris = clteEmpleado.PorcEmpleadoLiris;
                    _factura.NumeroTarjetaEmpresa = clteEmpleado.NumeroTarjetaEmpresa;
                    _factura.NumeroTarjetaEmpresaAdicional = clteEmpleado.NumeroTarjetaEmpresaAdicional;
                    txtCedula.Text = clteEmpleado.Identificacion;
                }

                usotarjetadscto = false;
                var val = txtCedula.Text.Trim();
                codigocliente = val;
                if (val.Length > 0)
                {
                    // cambiarCliente(val);
                    cambiarCliente(itendifacionCompleta);
                }


                // Llamar a Parqueo.  JM 08-03-2019
                if (Control.Common.GlobalParameters.Parking_TienePermiso)
                {
                    SolicitarParqueo();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "validacliente", "validaClienteSp - Antes de validar ValidarMonederoCampania ");
            }
         
        }


        private void validaCliente(string itendifacionCompleta)
        {
            ClienteEmpleado clteEmpleado = new ClienteEmpleado();

            try
            {
                if (txtCedula.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
                {
                   
                    clteEmpleado = Control.Common.General.ValidaClienteEmpleado(txtCedula.Text.Trim(), Control.Common.GlobalParameters.EstablecimientoAxCode);

                    if (clteEmpleado != null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "CodError: " + clteEmpleado.CodError + ", MsjError: " + clteEmpleado.MsjError);

                        codigoclienteAPP = clteEmpleado.CodigoClienteApp;
                        txtCedula.Text = clteEmpleado.Identificacion;
                        _factura.EsUsoAppMovil = true;

                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "ValidaClienteEmpleado no encontro datos o no encontro el proceso. Se recuperan datos manuales ");


                        if (txtCedula.Text.Trim().IndexOf("-") > 0)
                        {
                            codigoclienteAPP = txtCedula.Text.Trim().Substring(txtCedula.Text.Trim().IndexOf("-") + 1, (txtCedula.Text.Trim().Length - txtCedula.Text.Trim().IndexOf("-")) - 1);
                            txtCedula.Text = txtCedula.Text.Trim().Substring(0, txtCedula.Text.Trim().IndexOf("-"));
                            _factura.EsUsoAppMovil = true;
                        }
                    }

                    lblIdClienteApp.Text = codigoclienteAPP;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "CodError: " + clteEmpleado.CodError + ", MsjError: " + clteEmpleado.MsjError);

                    
                    ClienteCompraGratis = txtCedula.Text.Trim();

                    if (POS.Control.Common.GlobalParameters.CompraGratis)
                    {
                        VerificarSaldoCompraGratis();
                    }

                    txtCedula.Text = txtCedula.Text.Trim().Remove(0, POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp.Length);
                    _factura.EsUsoAppMovil = true;

                    usotarjetadscto = false;
                    var val1 = txtCedula.Text.Trim();
                    if (val1.Length > 0)
                    {
                        // Validar si tiene puntos acumulados para activar boton pagar con monedero.
                        ValidarMonederoCampania(txtCedula.Text);
                    }

                }

                usotarjetadscto = false;
                var val = txtCedula.Text.Trim();
                codigocliente = val;
                if (val.Length > 0)
                {
                    cambiarCliente(val);
                }

                // Llamar a Parqueo.  JM 08-03-2019
                if (Control.Common.GlobalParameters.Parking_TienePermiso)
                {
                    SolicitarParqueo();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "ValidaClienteEmpleado - error: " + ex.Message);
            }
        }

        private void cambiarCliente(string val)
        {
            //msgBoxCtrl = new MsgBoxCtrl();

            var pos = new POSEntities();
            decimal LimiteFacturacionCF = 0;

            var ParamCUPO_CF = (from deta in pos.core_parametro
                                       where deta.identificador == "CUPO_CF"
                                       select deta).ToList();

            string textMensaje = string.Empty;
            bool validaClteActual = false;

            try
            {
                if (cliente_actual == null)
                {
                    validaClteActual = false;
                }
                else {
                    if (cliente_actual.ACCOUNTNUM == null) { validaClteActual = false; }
                    else { validaClteActual = true; }
                }

            }
            catch (Exception ex)
            {
                validaClteActual = false;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "txtCedula_KeyPress", "cambiarCliente - error: " + ex.Message);
            }



            if (validaClteActual)
            {
                if (!cliente_actual.ACCOUNTNUM.Equals(val) && !cliente_actual.VATNUM.Equals(val))
                {

                    var result = Control.Common.General.GetMensajeToList(56);
                    if (result == MsgBoxCtrl.MessageBoxResult.No || result == MsgBoxCtrl.MessageBoxResult.Cancel)
                    {
                        _factura.EsUsoAppMovil = false;
                        return;
                    }

                    lblEtiquetaSaldo.Visible = false;
                    lblSaldoTarjeta.Visible = false;

                    if (_factura.Descuentos2.Any())
                    {
                        _factura.Descuentos2.Clear();
                        _factura.Pagos.Clear();
                    }

                    //Valida: al cambiar el cliente se borre el pago con billetera electrónica
                    Pago pagoBElect = null;
                    foreach (var pago in _factura.Pagos)
                    {
                        if (pago.Descripcion == "DINE ELECT")
                        {
                            pagoBElect = pago;
                        }
                    }
                    if (pagoBElect != null)
                    {
                        _factura.Pagos.Remove(pagoBElect);
                    }


                    //Valida: al cambiar el cliente se borre el pedido app
                    if (_factura.EsPedidoOtraApp == true)
                    {
                        _factura.EsPedidoOtraApp = false;
                        _factura.PedidoOtraApp.Pedido = string.Empty;
                        _factura.PedidoOtraApp.Tipo = (byte)CANALVENTA.VENTANORMALPOS;
                        lblCanalVenta.Text = "Venta POS";

                        _factura.Productos.Clear();
                        lblEtiquetaSaldo.Visible = false;
                        lblSaldoTarjeta.Visible = false;

                        lblPedidoOtrasApp.Text = string.Empty;
                        lblPedidoOtrasApp.Visible = false;
                    }

                    //calcularFactura();
                    insertaCabeceraFile();  //insertaCabecera();
                }
                else
                {
                    if (pos.core_parametro.Where(x => x.identificador == "SWITCH_COSTUMER" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
                    {
                        if (pos.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
                        {
                            Verifier = new VerificationForm(Data, _factura);
                            Verifier.Tag = "adm";
                            verificador = Verifier.ShowDialog();
                            //  MessageBox.Show(this,Verifier.Tag.ToString());
                            if (verificador == DialogResult.OK)
                            {
                                _factura.Productos.Clear();
                                lblEtiquetaSaldo.Visible = false;
                                lblSaldoTarjeta.Visible = false;

                                if (_factura.Descuentos2.Any())
                                {
                                    _factura.Descuentos2.Clear();
                                    _factura.Pagos.Clear();
                                }

                                calcularFactura();
                                insertaCabeceraFile();  //insertaCabecera();
                            }
                            return;
                        }
                        try
                        {
                            DialogResult _authorize;
                            _inputFormAuthUser.setValue(String.Empty);
                            _inputFormAuthUser._txtInput.PasswordChar = '•';
                            _authorize = _inputFormAuthUser.ShowDialog();
                            focused = (System.Windows.Forms.Control)_inputFormAuthUser._txtInput;
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "cambiarCliene", "Error:" + ex.Message);
                            Control.Common.General.GetMensajeToList(141);

                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Por favor, intente nuevamente", "POS - Advertencia Cambia Cliente");
                            //System.Windows.Forms.MessageBox.Show(this, "Por favor, intente nuevamente", "Advertencia Cambia Cliente", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }

                        if (focused.Text.ToString() != "")
                        {
                            if (ValidateAuthorizationUser(focused.Text.ToString()) == true)
                            {
                                _factura.Productos.Clear();
                                lblEtiquetaSaldo.Visible = false;
                                lblSaldoTarjeta.Visible = false;

                                if (_factura.Descuentos2.Any())
                                {
                                    _factura.Descuentos2.Clear();
                                    _factura.Pagos.Clear();
                                }

                                calcularFactura();
                                insertaCabeceraFile();  //insertaCabecera();
                            }
                            else
                            {
                                Control.Common.General.GetMensajeToList(140);
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Código no válido ó Usuario no autorizado. Intente nuevamente", "POS - Advertencia Cambia Cliente");
                                //System.Windows.Forms.MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                return;
                            }
                        }
                    }
                }
            }

            if (Cliente.clienteExiste(val))
            {
                if (val == "9999999999999")
                {
                    List<ParametrosMensajes> ListParametros = new List<ParametrosMensajes>();
                    ListParametros.Add(new ParametrosMensajes() { codigo = "[PARAMETRO_CF]", valor = Control.Common.GlobalParameters.CUPO_CF.ToString() });
                    Control.Common.General.GetMensajeToList(57, ListParametros);
                }

                core_tarjetacreditointerno tarjeta = null;
                core_tarjetacreditointerno tarjetaAdicional = null;

                //val = _factura
                var nuevo_cliente = Cliente.getCliente(val, out tarjeta, out tarjetaAdicional);

                _factura.TarjetaCreditoInterno = null;
                _factura.TarjetaCreditoInternoAdicional = null;

                if (tarjeta != null)
                {
                    //_factura.TarjetaCreditoInterno = null;
                    //_factura.TarjetaCreditoInternoAdicional = null;

                    _factura.EsTarjetaCreditoInterno = true;
                    _factura.TarjetaCreditoInterno = tarjeta;

                    if (tarjetaAdicional != null)
                    {
                        _factura.EsTarjetaCreditoInternoAdicional = true;
                        _factura.TarjetaCreditoInternoAdicional = tarjetaAdicional;
                    }
                    
                    lblEtiquetaSaldo.Visible = true;
                    lblSaldoTarjeta.Visible = true;
                    lblSaldoTarjeta.Text = string.Format("{0:C}", (tarjetaAdicional == null) ? tarjeta.saldo : tarjetaAdicional.saldo);
                }

                if (nuevo_cliente != null)
                {
                    if (nuevo_cliente.EMAIL == "")
                    {
                        var result = Control.Common.General.GetMensajeToList(58);

                        if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                        {
                            var f = new POS.Control.Clientes.ClienteForm();
                            f._cliente = nuevo_cliente;
                            f.ShowDialog();

                            if (f._cliente != null)
                            {
                                cliente_actual = f._cliente;
                                txtCedula.Text = cliente_actual.VATNUM;
                                txtCodigo.Focus();
                                txtCodigo.SelectAll();
                                nuevo_cliente = f._cliente;
                            }
                        }
                    }

                    cliente_actual = nuevo_cliente;
                }

                if (cliente_actual != null)
                {
                    setClienteData();
                    txtCodigo.Focus();
                    txtCodigo.SelectAll();

                    validaClienteSp(txtCedula.Text);

                    //codigocliente = txtCedula.Text;
                    insertaCabeceraFile();  //insertaCabecera();

                }
                else
                {
                    clearClienteData();
                    txtCedula.Text = "";
                    txtCedula.Focus();

                    //var result = Control.Common.General.GetMensajeToList(142);
                    Control.Common.General.GetMensajeToList(142);

                    //msgBoxCtrl = new MsgBoxCtrl();
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Question, "Tiene el código de Afiliación?", "POS - Afilización");
                }

                if (clienteActivo())
                {
                    CambiarCliente_RecalcularFactura();
                    
                    //Levantar encuesta
                    POS.Control.Encuestas.EncuestaHandler.LevantarEncuestaFactura();

                    //Mostrar mensajes para cliente
                    var InvoiceMessagePrompter = new POS.Control.Main.ClsFacturaMensaje();
                    InvoiceMessagePrompter.ShowInvoiceMessages(cliente_actual.ACCOUNTNUM);
                }



                //calcularFactura();
            }
            else
            {
                if (ValidarIdentificador.ValidarCedula(val)
                        || ValidarIdentificador.ValidarRUCPrivada(val)
                        || ValidarIdentificador.ValidarRUCPublica(val)
                        || ValidarIdentificador.ValidarRUCNatural(val))
                {

                    //msgBoxCtrl = new MsgBoxCtrl();
                    //result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Question, "Cliente no existe!. Desea crearlo?", "POS - Cliente");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "cambiarCliente", " Cliente no existe, se levanta mensaje para crear");

                    var result = Control.Common.General.GetMensajeToList(59);
                    if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                    {
                        var f = new POS.Control.Clientes.ClienteForm();
                        f._cliente = null;
                        f.identificacion = val; //txtCedula.Text;
                        f.ShowDialog();

                        if (f._cliente != null)
                        {
                            cliente_actual = f._cliente;
                            txtCedula.Text = cliente_actual.VATNUM;
                            setClienteData();
                            insertaCabeceraFile();  //insertaCabecera();
                            txtCodigo.Focus();
                            txtCodigo.SelectAll();

                            //Levantar encuesta
                            POS.Control.Encuestas.EncuestaHandler.LevantarEncuestaFactura();

                            //Mostrar mensajes para cliente
                            var InvoiceMessagePrompter = new POS.Control.Main.ClsFacturaMensaje();
                            InvoiceMessagePrompter.ShowInvoiceMessages(cliente_actual.ACCOUNTNUM);
                        }
                        else
                        {
                            if (cliente_actual == null)
                                txtCedula.Clear();
                            else
                                txtCedula.Text = cliente_actual.ACCOUNTNUM;
                        }
                    }

                    else
                    {
                        if (cliente_actual == null)
                            txtCedula.Clear();
                        else
                            txtCedula.Text = cliente_actual.ACCOUNTNUM;
                    }

                }
                else
                {

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "cambiarCliente", " Cedula o RUC no Valido!\nSi es Pasaporte Ingreselo en AX  ");

                    Control.Common.General.GetMensajeToList(60);
                    txtCedula.Clear();
                }
            }

            if (cliente_actual != null)
            {
                verificarPromocionEspanola(cliente_actual.ACCOUNTNUM);
            }
        }

        private void actualizarDatos(pos_customer cliente = null)
        {
            var c = cliente != null ? cliente : cliente_actual;
            _factura.updateItems(c);
            /*for (var i = 0; i < _factura.Productos.Count; i++)
            {
                var item = _factura.Productos[i];
                var descuento = item.getDescuento(_factura, c);
                item0.calcularDescuento(descuento);
                item.update();
            }*/
        }

        private void clearClienteData()
        {
            cliente_actual = null;
            lblNombre.Text = "";
            lblDireccion.Text = "";
            lblTelefono.Text = "";
            txtCedula.Clear();
            txtCedula.Focus();
            lblEtiquetaSaldo.Visible = false;
            lblSaldoTarjeta.Visible = false;
        }

        private void setClienteData()
        {
            lblNombre.Text = cliente_actual.NAME;
            lblDireccion.Text = cliente_actual.ADDRESS;
            lblTelefono.Text = cliente_actual.PHONE;
            txtCedula.Text = cliente_actual.VATNUM != "" ? cliente_actual.VATNUM : cliente_actual.ACCOUNTNUM;

            _factura.ClienteIdentificacion = cliente_actual.VATNUM != "" ? cliente_actual.VATNUM : cliente_actual.ACCOUNTNUM;
            _factura.Cliente_codigo = cliente_actual.ACCOUNTNUM;
            _factura.Cliente_direccion = cliente_actual.ADDRESS;
            _factura.Cliente_grupo = cliente_actual.CUSTGROUP;
            _factura.Cliente_nombre = cliente_actual.NAME;
            _factura.Cliente_telefono = cliente_actual.PHONE;
        }

        public BindingList<Pago> PagosBinding { get; protected set; }
        private void enlazarControles()
        {
            PagosBinding = _factura.Pagos;
            this.gridItems.DataSource = _factura.Productos;
            //Agrega sort descendente al RadGridView
            this.gridItems.EnableCustomSorting = true;
            this.gridItems.CustomSorting += new GridViewCustomSortingEventHandler(radGridView1_CustomSorting);
            this.gridItems.Columns["IdTemporal"].SortOrder = RadSortOrder.Descending;
            this.gridPagos.DataSource = PagosBinding;
        }

        private void radGridView1_CustomSorting(object sender, GridViewCustomSortingEventArgs e)
        {
            Int64 row1Freight = (Int64)e.Row1.Cells["IdTemporal"].Value;
            Int64 row2Freight = (Int64)e.Row2.Cells["IdTemporal"].Value;

            if (row1Freight < row2Freight)
            {
                e.SortResult = 1;
            }
            else if (row1Freight > row2Freight)
            {
                e.SortResult = -1;
            }
            else
            {
                e.SortResult = 0;
            }
        }
        /// <summary>
        /// Valida si puede saltar los items de etiquete configurado en los parametros por caegoria del item.
        /// </summary>
        private void ParametroParaSaltarProductoEtiqueta()
        {
            bool flag;
            string param2EstPtoEmi = string.Empty;
            //flag = false;
            try
            {
                param2EstPtoEmi = POS.Control.Common.GlobalParameters.Establecimiento + "" + POS.Control.Common.GlobalParameters.PuntoEmision;
                using (POSEntities db = new POSEntities())
                {
                    if (db.core_parametro.Any(x => x.identificador == "SALTARPRODUCTOETIQUETA" && x.parametro2 == param2EstPtoEmi && x.valor.ToUpper().Equals("TRUE")))
                    {
                        SALTARPRODUCTOETIQUETA = true;

                        var param = db.core_parametro.FirstOrDefault(x => x.identificador == "SALTARPRODUCTOETIQUETA" && x.parametro2 == param2EstPtoEmi && x.valor.ToUpper().Equals("TRUE"));
                        if (!string.IsNullOrEmpty(param.documento))
                        {
                            String[] split;
                            int count = 0;
                            split = param.documento.Split(';');
                            count = split.Count();
                            if (count == 0)
                            {
                                SALTARPRODUCTOETIQUETA = false;
                                return;
                            }
                            for (int i = 0; i < count; i++)
                            {
                                ListaCategoriaSaltarItemxEtiqueta.Add(split[i].ToString().ToUpper());
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "HabilitarMenuInicial", "Ha courrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }
        private void HabilitarMenuInicial()
        {
            bool flag;
            string param2EstPtoEmi = string.Empty;
            //flag = false;
            try
            {
                param2EstPtoEmi = POS.Control.Common.GlobalParameters.Establecimiento + "" + POS.Control.Common.GlobalParameters.PuntoEmision;
                using (POSEntities db = new POSEntities())
                {
                    if (db.core_parametro.Any(x => x.identificador == "MENUINCIALTIPOVENTA" && x.parametro2 == param2EstPtoEmi && x.valor.ToUpper().Equals("TRUE")))
                    {
                        UtilizaMenuInicialTipoVenta = true;
                    }

                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "HabilitarMenuInicial", "Ha courrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }

            //  return flag;
        }
        private void llamaMenuInicial()
        {
            // msgBoxCtrl = new MsgBoxCtrl();

            try
            {

                //btnMenuInicial.Visible = false;
                lblCanalVenta.Visible = false;
                lblPedidoOtrasApp.Visible = false;

                //Valida si el establecimiento y Punto Emisión tiene habilitado Menu Inicial.
                if (UtilizaMenuInicialTipoVenta)
                {
                    // btnMenuInicial.Visible = true;
                    lblCanalVenta.Visible = true;
                    lblPedidoOtrasApp.Visible = true;

                    HabilitarControlesPedidosApp(true);

                    //Validación cuando tiene productos temporales entonces que pase directo a la opción Venta Normal por POS.
                    if (tieneProductosTmp)
                    {
                        _factura.EsPedidoOtraApp = false;
                        _factura.PedidoOtraApp.Pedido = string.Empty;
                        _factura.PedidoOtraApp.Tipo = (byte)CANALVENTA.VENTANORMALPOS;
                    }
                    else
                    {
                        MenuIni = new MainWindowV1(this);
                        MenuIni.ShowDialog();
                    }


                    if (this.FacturaActual != null)
                    {
                        if (!tieneProductosTmp)
                            LimpiarcontrolesPedido();

                        HabilitaBotonesSuperior(true, true);

                        if (this.FacturaActual.PedidoOtraApp != null)
                        {
                            lblPedidoOtrasApp.Text = this.FacturaActual.PedidoOtraApp.Pedido;
                            switch (this.FacturaActual.PedidoOtraApp.Tipo)
                            {
                                case 0:
                                    lblCanalVenta.Text = "Venta POS";
                                    break;
                                case 1:
                                    lblCanalVenta.Text = "" + "App DelPortal";
                                    CargaPedidoAppDelPortal();
                                    break;
                                case 2:
                                    lblCanalVenta.Text = "Pedidos " + "Ya";
                                    if (Control.Common.GlobalParameters.ActivaIntegracionPedidos)
                                    {
                                        using (WSIntegracion.Service1Client Integ = new WSIntegracion.Service1Client())
                                        {
                                            var xml = Integ.IntegrationDelivery(XmlIntegracion("Glovo", 3));

                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "llamaMenuInicial ", "Acknowledgement:" + xml.ToString());
                                            var xmlC = Integ.IntegrationDelivery(XmlIntegracion("Glovo", 4));

                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "llamaMenuInicial ", "State_change:" + xml.ToString());
                                        }
                                    }
                                    break;
                                case 3:
                                    lblCanalVenta.Text = "Pedido " + "Rappi";
                                    break;
                                default:
                                    /*lblCanalVenta.Text = "Pedido " + this.FacturaActual.Delivery ;
                                    InactivaFormaPagoDelivery();
                                    ActivaFormaPagoDelivery();*/
                                    //Console.WriteLine("Default case");
                                    break;
                            }
                            lblPedidoOtrasApp.Visible = true;

                            // lblCanalVenta.Text = this.FacturaActual.PedidoOtraApp.Tipo=;
                        }
                    }
                }
               
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "llamaMenuInicial", "Ha courrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
            }
        }
        private string XmlIntegracion(string canalVenta, int opcion)
        {
            string xml = string.Empty;
            string Integ = string.Empty;
            string Metodo = string.Empty;
            string versionPOS = "1.1.1.972";
            string OrderId = string.Empty;
            string sInvoice = string.Empty;
            string Id = string.Empty;
            string Description = string.Empty;
            Version ver = null;
            try
            {
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                    ver = ad.CurrentVersion;
                    versionPOS += ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision;
                }
                switch (canalVenta)
                {
                    case "Glovo":
                        Integ = "YA";
                        break;
                    case "Rappi":
                        Integ = "RA";
                        break;
                    default:
                        //Console.WriteLine("Default case");
                        break;

                }
                if (opcion == 1) //Inicializacion
                {

                    Metodo = "IN"+ Integ;
                }
                else if (opcion == 2)//Recepcion
                {
                    Metodo = "RE" + Integ;
                    OrderId = this.FacturaActual.PedidoOtraApp.Pedido;
                }
                else if (opcion == 3)//reconocimiento
                {
                    Metodo = "AL" + Integ;
                    OrderId = this.FacturaActual.PedidoOtraApp.Pedido;
                }
                else if (opcion == 4)//State_change
                {
                    Metodo = "SC" + Integ;
                    OrderId = this.FacturaActual.PedidoOtraApp.Pedido;
                }
                XmlDocument xmlDoc = new XmlDocument();

                XmlNode rootNode = xmlDoc.CreateElement("Root");
                XmlAttribute attributer1 = xmlDoc.CreateAttribute("Integ");
                attributer1.Value = Integ;
                rootNode.Attributes.Append(attributer1);

                XmlAttribute attributer2 = xmlDoc.CreateAttribute("ProgId");
                attributer2.Value = Metodo;
                rootNode.Attributes.Append(attributer2);

                xmlDoc.AppendChild(rootNode);

                XmlNode userNode = xmlDoc.CreateElement("req");
                XmlAttribute attribute = xmlDoc.CreateAttribute("DelportalId");
                attribute.Value = Control.Common.GlobalParameters.Establecimiento;
                userNode.Attributes.Append(attribute);

                XmlAttribute attribute1 = xmlDoc.CreateAttribute("orderId");
                attribute1.Value = OrderId;
                userNode.Attributes.Append(attribute1);

                XmlAttribute attribute2 = xmlDoc.CreateAttribute("sInvoice");
                attribute2.Value = sInvoice;
                userNode.Attributes.Append(attribute2);

                XmlAttribute attribute3 = xmlDoc.CreateAttribute("Id");
                attribute3.Value = Id;
                userNode.Attributes.Append(attribute3);

                XmlAttribute attribute4 = xmlDoc.CreateAttribute("Name");
                attribute4.Value = Name;
                userNode.Attributes.Append(attribute4);

                XmlAttribute attribute5 = xmlDoc.CreateAttribute("Description");
                attribute5.Value = Description;
                userNode.Attributes.Append(attribute5);

                XmlAttribute attribute6 = xmlDoc.CreateAttribute("VersionOs");
                attribute6.Value = System.Environment.OSVersion.ToString();
                userNode.Attributes.Append(attribute6);

                XmlAttribute attribute7 = xmlDoc.CreateAttribute("VersionPos");
                attribute7.Value = versionPOS;
                userNode.Attributes.Append(attribute7);

                rootNode.AppendChild(userNode);

                xml = xmlDoc.InnerXml.ToString();
            }
            catch (Exception ex)
            {

            }
            return xml;
        }
        private void HabilitaBotonesSuperior(bool habilitar, bool saltar = false)
        {
            bool flag = false;
            try
            {
                if (!saltar)
                {
                    using (POSEntities db = new POSEntities())
                    {
                        if (db.core_parametro.Any(x => x.identificador == "BLOQUEOBOTONESFACTAPP" && x.parametro2 == this.FacturaActual.PedidoOtraApp.Tipo.ToString() && x.valor.ToUpper().Equals("TRUE")))
                        {
                            flag = true;
                        }

                    }
                }

                if (saltar)
                    flag = true;

                if (flag)
                {
                    //btnCreditoPavos.Enabled = habilitar;
                    //btn3Proveedor.Enabled = habilitar;
                    btnDescuentoEspecial.Enabled = habilitar;
                    btnGiftback.Enabled = habilitar;
                    btnFrmNC.Enabled = habilitar;
                    //btnRecarga.Enabled = habilitar;
                    btnCorresponsal.Enabled = habilitar;
                    btnParqueo.Enabled = habilitar;
                    btnWallet.Enabled = habilitar;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void prCambioCadenaConexion(string srvSelect, string srvPedidos)
        {
            try
            {
                string yourConnection = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"].ConnectionString.Replace(srvSelect, srvPedidos);

                var DBCS = System.Configuration.ConfigurationManager.ConnectionStrings["POSEntities"];
                var writable = typeof(System.Configuration.ConfigurationElement).GetField("_bReadOnly", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                writable.SetValue(DBCS, false);
                DBCS.ConnectionString = yourConnection;

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "prCambioCadenaConexion", "Se ha cambiado la cadena de conexion temporal por PedidoAPP, de :" + srvSelect + " a: " + srvPedidos);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Carga en el pos los items ingresados desde la App.
        /// </summary>
        private void CargaPedidoAppDelPortal()
        {
            bool retorno = false;
            decimal efectivoPagado = 0;
            int NumeroPedidoDelPortal = 0;
            try
            {
                HabilitaBotonesSuperior(false);
                NumeroPedidoDelPortal = string.IsNullOrEmpty(this.FacturaActual.PedidoOtraApp.Pedido) ? 0 : Convert.ToInt32(this.FacturaActual.PedidoOtraApp.Pedido);

                POSEntities db = new POSEntities();
                if (NumeroPedidoDelPortal > 0)
                {

                    List<core_facturadetalleAPP> listDetalleItems;
                    var queryCab = (from c in db.core_facturaAPP
                                    where c.orderApp == NumeroPedidoDelPortal
                                    select c
                                  ).FirstOrDefault();
                    if (queryCab != null)
                    {
                        core_facturaAPP cabFactApp;
                        cabFactApp = queryCab;//.FirstOrDefault();

                        var pos_cust = (from cust in db.pos_customer
                                        where cust.ACCOUNTNUM == cabFactApp.cliente_ax
                                        select cust).FirstOrDefault();

                        if (pos_cust != null)
                            this.cliente_actual = pos_cust;

                        this.setClienteData();

                        _factura.Subtotal = cabFactApp.subtotal;
                        _factura.Descuento = cabFactApp.descuento;
                        _factura.Descuento2 = cabFactApp.descuento2;
                        //_factura.Cambio = cabFactApp.descuentoPorc - cabFactApp.total;                        
                        _factura.Autorizacion = cabFactApp.autorizacion;
                        _factura.Base_imponible = cabFactApp.base0 + cabFactApp.base12;
                        _factura.Iva = cabFactApp.iva;
                        _factura.Total = cabFactApp.total;
                        // efectivoPagado = cabFactApp.descuentoPorc;
                        // _factura.Iva=0;                     

                    }
                    else
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                            POSEntities db1 = new POSEntities();
                            queryCab = (from c in db1.core_facturaAPP
                                        where c.orderApp == NumeroPedidoDelPortal
                                        select c
                                  ).FirstOrDefault();
                            if (queryCab != null)
                            {
                                core_facturaAPP cabFactApp;
                                cabFactApp = queryCab;//.FirstOrDefault();

                                var pos_cust = (from cust in db1.pos_customer
                                                where cust.ACCOUNTNUM == cabFactApp.cliente_ax
                                                select cust).FirstOrDefault();

                                if (pos_cust != null)
                                    this.cliente_actual = pos_cust;

                                this.setClienteData();

                                _factura.Subtotal = cabFactApp.subtotal;
                                _factura.Descuento = cabFactApp.descuento;
                                _factura.Descuento2 = cabFactApp.descuento2;
                                //_factura.Cambio = cabFactApp.descuentoPorc - cabFactApp.total;                        
                                _factura.Autorizacion = cabFactApp.autorizacion;
                                _factura.Base_imponible = cabFactApp.base0 + cabFactApp.base12;
                                _factura.Iva = cabFactApp.iva;
                                _factura.Total = cabFactApp.total;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "CargaPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                            }
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                    }
                    var query = (from c in db.core_facturaAPP
                                 join p in db.core_facturadetalleAPP on c.id equals p.factura_id
                                 join x in db.Tbl_PickOrderCab on c.id equals x.FacturaId
                                 join d in db.Tbl_PickOrderDet on x.Id equals d.PickOrderCabId
                                 where c.orderApp == NumeroPedidoDelPortal
                                  && p.item_id == d.ItemId
                                 && d.Estado == 5 && d.CantDespachado > 0
                                 select p
                                 );
                    if (query.Count() == 0)
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                            POSEntities db1 = new POSEntities();
                            query = (from c in db1.core_facturaAPP
                                     join p in db1.core_facturadetalleAPP on c.id equals p.factura_id
                                     join x in db1.Tbl_PickOrderCab on c.id equals x.FacturaId
                                     join d in db1.Tbl_PickOrderDet on x.Id equals d.PickOrderCabId
                                     where c.orderApp == NumeroPedidoDelPortal
                                      && p.item_id == d.ItemId
                                     && d.Estado == 5 && d.CantDespachado > 0
                                     select p
                                 );
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "CargaPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                    }

                    var queryServDomici = (from c in db.core_facturaAPP
                                           join p in db.core_facturadetalleAPP on c.id equals p.factura_id
                                           where c.orderApp == NumeroPedidoDelPortal
                                           && p.item_id.StartsWith("SERV")
                                           select p
                                          ).ToList();
                    if (queryServDomici.Count() == 0)
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                            POSEntities db1 = new POSEntities();
                            queryServDomici = (from c in db1.core_facturaAPP
                                               join p in db1.core_facturadetalleAPP on c.id equals p.factura_id
                                               where c.orderApp == NumeroPedidoDelPortal
                                               && p.item_id.StartsWith("SERV")
                                               select p
                                          ).ToList();
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "CargaPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);

                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                    }
                    _factura.Productos.Clear();
                    enlazarControles();

                    decimal descuento2 = 0;
                    if (_factura.Descuento2 > 0)
                    {
                        descuento2 = _factura.Descuento2 / _factura.Subtotal;
                    }
                    listDetalleItems = query.OrderBy(t => t.linea).ToList();
                    foreach (var itemApp in listDetalleItems)
                    {
                        //obtengo el codigo de barra del item, ya que en la APP no lo graba ni envia.
                        List<CodigoBarra> pos_itemb = db.pos_itembarra.Where(x => x.ITEMID == itemApp.item_id).Select(x => new CodigoBarra { ID = x.ITEMID, codigo = x.ITEMBARCODE }).ToList();

                        Producto itm = new Producto();
                        //itm.getProducto(item[0] /*item_id*/, _factura, cliente_actual);
                        itm.Id = itemApp.item_id;
                        //itm.CodigosBarra = pos_itemb;
                        itm.Nombre = itemApp.item_nombre;
                        itm.CantidadINEC = itemApp.cantidad;
                        itm.Cantidad = itemApp.cantidad;
                        itm.Unidades = Convert.ToInt32(itemApp.unidades);
                        itm.Unidad = itemApp.unidad;
                        itm.Pvp = itemApp.cantidad == 0 ? 0 : itemApp.subtotal / itemApp.cantidad;
                        itm.PrecioAx = itemApp.precio; itm.Costo = itemApp.costo;
                        //itm.Subtotal = itemApp.subtotal; 
                        itm.Descuento = itemApp.descuento;
                        itm.Iva = itemApp.iva;
                        itm.IvaProducto = itemApp.iva;
                        itm.EsExcluidoPromoIVA = false;
                        //itm.DescuentoAX = itm.Total; 
                        //itm.Total = itemApp.total;                         

                        _factura.Productos.Add(itm);
                    }

                    //agrega los items de Servicio que no los considera el picking.
                    foreach (var itemAppServ in queryServDomici)
                    {

                        Producto itm = new Producto();
                        itm.Id = itemAppServ.item_id;
                        itm.Nombre = itemAppServ.item_nombre;
                        itm.CantidadINEC = itemAppServ.cantidad;
                        itm.Cantidad = itemAppServ.cantidad;
                        itm.Unidades = Convert.ToInt32(itemAppServ.unidades);
                        itm.Unidad = itemAppServ.unidad;
                        itm.Pvp = itemAppServ.cantidad == 0 ? 0 : itemAppServ.subtotal / itemAppServ.cantidad;
                        itm.PrecioAx = itemAppServ.precio;
                        itm.Costo = itemAppServ.costo;
                        itm.Descuento = itemAppServ.descuento;
                        itm.Iva = itemAppServ.iva;
                        itm.IvaProducto = itemAppServ.iva;
                        itm.EsExcluidoPromoIVA = false;

                        _factura.Productos.Add(itm);
                    }

                    RefrescarGridItems();


                    ///Pagos:

                    var querypagos = (from c in db.core_facturaAPP
                                      join p in db.core_facturapagoAPP on c.id equals p.factura_id
                                      where c.orderApp == NumeroPedidoDelPortal
                                      select p
                                 );

                    _factura.Pagos.Clear();

                    if (querypagos.Count() == 0)
                    {
                        if (Control.Common.GlobalParameters.srvPrincipal == "TRUE")
                        {
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerSelect, Control.Common.GlobalParameters.ipServerPedAPP);
                            POSEntities db1 = new POSEntities();
                            querypagos = (from c in db1.core_facturaAPP
                                          join p in db1.core_facturapagoAPP on c.id equals p.factura_id
                                          where c.orderApp == NumeroPedidoDelPortal
                                          select p
                                     );
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "CargaPedidoAppDelPortal", "Se encontro informacion en : " + Control.Common.GlobalParameters.ipServerPedAPP);
                            prCambioCadenaConexion(Control.Common.GlobalParameters.ipServerPedAPP, Control.Common.GlobalParameters.ipServerSelect);
                        }
                    }
                    int pagosEfectivoAcumPtos = querypagos.Count();
                    //_factura.EsUsoAppMovil = false;
                    _factura.EsUsoAppMovil = true;

                    foreach (var pago in querypagos)
                    {
                        if (pago.tipo_id == "EFECTIVO")
                        {
                            /*
                            if (efectivoPagado == 0)
                            {
                                _factura.agregarPagoEfectivo(pago.valor);
                            }
                            else
                            {
                                _factura.agregarPagoEfectivo(efectivoPagado);
                            }
                            */
                            //permite acumular puntos para pagos en efectivo en su totalidad.
                            //eevv se quita vlidacion, ya que el sp de acumulacion ya realiza la validación.
                            //   if (pagosEfectivoAcumPtos == 1)
                            //     _factura.EsUsoAppMovil = true;                            
                        }

                        if (pago.tipo_id == "DINE ELECT")
                        {
                            _factura.AgregarPagoMonedero(pago.valor);
                        }

                        if (pago.tipo_id == "GIFT CARDV" || pago.tipo_id == "GIFT CARD")
                        {
                           
                            string nombreGrupo = string.Empty;
                            string msgError = string.Empty;
                            string identificacion = string.Empty;
                            bool puedeAgregarPago = true;
                            Decimal dValorGiftCard = 0;
                            
                            var t = new TarjetaRegalo();
                            //Sumar todas las giftcard matriculadas y presentar total acumulado
                            using (var db1 = new POSEntities())
                            {                               
                                string[] splitTarjetas = pago.datos.Split(',');
                                foreach (string tarjetas in splitTarjetas)
                                {
                                    string[] splitTarjetasValor = tarjetas.Split('-');
                                    
                                    string tCodigoTarjeta = splitTarjetasValor[0];
                                    string tValorTarjeta = splitTarjetasValor[1];
                                    var fp = db1.Tbl_DineroGiftCardApp.Where(x => x.IdCliente == this._factura.ClienteIdentificacion && x.Estado == 1 && x.Saldo > 0 && x.IdGiftCard == tCodigoTarjeta).OrderBy(y => y.Saldo).FirstOrDefault();
                                    //if (t.getTarjetaGiftCard(tCodigoTarjeta, fp.IdCliente) && Convert.ToDecimal(tValorTarjeta) > 0)
                                    if (t.getTarjetaGen(tCodigoTarjeta, fp.IdCliente, _factura.EsUsoAppMovil) && Convert.ToDecimal(tValorTarjeta) > 0)
                                    {
                                        _tarjetaRegalo = t;
                                        bool estaAsociadaGrupoCliente = _tarjetaRegalo.EstaAsociadaGrupoCliente(ref identificacion, ref msgError, ref nombreGrupo);
                                        dValorGiftCard = fp.Saldo;
                                        if (Convert.ToDecimal(tValorTarjeta) <= dValorGiftCard)
                                        {
                                            if (_tarjetaRegalo.getCodigoGiftCard().Trim().StartsWith("2222"))
                                            {
                                                _factura.AgregarPagoTarjetaRegalo(Convert.ToDecimal(tValorTarjeta), _tarjetaRegalo.getCodigoGiftCard(), _tarjetaRegalo.getSaldoGiftCard() - Convert.ToDecimal(tValorTarjeta), estaAsociadaGrupoCliente, identificacion, nombreGrupo, "GIFT CARD");
                                            }
                                            else
                                            {
                                                _factura.AgregarPagoTarjetaRegalo(Convert.ToDecimal(tValorTarjeta), _tarjetaRegalo.getCodigoGiftCard(), _tarjetaRegalo.getSaldoGiftCard() - Convert.ToDecimal(tValorTarjeta), estaAsociadaGrupoCliente, identificacion, nombreGrupo, "GIFT CARDV");
                                            }
                                            pago.valor = 0;
                                                
                                        }
                                        else
                                        {
                                            if (_tarjetaRegalo.getCodigoGiftCard().Trim().StartsWith("2222"))
                                            {
                                                _factura.AgregarPagoTarjetaRegalo(dValorGiftCard, _tarjetaRegalo.getCodigoGiftCard(), 0, estaAsociadaGrupoCliente, identificacion, nombreGrupo, "GIFT CARD");
                                            }
                                            else
                                            {
                                                _factura.AgregarPagoTarjetaRegalo(dValorGiftCard, _tarjetaRegalo.getCodigoGiftCard(), 0, estaAsociadaGrupoCliente, identificacion, nombreGrupo, "GIFT CARDV");
                                            }
                                                pago.valor = pago.valor - dValorGiftCard;
                                        }
                                       
                                    }
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaPedidoAppDelPortal", "Giftcard : " + tCodigoTarjeta + " Valor:" + tValorTarjeta);
                                }
                            }
                                //this.Close();                                
                        }                     
                        
                    }

                    txtPagoValor.Clear();
                    calcularTotalApp();
                    HabilitarControlesPedidosApp(false);

                    HabilitaControlesFormaPago("APPMOVIL_FORMAPAGO");

                }

            }
            catch (Exception ex)
            {
                throw;
            }


        }

        private void HabilitaControlesFormaPago(string parametroIdentifica)
        {

            string[] FormasPagoPermitidas;

            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var parametros = (from deta in db.core_parametro // .Any(x => x.identificador.Equals("APPMOVIL_FORMAPAGO") && x.valor.Equals("TRUE"));
                                      where deta.identificador == parametroIdentifica // "APPMOVIL_FORMAPAGO"
                                      && deta.valor == "TRUE"
                                      select deta).ToList();

                    FormasPagoPermitidas = null;
                    if (parametros.Count > 0)
                    {
                        string DetFormasPago = parametros.FirstOrDefault().parametro2;
                        FormasPagoPermitidas = DetFormasPago.Split(';');
                    }

                    if (FormasPagoPermitidas != null) {

                        btnEfectivo.Enabled = false;
                        btnTCredito.Enabled = false;
                        btnCheque.Enabled = false;
                        btnNC.Enabled = false;
                        btnDsctoEsp.Enabled = false;
                        btnPagoGiftCard.Enabled = false;
                        btnCreditoInterno.Enabled = false;
                        btnRetencion.Enabled = false;
                        btnDsctoPaviPlan.Enabled = false;

                        foreach (var deta in FormasPagoPermitidas)
                        {
                            switch (deta) {
                                case "E": //efectivo
                                    btnEfectivo.Enabled = true;
                                    break;
                                case "TC": //Tarjeta de Credito
                                    btnTCredito.Enabled = true;
                                    break;
                                case "CH": //Cheque
                                    btnCheque.Enabled = true;
                                    break;
                                case "NC"://NotaCredito
                                    btnNC.Enabled = true;
                                    break;
                                case "DE"://Descuento Especial
                                    btnDsctoEsp.Enabled = true;
                                    break;
                                case "GC"://GiftCard
                                    btnPagoGiftCard.Enabled = true;
                                    break;
                                case "RE"://Retención
                                    btnRetencion.Enabled = true;
                                    break;
                                case "TP"://Credito Interno
                                    btnCreditoInterno.Enabled = true;
                                    break;
                                case "DP"://PaviPlan
                                    btnDsctoPaviPlan.Enabled = true;
                                    break;
                            }
                        }

                        
                    }


                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "HabilitaControlesFormaPago", "Error al Habilitar controles por ventas desde la App. " + ex.Message);
            }



        }

        private void LimpiarcontrolesPedido()
        {
            _factura.Subtotal = 0;
            _factura.Descuento = 0;
            _factura.Descuento2 = 0;
            _factura.Autorizacion = string.Empty;
            _factura.Base_imponible = 0;
            _factura.Iva = 0;
            _factura.Total = 0;
            txtPagoValor.Clear();
            calcularTotalApp();
            _factura.Pagos.Clear();
            _factura.Productos.Clear();
            clearClienteData();
            _factura.EsUsoCuponPromocional = false;
        }

        private void calcularTotalApp()
        {
            try
            {
                decimal _valPagoMonedero = 0, _valRestante = 0, _valGiftCard =0;
                lblTotal.Text = string.Format("{0:C}", _factura.Total);
                lblTotal2.Text = lblTotal.Text;

                foreach (Pago p in _factura.Pagos)
                {
                    if (p.Descripcion.Equals("DINE ELECT"))
                    {
                        _valPagoMonedero += p.Valor;
                    }

                    if (p.Descripcion.Equals("GIFT CARDV"))
                    {
                        _valGiftCard += p.Valor;
                    }

                    if (p.Descripcion.Equals("GIFT CARD"))
                    {
                        _valGiftCard += p.Valor;
                    }
                }
                _valRestante = _factura.Total - _valPagoMonedero - _valGiftCard;
                lblRestante.Text = string.Format("{0:C}", _valRestante);
                var cambio = 0M;
                //_factura.validar_pagos(out cambio);
                lblCambio.Text = string.Format("{0:C}", _factura.Cambio);
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "calcularTotalApp", "Error al calcular. " + ex.Message);
            }
        }

        private void HabilitarControlesPedidosApp(bool bloquear)
        {
            try
            {
                txtCedula.Enabled = bloquear;
                btnCFinal.Enabled = bloquear;
                txtCodigo.Enabled = bloquear;
                btnCliente.Enabled = bloquear;
                btnQtyProduct.Enabled = bloquear;
                btnTCredito.Enabled = bloquear;
                btnCheque.Enabled = bloquear;
                btnNC.Enabled = bloquear;
                btnDsctoEsp.Enabled = bloquear;
                //btnPagoBorrar.Enabled = bloquear;
                btnPagoGiftCard.Enabled = bloquear;
                btnCreditoInterno.Enabled = bloquear;
                btnRetencion.Enabled = bloquear;
                btnDsctoPaviPlan.Enabled = bloquear;
                btnSearchPro.Enabled = bloquear;
                btnBorrarProducto.Enabled = bloquear;
                //btnEliminarPago.Enabled = bloquear;
                //btnEfectivo.Enabled = bloquear;
                //btnPagoAtras.Enabled = bloquear;
                btnBusqProd.Enabled = bloquear;
                BTN_DIREC_1.Enabled = bloquear;
                BTN_DIREC_2.Enabled = bloquear;
                BTN_DIREC_3.Enabled = bloquear;
                BTN_DIREC_4.Enabled = bloquear;
                BTN_DIREC_5.Enabled = bloquear;
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "HabilitarControlesPedidosApp", "Error al Al habilitr controles pedidos App. " + ex.Message);
            }
        }
        private void MainWindows_Load(object sender, EventArgs e)
        {
            // msgBoxCtrl = new MsgBoxCtrl();

            try
            {
                ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
                objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
                ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);

                CargarMainWindow();

                HabilitarMenuInicial();

                llamaMenuInicial();

                ////carga archivo con lista de productos 
                CreateFileProducts cfp = new CreateFileProducts(establecimiento_inicio);
                Thread thProductList = new Thread(new ThreadStart(cfp.CrearArchivoProductos));
                thProductList.Start(); //th.Join();  

                //btn3Proveedor.Text = POS.Control.Common.GlobalParameters.Labelbtn3Proveedor;
                btnRetencion.Visible = POS.Control.Common.GlobalParameters.VisibleFormaPagoRetencion;

                if (Control.Common.GlobalParameters.PosQuitaTopMost)
                {
                    this.TopMost = false;
                }
            }
            catch (Exception ex)
            {
                Control.Common.General.GetMensajeToList(583);

                //Control.Common.General.GetMensaje("POS", "El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS", "I");
                // MsgBoxCtrl.MessageBoxResult result = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS");
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS", "POS");

                // System.Windows.Forms.MessageBox.Show("El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se cerrará. Por favor abrir nuevamente POS");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MainWindow(Constructor)", "El aplicativo no pudo obtener todos los parámetros necesarios de seguridad por lo que se envió a cerrar el aplicativo, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Environment.Exit(1);
            }
            
        }


        //private long validaSecuencialMQ(string tipoDocumento)
        //{
        //    long secuencia = 0;
        //    ClsFacturaMQExcept factMQ;

        //    try
        //    {
        //        factMQ = ClsMessageQueue.getMessageQueue(tipoDocumento, POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision);
        //        if (factMQ != null)
        //        {

        //            if (factMQ.tipoException == TipoQueueExcepcion.NINGUNO)
        //            {
        //                //obtengo la secuencia del MSQUEUE del equipo.
        //                secuencia = Convert.ToInt64(factMQ.Secuencia);
        //            }
        //            if (factMQ.tipoException == TipoQueueExcepcion.MSQNOINSTALADO)
        //            {
        //                EnvioMailError("Caja no tiene instalado MS MESSAGEQUEUE, contactese con el departamento de Sistemas para la instalación.",
        //                            "El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
        //                            "Caja no tiene instalado MS MESSAGEQUEUE. "+ Environment.NewLine +factMQ._mqExc.Message,
        //                            "validaSecuencialMQ"
        //                            );

        //            }
        //            if (factMQ.tipoException == TipoQueueExcepcion.QUEUENOENCONTRADO)
        //            {
        //                EnvioMailError("Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
        //                            "El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
        //                            "El nombre de la cola '" + factMQ .QueueName+ "' no fue encontrado. " + Environment.NewLine  + factMQ._mqExc.Message,
        //                            "validaSecuencialMQ"
        //                            );

        //            }
        //            if (factMQ.tipoException == TipoQueueExcepcion.TIMEOUT)
        //            {
        //                EnvioMailError("Caja no pudo obtener la Cola de Mensaje(MS MESSAGEQUEUE), contactese con el departamento de Sistemas.",
        //                            "El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
        //                            "expiró el tiempo de respuesta para obtener la cola de mensaje '" + factMQ.QueueName + "'. " + Environment.NewLine + factMQ._mqExc.Message,
        //                            "validaSecuencialMQ"
        //                            );

        //            }

        //            if (factMQ.tipoException == TipoQueueExcepcion.NODISPONIBLE)
        //            {
        //                EnvioMailError("Caja no pudo obtener la Cola de Mensaje(MS MESSAGEQUEUE), contactese con el departamento de Sistemas.",
        //                            "El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
        //                            "La cola de mensaje '" + factMQ.QueueName + "' no se encuentra disponible. " + Environment.NewLine  +factMQ._mqExc.Message,
        //                            "validaSecuencialMQ"
        //                            );

        //            }

        //        }
        //        else
        //        {
        //            EnvioMailError( "Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
        //                            "El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
        //                            "no se enconstró registro en el MessageQueue",
        //                            "validaSecuencialMQ"
        //                            );

        //        }

        //    }
        //    catch (MessageQueueException exMQ)
        //    {

        //        EnvioMailError("Caja no tiene configurado MS MESSAGEQUEUE, contactese con el departamento de Sistemas.",
        //                        "El POS no pudo obtener la numeración de la factura del Message Queue del equipo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
        //                        Control.Common.ExceptionHandler.GetExceptionMessages(exMQ),
        //                        "validaSecuencialMQ"
        //                      );
        //    }               

        //    return secuencia;
        //}



        /// <summary>
        /// Obtiene el número de secuencia entre lo que está facturado, tabla documento secuencia y lo de la cola de mensaje del POS, el mayor numero de secuencia entre ellos es el numero de secuencia que va a utilizar la factura.
        /// </summary>
        /// <returns></returns>
        public static long GetSecuenciaValidada()
        {
            long secuencia = 0;
            long secuenciaMSQUEUELocal = 0;
            
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var documento_secuencia = db.core_documentosecuencia.FirstOrDefault(x => x.core_puntoemision.establecimiento_id == POS.Control.Common.GlobalParameters.Establecimiento
                                                                        && x.core_puntoemision.punto_emision == POS.Control.Common.GlobalParameters.PuntoEmision
                                                                        && x.core_documento.codigo == "F");

                    //Tomar el max secuencial utilizado para verificar que numero de secuencia este correcto
                    int maxNroFactura = db.core_factura.Where(x => x.establecimiento == POS.Control.Common.GlobalParameters.Establecimiento
                                                                     && x.punto_emision == POS.Control.Common.GlobalParameters.PuntoEmision)
                                                       .Max(x => (int?)x.numero) ?? 0;



                    //evelasco  obtengo la secuencia del MEssageQueue del SO del equipo POS.
                    secuenciaMSQUEUELocal = POS.Control.POS.validaSecuencialMQ("F");
                    if (secuenciaMSQUEUELocal == 0)
                    {

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "No se encontró el valor del secuencial del MSQUEUE." + Environment.NewLine);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Graba secuencial de la tabla documento_secuencia en la cola de mensajes MSQUEUE del Equipo Local.");

                        secuencia = documento_secuencia.siguiente;
                        if ((maxNroFactura + 1) > documento_secuencia.siguiente)
                        {
                            secuencia = (maxNroFactura + 1);
                        }

                        //ClsMessageQueue.setMessageQueue(POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision, secuencia.ToString(), "F");

                    }

                    if ((secuenciaMSQUEUELocal) > documento_secuencia.siguiente)
                    {
                        secuencia = secuenciaMSQUEUELocal;
                    }
                    else if ((maxNroFactura + 1) > documento_secuencia.siguiente)  //Corregir el secuencial de factura de ser necesario
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Se detectó secuencial de facturación incorrecto. Se intentará realizar el ajuste de secuencial" + Environment.NewLine +
                                                                            "Secuencial incorrecto: " + documento_secuencia.siguiente.ToString() + Environment.NewLine +
                                                                            "Secuencial que se va dar: " + (maxNroFactura + 1).ToString() + Environment.NewLine +
                                                                            "DireccionIp caja: " + Control.Common.GlobalParameters.IpMaquina + Environment.NewLine +
                                                                            "DireccionIp servidor: " + Control.Common.GlobalParameters.SelectedServerIp + Environment.NewLine +
                                                                            "Usuario Id: " + Control.Common.GlobalParameters.Usuario +
                                                                            "Usuario Nombre: " + Control.Common.GlobalParameters.UsuarioNombre
                                                                            );

                        secuencia = maxNroFactura + 1;

                    }
                    else
                    {
                        secuencia = documento_secuencia.siguiente;
                    }


                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GetSecuenciaValidada", "Error: "+ ex.Message);

                throw;
            }

            return secuencia;
        }
        private void VerificarSecuencialFactura()
        {
            long secuenciaMSQUEUELocal = 0;
            long secuencia1 = 0;
            //msgBoxCtrl = new MsgBoxCtrl();

            try
            {
                bool esIpCajaValida = POS.Control.Common.Network.ValidateIPv4(POS.Control.Common.GlobalParameters.IpMaquina);
                if (!esIpCajaValida)
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Variable GlobalParameters.IpMaquina no es una direccion Ipv4 (valor: '" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.IpMaquina) ? "" : Control.Common.GlobalParameters.IpMaquina) + "')");

                bool esIpServerValida = POS.Control.Common.Network.ValidateIPv4(POS.Control.Common.GlobalParameters.SelectedServerIp);
                if (!esIpServerValida)
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Variable GlobalParameters.SelectedServerIp no es una direccion Ipv4 (valor: '" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.SelectedServerIp) ? "" : Control.Common.GlobalParameters.SelectedServerIp) + "')");

                bool estaPOSApuntandoServerLocal = false;
                if (esIpCajaValida && esIpServerValida)
                {
                    if (POS.Control.Common.GlobalParameters.IpMaquina.Substring(0, 11) == POS.Control.Common.GlobalParameters.SelectedServerIp.Substring(0, 11))
                        estaPOSApuntandoServerLocal = true;
                }

                using (POSEntities db = new POSEntities())
                {
                    //var documento_secuencia = db.core_documentosecuencia.Single(x => x.core_puntoemision.establecimiento_id == POS.Control.Common.GlobalParameters.Establecimiento
                    //                                                    && x.core_puntoemision.punto_emision == POS.Control.Common.GlobalParameters.PuntoEmision
                    //                                                    && x.core_documento.codigo == "F");


                    long siguiente = 0;                                      
                    var listSecDocumento = (from deta in db.core_documentosecuencia
                                               where deta.core_puntoemision.establecimiento_id == POS.Control.Common.GlobalParameters.Establecimiento
                                               && deta.core_puntoemision.punto_emision == POS.Control.Common.GlobalParameters.PuntoEmision
                                               && deta.core_documento.codigo == "F"
                                               select deta).ToList();

                    if (listSecDocumento.Count > 1)
                    {
                        var documento_secuencia = (from deta in listSecDocumento
                                                   where deta.core_puntoemision.ip_address == Control.Common.GlobalParameters.IpMaquina
                                                   select deta).ToList().FirstOrDefault();

                        siguiente = documento_secuencia.siguiente;

                    }
                    else if(listSecDocumento.Count ==1) {
                        var documento_secuencia = (from deta in listSecDocumento
                                                   where deta.core_puntoemision.ip_address == Control.Common.GlobalParameters.IpMaquina
                                                   select deta).ToList().FirstOrDefault();

                        siguiente = documento_secuencia.siguiente;
                    }


                    //Tomar el max secuencial utilizado para verificar que numero de secuencia este correcto
                    int maxNroFactura = db.core_factura.Where(x => x.establecimiento == POS.Control.Common.GlobalParameters.Establecimiento
                                                                    && x.punto_emision == POS.Control.Common.GlobalParameters.PuntoEmision)
                                                       .Max(x => (int?)x.numero) ?? 0;

                    //evelasco  obtengo la secuencia del MEssageQueue del SO del equipo POS.
                    secuenciaMSQUEUELocal = POS.Control.POS.validaSecuencialMQ("F");
                    if (secuenciaMSQUEUELocal == 0)
                    {

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "No se encontró el valor del secuencial del MSQUEUE." + Environment.NewLine);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Graba secuencial de la tabla documento_secuencia en la cola de mensajes MSQUEUE del Equipo Local.");

                        secuencia1 = siguiente;
                        //secuencia1 = documento_secuencia.siguiente;
                        //if ((maxNroFactura + 1) > documento_secuencia.siguiente)
                        if ((maxNroFactura + 1) > siguiente)
                        {
                            secuencia1 = (maxNroFactura + 1);
                        }

                        ClsMessageQueue.setMessageQueue(POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision, secuencia1.ToString(), "F");
                        secuencia1 = 0;
                    }
                    //(secuenciaMSQUEUELocal+1) > (maxNroFactura + 1) || 
                    //if ((secuenciaMSQUEUELocal) > documento_secuencia.siguiente)
                    if ((secuenciaMSQUEUELocal) > siguiente)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "La secuencia de la cola de mensaje(Local) es mayor a la secuencia de la tabla documento_secuencia. Se actualiza tabla documento_secuencia con el valor de la cola de mensaje." + Environment.NewLine +
                                                                            //"Secuencial incorrecto: " + documento_secuencia.siguiente.ToString() + Environment.NewLine +
                                                                            "Secuencial incorrecto: " + siguiente.ToString() + Environment.NewLine +
                                                                            "Secuencial que se va dar: " + secuenciaMSQUEUELocal + Environment.NewLine +
                                                                            "DireccionIp caja: " + Control.Common.GlobalParameters.IpMaquina + Environment.NewLine +
                                                                            "DireccionIp servidor: " + Control.Common.GlobalParameters.SelectedServerIp + Environment.NewLine +
                                                                            "Usuario Id: " + Control.Common.GlobalParameters.Usuario +
                                                                            "Usuario Nombre: " + Control.Common.GlobalParameters.UsuarioNombre);

                        siguiente = secuenciaMSQUEUELocal;
                        //documento_secuencia.siguiente = secuenciaMSQUEUELocal;
                        db.SaveChanges();

                        clearClienteData();
                        _factura = null;
                        _factura = new Factura();
                        if (POS.Control.POS.init(ref _factura))
                        {
                            _factura.User = this._current_user;
                            setTituloDocumento();
                            enlazarControles();
                            calcularFactura();
                            this.splitControl.Panel2Collapsed = true;
                        }
                    }
                    // else if ((maxNroFactura + 1) != documento_secuencia.siguiente)  //Corregir el secuencial de factura de ser necesario
                    else if ((maxNroFactura + 1) > siguiente)  //Corregir el secuencial de factura de ser necesario
                    //else if ((maxNroFactura + 1) > documento_secuencia.siguiente)  //Corregir el secuencial de factura de ser necesario
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Se detectó secuencial de facturación incorrecto. Se intentará realizar el ajuste de secuencial" + Environment.NewLine +
                                                                            "Secuencial incorrecto: " + siguiente.ToString() + Environment.NewLine +
                                                                            //"Secuencial incorrecto: " + documento_secuencia.siguiente.ToString() + Environment.NewLine +
                                                                            "Secuencial que se va dar: " + (maxNroFactura + 1).ToString() + Environment.NewLine +
                                                                            "DireccionIp caja: " + Control.Common.GlobalParameters.IpMaquina + Environment.NewLine +
                                                                            "DireccionIp servidor: " + Control.Common.GlobalParameters.SelectedServerIp + Environment.NewLine +
                                                                            "Usuario Id: " + Control.Common.GlobalParameters.Usuario +
                                                                            "Usuario Nombre: " + Control.Common.GlobalParameters.UsuarioNombre
                                                                            );

                        //if (!estaPOSApuntandoServerLocal)
                        //{
                        //    if (MessageBox.Show("Se detectó que el secuencial siguiente de facturación no es correcto pero ud y su caja están apuntando a un servidor externo al local por lo que no se realizó automáticamente el ajuste ya que podría tratarse de una emergencia. " +
                        //                                    Environment.NewLine +
                        //                                    Environment.NewLine +
                        //                                    "Si ud no desea realizar el ajuste o no está seguro de continuar pulse en NO (En caso de ud no ser personal técnico contacte al administrador y no realice facturación en esta caja)." +
                        //                                    Environment.NewLine +
                        //                                    Environment.NewLine +
                        //                                    "Si desea corregir el secuencial basado en la información del servidor al que está apuntando pulse Sí para continuar"
                        //                         , "POS", MessageBoxButtons.YesNo) == DialogResult.No)
                        //    {
                        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Se detectó secuencial incorrecto pero POS estaba apuntando a otro servidor. El usuario " + Control.Common.GlobalParameters.Usuario + ":" + Control.Common.GlobalParameters.UsuarioNombre + " pidió no realizar el ajuste de secuencial");
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "El usuario " + Control.Common.GlobalParameters.Usuario + ":" + Control.Common.GlobalParameters.UsuarioNombre + " aceptó la responsabilidad de realizar el ajuste de secuencial estando apuntando a otro servidor");
                        //    }
                        //}

                        siguiente = maxNroFactura + 1;
                        //documento_secuencia.siguiente = maxNroFactura + 1;
                        db.SaveChanges();

                        //crea la cola de mensaje para solocar el nuevo valor del secuencial
                        try
                        {
                            ClsMessageQueue.receiveMessageQueue("F", POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision);
                        }
                        catch (Exception ex1)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "No se pudo leer los mensajes de la cola pendiente para la secuencia de la Factura." + Environment.NewLine + Control.Common.ExceptionHandler.GetExceptionMessages(ex1));
                        }

                        try
                        {
                            ClsMessageQueue.setMessageQueue(POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision, siguiente.ToString(), "F");
                            // ClsMessageQueue.setMessageQueue(POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision, documento_secuencia.siguiente.ToString(), "F");
                        }
                        catch (Exception ex1)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "VerificarSecuencialFactura", "Se detectó secuencial de facturación incorrecto. Se intentará realizar el ajuste de secuencial" + Environment.NewLine + Control.Common.ExceptionHandler.GetExceptionMessages(ex1));
                        }


                        /*clearClienteData();
                        _factura = null;
                        _factura = new Factura();
                        _factura.User = this._current_user;
                        if (POS.Control.POS.init(ref _factura))
                        {
                         //   _factura.User = this._current_user;
                            setTituloDocumento();
                            enlazarControles();
                            calcularFactura();
                            this.splitControl.Panel2Collapsed = true;
                        }*/
                    }
                    else
                    {
                        //crea la cola de mensaje para solocar el nuevo valor del secuencial
                        try
                        {
                            ClsMessageQueue.receiveMessageQueue("F", POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision);
                            ClsMessageQueue.setMessageQueue(POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision, siguiente.ToString(), "F");
                            //ClsMessageQueue.setMessageQueue(POS.Control.Common.GlobalParameters.Establecimiento, POS.Control.Common.GlobalParameters.PuntoEmision, documento_secuencia.siguiente.ToString(), "F");
                        }
                        catch (Exception ex2)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "No se pudo grabar en la cola de mensaje. A continuación excepciones encontradas:" + Control.Common.ExceptionHandler.GetExceptionMessages(ex2));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSecuencialFactura", "El aplicativo no pudo completar la verificacion del secuencial de factura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

                Control.Common.General.GetMensajeToList(143);

                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Se detectó que el secuencial de factura no estaba correcto pero mientras se lo corregía ocurrió un inconveniente, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", "POS");
                // System.Windows.Forms.MessageBox.Show("Se detectó que el secuencial de factura no estaba correcto pero mientras se lo corregía ocurrió un inconveniente, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        private void SetGlobalParameters(Factura _factura)
        {
            //Setear parametros globales
            POS.Control.Common.GlobalParameters.Establecimiento = _factura.Establecimiento;
            POS.Control.Common.GlobalParameters.EstablecimientoNombre = _factura.Establecimiento_nombre;
            POS.Control.Common.GlobalParameters.EstablecimientoDireccion = _factura.Establecimiento_direccion;
            POS.Control.Common.GlobalParameters.EstablecimientoTelefono = _factura.Establecimiento_telefono;
            POS.Control.Common.GlobalParameters.PuntoEmision = _factura.PtoEmision;
            POS.Control.Common.GlobalParameters.IpMaquina = _factura.Ip_address;
            POS.Control.Common.GlobalParameters.Usuario = this._current_user.username;
            POS.Control.Common.GlobalParameters.UsuarioNombre = this._current_user.nombres;
            POS.Control.Common.GlobalParameters.UserObj = this._current_user;
            POS.Control.Common.GlobalParameters.DataForFingerprint = Data;

            POS.Control.Common.GlobalParameters.IPPinPad = _factura.IpPinPad;
            POS.Control.Common.GlobalParameters.PuertoPinPad = _factura.PuertoPinPad;
            POS.Control.Common.GlobalParameters.EstTcpIpPinpad = _factura.EstTcpIpPinpad;


            //Cargar la ip del servidor sql al cual POS se encuentra conectado
            //Nota: La propiedad SelectedServerIp fue agregada con la intencion inicial de auditar las cajas que no estan apuntando a la ip que 
            //  le corresponde, comparando los segmentos de red del ip de caja que se guarda en todas las facturas y la ip del servidor sql 
            //  usando un Job sql
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //El query enviado devuelve la ip del servidor sql, esto permitira hacer comparacion entre segementos de red
                    POS.Control.Common.GlobalParameters.SelectedServerIp = db.Database.SqlQuery<string>("select LOCAL_NET_ADDRESS from SYS.DM_EXEC_CONNECTIONS where SESSION_ID = @@SPID").FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                POS.Control.Common.GlobalParameters.SelectedServerIp = string.Empty;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "SetGlobalParameters", "No se pudo obtener la ip del servidor sql al que se esta conectado, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

                try
                {
                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "Caja no pudo obtener ip del servidor sql",
                        String.Format("El POS del siguiente punto de emision no pudo obtener ip del servidor sql, esto no afecta a su facturacion y normal funcionamiento, pero impedira al staff de sistemas saber si la caja esta apuntando a un servidor correcto. Esto pudo deberse a un breve inconveniente o por permisos del usuario sql y se recomienda una vez verificada la novedad ponerse en contacto inmediatamente y reiniciar el aplicativo  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    POS.Control.Common.GlobalParameters.Establecimiento,
                                    POS.Control.Common.GlobalParameters.PuntoEmision,
                                    POS.Control.Common.GlobalParameters.IpMaquina,
                                    POS.Control.Common.GlobalParameters.UsuarioNombre,
                                    POS.Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex)),
                        false,
                        String.Empty);

                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "SetGlobalParameters", "No se pudo enviar notificacion del problema de caja para obtener ip del servidor sql, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                    }
                }
                catch (Exception exMail)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "SetGlobalParameters", "Excepcion grave al llamar a la clase de envio de email, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exMail));
                }
            }

            RecargarParametrosPOS();
        }

        /// <summary>
        /// Se agrega promocion en memoria para evitar se demore al gudardar los registros.
        /// </summary>
        /// <param name="_establecimiento"></param>
        /// <returns></returns>
       public  List<Promocion> getPromociones(string _establecimiento)
        {
            List<Promocion> list = new List<Promocion>();
            using (var db = new POSEntities())
            {
                
                try
                {
                    foreach (var vw_cabecera in (new Promocion().getCabeceraPorDiaHoy(db, _establecimiento)))
                    {
                        Promocion promo = new Promocion();
                        promo.Establecimiento = vw_cabecera.ALMACEN;
                        promo.Tipo = vw_cabecera.TIPODESCUENTO;
                        promo.Descripcion = vw_cabecera.DESCRIPCION;
                        promo.FechaDesde = (DateTime)vw_cabecera.FECHADESDE;
                        promo.FechaHasta = (DateTime)vw_cabecera.FECHAHASTA;
                        promo.Estado = vw_cabecera.ESTADO;
                        promo.RecId = vw_cabecera.RECID;
                        promo.ListProductos = new List<Producto>();

                        var param = db.core_parametro.Where(x => x.identificador == "PROMOAX_MAXCANTDSCTO"
                                                                    && x.valor == promo.RecId.ToString()).FirstOrDefault();
                        if (param != null)
                        {
                            decimal maxCantDscto;
                            if (decimal.TryParse(param.parametro2, out maxCantDscto))
                            {
                                //La cantidad maxima debe ser configurada con un valor superior a cero
                                if (maxCantDscto > 0) promo.MaxCantidadDscto = maxCantDscto;
                            }

                            promo.EsRestrictiva = (param.documento == "1");
                        }


                        var param2 = db.core_parametro.Where(x => x.identificador == "PROMOAX_MAXCANTDSCTOG"
                                                                    &&
                                                                    x.valor == promo.RecId.ToString()).FirstOrDefault();
                        if (param2 != null)
                        {
                            decimal maxCantDscto;
                            if (decimal.TryParse(param2.parametro2, out maxCantDscto))
                            {
                                promo.GeneralPromo = true;
                                //La cantidad maxima debe ser configurada con un valor superior a cero
                                if (maxCantDscto > 0) promo.MaxCantidadDscto = maxCantDscto;
                            }

                            promo.EsRestrictiva = (param2.documento == "1");
                        }

                        list.Add(promo);
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.General.GetMensajeToList(144);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No fue posible obtener la lista de promociones para estas fechas. Contacte al administrador", "POS");
                    //System.Windows.Forms.MessageBox.Show("No fue posible obtener la lista de promociones para estas fechas. Contacte al administrador");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "Factura", "getPromociones", "No fue posible obtener la lista de promociones para estas fechas, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                }
            }

            return list;
        }


        private dynamic GetBotonDirec(int NumBoton, string EstablecimientoAxCode, POSEntities  posEF )
        {

            string sQuery = string.Empty;
            //SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
            //string connectionString = posEF.Database.Connection.ConnectionString;
            string connectionString = POS.Properties.Settings.Default.CONECTA_AX;
            string parametro2 = string.Empty;
            string valor = string.Empty;


            try
            {

                sQuery = string.Concat(sQuery, "Select * from POS.dbo.core_parametro ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "where 1=1 ", Environment.NewLine);
                sQuery = string.Concat(sQuery, "and identificador like 'BOTON_DIREC%' ", Environment.NewLine);

                string BotonDireccion = string.Empty;

                if (NumBoton > 0)
                {
                    BotonDireccion = string.Concat(BotonDireccion, "BOTON_DIREC_", NumBoton.ToString());
                }

                if (!string.IsNullOrEmpty(EstablecimientoAxCode))
                {
                    BotonDireccion = string.Concat(BotonDireccion, "_", EstablecimientoAxCode);
                }
                sQuery = string.Concat(sQuery, "and identificador = '", BotonDireccion, "'", Environment.NewLine);
                //var objResult = posEF.Database.ExecuteSqlCommandAsync(sQuery, null);
                DataSet dtsConsulta = new DataSet();
                dtsConsulta = Control.Common.General.GetDataSet(sQuery, connectionString);

                if (dtsConsulta.Tables.Count > 0) {
                    if (dtsConsulta.Tables[0].Rows.Count > 0) {
                        foreach (DataRow data in dtsConsulta.Tables[0].Rows) {
                            parametro2 = data["parametro2"].ToString();
                            valor = data["valor"].ToString();
                        }
                    }
                }


                var Result = new
                {
                    CodError = 0,
                    MsjError = "",
                    parametro2 = parametro2,
                    valor = valor
                };

                return Result;
            }
            catch (Exception ex)
            {
                var Result = new
                {
                    CodError = -1,
                    MsjError = "Error: " + ex.Message,
                    parametro2 = parametro2,
                    valor = valor
                };

                return Result;
            }
        }


        private void CargarMainWindow()
        {
            //msgBoxCtrl = new MsgBoxCtrl();
            this.splitControl.Panel2Collapsed = true;
            _factura = new Factura();


            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Método CargarMainWindow acaba de ser accesado, el objeto factura sera reinicializado");

            listPromociones =  this.getPromociones(_factura.Establecimiento).ToList();
           // _factura.PromocionesActuales = listPromociones;
            _factura.User = this._current_user;
            
            var initPOS = POS.Control.POS.init(ref _factura);
            

            if (initPOS)
            {
                //_factura.User = this._current_user;
                if (_factura.User.isSuperUser)
                    btnLocal.Enabled = true;
                else
                    btnLocal.Enabled = false;

                establecimiento_inicio = _factura.Establecimiento;
                _factura.configurarPromociones();

                setTituloDocumento();
                enlazarControles();
                ((FillPrimitive)btnFactura.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]).BackColor = Control.Common.GlobalParameters.Color_ButtonPulsed;

                using (var db = new POSEntities())
                {
                    if (db.core_parametro.Where(x => x.identificador == "LOCAL_TAR_DESC" && x.valor != "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                    {
                        btnDescuentoEspecial.Visible = false; //Descuento Hiden
                    }

                    string EstablecimientoAxCode = string.Empty;
                    if (_factura.User.username != "1234") {
                        EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode;
                    }


                    var objBoton1 = GetBotonDirec(1, EstablecimientoAxCode, db);
                    var objBoton2 = GetBotonDirec(2, EstablecimientoAxCode, db);
                    var objBoton3 = GetBotonDirec(3, EstablecimientoAxCode, db);
                    var objBoton4 = GetBotonDirec(4, EstablecimientoAxCode, db);
                    var objBoton5 = GetBotonDirec(5, EstablecimientoAxCode, db);


                    BotonDirec1 = objBoton1.parametro2;
                    BTN_DIREC_1.Text = objBoton1.valor;
                    BotonDirec2 = objBoton2.parametro2;
                    BTN_DIREC_2.Text = objBoton2.valor;
                    BotonDirec3 = objBoton3.parametro2;
                    BTN_DIREC_3.Text = objBoton3.valor;
                    BotonDirec4 = objBoton4.parametro2;
                    BTN_DIREC_4.Text = objBoton4.valor;
                    BotonDirec5 = objBoton5.parametro2;
                    BTN_DIREC_5.Text = objBoton5.valor;

                  
                    //Ejecución no Graba Base Temporal
                    if (db.core_parametro.Where(x => x.identificador == "NO_GRABA_TMP_DB" && x.valor == _factura.Establecimiento && x.parametro2 == _factura.PtoEmision).FirstOrDefault() != null)
                    { NoGrabaTMP = "TRUE"; }

                    //Si es modo Consulta 2X
                    cmbLocalConsulta.Visible = false;
                    //btn3Proveedor.Visible = false;
                    //btnMenuInicial.Visible = false;
                    
                    this.btnCuponApp.Name = "btnCuponApp";

                    lbl_Local2x.Visible = false;
                    var es2x = false;
                    if (es2x = db.core_parametro.Any(x => x.identificador == "2X_CONSULTA_POS" && x.parametro2 == _factura.Ip_address && x.valor == "TRUE"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Caja acaba de ser identificada como POS DE CONSULTAS 2X. Combo 'Cambio Local' será habilitado");

                        btnEliminarPago.Visible = false;
                        btnGrabar.Visible = false;
                        btnFrmNC.Visible = false;
                        btnPagar.Visible = false;
                        btnGiftback.Visible = false;
                        //btnCreditoPavos.Visible = false;
                        cmbLocalConsulta.Visible = true;
                        lbl_Local2x.Visible = true;
                        this.Text += "-POS DE CONSULTAS 2X";
                        Es2X_CONSULTA_POS = true;
                        BTN_DIREC_3.Text = "CERRAR POS";
                    }

                    var parametroAcumulaPromoAx = db.core_parametro.Where(x => x.identificador == "ACUMULA_PROMO_AX" && x.parametro2 == establecimiento_inicio).FirstOrDefault();
                    if (parametroAcumulaPromoAx != null)
                    {
                        POS.Control.Common.GlobalParameters.acumulaPromoAx = parametroAcumulaPromoAx.valor == "TRUE";
                    }

                    
                    Control.Common.GlobalParameters.AutorizadorDefault = 2;
                    var AutorizaDefault = (from deta in db.core_parametro
                                           where deta.identificador == "AUTORIZAODR_PINPAD_DEFAULT"
                                           select deta).ToList();

                    if (AutorizaDefault.Count > 0) {
                        
                        Control.Common.GlobalParameters.AutorizadorDefault =  Int32.Parse( AutorizaDefault.FirstOrDefault().valor );
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Se asigna Autorizador por defecto, parametro: AUTORIZAODR_PINPAD_DEFAULT: Autorizador: " + Control.Common.GlobalParameters.AutorizadorDefault.ToString());
                    }

                    Control.Common.GlobalParameters.TituloApp = POS.Properties.Resources.TituloApp;
                    
                }

                if (_factura.UsaBalanza)
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(_factura.ModeloBalanza))
                        {
                            string[] parametrosbalanza = _factura.ModeloBalanza.Split('|');
                            if (parametrosbalanza[0] == "1")
                            {
                                scanner = new TomaPeso(_factura.PuertoBalanza, TomaPeso.BalanzaMarcas.DATALOGIC, true, false, false);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Se configura con Balanza Datalogic segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen + " - " + _factura.ModeloBalanza);
                            }
                            else
                            {
                                scanner = new TomaPeso(_factura.PuertoBalanza, TomaPeso.BalanzaMarcas.CAS, true, false, false);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Se configura con Balanza honeywell segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen + " - " + _factura.ModeloBalanza);
                            }
                        }
                        else
                        {
                            if ((_factura.Establecimiento == "029" && _factura.PtoEmisionOrigen != "008" && _factura.PtoEmisionOrigen != "004")
                                        || (_factura.Establecimiento == "024" && (_factura.PtoEmisionOrigen != "003" && _factura.PtoEmisionOrigen != "005")) //&& _factura.PtoEmisionOrigen != "006")
                                        || _factura.Establecimiento == "035" || _factura.Establecimiento == "037" || _factura.Establecimiento == "040"
                                        || (_factura.Establecimiento == "007" && _factura.PtoEmisionOrigen == "007")
                                        || (_factura.Establecimiento == "011" && _factura.PtoEmisionOrigen == "008"))
                            {
                                scanner = new TomaPeso(_factura.PuertoBalanza, TomaPeso.BalanzaMarcas.DATALOGIC, true, false, false);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Se configura con Balanza Datalogic segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                            }
                            else
                            {
                                scanner = new TomaPeso(_factura.PuertoBalanza, TomaPeso.BalanzaMarcas.CAS, true, false, false);
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Se configura con Balanza honeywell segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                                //MessageBox.Show(this,"New Toma peso");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargarMainWindow", "Sección Validacion if(_factura.UsaBalanza) - No se pudo setear configuraciones de balanza, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    }
                }

                if (_factura.UsaBalanza && _factura.UsarScannerIntegrado)
                {
                    scanner.ControlToShowText = this.txtCodigo;
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Carga Balanza Scanner", "Sección Validacion _factura.UsaBalanza && _factura.UsarScannerIntegrado");
                    try
                    {
                        if (!String.IsNullOrEmpty(_factura.ModeloBalanza))
                        {
                            string[] parametrosbalanza = _factura.ModeloBalanza.Split('|');
                            if (parametrosbalanza[1] != "")
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Carga Balanza Scanner", "Sección Validacion primer if, configura: " + parametrosbalanza[1].ToString() + " - " + _factura.ModeloBalanza);
                                scannerDL = new OposScanner_CCO.OPOSScanner();
                                scannerDL.Open(parametrosbalanza[1].ToString());
                                scannerDL.ClaimDevice(Convert.ToInt32(parametrosbalanza[5].ToString()));
                                scannerDL.DeviceEnabled = true;
                                scannerDL.DataEventEnabled = true;
                                scannerDL.DecodeData = true;
                                scannerDL.AutoDisable = false;
                                scannerDL.DataEvent += new OposScanner_CCO._IOPOSScannerEvents_DataEventEventHandler(scanner_DataLogic);
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Carga Balanza Scanner", "Sección Validacion primer else if, configura NULL" + " - " + _factura.ModeloBalanza);
                                scanner.ScannerDataReceived += scanner_ScannerDataReceived;
                                scanner.Open();
                                //MessageBox.Show(this,"Scanner Open");
                            }
                        }
                        else
                        {
                            //if (_factura.Establecimiento == "024" && (_factura.PtoEmisionOrigen != "003" && _factura.PtoEmisionOrigen != "006" && _factura.PtoEmisionOrigen != "004" && _factura.PtoEmisionOrigen != "007")) 
                            if ((_factura.Establecimiento == "024" && (_factura.PtoEmisionOrigen != "003" && _factura.PtoEmisionOrigen != "005" && _factura.PtoEmisionOrigen != "004" && _factura.PtoEmisionOrigen != "007"))
                        || (_factura.Establecimiento == "007" && _factura.PtoEmisionOrigen == "007")
                        )
                            {

                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Carga Balanza Scanner", "Sección Validacion primer if, configura MagellanSC");
                                scannerDL = new OposScanner_CCO.OPOSScanner();
                                //scannerDL.Open("SCRS232Scanner");
                                scannerDL.Open("MagellanSC");
                                scannerDL.ClaimDevice(0);
                                scannerDL.DeviceEnabled = true;
                                scannerDL.DataEventEnabled = true;
                                scannerDL.DecodeData = true;
                                scannerDL.AutoDisable = false;
                                scannerDL.DataEvent += new OposScanner_CCO._IOPOSScannerEvents_DataEventEventHandler(scanner_DataLogic);
                            }
                            else if ((_factura.Establecimiento == "029" && _factura.PtoEmisionOrigen != "008" && _factura.PtoEmisionOrigen != "004") || _factura.Establecimiento == "035" || _factura.Establecimiento == "037" || _factura.Establecimiento == "040"
                                //|| ( _factura.Establecimiento == "024" && _factura.PtoEmisionOrigen == "004" && _factura.PtoEmisionOrigen == "007"))
                                || (_factura.Establecimiento == "024" && (_factura.PtoEmisionOrigen == "004" || _factura.PtoEmisionOrigen == "007"))
                                )
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "Carga Balanza Scanner", "Sección Validacion primer else if, configura USBScanner");
                                scannerDL = new OposScanner_CCO.OPOSScanner();
                                //scannerDL.Open("USBHHScanner");
                                scannerDL.Open("USBScanner");
                                scannerDL.ClaimDevice(0);
                                scannerDL.DeviceEnabled = true;

                                scannerDL.DataEventEnabled = true;
                                scannerDL.DecodeData = true;
                                scannerDL.AutoDisable = false;
                                scannerDL.DataEvent += new OposScanner_CCO._IOPOSScannerEvents_DataEventEventHandler(scanner_DataLogic);
                            }
                            else if (_factura.Establecimiento == "011" && _factura.PtoEmisionOrigen == "008")
                            {
                                scannerDL = new OposScanner_CCO.OPOSScanner();
                                scannerDL.Open("RS232Scanner");
                                scannerDL.ClaimDevice(0);
                                scannerDL.DeviceEnabled = true;
                                scannerDL.DataEventEnabled = true;
                                scannerDL.DecodeData = true;
                                scannerDL.AutoDisable = false;
                                scannerDL.DataEvent += new OposScanner_CCO._IOPOSScannerEvents_DataEventEventHandler(scanner_DataLogic);
                            }
                            else
                            {
                                scanner.ScannerDataReceived += scanner_ScannerDataReceived;
                                scanner.Open();
                                //MessageBox.Show(this,"Scanner Open");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargarMainWindow", "No se pudo setear configuraciones de balanza, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    }
                }


                txtCedula.Focus();
                lblEtiquetaSaldo.Visible = false;
                lblSaldoTarjeta.Visible = false;
                _inputform = new InputBoxDialog("Ingrese cantidad de artículos:", "Cantidad de Artículos");
                _inputFormAuthUser = new InputBoxDialog("Ingrese código de autorización", "Anulación de Productos");

                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    Version ver = null;
                    System.Deployment.Application.ApplicationDeployment ad = System.Deployment.Application.ApplicationDeployment.CurrentDeployment;
                    ver = ad.CurrentVersion;
                    this.Text += " ver. " + ver.Major + "." + ver.Minor + "." + ver.Build + "." + ver.Revision;
                }
                //this.Text += " - [" + POS.Properties.Settings.Default.ROOT_URL.Replace("http://", "").Replace(":8000/", "").Replace("192.168.", "").Replace("srv-", "") + "]";
                if (IsServerChanged == 1)
                {
                    string srvConnected = string.Empty;
                    srvConnected = IPServidorConectado;
                    this.Text += " - [" + srvConnected.Replace("192.168.", "").Replace("srv-", "") + "]";
                }
                else
                {
                    this.Text += " - [" + POS.Properties.Settings.Default.ROOT_URL.Replace("http://", "").Replace(":8000/", "").Replace("192.168.", "").Replace("srv-", "") + "]";
                }

                IPServidorConectado = POS.Properties.Settings.Default.ROOT_URL.Replace("http://", "").Replace(":8000/", "");
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show(this, "No tiene asignado una IP o documento asigando a un punto de emisión, contacte a administrador!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No tiene asignado una IP o documento asigando a un punto de emisión, contacte a administrador!", "POS");
                Control.Common.General.GetMensajeToList(145);
                
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }

            tempo.Stop();
            tempo5min.Stop();

            //El load de MainWindow tiene la tarea de llamar al metodo que trae datos para RedActiva
            SetGlobalParameters(_factura);

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargarMainWindow", "Recupero los valores de PINPAD ");
            Control.Common.GlobalParameters.ConectContingente = new ConexionContingente();
            var validaPinPadEstab = Control.CajaPinpad.GeneralPagos.RecuperaDatosPinPad();


            //Validar que el secuencial de factura sea el correcto
            VerificarSecuencialFactura();

            //Carga factura temporal si la hubiere
            CargaFacturaTmpFile(); //cargaFacturatmp();

            //if (_factura.ClienteIdentificacion == null)
            //{
            //    setClienteData();
            //}
            
            
            //Procesa los voucher que no se insertaron en la tabla POS_Voucher
            // ProcesaPosVoucherTmp();

            //Obtiene los parametros para validar si el item puede saltar si ya ha sido pesado en balanza de carniceria y no vuelva a capturar peso en Caja.
            ParametroParaSaltarProductoEtiqueta();

            //Recupero la lista de Precios base
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargarMainWindow", "Recupero la lista de Precios base ");
            var ListaPrecioInit = Producto.GetListaPreciosInit(Control.Common.GlobalParameters.Establecimiento, "");
            if (ListaPrecioInit.ToList().Count == 0) { ListaPrecioInit = new List<Precio>(); }
            Control.Common.GlobalParameters.ListPrecioInit = ListaPrecioInit;

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargarMainWindow", "Lista de Precios: " + ListaPrecioInit.Count.ToString());

            tempo.Start();
            tempo5min.Start();

        }



        public MainWindowV1 IpServerConnectedTitleForm()
        {
            MainWindowV1 mw1;
            string srvConnected = string.Empty;
            srvConnected = IPServidorConectado;
            // setTextForm(this,IPServidorConectado);
            //  MainWindow.Text += " - [" + srvConnected.Replace("192.168.", "").Replace("srv-", "") + "]";
            mw1 = this;
            return mw1;

        }

        void setPromoIvaOptions()
        {
            //bool esPromoIVA = POS.Control.Common.Promo.EsPromoIVA(_factura.Establecimiento);

            //Columna Dscto IVA solo visible durante dias de Promo IVA
            var colDsctoIva = gridItems.Columns.GetColumnByFieldName("DescuentoIVA");
            colDsctoIva[0].IsVisible = Control.Promos.PromoIVA.EsPromoIVA();// esPromoIVA;            
        }

        void scanner_DataLogic(int Status)
        {
            string cad;
            int chk = -1;

            if (!String.IsNullOrEmpty(_factura.ModeloBalanza))
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "scanner_DataLogic", "Se configura con Balanza Datalogic segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen + " - " + _factura.ModeloBalanza);
                string[] parametrosbalanza = _factura.ModeloBalanza.Split('|');
                if (parametrosbalanza[3] == "1")
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "scanner_DataLogic", "Se configura ScanDataLabel segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                    cad = scannerDL.ScanDataLabel.ToString().Replace("F", "").Replace("A", "").Replace("\r", "");
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "scanner_DataLogic", "Se configura ScanData segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                    cad = scannerDL.ScanData.ToString().Replace("F", "").Replace("A", "").Replace("\r", "");
                }
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "scanner_DataLogic", "Se configura con Balanza Datalogic segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                if ((_factura.Establecimiento == "029" && _factura.PtoEmisionOrigen != "008" && _factura.PtoEmisionOrigen != "004")
                || (_factura.Establecimiento == "024" && (_factura.PtoEmisionOrigen != "003" && _factura.PtoEmisionOrigen != "005")) //&& _factura.PtoEmisionOrigen != "006")
                || _factura.Establecimiento == "035" || _factura.Establecimiento == "037" || _factura.Establecimiento == "040"
                || (_factura.Establecimiento == "007" && _factura.PtoEmisionOrigen == "007")
                || (_factura.Establecimiento == "011" && _factura.PtoEmisionOrigen == "008")
             )
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "scanner_DataLogic", "Se configura ScanDataLabel segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                    cad = scannerDL.ScanDataLabel.ToString().Replace("F", "").Replace("A", "").Replace("\r", "");
                }
                else
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "scanner_DataLogic", "Se configura ScanData segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen);
                    cad = scannerDL.ScanData.ToString().Replace("F", "").Replace("A", "").Replace("\r", "");
                }
            }
            scanner.ControlToShowText = this.txtCodigo;
            scanner.UpdateText2(cad);
            scannerDL.DataEventEnabled = true;
            this.txtCodigo.Focus();
            SendKeys.Send("{Enter}");

        }


        public static int CalculateChecksumEAN13(string code)
        {
            try
            {
                if (code == null || code.Length != 12)
                    throw new ArgumentException("Code length should be 12, i.e. excluding the checksum digit");

                int sum = 0;
                for (int i = 0; i < 12; i++)
                {
                    int v;
                    if (!int.TryParse(code[i].ToString(), out v))
                        throw new ArgumentException("Invalid character encountered in specified code.");
                    sum += (i % 2 == 0 ? v : v * 3);
                }
                int check = 10 - (sum % 10);
                return check % 10;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }


        public static int CalculateChecksumUPCA(string code)
        {
            try
            {
                int checksum = 0;
                int i, len, mul = 3, sum = 0, m10 = 0;
                len = code.Length;
                if (len != 11)
                    return -1;

                for (i = 0; i < len; i++)
                {
                    sum += int.Parse(code.Substring(i, 1)) * mul;
                    if (mul == 3) mul = 1;
                    else mul = 3;
                }

                m10 = sum % 10;

                if (m10 == 0)
                    return 1;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        void scanner_ScannerDataReceived(object sender, EventArgs e)
        {
            if (!flagCarnicero)
            {
                scanner.ControlToShowText = this.txtCodigo;
                this.txtCodigo.Focus();
            }
            else
            {
                scanner.ControlToShowText = this._inputform._txtInput;
                this._inputform._txtInput.Focus();
            }
            SendKeys.Send("{Enter}");
        }

        private void setTituloDocumento()
        {
            if (_factura != null)
            {
                clienteGroup.Text = _factura.Documento + "-" + _factura.Establecimiento + "-" + _factura.PtoEmision + "-" + _factura.Secuencia.ToString("000000000.##");
                if (_factura.Documento == "F")
                {
                    esFactura = true;
                    esGiftCard = false;
                }
                else if (_factura.Documento == "R")
                {
                    esFactura = false;
                    esGiftCard = true;
                }
            }
        }

        private bool clienteActivo()
        {
            return cliente_actual != null && !string.IsNullOrEmpty(_factura.Cliente_codigo) && _factura.Cliente_codigo.Length > 0;
        }

        private bool AplicarDescuentoCuponV2(string codigoCupon)
        {
            bool respuesta = false;

            int tienePago = _factura.Pagos.Count;
            bool EsUsoCuponPromocional = _factura.EsUsoCuponPromocional;

            string sQuery = string.Empty;
            var pos = new POSEntities();
            DataSet dtsConsulta = new DataSet();
            string CadenaConexion = pos.Database.Connection.ConnectionString;
            DataTable tblVoucher = new DataTable();
            SqlConnection con = new SqlConnection();

            if (string.IsNullOrEmpty(codigoCupon)) {
                respuesta = false;
                return respuesta;
            }

            DataTable ArticuloPOS = UDT_DetArticuloPOS.GetDataTableArticuloPOS();
            DataTable DetDescuentos = UDT_DetDescuentos.GetDataTableDetDescuentos();

            var db = new POSEntities();
            var cupon = db.core_TarjetaDescuento.Where(x => x.codigo == codigoCupon && x.numeroFactura == -1).FirstOrDefault();

           
            if (_factura.Productos.Count() > 0)
            {
                foreach (var producto in _factura.Productos)
                {
                    
                    DataRow workRow = ArticuloPOS.NewRow();
                    workRow["OrigenConsulta"] = "";
                    workRow["ITEMID"] = producto.Id;
                    workRow["CANTIDAD"] = producto.Cantidad;
                    ArticuloPOS.Rows.Add(workRow);

                    if (producto.DescuentosCupon == null)
                    {
                        producto.DescuentosCupon = new List<Descuento>();
                    }
                    else
                    {

                        foreach (var detDscto in producto.DescuentosCupon)
                        {
                            DataRow workRowDsct = DetDescuentos.NewRow();
                            workRowDsct["establecimiento"] = "";
                            workRowDsct["punto_emision"] = "";
                            workRowDsct["tipo_descuento"] = "";
                            workRowDsct["codigo"] = detDscto.codigo;
                            workRowDsct["parametro"] = detDscto.parametro;
                            workRowDsct["parametro2"] = detDscto.parametro2;
                            workRowDsct["valor"] = detDscto.valor;
                        }

                    }
                }
                

            }

            try
            {
                using (con = new SqlConnection(CadenaConexion))
                {
                    con.Open();
                    int usuCuponPromociona = EsUsoCuponPromocional == false ? 0 : 1;


                    sQuery = string.Empty;
                    sQuery = string.Concat(sQuery, $"Declare @CodError int  ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"Declare @MsjError varchar(254) ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"Declare @RespAplicaDsto varchar(254) ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $" Exec spPOSAplicarDescuentoCupon ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"  @codigoCupon = '{codigoCupon}'", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"  ,@TblDetalleArticulosVta ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"  ,@TblDetCuponDescuento ", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"  ,@EsUsoCuponPromocional = {usuCuponPromociona}", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"  ,@TienePagos = {tienePago}", Environment.NewLine);
                    sQuery = string.Concat(sQuery, $"  ,@CodError , @MsjError, @RespAplicaDsto", Environment.NewLine);
                    

                    using (SqlCommand cmd = new SqlCommand(sQuery, con))
                    {
                        var objArticulo = new SqlParameter("@TblDetalleArticulosVta", SqlDbType.Structured);
                        objArticulo.TypeName = "dbo.UDT_DetArticuloPOS";
                        objArticulo.Value = ArticuloPOS;
                        cmd.Parameters.Add(objArticulo);

                        var DetCuponDescuento = new SqlParameter("@TblDetCuponDescuento", SqlDbType.Structured);
                        DetCuponDescuento.TypeName = "dbo.UDT_CuponDescuento";
                        DetCuponDescuento.Value = DetDescuentos;
                        cmd.Parameters.Add(DetCuponDescuento);
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }

            respuesta = true;

            return respuesta;

        }

        private bool AplicarDescuentoCupon(string codigoCupon, ref bool encontroCupon)
        {
            bool respuesta = false;
            // msgBoxCtrl = new MsgBoxCtrl();

            try
            {
                //Reemplazo obligado por error de impresion en Manejo Seguro
                codigoCupon = codigoCupon.Replace("A", "");
                
                using (POSEntities db = new POSEntities())
                {
                    var cupon = db.core_TarjetaDescuento.Where(x => x.codigo == codigoCupon && x.numeroFactura == -1).FirstOrDefault();
                    if (cupon != null)
                    {
                        encontroCupon = true;
                        var itemsCupon = db.core_descuento.Where(x => x.tipo_descuento == "ProductoCantidadCupon" 
                        && x.parametro2 == cupon.codigo && x.activo).ToList();

                        if (_factura.EsUsoCuponPromocional)
                        {
                            if (itemsCupon != null)
                            {
                                if (itemsCupon.First().valor != -1)
                                {
                                    //System.Windows.Forms.MessageBox.Show(this, "Ya utilizó un cupón promocional en esta factura", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Ya utilizó un cupón promocional en esta factura", "POS");
                                    Control.Common.General.GetMensajeToList(132);

                                    return false;
                                }
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Ya utilizó un cupón promocional en esta factura", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Ya utilizó un cupón promocional en esta factura", "POS - Cupón Promocional");
                                Control.Common.General.GetMensajeToList(132);

                                return false;
                            }
                        }

                        if (_factura.Pagos.Count > 0)
                        {
                            Control.Common.General.GetMensajeToList(55);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "No se puede utilizar cupón promocional una vez que ha cargado formas de pago", "POS - Cupón Promocional");
                            //System.Windows.Forms.MessageBox.Show(this, "No se puede utilizar cupón promocional una vez que ha cargado formas de pago", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return false;
                        }

                        if (!cupon.activo)
                        {
                            Control.Common.General.GetMensajeToList(146);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Cupón ya fue utilizado en otra facturar", "POS - Cupón Promocional");
                            //System.Windows.Forms.MessageBox.Show(this, "Cupón ya fue utilizado en otra factura", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return false;
                        }

                        decimal porcDesc = cupon.saldo / 100M;

                        if (itemsCupon.Count > 0)
                        {
                            foreach (var producto in _factura.Productos)
                            {
                                var itemcupon = itemsCupon.Where(x => x.parametro == producto.Id).FirstOrDefault();

                                if (itemcupon != null)
                                {
                                    var valorDscto = 0M;
                                    if (itemcupon.valor ==-1)
                                    {
                                        if (producto.DescuentosCupon == null)
                                        {
                                            producto.DescuentosCupon = new List<Descuento>();
                                        }

                                        if (producto.DescuentosCupon.Count() < producto.Cantidad)
                                        {
                                            var cuponIngresado = producto.DescuentosCupon.Where(x => x.codigo == itemcupon.parametro2);

                                            if (cuponIngresado.Count()==0)
                                            {
                                                if (producto.DescuentosCupon.Count() ==0)
                                                {
                                                    producto.update(true);
                                                }

                                                valorDscto = (producto.SubtotalSinDescuento / producto.Unidades) * porcDesc;

                                                Descuento descuento = new Descuento();
                                                descuento.codigo = itemcupon.parametro2;
                                                descuento.valor = itemcupon.valor;
                                                descuento.parametro = porcDesc.ToString();

                                                producto.DescuentosCupon.Add(descuento);

                                                producto.DescuentoAX = producto.DescuentoAX + valorDscto;
                                                producto.update(false);
                                            }
                                            else
                                            {
                                                Control.Common.General.GetMensajeToList(131);
                                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Stop, "El Cupón ya fue aplicado", "POS - Cupón Promocional");
                                                //System.Windows.Forms.MessageBox.Show(this, "El Cupón ya fue aplicado", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                                return false;
                                            }
                                        }
                                        else
                                        {
                                            Control.Common.General.GetMensajeToList(147);
                                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Stop, "No puede ingresar más Cupones para este producto", "POS - Cupón Promocional");
                                            //System.Windows.Forms.MessageBox.Show(this, "No puede ingresar más Cupones para este producto", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        valorDscto = (producto.SubtotalSinDescuento / producto.Unidades) * porcDesc;
                                        producto.Descuentos.Clear();
                                        producto.Descuento = 0;
                                        producto.DescuentoAX = 0;
                                        producto.update(false);
                                        producto.DescuentoAX = valorDscto;
                                        producto.update();
                                    }
                                    
                                    _factura.EsUsoCuponPromocional = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (var producto in _factura.Productos)
                            {
                                var valorDscto = producto.SubtotalSinDescuento * porcDesc;
                                producto.Descuentos.Clear();
                                producto.Descuento = 0;
                                producto.DescuentoAX = 0;
                                producto.update(false);

                                producto.DescuentoAX = valorDscto;
                                producto.update();

                                _factura.EsUsoCuponPromocional = true;
                            }
                        }

                        if (_factura.EsUsoCuponPromocional)
                        {
                            codigoCuponPromocional = cupon;
                            //cupon.activo = false;
                            //cupon.fecha_desactivacion = DateTime.Now;
                            //cupon.codigoCliente = _factura.getNumeroFacturaOpcional();
                            //db.SaveChanges();

                            calcularFactura();
                            //System.Windows.Forms.MessageBox.Show(this, "Se aplicó correctamente el cupón de descuento '" + codigoCupon + "'", "Cupón Promocional", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Se aplicó correctamente el cupón de descuento ", "POS - Cupón Promocional");

                            List<ParametrosMensajes> parametrosMensajes = new List<ParametrosMensajes>();
                            parametrosMensajes.Add(new ParametrosMensajes() { codigo = "[codigoCupon]", valor = codigoCupon.ToString() });
                            Control.Common.General.GetMensajeToList(148, parametrosMensajes);

                            respuesta = true;

                            //_factura.EsUsoCuponPromocional = true;
                            _factura.CuponPromocionalPorcDesc = porcDesc;
                            _factura.CuponPromocionalCodigo = cupon.codigo;
                        }
                        else {

                            //System.Windows.Forms.MessageBox.Show(this, "No se aplicó cupón promocional en esta factura", "No aplica", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No se aplicó cupón promocional en esta factura", "POS - Cupón Promocional");
                            Control.Common.General.GetMensajeToList(149);
                        }
                        



                    }
                }
            }
            catch (Exception ex)
            {
                POS.Control.Common.Logger.LogMessage(POS.Control.Common.Enum.LogTypes.Error, "MainWindow", "AplicarDescuentoCupon", "Incidente mientras se intentaba aplicar codigoCupon '" + codigoCupon + "', a continuacion las excepciones encontradas - " + POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                respuesta = false;
            }

            return respuesta;
        }

        private void txtCodigo_KeyPress(object sender, KeyPressEventArgs e)
        {


            if (txtCodigo.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
            {
                LimpiarClienteCompraGratis();
                tempo666.Start();
            }

            if (txtCodigo.Text.Length != txtCodigo.SelectionStart)
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)Keys.Enter)
            {
                /*Si el codigo, comienza con los 3 primero digitos AppMovil_PrefijoUsaApp, se lo considera como lectura de cliente*/
                if (txtCodigo.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
                {
                    txtCedula.Text = txtCodigo.Text;

                    txtCodigo.Text = "";
                    txtCedula_KeyPress(sender, e);
                    return;
                }

                if (clienteActivo())
                {
                    var val = txtCodigo.Text.Trim();
                    bool encontroCupon = false;

                    if (AplicarDescuentoCupon(val, ref encontroCupon))
                    {
                        txtCodigo.Clear();
                        txtCodigo.Focus();
                        return;
                    }
                    else if (encontroCupon)
                    {
                        txtCodigo.Clear();
                        txtCodigo.Focus();
                        return;
                    }

                    if (_factura.Documento == "F")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "txtCodigo_KeyPress", $" INICIO  getProducto({val}); ");
                        getProducto(val);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "txtCodigo_KeyPress", $" FIN  getProducto({val}); ");
                        //agregar log Info del item.
                    }
                    else
                    {
                        getGiftCard(val);
                    }

                    txtCodigo.Clear();
                    txtCodigo.Focus();

                    /********************************************************************          
                     * hasta aqui promos personalizadas                       
                     * ******************************************************************
                     */
                    
                    //if (productotmp.Id != null)
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "txtCodigo_KeyPress", " INICIO Va escribir archivo temporal ");

                    agregaProductosTmpFile();  //agregaProductosTmp(codigo);

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "txtCodigo_KeyPress", " FIN Va escribir archivo temporal ");

                }
                else
                {
                    ShowDesktopAlert("Acción no valida",
                        "No ha escogido un cliente!", position: AlertScreenPosition.TopCenter, autoCloseDelay: 2);

                    txtCedula.Focus();
                    txtCedula.SelectAll();
                }

            }

        }

       
        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
         
            var db = new POSEntities();
            var strtecla = keyData.ToString();
            var parkey = db.core_parametro.Where(x => x.identificador == "POS_KEYBOARD" && x.valor == strtecla).FirstOrDefault();

            if (parkey != null)
            {
                if (parkey.parametro2 == "GRABARFACTURA")
                {
                    btnGrabar_Click(null, null);
                    return false;
                }
                else if (parkey.parametro2 == "EFECTIVOEXACTO")
                {
                    txtPagoValor.Text = _factura.GetTotal().ToString();
                    return false;
                }
                else if (parkey.parametro2 == "REIMPRIMIRVOUCHER")
                {
                    f4();
                    return false;
                }
                else if (parkey.parametro2 == "REIMPRIMIRFACTURA")
                {
                    f8();
                    return false;
                }
                else if (parkey.parametro2 == "CONSULTAPRECIO")
                {
                    btnPrecio_Click(null, null);
                    return false;
                }
                else if (parkey.parametro2 == "CANTIDADPRODUCTO")
                {
                    SelectQtyProduct();
                    return false;
                }
                else if (parkey.parametro2 == "PAGOTCREDITO")
                {
                    btnTCredito_Click(new object(), new EventArgs());
                    return false;
                }
                else if (parkey.parametro2 == "CONSUMIDORFINAL")
                {
                    if (btnCFinal.Enabled == false) {
                        //System.Windows.Forms.MessageBox.Show(this, "Consumidor Final deshabilitado debe seleccionar cliente con Cedula o RUC");
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Consumidor Final deshabilitado debe seleccionar cliente con Cedula o RUC", "POS - Cupón Promocional");
                        Control.Common.General.GetMensajeToList(150);

                    }
                    else
                        FinalClient();

                    return false;
                }
                else if (parkey.parametro2 == "PAGOCHEQUE")
                {
                    btnCheque_Click(new object(), new EventArgs());
                    return false;
                }
                else if (parkey.parametro2 == "PAGOSERVICIO")
                {
                    POS.Control.CorrBan.PagosServicios _frm = new POS.Control.CorrBan.PagosServicios();
                    _frm.ShowDialog();
                    return false;
                }
                else if (parkey.parametro2 == "FORMAPAGO")
                {
                    this.btnPagar_Click(new object(), new EventArgs());
                    return false;
                }
                else if (parkey.parametro2 == "ADMINISTRADOR")
                {
                    //Abre ToolBox
                    f10();
                    return false;
                }
            }

            switch (keyData)
            {
                case Keys.Space:
                    if (esFactura)
                    {
                        //SearchProFunction();
                        btnBusqProd_Click(new object(), new EventArgs());
                    }
                    break;
                case Keys.F1:
                    //Arqueo para auditores
                    f1();
                    break;
                case Keys.F2:
                    //Realiza cierre de caja
                    f2();
                    break;
                case Keys.F3:
                    break;
                case Keys.F4:
                    //Abre base pagos PINPAD
                    if (db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                    {
                        f4();
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(151);
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "No tiene habilitada esta opcion", "POS - Cupón Promocional");
                        //System.Windows.Forms.MessageBox.Show(this, "No tiene habilitada esta opcion");
                    }
                    break;
                case Keys.F5:
                    btnGrabar_Click(null, null);
                    break;
                case Keys.F6:
                    //Genera reporte cierre de caja parcial.
                    f6();
                    break;
                case Keys.F7:

                    // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Procediendo a actualizar los parámetros de POS, pulse Sí para continuar", "POS");
                    // var result = Control.Common.General.GetMensaje("POS", "Procediendo a actualizar los parámetros de POS, pulse Sí para continuar", "I");
                    var result = Control.Common.General.GetMensajeToList(587);
                    if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                    {
                        RecargarParametrosPOS();

                        listPromociones = this.getPromociones(_factura.Establecimiento);
                        _factura.PromocionesActuales = listPromociones;

                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Proceso finalizado", "POS");
                        //Control.Common.General.GetMensaje("POS", "Parametros Actualizados correctamente", "I");
                        Control.Common.General.GetMensajeToList(588);

                    }


                    //if (System.Windows.Forms.MessageBox.Show("Procediendo a actualizar los parámetros de POS, pulse Sí para continuar", "POS", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                    //    RecargarParametrosPOS();
                    //    //  _factura.configurarPromociones();
                    //    listPromociones = this.getPromociones(_factura.Establecimiento);
                    //    _factura.PromocionesActuales = listPromociones;

                    //    msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Proceso Finalizado", "POS");

                    //    //System.Windows.Forms.MessageBox.Show("Proceso finalizado");

                    //    //msgBox = new MsgBox("question", "Proceso finalizado", "");
                    //    //dialogResult = msgBox.ShowDialog();
                    //}


                    break;
                case Keys.F9:
                    SelectQtyProduct();
                    break;
                case Keys.F8:
                    //JCanarte 11Ene2021
                    if (db.core_parametro.Where(x => x.identificador == "REIMPRIME_FACTURA" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                    {
                        f8();
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(590);

                        //Control.Common.General.GetMensajeToList("POS", "No tiene habilitada esta opcion", "I");
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "No tiene habilitada esta opcion", "POS");
                        //System.Windows.Forms.MessageBox.Show(this, "No tiene habilitada esta opcion");
                        //msgBox = new MsgBox("question", "No tiene habilitada esta opcion", "");
                        //dialogResult = msgBox.ShowDialog();

                    }
                    break;
                case Keys.F10:
                    //Abre ToolBox
                    f10();
                    break;
                case Keys.F12:
                    //MessageBox.Show(this,Program.ID_Caja_POS);
                    if (btnCFinal.Enabled == false)
                    {
                        Control.Common.General.GetMensajeToList(589);
                        //Control.Common.General.GetMensaje("POS", "Consumidor Final deshabilitado debe seleccionar cliente con Cedula o RUC", "I");
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "Consumidor Final deshabilitado debe seleccionar cliente con Cedula o RUC", "POS");

                        //System.Windows.Forms.MessageBox.Show(this, "Consumidor Final deshabilitado debe seleccionar cliente con Cedula o RUC");
                        //msgBox = new MsgBox("question", "Consumidor Final deshabilitado debe seleccionar cliente con Cedula o RUC", "");
                        //dialogResult = msgBox.ShowDialog();
                    }
                    else { FinalClient();  }
                        
                    break;


            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        #region Parametros POS

        private void RecargaListaMensajes()
        {

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargaListaMensajes", " Recarga lista de Mensajes POS ");

            try
            {
                // Control.Common.General.ListMensaje;
                Control.Common.General.GetListMensaje();

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargaListaMensajes", $" error al cargar la Lista de Mensajes POS: {ex.Message} ");
            }
        }

        private void RecargarParametrosPOS()
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "Se ejecuta Recarga Parametros de POS ");
            //Recupera lista de Mensajes
            RecargaListaMensajes();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargaListaMensajes");

            //Parametros Generales
            RecargarParametrosGenerales();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosGenerales");

            //Recarga parametros de Pinpad
            Control.Common.GlobalParameters.ConectContingente = new ConexionContingente();
            var validaPinPadEstab = Control.CajaPinpad.GeneralPagos.RecuperaDatosPinPad();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecuperaDatosPinPad");

            //Parametros CorrBan
            RecargarParametrosRedActiva();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosRedActiva");


            //Parametros Ptos Acumulados
            RecargarParametrosPuntos();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosPuntos");

            //Parametros Parking
            
            //RecargarParametrosParking();
            //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosParking");

            RecargarParametrosParkingV2();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosParkingV2");

            //Parametros que se deben recargar en cada factura
            RecargarParametrosCadaFactura();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosCadaFactura");

            //Parametros retenciones electronicas
            RecargarParametrosRetencionElectronica();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosRetencionElectronica");

            //Parametros tarjeta empresarial
            RecargarParametrosTarjetaEmpresarial();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosTarjetaEmpresarial");

            //Parametros de POS Liquidacion
            RecargarParametrosLiquidacion();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarParametrosLiquidacion");


            //Recargar Imagenes
            RecargarImagenes();
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "RecargarImagenes");

            //Recupero la lista de Precios base
            var ListaPrecioInit = Producto.GetListaPreciosInit(Control.Common.GlobalParameters.Establecimiento, "");
            if (ListaPrecioInit.ToList().Count == 0) { ListaPrecioInit = new List<Precio>(); }
            Control.Common.GlobalParameters.ListPrecioInit = ListaPrecioInit;

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "RecargarParametrosPOS", "GetListaPreciosInit");

            /*Parametro para Boton Dscto. PaviPlan*/
            //btnDsctoPaviPlan
            ParametroBotonesFormaPagoPOS();



        }

        private void ParametroBotonesFormaPagoPOS()
        {
            bool DsctoPaviPlan = false;

            try
            {
                using (POSEntities db = new POSEntities())
                {

                    var bntDsctoPaviPlan = (from deta in db.core_parametro
                                            where deta.identificador == "FORMA_PAGO_DSCTO_PAVIPLAN"
                                            select deta).ToList();

                    if (bntDsctoPaviPlan.Count > 0)
                    {
                        DsctoPaviPlan = bntDsctoPaviPlan.FirstOrDefault().valor == "TRUE" ? true: false ;
                    }

                    btnDsctoPaviPlan.Visible = DsctoPaviPlan;
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        private void RecargarParametrosCadaFactura()
        {
            //Parametros PromoIVA
            RecargarParametrosPromoIVA();
        }

        private void RecargarParametrosPromoIVA()
        {
            //msgBoxCtrl = new MsgBoxCtrl();
            if (Control.Promos.PromoIVA.RecargarParametrosPromoIVA())
            {
                //Setear dias de promo IVA para el local
                //Control.Common.Promo.SetDiasPromoIVA();

                //Setear controles especiales para dia de Promo IVA
                setPromoIvaOptions();
            }
            else
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "RecargarParametrosGenerales", "El aplicativo no pudo tomar todos los parámetros de Promo IVA por lo que se cerrara para evitar inconsistencias");

                Control.Common.General.GetMensajeToList(152);

                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Warning, " El aplicativo no pudo tomar todos los parámetros de Promo IVA, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", " POS - Parámetros Generales");
                //MsgBox msgBox = new MsgBox("question", " El aplicativo no pudo tomar todos los parámetros de Promo IVA, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", "POS - Parámetros Generales");
                //DialogResult dialogResult = msgBox.ShowDialog();

                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo tomar todos los parámetros de Promo IVA, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        private void RecargarParametrosRedActiva()
        {
            //Cargar configuraciones de Corresponsal Bancario
            Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();

            btnCorresponsal.Visible = Control.CorrBan.ClsCorrBan.EsCorresponsalActivo;
           
        }

        public void RecargarParametrosPuntos()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    var paramWalletPoints = db.core_parametro.Where(x => x.identificador.Equals("WALLETPOINTS_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault();

                    if (paramWalletPoints == null)
                    {
                        throw new Exception("El local no tiene parametro WALLETPOINTS_XXXX-XXXXX configurado en la tabla core_parametro");
                    }
                    else
                    {
                        Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva = (paramWalletPoints.valor == "TRUE");

                        if (Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva)
                        {

                            // Multi Campañas que este vigentes.    JMM  29-08-2019                                                        
                            var campaniasvigentes = db.LstCampania.Where(x => x.Estado == true);

                            foreach (var campa in campaniasvigentes)
                            {
                                var _points = new Control.WalletPoints.ClsListPoints();
                                _points.idCampania = campa.IdLstCampania;
                                _points.EsOpcionPuntosActiva = Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva;

                                ////Debe Recargar Parametros En Tiempo Real
                                _points.DebeRecargarParametrosEnTiempoReal = (paramWalletPoints.parametro2 == "TRUE");

                                ////Factor Acumulacion
                                var factorAcumulacion = db.core_parametro.Where(x => x.identificador == "WALLETPOINTS_FACTORACUMULACION_" + campa.IdLstCampania).FirstOrDefault();
                                if (factorAcumulacion == null) throw new Exception("No hay parametro WALLETPOINTS_FACTORACUMULACION_" + campa.IdLstCampania + " configurado en la tabla core_parametro");
                                _points.FactorAcumulacion = decimal.Parse(factorAcumulacion.valor);

                                //Factor Canje
                                var factorCanje = db.core_parametro.Where(x => x.identificador == "WALLETPOINTS_FACTORCANJE").FirstOrDefault();
                                if (factorCanje == null) throw new Exception("No hay parametro WALLETPOINTS_FACTORCANJE configurado en la tabla core_parametro");
                                _points.FactorCanje = decimal.Parse(factorCanje.valor);
                                Control.WalletPoints.ClsPoints.FactorCanje = decimal.Parse(factorCanje.valor);

                                //Plantilla para recibo en factura
                                var plantillaMonedero = db.core_recibo.Where(x => x.identificador == "WALLETPOINTS_RECIPT_" + campa.IdLstCampania).FirstOrDefault();
                                if (plantillaMonedero == null) throw new Exception("No hay recibo WALLETPOINTS_RECIPT_" + campa.IdLstCampania + " configurado en la tabla core_recibo");
                                _points.PlantillaMonederoFactura = plantillaMonedero.cuerpo;

                                //Plantilla para recibo de debito de puntos
                                var plantillaDebitoMonedero = db.core_recibo.Where(x => x.identificador == "WALLETPOINTS_DEBITRECIPT_" + campa.IdLstCampania).FirstOrDefault();
                                if (plantillaDebitoMonedero != null) //throw new Exception("No hay recibo WALLETPOINTS_DEBITRECIPT configurado en la tabla core_recibo");
                                    _points.PlantillaDebitoMonedero = plantillaDebitoMonedero.cuerpo;

                                if (listPoints.Any(x => x.idCampania == _points.idCampania))
                                    listPoints.RemoveAll(x => x.idCampania == _points.idCampania);

                                //Xml Acumulacion Puntos
                                var param = db.core_parametro.Where(x => x.identificador.Equals("WALLET_XMLACUMULAPUNTOS")).FirstOrDefault();
                                if (param != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(param.valor))
                                        _points.PtsCliente_XmlAcumulacion = param.valor;
                                    else
                                        throw new Exception("Campo 'valor' de registro 'WALLET_XMLACUMULAPUNTOS' en core_parametro es incorrecto, no puede ser nulo ni espacios");
                                }
                                else
                                    throw new Exception("No hay parametro 'WALLET_XMLACUMULAPUNTOS' en core_parametro");

                                var paramXml = db.core_parametro.Where(x => x.identificador.Equals("WALLET_XMLACUMULAPUNTOSGEN")).FirstOrDefault();
                                if (param != null)
                                {
                                    if (!string.IsNullOrWhiteSpace(param.valor))
                                        _points.PtsCliente_XmlAcumulacionGen = paramXml.valor;
                                    else
                                        throw new Exception("Campo 'valor' de registro 'WALLET_XMLACUMULAPUNTOSGEN' en core_parametro es incorrecto, no puede ser nulo ni espacios");
                                }
                                else
                                    throw new Exception("No hay parametro 'WALLET_XMLACUMULAPUNTOSGEN' en core_parametro");


                                listPoints.Add(_points);

                                if (db.core_parametro.AsEnumerable().Any(x => x.identificador == "CAMPANIA_MONEDERO"
                                                          && x.valor == _points.idCampania.ToString()))
                                    POS.Control.Common.GlobalParameters.MonederoActivo = true;

                            }

                            // Se comenta porque se manejara multi campaña.     JM  29-08-2019
                            ////Debe Recargar Parametros En Tiempo Real
                            //Control.WalletPoints.ClsPoints.DebeRecargarParametrosEnTiempoReal = (paramWalletPoints.parametro2 == "TRUE");

                            ////Factor Acumulacion
                            //var factorAcumulacion = db.core_parametro.Where(x => x.identificador == "WALLETPOINTS_FACTORACUMULACION").FirstOrDefault();
                            //if (factorAcumulacion == null) throw new Exception("No hay parametro WALLETPOINTS_FACTORACUMULACION configurado en la tabla core_parametro");
                            //Control.WalletPoints.ClsPoints.FactorAcumulacion = decimal.Parse(factorAcumulacion.valor);

                            ////Factor Canje
                            //var factorCanje = db.core_parametro.Where(x => x.identificador == "WALLETPOINTS_FACTORCANJE").FirstOrDefault();
                            //if (factorCanje == null) throw new Exception("No hay parametro WALLETPOINTS_FACTORCANJE configurado en la tabla core_parametro");
                            //Control.WalletPoints.ClsPoints.FactorCanje = decimal.Parse(factorCanje.valor);

                            ////Plantilla para recibo en factura
                            //var plantillaMonedero = db.core_recibo.Where(x => x.identificador == "WALLETPOINTS_RECIPT").FirstOrDefault();
                            //if (plantillaMonedero == null) throw new Exception("No hay recibo WALLETPOINTS_RECIPT configurado en la tabla core_recibo");
                            //Control.WalletPoints.ClsPoints.PlantillaMonederoFactura = plantillaMonedero.cuerpo;

                            ////Plantilla para recibo de debito de puntos
                            //var plantillaDebitoMonedero = db.core_recibo.Where(x => x.identificador == "WALLETPOINTS_DEBITRECIPT").FirstOrDefault();
                            //if (plantillaDebitoMonedero == null) throw new Exception("No hay recibo WALLETPOINTS_DEBITRECIPT configurado en la tabla core_recibo");
                            //Control.WalletPoints.ClsPoints.PlantillaDebitoMonedero = plantillaDebitoMonedero.cuerpo;

                            ////Xml Acumulacion Puntos
                            //var param = db.core_parametro.Where(x => x.identificador.Equals("WALLET_XMLACUMULAPUNTOS")).FirstOrDefault();
                            //if (param != null)
                            //{
                            //    if (!string.IsNullOrWhiteSpace(param.valor))
                            //    {
                            //        POS.Control.WalletPoints.ClsPoints.PtsCliente_XmlAcumulacion = param.valor;
                            //    }
                            //    else
                            //    {
                            //        throw new Exception("Campo 'valor' de registro 'WALLET_XMLACUMULAPUNTOS' en core_parametro es incorrecto, no puede ser nulo ni espacios");
                            //    }
                            //}
                            //else
                            //{
                            //    throw new Exception("No hay parametro 'WALLET_XMLACUMULAPUNTOS' en core_parametro");
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosPuntos", Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                //System.Windows.Forms.MessageBox.Show("No se pudo establecer permiso para Puntos Acumulados, esto puede deberse a que al local no se le ha configurado un permiso para esta opción o a que la red estuvo fuera de servicio brevemente");
                Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva = false;
            }

            btnWallet.Visible = Control.WalletPoints.ClsPoints.EsOpcionPuntosActiva;
        }

        private void RecargarParametrosGenerales()
        {
            try
            {

                //Parametro que identifica el codigo del Autorizador PINPAD 
                /*
                 * 1 - Medianet
                 * 2 - Datafast
                 * 3 - Autro
                 */
                //POS.Control.Common.GlobalParameters.ConectContingente = new ConexionContingente();
                

                using (POSEntities db = new POSEntities())
                {
                    var paramInactividadIntervalo = db.core_parametro.Where(x => x.identificador.Equals("INACTIVIDAD_INTERVALO")).FirstOrDefault();

                    if (paramInactividadIntervalo == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No está configurado el parametro INACTIVIDAD_INTERVALO configurado en la tabla core_parametro");
                        Control.Common.GlobalParameters.SensorInactividadSegundosIntervalo = 180000;
                    }
                    else
                    {
                        Control.Common.GlobalParameters.SensorInactividadSegundosIntervalo = int.Parse(paramInactividadIntervalo.valor);
                    }

                    //Pinpad Msj Autorizador no valido
                    var param = db.core_parametro.Where(x => x.identificador.Equals("MSJ_PINPAD_AUTORIZADORNOVALIDO")).FirstOrDefault();

                    if (param == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No está configurado el parametro MSJ_PINPAD_AUTORIZADORNOVALIDO configurado en la tabla core_parametro");
                        Control.Common.GlobalParameters.PinpadMsjAutorizadorNoValido = "Tarjeta no se encuentra autorizada para este tipo de transaccion";
                    }
                    else
                    {
                        Control.Common.GlobalParameters.PinpadMsjAutorizadorNoValido = string.IsNullOrWhiteSpace(param.valor) ? string.Empty : param.valor;
                    }

                    //Credenciales Administrador
                    param = db.core_parametro.Where(x => x.identificador.Equals("ADMINCREDENCIAL")).FirstOrDefault();

                    if (param != null)
                    {
                        if (!string.IsNullOrWhiteSpace(param.valor) && !string.IsNullOrWhiteSpace(param.parametro2))
                        {
                            POS.Control.Common.GlobalParameters.MasterUser = param.valor;
                            POS.Control.Common.GlobalParameters.MasterPassword = param.parametro2;
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "Campo 'valor' o 'parametro2' de registro 'ADMINCREDENCIAL' en core_parametro es incorrecto, no puede ser nulo ni espacios. Utilizar credenciales por defecto");
                        }
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'ADMINCREDENCIAL' en core_parametro. Utilizar credenciales por defecto");
                    }

                    //Producto Item Peso
                  //  cambiaor22
                    param = db.core_parametro.Where(x => x.identificador.Equals("ID_PRODUCTO_PESO")).FirstOrDefault();

                    if (param != null)
                    {
                        if (!string.IsNullOrWhiteSpace(param.valor) && !string.IsNullOrWhiteSpace(param.parametro2))
                        {
                            POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPeso = param.valor;
                            POS.Control.Common.GlobalParameters.ProductoIdentificadorItemPesoAlt = param.parametro2;
                        }
                        else
                        {
                            throw new Exception("Campo 'valor' o 'parametro2' de registro 'ID_PRODUCTO_PESO' en core_parametro es incorrecto, no puede ser nulo ni espacios");
                        }
                    }
                    else
                    {
                        throw new Exception("No hay parametro 'ID_PRODUCTO_PESO' en core_parametro");
                    }
    

                    // inicio parametros de PROMO IVA ref:abc 20220728
                    param = db.core_parametro.Where(x => x.identificador == "DESC_PROMO_IVA" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault();
                    if (param != null)
                    {
                        if (!string.IsNullOrEmpty(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.DESC_PROMO_IVA = Decimal.Parse(param.parametro2);
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.DESC_PROMO_IVA = 0;
                            //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'VOUCHERINSERT_PATH'  en core_parametro.");// + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.DESC_PROMO_IVA = 0;
                    }
                    //FIN parametros de PROMO IVA ref:abc 20220728


                    // inicio parametros de PROMO IVA ref:abc 20220728
                    param = db.core_parametro.Where(x => x.identificador == "PROMO_IVA" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault();
                    if (param != null)
                    {
                        if (!string.IsNullOrEmpty(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.PROMO_IVA = true;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.PROMO_IVA = false;
                            //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'VOUCHERINSERT_PATH'  en core_parametro.");// + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.PROMO_IVA = false;
                    }
                    //FIN parametros de PROMO IVA ref:abc 20220728


                    // inicio parametros de CUPO_CF ref:abc 20220728
                    param = db.core_parametro.Where(x => x.identificador == "CUPO_CF" ).FirstOrDefault();

                    if (param != null)
                    {
                        if (!string.IsNullOrEmpty(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.CUPO_CF = Decimal.Parse(param.valor);
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.CUPO_CF = 200;
                            //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'VOUCHERINSERT_PATH'  en core_parametro.");// + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.CUPO_CF = 200;
                    }
                    //FIN parametros de CUPO_CF ref:abc 20220728

                    // inicio parametros de CUPO_CF ref:abc 20220728
                    param = db.core_parametro.Where(x => x.identificador == "IVA").FirstOrDefault();
                    if (param != null)
                    {
                        if (!string.IsNullOrEmpty(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.IVAGEN = Decimal.Parse(param.valor);
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.IVAGEN = 12;
                            //Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'VOUCHERINSERT_PATH'  en core_parametro.");// + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.IVAGEN = 12;
                    }
                    //FIN parametros de CUPO_CF ref:abc 20220728


                    
                    param = db.core_parametro.Where(x => x.identificador == "IVA_ANTERIOR").FirstOrDefault();
                    if (param != null)
                    {
                        if (!string.IsNullOrEmpty(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.IVA_ANTERIOR  = Decimal.Parse(param.valor);
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.IVA_ANTERIOR = 12;
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.IVA_ANTERIOR = 12;
                    }

                    //Prefijo Usa App Movil                    
                    param = db.core_parametro.Where(x => x.identificador.Equals("APPMOVIL_PREFIJOLECTURAPOS")).FirstOrDefault();

                    if (param != null)
                    {
                        if (!string.IsNullOrWhiteSpace(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp = param.valor;
                        }
                        else
                        {
                            throw new Exception("Campo 'valor' de registro 'APPMOVIL_PREFIJOLECTURAPOS' en core_parametro es incorrecto, no puede ser nulo ni espacios");
                        }
                    }
                    else
                    {
                        throw new Exception("No hay parametro 'APPMOVIL_PREFIJOLECTURAPOS' en core_parametro");
                    }

                    //Venta Giftcard
                    param = db.core_parametro.Where(x => x.identificador.Equals("GIFTCARDVENTA")
                                                    && (x.parametro2 == null || x.parametro2.Contains(Control.Common.GlobalParameters.EstablecimientoAxCode + ";"))).FirstOrDefault();

                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.TienePermisoGiftcardVenta = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.TienePermisoGiftcardVenta = false;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'GIFTCARDVENTA' para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                    }

                    //ciudad de local
                    param = db.core_parametro.Where(x => x.identificador.Equals("CIUDAD_LOCAL")
                                                    &&
                                                    (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";")))
                                             .FirstOrDefault();

                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.CiudadLocal = param.valor;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.CiudadLocal = "";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'CIUDAD_LOCAL' para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                    }

                    //integracion pedidos
                    param = db.core_parametro.Where(x => x.identificador.Equals("INTEGRACION_PEDIDOS")
                                                    &&
                                                    (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";")))
                                             .FirstOrDefault();

                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ActivaIntegracionPedidos = Convert.ToBoolean(param.valor);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "parametro 'INTEGRACION_PEDIDOS' para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? " " : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.ActivaIntegracionPedidos = false;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'INTEGRACION_PEDIDOS' para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                    }

                    //PROMOCION POR VALOR TOTAL DE FACTURA E ITEM PARTICIPANTE
                    param = db.core_parametro.Where(x => x.identificador.Equals("PROMO_FACT_ITEM") && (x.valor == "TRUE") && x.documento.Contains(Control.Common.GlobalParameters.Establecimiento +";"))
                                             .FirstOrDefault();

                    if (param != null)
                    {
                        var valores = param.parametro2.Split('|');
                        POS.Control.Common.GlobalParameters.PromoValorFactItemValor = Convert.ToDecimal(valores[1]);
                        POS.Control.Common.GlobalParameters.PromoValorFactItemIdTick = Convert.ToInt32(valores[0]);
                        POS.Control.Common.GlobalParameters.PromoValorFactItemCF = Convert.ToInt16(valores[2]);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.PromoValorFactItemValor = Convert.ToDecimal("0");
                        POS.Control.Common.GlobalParameters.PromoValorFactItemIdTick = 0;
                        POS.Control.Common.GlobalParameters.PromoValorFactItemCF = 1;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'PROMO_FACT_ITEM' en core_parametro.");
                    }

                    //PARAMETRO PARA VALIDAR LAS IMPRESIONES DE TICKET SIN DETALLES EN LA core_promocionticket_items CUANDO SON PROMOCIONES POR MONTO VENTA
                    param = db.core_parametro.Where(x => x.identificador.Equals("FECHA_INICIO_PROMO_TICKET_SIN_ITEM") && (x.valor == "TRUE") && x.documento.Contains(Control.Common.GlobalParameters.Establecimiento + ";"))
                                             .FirstOrDefault();
                    if (param != null)
                    {
                        
                        if (DateTime.Now >= DateTime.Parse(param.parametro2))
                            POS.Control.Common.GlobalParameters.PermitirPromocionTicketSinItem = true;
                        else
                            POS.Control.Common.GlobalParameters.PermitirPromocionTicketSinItem = false;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.PermitirPromocionTicketSinItem = false;
                    }
                        //Iniciar el sensor de inactividad
                    POS.Control.Security.InactivityChecker.RestartSensor();

                    var paramAmbienteProduccion = db.core_parametro.Where(x => x.identificador.Equals("AMBIENTEPRODUCCION")).FirstOrDefault();

                    if (paramAmbienteProduccion == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No está configurado el parametro AMBIENTEPRODUCCION configurado en la tabla core_parametro");
                        //Si una base no tiene configurado el parametro, la consideraremos por defecto PRODUCCION
                        Control.Common.GlobalParameters.EsAmbienteProduccion = true;
                    }
                    else
                    {
                        Control.Common.GlobalParameters.EsAmbienteProduccion = paramAmbienteProduccion.valor == "TRUE" ? true : false;
                        Control.Common.GlobalParameters.ListaIpPermitidasAmbienteDev = (string.IsNullOrWhiteSpace(paramAmbienteProduccion.parametro2) ? string.Empty : paramAmbienteProduccion.parametro2).Split(';').ToList();
                    }

                    //Validar permisos de ambiente
                    var environmentSecurity = new POS.Control.Security.Environment();
                    environmentSecurity.CheckIpRunPermission();
                    environmentSecurity.CheckCompletedChecklists();

                    //POSVOUCHER_INSERT. //eevv 2020-01-13
                    if (db.core_parametro.Any(x => x.identificador.Equals("VOUCHERINSERT_PATH")))
                    {
                        param = db.core_parametro.Where(x => x.identificador.Equals("VOUCHERINSERT_PATH"))
                                                .FirstOrDefault();

                        if (!string.IsNullOrEmpty(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.VoucherInsertPath = param.valor;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.VoucherInsertPath = "";

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'VOUCHERINSERT_PATH'  en core_parametro.");// + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                        }


                    }
                    //valida cash back
                    param = db.core_parametro.Where(x => x.identificador.Equals("VALIDA_CASHBACK") && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento) && x.valor == "TRUE").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ValidaCashBack = "OK";
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.ValidaCashBack = "";
                    }
                    //llenar lista de retiva
                    param = db.core_parametro.Where(x => x.identificador.Equals("RET_IVA") && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento) && x.valor == "TRUE").FirstOrDefault();
                    if (param != null)
                    {
                        param = db.core_parametro.Where(x => x.identificador.Contains("RET_IVA_DETALLE")).FirstOrDefault();
                        if (param != null)
                        {
                            POS.Control.Common.GlobalParameters.RetIvaRuc = param.valor;
                            POS.Control.Common.GlobalParameters.RetIvaTipo = param.parametro2;
                            POS.Control.Common.GlobalParameters.RetIvaLeyenda = param.documento;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.RetIvaRuc = "";
                            POS.Control.Common.GlobalParameters.RetIvaTipo = "";
                            POS.Control.Common.GlobalParameters.RetIvaLeyenda = "";
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'RET_IVA_DETALLE'  en core_parametro.");
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.RetIvaRuc = "";
                        POS.Control.Common.GlobalParameters.RetIvaTipo = "";
                        POS.Control.Common.GlobalParameters.RetIvaLeyenda = "";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'RET_IVA' en core_parametro, para este local.");
                    }
                    //PATH LISTA PRODUCTOS
                    param = db.core_parametro.Where(x => x.identificador.Equals("PATH_PRODUCT_" + Control.Common.GlobalParameters.EstablecimientoAxCode))
                                            .FirstOrDefault();
                    if (param != null)
                    {
                        if (!string.IsNullOrWhiteSpace(param.valor) && !string.IsNullOrWhiteSpace(param.parametro2))
                        {
                            POS.Control.Common.GlobalParameters.ListaProductos_Path = param.valor;
                            POS.Control.Common.GlobalParameters.ListaProductos_Path_Valida = param.parametro2;
                            POS.Control.Common.GlobalParameters.ListaProductos_File = param.documento;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.ListaProductos_Path = "";
                            POS.Control.Common.GlobalParameters.ListaProductos_Path_Valida = "";
                            POS.Control.Common.GlobalParameters.ListaProductos_File = "";
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.ListaProductos_Path = "";
                        POS.Control.Common.GlobalParameters.ListaProductos_Path_Valida = "";
                        POS.Control.Common.GlobalParameters.ListaProductos_File = "";
                    }

                    //CHECK LIST REVISIONES RETENCION
                    param = db.core_parametro.Where(x => x.identificador.Equals("CHECK_RET") && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();
                    if (param != null)
                    {

                        POS.Control.Common.GlobalParameters.checkRevisionesRet = param.valor;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.checkRevisionesRet = "";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'CHECK_RET'  en core_parametro.");
                    }

                    //CHECK LIST REVISION GENERAL DE RETENCION
                    param = db.core_parametro.Where(x => x.identificador.Equals("CHECK_RET_G") && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();
                    if (param != null)
                    {

                        POS.Control.Common.GlobalParameters.checkRevisionGeneralRet = param.valor;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.checkRevisionGeneralRet = "";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'CHECK_RET_G'  en core_parametro.");
                    }

                    //CLAVE ACCESO
                    param = db.core_parametro.Where(x => x.identificador.Equals("CLAVE_ACCESO")).FirstOrDefault();
                    if (param != null)
                    {
                        if (!string.IsNullOrWhiteSpace(param.valor) && !string.IsNullOrWhiteSpace(param.parametro2) && !string.IsNullOrWhiteSpace(param.documento))
                        {
                            POS.Control.Common.GlobalParameters.NRuc = param.valor;
                            POS.Control.Common.GlobalParameters.TipoAmbiente = param.parametro2;
                            POS.Control.Common.GlobalParameters.TipoEmision = param.documento;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.NRuc = "";
                            POS.Control.Common.GlobalParameters.TipoAmbiente = "";
                            POS.Control.Common.GlobalParameters.TipoEmision = "";

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'CLAVE_ACCESO'  en core_parametro.");
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.NRuc = "";
                        POS.Control.Common.GlobalParameters.TipoAmbiente = "";
                        POS.Control.Common.GlobalParameters.TipoEmision = "";

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'CLAVE_ACCESO'  en core_parametro.");
                    }


                    //APLICA PROMO MAYOR 
                    param = db.core_parametro.Where(x => x.identificador == "APLICA_PROMO_MAYOR"
                                                            &&
                                                            (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento))
                                                            ).FirstOrDefault();

                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.AplicaPromoMayor = param.valor;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.AplicaPromoMayor = "FALSE";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'APLICA_PROMO_MAYOR' para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                    }


                    //DB IdCaja. JM 11-09-2019
                    param = db.core_parametro.Where(x => x.identificador.Equals("DB_IDCAJA_" + Control.Common.GlobalParameters.EstablecimientoAxCode))
                                                .FirstOrDefault();

                    if (param != null)
                    {
                        if (string.IsNullOrEmpty(IPServidorConectado))
                        {
                            POS.Control.Common.GlobalParameters.DBIdCaja = param.valor;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.DBIdCaja = param.valor.ToString().ToLower().Replace("srv-pos", IPServidorConectado);
                        }

                        POS.Control.Common.GlobalParameters.DBIdCajaLocal = param.parametro2;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.DBIdCaja = "";
                        POS.Control.Common.GlobalParameters.DBIdCajaLocal = "";

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'DB_IDCAJA_'" + Control.Common.GlobalParameters.EstablecimientoAxCode + " para este local (" + (string.IsNullOrWhiteSpace(Control.Common.GlobalParameters.EstablecimientoAxCode) ? "La variable estática Control.Common.GlobalParameters.EstablecimientoAxCode tiene valor nulo o solo contiene espacios en blanco" : Control.Common.GlobalParameters.EstablecimientoAxCode) + ") en core_parametro.");
                    }

                    POS.Control.Common.GlobalParameters.ActivarFormaPagoTC = false;
                    if (db.core_parametro.Any(x => x.identificador.Equals("ACTIVAR_TRAMA_TC")))
                    {
                        param = db.core_parametro.Where(x => x.identificador.Equals("ACTIVAR_TRAMA_TC")).FirstOrDefault();
                        if (param.valor.ToLower().Equals("true"))
                        {
                            POS.Control.Common.GlobalParameters.ActivarFormaPagoTC = true;
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.ActivarFormaPagoTC = false;
                        }
                    }

                    //Eliminar items por local Usr o Adm. JM 25-06-2020
                    param = db.core_parametro.Where(x => x.identificador == "DELETE_PRODUCT_USR_ADM" && x.parametro2 == establecimiento_inicio)
                                            .FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.DeleteProductUsrAdm = param.valor;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.DeleteProductUsrAdm = "";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'DELETE_PRODUCT_USR_ADM'  en core_parametro para este local " + establecimiento_inicio);
                    }

                    //Si esta activo Compra Gratis.     JM 30-06-2020
                    param = db.core_parametro.Where(x => x.identificador == "COMPRA_GRATIS")
                                            .FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.CompraGratis = (param.valor == "TRUE" ? true : false);
                        if (POS.Control.Common.GlobalParameters.CompraGratis)
                        {
                            core_parametro fecha_inicio = db.core_parametro.Where(x => x.identificador == "COMPRA_GRATIS_FECHA_INICIO_ACUMULA").FirstOrDefault();
                            core_parametro fecha_fin = db.core_parametro.Where(x => x.identificador == "COMPRA_GRATIS_FECHA_FIN_CONSUMO").FirstOrDefault();
                            if (!(DateTime.Now >= DateTime.Parse(fecha_inicio.valor) && DateTime.Now <= DateTime.Parse(fecha_fin.valor)))
                                POS.Control.Common.GlobalParameters.CompraGratis = false;
                        }
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'COMPRA_GRATIS'  en core_parametro ");
                    }

                    //Etiqueta recibo con forma pago Monedero. JM 10-07-2020
                    param = db.core_parametro.Where(x => x.identificador == "MONEDERO_ETIQUETA_RECIBO")
                                            .FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.MonederoEtiquetaRecibo = param.valor;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.MonederoEtiquetaRecibo = "DINERO ELECTRONICO";
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'MONEDERO_ETIQUETA_RECIBO'  en core_parametro ");
                    }

                    //Porcentaje descuento con forma pago Monedero. JM 10-07-2020
                    param = db.core_parametro.Where(x => x.identificador == "MONEDERO_PORCENTAJE_CONSUMO")
                                            .FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.MonederoPorcentajeConsumo = decimal.Parse(param.valor);
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'MONEDERO_PORCENTAJE_CONSUMO'  en core_parametro ");
                    }

                    //Periodo de Consumo del Monedero. JM 21-07-2020
                    var paramfecini = db.core_parametro.Where(x => x.identificador == "MONEDERO_FECHA_INICIO_CONSUMO").FirstOrDefault();
                    var paramfecfin = db.core_parametro.Where(x => x.identificador == "MONEDERO_FECHA_FIN_CONSUMO").FirstOrDefault();
                    if (paramfecini != null && paramfecfin != null)
                    {
                        if ((DateTime.Now >= DateTime.Parse(paramfecini.valor) && DateTime.Now <= DateTime.Parse(paramfecfin.valor)))
                            POS.Control.Common.GlobalParameters.MonederoConsumoActivo = true;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.MonederoConsumoActivo = false;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "No hay parametro 'MONEDERO_FECHA_INICIO_CONSUMO' 'MONEDERO_FECHA_FIN_CONSUMO'  en core_parametro ");
                    }


                    var PinpadKeyDerecho = db.core_parametro.Where(x => x.identificador == "PINPADKEYDERECHO").FirstOrDefault();
                    if (PinpadKeyDerecho != null)
                    {
                        POS.Control.Common.GlobalParameters.PinpadKeyDerechoTCPIP = PinpadKeyDerecho.valor.Trim();
                    }

                    var PinpadKeIzquierdo = db.core_parametro.Where(x => x.identificador == "PINPADKEYIZQUIERDA").FirstOrDefault();
                    if (PinpadKeIzquierdo != null)
                    {
                        POS.Control.Common.GlobalParameters.PinpadKeyIzquierdoTCPIP = PinpadKeIzquierdo.valor.Trim();
                    }

                    param = db.core_parametro.Where(x => x.identificador == "ACTIVA_VERSION_PINPAD_MEDIANET").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ActivaVersionPinPadMedianet = (param.valor == "TRUE" ? true : false);
                    }

                    param = db.core_parametro.Where(x => x.identificador == "PINPAD_RECEIVE_TIMEOUT").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.PinPadReceiveTimeout = int.Parse(param.valor);
                    }

                    param = db.core_parametro.Where(x => x.identificador == "CONSUMOBILLETERAINSERT_PATH").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ConsumoBilleteraInsertPath = param.valor.Trim();
                    }
                    param = db.core_parametro.Where(x => x.identificador == "CONSUMOGIFTCARDINSERT_PATH").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ConsumoGiftCardInsertPath = param.valor.Trim();
                    }
                    param = db.core_parametro.Where(x => x.identificador == "CONSUMOCUPONAPPINSERT_PATH").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ConsumoCuponAppInsertPath = param.valor.Trim();
                    }
                    param = db.core_parametro.Where(x => x.identificador == "CON_SERVER_PUNTOS").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ConServerPuntos = param.valor.Trim();
                    }


                    param = db.core_parametro.Where(x => x.identificador == "VIGENCIA_IVA12_SEGUN_FECHA_NC").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.VALIDAR_VIGENCIA_IVA12 = bool.Parse(param.valor);
                        if (bool.Parse(param.valor))
                        {
                            POS.Control.Common.GlobalParameters.VIGENCIA_IVA12_SEGUN_FECHA_NC = DateTime.Parse(param.parametro2);
                        }

                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.VALIDAR_VIGENCIA_IVA12 = false;
                    }
                    param = db.core_parametro.Where(x => x.identificador == "BUSCAR_DETALLE_IVA12_NC").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.BUSCAR_DETALLE_IVA12_NC = bool.Parse(param.valor);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.BUSCAR_DETALLE_IVA12_NC = false;
                    }



                    param = db.core_parametro.Where(x => x.identificador.Equals("BTNPROVEEDOR") && (x.valor == "TRUE") && (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento))).FirstOrDefault();

                    if (param != null)
                    {
                        var valores = param.documento.Split('|');

                        if (valores.Count() != 1)
                        {
                            POS.Control.Common.GlobalParameters.Labelbtn3Proveedor = valores[0];
                            POS.Control.Common.GlobalParameters.Linkbtn3Proveedor = valores[1];
                        }
                        else
                        {
                            POS.Control.Common.GlobalParameters.Labelbtn3Proveedor = "Garancheck";
                            POS.Control.Common.GlobalParameters.Linkbtn3Proveedor = "http://www.garancheck.com.ec/Cheques/Account/Login.aspx";
                        }
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.Labelbtn3Proveedor = "Garancheck";
                        POS.Control.Common.GlobalParameters.Linkbtn3Proveedor = "http://www.garancheck.com.ec/Cheques/Account/Login.aspx";
                    }

                    param = db.core_parametro.Where(x => x.identificador.Equals("MONTO_VOUCHER_TARJETA_CREDITO_SIN_FIRMA") && (x.valor == "TRUE")).FirstOrDefault();

                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma = decimal.Parse(param.parametro2);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma = 0;
                    }

                    param = db.core_parametro.Where(x => x.identificador.Equals("COPIA_VOUCHER_TARJETA_CREDITO_SIN_FIRMA")).FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.CopiaVoucherTarjetaCreditoSinFirma = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.CopiaVoucherTarjetaCreditoSinFirma = false;
                    }



                    param = db.core_parametro.Where(x => x.identificador == "POSQUITARTOPMOST").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.PosQuitaTopMost = bool.Parse(param.valor);
                    }


                    param = db.core_parametro.Where(x => x.identificador == "XULRUNNER_PATH").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.xulrunnerPath = param.valor.Trim();
                    }

                    param = db.core_parametro.Where(x => x.identificador == "FORMA_PAGO_RETENCION").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.VisibleFormaPagoRetencion = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.VisibleFormaPagoRetencion = true;
                    }

                    param = db.core_parametro.Where(x => x.identificador == "DEV_DINERO_RETENCION_FISICA").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.VisibleDevolucionDineroRetencionFisica = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.VisibleDevolucionDineroRetencionFisica = true;
                    }

                    param = db.core_parametro.Where(x => x.identificador == "PRESENTAR_MSG_SERVIDOR_INCORRECTO" && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.PresentarMensajeServidorIncorrecto = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.PresentarMensajeServidorIncorrecto = false;
                    }


                    param = db.core_parametro.Where(x => x.identificador == "PREGUNTAR_IMPRIME_DATO_RETENCION" && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.PreguntarSiImprimeDatoRetencion = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.PreguntarSiImprimeDatoRetencion = false;
                    }

                    param = db.core_parametro.Where(x => x.identificador == "IMPRIME_DATO_RETENCION" && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.ImprimeDatoRetencion = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.ImprimeDatoRetencion = false;
                    }

                    param = db.core_parametro.Where(x => x.identificador == "BLOQUEO_RETENCION_FUENTE" && x.valor == "TRUE").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.BloqRetFte = (param.valor == "TRUE" ? true : false);
                        POS.Control.Common.GlobalParameters.BloqRetFteCodigo = param.parametro2;
                        POS.Control.Common.GlobalParameters.BloqRetFteMsj = param.documento;
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.BloqRetFte = false;
                        POS.Control.Common.GlobalParameters.BloqRetFteCodigo = "";
                        POS.Control.Common.GlobalParameters.BloqRetFteMsj = "";
                    }

                    param = db.core_parametro.Where(x => x.identificador == "BLOQUEO_RETENCION_FUENTE_ANIO" && x.valor == "TRUE").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.BloqRetFteAnio = (param.parametro2 == null ? 0 : int.Parse(param.parametro2));
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.BloqRetFteAnio = 0;
                    }



                    /**/
                    var WallPaper = db.core_parametro.Where(x => x.identificador.Equals("WALLPAPER_" + Control.Common.GlobalParameters.EstablecimientoAxCode))
                                            .FirstOrDefault();

                    RutaImagen rutaImagen = new RutaImagen();
                    List<RutaImagen> listRutaImagen = new List<RutaImagen>();
                    POS.Control.Common.GlobalParameters.ListWallPapers = new List<RutaImagen>();

                    if (WallPaper != null) {
                        var DetWallPaper = (from deta in db.core_parametro
                                             where deta.identificador == "DETWALLPAPER"
                                             && deta.valor == WallPaper.id.ToString()
                                             select new {
                                                 Establecimiento = Control.Common.GlobalParameters.Establecimiento,
                                                 EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode,
                                                 Ruta = deta.parametro2
                                             }).ToList();

                        foreach (var deta in DetWallPaper) {
                            rutaImagen = new RutaImagen();
                            rutaImagen.Establecmiento = deta.Establecimiento;
                            rutaImagen.Ruta = deta.Ruta;
                            rutaImagen.EstablecimientoAxCode = deta.EstablecimientoAxCode;
                            listRutaImagen.Add(rutaImagen);
                        }
                        //POS.Control.Common.GlobalParameters.ListWallPapers.AddRange(DetWallPapaer);
                    }
                    else
                    {
                        rutaImagen = new RutaImagen();
                        rutaImagen.Establecmiento = Control.Common.GlobalParameters.Establecimiento;
                        rutaImagen.EstablecimientoAxCode = Control.Common.GlobalParameters.EstablecimientoAxCode;
                        rutaImagen.Ruta = @"\\srvatila\Shares\Compartido\ImgPOS\Wallpaper.jpg";
                        listRutaImagen.Add(rutaImagen);
                    }

                    POS.Control.Common.GlobalParameters.ListWallPapers = listRutaImagen;
                   
                    ///**/
                    //param = db.core_parametro.Where(x => x.identificador.Equals("WALLPAPER_" + Control.Common.GlobalParameters.EstablecimientoAxCode))
                    //                        .FirstOrDefault();
                    //if (param != null)
                    //{
                    //    POS.Control.Common.GlobalParameters.WallpaperLocal = param.valor;
                    //}
                    //else
                    //{
                    //    POS.Control.Common.GlobalParameters.WallpaperLocal = @"\\srvatila\Shares\Compartido\ImgPOS\Wallpaper.jpg";
                    //}

                    param = db.core_parametro.Where(x => x.identificador.Equals("MONTO_DIFERENCIA_PERMITIDA_DEV_RETENCION") && (x.valor == "TRUE")).FirstOrDefault();

                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.MontoDiferenciaPermitidaEnDevolucionRetencion = decimal.Parse(param.parametro2);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.MontoVoucherTarjetaCreditoSinFirma = 0;
                    }

                    param = db.core_parametro.Where(x => x.identificador == "NC_CONSUMIDORFINAL").FirstOrDefault();
                    if (param != null)
                    {
                        POS.Control.Common.GlobalParameters.NCConsumidorFinal = (param.valor == "TRUE" ? true : false);
                    }
                    else
                    {
                        POS.Control.Common.GlobalParameters.NCConsumidorFinal = false;
                    }

                    //obtiene ipserver pedidosapp

                    param = db.core_parametro.Where(x => x.identificador == "PEDIDOSAPP_IPSERVER" && x.documento.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();

                    if (param != null)
                    {
                        if (string.IsNullOrEmpty(param.parametro2))
                        {
                            POS.Control.Common.GlobalParameters.srvPrincipal = "FALSE";
                            //throw new Exception("Valor parametro2 de 'PEDIDOSAPP_IPSERVER' esta vacio, por favor configurar");                            
                        }
                        else
                        {
                            var credentials = param.parametro2.Split('|');

                            if (credentials.Count() != 1)
                            {
                                throw new Exception("Valor parametro2 de 'PEDIDOSAPP_IPSERVER' no tiene el formato correcto '{usrSql}|{pswSql}', por favor configurar");
                            }
                            POS.Control.Common.GlobalParameters.ipServerPedAPP = param.valor;
                            //POS.Control.Common.GlobalParameters.UserServerPedAPP = credentials[0];
                            //POS.Control.Common.GlobalParameters.PaswServerPedAPP = credentials[1];
                            POS.Control.Common.GlobalParameters.srvPrincipal = credentials[0];

                            System.Configuration.Configuration appconfig =
                            System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);//System.Reflection.Assembly.GetEntryAssembly().Location/ConfigurationUserLevel.None);// PerUserRoamingAndLocal);//Application.ExecutablePath);// 
                            var cadena = appconfig.ConnectionStrings.ConnectionStrings["POSEntities"].ConnectionString;

                            cadena = cadena.Split('\"')[1];

                            string[] credenciales = cadena.Split(';');
                            foreach (var s in credenciales)
                            {
                                string[] valores = s.Split('=');
                                switch (valores[0].ToUpper())
                                {
                                    case "DATA SOURCE":
                                        POS.Control.Common.GlobalParameters.ipServerSelect = valores[1];
                                        break;
                                        //case "USER ID":
                                        //    POS.Control.Common.GlobalParameters.UserServerSelect = valores[1];
                                        //    break;
                                        //case "PASSWORD":
                                        //    POS.Control.Common.GlobalParameters.PaswServerSelect = valores[1];
                                        //    break;
                                }
                            }
                        }
                    }

                    Control.Common.GlobalParameters.AutorizadorDefault = 2;
                    var AutorizaDefault = (from deta in db.core_parametro
                                           where deta.identificador == "AUTORIZAODR_PINPAD_DEFAULT"
                                           select deta).ToList();

                    if (AutorizaDefault.Count > 0)
                    {
                        Control.Common.GlobalParameters.AutorizadorDefault = Int32.Parse(AutorizaDefault.FirstOrDefault().valor);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargarMainWindow", "Recarga Parametros Generales: AUTORIZAODR_PINPAD_DEFAULT: Autorizador: " + Control.Common.GlobalParameters.AutorizadorDefault.ToString());
                    }



                    /*Si el valor es TRUE, utiliza*/
                    Control.Common.GlobalParameters.USA_BALANZA = true;
                    param = db.core_parametro.Where(x => x.identificador == "PESO_MANUAL" && x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento)).FirstOrDefault();
                    if (param != null)
                    {
                        if (param.valor == "FALSE") { Control.Common.GlobalParameters.USA_BALANZA = false; }
                        
                    }
                   


                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosGenerales", "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. A continuacion las excepciones encontradas " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                Control.Common.General.GetMensajeToList(153);
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        public void RecargarParametrosParkingV2()
        {

            string establecimiento = string.Empty;
            string query = string.Empty;
            DataSet dtsConsulta = new DataSet();

            try
            {

                establecimiento = Control.Common.GlobalParameters.Establecimiento;

                query = string.Empty;
                query = string.Concat(query, $"Exec spConsultaParametroParking", Environment.NewLine);
                query = string.Concat(query, $" @Establecimiento = '{establecimiento}' ", Environment.NewLine);
                dtsConsulta = Control.Common.General.GetDataSet(query);

                if (dtsConsulta.Tables.Count > 0) {

                    for (int index = 0; index <= dtsConsulta.Tables.Count - 1; index++)
                    {
                        string tipoConsulta = string.Empty;
                        if (dtsConsulta.Tables[index].Rows.Count > 0)
                        {
                            tipoConsulta = dtsConsulta.Tables[index].Rows[0]["tipoConsulta"].ToString();

                            if (tipoConsulta == "PARKING_CONFIG")
                            {
                                foreach (DataRow dataConfig in dtsConsulta.Tables[index].Rows)
                                {
                                    bool Parking_TienePermiso = (bool)dataConfig["Parking_TienePermiso"];

                                    if (Parking_TienePermiso)
                                    {
                                        Control.Common.GlobalParameters.Parking_TienePermiso = Parking_TienePermiso;
                                        btnParqueo.Visible = Control.Common.GlobalParameters.Parking_TienePermiso;


                                        string Parking_PathIngreso = (string)dataConfig["Parking_PathIngreso"];

                                        if (string.IsNullOrEmpty(Parking_PathIngreso))
                                        {
                                            throw new Exception("Campo 'Parking_PathIngreso' de registro 'spConsultaParametroParking', está vacío y no contiene la ruta de ficheros de ingresos");
                                        }

                                        Control.Common.GlobalParameters.Parking_PathIngreso = Parking_PathIngreso;

                                        string Parking_WSSalida = (string)dataConfig["Parking_WSSalida"];
                                        if (string.IsNullOrEmpty(Parking_WSSalida))
                                        {
                                            throw new Exception("Campo 'Parking_WSSalida' de registro 'spConsultaParametroParking', está vacío y no contiene la ruta de ficheros de ingresos");
                                        }

                                        Control.Common.GlobalParameters.Parking_WSSalida = Parking_WSSalida;

                                        string Parking_PathSalida = (string)dataConfig["Parking_PathSalida"];
                                        if (string.IsNullOrEmpty(Parking_WSSalida))
                                        {
                                            throw new Exception("Campo 'Parking_PathSalida' de registro 'spConsultaParametroParking', está vacío y no contiene la ruta de ficheros de ingresos");
                                        }


                                        Control.Common.GlobalParameters.Parking_PathSalida = Parking_PathSalida;

                                        string Parking_WSGetTicket = (string)dataConfig["Parking_WSGetTicket"];
                                        if (string.IsNullOrEmpty(Parking_WSSalida))
                                        {
                                            throw new Exception("Campo 'Parking_WSGetTicket' de registro 'spConsultaParametroParking', está vacío y no contiene la ruta de ficheros de ingresos");
                                        }

                                        Control.Common.GlobalParameters.Parking_WSGetTicket = Parking_WSGetTicket;


                                        string Parking_HoraDesde = (string)dataConfig["Parking_HoraDesde"];
                                        if (string.IsNullOrEmpty(Parking_HoraDesde))
                                        {
                                            throw new Exception("Campo 'Parking_HoraDesde' de registro 'spConsultaParametroParking', está vacío y no contiene la valor de Hora Desde");
                                        }

                                        Control.Common.GlobalParameters.Parking_HoraDesde = Parking_HoraDesde;

                                        string Parking_HoraHasta = (string)dataConfig["Parking_HoraHasta"];
                                        if (string.IsNullOrEmpty(Parking_HoraDesde))
                                        {
                                            throw new Exception("Campo 'Parking_HoraHasta' de registro 'spConsultaParametroParking', está vacío y no contiene la valor de Hora Hasta");
                                        }

                                        Control.Common.GlobalParameters.Parking_HoraHasta = Parking_HoraHasta;

                                        int Parking_MinutosGracia = (int)dataConfig["Parking_MinutosGracia"];
                                        if (string.IsNullOrEmpty(Parking_MinutosGracia.ToString()) || Parking_MinutosGracia == 0)
                                        {
                                            throw new Exception("Campo 'Parking_MinutosGracia' de registro 'spConsultaParametroParking', está vacío o el valor es igual a CERO");
                                        }


                                        Control.Common.GlobalParameters.Parking_MinutosGracia = Parking_MinutosGracia;

                                        string Parking_ItemPerdidaTicket = (string)dataConfig["Parking_ItemPerdidaTicket"];
                                        if (string.IsNullOrEmpty(Parking_ItemPerdidaTicket))
                                        {
                                            throw new Exception("Campo 'Parking_ItemPerdidaTicket' de registro 'spConsultaParametroParking', está vacio o no contiene un valor correcto");
                                        }
                                        Control.Common.GlobalParameters.Parking_ItemPerdidaTicket = Parking_ItemPerdidaTicket;

                                        string Parking_ReciboPerdida = (string)dataConfig["Parking_ReciboPerdida"];
                                        if (string.IsNullOrEmpty(Parking_ReciboPerdida))
                                        {
                                            throw new Exception("Campo 'Parking_ReciboPerdida' de registro 'spConsultaParametroParking', está vacio o no contiene un valor correcto");
                                        }

                                        Control.Common.GlobalParameters.Parking_ReciboPerdida = Parking_ReciboPerdida;

                                        string ParkingReciboPerdidaLeyenda = (string)dataConfig["ParkingReciboPerdidaLeyenda"];
                                        if (string.IsNullOrEmpty(ParkingReciboPerdidaLeyenda))
                                        {
                                            throw new Exception("Campo 'ParkingReciboPerdidaLeyenda' de registro 'spConsultaParametroParking', está vacio o no contiene un valor correcto");
                                        }
                                        Control.Common.GlobalParameters.ParkingReciboPerdidaLeyenda = ParkingReciboPerdidaLeyenda;
                                    }

                                }
                                continue;
                            }

                            if (tipoConsulta == "PARKING_MINUTOSFRACCION")
                            {
                                Control.Common.GlobalParameters.Parking_ListaItemParqueo = new List<string>();
                                foreach (DataRow dataConfig in dtsConsulta.Tables[index].Rows)
                                {
                                    string ITEMID = (string)dataConfig["ItemId"];
                                    Control.Common.GlobalParameters.Parking_ListaItemParqueo.Add(ITEMID);
                                }
                                continue;
                            }

                            if (tipoConsulta == "PARKING_ITEMPERDIDATICKET")
                            {
                                Control.Common.GlobalParameters.Parking_ListaItemPerdida = new List<string>();
                                foreach (DataRow dataConfig in dtsConsulta.Tables[index].Rows)
                                {
                                    string ITEMID = (string)dataConfig["ItemId"];
                                    Control.Common.GlobalParameters.Parking_ListaItemPerdida.Add(ITEMID);
                                }
                                continue;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosParkingV2", $"Exception {ex.Message}");
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosParkingV2", "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. A continuacion las excepciones encontradas " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                Control.Common.General.GetMensajeToList(153);

                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }

        }


        private void RecargarParametrosParking()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //------------------------PARKING ACTIVO--------------------------------------------------------
                    //var param = db.core_parametro.Where(x => x.identificador == "PARKING_ACTIVO"
                    //                                     && (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.parametro2 == null)
                    //                               )
                    //                         .FirstOrDefault();
                    
                    //Valido  si el parametro de parking se encuentra activo. 
                    var param = (from deta in db.core_parametro
                                        where deta.identificador == "PARKING_ACTIVO"
                                        && deta.valor == "TRUE"
                                        && (deta.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || deta.parametro2 == null)
                                        select deta).ToList().FirstOrDefault();


                    if (param == null)
                    {
                        Control.Common.GlobalParameters.Parking_TienePermiso = false;
                    }
                    else
                    {
                        Control.Common.GlobalParameters.Parking_TienePermiso = param.valor == "TRUE";
                    }

                    btnParqueo.Visible = Control.Common.GlobalParameters.Parking_TienePermiso;

                    //------------------------LISTADO TODOS ITEMID PARQUEO--------------------------------------------------------
                    var listaParam = db.core_parametro.Where(x => x.identificador.Contains("PARKING_MINUTOSFRACCION")).ToList();
                    Control.Common.GlobalParameters.Parking_ListaItemParqueo = new List<string>();
                    foreach (var parametro in listaParam)
                    {
                        Control.Common.GlobalParameters.Parking_ListaItemParqueo.Add(parametro.parametro2);
                    }

                    //------------------------LISTADO TODOS ITEMID PERDIDA--------------------------------------------------------
                    listaParam = db.core_parametro.Where(x => x.identificador == "PARKING_ITEMPERDIDATICKET").ToList();
                    Control.Common.GlobalParameters.Parking_ListaItemPerdida = new List<string>();
                    foreach (var parametro in listaParam)
                    {
                        Control.Common.GlobalParameters.Parking_ListaItemPerdida.Add(parametro.valor);
                    }

                    if (Control.Common.GlobalParameters.Parking_TienePermiso)
                    {

                        //------------------------PATH INGRESO--------------------------------------------------------
                        param = db.core_parametro.Where(x => x.identificador == "PARKING_PATHINGRESO" && x.valor == Control.Common.GlobalParameters.Establecimiento).FirstOrDefault();
                        if (param == null)
                        {
                            throw new Exception("No hay parametro 'PARKING_PATHINGRESO' en core_parametro");
                        }

                        if (string.IsNullOrEmpty(param.parametro2))
                        {
                            throw new Exception("Campo 'parametro2' de registro 'PARKING_PATHINGRESO' en core_parametro está vacío y no contiene la ruta de ficheros de ingresos");
                        }

                        Control.Common.GlobalParameters.Parking_PathIngreso = param.parametro2;

                        //------------------------PATH SALIDA--------------------------------------------------------
                        param = db.core_parametro.Where(x => x.identificador == "PARKING_PATHSALIDA" && x.valor == Control.Common.GlobalParameters.Establecimiento).FirstOrDefault();
                        if (param == null)
                        {
                            throw new Exception("No hay parametro 'PARKING_PATHSALIDA' en core_parametro");
                        }

                        if (string.IsNullOrEmpty(param.parametro2))
                        {
                            throw new Exception("Campo 'parametro2' de registro 'PARKING_PATHSALIDA' en core_parametro está vacío y no contiene la ruta de ficheros de salidas");
                        }

                        if (param.parametro2 == "API")
                        {
                            if (string.IsNullOrEmpty(param.documento))
                            {
                                throw new Exception("Campo 'documento' de registro 'PARKING_PATHSALIDA' en core_parametro está vacío y no contiene la url del web service ficheros de ingresos");
                            }
                            Control.Common.GlobalParameters.Parking_WSSalida = param.documento;
                        }

                        Control.Common.GlobalParameters.Parking_PathSalida = param.parametro2;


                        if (Control.Common.GlobalParameters.Parking_PathSalida=="API")
                        {
                            param = db.core_parametro.Where(x => x.identificador == "PARKING_APIGENERATICKET" && x.valor == Control.Common.GlobalParameters.Establecimiento).FirstOrDefault();
                            if (param == null)
                            {
                                throw new Exception("No hay parametro 'PARKING_APIGENERATICKET' en core_parametro");
                            }

                            if (string.IsNullOrEmpty(param.parametro2))
                            {
                                throw new Exception("Campo 'parametro2' de registro 'PARKING_APIGENERATICKET' en core_parametro está vacío y no contiene la ruta de ficheros de salidas");
                            }

                            Control.Common.GlobalParameters.Parking_WSGetTicket = param.parametro2;
                        }



                        //------------------------HORARIO--------------------------------------------------------
                        param = db.core_parametro.Where(x => x.identificador == "PARKING_HORARIO"
                                                             &&
                                                             (x.documento.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                       )
                                                 .FirstOrDefault();
                        if (param == null)
                        {
                            throw new Exception("No hay parametro 'PARKING_HORARIO' en core_parametro para este establecimiento");
                        }

                        TimeSpan _horarioDesde;
                        if (!TimeSpan.TryParse(param.valor, out _horarioDesde))
                        {
                            throw new Exception("Campo 'valor' de registro 'PARKING_HORARIO' en core_parametro es incorrecto y no se puede parsear a timespan");
                        }
                        Control.Common.GlobalParameters.Parking_HoraDesde = param.valor;

                        TimeSpan _horarioHasta;
                        if (!TimeSpan.TryParse(param.parametro2, out _horarioHasta))
                        {
                            throw new Exception("Campo 'parametro2' de registro 'PARKING_HORARIO' en core_parametro es incorrecto y no se puede parsear a timespan");
                        }
                        Control.Common.GlobalParameters.Parking_HoraHasta = param.parametro2;

                        //------------------------TIEMPO GRACIA DEFAULT--------------------------------------------------------
                        param = db.core_parametro.Where(x => x.identificador == "PARKING_MINUTOSGRACIA"
                                                             &&
                                                             (x.documento.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                       )
                                                 .FirstOrDefault();
                        if (param == null)
                        {
                            throw new Exception("No hay parametro 'PARKING_MINUTOSGRACIA' en core_parametro para este establecimiento");
                        }

                        int _tiempoDefaultGracia = 0;
                        if (!int.TryParse(param.valor, out _tiempoDefaultGracia))
                        {
                            throw new Exception("Campo 'valor' de registro 'PARKING_MINUTOSGRACIA' en core_parametro es incorrecto y no se puede parsear a entero");
                        }

                        if (_tiempoDefaultGracia < 0)
                        {
                            throw new Exception("Parametro 'PARKING_MINUTOSGRACIA' tiene configurado un valor incorrecto en columna 'valor'. Valor debe ser mayor o igual a 0");
                        }

                        Control.Common.GlobalParameters.Parking_MinutosGracia = _tiempoDefaultGracia;

                        //------------------------ITEMID PERDIDA--------------------------------------------------------
                        param = db.core_parametro.Where(x => x.identificador == "PARKING_ITEMPERDIDATICKET"
                                                             &&
                                                             (x.documento.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.documento == null)
                                                       )
                                                 .FirstOrDefault();
                        if (param == null)
                        {
                            throw new Exception("No hay parametro 'PARKING_ITEMPERDIDATICKET' en core_parametro para este establecimiento");
                        }

                        if (string.IsNullOrWhiteSpace(param.valor))
                        {
                            throw new Exception("Campo 'valor' de registro 'PARKING_ITEMPERDIDATICKET' en core_parametro está vacío");
                        }

                        Control.Common.GlobalParameters.Parking_ItemPerdidaTicket = param.valor;

                        //------------------------RECIBO PERDIDA--------------------------------------------------------
                        var recibo = db.core_recibo.Where(x => x.identificador == "PARKING_PERDIDA").FirstOrDefault();
                        if (recibo == null)
                        {
                            throw new Exception("No hay recibo 'PARKING_PERDIDA' en core_recibo");
                        }

                        if (string.IsNullOrEmpty(recibo.cuerpo))
                        {
                            throw new Exception("Campo 'cuerpo' de registro 'PARKING_PERDIDA' en core_recibo está vacío");
                        }

                        Control.Common.GlobalParameters.Parking_ReciboPerdida = recibo.cuerpo;

                        //------------------------RECIBO PERDIDA LEYENDA--------------------------------------------------------
                        recibo = db.core_recibo.Where(x => x.identificador == "PARKING_PERDIDALEYENDA").FirstOrDefault();
                        if (recibo == null)
                        {
                            throw new Exception("No hay recibo 'PARKING_PERDIDALEYENDA' en core_recibo");
                        }

                        if (string.IsNullOrEmpty(recibo.cuerpo))
                        {
                            throw new Exception("Campo 'cuerpo' de registro 'PARKING_PERDIDALEYENDA' en core_recibo está vacío");
                        }

                        Control.Common.GlobalParameters.ParkingReciboPerdidaLeyenda = recibo.cuerpo;
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosParking", "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. A continuacion las excepciones encontradas " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                Control.Common.General.GetMensajeToList(153);

                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", "POS Parameros ");
                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        private void RecargarParametrosRetencionElectronica()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //------------------------RETENCION ACTIVO--------------------------------------------------------
                    var param = db.core_parametro.Where(x => x.identificador == "RETENCION_ACTIVO"
                                                         &&
                                                         (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.parametro2 == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        Control.Common.GlobalParameters.Retencion_TienePermiso = false;
                    }
                    else
                    {
                        Control.Common.GlobalParameters.Retencion_TienePermiso = param.valor == "TRUE";
                    }
                    //btnRetencionElect.Visible = Control.Common.GlobalParameters.Retencion_TienePermiso;

                    //------------------------DIAS VIGENCIA--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "RETENCION_DIAS_VIGENCIA"
                                                         &&
                                                         (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.parametro2 == null)
                                                   )
                                             .FirstOrDefault();

                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'RETENCION_DIAS_VIGENCIA' en core_parametro para este establecimiento");
                    }

                    if (string.IsNullOrWhiteSpace(param.valor))
                    {
                        throw new Exception("Campo 'valor' de registro 'RETENCION_DIAS_VIGENCIA' en core_parametro está vacío");
                    }

                    Control.Common.GlobalParameters.Retencion_DiasVigencia = int.Parse(param.valor);


                    //------------------------DIAS VIGENCIA ADICIONALES--------------------------------------------------------
                    param = db.core_parametro.Where(x => x.identificador == "RETENCION_DIAS_VIGENCIA_ADICIONAL"
                                                         &&
                                                         (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.parametro2 == null)
                                                   )
                                             .FirstOrDefault();

                    if (param == null)
                    {
                        throw new Exception("No hay parametro 'RETENCION_DIAS_VIGENCIA_ADICIONAL' en core_parametro para este establecimiento");
                    }

                    if (string.IsNullOrWhiteSpace(param.valor))
                    {
                        throw new Exception("Campo 'valor' de registro 'RETENCION_DIAS_VIGENCIA_ADICIONAL' en core_parametro está vacío");
                    }

                    Control.Common.GlobalParameters.Retencion_DiasVigenciaAdicional = int.Parse(param.valor);
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosRetencionElectronica", "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. A continuacion las excepciones encontradas " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                Control.Common.General.GetMensajeToList(153);

                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", "POS Parameros ");

                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        private void RecargarParametrosTarjetaEmpresarial()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //------------------------RETENCION ACTIVO--------------------------------------------------------
                    var param = db.core_parametro.Where(x => x.identificador == "TARJETAEMPRESARIAL_ACTIVO"
                                                         &&
                                                         (x.parametro2.Contains(Control.Common.GlobalParameters.Establecimiento + ";") || x.parametro2 == null)
                                                   )
                                             .FirstOrDefault();
                    if (param == null)
                    {
                        Control.Common.GlobalParameters.TarjetaEmpresa_TienePermiso = false;
                    }
                    else
                    {
                        Control.Common.GlobalParameters.TarjetaEmpresa_TienePermiso = param.valor == "TRUE";
                    }
                    //btnTarjetaEmpre.Visible = Control.Common.GlobalParameters.TarjetaEmpresa_TienePermiso;

                }
            }
            catch (Exception ex)
            {
                Control.Common.General.GetMensajeToList(153);
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosTarjetaEmpresarial", "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. A continuacion las excepciones encontradas " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", "POS Parameros ");

                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        #endregion

        private void FinalClient()
        {
            usotarjetadscto = false;
            cambiarCliente(Control.Common.GlobalParameters.IdConsumidorFinal);

            if (_factura.Descuentos2.Any())
            {
                _factura.Descuentos2.Clear();
                _factura.Pagos.Clear();
            }

            calcularFactura();

            // Llamar a Parqueo.  JM 08-03-2019
            if (Control.Common.GlobalParameters.Parking_TienePermiso)
            {
                SolicitarParqueo();
            }
        }
        private void getProductoCP(string codigo)
        {
            var productotmp = new Producto();

            using (POSEntities db = new POSEntities())
            {
                //var cab = db.cp_ordencabecera.Where(x => x.numOrder == codigo);

                var lista = db.VW_CP_DETALLE.Where(x => x.numOrder == codigo);

                foreach (var item in lista)
                {
                    Producto newprod = new Producto();
                    newprod.getProducto(item.codigobarra.Substring(0, 13), _factura, cliente_actual);
                    //newprod.CodigosBarra = item.codigobarra;
                    newprod.Cantidad = item.cantidad;
                    newprod.CantidadINEC = newprod.Cantidad;
                    newprod.update();
                    //newprod.Pvp = newprod.Pvp - Math.Truncate(Math.Round((newprod.Cantidad * newprod.Pvp) + newprod.Iva - item.subtotal, 3, MidpointRounding.AwayFromZero) * 100) / 100;
                    newprod.DescuentoAX = Math.Round((newprod.Cantidad * newprod.Pvp) + newprod.Iva - item.subtotal, 3, MidpointRounding.AwayFromZero);
                    //newprod.DescuentoAX = Math.Truncate(Math.Round((newprod.Cantidad * newprod.Pvp) + newprod.Iva - item.subtotal, 3, MidpointRounding.AwayFromZero) * 100) / 100;
                    //newprod.Subtotal = Math.Truncate(Math.Round(newprod.Cantidad* newprod.Pvp, 3, MidpointRounding.AwayFromZero) * 100) / 100;
                    //newprod.Total = Math.Truncate(Math.Round((newprod.Cantidad * newprod.Pvp) + newprod.Iva - newprod.DescuentoAX, 3, MidpointRounding.AwayFromZero) * 100) / 100;
                    newprod.update();
                    _factura.Productos.Add(newprod);
                }

            }
        }

        private void getProducto(string codigo, bool debeValidarParqueo = true)
        {
            bool validaPeso = true;
            decimal peso = 0M;
            string operador = "";
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            //JCanarte 5Ene2021 obtener código de Operador
            if (codigo.Length >=16)
            {
                operador = codigo.Substring(12,3);
            }

            //Quitamos letra F inicio. Tenemos balanzas que devuelven codigo de barras agregando F al principio
            if (codigo.StartsWith("F")) codigo = codigo.Substring(1, codigo.Length - 1);
            if (!PermiteAgregarItemPorParqueo(codigo.ToUpper())) return;

            //DateTime dt1 = DateTime.Now;
            //lblFechaInicio.Text = dt1.Hour.ToString().PadLeft(2, '0') + ":" + dt1.Minute.ToString().PadLeft(2, '0') + ":" + dt1.Second.ToString().PadLeft(2, '0') + "." + dt1.Millisecond.ToString().PadLeft(3, '0');
            //Por regla general todos los productos tienen su codigo en mayuscula, se usa TOUPPER para evitar errores por CaseSensitive
            codigo = codigo.ToUpper();

            var productotmp = new Producto();

            if (codigo.ToString().Trim().Length > 0)
            {
                var db = new POSEntities();
                if (!existeEnLista(codigo))
                {
                    DateTime now = DateTime.Now;
                    string today = ((int)now.DayOfWeek).ToString();
                    var producto = new Producto();

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " INICIO  producto.getProducto(codigo, _factura, cliente_actual)); ");
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " ITEMID: " + codigo);

                    if (producto.getProducto(codigo, _factura, cliente_actual))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " FIN  producto.getProducto(codigo, _factura, cliente_actual)); ");
                        AnulaDsctoCompraGratis();

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " INICIO  BlockProducts(codigo); ");
                        if (BlockProducts(codigo))
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", $" FIN  BlockProducts({codigo}); ");
                            Control.Common.General.GetMensajeToList(154);

                            //MessageBox.Show(this,"No se puede vender bebidas alcohólicas los días Domingos", "Venta Prohibida", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //System.Windows.Forms.MessageBox.Show(this, "Este producto esta deshabilitado", "Venta Prohibida", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Este producto esta deshabilitado", "POS Consulta Producto");
                            return;
                        }

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", $" INICIO existeEnListaDescuento({codigo}); ");

                        existeEnListaDescuento(codigo); //Verifica si esta en lista de descuentos AX         // se comenta esta linea porque no es usada para nada.
                       
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", $" FIN  existeEnListaDescuento({codigo}); ");

                        if (producto.esPeso)
                        {
                            //var peso = 0M;
                            peso = 0M;

                            //View_CarnicerosAX _carnicero = null;
                            //2020-04-24 evelasco: validar si el item es de carnisariato comienza con "210" ( código de barra), validar:
                            // agregar un parametro "SALTARPRODUCTOETIQUETA" si existe por establecimiento y ptoemision y esté habilitado entonces; 
                            //no mostrar pantalla de toma peso y dejar que obtenga el peso y precio de la etiqueta; 
                            //caso contrario, valida el peso del formulario.
                            validaPeso = true;
                            // if (codigo.StartsWith("210"))
                            // {
                            //Valida si el item puede saltar la toma de Peso, obtiene el precio de la etiqueta.
                            if (SALTARPRODUCTOETIQUETA && ListaCategoriaSaltarItemxEtiqueta.Contains(producto.Categoria.ToUpper()))
                            {
                                validaPeso = false;
                                peso = 0;
                            }
                            // }

                            if (validaPeso)
                            {
                                while (peso <= 0)
                                {
                                    peso = tomarPeso(producto);
                                    if (peso == 0)
                                    {
                                        parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                                        Control.Common.General.GetMensajeToList(580, parametros);

                                        //System.Windows.Forms.MessageBox.Show(this, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "POS Valida Peso");
                                        //Control.Common.General.GetMensaje("POS - Valida Peso", $"Peso No válido, por favor revisar en báscula {codigo}!", "I");
                                    }
                                }
                            }

                            decimal pesobascula;
                            validaPesoCajaBascula(producto, codigo, peso, out pesobascula);

                            if (pesobascula > 0)
                            {
                                producto.CantidadINEC = pesobascula;
                                producto.Cantidad = decimal.Round(pesobascula, 2);
                                producto.Operador = operador;  //JCanarte 5Ene2021 asigno código de operador
                                //producto.CarniceroCOD = _carnicero != null ? _carnicero.CODIGO : String.Empty;

                                if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))//cliente_actual.CUSTGROUP != "07" && cliente_actual.CUSTGROUP != "09" /*&& cliente_actual.CUSTGROUP != "EM"*/ && cliente_actual.CUSTGROUP != "CE")
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " iNICIO  producto.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string ");
                                    producto.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " FIN  producto.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string ");
                                }

                                producto.update();

                                if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                                {
                                    producto.VerifyComboProducts(codigo, _factura); //Verifica si el producto tiene promo de combos
                                }

                                _factura.Productos.Add(producto); //AGREGA EL PRODUCTO A LA LISTA EN LA FACTURA


                                if (db.core_parametro.Where(x => x.identificador == "PROMO_CHOOSE" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                                {
                                    refrescar_promocion(codigo);
                                }

                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "POS Valida Peso");
                                //Control.Common.General.GetMensaje("POS - Valida Peso", $"Peso No válido, por favor revisar en báscula {codigo}!", "I");

                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                                Control.Common.General.GetMensajeToList(580, parametros);
                                
                            }
                        }
                        else
                        {

                            peso = tomarPeso(producto);

                            //var peso = tomarPeso(producto);
                            producto.Cantidad = decimal.Round(peso, 2);
                            producto.CantidadINEC = peso;

                            if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))//cliente_actual.CUSTGROUP != "07" && cliente_actual.CUSTGROUP != "09" /*&& cliente_actual.CUSTGROUP != "EM"*/ && cliente_actual.CUSTGROUP != "CE")
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " INICIO producto.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM) ");
                                producto.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura); // Se envía factura para nueva promo Piazza 22-02-2019  
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindows", "getProducto", " FIN producto.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM) ");
                            }
                            producto.update();

                            if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                            {
                                producto.VerifyComboProducts(codigo, _factura); //Verifica si el producto tiene promo de combos
                            }
                            
                            _factura.Productos.Add(producto); //AGREGA EL PRODUCTO A LA LISTA EN LA FACTURA

                            if (db.core_parametro.Where(x => x.identificador == "PROMO_CHOOSE" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                            {
                                refrescar_promocion(codigo);
                            }

                            if (_factura.Descuentos2.Any())
                            {
                                _factura.Descuentos2.Clear();
                                _factura.Pagos.Clear();
                            }
                        }

                        //Calcular al producto descuentos por DescuentoCuponPromocional
                        if (_factura.EsUsoCuponPromocional)
                        {
                            //valida si el cupon es de otro producto
                            var EsCuponProductoCantidad = false;
                            var cupondeOtroProducto = _factura.Productos.Where(x => x.Id != producto.Id);

                            if (cupondeOtroProducto != null)
                            {
                                if (cupondeOtroProducto.Count() > 0)
                                {
                                    foreach (var item in cupondeOtroProducto)
                                    {
                                        if (item.DescuentosCupon != null)
                                        {
                                            if (item.DescuentosCupon.Count() > 0)
                                            {
                                                var cupon = item.DescuentosCupon.Where(x => x.codigo != _factura.CuponPromocionalCodigo);
                                                if (cupon != null)
                                                {
                                                    EsCuponProductoCantidad = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (EsCuponProductoCantidad == false)
                            {
                                producto.DescuentoAX = producto.SubtotalSinDescuento * _factura.CuponPromocionalPorcDesc;
                                producto.update();
                            }
                                
                        }
   
                    }
                    else
                    {
                        if (!codigo.StartsWith("30"))//Valida que no sea barra de descuento
                        {
                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[codigo_articulo]", valor = codigo });
                            Control.Common.General.GetMensajeToList(123, parametros);
                        }
                    }
                }
                else
                {
                    AnulaDsctoCompraGratis();
                    var existente = getExistente(codigo);
                    //Reiniciar descuentos del producto
                    existente.Descuento = 0M;
                    existente.DescuentoActual = 0M;
                    existente.DescuentoPorCombinacion = 0M;

                    decimal pesobascula;
                    peso = 0M;


                    validaPeso = true;
                    //Valida si el item puede saltar la toma de Peso, obtiene el precio/peso de la etiqueta.
                    if (SALTARPRODUCTOETIQUETA && ListaCategoriaSaltarItemxEtiqueta.Contains(existente.Categoria.ToUpper()))
                    {
                        validaPeso = false;
                        peso = 0;
                    }

                    if (validaPeso)
                    {
                        while (peso <= 0)
                        {
                            peso = tomarPeso(existente);// decimal.Round(tomarPeso(existente), 2);
                            if (peso == 0)
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Peso No válido, por favor revisar en báscula " + codigo.ToString() + "!", "POS Valida Peso");
                                //Control.Common.General.GetMensaje("POS - Valida Peso", $"Peso No válido, por favor revisar en báscula {codigo} !", "I");

                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                                Control.Common.General.GetMensajeToList(580, parametros);
                            }
                        }
                    }
                    //var peso=tomarPeso(producto);

                    validaPesoCajaBascula(existente, codigo, peso, out pesobascula);

                    if (pesobascula > 0)
                    {
                        //Producto p = new Producto();
                        //p.getProductoPeso(codigo);
                        int ext = existente.qtyBar;

                        if (ext > 0)
                        {
                            //existente.agregarAdicional(cantidad: pesobascula + (getCantidadXCaja(codigo))); 
                            existente.Cantidad = existente.Cantidad + ext;
                            existente.CantidadINEC = existente.Cantidad;
                            existente.Unidades = existente.Unidades + ext;
                            existente.update();
                        }
                        else
                        {
                            existente.agregarAdicional(cantidad: pesobascula + (getCantidadXCaja(codigo) - 1));
                        }

                        if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP)) //cliente_actual.CUSTGROUP != "07" && cliente_actual.CUSTGROUP != "09" /*&& cliente_actual.CUSTGROUP != "EM"*/ && cliente_actual.CUSTGROUP != "CE")
                        {
                            existente.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);  // Se envía factura para nueva promo Piazza 22-02-2019                              
                        }

                        existente.update();

                        //existente.agregarAdicional(cantidad: pesobascula + (getCantidadXCaja(codigo) - 1)); 
                        if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                        {
                            existente.VerifyComboProducts(codigo, _factura);
                        }



                        if (db.core_parametro.Where(x => x.identificador == "PROMO_CHOOSE" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                        {
                            refrescar_promocion(codigo);
                        }

                        if (_factura.Descuentos2.Any())
                        {
                            _factura.Descuentos2.Clear();
                            _factura.Pagos.Clear();
                        }
                    }
                    else
                    {
                        // System.Windows.Forms.MessageBox.Show(this, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //Control.Common.General.GetMensaje("POS", $"Peso No válido, por favor revisar en báscula{codigo.ToString()}! ", "ER");

                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                        Control.Common.General.GetMensajeToList(580, parametros);

                    }

                    //Reasignar descuentos por cliente
                    var productotmp2 = new Producto();
                    productotmp2.getProducto(codigo, _factura, cliente_actual);
                    if (productotmp2.DescuentoActual > 0)
                    {
                        existente.DescuentoAX = Math.Round(Math.Round(existente.Pvp * existente.Cantidad, 2) * productotmp2.DescuentoActual, 2);
                        existente.update();
                    }

                    //Retornar al producto sus descuentos acumulados por cupones de caducidad
                    if (existente.DescuentoCuponesCaducidad > 0)
                    {
                        existente.DescuentoAX = existente.DescuentoCuponesCaducidad;
                        existente.update();
                    }

                    //Retornar al producto sus descuentos acumulados por tarjetas PaviPlan
                    if (existente.DescuentoTarjetasPaviPlan > 0)
                    {
                        existente.DescuentoAX = existente.DescuentoTarjetasPaviPlan;
                        existente.update();
                    }

                    //Retornar al producto sus descuentos acumulados por tarjetas CompraGratis
                    if (existente.DescuentoTarjetasCompraGratis > 0)
                    {
                        existente.DescuentoAX = existente.DescuentoTarjetasCompraGratis;
                        existente.update();
                    }

                    //Calcular al producto descuentos por DescuentoCuponPromocional
                    if (_factura.EsUsoCuponPromocional)
                    {//validar si descuento es por producto 

                        //var product = _factura.Productos.Where(x => x.Id == codigo).First();
                        var productAdd = _factura.Productos.Where(x => x.Id == productotmp2.Id);

                        if (productAdd != null)
                        {
                            var product = productAdd.First();
                            if (product.DescuentosCupon != null)
                            {
                                

                                //***********************
                                var valorDscto = 0M;
                                foreach (var desct in product.DescuentosCupon)
                                {
                                    decimal descuento = 0M;
                                    descuento = decimal.Parse(desct.parametro);

                                    valorDscto = (existente.SubtotalSinDescuento / existente.Unidades) * descuento;
                                    existente.DescuentoAX = existente.DescuentoAX + valorDscto;
                                }
                                existente.update();
                                //**************************
                            }
                            else 
                            {
                                //valida si el cupon es de otro producto
                                var EsCuponProductoCantidad = false;
                                var cupondeOtroProducto = _factura.Productos.Where(x => x.Id != productotmp2.Id);

                                if(cupondeOtroProducto!=null)
                                {
                                    if(cupondeOtroProducto.Count()>0)
                                    {
                                        foreach (var item in cupondeOtroProducto)
                                        {
                                            if(item.DescuentosCupon!=null)
                                            {
                                                if (item.DescuentosCupon.Count()>0)
                                                {
                                                    var cupon = item.DescuentosCupon.Where(x => x.codigo  != _factura.CuponPromocionalCodigo);
                                                    if (cupon!=null)
                                                    {
                                                        EsCuponProductoCantidad = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (EsCuponProductoCantidad == false)
                                {
                                    existente.DescuentoAX = existente.SubtotalSinDescuento * _factura.CuponPromocionalPorcDesc;
                                    existente.update();
                                }
                            }
                        }
                        else
                        {
                            existente.DescuentoAX = existente.SubtotalSinDescuento * _factura.CuponPromocionalPorcDesc;
                            existente.update();
                        }

                       // existente.DescuentoAX = existente.SubtotalSinDescuento * _factura.CuponPromocionalPorcDesc;
                        
                    }
                }
                /********************************************************************
                 *          aqui promos personalizadas                              *
                 ********************************************************************/

                productotmp.getProducto(codigo, _factura, cliente_actual);

                promobucket(false);
                //////BG promo pavo 2 x 1
                promopavo(_factura);
                promolonchera(_factura, productotmp);
                promobonella(_factura, productotmp);
                promovino(_factura, productotmp);
                promojohnson(_factura, productotmp);
                promopulpa(_factura);
                promohotdog(_factura, productotmp);
                promochifle(_factura, productotmp);

                PromosPrecioPorCombinacion();

                PromosDsctoPorSuplemento();

                promocuponapp();

                //Valida que no sea barra de descuento
                if (!codigo.StartsWith("30")) promoivaSingle(codigo);

                /****************************************************************
                * 
                * VALIDACION DE CODIGO CON DESCUENTO
                * 
                *****************************************************************/
                if (codigo.StartsWith("211"))
                {
                    string codigoAux = codigo.Substring(3, codigo.Length - 9);
                    string porcdesc = codigo.Substring(codigo.Length - 6, 3);
                    string secuencia = codigo.Substring(codigo.Length - 3, 3);
                    bool encuentra = false;

                    for (var i = 0; i < _factura.Productos.Count; i++)
                    {
                        var item = _factura.Productos[i];
                        if (item.CodigosBarra.Any(x => x.codigo == codigoAux))
                        {
                            if (db.pos_itembarra_desc.Any(x => x.ITEMBARCODE == codigo))
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Este cupon ya fue utilizado");
                                //Control.Common.General.GetMensaje("POS", "Este cupon ya fue utilizado", "I");
                                Control.Common.General.GetMensajeToList(581);

                                encuentra = true;
                                break;
                            }
                            var porcentaje = Decimal.Parse(porcdesc);
                            item.DescuentoAX = item.Descuento + ((porcentaje) * item.Pvp) / 100;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            var cupon = new pos_itembarra_desc();
                            cupon.ITEMBARCODE = codigo;
                            cupon.FACTURA = _factura.GetNumeroFactura();
                            db.pos_itembarra_desc.Add(cupon);
                            db.SaveChanges();
                            encuentra = true;
                            break;
                        }
                    }
                    if (!encuentra)
                    {
                        //System.Windows.Forms.MessageBox.Show(this, "No existe ningun producto para usar este cupon.");
                        //Control.Common.General.GetMensaje("POS", "No existe ningun producto para usar este cupon.", "I");
                        Control.Common.General.GetMensajeToList(582);

                    }
                }


                if (codigo.StartsWith("30") && codigo.Length == 12)
                {
                    Decimal porcdesc; //= codigo.Substring(7, 2);
                    string ItemId, FacturaPOS;
                    int Inactivo;
                    SqlConnection conexion = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                    SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                    string Query = null;
                    SqlCommand comando = default(SqlCommand);
                    using (conexion)
                    {
                        conexion.Open();
                        Query = "Select top 1 ITEMID,INACTIVO,PORCENTAJE,FACTURAPOS from tblliquidacion WHERE CODIGOCUPON = '" + codigo + "'";
                        comando = new SqlCommand(Query, conexion);
                        SqlDataReader dr = comando.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            ItemId = dr.GetValue(0).ToString();
                            Inactivo = int.Parse(dr.GetValue(1).ToString());
                            porcdesc = Decimal.Parse(dr.GetValue(2).ToString());
                            FacturaPOS = dr.GetValue(3).ToString();

                            bool existefac = false;
                            if (!string.IsNullOrEmpty(FacturaPOS))
                            {
                                var estab = FacturaPOS.Substring(2, 3);
                                var ptoemi = FacturaPOS.Substring(6, 3);
                                var numfac = int.Parse(FacturaPOS.Substring(10, 9));

                                existefac = db.core_factura.Any(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == numfac);
                            }

                            if ((Inactivo == 1 && existefac)) // || _factura.Productos.Count(x => x.Id == ItemId) > 0)
                            {
                                // System.Windows.Forms.MessageBox.Show(this, "Este cupon ya fue utilizado");
                                //Control.Common.General.GetMensaje("POS", "Este cupon ya fue utilizado", "I");
                                Control.Common.General.GetMensajeToList(581);

                            }
                            else
                            {
                                txtCodigo.Text = ItemId;
                                txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));

                                if (_factura.Productos.Any(x => x.Id == ItemId))
                                {
                                    var item = _factura.Productos.FirstOrDefault(x => x.Id == ItemId);
                                    var porcentaje = decimal.Round(porcdesc, 2);
                                    var descuentoCaducidad = (porcentaje) * item.Pvp;
                                    item.DescuentoAX = descuentoCaducidad;
                                    item.update();
                                    calcularFactura();
                                    item.DescuentoCuponesCaducidad += descuentoCaducidad;
                                    flagDescuentoBarra = true;
                                    using (conexion2)
                                    {
                                        conexion2.Open();
                                        String Query1 = "Update tblliquidacion set INACTIVO =1, FacturaPos = '" + _factura.GetNumeroFactura() + "' where CODIGOCUPON = '" + codigo + "'";
                                        SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                        comandoupd.ExecuteNonQuery();
                                        conexion2.Close();
                                        conexion.Close();
                                    }
                                }

                                //for (var i = 0; i < _factura.Productos.Count; i++)
                                //{
                                //    var item = _factura.Productos[i];
                                //    if (item.Id == ItemId)
                                //    {
                                //        var porcentaje = decimal.Round(porcdesc, 2);
                                //        var descuentoCaducidad = (porcentaje) * item.Pvp;
                                //        item.DescuentoAX = descuentoCaducidad;
                                //        item.update();
                                //        calcularFactura();
                                //        item.DescuentoCuponesCaducidad += descuentoCaducidad;
                                //        flagDescuentoBarra = true;
                                //        using (conexion2)
                                //        {
                                //            conexion2.Open();
                                //            String Query1 = "Update tblliquidacion set INACTIVO =1, FacturaPos = '" + _factura.GetNumeroFactura() + "' where CODIGOCUPON = '" + codigo + "'";
                                //            SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                //            comandoupd.ExecuteNonQuery();
                                //            conexion2.Close();
                                //            conexion.Close();
                                //        }

                                //        break;
                                //    }
                                //}
                            }

                        }
                    }
                }

                calcularFactura();

            }

            RefrescarGridItems();

            ScrollLastGridItems();

            //DateTime dt2 = DateTime.Now;
            //lblFechaFin.Text = dt2.Hour.ToString().PadLeft(2, '0') + ":" + dt2.Minute.ToString().PadLeft(2, '0') + ":" + dt2.Second.ToString().PadLeft(2, '0') + "." + dt2.Millisecond.ToString().PadLeft(3, '0');

            //TimeSpan span = dt2 - dt1;
            //int ms = (int)span.TotalMilliseconds;

            //lblFechaDiferencia.Text = ms.ToString();

            if (debeValidarParqueo) CalcularParqueo();

            /*******************************************************************
                 *          hasta aqui promos personalizadas                       *
                 *******************************************************************/
            //if (productotmp.Id != null)
              agregaProductosTmpFile();  //agregaProductosTmp(codigo);
        }


        private void recalcularDsctosProducto(string codigo, Producto _product)
        {
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            //Quitamos letra F inicio. Tenemos balanzas que devuelven codigo de barras agregando F al principio
            if (codigo.StartsWith("F")) codigo = codigo.Substring(1, codigo.Length - 1);
            //if (!PermiteAgregarItemPorParqueo(codigo.ToUpper())) return;

            //DateTime dt1 = DateTime.Now;
            //lblFechaInicio.Text = dt1.Hour.ToString().PadLeft(2, '0') + ":" + dt1.Minute.ToString().PadLeft(2, '0') + ":" + dt1.Second.ToString().PadLeft(2, '0') + "." + dt1.Millisecond.ToString().PadLeft(3, '0');

            //Por regla general todos los productos tienen su codigo en mayuscula, se usa TOUPPER para evitar errores por CaseSensitive
            codigo = codigo.ToUpper();

            var productotmp = new Producto();

            if (codigo.ToString().Trim().Length > 0)
            {
                var db = new POSEntities();


                //Desde AQUI debe validar el recalculo de descuentos.

                var existente = getExistente(codigo);
                //Reiniciar descuentos del producto
                existente.Descuento = 0M;
                existente.DescuentoActual = 0M;
                existente.DescuentoPorCombinacion = 0M;

                decimal pesobascula;
                var peso = 0M;
                peso = _product.Cantidad;
                /*
                if (_product.esPeso)
                {
                    peso = _product.Cantidad;
                }
                */

                // peso = producto.cantidad;
                /*
                while (peso <= 0)
                {
                    peso = tomarPeso(existente);// decimal.Round(tomarPeso(existente), 2);
                    if (peso == 0)
                        MessageBox.Show(this, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                */

                //var peso=tomarPeso(producto);

                //validaPesoCajaBascula(existente, codigo, peso, out pesobascula);
                pesobascula = peso;

                if (pesobascula > 0)
                {
                    //Producto p = new Producto();
                    //p.getProductoPeso(codigo);
                    int ext = existente.qtyBar;
                    if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP)) //cliente_actual.CUSTGROUP != "07" && cliente_actual.CUSTGROUP != "09" /*&& cliente_actual.CUSTGROUP != "EM"*/ && cliente_actual.CUSTGROUP != "CE")
                    {
                        existente.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);  // Se envía factura para nueva promo Piazza 22-02-2019                              
                    }

                    existente.update();

                    //existente.agregarAdicional(cantidad: pesobascula + (getCantidadXCaja(codigo) - 1)); 
                    if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                    {
                        existente.VerifyComboProducts(codigo, _factura);
                    }



                    if (db.core_parametro.Where(x => x.identificador == "PROMO_CHOOSE" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
                    {
                        refrescar_promocion(codigo);
                    }

                    if (_factura.Descuentos2.Any())
                    {
                        _factura.Descuentos2.Clear();
                        _factura.Pagos.Clear();
                    }
                }
                else
                {
                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[codigo]", valor = codigo });
                    Control.Common.General.GetMensajeToList(602, parametros);

                    //Control.Common.General.GetMensaje("POS", $"Peso No válido, por favor revisar en báscula {codigo.ToString()}!", "ER");
                    //System.Windows.Forms.MessageBox.Show(this, "Peso No válido, por favor revisar en báscula  " + codigo.ToString() + "!", "Acción no valida", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //Reasignar descuentos por cliente
                var productotmp2 = new Producto();
                productotmp2.getProducto(codigo, _factura, cliente_actual);
                if (productotmp2.DescuentoActual > 0)
                {
                    existente.DescuentoAX = Math.Round(Math.Round(existente.Pvp * existente.Cantidad, 2) * productotmp2.DescuentoActual, 2);
                    existente.update();
                }

                //Retornar al producto sus descuentos acumulados por cupones de caducidad
                if (existente.DescuentoCuponesCaducidad > 0)
                {
                    existente.DescuentoAX = existente.DescuentoCuponesCaducidad;
                    existente.update();
                }

                //Retornar al producto sus descuentos acumulados por tarjetas PaviPlan
                if (existente.DescuentoTarjetasPaviPlan > 0)
                {
                    existente.DescuentoAX = existente.DescuentoTarjetasPaviPlan;
                    existente.update();
                }

                //Retornar al producto sus descuentos acumulados por tarjetas CompraGratis
                if (existente.DescuentoTarjetasCompraGratis > 0)
                {
                    existente.DescuentoAX = existente.DescuentoTarjetasCompraGratis;
                    existente.update();
                }

                //Calcular al producto descuentos por DescuentoCuponPromocional
                if (_factura.EsUsoCuponPromocional)
                {
                    existente.DescuentoAX = existente.SubtotalSinDescuento * _factura.CuponPromocionalPorcDesc;
                    existente.update();
                }


                //AQUI FINALIZA BLOQUE .

                /********************************************************************
                 *          aqui promos personalizadas                              *
                 ********************************************************************/

                productotmp.getProducto(codigo, _factura, cliente_actual);

                promobucket(false);
                //////BG promo pavo 2 x 1
                promopavo(_factura);
                promolonchera(_factura, productotmp);
                promobonella(_factura, productotmp);
                promovino(_factura, productotmp);
                promojohnson(_factura, productotmp);
                promopulpa(_factura);
                promohotdog(_factura, productotmp);
                promochifle(_factura, productotmp);

                PromosPrecioPorCombinacion();

                PromosDsctoPorSuplemento();

                promocuponapp();

                //Valida que no sea barra de descuento
                if (!codigo.StartsWith("30")) promoivaSingle(codigo);

                /****************************************************************
                * 
                * VALIDACION DE CODIGO CON DESCUENTO
                * 
                *****************************************************************/
                if (codigo.StartsWith("211"))
                {
                    string codigoAux = codigo.Substring(3, codigo.Length - 9);
                    string porcdesc = codigo.Substring(codigo.Length - 6, 3);
                    string secuencia = codigo.Substring(codigo.Length - 3, 3);
                    bool encuentra = false;

                    for (var i = 0; i < _factura.Productos.Count; i++)
                    {
                        var item = _factura.Productos[i];
                        if (item.CodigosBarra.Any(x => x.codigo == codigoAux))
                        {
                            if (db.pos_itembarra_desc.Any(x => x.ITEMBARCODE == codigo))
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Este cupon ya fue utilizado");
                                //Control.Common.General.GetMensaje("POS", "Este cupon ya fue utilizado", "I");
                                Control.Common.General.GetMensajeToList(603);

                                encuentra = true;
                                break;
                            }
                            var porcentaje = Decimal.Parse(porcdesc);
                            item.DescuentoAX = item.Descuento + ((porcentaje) * item.Pvp) / 100;
                            item.update();
                            var existente1 = getExistente(item.Id);
                            existente1.update();
                            calcularFactura();
                            var cupon = new pos_itembarra_desc();
                            cupon.ITEMBARCODE = codigo;
                            cupon.FACTURA = _factura.GetNumeroFactura();
                            db.pos_itembarra_desc.Add(cupon);
                            db.SaveChanges();
                            encuentra = true;
                            break;
                        }
                    }
                    if (!encuentra)
                    {
                        // System.Windows.Forms.MessageBox.Show(this, "No existe ningun producto para usar este cupon.");
                        //Control.Common.General.GetMensaje("POS", "No existe ningun producto para usar este cupon.", "I");
                        Control.Common.General.GetMensajeToList(582);

                    }
                }


                if (codigo.StartsWith("30") && codigo.Length == 12)
                {
                    Decimal porcdesc; //= codigo.Substring(7, 2);
                    string ItemId, FacturaPOS;
                    int Inactivo;
                    SqlConnection conexion = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                    SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                    string Query = null;
                    SqlCommand comando = default(SqlCommand);
                    using (conexion)
                    {
                        conexion.Open();
                        Query = "Select top 1 ITEMID,INACTIVO,PORCENTAJE,FACTURAPOS from tblliquidacion WHERE CODIGOCUPON = '" + codigo + "'";
                        comando = new SqlCommand(Query, conexion);
                        SqlDataReader dr = comando.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            ItemId = dr.GetValue(0).ToString();
                            Inactivo = int.Parse(dr.GetValue(1).ToString());
                            porcdesc = Decimal.Parse(dr.GetValue(2).ToString());
                            FacturaPOS = dr.GetValue(3).ToString();

                            bool existefac = false;
                            if (!string.IsNullOrEmpty(FacturaPOS))
                            {
                                var estab = FacturaPOS.Substring(2, 3);
                                var ptoemi = FacturaPOS.Substring(6, 3);
                                var numfac = int.Parse(FacturaPOS.Substring(10, 9));

                                existefac = db.core_factura.Any(x => x.establecimiento == estab && x.punto_emision == ptoemi && x.numero == numfac);
                            }

                            if ((Inactivo == 1 && existefac)) // || _factura.Productos.Count(x => x.Id == ItemId) > 0)
                            {
                                // System.Windows.Forms.MessageBox.Show(this, "Este cupon ya fue utilizado");
                                //Control.Common.General.GetMensaje("POS", "Este cupon ya fue utilizado.", "I");
                                Control.Common.General.GetMensajeToList(603);

                            }
                            else
                            {
                                txtCodigo.Text = ItemId;
                                txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));

                                if (_factura.Productos.Any(x => x.Id == ItemId))
                                {
                                    var item = _factura.Productos.FirstOrDefault(x => x.Id == ItemId);
                                    var porcentaje = decimal.Round(porcdesc, 2);
                                    var descuentoCaducidad = (porcentaje) * item.Pvp;
                                    item.DescuentoAX = descuentoCaducidad;
                                    item.update();
                                    calcularFactura();
                                    item.DescuentoCuponesCaducidad += descuentoCaducidad;
                                    flagDescuentoBarra = true;
                                    using (conexion2)
                                    {
                                        conexion2.Open();
                                        String Query1 = "Update tblliquidacion set INACTIVO =1, FacturaPos = '" + _factura.GetNumeroFactura() + "' where CODIGOCUPON = '" + codigo + "'";
                                        SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                        comandoupd.ExecuteNonQuery();
                                        conexion2.Close();
                                        conexion.Close();
                                    }
                                }


                            }

                        }
                    }
                }



                calcularFactura();

            }

            RefrescarGridItems();

            ScrollLastGridItems();

            //DateTime dt2 = DateTime.Now;
            //lblFechaFin.Text = dt2.Hour.ToString().PadLeft(2, '0') + ":" + dt2.Minute.ToString().PadLeft(2, '0') + ":" + dt2.Second.ToString().PadLeft(2, '0') + "." + dt2.Millisecond.ToString().PadLeft(3, '0');

            //TimeSpan span = dt2 - dt1;
            //int ms = (int)span.TotalMilliseconds;

            //lblFechaDiferencia.Text = ms.ToString();

            /*******************************************************************
                 *          hasta aqui promos personalizadas                       *
                 *******************************************************************/
            //if (productotmp.Id != null)
            //    agregaProductosTmpFile();  //agregaProductosTmp(codigo);
        }

        private void PromosDsctoPorSuplemento()
        {
            /*
            Configurar Tipo 40

            Escenario Comun Ejemplos:
            1: Compra una funda de raviolis y llévate una salsa gratis (habiendo 3 diferentes tipos de salsa)
            2: PROMOCION DE POR 4 LBS CARNE LLEVA 6 CERVEZA STELLA AL 20% DESC
            
            tbl_descuentoPOSCabecera:
            TipoDescuento: 40
            Almacen: Todos/CodigoAx
            FechaDesde/FechaHasta: Utc Datetime
            
            tbl_descuentoPOSDetalle(Combinaciones requeridas):
            //ItemId es el unico campo que se diferencia en cada registro
            ItemId: CodigoAx
            //Campo TieneDescuento debe ser true para los items que participan como combinacion requerida
            TieneDescuento: true
            //Campo Cantidad indica cuantos items debe llevar para aplicar a los items que ganan dscto 
            //DEBE repetir el valor en todos los registros para combinacion
            Cantidad: decimal
            Descuento: NO REQUERIDO
            
            tbl_descuentoPOSDetalle(Suplementarios que ganan dscto):
            //ItemId es el unico campo que se diferencia en cada registro
            ItemId: CodigoAx
            //Campo TieneDescuento debe ser false para los items que participan como suplementos que ganan dscto
            TieneDescuento: false
            //Campo Cantidad indica cuantas unidades, entre todos los suplementos en la factura, ganaran dscto por cada combinacion registrada
            //DEBE repetir el valor en todos los registros suplemento
            Cantidad: decimal
            //Campo Descuento indica el porcentaje dscto para los suplementos
            //DEBE repetir el valor en todos los registros suplemento
            Descuento: decimal
             */


            //Recorremos las combinaciones de descuento aleatorio (Tipo = 40) activas
            var promosPorCombinacionVigentes = _factura.PromocionesActuales.Where(x => x.Tipo == 40).ToList();

            if (promosPorCombinacionVigentes.Count > 0)
            {
                //Reset descuentos por combinacion de productos
                foreach (var producto in _factura.Productos)
                {
                    if (producto.DescuentoPorSerItemSuplemento > 0)
                    {
                        producto.DescuentoAX = -producto.DescuentoPorSerItemSuplemento;
                        producto.update();
                        producto.DescuentoPorSerItemSuplemento = 0;
                    }
                }

                foreach (var promo in promosPorCombinacionVigentes)
                {
                    if (promo.ListProductos.Count == 0) continue;

                    var prodCombinacion = promo.ListProductos.Where(x => x.TieneDescuentoProductoCliente).FirstOrDefault();
                    var prodSuplementario = promo.ListProductos.Where(x => !x.TieneDescuentoProductoCliente).FirstOrDefault();

                    if (prodCombinacion != null && prodSuplementario != null)
                    {
                        var cantidadRequeridaCombo = prodCombinacion.Cantidad;
                        var cantidadRequeridoSuplementarios = prodSuplementario.Cantidad;

                        var cantCombinaciones = Math.Truncate(_factura.Productos.Where(x => promo.ListProductos.Any(y => y.TieneDescuentoProductoCliente && y.Id == x.Id)).Sum(x => x.Cantidad) / cantidadRequeridaCombo);
                        var cantSuplementariosPromo = Math.Truncate(_factura.Productos.Where(x => promo.ListProductos.Any(y => !y.TieneDescuentoProductoCliente && y.Id == x.Id)).Sum(x => x.Cantidad) / cantidadRequeridoSuplementarios);

                        if (cantCombinaciones > cantSuplementariosPromo) cantCombinaciones = cantSuplementariosPromo;

                        if (cantCombinaciones > 0)
                        {
                            var cantidadAplicable = cantCombinaciones * cantidadRequeridoSuplementarios;
                            foreach (var prodFactura in _factura.Productos)
                            {
                                if (cantidadAplicable <= 0) break;

                                var prodPromo = promo.ListProductos.Where(x => !x.TieneDescuentoProductoCliente && x.Id == prodFactura.Id).FirstOrDefault();
                                if (prodPromo != null)
                                {
                                    var cantParaDscto = (cantidadAplicable <= prodFactura.Cantidad) ? cantidadAplicable : prodFactura.Cantidad;
                                    var dscto = cantParaDscto * prodFactura.Pvp * (prodPromo.Descuento / 100);
                                    if (prodFactura.Descuento < prodFactura.SubtotalSinDescuento)
                                    {
                                        if (dscto > (prodFactura.SubtotalSinDescuento - prodFactura.Descuento))
                                            dscto = prodFactura.SubtotalSinDescuento - prodFactura.Descuento;

                                        prodFactura.DescuentoAX = dscto;
                                        prodFactura.DescuentoPorSerItemSuplemento += dscto;
                                        prodFactura.update();
                                        cantidadAplicable -= cantParaDscto;

                                        using (var db = new POSEntities())
                                        {
                                            if (Control.Common.GlobalParameters.PROMO_IVA)
                                            {
                                                if (POS.Control.Common.Promo.EsDiaPromoIVA() && prodFactura.IvaProducto != 0
                                                            && !prodFactura.EsExcluidoPromoIVA)
                                                {
                                                    var descuentoPromoIVA = (prodFactura.Subtotal * prodFactura.IvaProducto);
                                                    prodFactura.DescuentoIVA = descuentoPromoIVA;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        private void PromosPrecioPorCombinacion()
        {
            decimal cantUnd;
            int minimoCant;
            decimal precioGrupo;
            int cantGrupos;
            decimal subtotalCantidadAplicaPromo;

            decimal conteoProductoGrup = 0;
            decimal acumuladoPreciosGrup = 0;
            decimal precioTotalGrupos = 0;
            decimal cantProductosProcesarEnItem = 0;
            decimal valorDescuento = 0;
            decimal cantProductosFaltan = 0;
            decimal totalEsperado = 0;

            //Recorremos las combinaciones por combinacion (Tipo = 50) activas
            var promosPorCombinacionVigentes = _factura.PromocionesActuales.Where(x => x.Tipo == 50).ToList();

            if (promosPorCombinacionVigentes.Count > 0)
            {
                //Reset descuentos por combinacion de productos
                foreach (var producto in _factura.Productos)
                {
                    if (producto.DescuentoPorCombinacion > 0)
                    {
                        producto.DescuentoAX = -producto.DescuentoPorCombinacion;
                        producto.update();
                        producto.DescuentoPorCombinacion = 0;
                    }
                }

                foreach (var promo in promosPorCombinacionVigentes)
                {
                    if (promo.ListProductos.Count == 0) continue;

                    cantUnd = 0;
                    minimoCant = 0;
                    precioGrupo = 0;
                    cantGrupos = 0;
                    subtotalCantidadAplicaPromo = 0;

                    conteoProductoGrup = 0;
                    acumuladoPreciosGrup = 0;
                    precioTotalGrupos = 0;
                    cantProductosProcesarEnItem = 0;
                    valorDescuento = 0;
                    cantProductosFaltan = 0;
                    totalEsperado = 0;

                    //Obtengo la sumatoria de productos participantes que hay en la factura
                    cantUnd = _factura.Productos.Where(p => promo.ListProductos.Any(x => x.Id == p.Id)).Sum(p => p.Cantidad);

                    //Obtengo cantidad minima y precio por grupo, se encuentra repetido en todos los detalles
                    minimoCant = (int)promo.ListProductos.First().Cantidad;
                    precioGrupo = promo.ListProductos.First().Descuento;

                    //Calculo la cantidad de grupos que se puede armar en la presente factura
                    cantGrupos = (int)Math.Truncate(cantUnd / minimoCant);

                    //Procesar si hay al menos un grupo
                    if (cantGrupos > 0)
                    {
                        //Obtengo el precio total que representan el total de grupos
                        precioTotalGrupos = (cantGrupos * precioGrupo);

                        //Recorro los productos en la factura que aplican a la promocion
                        foreach (var productoAplicaPromo in _factura.Productos.Where(p => promo.ListProductos.Any(x => x.Id == p.Id)))
                        {
                            cantProductosProcesarEnItem = 0;
                            valorDescuento = 0;

                            cantProductosFaltan = (cantGrupos * minimoCant) - conteoProductoGrup;

                            if (cantProductosFaltan > productoAplicaPromo.Cantidad)
                            {
                                //Si falta todavia por asignar aun con la cantidad de items de esta linea, calcular ponderado
                                cantProductosProcesarEnItem = productoAplicaPromo.Cantidad;
                            }
                            else
                            {
                                //Si es lo ultimo que se debe asignar, asignar automaticamente el restante
                                cantProductosProcesarEnItem = cantProductosFaltan;
                                //valorDescuento = precioTotalGrupos - acumuladoPreciosGrup;
                            }

                            //Por regla de 3 obtengo el precio total que corresponde en esta linea de factura
                            totalEsperado = (cantProductosProcesarEnItem * precioTotalGrupos) / (cantGrupos * minimoCant);

                            //Se aplica formula para obtener el valor de descuento correspondiente
                            subtotalCantidadAplicaPromo = Math.Round(productoAplicaPromo.Pvp * cantProductosProcesarEnItem, 2, MidpointRounding.AwayFromZero);
                            //subtotalCantidadAplicaPromo = productoAplicaPromo.Pvp * cantProductosProcesarEnItem;
                            valorDescuento = Math.Round((totalEsperado - subtotalCantidadAplicaPromo - (productoAplicaPromo.IvaProducto * subtotalCantidadAplicaPromo)) / (-productoAplicaPromo.IvaProducto - 1), 2, MidpointRounding.AwayFromZero);


                            //Actualizar descuento en linea de factura
                            productoAplicaPromo.DescuentoPorCombinacion += valorDescuento;
                            productoAplicaPromo.DescuentoAX = valorDescuento;
                            productoAplicaPromo.update();

                            acumuladoPreciosGrup += valorDescuento;
                            conteoProductoGrup += cantProductosProcesarEnItem;

                            using (var db = new POSEntities())
                            {
                                if (db.core_parametro.Where(x => x.identificador == "PROMO_IVA" && x.valor == "TRUE"
                                                 && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null))
                                         .FirstOrDefault() != null)
                                {
                                  //  var porcPromo = Decimal.Parse((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                                    if (POS.Control.Common.Promo.EsDiaPromoIVA() && productoAplicaPromo.IvaProducto != 0
                                                && !productoAplicaPromo.EsExcluidoPromoIVA)
                                    {
                                        var descuentoPromoIVA = (productoAplicaPromo.Subtotal * productoAplicaPromo.IvaProducto);
                                        productoAplicaPromo.DescuentoIVA = descuentoPromoIVA;
                                    }

                                }
                            }

                            if (conteoProductoGrup >= (cantGrupos * minimoCant)) break;

                        }
                    }
                }
            }
        }

        private void CambiarCliente_RecalcularFactura()
        {
            foreach (Producto producto in this._factura.Productos)
            {
                CambiarCliente_RecalcularProducto(producto.Id);
            }

            PromosPrecioPorCombinacion();
            PromosDsctoPorSuplemento();

            calcularFactura();

            RefrescarGridItems();
        }

        private void CambiarCliente_RecalcularFactura(bool EsEmpleadoLiris)
        {
            foreach (Producto producto in this._factura.Productos)
            {
                CambiarCliente_RecalcularProducto(producto.Id, EsEmpleadoLiris);
            }

            PromosPrecioPorCombinacion();
            PromosDsctoPorSuplemento();

            calcularFactura();

            RefrescarGridItems();
        }
        private void EjecutaPromocionesPersonalizada(string codigo, Producto productotmp)
        {
            promoivaSingle(codigo);

            promobucket(false);
            //////BG promo pavo 2 x 1
            promopavo(_factura);

            promolonchera(_factura, productotmp);

            promobonella(_factura, productotmp);

            promovino(_factura, productotmp);

            promojohnson(_factura, productotmp);

            promopulpa(_factura);

            promohotdog(_factura, productotmp);

            promochifle(_factura, productotmp);


        }
        private void CambiarCliente_RecalcularProducto(string codigo, bool EsEmpleadoLiris)
        {


            var db = new POSEntities();

            var existente = getExistente(codigo);
            //Reiniciar descuentos del producto
            existente.Descuento = 0M;
            existente.DescuentoActual = 0M;

            if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))
            {
                existente.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);
            }

            existente.update();

            //existente.agregarAdicional(cantidad: pesobascula + (getCantidadXCaja(codigo) - 1)); 
            if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
            {
                existente.VerifyComboProducts(codigo, _factura);
            }

            if (db.core_parametro.Where(x => x.identificador == "PROMO_CHOOSE" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
            {
                refrescar_promocion(codigo);
            }

            if (_factura.Descuentos2.Any())
            {
                _factura.Descuentos2.Clear();
                _factura.Pagos.Clear();
            }



            var productotmp = new Producto();
            productotmp.getProducto(codigo, _factura, cliente_actual);

            existente.Pvp = productotmp.Pvp;
            existente.TieneDescuentoProductoCliente = productotmp.TieneDescuentoProductoCliente;
            existente.DescuentoLocalProducto = productotmp.DescuentoLocalProducto;
            if (productotmp.DescuentoActual > 0)
            {
                existente.DescuentoAX = Math.Round(Math.Round(existente.Pvp * existente.Cantidad, 2) * productotmp.DescuentoActual, 2);
                existente.update();
            }
            existente.PorcDescuentoDivisionEmpleado = productotmp.PorcDescuentoDivisionEmpleado;

            //Retornar al producto sus descuentos acumulados por cupones de caducidad
            if (existente.DescuentoCuponesCaducidad > 0)
            {
                existente.DescuentoAX = existente.DescuentoCuponesCaducidad;
                existente.update();
            }

            //Retornar al producto sus descuentos acumulados por cupones de descuento VF
            if (existente.DescuentosCupon != null)
            {
                if (existente.DescuentosCupon.Count() > 0)
                {
                    productotmp.DescuentosCupon = existente.DescuentosCupon;

                    if (existente.Descuento == 0)
                    {
                        foreach (var cupon in productotmp.DescuentosCupon)
                        {
                            if (cupon.valor == -1)
                            {

                                decimal valorDscto = (existente.SubtotalSinDescuento / existente.Unidades) * decimal.Parse(cupon.parametro);
                                existente.DescuentoAX = existente.DescuentoAX + valorDscto;
                                existente.update(false);
                            }
                        }
                    }
                }
            }

            //Retornar al producto sus descuentos acumulados por tarjetas PaviPlan
            if (existente.DescuentoTarjetasPaviPlan > 0)
            {
                existente.DescuentoAX = existente.DescuentoTarjetasPaviPlan;
                existente.update();
            }

            EjecutaPromocionesPersonalizada(codigo, productotmp);

        }


        private void CambiarCliente_RecalcularProducto(string codigo)
        {
            var db = new POSEntities();

            var existente = getExistente(codigo);
            //Reiniciar descuentos del producto
            existente.Descuento = 0M;
            existente.DescuentoActual = 0M;

            if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))
            {
                existente.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);
            }

            existente.update();

            //existente.agregarAdicional(cantidad: pesobascula + (getCantidadXCaja(codigo) - 1)); 
            if (db.core_parametro.Where(x => x.identificador == "COMBO_PRODUCTS" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
            {
                existente.VerifyComboProducts(codigo, _factura);
            }

            if (db.core_parametro.Where(x => x.identificador == "PROMO_CHOOSE" && x.valor == "TRUE" && x.parametro2 == establecimiento_inicio).FirstOrDefault() != null)
            {
                refrescar_promocion(codigo);
            }

            if (_factura.Descuentos2.Any())
            {
                _factura.Descuentos2.Clear();
                _factura.Pagos.Clear();
            }

            var productotmp = new Producto();
            productotmp.getProducto(codigo, _factura, cliente_actual);

            existente.Pvp = productotmp.Pvp;
            existente.TieneDescuentoProductoCliente = productotmp.TieneDescuentoProductoCliente;
            existente.DescuentoLocalProducto = productotmp.DescuentoLocalProducto;
            if (productotmp.DescuentoActual > 0)
            {
                existente.DescuentoAX = Math.Round(Math.Round(existente.Pvp * existente.Cantidad, 2) * productotmp.DescuentoActual, 2);
                existente.update();
            }
            existente.PorcDescuentoDivisionEmpleado = productotmp.PorcDescuentoDivisionEmpleado;

            //Retornar al producto sus descuentos acumulados por cupones de caducidad
            if (existente.DescuentoCuponesCaducidad > 0)
            {
                existente.DescuentoAX = existente.DescuentoCuponesCaducidad;
                existente.update();
            }

            //Retornar al producto sus descuentos acumulados por cupones de descuento VF
            if (existente.DescuentosCupon != null)
            {
                if (existente.DescuentosCupon.Count()>0)
                { 
                    productotmp.DescuentosCupon = existente.DescuentosCupon;

                if (existente.Descuento == 0)
                {
                    foreach (var cupon in productotmp.DescuentosCupon)
                    {
                            if (cupon.valor == -1)
                            {

                                decimal valorDscto =  (existente.SubtotalSinDescuento / existente.Unidades) * decimal.Parse(cupon.parametro);


                                //Descuento descuento = new Descuento();
                                //descuento.codigo = itemcupon.parametro2;
                                //descuento.valor = itemcupon.valor;
                                //descuento.parametro = porcDesc.ToString();

                                //producto.DescuentosCupon.Add(descuento);

                                existente.DescuentoAX = existente.DescuentoAX + valorDscto;
                                existente.update(false);
                            }
                    }
                }
            }
            }

            //Retornar al producto sus descuentos acumulados por tarjetas PaviPlan
            if (existente.DescuentoTarjetasPaviPlan > 0)
            {
                existente.DescuentoAX = existente.DescuentoTarjetasPaviPlan;
                existente.update();
            }

            //Producto no recupera sus descuentos acumulados por tarjetas CompraGratis cuando se cambia cliente
            //Debe volver a pasar tarjeta si es que el nuevo cliente dispone de una


            /********************************************************************
             *          aqui promos personalizadas                              *
             ********************************************************************/
            promoivaSingle(codigo);

            promobucket(false);
            //////BG promo pavo 2 x 1
            promopavo(_factura);
            promolonchera(_factura, productotmp);
            promobonella(_factura, productotmp);
            promovino(_factura, productotmp);
            promojohnson(_factura, productotmp);
            promopulpa(_factura);
            promohotdog(_factura, productotmp);
            promochifle(_factura, productotmp);

            /*******************************************************************
             *          hasta aqui promos personalizadas                       *
             *******************************************************************/

            calcularFactura();

        }

        private void refrescar_promocion(string codigo)
        {
            _factura.esComboPerfecto = false;
            Producto producto = new Producto();
            VW_PromoChoose prod = producto.VerifyPromoChoose(codigo);

            if (prod != null)
            {
                //promo_list.Add(prod);
                //promo_list = promo_list.GroupBy(x => x.ITEMID).Select(x => x.First()).Distinct().ToList();
                agregar_a_estructura(prod);
            }

        }

        public void agregar_a_estructura(VW_PromoChoose _prod)
        {
            Producto prod = new Producto();
            prod.Id = _prod.ITEMID;
            var posiciones = existe_producto_en_estructura(prod);
            int num_combos = 0;
            decimal[] listanumeros = new decimal[(int)_prod.QTY];
            for (int i = 0; i < (int)_prod.QTY; i++)
            {
                listanumeros[i] = 0;
            }

            if (posiciones.Item1 != -1 && posiciones.Item2 != -1)
            {
                //se encontro producto, se aumenta en uno
                promo_list[posiciones.Item1][posiciones.Item2].P2 = (int.Parse(promo_list[posiciones.Item1][posiciones.Item2].P2) + 1).ToString();
                var esta_completo_combo = true;

                for (int i = 0; i < promo_list[posiciones.Item1].Count; i++)
                {
                    /*
                      if (promo_list[posiciones.Item1][i].P2 == "0")                
                      {
                          esta_completo_combo = false;
                      }
                     */

                    listanumeros[i] = int.Parse(promo_list[posiciones.Item1][i].P2);

                }
                Array.Sort(listanumeros);
                if (listanumeros[0] != listanumeros[((int)_prod.QTY) - 1])
                {
                    esta_completo_combo = false;
                }

                if (esta_completo_combo)
                {
                    //se habilita el combo nuevamente
                    promo_list[posiciones.Item1][0].ESTADO = true;

                }
                else
                {
                    promo_list[posiciones.Item1][0].ESTADO = false;
                    num_combos = 0;
                }

                num_combos = extraer_combos(promo_list[posiciones.Item1], decimal.Parse(_prod.QTY.ToString()));
                promo_list[posiciones.Item1][0].P3 = num_combos.ToString();

            }
            else
            {
                decimal residuo = 0.00M;
                //
                if (promo_list.Count > 0)
                {
                    //int num = promo_list.Count + promo_list[promo_list.Count - 1].Count;
                    int num = promo_list[promo_list.Count - 1].Count;
                    residuo = decimal.Parse(num.ToString()) % (decimal.Parse(_prod.QTY.ToString()));
                }

                int pos_insertar = 0;
                pos_insertar = getPosInsertar(_prod);
                if (pos_insertar != -1)
                {
                    _prod.ESTADO = true;
                    _prod.P2 = "1";
                    promo_list[promo_list.Count - 1].Add(_prod);
                    num_combos = extraer_combos(promo_list[pos_insertar], decimal.Parse(_prod.QTY.ToString()));

                    if (num_combos == 0)
                    {
                        promo_list[pos_insertar][0].P3 = "0";
                        promo_list[pos_insertar][0].ESTADO = false;
                    }
                    else
                    {
                        promo_list[pos_insertar][0].P3 = num_combos.ToString();
                        promo_list[pos_insertar][0].ESTADO = true;
                    }
                }
                else
                {
                    var nuevo_combo = new List<VW_PromoChoose>();
                    _prod.ESTADO = false;
                    _prod.P2 = "1";
                    _prod.P3 = "0";
                    nuevo_combo.Add(_prod);
                    promo_list.Add(nuevo_combo);
                }
            }
            refrescar_precios_por_combo();
        }

        private int getPosInsertar(VW_PromoChoose _prod)
        {
            for (int i = 0; i < promo_list.Count; i++)
            {
                if (promo_list[i].Count < _prod.QTY && promo_list[i].Count > 0)
                {
                    if (promo_list[i][0].PRECIO_COMBO == _prod.PRECIO_COMBO)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void quitar_de_estructura(Producto _prod)
        {
            //para borrar promo list en caso de que no hayan productos
            if (_factura.Productos.Count - 1 == 0)
            {
                promo_list = new List<List<VW_PromoChoose>>();
                return;
            }
            var posiciones = existe_producto_en_estructura(_prod);

            if (posiciones.Item1 != -1 && posiciones.Item2 != -1)
            {
                //se encontro producto, se quita
                promo_list[posiciones.Item1][posiciones.Item2].P2 = "0";
                int num_combos = extraer_combos(promo_list[posiciones.Item1], decimal.Parse(promo_list[posiciones.Item1][posiciones.Item2].QTY.ToString()));
                if (num_combos == 0)
                {
                    promo_list[posiciones.Item1][0].P3 = "0";
                    promo_list[posiciones.Item1][0].ESTADO = false;
                }
                else
                {
                    promo_list[posiciones.Item1][0].P3 = num_combos.ToString();
                    promo_list[posiciones.Item1][0].ESTADO = true;
                }

            }

            refrescar_precios_por_combo();
        }

        public int extraer_combos(List<VW_PromoChoose> _productos_combo, decimal _qty)
        {
            int num_combos = 0;
            List<VW_PromoChoose> _productos_comb_tmp = new List<VW_PromoChoose>();
            _productos_comb_tmp = replicar_lista(_productos_combo);
            //VW_PromoChoose[] _productos_comb_tmp = new VW_PromoChoose[_productos_combo.Count];
            // _productos_comb_tmp = _productos_combo.ToList();
            int count = 0;
            while (true)
            {
                count = 0;
                for (int i = 0; i < _productos_comb_tmp.Count; i++)
                {
                    if (_productos_comb_tmp[i].P2 == "0")
                    {
                        return num_combos;
                    }
                    else
                    {
                        _productos_comb_tmp[i].P2 = (int.Parse(_productos_comb_tmp[i].P2) - 1).ToString();
                        count++;
                    }
                }

                if (count == _qty) //para asegurarse que sea un combo de N productos
                {
                    num_combos++;
                }
            }
        }

        public List<VW_PromoChoose> replicar_lista(List<VW_PromoChoose> _productos_combo)
        {
            List<VW_PromoChoose> list = new List<VW_PromoChoose>();

            for (int i = 0; i < _productos_combo.Count; i++)
            {
                VW_PromoChoose prod = new VW_PromoChoose();
                //prod = _productos_combo[i];
                prod.BARCODE = _productos_combo[i].BARCODE;
                prod.ITEMID = _productos_combo[i].ITEMID;
                prod.P1 = _productos_combo[i].P1;
                prod.P2 = _productos_combo[i].P2;
                prod.P3 = _productos_combo[i].P3;
                prod.ESTADO = _productos_combo[i].ESTADO;
                list.Add(prod);
            }

            return list;
        }

        public Tuple<int, int> existe_producto_en_estructura(Producto _prod)
        {

            for (int i = 0; i < promo_list.Count; i++)
            {
                for (int j = 0; j < promo_list[i].Count; j++)
                {
                    if (promo_list[i][j].ITEMID == _prod.Id)
                    {

                        return Tuple.Create(i, j);
                    }
                }
            }
            //retorna la posición en el arreglo vertical
            return Tuple.Create(-1, -1); ;
        }

        private void refrescar_precios_por_combo()
        {
            //decimal acum_combos = 0.00M;
            for (int j = 0; j < _factura.Productos.Count; j++)
            {
                Producto producto = _factura.Productos[j];

                foreach (var promo_prod in promo_list)
                {
                    if (promo_prod[0].ESTADO == true) //Si el Combo es Completo
                    {
                        for (int i = 0; i < promo_prod.Count; i++)
                        {
                            VW_PromoChoose promo_prod_hijo = promo_prod[i];

                            if (promo_prod_hijo.ITEMID == producto.Id)
                            {
                                decimal nuevo_precio = 0.00M;

                                nuevo_precio = (decimal.Parse(promo_prod_hijo.PRECIO_COMBO.ToString())) / Math.Abs(decimal.Parse(promo_prod_hijo.QTY.ToString()));
                                nuevo_precio = Math.Truncate(100 * nuevo_precio) / 100;

                                if (i == decimal.Parse(promo_prod_hijo.QTY.ToString()) - 1)
                                {
                                    nuevo_precio = (decimal.Parse(promo_prod_hijo.PRECIO_COMBO.ToString())) - nuevo_precio * (decimal.Parse(promo_prod_hijo.QTY.ToString()) - 1);
                                }

                                decimal nuevo_subt = 0.00M;
                                // nuevo_subt = nuevo_precio * (decimal.Parse(promo_prod[0].P3));
                                nuevo_subt = Math.Round(nuevo_precio * (decimal.Parse(promo_prod[0].P3)), 2, MidpointRounding.AwayFromZero);

                                decimal subt_normal = 0.00M;
                                subt_normal = (decimal.Parse(promo_prod[0].P3)) * producto.Pvp;
                                subt_normal = subt_normal + (producto.IvaProducto * subt_normal);

                                decimal nuevo_ahorro = 0.00M;
                                nuevo_ahorro = subt_normal - nuevo_subt;

                                if (nuevo_ahorro > 0)
                                {
                                    if (producto.Iva > 0)
                                    {
                                        producto.DescuentoAX = producto.Subtotal - nuevo_ahorro - 0.70M;// Original producto.DescuentoAX = nuevo_ahorro - 0.0097M;
                                        producto.DescuentoAX = producto.DescuentoAX - 0.003M;
                                    }
                                    else
                                    {
                                        producto.DescuentoAX = producto.Subtotal - nuevo_ahorro - 0.903M;// Original producto.DescuentoAX = nuevo_ahorro 
                                    }
                                    producto.update();
                                }
                            }

                        }
                    }
                    else
                    {
                        //Cuando el combo es incompleto
                        for (int i = 0; i < promo_prod.Count; i++)
                        {
                            VW_PromoChoose promo_prod_hijo = promo_prod[i];

                            if (promo_prod_hijo.ITEMID == producto.Id)
                            {
                                if (producto.DescuentoAX == 0)
                                {
                                    producto.DescuentoAX = 0.00M;
                                }

                                producto.update();
                                break;
                            }
                        }

                    }
                }
            }

            // sumar todos los combos
            bool esComboPerfecto = true;
            decimal acum_combo = 0;
            for (int i = 0; i < promo_list.Count; i++)
            {
                decimal total_combo = 0;
                for (int j = 0; j < promo_list[i].Count; j++)
                {
                    total_combo = total_combo + decimal.Parse(promo_list[i][j].P2.ToString());
                }

                decimal total_num_combos = decimal.Parse(promo_list[i][0].P3.ToString());
                acum_combo = acum_combo + decimal.Parse(promo_list[i][0].P3.ToString()) * decimal.Parse(promo_list[i][0].PRECIO_COMBO.ToString());

                if (total_combo != (total_num_combos * promo_list[i][0].QTY) || promo_list[i][0].ESTADO.Equals("false"))
                {
                    esComboPerfecto = false;
                    break;
                }
            }

            if (promo_list.Count == 0)
            {
                esComboPerfecto = false;
            }

            decimal contar_total_promo = 0;
            //validar cuando no solo hay combo
            for (int i = 0; i < promo_list.Count; i++)
            {
                for (int j = 0; j < promo_list[i].Count; j++)
                {
                    if (decimal.Parse(promo_list[i][j].P2.ToString()) > 0)
                    {
                        contar_total_promo = contar_total_promo + 1;
                    }
                }
            }

            if (_factura.Productos.Count != contar_total_promo)
            {
                esComboPerfecto = false;
            }

            _factura.esComboPerfecto = esComboPerfecto;
            _factura.total_combo = acum_combo;
        }

        private int getCantidadXCaja(string codigo)
        {
            if (codigo != "")
            {
                using (var db = new POSEntities())
                {
                    var qty = (from q in db.VW_ITEMBARCODEQTY
                               where q.ITEMBARCODE == codigo
                               && q.QTY > 0
                               select q).FirstOrDefault();

                    if (qty != null)
                    {
                        return (int)qty.QTY;
                    }
                    else
                    {
                        return 1;
                    }
                }
            }
            else
            {
                return 1;
            }
        }

        #region ValidarProductoCarnicero
        private bool ValidarProductoCarnicero(Producto _item)
        {
            using (var db = new POSEntities())
            {
                var _almacen = db.core_establecimiento.FirstOrDefault(z => z.establecimiento == this._factura.Establecimiento).almacen;
                if (db.View_AlmacenProductoPeso.Any(x => x.ITEMID == _item.Id && x.INVENTLOCATIONID == _almacen))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion ValidarProductoCarnicero
        #region ValidarCodigoCarnicero
        private View_CarnicerosAX ValidarCodigoCarnicero()
        {
            try
            {
                using (var db = new POSEntities())
                {
                    DialogResult _carnicero;//= _inputform.ShowDialog();
                    int i = 1;
                    do
                    {
                        _inputform.setValue(String.Empty);
                        _carnicero = _inputform.ShowDialog();
                        if (_carnicero.Equals(System.Windows.Forms.DialogResult.Cancel))
                            return null;
                        if (db.View_CarnicerosAX.Any(x => x.CODIGO == _inputform.Value))
                        {
                            return db.View_CarnicerosAX.FirstOrDefault(x => x.CODIGO == _inputform.Value);
                        }
                        else
                        {
                            ShowDesktopAlert("Acción no valida",
                                "Codigo de Carnicero no registrado!", position: AlertScreenPosition.TopCenter, autoCloseDelay: 2);

                        }
                        i++;
                        //} while (i <= 3);
                    } while (_inputform.Value.Length >= 0);
                }
                return null;
            }
            catch (Exception ex)
            {
                ShowDesktopAlert("Acción no valida",
                    "Error al validar codigo carnicero!", position: AlertScreenPosition.TopCenter, autoCloseDelay: 2);
                return null;

            }
        }
        #endregion ValidarCodigoCarnicero

        public void validaPesoCajaBascula(Producto producto, string codigo, decimal peso, out decimal pesob)
        {
            try
            {

                pesob = 0M;
                if (producto.esPeso)
                {
                    using (var db = new POSEntities())
                    {
                        var tolerancia = db.core_parametro.First(x => x.identificador == "MARGEN_TOLERANCIA_PESO");
                        decimal descuento_local = 0M;
                        decimal descuento_local2 = 0M;
                        decimal PRECIO_LOCAL = 0M;

                        if (producto.Descuentos.Any(x => x.codigo == "Establecimiento"))
                        {
                            descuento_local = producto.Descuentos.First(x => x.codigo == "Establecimiento").valor / 100;
                        }

                        if (producto.Descuentos.Any(x => x.codigo == "EstablecimientoProducto" && x.parametro == _factura.Establecimiento && x.parametro2 == producto.Id))
                        {
                            descuento_local2 = (producto.Descuentos.First(x => x.codigo == "EstablecimientoProducto").valor > descuento_local ? producto.Descuentos.First(x => x.codigo == "EstablecimientoProducto" && x.parametro == _factura.Establecimiento && x.parametro2 == producto.Id).valor / 100 : 0);
                        }

                        PRECIO_LOCAL = decimal.Round((producto.Pvp - (producto.Pvp * descuento_local) - ((producto.Pvp - (producto.Pvp * descuento_local)) * descuento_local2)), 2);

                        if (producto.IvaProducto > 0)
                        {

                            //PRECIO_LOCAL = PRECIO_LOCAL * 1.14M;
                            //decimal bg_iva= ;
                            PRECIO_LOCAL = PRECIO_LOCAL * ((Control.Common.GlobalParameters.IVAGEN / 100) + 1M);
                        }

                        decimal precioetiqueta = decimal.Round(producto.getPrecioEtiqueta(codigo, producto.Id), 2);
                        decimal pesoetiqueta = decimal.Round(precioetiqueta / PRECIO_LOCAL, 2);

                        //Relacion contra tolerancia deprecada: POS elegirá el mayor entre el peso de la etiqueta y lo que marque la balanza de la caja
                        //Update: No perjudicar descuentos de core_descuento del tipo ProductoCliente
                        pesob = (pesoetiqueta >= peso ? (producto.TieneDescuentoProductoCliente ? peso : pesoetiqueta) : peso);

                        //if (pesoetiqueta > 0 && decimal.Round(pesoetiqueta + (pesoetiqueta * decimal.Parse(tolerancia.valor.ToString()) / 100), 2) < decimal.Round(peso, 2))
                        //{
                        //    pesob = 0M;
                        //}
                        //else
                        //{
                        //    if (pesoetiqueta > 0)
                        //        pesob = pesoetiqueta;
                        //    else
                        //        pesob = peso;
                        //}
                    }
                }
                else
                {
                    if (pesob == 0)
                        pesob = peso;
                }
            }
            catch (Exception ex)
            {                
                pesob = peso;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "validaPesoCajaBascula", ex.Message + ex.StackTrace);
            }
        }

        private decimal tomarPeso(Producto producto)
        {
            if (producto.esPeso)
            {
                if (!String.IsNullOrEmpty(_factura.ModeloBalanza))
                {
                    string[] parametrosbalanza = _factura.ModeloBalanza.Split('|');
                    if (parametrosbalanza[4] == "1")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "tomarPeso", "Segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen + " - " + _factura.ModeloBalanza);
                        var balanzaForm = new POS.Control.Peso.TomaPesoUI(producto.Nombre, this._factura);
                        balanzaForm._estab = this._factura.Establecimiento;
                        balanzaForm._modeloBalanza = this._factura.ModeloBalanza;

                        balanzaForm.StartPosition = FormStartPosition.CenterScreen;
                        balanzaForm.ShowDialog();
                        if (balanzaForm.correcto)
                        {
                            return balanzaForm.Peso;
                        }
                        else
                        {
                            return 0M;
                        }
                       
                    }
                    else if(parametrosbalanza[4] == "0")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "tomarPeso", "Segun punto: " + _factura.Establecimiento + "-" + _factura.PtoEmisionOrigen + " - " + _factura.ModeloBalanza);
                        if (scanner != null)
                        {
                            //MessageBox.Show(this,"Israel ");
                            if (scanner.IsOpen) scanner.Close();
                            var balanzaForm = new POS.Control.Peso.TomaPesoUI(producto.Nombre, this._factura);
                            balanzaForm._estab = this._factura.Establecimiento;
                            balanzaForm._modeloBalanza = this._factura.ModeloBalanza;
                            balanzaForm.StartPosition = FormStartPosition.CenterScreen;
                            balanzaForm.ShowDialog();
                            if (!_factura.User.isSuperUser)
                            {
                                scanner.Open();
                            }
                            if (balanzaForm.correcto)
                            {
                                return balanzaForm.Peso;
                            }
                            else
                            {
                                return 0M;
                            }
                        }
                        else
                        {
                            return 0M;
                        }
                    }
                }
                else
                {
                    if ((_factura.Establecimiento == "029" && _factura.PtoEmisionOrigen != "008" && _factura.PtoEmisionOrigen != "004")
                                || (_factura.Establecimiento == "024" && (_factura.PtoEmisionOrigen != "003" && _factura.PtoEmisionOrigen != "005"))
                                || (_factura.Establecimiento == "011" && (_factura.PtoEmisionOrigen == "001" || _factura.PtoEmisionOrigen == "008"))
                                || (_factura.Establecimiento == "006" && _factura.PtoEmisionOrigen == "999")
                                || (_factura.PtoEmisionOrigen == "999")
                                || (_factura.PtoEmisionOrigen == "888")
                                || (_factura.Establecimiento == "007" && _factura.PtoEmisionOrigen == "007")
                                )
                    {

                        var balanzaForm = new POS.Control.Peso.TomaPesoUI(producto.Nombre, this._factura);
                        balanzaForm._estab = this._factura.Establecimiento;
                        balanzaForm._modeloBalanza = this._factura.ModeloBalanza;

                        balanzaForm.StartPosition = FormStartPosition.CenterScreen;
                        balanzaForm.ShowDialog();
                        if (balanzaForm.correcto)
                        {
                            return balanzaForm.Peso;
                        }
                        else
                        {
                            return 0M;
                        }

                    }
                    else
                    {
                        if (scanner != null)
                        {
                            //MessageBox.Show(this,"Israel ");
                            if (scanner.IsOpen) scanner.Close();
                            var balanzaForm = new POS.Control.Peso.TomaPesoUI(producto.Nombre, this._factura);
                            balanzaForm._estab = this._factura.Establecimiento;
                            balanzaForm._modeloBalanza = this._factura.ModeloBalanza;
                            balanzaForm.StartPosition = FormStartPosition.CenterScreen;
                            balanzaForm.ShowDialog();
                            if (!_factura.User.isSuperUser)
                            {
                                scanner.Open();
                            }
                            if (balanzaForm.correcto)
                            {
                                return balanzaForm.Peso;
                            }
                            else
                            {
                                return 0M;
                            }
                        }
                        else
                        {
                            return 0M;
                        }
                    }
                }
            }
            return producto.Cantidad;
        }

        private void getGiftCard(string codigo)
        {
            var producto = new Producto();
            if (producto.getGiftCard(codigo, this._factura, cliente_actual))
            {
                var f = new POS.Control.TarjetaRegaloInput();
                f.StartPosition = FormStartPosition.CenterScreen;
                f.ShowDialog();
                if (f.Valor > 0)
                {
                    producto.Cantidad = f.Valor;
                    producto.CantidadINEC = producto.Cantidad;
                    _factura.Productos.Add(producto);
                    var db = new POSEntities();
                    var Tarjeta = db.core_giftcard.Single(x => x.codigo == codigo);
                    Tarjeta.saldo = f.Valor;
                    Tarjeta.activo = true;
                    Tarjeta.fecha_activacion = DateTime.Now;
                    Tarjeta.fecha_expiracion = DateTime.Now.AddYears(1);
                    db.SaveChanges();
                }
            }
            else
            {
                ShowDesktopAlert("Acción no valida",
                    "Código no existente!", position: AlertScreenPosition.TopCenter, autoCloseDelay: 2);
            }
            calcularFactura();

        }

        private bool BlockProducts(string code)
        {
            var db = new POSEntities();
            bool response = false;
          //  string A = db.core_parametro.Where(z => z.identificador == "NOSALE").FirstOrDefault().valor;
            var tmprod = db.pos_itembarra.Where(x => x.ITEMBARCODE == code).FirstOrDefault();
            string producto = tmprod != null ? tmprod.ITEMID : code;

            //if (db.VW_BLOCK_PRODUCTS.Where(x => x.BARCODE == code && x.VARIETY.ToUpper() == A).FirstOrDefault() != null)
            if ((db.VW_BLOCK_PRODUCTS.Where(x => x.BARCODE == code || x.PRODUCT == code).FirstOrDefault() != null)
            || (db.pos_itembloq_est.Where(x => x.ESTABLECIMIENTO_ID == _factura.Establecimiento && x.ITEMID == producto).FirstOrDefault() != null)
                )
            {

                /* TblFeriado result;
                 result = db.TblFeriados.Where(y => y.permiteVenta == false).FirstOrDefault();
                
                 if (result != null)
                 {*/
                response = true;
                //  }
            }
            else
            {
                response = false;
            }

            return response;
        }

        private bool existeEnLista(string codigo)
        {
            string codigoAct = "";


            //JCanarte 5Ene2021 obtener código de Operador
            codigoAct = codigo;

            if (codigo.Length >= 16)
            {
                codigoAct = codigo.Substring(0, 13);
            }

            // _factura.Productos.FirstOrDefault().CodigosBarra.ToList();

            //Buscar por barcode o por codigo Ax
            bool respuesta = _factura.Productos.Where(x => x.Id == codigoAct || x.CodigosBarra.Any(cb =>
                                                            cb.codigo == codigoAct
                                                            ||
                                                            ("F" + cb.codigo) == codigoAct)
                                                     )
                                               .FirstOrDefault() != null;

            //Buscar por barcode de peso
            if (!respuesta)
            {
                if (codigo.StartsWith(Control.Common.GlobalParameters.ProductoIdentificadorItemPeso) || codigo.StartsWith(Control.Common.GlobalParameters.ProductoIdentificadorItemPesoAlt))
                {
                    respuesta = _factura.Productos.Where(x => x.CodigosBarra.Any(cb =>
                                               ((cb.codigo.Length > 6 && codigoAct.Length > 6 && cb.codigo.EndsWith("000000")) ? 
                                               (cb.codigo == (codigoAct.Substring(0, codigoAct.Length - 6) + "000000")) : false))
                                        )
                                  .FirstOrDefault() != null;
                }
            }

            return respuesta;
        }

        public void existeEnListaDescuento(string codigo)
        {

            List<Promocion> lista_promociones = _factura.PromocionesActuales;

            int index = 0;

            var db = new POSEntities();
            pos_itembarra _codBarras = (from p in db.pos_itembarra
                                        where p.ITEMBARCODE == codigo
                                        || p.ITEMID == codigo
                                        select p).FirstOrDefault();

            if (_codBarras == null)
            {
                _codBarras = (from p in db.pos_itembarra
                              where p.ITEMBARCODE.StartsWith(codigo.Substring(0, 7))
                              select p).FirstOrDefault();
            }




            if (lista_promociones != null)
            {

                /*
                            descuento = vw_descuento.DESCUENTO;
                    cantidad = vw_descuento.CANTIDAD;
                    item_id = vw_descuento.ITEMID;

                    Producto prod = new Producto();
                    prod.Descuento = descuento;
                    prod.Cantidad = cantidad;
                    prod.CantidadINEC = prod.Cantidad;
                    prod.Id = item_id;
                    prod.TieneDescuentoProductoCliente = vw_descuento.TIENEDESCUENTO == 1;
                 
                 */
                /*
               var PromocionEspecificas =
            (from x in db.vw_DescuentosDetalleAX.Where(x => x.ITEMID == _codBarras.ITEMID).AsEnumerable()
                  .Select(x => new {
                      x.CANTIDAD,
                      x.ITEMID,
                      x.TIENEDESCUENTO,
                      x.DESCUENTO,
                      x.REFRECID,
                      x.RECID
                  }).AsEnumerable()
             join zz in arraypromocion
                on new { RecId = x.REFRECID } equals new { zz.RecId}
             select zz).ToList();*/


                ///  sfafafseverificaaquiiiiii
                ///  

               


                for (int i = 0; i < lista_promociones.Count; i++)
                {
                    

                    var promo = lista_promociones[i];

                    if (promo.ListProductos == null) promo.ListProductos = new List<Producto>();
                                       

                    if (_codBarras != null)
                    {
                        if (!promo.ListProductos.Any(x => x.Id == _codBarras.ITEMID))
                        {


                            llenarProductosDescuentosDesdeDB(promo, _codBarras.ITEMID);
                        }
                    }

                    lista_promociones.RemoveAt(index);
                    lista_promociones.Insert(index, promo);
                    index++;
                }

            }
        }

        private void llenarProductosDescuentosDesdeDB(Promocion promo, String codigo)
        {
            decimal descuento;
            decimal cantidad;
            string item_id;

            using (var db = new POSEntities())
            {
                // realizarUnLinqparasacarTodoaslasVigentesParaeseItems
                //INICIO VJFRANCO 06/09/2022
               
                var desctDetalleAX = db.vw_DescuentosDetalleAX.Where(x => x.ITEMID == codigo && x.REFRECID == promo.RecId).ToList();
                if (desctDetalleAX.Count == 0) return;
                else
                {
                    if (desctDetalleAX.Count > 1)
                    {
                        foreach (var vw_descuento in desctDetalleAX)
                        {
                            descuento = vw_descuento.DESCUENTO;
                            cantidad = vw_descuento.CANTIDAD;
                            item_id = vw_descuento.ITEMID;

                            Producto prod = new Producto();
                            prod.Descuento = descuento;
                            prod.Cantidad = cantidad;
                            prod.CantidadINEC = prod.Cantidad;
                            prod.Id = item_id;
                            prod.TieneDescuentoProductoCliente = vw_descuento.TIENEDESCUENTO == 1;
                            promo.ListProductos.Add(prod);
                        }
                    }
                    else
                    {
                        var vw_descuento = desctDetalleAX.First();
                        descuento = vw_descuento.DESCUENTO;
                        cantidad = vw_descuento.CANTIDAD;
                        item_id = vw_descuento.ITEMID;

                        Producto prod = new Producto();
                        prod.Descuento = descuento;
                        prod.Cantidad = cantidad;
                        prod.CantidadINEC = prod.Cantidad;
                        prod.Id = item_id;
                        prod.TieneDescuentoProductoCliente = vw_descuento.TIENEDESCUENTO == 1;
                        promo.ListProductos.Add(prod);

                    }

                }
                //FIN VJFRANCO 06/09/2022


            }

        }

        private Producto GetItemExistente(string codigo)
        {
            //Buscar por barcode o por codigo Ax
            Producto item = _factura.Productos.Where(x => x.Id == codigo || x.CodigosBarra.Any(cb =>
                                                            cb.codigo == codigo
                                                            ||
                                                            ("F" + cb.codigo) == codigo)
                                                     )
                                               .FirstOrDefault();

            //Buscar por barcode de peso
            if (item == null)
            {
                if (codigo.StartsWith(Control.Common.GlobalParameters.ProductoIdentificadorItemPeso) || codigo.StartsWith(Control.Common.GlobalParameters.ProductoIdentificadorItemPesoAlt))
                {
                    item = _factura.Productos.Where(x => x.CodigosBarra.Any(cb =>
                                                                ((cb.codigo.Length > 6 && codigo.Length > 6 && cb.codigo.EndsWith("000000")) ? (cb.codigo == (codigo.Substring(0, codigo.Length - 6) + "000000")) : false))
                                                         )
                                                   .FirstOrDefault();
                }
            }

            return item;
        }

        private Producto getExistente(string codigo)
        {
            //Por regla general todos los productos tienen su codigo en mayuscula, se usa TOUPPER para evitar errores por CaseSensitive
            codigo = codigo.ToUpper();
            var codigoBarraComparador = codigo;

            //Este metodo intentara encontrar que el codigo enviado es un codigo de barras en base POS
            Producto p = new Producto();
            pos_item item = p.getProductoPeso(codigo);

            foreach (var i in _factura.Productos)
            {
                foreach (var c in i.CodigosBarra)
                {
                    if (item != null)
                    {
                        using (var db = new POSEntities())
                        {
                            codigoBarraComparador = db.pos_itembarra.First(x => x.ITEMID == item.ITEMID).ITEMBARCODE;
                        }
                    }
                    if (c.codigo == codigoBarraComparador || "F" + c.codigo == codigoBarraComparador)
                    {
                        i.qtyBar = p.qtyBar;
                        return i;
                    }
                }
            }

            return _factura.Productos.Single(x => x.Id == codigo);
        }

        void ShowDesktopAlert(string title, string text, bool autoClose = true,
            int autoCloseDelay = 2, int width = 329, int height = 100, AlertScreenPosition position = AlertScreenPosition.TopCenter)
        {
            RadDesktopAlert alert = new RadDesktopAlert();
            alert.ThemeName = this.ThemeName;
            alert.AutoClose = autoClose;
            alert.AutoCloseDelay = autoCloseDelay;
            alert.FixedSize = new Size(width, height);
            alert.FadeAnimationType = FadeAnimationType.FadeIn | FadeAnimationType.FadeOut;
            alert.CaptionText = title;
            alert.ContentText = text;
            alert.ScreenPosition = position;
            alert.Show();
            alert.Closed += new RadPopupClosedEventHandler(alert_Closed);
        }

        private static void alert_Closed(object sender, RadPopupClosedEventArgs args)
        {
            RadDesktopAlert a = sender as RadDesktopAlert;
            a.Dispose();
        }

        private void btnGiftback_Click(object sender, EventArgs e)
        {
            bool debeCambiarTipoDocumento = false;
            if (POS.Control.Common.GlobalParameters.TienePermisoGiftcardVenta)
            {
                var frm = new Control.Giftcard.MenuPrincipal();
                frm.ShowDialog();
                debeCambiarTipoDocumento = frm.DebeCambiarTipoDocumento;
            }
            else
            {
                debeCambiarTipoDocumento = true;
            }

            if (debeCambiarTipoDocumento)
            {
                if (esFactura)
                {
                    if (POS.Control.POS.init(ref _factura, "R"))
                    {
                        setTituloDocumento();
                        _factura.Productos.Clear();
                        calcularFactura();
                        esRecarga = false;
                    }
                    else
                    {
                        ShowDesktopAlert("Aviso!", "No tiene activado Documento tipo RECIBO", position: AlertScreenPosition.TopCenter, autoCloseDelay: 2);

                    }
                }
            }
        }


        private void btn_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void btnNumero(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                var button = sender as System.Windows.Forms.Control;
                if(text.Name == "txtCedula")
                {
                    if (text.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
                    {
                        LimpiarClienteCompraGratis();
                        tempo666.Start();
                    }
                }
                text.Text = text.Text + button.Text;
                text.Focus();
            }

        }

        private void btnFactura_Click(object sender, EventArgs e)
        {
            if (esGiftCard)
            {

                POS.Control.POS.init(ref _factura);
                esRecarga = false;
                setTituloDocumento();
                _factura.Productos.Clear();
                calcularFactura();
            }
        }

        private void calcularFactura()
        {
            //JCanarte 27Abril2021  Ejecuta Promocion descuentos

            AplicaPromoDescuentoProductos = "";

            AplicaPromoDescuentoProductos = Control.Common.Promo.EjecutarPromoDescuentoProducto(ref _factura, _factura.EsUsoAppMovil);

            if (_factura != null)
            {
                var pos = new POSEntities();

                if (_factura.Pagos.Where(x => x.Descripcion == "RETCLIENTE").Sum(x => x.Valor) > 0)
                {
                    var pagoret = _factura.getSubTotal() * Decimal.Parse(pos.core_parametro.Where(x => x.identificador == "RETENCION_FTE").First().valor) - _factura.Pagos.Where(x => x.Descripcion == "RETCLIENTE").Sum(x => x.Valor);
                    if (Math.Round(pagoret, 2, MidpointRounding.ToEven) > 0)
                    {
                        _factura.agregarPagoRetencion(Math.Round(pagoret, 2, MidpointRounding.ToEven), "RETCLIENTE");
                    }
                    _factura.Retencion.ValorRetFte = _factura.Pagos.Where(x => x.Descripcion == "RETCLIENTE").Sum(x => x.Valor);
                    _factura.Retencion.ValorBase = _factura.getSubTotal();
                }

                //Agrega la línea de descuento al grid - Española
                //agregarDescuentoPromocionEspanola();

                for (int i = 0; i < _factura.Pagos.Count; i++)
                {
                    if (_factura.Pagos[i].Valor == 0)
                    {
                        _factura.Pagos.Remove(_factura.Pagos[i]);
                    }
                }

                if (_factura.GetTotal() > Control.Common.GlobalParameters.CUPO_CF  && _factura.ClienteIdentificacion == "9999999999999")
                {
                    DialogResult dr = new DialogResult();
                    TopeCF frm = new TopeCF();
                    while (dr != DialogResult.OK && dr != DialogResult.Retry)
                    {
                        dr = frm.ShowDialog();

                    }
                    switch (dr)
                    {
                        case DialogResult.OK:
                            // Activa botones deshabilitados x CF
                            txtCedula.Enabled = true;
                            btnCFinal.Enabled = false;
                            txtCodigo.Enabled = false;
                            btnBorrarProducto.Enabled = false;
                            btnSearchPro.Enabled = false;
                            btnQtyProduct.Enabled = false;

                            break;
                        case DialogResult.Retry:
                            /*
                          //  MessageBox.Show(this,"El producto "+gridItems.SelectedRows[0].Cells[0].Value+" Cantidad: "+ gridItems.SelectedRows[0].Cells[1].Value + "\nDebe retirar de lo despachado" );
                            txtCedula.Enabled = false;
                            btnCFinal.Enabled = false;
                            txtCodigo.Enabled = false;
                            btnBorrarProducto.Enabled = false;
                            btnSearchPro.Enabled = false;
                            btnQtyProduct.Enabled = false;
                          //  gridItems.SelectedRows[0].Delete();
                          */
                            break;

                    }
                }


                lblTotal.Text = string.Format("{0:C}", _factura.GetTotal());
                lblTotal2.Text = lblTotal.Text;
                lblRestante.Text = string.Format("{0:C}", _factura.getSaldoDisplay());
                var cambio = 0M;

                _factura.validar_pagos(out cambio);
                lblCambio.Text = string.Format("{0:C}", this._factura.Cambio);
                
            }
        }

        private void txtCedula_Enter(object sender, EventArgs e)
        { 
            focused = (System.Windows.Forms.Control)sender;             
        }

        private void txtCodigo_Enter(object sender, EventArgs e)
        {

            txtCodigo.SelectionStart = txtCodigo.Text.Length;
            focused = (System.Windows.Forms.Control)sender;
            
        }

        private void btnBorrar_Click(object sender, EventArgs e)
        {
            var text = focused as Telerik.WinControls.UI.RadTextBox;
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                SendKeys.Send("{ENTER}");
            }

        }

        private void pagoBotonEvent(object sender, EventArgs e)
        {
            var text = txtPagoValor;
            if (text != null)
            {
                var button = sender as System.Windows.Forms.Control;
                text.Text = text.Text + button.Text;
                text.Focus();
            }
        }

        private void gridItems_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            var new_font = new Font("Segoe UI", 14);
            e.CellElement.Font = new_font;
            SetGlobalCellFormatting(sender, e, ref gridItems);
        }

        private void btnPagar_Click(object sender, EventArgs e)
        {//Boton Pagar
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            if (AplicaPromoDescuentoProductos != "")
            {

                parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[AplicaPromoDescuentoProductos]", valor = AplicaPromoDescuentoProductos });
                Control.Common.General.GetMensajeToList(601, parametros);
                
                //MessageBoxTemporal.Show("Usuario aplica a promoción % descuento en: " + AplicaPromoDescuentoProductos , "Promoción POS", 10, true);
                //Control.Common.General.GetMensaje("POS - Promoción", "Usuario aplica a promoción % descuento en: {AplicaPromoDescuentoProductos} ", "I",10);
            }

            splitControl.Panel1Collapsed = true;
            panelPagoSecundario.Visible = true;
            flagProcesarDsct = false;
            var db = new POSEntities();

            //*****PROMO COMPRA GRATIS******************************************************************************************
            var PromoGratis = new List<core_promocompragratis>();
            decimal Disponible;

            PromoGratis = db.core_promocompragratis.Where(x => x.fecha == DateTime.Today && x.Entregado < x.Cantidad && (x.Local == _factura.Establecimiento || x.Local == null)).ToList();
            foreach (var desc in PromoGratis)
            {
                Disponible = desc.Cantidad - desc.Entregado;
                if (_factura.GetTotal() >= desc.TotalFacDesde && _factura.GetTotal() <= desc.TotalFacHasta && _factura.GetTotal() <= Disponible && !flagCompraGratis)
                {
                    var Parametros = new List<core_parametro>();
                    Parametros = db.core_parametro.Where(x => x.identificador == "PROMO_COMPRAGRATIS_HORA").ToList();
                    var RangoDefault = db.core_parametro.Where(x => x.identificador == "PROMO_COMPRAGRATIS_RANDON").FirstOrDefault();
                    int RangoRandom = int.Parse(RangoDefault.valor);
                    int suerte = 0;

                    foreach (var Parm in Parametros)
                    {
                        if (DateTime.Now >= DateTime.Parse(Parm.valor) && DateTime.Now <= DateTime.Parse(Parm.valor).AddMinutes(140))
                        {
                            RangoRandom = int.Parse(Parm.parametro2);
                        }
                    }

                    Random rnd = new Random();
                    suerte = rnd.Next(RangoRandom);

                    if (suerte == 1)
                    {
                        try
                        {
                            SmtpClient server = new SmtpClient("smtp.gmail.com", 587);
                            server.Credentials = new System.Net.NetworkCredential("tecworkec@gmail.com", "Ecuador1");
                            server.EnableSsl = true;
                            MailMessage mnsj = new MailMessage();
                            mnsj.Subject = "COMPRA GRATIS ";
                            mnsj.To.Add(new MailAddress("devteam@liris.com.ec"));
                            mnsj.CC.Add(new MailAddress("amolestina@liris.com.ec"));
                            //mnsj.From = new MailAddress("tecworkec@gmail.com", "LIRIS COMPRA_GRATIS");
                            mnsj.Body = "Ganador con una Factura de: $" + _factura.GetTotal() + " Factura No: " + _factura.GetNumeroFactura();
                            server.Send(mnsj);

                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnPagar_click", "No se pudo enviar correo de ganador de compra gratis, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                        }

                        DialogResult dr = new DialogResult();
                        Ganador frm = new Ganador();
                        while (dr != DialogResult.OK && dr != DialogResult.Retry)
                        {
                            dr = frm.ShowDialog();

                        }
                        switch (dr)
                        {
                            case DialogResult.OK:
                                break;
                            case DialogResult.Retry:
                                break;
                        }

                        var Promo = db.core_promocompragratis.Single(x => x.id == desc.id);
                        Promo.Entregado = Promo.Entregado + _factura.GetTotal();
                        db.SaveChanges();


                        var giftcard = new core_giftcard();
                        giftcard.fecha_creacion = DateTime.Now;
                        giftcard.fecha_modificacion = DateTime.Now;
                        giftcard.fecha_activacion = DateTime.Now;
                        giftcard.fecha_expiracion = DateTime.Now.AddDays(1);
                        giftcard.activo = true;
                        giftcard.bono = true;
                        giftcard.codigo = "000" + (_factura.GetNumeroFactura().Replace("-", "")).Replace("F", "");
                        giftcard.saldo = _factura.GetTotal();

                        db.core_giftcard.Add(giftcard);
                        db.SaveChanges();

                        //Agregar lineas de insert
                        Control.Common.Logger.Agregar_Trace_Giftcard(giftcard);

                        _factura.mensaje_promo = "    ¡Has ganado tu compra gratis\n de nuestra promoción tu Caja Sorpresa!\n";
                        _factura.mensaje_promo = _factura.mensaje_promo + " Tu compra de este momento te la llevas\n      sin pagar ni un centavo.\n";
                        _factura.mensaje_promo = _factura.mensaje_promo + "    ¡Feliz Navidad con Delportal.\n       Más fresco, Más cerca!";

                        //Se envia saldo cero porque esta giftcard será usada inmediatamente
                        _factura.AgregarPagoTarjetaRegalo(_factura.GetTotal(), "000" + (_factura.GetNumeroFactura().Replace("-", "")).Replace("F", ""), 0, false, string.Empty, string.Empty);
                        btnEliminarPago.Visible = false;
                        btnPagoAtras.Visible = false;
                    }
                }
            }
            flagCompraGratis = true;//Solo un sorteo
            //**************************************************************************************************************************


            string permite_almacen = db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor;
            if (permite_almacen == "TRUE")
            {
                bool permitedsctocliente = db.TblDiscountCards.Any(x => x.itemid == this._factura.ClienteIdentificacion);
                Decimal ventaminima = Decimal.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_VENTA_MINIMA" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);

                if (permitedsctocliente && _factura.GetTotal() >= ventaminima)
                {
                    var result = Control.Common.General.GetMensajeToList(20);

                    if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                    {
                        btnDescuentoEspecial_Click(sender, e);
                    }
                }

            }

            // Se levanta forma de pago dinero electronico.  JM 14-07-2020
            /*if (_factura.SaldoPuntos > 0 && (_factura.EsUsoAppMovil && POS.Control.Common.GlobalParameters.MonederoConsumoActivo))
            {
                if (MessageBox.Show(this, "Desea pagar el "+decimal.Round(Control.Common.GlobalParameters.MonederoPorcentajeConsumo) +"% de su compra con dinero electrónico que tiene en su billetera electrónica.?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    btnMonedero_Click(sender, e);
                }
            }*/

        }


        private void btnPagoAtras_Click(object sender, EventArgs e)
        {

            splitControl.Panel2Collapsed = true;
            panelPagoSecundario.Visible = false;
        }

        private void btnPagoBorrar_Click(object sender, EventArgs e)
        {
            var text = txtPagoValor;
            if (text != null)
            {
                if (text.Text.Length > 1)
                    text.Text = text.Text.Substring(0, text.Text.Length - 1);
                else
                    text.Text = "";
            }
        }

        private decimal getValorPago()
        {
            var valor = 0M;
            decimal.TryParse(txtPagoValor.Text, out valor);
            return valor;
        }

        private void btnTCredito_Click(object sender, EventArgs e)
        {
            //valida si no ha presionado el botón Borrar Pago. muestra pantalla de Promo Bines.
            if (!flagBorrarPago)
            {
                ValidaDescuentoenBines();
            }
            else
            {
                flagBorrarPago = false;
            }

            if (SolicitaParqueoEmergente()) return;

            string formaPago = "T. CREDITO";

            if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional)
            {
                formaPago = "TAR PORTAL";
            }

            if (revisarCambioEnTotal(formaPago, formaPago))
            {
                return;
            }

            //if (revisarCambioEnTotal("T. CREDITO", "T. CREDITO"))
            //{
            //    return;
            //}

            decimal valorRetante = decimal.Parse(this.lblRestante.Text.Replace("$",""));
            //var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaCredito, ref _factura, getValorPago());
            var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaCredito, ref _factura, valorRetante);

            f.Owner = this;
            // f.ShowDialog();
            f.ShowDialog(this);

            txtPagoValor.Clear();
            calcularFactura();
            agregaFormasPagoTmpFile();
        }

        private void ValidaDescuentoenBines()
        {
            try
            {
                using (POSEntities db = new POSEntities())
                {


                    if (db.core_parametro.Any(x => x.identificador.Equals("DESCUENTO_BINES_TARJETA") && x.parametro2 == POS.Control.Common.GlobalParameters.Establecimiento && x.valor.ToString().ToUpper().Equals("TRUE")))
                    {
                        var paramDsctoBinesTarj = db.core_parametro.Where(x => x.identificador.Equals("DESCUENTO_BINES_TARJETA") && x.parametro2 == POS.Control.Common.GlobalParameters.Establecimiento && x.valor.ToString().ToUpper().Equals("TRUE")).FirstOrDefault();

                        var productosdscto = db.core_parametro.Where(x => x.identificador == "DESCUENTO_BINES_TARJETA_PRODUCTOS" && x.valor == "TRUE").FirstOrDefault();

                        // Si no encuentra en la venta productos configurados no llama a la pantalla promo bines.   JM   19-11-2020
                        if (productosdscto != null)
                        {
                            var list_productos = productosdscto.documento.Split(';');
                            bool existeDscto = FacturaActual.Productos.Select(x => x.Id).Intersect(list_productos) .Any();
                            if (!existeDscto) 
                                return;                                 
                        }

                        SearchPromoBinesTarj promBines = new SearchPromoBinesTarj();
                        promBines.ShowDialog();
                        DescuentoPromoTarjBines = promBines.DescuentoPromTarjetaBines;
                        aplicaDsctoPromoTarjetaBines = promBines.AplicaPromoTarjBines;
                        binTarjetaPromoDscto = promBines.BinTarjetaPromo;
                        _factura.BinNumeroTarjetaPromo = binTarjetaPromoDscto;
                        // if(aplicaDsctoPromoTarjetaBines && _factura.apl)
                        agregarDescuentoPromocionTarjetaBines(DescuentoPromoTarjBines);
                    }

                }


            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidaDescuentoBines", "Ha Ocurrido una Excepción: " + ex.Message);
            }
        }

        private void agregarDescuentoPromocionTarjetaBines(decimal valorDescuento)
        {
            try
            {


                if (_factura != null && _factura.GetTotal() > 0)
                {
                    POSEntities db = new POSEntities();
                    if (aplicaDsctoPromoTarjetaBines)
                    {
                        var valor = 0M;
                        valor = valorDescuento;

                        _factura.agregarPromocionTarjetaBines(valor);
                        aplicaDsctoPromoTarjetaBines = false;
                        calcularFactura();


                    }
                    //}
                }
            }
            catch (Exception ex1)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregarDescuentoPromocionTarjetaBines", "Ha Ocurrido una Excepción: " + ex1.Message);
            }
        }

        //Elimina el descuento aplicado de la promoción de Bines Tarjeta de Crédito y vuelve a realizar calculo de los descuentos.
        public void QuitarDescuentoPromocionTarjetaBines()
        {
            try
            {
                if (_factura != null && _factura.GetTotal() > 0 && _factura.AplicaDescuentoPromoBines)
                {
                    POSEntities db = new POSEntities();
                    _factura.quitarPromocionTarjetaBines();
                    _factura.BinNumeroTarjetaPromo = string.Empty;
                    aplicaDsctoPromoTarjetaBines = false;
                    DescuentoPromoTarjBines = 0;

                    foreach (var p in _factura.Productos)
                    {
                        //  p.Cantidad = p.Cantidad - 1;
                        //  p.CantidadINEC = p.Cantidad;
                        //  p.Unidades = p.Unidades - 1;
                        recalcularDsctosProducto(p.Id, p);
                    }
                    calcularFactura();
                    //}
                }

            }
            catch (Exception ex1)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "QuitarDescuentoPromocionTarjetaBines", "Ha Ocurrido una Excepción: " + ex1.Message);
            }
        }


        private bool revisarCambioEnTotal(string forma_pago, string codigo)
        {
            if (_factura.Documento == "F")
            {
                if (_factura.ClienteIdentificacion != null)
                {
                    if (_factura.buscarDescuentosFormaPago(forma_pago, codigo))
                    {
                        var f = new POS.Control.Pagos.CalculoPago(_factura);
                        f.StartPosition = FormStartPosition.CenterScreen;
                        f.ShowDialog();
                        calcularFactura();
                        return true;
                    }
                }
            }
            return false;
        }

        private void btnEfectivo_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();
            string formaPago = "EFECTIVO";
            
            if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional )
            {
                formaPago = "TAR PORTAL";
                if (revisarCambioEnTotal(formaPago, formaPago))
                {
                    return;
                }
            }
            
            if (_factura != null && _factura.GetTotal() > 0)
            {
                var valor = 0M;
                if (decimal.TryParse(txtPagoValor.Text, out valor) && valor > 0)
                {
                    _factura.agregarPagoEfectivo(valor);
                    txtPagoValor.Clear();
                    calcularFactura();
                    agregaFormasPagoTmpFile();
                }
            }
        }

        private void btnEliminarPago_Click(object sender, EventArgs e)
        {

            if (gridPagos.SelectedRows.Count > 0)
            {
                var codigo = gridPagos.SelectedRows[0].DataBoundItem as POS.Models.Pago;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnEliminarPago_Click", "Acción borrar pago solicitada por usuario sobre pago tipo: " + codigo.Descripcion + ", valor: " + codigo.Valor.ToString("N2"));
                //QuitarDescuentoPromocionTarjetaBines();
                if (codigo.Descripcion == "T. CREDITO")
                {
                    flagBorrarPago = true; //validación para que no muestre pantalla promo tarjeta bines si se va a eliminar un pago con TC.
                    btnTCredito_Click(sender, e);
                }
                else
                {
                    this._factura.borrarDescuentoFormaPago(codigo.Descripcion);

                    if (codigo.Descripcion == "TAR PORTAL")
                    {
                        ActivateDivisionEmployDiscounts(false, true);
                    }

                    if (codigo.Descripcion == "ANTCLIEN/C" && Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC==true)
                    {
                        Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;
                        foreach (var item in _factura.Productos)
                        {
                            if (item.Iva > 0M)
                            {
                                item.calcularIVA(Control.Common.GlobalParameters.IVAGEN);
                            }
                        }
                    }

                    GridViewRowInfo selectedRow = gridPagos.SelectedRows[0];
                    //if (codigo.Descripcion.ToString().ToUpper() == "DINE ELECT" && this.FacturaActual.PedidoOtraApp.Tipo == (int)CANALVENTA.VENTAAPPPOS)
                    if (this.FacturaActual.PedidoOtraApp.Tipo == (int)CANALVENTA.VENTAAPPPOS)
                    {
                        Control.Common.General.GetMensajeToList(178);
                        //System.Windows.Forms.MessageBox.Show("Lo sentimos, no se puede eliminar la forma de pago ya que viene desde la App.", "Formas de Pago", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        gridPagos.SelectedRows[0].Delete();
                        calcularFactura();
                        agregaFormasPagoTmpFile();
                    }
                }
            }
        }

        /// <summary>
        /// Verifica si es la impresora esta con un driver generico, si es asi le agrega un ENDLINE adicional.
        /// </summary>
        /// <param name="printer">Objeto PCPrint</param>
        /// <param name="s">Contenedor de texto</param>
        /// <param name="text">Text a agregar</param>
        private void addCL(DSS.Controles.Impresion.DSSPrint printer, StringBuilder s, string text)
        {
            s.AppendLine(text);
            if (printer.PrinterSettings.PrinterName.Contains("Generic"))
                s.Append("\n");
        }

        private DSS.Controles.Impresion.DSSPrint printer = new DSS.Controles.Impresion.DSSPrint();

        frmLoading loading = new frmLoading();
        private void BackgroundGrabarDocumentoProcess()
        {

            POS.Control.Common.GlobalParameters.DatoRetencionEnFactura = false;

            Thread.Sleep(500);
            this.Invoke(new Action(() => {
                loading.lblSeguimientoEvent.Text = "Inicio de proceso.... Por favor espere!";
            }));

            Thread.Sleep(500);
            this.Invoke(new Action(() => {
                loading.lblSeguimientoEvent.Text = "Ejecutando validaciones generales de POS!..";
            }));

            if (!POS.Control.Common.GlobalParameters.PosEnServidorCorrecto && POS.Control.Common.GlobalParameters.PresentarMensajeServidorIncorrecto)
            {
                Control.Common.General.GetMensajeToList(175);
                //System.Windows.Forms.MessageBox.Show(this, "Está grabando la factura en el servidor incorrecto, por favor cambiar al servidor correcto.", "Servidor incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Thread.Sleep(500);
            //this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Valida Cliente RetencionFactura"; }));

            if (txtCedula.Text != "9999999999999" && txtCedula.Text.ToString().Length == 13) //RUC
            {
                if (POS.Control.Common.GlobalParameters.PreguntarSiImprimeDatoRetencion == true)
                {
                    Control.Common.General.GetMensajeToList(176);
                    POS.Control.Common.GlobalParameters.DatoRetencionEnFactura = true;

                }
                else
                {
                    if (POS.Control.Common.GlobalParameters.ImprimeDatoRetencion == true)
                    {
                        POS.Control.Common.GlobalParameters.DatoRetencionEnFactura = true;
                    }
                }
            }

            bool _validafactura = true;
            List<ParametrosMensajes> parametrosMensajes = new List<ParametrosMensajes>();
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            Thread.Sleep(500);
            //this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Solicita Parqueo Emergente";}));
            if (SolicitaParqueoEmergente()) return;


            Thread.Sleep(500);
            //this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Aplica Promoción Descuento Productos "; }));

            if (AplicaPromoDescuentoProductos != "")
            {

                parametrosMensajes.Add(new ParametrosMensajes() { codigo = "[AplicaPromoDescuentoProductos]", valor = AplicaPromoDescuentoProductos });

                var resutl = Control.Common.General.GetMensajeToList(177, parametrosMensajes);
                if (resutl == MsgBoxCtrl.MessageBoxResult.Ok || resutl == MsgBoxCtrl.MessageBoxResult.Yes)
                {
                    return;
                }
            }

            Thread.Sleep(500);
            //this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Genera Salida Parqueo / Genera Salida por Perdida "; }));
            GenerarSalidaParqueo();
            GenerarSalidaPorPerdida();

            //Validar que pagos Giftcard tipo GrupoCliente solo puedan salir a nombre de la identificacion principal
            Thread.Sleep(500);
            //this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Validar que pagos Giftcard tipo GrupoCliente solo puedan salir a nombre de la identificacion principal "; }));

            var pagosGiftCard = _factura.Pagos.Where(x => x.Descripcion == "GIFT CARD" || x.Descripcion == "GIFT CARDV").ToList();
            if (pagosGiftCard.Count > 0)
            {
                var gcGrupoClienteNoFactura = (PagoGiftCard)pagosGiftCard.SelectMany(x => x.Pagos).Where(y => ((PagoGiftCard)y).EstaAsociadaGrupoCliente
                                                            &&
                                                            !(((PagoGiftCard)y).IdentificacionGrupoCliente.Equals(_factura.ClienteIdentificacion))
                                                      ).FirstOrDefault();


                if (gcGrupoClienteNoFactura != null)
                {

                    string MsjError = string.Empty;
                    MsjError = string.Concat(MsjError, "Hemos detectado que está usando una(s) giftcard de GrupoCliente");
                    MsjError = string.Concat(MsjError, Environment.NewLine, Environment.NewLine);
                    MsjError = string.Concat(MsjError, $" - {gcGrupoClienteNoFactura.NombreGrupoCliente}");
                    MsjError = string.Concat(MsjError, Environment.NewLine, Environment.NewLine);
                    MsjError = string.Concat(MsjError, $"por lo que la factura debe salir a nombre del cliente principal. Giftcard => '{gcGrupoClienteNoFactura.Codigo}'");
                    //Control.Common.General.GetMensaje("POS", MsjError, "I");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = MsjError });
                    Control.Common.General.GetMensajeToList(584, parametros);


                    MsjError = string.Empty;
                    MsjError = string.Concat(MsjError, $"Procediendo a cambiar la factura con el cliente principal del grupo : '{gcGrupoClienteNoFactura.IdentificacionGrupoCliente}'");
                 
                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = MsjError });
                    Control.Common.General.GetMensajeToList(584, parametros);

                    cambiarCliente(gcGrupoClienteNoFactura.IdentificacionGrupoCliente);
                    return;
                }
            }

            Thread.Sleep(500);
            //this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Validar que pagos TAR PORTAL solo puedan ser usados por el titular de la cuenta "; }));
            //Validar que pagos TAR PORTAL solo puedan ser usados por el titular de la cuenta 
            POS.Control.Pagos.ClsTarjetaCreditoInterno agenteTarjetaInterna = new Control.Pagos.ClsTarjetaCreditoInterno();
            if (!agenteTarjetaInterna.TieneYPuedeRealizarPagoTarPortal(_factura))
            {
                return;
            }


            btnGrabar.Enabled = false;


            //Registrar en log el click del boton Grabar y cantidad de Pagos/Productos en el momento
            string strLogPagosProductos = "Usuario ha pulsado botón Grabar, la factura en este momento es '" + (_factura == null ? "Objeto _factura está nulo" : _factura.GetNumeroFactura()) + "'. Total factura: " + lblTotal2.Text + ". ";
            if (_factura == null)
            {
                strLogPagosProductos += "Informacion de cantidad Pagos/Productos no disponible porque el objeto _factura estaba en nulo";
            }
            else
            {
                if (_factura.Pagos == null)
                    strLogPagosProductos += "Cantidad de Pagos no disponible porque el objeto _factura.Pagos estaba en nulo. ";
                else
                    strLogPagosProductos += "Cantidad de Pagos: " + _factura.Pagos.Count.ToString() + ". ";


                if (_factura.Productos == null)
                    strLogPagosProductos += "Cantidad de Productos no disponible porque el objeto _factura.Productos estaba en nulo. ";
                else
                    strLogPagosProductos += "Cantidad de Productos: " + _factura.Productos.Count.ToString() + ". ";
            }

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", strLogPagosProductos);

            //Control.Common.Promo.EjecutarPromoRegala(ref _factura);
            Control.Common.Promo.EjecutarPromoRegalaStock(ref _factura, _factura.EsUsoAppMovil);

            //Revisar si se está facturando con una version actualizada
            var verificador = new POS.Control.Security.Environment();
            verificador.CheckHasLatestVersion();

            StringBuilder seguimiento = new StringBuilder();
            seguimiento.AppendLine(" Inicia seguimiento:");
            try
            {
                foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
                {
                    if (lang.Culture.EnglishName.ToUpper() == "ENGLISH (UNITED STATES)")
                    {
                        InputLanguage.CurrentInputLanguage = lang;
                    }
                }

                seguimiento.AppendLine("Realizado cambio de configuracion regional");

                using (POSEntities pos = new POSEntities())
                {
                    seguimiento.AppendLine("Antes de validar maximo Cupo CF");
                    if (_factura.GetTotal() > Control.Common.GlobalParameters.CUPO_CF && _factura.ClienteIdentificacion == "9999999999999")
                    {
                        seguimiento.AppendLine("POS detecto que cliente es CF y que excedio el maximo Cupo CF");
                        DialogResult dr = new DialogResult();
                        TopeCF frm = new TopeCF();
                        while (dr != DialogResult.OK && dr != DialogResult.Retry)
                        {
                            dr = frm.ShowDialog();

                        }
                        switch (dr)
                        {
                            case DialogResult.OK:
                                // Activa botones deshabilitados x CF
                                txtCedula.Enabled = true;
                                btnCFinal.Enabled = false;
                                txtCodigo.Enabled = false;
                                btnBorrarProducto.Enabled = false;
                                btnSearchPro.Enabled = false;
                                btnQtyProduct.Enabled = false;

                                break;
                            case DialogResult.Retry:
                             
                                btnGrabar.Enabled = true;
                                return;
                        }
                    }
                    seguimiento.AppendLine("Finaliza validacion maximo Cupo CF");
                }

                if (cliente_actual == null)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Botón Grabar fue pulsado pero POS no tiene un cliente cargado en la variable 'cliente_actual'. Se da por terminado el método");
                    btnGrabar.Enabled = true;
                    return;
                }

                string cedula = cliente_actual.ACCOUNTNUM;
                string msj_error;
                promo_list = new List<List<VW_PromoChoose>>();
                core_parametro parametro = new core_parametro(); // Objeto para la validación - Tarjeta de Descuento Española
                                                                 //core_TarjetaDescuento tarjetaDescuento = new core_TarjetaDescuento(); // Modelo que tenga la info para el Descuento
                #region Facturación

                seguimiento.AppendLine("Antes de entrar al metodo de validar factura");
                if (_factura.validar())
                {
                    seguimiento.AppendLine("Validacion de factura finalizada y aprobada");
                    POSEntities db = new POSEntities();

                    #region Grabar Factura
                    //evelasco. valida secuencia antes de grabar.
                    //valida el número de secuencia entre el Message Queue, Numero de Factura y Documento_secuencia, el mayor de ellos es el valido para la secuencia.
                    try
                    {
                        long secuenciavalida = 0;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Valida el número de Secuencia sea el último para que no de error de UNIQUE  KEY al grabar.");
                        secuenciavalida = GetSecuenciaValidada();
                        if (secuenciavalida > _factura.Secuencia)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Se ha detectado que el número de secuencia actual '" + _factura.Secuencia.ToString() + "' no es actualizado. se va actualizar el número de secuencia '" + secuenciavalida.ToString() + "' para evitar problemas de Unique Key al grabar la factura..");
                            _factura.Secuencia = secuenciavalida;
                        }
                    }
                    catch (Exception ex1)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "Error al Validar la Secuencia.");
                    }

                    seguimiento.AppendLine("Antes de entrar al metodo de grabar factura");
                    //eevv .ini  //valida si tiene apertura manual, si es asi; verifica si ya tiene una apertura de caja desde AX y asigna el valor de IDCAJA.
                    if (Control.Common.GlobalParameters.TieneAperturaCajaManual)
                    {
                        POS.Control.POS.TieneAperturaCajaAX(_factura.User.username);
                    }


                    Thread.Sleep(500);
                    this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Grabando Factura.. por favor espere"; }));



                    if (_factura.grabar(out msj_error))
                    {

                        seguimiento.AppendLine("Factura grabada en base de datos, procediendo a eliminar temporales de factura");
                        EliminaFacturaTmpFile();    //eliminaFacturatmp();
                        seguimiento.AppendLine("Temporales de factura eliminados");
                        flagDescuentoBarra = false;     //Reseteo indicador de descuento Barra
                        btnEliminarPago.Visible = true; //Visualizo Boton por Promo Compra Gratis
                        btnPagoAtras.Visible = true;    //Visualizo Boton por Promo Compra Gratis
                        flagCompraGratis = false;       //Reinicio Promo Compra Gratis
                        seguimiento.AppendLine("Antes de entrar al metodo grabarPromocionEspanola");
                        grabarPromocionEspanola(msj_error);
                        seguimiento.AppendLine("Finalizado metodo grabarPromocionEspanola");
                        seguimiento.AppendLine("Antes de entrar al metodo grabarPromocionCodigo");
                        grabarPromocionCodigo(msj_error);
                        seguimiento.AppendLine("Finalizado metodo grabarPromocionCodigo");

                        grabarConsumoCompraGratis(msj_error, ClienteCompraGratis);
                        seguimiento.AppendLine("Finalizado metodo grabarConsumoCompraGratis");
                        grabarAcumulaCompraGratis(msj_error, _factura);
                        seguimiento.AppendLine("Finalizado metodo grabarAcumulaCompraGratis");

                        grabarCodigoCuponPromocional(msj_error);
                        seguimiento.AppendLine("Finalizado metodo grabarCodigoCuponPromocional");

                        if (_factura.ObjCuponApp != null)
                        {
                            if (_factura.ObjCuponApp.IdTblPremio >= 0)
                            {
                                if (_factura.ObjCuponApp.SeUsoCuponApp == false)
                                {
                                    Control.Common.General.GetMensajeToList(585);
                                    //Control.Common.General.GetMensaje("POS", "No se aplicó cupón promocional en esta factura. No lleva productos de la promoción, el cupón sigue activo", "I");
                                    // System.Windows.Forms.MessageBox.Show(this, "No se aplicó cupón promocional en esta factura. No lleva productos de la promoción, el cupón sigue activo", "No aplica Cupón App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }

                        //Acumular puntos.   
                        POS.Control.WalletPoints.ClsAcumulacion acumuladorPuntos = new Control.WalletPoints.ClsAcumulacion();
                        acumuladorPuntos.AcumularSQL(ref _factura, listPoints, codigoclienteAPP);
                        btnMonedero.Visible = false;
                        btnCuponApp.Visible = false;
                        //Agregar lineas de XmlAcumulaPuntos
                        POS.Control.Common.Logger.Agregar_Trace_PagoCanjePuntos(Control.Common.GlobalParameters.XmlAcumulaPuntos);
                        try
                        {

                            Thread.Sleep(500);
                            this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Se imprime Factura Generada"; }));
                            
                            //Enviar a imprimir
                            _factura.prepararImpresion();

                            if (Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC == true)
                            {
                                _factura.Recibo = _factura.Recibo.Replace(("Tarifa " + Control.Common.GlobalParameters.IVAGEN + "%"), ("Tarifa " + Control.Common.GlobalParameters.IVA_ANTERIOR + "%"));
                                _factura.Recibo = _factura.Recibo.Replace(("I.V.A. " + Control.Common.GlobalParameters.IVAGEN + "%"), ("I.V.A. " + Control.Common.GlobalParameters.IVA_ANTERIOR + "%"));
                            }

                            Control.Common.Printer.Imprimir(_factura.Recibo, 3, 11);

                            if (_factura.ReciboCovid19 != "")
                                Control.Common.Printer.Imprimir(_factura.ReciboCovid19, 3, 11);

                            LimpiarClienteCompraGratis();

                            if (_factura.Documento == "F")
                            {
                                _factura.prepararImpresionCupones(_factura.GetTotal(), _factura.Cliente_grupo);
                            }

                            foreach (var fc in _factura.Cupon)
                            {
                                Thread.Sleep(500);
                                this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Se han encontrado cupones para impreción"; }));

                                if (fc.Unico)
                                {
                                    if ((fc.Valorgiftcard != 999M) || (_factura.ClienteIdentificacion != "9999999999999" && fc.Valorgiftcard == 999M))
                                        Control.Common.Printer.Imprimir(fc.Texto, (fc.Referencia == "PERDIDAPARQUEO") ? 5 : 3, 11);
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar", "Se imprime cupon correctamente");
                                    //imprimir(fc.Texto);

                                }
                                else
                                {
                                    for (int i = 1; i <= Math.Truncate(_factura.GetTotal() / fc.Valor); i++)
                                    {
                                        ///Cupon con GiftCard
                                        if (fc.Giftcard == true)
                                        {
                                            string coded = "11" + i.ToString().PadLeft(2, '0') + (_factura.getNumeroFacturaGiftcard().Replace("-", "")).Replace("F", "");

                                            var pos = new POSEntities();
                                            var giftcard = new core_giftcard();
                                            giftcard.fecha_creacion = DateTime.Now;
                                            giftcard.fecha_modificacion = DateTime.Now;
                                            giftcard.fecha_activacion = DateTime.Now;
                                            giftcard.fecha_expiracion = DateTime.Now.AddDays(1);
                                            giftcard.activo = true;
                                            giftcard.bono = true;
                                            giftcard.codigo = coded;
                                            giftcard.saldo = fc.Valorgiftcard;

                                            pos.core_giftcard.Add(giftcard);
                                            pos.SaveChanges();

                                            //Agregar lineas de insert
                                            Control.Common.Logger.Agregar_Trace_Giftcard(giftcard);

                                            StringBuilder lineas_impresion = new StringBuilder();
                                            addCL(printer, lineas_impresion, fc.Texto);

                                            Barcode bc = new Barcode();
                                            coded = bc.encodeString(coded);
                                            addCL(printer, lineas_impresion, "<barcode>" + "" + coded + "</barcode>");
                                            printer.TextToPrint = lineas_impresion.ToString();
                                            printer.Print();

                                        }
                                        else
                                        {
                                            if ((fc.Valorgiftcard != 999M) || (_factura.ClienteIdentificacion != "9999999999999" && fc.Valorgiftcard == 999M))//999 Consumidor final no imprime
                                                Control.Common.Printer.Imprimir(fc.Texto, 3, 11);
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar", "Se imprime cupon correctamente");
                                            //imprimir(fc.Texto);
                                        }
                                    }
                                }
                            }


                            CreaVouchers();

                            Thread.Sleep(500);
                            this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Impresión Tickets"; }));

                            foreach (var fp in _factura.Voucher)
                            {
                                imprimir(fp.Texto);

                                imprimir(fp.Texto);
                            }

                            if (_factura.activeInvoiceWinner == true)
                            {
                                Thread.Sleep(500);
                                this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Impresión Tickets - Winner"; }));

                                _factura.PrintWonTicket();
                                imprimir(_factura.Recibo);

                                


                                if (_factura.flagMain == true)
                                {
                                    WonForm won = new WonForm();
                                    won.ShowDialog();

                                    if (ValidateAuthorizationUser(won.clave))
                                    {
                                        Control.Common.General.GetMensajeToList(217);
                                    }
                                    else
                                    {
                                        Control.Common.General.GetMensajeToList(218);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            List<ParametrosMensajes> parametrosError = new List<ParametrosMensajes>();
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[Exception]", valor = Control.Common.ExceptionHandler.GetExceptionMessages(ex) });
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[StackTrace]", valor = ex.StackTrace });
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[Message]", valor = ex.Message.ToString() });

                            Control.Common.General.GetMensajeToList(219, parametrosError);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click",
                                "Ocurrió una novedad durante la impresión de recibos, a continuacion las excepciones encontradas - "
                                + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                            // System.Windows.Forms.MessageBox.Show(this, "Ocurrió un problema al imprimir. Detalle: " + ex.Message.ToString(), "Impresión", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            // Control.Common.General.GetMensaje("POS", $"Ocurrió un problema al imprimir. Detalle: {ex.Message.ToString()}", "I");
                        }

                        try
                        {
                            clearClienteData();

                            var factura = new Factura();
                            factura.Establecimiento = _factura.Establecimiento;
                            factura.PtoEmision = _factura.PtoEmision;
                            factura.Secuencia = _factura.Secuencia;
                            factura.Documento = _factura.Documento;
                            factura.Ip_address = _factura.Ip_address;
                            factura.Establecimiento_nombre = _factura.Establecimiento_nombre;
                            factura.Establecimiento_direccion = _factura.Establecimiento_direccion;
                            factura.Establecimiento_telefono = _factura.Establecimiento_telefono;
                            factura.Autorizacion = _factura.Autorizacion;
                            factura.Fecha_inicio_autorizacion = _factura.Fecha_inicio_autorizacion;
                            factura.Fecha_fin_autorizacion = _factura.Fecha_fin_autorizacion;
                            factura.UsaBalanza = _factura.UsaBalanza;
                            factura.UsarScannerIntegrado = _factura.UsarScannerIntegrado;
                            factura.PuertoBalanza = _factura.PuertoBalanza;
                            factura.MarcaBalanza = _factura.MarcaBalanza;
                            factura.Nombre_sucursal = _factura.Nombre_sucursal;
                            factura.Direccion_sucursal = _factura.Direccion_sucursal;
                            factura.Direccion_matriz = _factura.Direccion_matriz;
                            factura.Razon_social_matriz = _factura.Razon_social_matriz;
                            factura.Ruc_matriz = _factura.Ruc_matriz;
                            factura.Recibo = _factura.Recibo;
                            factura.User = _factura.User;


                            _factura = factura;
                            enlazarControles();
                            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;

                            lblTotal.Text = string.Format("{0:C}", 0);
                            lblTotal2.Text = lblTotal.Text;
                            lblRestante.Text = string.Format("{0:C}", 0);
                            lblCambio.Text = string.Format("{0:C}", 0);

                            if (POS.Control.POS.actualizarSecuencia(_factura))
                            {
                                clearClienteData();
                                _factura = null;
                                _factura = new Factura();
                                _factura.User = this._current_user;
                                if (POS.Control.POS.init(ref _factura))
                                {
                                    lblPedidoOtrasApp.Text = string.Empty;
                                    lblPedidoOtrasApp.Visible = false;
                                    //_factura.User = this._current_user;
                                    setTituloDocumento();
                                    enlazarControles();
                                    calcularFactura();
                                    this.splitControl.Panel2Collapsed = true;

                                    this.aplicaDsctoPromoTarjetaBines = false;
                                    this.DescuentoPromoTarjBines = 0;

                                    RecargarParametrosCadaFactura();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "Ocurrió un problema en el bloque de código donde se actualiza el secuencial, a continuación el detalle de lo ocurrido - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "StackTrace:" + Environment.NewLine + ex.StackTrace);

                            List<ParametrosMensajes> parametrosError = new List<ParametrosMensajes>();
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[Message]", valor = ex.Message.ToString() });
                            Control.Common.General.GetMensajeToList(220, parametrosError);

                            Control.Common.General.GetMensajeToList(221, parametrosError);
                            POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                            Application.Exit();
                        }


                    }
                    else
                    {
                        if (!_factura.EsPedidoOtraApp)
                        {
                            seguimiento.AppendLine("Factura no grabada, avisando al cajero");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Factura no grabada, esto pudo deberse a un problema de clave primaria, se intentará ejecutar el procedimiento SP_SECUENCIAS_IG en base de datos");
                            ShowDesktopAlert("Aviso", "Ocurrio un problema al grabar la factura no puede ser procesada. \n" + msj_error.ToString(), autoCloseDelay: 20);
                            POSEntities db1 = new POSEntities();
                            //db1.SP_SECUENCIAS_IG("U");
                            seguimiento.AppendLine("Antes de ejecutar SP_SECUENCIAS_IG");

                            db1.Database.ExecuteSqlCommand("SP_SECUENCIAS_IG @Estado", new SqlParameter("@Estado", "U"));
                            seguimiento.AppendLine("Ejecutado procedimiento SP_SECUENCIAS_IG. Procediendo a resetear datos principales en la factura");

                            POS.Control.POS.init(ref _factura);
                            seguimiento.AppendLine("Datos principales de factura reinicializados");
                            btnGrabar.Enabled = true;
                            return;
                        }
                        else
                        {
                            btnGrabar.Enabled = true;
                            return;
                        }
                    }
                    #endregion

                    #region PAVIPLAN
                    //****  VERIFICA SI EL CLIENTE POSEE PAVIPLAN  ****//

                    seguimiento.AppendLine("Antes de verificar paviplan");

                    if (POS.Control.Common.Promo.EstaActivaPromoPaviPlan())
                    {
                        seguimiento.AppendLine("Antes de consultar vw_CreditoLocalesCabecera");


                        POSEntities db2 = new POSEntities();
                        var query = (from n in db2.vw_CreditoLocalesCabecera
                                     where n.CLIENTE == cedula && (n.ESTADO != 2 || n.ANULADO != 0)
                                     select n).FirstOrDefault();

                        seguimiento.AppendLine("Antes de consultar finalizar");

                        if (query != null)
                        {
                            var result = Control.Common.General.GetMensajeToList(222);
                            // var result = Control.Common.General.GetMensaje("POS - PaviPlan", "El cliente posee PaviPLAN. ¿Consulte si desea realizar un abono?", "Q");

                            if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                            {
                                ejecutaVentanaPaviplan(cedula);
                            }

                            //DialogResult dr = System.Windows.Forms.MessageBox.Show(this, "El cliente posee PaviPLAN. ¿Consulte si desea realizar un abono?", "PaviPLAN", MessageBoxButtons.YesNo);
                            //if (dr == DialogResult.Yes)
                            //{
                            //    ejecutaVentanaPaviplan(cedula);
                            //}
                        }


                        seguimiento.AppendLine("Finalizando verficacion paviplan");

                    }//** FIN VERIFICACION PAVIPLAN
                    #endregion

                    // Activa botones deshabilitados x CF
                    seguimiento.AppendLine("Reactivando txtCedula");
                    txtCedula.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnCFinal");
                    btnCFinal.Enabled = true;
                    seguimiento.AppendLine("Reactivando txtCodigo");
                    txtCodigo.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnBorrarProducto");
                    btnBorrarProducto.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnSearchPro");
                    btnSearchPro.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnQtyProduct");
                    btnQtyProduct.Enabled = true;
                    seguimiento.AppendLine("Final de reactivaciones y de proceso");
                }
                else
                {
                    _validafactura = false;
                    ShowDesktopAlert("Aviso", "Revise los pagos, los pagos que no sean efectivo no pueden ser mayor a total cuando se usa más de un método de pago.", autoCloseDelay: 5);

                }
                #endregion

                VerificarSecuencialFactura(); //evelasco
            }
            catch (Exception ex)
            {
                string detalleEx = string.Concat(POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), seguimiento.ToString());

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + detalleEx, "StackTrace: " + ex.StackTrace);

                var xmlRespuesta = POS.Control.Common.Mail.EnviaCorreo(
                    Properties.Settings.Default.MAILERROR_FROM,
                    Properties.Settings.Default.MAILERROR_ALIAS,
                    Properties.Settings.Default.MAILERROR_DESTINO,
                    Properties.Settings.Default.MAILERROR_CC,
                    Properties.Settings.Default.MAILERROR_MOTIVO,
                    String.Format("Establecimiento: {0} \nPto Emision: {1} \nSecuencia: {2} \nDocumento: {3} \nIpMaquina: {4} \nCajeroNombre: {5} \nCajeroId: {6} \n\nDatos Excepcion ------------\nClass: {7} \nMethod: {8} \nMessage: {9} \nStackTrace: {10}",
                                    this._factura.Establecimiento,
                                    this._factura.PtoEmision,
                                    this._factura.Secuencia,
                                    this._factura.Documento,
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.UserObj.nombres,
                                    Control.Common.GlobalParameters.UserObj.username,
                                    "MainWindow",
                                    "btnGrabar_Click",
                                    detalleEx,
                                    ex.StackTrace),
                    false,
                    String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }
            }

            Thread.Sleep(500);
            this.Invoke(new Action(() => { loading.lblSeguimientoEvent.Text = "Venta culminada con éxito"; }));

            btnGrabar.Enabled = true;
            //si la factura esta ok entonces llama al menu Inicial.
            if (_validafactura)
            {
                llamaMenuInicial();
            }

            //// Una vez terminado el proceso, cerrar el SplashForm y mostrar el MainForm
            //for (int i = 0; i <= 100; i++)
            //{
            //    Thread.Sleep(50); // Simula tiempo de procesamiento (50 ms por cada paso)

            //    loading.progressBar1.Visible = true;
            //    this.Invoke(new Action(() =>
            //    {
            //        // Actualiza el ProgressBar con el valor de progreso
            //        loading.progressBar1.Value = i;
            //    }));
            //}

            // Una vez terminado el proceso, cerrar el SplashForm y mostrar el MainForm
            this.Invoke(new Action(() =>
            {
                loading.Close();  // Cerrar el SplashForm
                this.Show();  // Mostrar el formulario principal
            }));


            this.TopMost = true;
        }

        private void ejecutaGrabar()
        {


            POS.Control.Common.GlobalParameters.DatoRetencionEnFactura = false;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            if (!POS.Control.Common.GlobalParameters.PosEnServidorCorrecto && POS.Control.Common.GlobalParameters.PresentarMensajeServidorIncorrecto)
            {
                Control.Common.General.GetMensajeToList(175);
                //System.Windows.Forms.MessageBox.Show(this, "Está grabando la factura en el servidor incorrecto, por favor cambiar al servidor correcto.", "Servidor incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (txtCedula.Text != "9999999999999" && txtCedula.Text.ToString().Length == 13) //RUC
            {
                if (POS.Control.Common.GlobalParameters.PreguntarSiImprimeDatoRetencion == true)
                {
                    Control.Common.General.GetMensajeToList(176);
                    // System.Windows.Forms.MessageBox.Show("En la parte inferior de la factura encontrará los datos para generar la retención en el caso de requerirlo.", "POS", MessageBoxButtons.OK);
                    POS.Control.Common.GlobalParameters.DatoRetencionEnFactura = true;

                }
                else
                {
                    if (POS.Control.Common.GlobalParameters.ImprimeDatoRetencion == true)
                    {
                        POS.Control.Common.GlobalParameters.DatoRetencionEnFactura = true;
                    }
                }
            }


            bool _validafactura = true;
            List<ParametrosMensajes> parametrosMensajes = new List<ParametrosMensajes>();

            if (SolicitaParqueoEmergente()) return;

            if (AplicaPromoDescuentoProductos != "")
            {

                parametrosMensajes.Add(new ParametrosMensajes() { codigo = "[AplicaPromoDescuentoProductos]", valor = AplicaPromoDescuentoProductos });

                var resutl = Control.Common.General.GetMensajeToList(177, parametrosMensajes);
                if (resutl == MsgBoxCtrl.MessageBoxResult.Ok || resutl == MsgBoxCtrl.MessageBoxResult.Yes)
                {
                    return;
                }

                //if (System.Windows.Forms.MessageBox.Show("Promoción % descuento en: " + AplicaPromoDescuentoProductos + "\n" + ". Desea aplicar?", "Promoción POS", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //    return;
                //}

            }


            GenerarSalidaParqueo();

            GenerarSalidaPorPerdida();

            //Validar que pagos Giftcard tipo GrupoCliente solo puedan salir a nombre de la identificacion principal
            var pagosGiftCard = _factura.Pagos.Where(x => x.Descripcion == "GIFT CARD" || x.Descripcion == "GIFT CARDV").ToList();
            if (pagosGiftCard.Count > 0)
            {
                var gcGrupoClienteNoFactura = (PagoGiftCard)pagosGiftCard.SelectMany(x => x.Pagos).Where(y => ((PagoGiftCard)y).EstaAsociadaGrupoCliente
                                                            &&
                                                            !(((PagoGiftCard)y).IdentificacionGrupoCliente.Equals(_factura.ClienteIdentificacion))
                                                      ).FirstOrDefault();


                if (gcGrupoClienteNoFactura != null)
                {


                    string MsjError = string.Empty;
                    MsjError = string.Concat(MsjError, "Hemos detectado que está usando una(s) giftcard de GrupoCliente");
                    MsjError = string.Concat(MsjError, Environment.NewLine, Environment.NewLine);
                    MsjError = string.Concat(MsjError, $" - {gcGrupoClienteNoFactura.NombreGrupoCliente}");
                    MsjError = string.Concat(MsjError, Environment.NewLine, Environment.NewLine);
                    MsjError = string.Concat(MsjError, $"por lo que la factura debe salir a nombre del cliente principal. Giftcard => '{gcGrupoClienteNoFactura.Codigo}'");
                    //Control.Common.General.GetMensaje("POS", MsjError, "I");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = MsjError });
                    Control.Common.General.GetMensajeToList(584, parametros);


                    MsjError = string.Empty;
                    MsjError = string.Concat(MsjError, $"Procediendo a cambiar la factura con el cliente principal del grupo : '{gcGrupoClienteNoFactura.IdentificacionGrupoCliente}'");
                    //Control.Common.General.GetMensaje("POS", MsjError, "I");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[MsjError]", valor = MsjError });
                    Control.Common.General.GetMensajeToList(584, parametros);

                    //Control.Common.WinForm.ShowMessage("Hemos detectado que está usando una(s) giftcard de GrupoCliente " 
                    //    + Environment.NewLine 
                    //    + Environment.NewLine 
                    //    + " -" + gcGrupoClienteNoFactura.NombreGrupoCliente 
                    //    + Environment.NewLine 
                    //    + Environment.NewLine 
                    //    + " por lo que la factura debe salir a nombre del cliente principal. Giftcard => '" + gcGrupoClienteNoFactura.Codigo + "'");
                    //Control.Common.WinForm.ShowMessage("Procediendo a cambiar la factura con el cliente principal del grupo : '" + gcGrupoClienteNoFactura.IdentificacionGrupoCliente + "'");

                    cambiarCliente(gcGrupoClienteNoFactura.IdentificacionGrupoCliente);
                    return;
                }
            }

            //Validar que pagos TAR PORTAL solo puedan ser usados por el titular de la cuenta 
            POS.Control.Pagos.ClsTarjetaCreditoInterno agenteTarjetaInterna = new Control.Pagos.ClsTarjetaCreditoInterno();
            if (!agenteTarjetaInterna.TieneYPuedeRealizarPagoTarPortal(_factura))
            {
                return;
            }

            btnGrabar.Enabled = false;

            //Registrar en log el click del boton Grabar y cantidad de Pagos/Productos en el momento
            string strLogPagosProductos = "Usuario ha pulsado botón Grabar, la factura en este momento es '" + (_factura == null ? "Objeto _factura está nulo" : _factura.GetNumeroFactura()) + "'. Total factura: " + lblTotal2.Text + ". ";
            if (_factura == null)
            {
                strLogPagosProductos += "Informacion de cantidad Pagos/Productos no disponible porque el objeto _factura estaba en nulo";
            }
            else
            {
                if (_factura.Pagos == null)
                    strLogPagosProductos += "Cantidad de Pagos no disponible porque el objeto _factura.Pagos estaba en nulo. ";
                else
                    strLogPagosProductos += "Cantidad de Pagos: " + _factura.Pagos.Count.ToString() + ". ";


                if (_factura.Productos == null)
                    strLogPagosProductos += "Cantidad de Productos no disponible porque el objeto _factura.Productos estaba en nulo. ";
                else
                    strLogPagosProductos += "Cantidad de Productos: " + _factura.Productos.Count.ToString() + ". ";
            }

            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", strLogPagosProductos);

            //Control.Common.Promo.EjecutarPromoRegala(ref _factura);
            Control.Common.Promo.EjecutarPromoRegalaStock(ref _factura, _factura.EsUsoAppMovil);

            //Revisar si se está facturando con una version actualizada
            var verificador = new POS.Control.Security.Environment();
            verificador.CheckHasLatestVersion();

            StringBuilder seguimiento = new StringBuilder();
            seguimiento.AppendLine(" Inicia seguimiento:");
            try
            {
                foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
                {
                    if (lang.Culture.EnglishName.ToUpper() == "ENGLISH (UNITED STATES)")
                    {
                        InputLanguage.CurrentInputLanguage = lang;
                    }
                }

                seguimiento.AppendLine("Realizado cambio de configuracion regional");

                using (POSEntities pos = new POSEntities())
                {
                    seguimiento.AppendLine("Antes de validar maximo Cupo CF");
                    if (_factura.GetTotal() > Control.Common.GlobalParameters.CUPO_CF && _factura.ClienteIdentificacion == "9999999999999")
                    {
                        seguimiento.AppendLine("POS detecto que cliente es CF y que excedio el maximo Cupo CF");
                        DialogResult dr = new DialogResult();
                        TopeCF frm = new TopeCF();
                        while (dr != DialogResult.OK && dr != DialogResult.Retry)
                        {
                            dr = frm.ShowDialog();

                        }
                        switch (dr)
                        {
                            case DialogResult.OK:
                                // Activa botones deshabilitados x CF
                                txtCedula.Enabled = true;
                                btnCFinal.Enabled = false;
                                txtCodigo.Enabled = false;
                                btnBorrarProducto.Enabled = false;
                                btnSearchPro.Enabled = false;
                                btnQtyProduct.Enabled = false;

                                break;
                            case DialogResult.Retry:
                                /*
                              //  MessageBox.Show(this,"El producto "+gridItems.SelectedRows[0].Cells[0].Value+" Cantidad: "+ gridItems.SelectedRows[0].Cells[1].Value + "\nDebe retirar de lo despachado" );
                                txtCedula.Enabled = false;
                                btnCFinal.Enabled = false;
                                txtCodigo.Enabled = false;
                                btnBorrarProducto.Enabled = false;
                                btnSearchPro.Enabled = false;
                                btnQtyProduct.Enabled = false;
                              //  gridItems.SelectedRows[0].Delete();
                              */
                                btnGrabar.Enabled = true;
                                return;
                        }
                    }
                    seguimiento.AppendLine("Finaliza validacion maximo Cupo CF");
                }

                if (cliente_actual == null)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Botón Grabar fue pulsado pero POS no tiene un cliente cargado en la variable 'cliente_actual'. Se da por terminado el método");
                    btnGrabar.Enabled = true;
                    return;
                }

                string cedula = cliente_actual.ACCOUNTNUM;
                string msj_error;
                promo_list = new List<List<VW_PromoChoose>>();
                core_parametro parametro = new core_parametro(); // Objeto para la validación - Tarjeta de Descuento Española
                                                                 //core_TarjetaDescuento tarjetaDescuento = new core_TarjetaDescuento(); // Modelo que tenga la info para el Descuento
                #region Facturación

                seguimiento.AppendLine("Antes de entrar al metodo de validar factura");
                if (_factura.validar())
                {
                    seguimiento.AppendLine("Validacion de factura finalizada y aprobada");
                    POSEntities db = new POSEntities();

                    #region Grabar Factura
                    //evelasco. valida secuencia antes de grabar.
                    //valida el número de secuencia entre el Message Queue, Numero de Factura y Documento_secuencia, el mayor de ellos es el valido para la secuencia.
                    try
                    {
                        long secuenciavalida = 0;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Valida el número de Secuencia sea el último para que no de error de UNIQUE  KEY al grabar.");
                        secuenciavalida = GetSecuenciaValidada();
                        if (secuenciavalida > _factura.Secuencia)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Se ha detectado que el número de secuencia actual '" + _factura.Secuencia.ToString() + "' no es actualizado. se va actualizar el número de secuencia '" + secuenciavalida.ToString() + "' para evitar problemas de Unique Key al grabar la factura..");
                            _factura.Secuencia = secuenciavalida;
                        }
                    }
                    catch (Exception ex1)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "Error al Validar la Secuencia.");
                    }

                    seguimiento.AppendLine("Antes de entrar al metodo de grabar factura");
                    //eevv .ini  //valida si tiene apertura manual, si es asi; verifica si ya tiene una apertura de caja desde AX y asigna el valor de IDCAJA.
                    if (Control.Common.GlobalParameters.TieneAperturaCajaManual)
                    {
                        POS.Control.POS.TieneAperturaCajaAX(_factura.User.username);
                    }
                    //eevv .fin
                    if (_factura.grabar(out msj_error))
                    {

                        seguimiento.AppendLine("Factura grabada en base de datos, procediendo a eliminar temporales de factura");
                        EliminaFacturaTmpFile();    //eliminaFacturatmp();
                        seguimiento.AppendLine("Temporales de factura eliminados");
                        flagDescuentoBarra = false;     //Reseteo indicador de descuento Barra
                        btnEliminarPago.Visible = true; //Visualizo Boton por Promo Compra Gratis
                        btnPagoAtras.Visible = true;    //Visualizo Boton por Promo Compra Gratis
                        flagCompraGratis = false;       //Reinicio Promo Compra Gratis
                        seguimiento.AppendLine("Antes de entrar al metodo grabarPromocionEspanola");
                        grabarPromocionEspanola(msj_error);
                        seguimiento.AppendLine("Finalizado metodo grabarPromocionEspanola");
                        seguimiento.AppendLine("Antes de entrar al metodo grabarPromocionCodigo");
                        grabarPromocionCodigo(msj_error);
                        seguimiento.AppendLine("Finalizado metodo grabarPromocionCodigo");

                        grabarConsumoCompraGratis(msj_error, ClienteCompraGratis);
                        seguimiento.AppendLine("Finalizado metodo grabarConsumoCompraGratis");
                        grabarAcumulaCompraGratis(msj_error, _factura);
                        seguimiento.AppendLine("Finalizado metodo grabarAcumulaCompraGratis");

                        grabarCodigoCuponPromocional(msj_error);
                        seguimiento.AppendLine("Finalizado metodo grabarCodigoCuponPromocional");

                        if (_factura.ObjCuponApp != null)
                        {
                            if (_factura.ObjCuponApp.IdTblPremio >= 0)
                            {
                                if (_factura.ObjCuponApp.SeUsoCuponApp == false)
                                {
                                    Control.Common.General.GetMensajeToList(585);
                                    //Control.Common.General.GetMensaje("POS", "No se aplicó cupón promocional en esta factura. No lleva productos de la promoción, el cupón sigue activo", "I");
                                    // System.Windows.Forms.MessageBox.Show(this, "No se aplicó cupón promocional en esta factura. No lleva productos de la promoción, el cupón sigue activo", "No aplica Cupón App", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }

                        //Acumular puntos.   
                        POS.Control.WalletPoints.ClsAcumulacion acumuladorPuntos = new Control.WalletPoints.ClsAcumulacion();
                        acumuladorPuntos.AcumularSQL(ref _factura, listPoints, codigoclienteAPP);
                        btnMonedero.Visible = false;
                        btnCuponApp.Visible = false;
                        //Agregar lineas de XmlAcumulaPuntos
                        POS.Control.Common.Logger.Agregar_Trace_PagoCanjePuntos(Control.Common.GlobalParameters.XmlAcumulaPuntos);
                        try
                        {
                            //Enviar a imprimir
                            _factura.prepararImpresion();

                            if (Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC == true)
                            {
                                _factura.Recibo = _factura.Recibo.Replace(("Tarifa " + Control.Common.GlobalParameters.IVAGEN + "%"), ("Tarifa " + Control.Common.GlobalParameters.IVA_ANTERIOR + "%"));
                                _factura.Recibo = _factura.Recibo.Replace(("I.V.A. " + Control.Common.GlobalParameters.IVAGEN + "%"), ("I.V.A. " + Control.Common.GlobalParameters.IVA_ANTERIOR + "%"));
                            }

                            Control.Common.Printer.Imprimir(_factura.Recibo, 3, 11);

                            if (_factura.ReciboCovid19 != "")
                                Control.Common.Printer.Imprimir(_factura.ReciboCovid19, 3, 11);

                            LimpiarClienteCompraGratis();

                            if (_factura.Documento == "F")
                            {
                                _factura.prepararImpresionCupones(_factura.GetTotal(), _factura.Cliente_grupo);
                            }

                            foreach (var fc in _factura.Cupon)
                            {
                                if (fc.Unico)
                                {
                                    if ((fc.Valorgiftcard != 999M) || (_factura.ClienteIdentificacion != "9999999999999" && fc.Valorgiftcard == 999M))
                                        Control.Common.Printer.Imprimir(fc.Texto, (fc.Referencia == "PERDIDAPARQUEO") ? 5 : 3, 11);
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar", "Se imprime cupon correctamente");
                                    //imprimir(fc.Texto);

                                }
                                else
                                {
                                    for (int i = 1; i <= Math.Truncate(_factura.GetTotal() / fc.Valor); i++)
                                    {
                                        ///Cupon con GiftCard
                                        if (fc.Giftcard == true)
                                        {
                                            string coded = "11" + i.ToString().PadLeft(2, '0') + (_factura.getNumeroFacturaGiftcard().Replace("-", "")).Replace("F", "");

                                            var pos = new POSEntities();
                                            var giftcard = new core_giftcard();
                                            giftcard.fecha_creacion = DateTime.Now;
                                            giftcard.fecha_modificacion = DateTime.Now;
                                            giftcard.fecha_activacion = DateTime.Now;
                                            giftcard.fecha_expiracion = DateTime.Now.AddDays(1);
                                            giftcard.activo = true;
                                            giftcard.bono = true;
                                            giftcard.codigo = coded;
                                            giftcard.saldo = fc.Valorgiftcard;

                                            pos.core_giftcard.Add(giftcard);
                                            pos.SaveChanges();

                                            //Agregar lineas de insert
                                            Control.Common.Logger.Agregar_Trace_Giftcard(giftcard);

                                            StringBuilder lineas_impresion = new StringBuilder();
                                            addCL(printer, lineas_impresion, fc.Texto);

                                            Barcode bc = new Barcode();
                                            coded = bc.encodeString(coded);
                                            addCL(printer, lineas_impresion, "<barcode>" + "" + coded + "</barcode>");
                                            printer.TextToPrint = lineas_impresion.ToString();
                                            printer.Print();

                                        }
                                        else
                                        {
                                            if ((fc.Valorgiftcard != 999M) || (_factura.ClienteIdentificacion != "9999999999999" && fc.Valorgiftcard == 999M))//999 Consumidor final no imprime
                                                Control.Common.Printer.Imprimir(fc.Texto, 3, 11);
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar", "Se imprime cupon correctamente");
                                            //imprimir(fc.Texto);
                                        }
                                    }
                                }
                            }


                            CreaVouchers();

                            foreach (var fp in _factura.Voucher)
                            {
                                imprimir(fp.Texto);

                                imprimir(fp.Texto);
                            }

                            if (_factura.activeInvoiceWinner == true)
                            {
                                _factura.PrintWonTicket();
                                imprimir(_factura.Recibo);

                                if (_factura.flagMain == true)
                                {
                                    WonForm won = new WonForm();
                                    won.ShowDialog();

                                    if (ValidateAuthorizationUser(won.clave))
                                    {
                                        Control.Common.General.GetMensajeToList(217);
                                        //Control.Common.General.GetMensaje("POS", "¡FELICIDADES ERES EL GANADOR DEL DIA!", "I");
                                        // System.Windows.Forms.MessageBox.Show(this, "¡FELICIDADES ERES EL GANADOR DEL DIA!", "GANADOR", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    else
                                    {
                                        Control.Common.General.GetMensajeToList(218);
                                        //Control.Common.General.GetMensaje("POS", "Codigo incorrecto", "I");
                                        // System.Windows.Forms.MessageBox.Show(this, "Codigo incorrecto", "Código de autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            List<ParametrosMensajes> parametrosError = new List<ParametrosMensajes>();
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[Exception]", valor = Control.Common.ExceptionHandler.GetExceptionMessages(ex) });
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[StackTrace]", valor = ex.StackTrace });
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[Message]", valor = ex.Message.ToString() });

                            Control.Common.General.GetMensajeToList(219, parametrosError);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click",
                                "Ocurrió una novedad durante la impresión de recibos, a continuacion las excepciones encontradas - "
                                + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);

                            // System.Windows.Forms.MessageBox.Show(this, "Ocurrió un problema al imprimir. Detalle: " + ex.Message.ToString(), "Impresión", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            // Control.Common.General.GetMensaje("POS", $"Ocurrió un problema al imprimir. Detalle: {ex.Message.ToString()}", "I");
                        }

                        try
                        {
                            clearClienteData();

                            var factura = new Factura();
                            factura.Establecimiento = _factura.Establecimiento;
                            factura.PtoEmision = _factura.PtoEmision;
                            factura.Secuencia = _factura.Secuencia;
                            factura.Documento = _factura.Documento;
                            factura.Ip_address = _factura.Ip_address;
                            factura.Establecimiento_nombre = _factura.Establecimiento_nombre;
                            factura.Establecimiento_direccion = _factura.Establecimiento_direccion;
                            factura.Establecimiento_telefono = _factura.Establecimiento_telefono;
                            factura.Autorizacion = _factura.Autorizacion;
                            factura.Fecha_inicio_autorizacion = _factura.Fecha_inicio_autorizacion;
                            factura.Fecha_fin_autorizacion = _factura.Fecha_fin_autorizacion;
                            factura.UsaBalanza = _factura.UsaBalanza;
                            factura.UsarScannerIntegrado = _factura.UsarScannerIntegrado;
                            factura.PuertoBalanza = _factura.PuertoBalanza;
                            factura.MarcaBalanza = _factura.MarcaBalanza;
                            factura.Nombre_sucursal = _factura.Nombre_sucursal;
                            factura.Direccion_sucursal = _factura.Direccion_sucursal;
                            factura.Direccion_matriz = _factura.Direccion_matriz;
                            factura.Razon_social_matriz = _factura.Razon_social_matriz;
                            factura.Ruc_matriz = _factura.Ruc_matriz;
                            factura.Recibo = _factura.Recibo;
                            factura.User = _factura.User;


                            _factura = factura;
                            enlazarControles();
                            Control.Common.GlobalParameters.RECALCULAR_IVA12_X_PAGONC = false;

                            lblTotal.Text = string.Format("{0:C}", 0);
                            lblTotal2.Text = lblTotal.Text;
                            lblRestante.Text = string.Format("{0:C}", 0);
                            lblCambio.Text = string.Format("{0:C}", 0);

                            if (POS.Control.POS.actualizarSecuencia(_factura))
                            {
                                clearClienteData();
                                _factura = null;
                                _factura = new Factura();
                                _factura.User = this._current_user;
                                if (POS.Control.POS.init(ref _factura))
                                {
                                    lblPedidoOtrasApp.Text = string.Empty;
                                    lblPedidoOtrasApp.Visible = false;
                                    //_factura.User = this._current_user;
                                    setTituloDocumento();
                                    enlazarControles();
                                    calcularFactura();
                                    this.splitControl.Panel2Collapsed = true;

                                    this.aplicaDsctoPromoTarjetaBines = false;
                                    this.DescuentoPromoTarjBines = 0;

                                    RecargarParametrosCadaFactura();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "Ocurrió un problema en el bloque de código donde se actualiza el secuencial, a continuación el detalle de lo ocurrido - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex) + Environment.NewLine + "StackTrace:" + Environment.NewLine + ex.StackTrace);

                            List<ParametrosMensajes> parametrosError = new List<ParametrosMensajes>();
                            parametrosError.Add(new ParametrosMensajes() { codigo = "[Message]", valor = ex.Message.ToString() });
                            Control.Common.General.GetMensajeToList(220, parametrosError);

                            Control.Common.General.GetMensajeToList(221, parametrosError);

                            //string msjError = string.Empty;
                            //msjError = string.Concat(msjError, $"La factura se grabó pero ocurrió un problema al actualizar el secuencial. Detalle {ex.Message.ToString()}, Secuencia Numérica" );
                            //Control.Common.General.GetMensaje("POS", msjError, "ER");

                            //msjError = string.Empty;
                            //msjError = string.Concat(msjError, "Estimad@ usuario debido a una interrupcion detectada en la red el aplicativo se cerrará para evitar descuadres en su turno. Por favor reinicie el aplicativo para continuar la facturación");
                            //Control.Common.General.GetMensaje("POS", msjError, "ER");

                            //System.Windows.Forms.MessageBox.Show(this, "La factura se grabó pero ocurrió un problema al actualizar el secuencial. Detalle: " + ex.Message.ToString(), "Secuencia Numérica", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            //System.Windows.Forms.MessageBox.Show(this, "Estimad@ usuario debido a una interrupcion detectada en la red el aplicativo se cerrará para evitar descuadres en su turno. Por favor reinicie el aplicativo para continuar la facturación");

                            POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                            Application.Exit();
                        }


                    }
                    else
                    {
                        if (!_factura.EsPedidoOtraApp)
                        {
                            seguimiento.AppendLine("Factura no grabada, avisando al cajero");
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnGrabar_Click", "Factura no grabada, esto pudo deberse a un problema de clave primaria, se intentará ejecutar el procedimiento SP_SECUENCIAS_IG en base de datos");
                            ShowDesktopAlert("Aviso", "Ocurrio un problema al grabar la factura no puede ser procesada. \n" + msj_error.ToString(), autoCloseDelay: 20);
                            POSEntities db1 = new POSEntities();
                            //db1.SP_SECUENCIAS_IG("U");
                            seguimiento.AppendLine("Antes de ejecutar SP_SECUENCIAS_IG");

                            db1.Database.ExecuteSqlCommand("SP_SECUENCIAS_IG @Estado", new SqlParameter("@Estado", "U"));
                            seguimiento.AppendLine("Ejecutado procedimiento SP_SECUENCIAS_IG. Procediendo a resetear datos principales en la factura");

                            POS.Control.POS.init(ref _factura);
                            seguimiento.AppendLine("Datos principales de factura reinicializados");
                            btnGrabar.Enabled = true;
                            return;
                        }
                        else
                        {
                            btnGrabar.Enabled = true;
                            return;
                        }
                    }
                    #endregion

                    #region PAVIPLAN
                    //****  VERIFICA SI EL CLIENTE POSEE PAVIPLAN  ****//

                    seguimiento.AppendLine("Antes de verificar paviplan");

                    if (POS.Control.Common.Promo.EstaActivaPromoPaviPlan())
                    {
                        seguimiento.AppendLine("Antes de consultar vw_CreditoLocalesCabecera");


                        POSEntities db2 = new POSEntities();
                        var query = (from n in db2.vw_CreditoLocalesCabecera
                                     where n.CLIENTE == cedula && (n.ESTADO != 2 || n.ANULADO != 0)
                                     select n).FirstOrDefault();

                        seguimiento.AppendLine("Antes de consultar finalizar");

                        if (query != null)
                        {
                            var result = Control.Common.General.GetMensajeToList(222);
                            // var result = Control.Common.General.GetMensaje("POS - PaviPlan", "El cliente posee PaviPLAN. ¿Consulte si desea realizar un abono?", "Q");

                            if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                            {
                                ejecutaVentanaPaviplan(cedula);
                            }

                            //DialogResult dr = System.Windows.Forms.MessageBox.Show(this, "El cliente posee PaviPLAN. ¿Consulte si desea realizar un abono?", "PaviPLAN", MessageBoxButtons.YesNo);
                            //if (dr == DialogResult.Yes)
                            //{
                            //    ejecutaVentanaPaviplan(cedula);
                            //}
                        }


                        seguimiento.AppendLine("Finalizando verficacion paviplan");

                    }//** FIN VERIFICACION PAVIPLAN
                    #endregion

                    // Activa botones deshabilitados x CF
                    seguimiento.AppendLine("Reactivando txtCedula");
                    txtCedula.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnCFinal");
                    btnCFinal.Enabled = true;
                    seguimiento.AppendLine("Reactivando txtCodigo");
                    txtCodigo.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnBorrarProducto");
                    btnBorrarProducto.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnSearchPro");
                    btnSearchPro.Enabled = true;
                    seguimiento.AppendLine("Reactivando btnQtyProduct");
                    btnQtyProduct.Enabled = true;
                    seguimiento.AppendLine("Final de reactivaciones y de proceso");
                }
                else
                {
                    _validafactura = false;
                    ShowDesktopAlert("Aviso", "Revise los pagos, los pagos que no sean efectivo no pueden ser mayor a total cuando se usa más de un método de pago.", autoCloseDelay: 5);

                }
                #endregion

                VerificarSecuencialFactura(); //evelasco
            }
            catch (Exception ex)
            {
                string detalleEx = string.Concat(POS.Control.Common.ExceptionHandler.GetExceptionMessages(ex), seguimiento.ToString());

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "No se pudo completar la ejecución del método, a continuacion las excepciones encontradas - " + detalleEx, "StackTrace: " + ex.StackTrace);

                var xmlRespuesta = POS.Control.Common.Mail.EnviaCorreo(
                    Properties.Settings.Default.MAILERROR_FROM,
                    Properties.Settings.Default.MAILERROR_ALIAS,
                    Properties.Settings.Default.MAILERROR_DESTINO,
                    Properties.Settings.Default.MAILERROR_CC,
                    Properties.Settings.Default.MAILERROR_MOTIVO,
                    String.Format("Establecimiento: {0} \nPto Emision: {1} \nSecuencia: {2} \nDocumento: {3} \nIpMaquina: {4} \nCajeroNombre: {5} \nCajeroId: {6} \n\nDatos Excepcion ------------\nClass: {7} \nMethod: {8} \nMessage: {9} \nStackTrace: {10}",
                                    this._factura.Establecimiento,
                                    this._factura.PtoEmision,
                                    this._factura.Secuencia,
                                    this._factura.Documento,
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.UserObj.nombres,
                                    Control.Common.GlobalParameters.UserObj.username,
                                    "MainWindow",
                                    "btnGrabar_Click",
                                    detalleEx,
                                    ex.StackTrace),
                    false,
                    String.Empty);

                if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGrabar_Click", "No se pudo enviar email de error durante la ejecución del método, a continuacion el detalle de la excepcion - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                }
            }

            btnGrabar.Enabled = true;
            //si la factura esta ok entonces llama al menu Inicial.
            if (_validafactura)
            {
                llamaMenuInicial();
            }
        }
        private void btnGrabar_Click(object sender, EventArgs e)
        {
            /*Este codigo agrega*/
            //this.TopMost = false;
            //loading = new frmLoading();
            //loading.Show();

            //// Crear un hilo para ejecutar el proceso en segundo plano
            //Thread backgroundThread = new Thread(new ThreadStart(BackgroundGrabarDocumentoProcess));
            //backgroundThread.Start();  // Iniciar el hilo
            ////Thread.Sleep(5000);

            ejecutaGrabar();
            
        }

        #region Métodos para Verificar Tarjeta de Descuento Española - flara
        private Boolean procesarTarjetaDescuentoEspanola(string codigo, string porcentaje, string dias, string numeroFactura, decimal saldo, string codigoCliente)
        {
            string textoMsj = string.Empty;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            try
            {
                if (_factura.Productos.Count() > 0)
                {
                    var db = new POSEntities();
                    var db1 = new POSEntities();
                    //bool flagPrimera = false;

                    string permite_almacen = db.core_parametro.Where(k => k.identificador == "PROMO_ESPANOLA" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor;

                    if (permite_almacen == "TRUE")
                    {
                        if (flagPromocionAcumulado == false)
                        {
                            #region Asignacion por primera vez y descuento de las compras
                            if (codigo != "" && codigo != null)
                            {
                                //int how_times = int.Parse(db.core_parametro.Where(z => z.identificador == "PROMO_ESPANOLA_DIAS" && z.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);

                                core_TarjetaDescuento tarjeta = db1.core_TarjetaDescuento.Where(x => x.codigo == codigo).FirstOrDefault();

                                if (flagPromocionPrimera)
                                {
                                    /*var _list = (from t in db.core_factura
                                                 where
                                                 (t.cliente == this._factura.Cliente_cedula_ruc &&
                                                  t.cliente != "9999999999999")
                                                 && t.fecha_creacion == DateTime.Now
                                                 && t.establecimiento == this._factura.Establecimiento
                                                 && t.documento == "F"
                                                 select t).ToList();*/

                                    #region Factura por Primera vez
                                    if (tarjeta == null)
                                    {
                                        // System.Windows.Forms.MessageBox.Show(this, "Código de Tarjeta " + codigo + " NO EXISTE", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", $"Código de Tarjeta {codigo} NO EXISTE", "I");

                                        parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = codigo });
                                        Control.Common.General.GetMensajeToList(223, parametros);

                                        //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", $"Código de Tarjeta {codigo} NO EXISTE", "I");

                                        return false;
                                    }

                                    if (tarjeta.activo)
                                    {
                                        parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = tarjeta.codigo });
                                        Control.Common.General.GetMensajeToList(224, parametros);

                                        // Control.Common.General.GetMensaje("POS - Tarjeta Descuento", $"La tarjeta  {tarjeta.codigo} ya se encuentra activa, registre otra tarjeta", "I");
                                        // System.Windows.Forms.MessageBox.Show(this, "La tarjeta " + tarjeta.codigo + " ya se encuentra activa, registre otra tarjeta", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                        return false;
                                    }

                                    db1.ActualizarTarjetaDescuentoEspanola(codigo, true, true, int.Parse(dias), saldo, long.Parse(numeroFactura.Substring(10, 9)), this._factura.ClienteIdentificacion);

                                    imprimirEspanola(codigo, saldo.ToString(), "P", DateTime.Now.Date.ToShortDateString(), DateTime.Now.AddDays(int.Parse(dias)).Date.ToShortDateString());

                                    parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = tarjeta.codigo });
                                    parametros.Add(new ParametrosMensajes() { codigo = "[saldo_tarjeta]", valor = saldo.ToString("$ ###,##0.00") });
                                    Control.Common.General.GetMensajeToList(225, parametros);

                                    // Control.Common.General.GetMensaje("POS - Tarjeta Descuento", $"Asignación con éxito de la tarjeta descuento {tarjeta.codigo} por un valor de {saldo.ToString("$ ###,##0.00")} ", "I");
                                    //System.Windows.Forms.MessageBox.Show(this, "Asignación con éxito de la tarjeta descuento " + tarjeta.codigo + " por un valor de " + saldo.ToString("$ ###,##0.00"), "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    #endregion
                                }
                                else
                                {

                                    if (flagPromocionDescuento)
                                    {
                                        if (tarjeta == null)
                                        {
                                            //System.Windows.Forms.MessageBox.Show(this, "Código de Tarjeta " + codigo + " NO EXISTE", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", $"Código de Tarjeta {codigo} NO EXISTE", "I");

                                            parametros = new List<ParametrosMensajes>();
                                            parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = codigo });
                                            Control.Common.General.GetMensajeToList(223, parametros);

                                            return false;
                                        }

                                        #region Descuento de las Compras realizadas
                                        if (tarjeta.activo == false)
                                        {

                                            parametros = new List<ParametrosMensajes>();
                                            parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = tarjeta.codigo });
                                            Control.Common.General.GetMensajeToList(226, parametros);

                                            // Control.Common.General.GetMensaje("POS - Tarjeta Descuento", $"La tarjeta  {tarjeta.codigo} no se encuentra activa, registre otra tarjeta", "I");
                                            // System.Windows.Forms.MessageBox.Show(this, "La tarjeta " + tarjeta.codigo + " no se encuentra activa, registre otra tarjeta", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                            return false;
                                        }

                                        decimal totalDesc = 0;
                                        decimal nuevoSaldo = 0;
                                        bool tarjetaSigueActiva = true;
                                        // if (this._factura.Pagos.Any(x => x.Descripcion == "DSCT_PROMO"))
                                        {
                                            //Proceso de usar tarjeta debe calcularse contra subtotal sin descuentos
                                            saldo = this._factura.getSubTotalSinDescuento();

                                            //Se obtiene el descuento que representa el subtotal de la factura x el porcentaje promocional
                                            totalDesc = decimal.Round((saldo * (decimal.Parse(porcentaje) / 100)), 2);

                                            //Si el descuento excede o iguala al saldo disponible, marcar como inactiva la tarjeta
                                            if (totalDesc >= tarjeta.saldo)
                                            {
                                                totalDesc = tarjeta.saldo;
                                                tarjetaSigueActiva = false;
                                            }

                                            //Calcular el nuevo saldo de la tarjeta
                                            nuevoSaldo = tarjeta.saldo - totalDesc;

                                            db1.ActualizarTarjetaDescuentoEspanola(codigo, true, tarjetaSigueActiva, int.Parse(dias), nuevoSaldo, 0, "");
                                            imprimirEspanola(tarjeta.codigo, nuevoSaldo.ToString(), "D", tarjeta.fecha_activacion.Value.ToShortDateString(), tarjeta.fecha_desactivacion.Value.ToShortDateString());

                                            parametros = new List<ParametrosMensajes>();
                                            parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = tarjeta.codigo });
                                            parametros.Add(new ParametrosMensajes() { codigo = "[saldo_tarjeta]", valor = nuevoSaldo.ToString("$ ###,##0.00") });
                                            Control.Common.General.GetMensajeToList(227, parametros);


                                            //textoMsj = $"Descuento con éxito de la tarjeta {tarjeta.codigo} . Saldo en la tarjeta: {nuevoSaldo.ToString("$ ###,##0.00")} ";
                                            //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I");

                                            // System.Windows.Forms.MessageBox.Show(this, "Descuento con éxito de la tarjeta " + tarjeta.codigo + " - Saldo en la tarjeta: " + nuevoSaldo.ToString("$ ###,##0.00"), "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);

                   
                                            /*totalDesc = tarjeta.saldo - this._factura.getDescuentos();

                                            if (tarjeta.saldo > (saldo * (decimal.Parse(porcentaje) / 100)))
                                            {
                                                db1.ActualizarTarjetaDescuentoEspanola(codigo, true, true, int.Parse(dias), nuevoSaldo, 0, "");
                                                imprimirEspanola(tarjeta.codigo, nuevoSaldo.ToString(), "D", tarjeta.fecha_activacion.Value.ToShortDateString(), tarjeta.fecha_desactivacion.Value.ToShortDateString());
                                                MessageBox.Show(this,"Descuento con éxito de la tarjeta " + tarjeta.codigo + " - Saldo en la tarjeta: " + nuevoSaldo.ToString("$ ###,##0.00"), "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }
                                            else
                                            {
                                                db1.ActualizarTarjetaDescuentoEspanola(codigo, true, false, int.Parse(dias), 0, 0, "");
                                                imprimirEspanola(tarjeta.codigo, nuevoSaldo.ToString(), "D", tarjeta.fecha_activacion.Value.ToShortDateString(), tarjeta.fecha_desactivacion.Value.ToShortDateString());
                                                MessageBox.Show(this,"::::  Descuento con éxito de la tarjeta " + tarjeta.codigo + " - Saldo en la tarjeta: " + 0.ToString("$ ###,##0.00"), "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            }*/

                                            //BRAYEN ESPANOLA
                                            core_TarjetaDescuentoHistorico ctdh = new core_TarjetaDescuentoHistorico();
                                            ctdh.codigoTarjeta = tarjeta.codigo;
                                            ctdh.fecha_Transaccion = DateTime.Now;
                                            ctdh.num_factura = _factura.GetNumeroFactura();
                                            ctdh.valorDescuento = totalDesc; //(saldo * (decimal.Parse(porcentaje) / 100));
                                            db.core_TarjetaDescuentoHistorico.Add(ctdh);
                                            db.SaveChanges();
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        if (flagPromocionAcumulado)
                                        {
                                            decimal totalDesc = 0;

                                            totalDesc = tarjeta.saldo + saldo;

                                            db1.ActualizarTarjetaDescuentoEspanola(codigo, true, true, int.Parse(dias), totalDesc, 0, "");

                                            parametros = new List<ParametrosMensajes>();
                                            parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = tarjeta.codigo });
                                            parametros.Add(new ParametrosMensajes() { codigo = "[saldo_tarjeta]", valor = totalDesc.ToString("$ ###,##0.00") });
                                            Control.Common.General.GetMensajeToList(228, parametros);


                                            //textoMsj = $"Valor acumulado con éxito de la tarjeta {tarjeta.codigo} . Saldo en la tarjeta: {totalDesc.ToString("$ ###,##0.00")} ";
                                            //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I");

                                            // System.Windows.Forms.MessageBox.Show(this, "Valor acumulado con éxito de la tarjeta " + tarjeta.codigo + " - Saldo en la tarjeta: " + totalDesc.ToString("$ ###,##0.00"), "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                            imprimirEspanola(tarjeta.codigo, totalDesc.ToString(), "A", tarjeta.fecha_activacion.Value.ToShortDateString(), tarjeta.fecha_desactivacion.Value.ToShortDateString());
                                        }
                                        else
                                        {
                                            Control.Common.General.GetMensajeToList(229, parametros);


                                            //textoMsj = $"No se obtuvo información de la tarjeta. Por favor deslice la tarjeta por el lector magnético.";
                                            //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "ER");

                                            // System.Windows.Forms.MessageBox.Show(this, "No se obtuvo información de la tarjeta. Por favor deslice la tarjeta por el lector magnético.", "Información de Tarjeta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            return false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //MessageBox.Show(this,"Ingrese código de tarjeta", "Tarjeta de Descuento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return false;
                            }
                            #endregion
                        }
                        else
                        {
                            if (flagPromocionAcumulado)
                            {
                                core_TarjetaDescuento tarjeta = db1.core_TarjetaDescuento.Where(x => x.codigoCliente == codigoCliente).FirstOrDefault();
                                decimal totalDesc = 0;

                                totalDesc = tarjeta.saldo + saldo;

                                db1.ActualizarTarjetaDescuentoEspanola(tarjeta.codigo, true, true, int.Parse(dias), totalDesc, 0, "");

                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[codigo_tarjeta]", valor = tarjeta.codigo });
                                parametros.Add(new ParametrosMensajes() { codigo = "[saldo_tarjeta]", valor = totalDesc.ToString("$ ###,##0.00") });
                                Control.Common.General.GetMensajeToList(228, parametros);


                                //textoMsj = $"Valor acumulado con éxito de la tarjeta {tarjeta.codigo} - Saldo en la tarjeta: {totalDesc.ToString("$ ###,##0.00")} ";
                                //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I");

                                //System.Windows.Forms.MessageBox.Show(this, "Valor acumulado con éxito de la tarjeta " + tarjeta.codigo + " - Saldo en la tarjeta: " + totalDesc.ToString("$ ###,##0.00"), "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                imprimirEspanola(tarjeta.codigo, totalDesc.ToString(), "A", tarjeta.fecha_activacion.Value.ToShortDateString(), tarjeta.fecha_desactivacion.Value.ToShortDateString());
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "No se obtuvo información de la tarjeta. Por favor deslice la tarjeta por el lector magnético.", "Información de Tarjeta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                //textoMsj = $"No se obtuvo información de la tarjeta. Por favor deslice la tarjeta por el lector magnético.";
                                //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I");
                                Control.Common.General.GetMensajeToList(229);


                                return false;
                            }
                        }
                    }
                    else
                    {
                        //textoMsj = $"Tarjeta no válida para este local";
                        //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I");

                        Control.Common.General.GetMensajeToList(230);

                        //System.Windows.Forms.MessageBox.Show(this, "Tarjeta no válida para este local", "Tarjeta de Descuento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                else
                {
                    //textoMsj = $"Tarjeta no válida para este local";
                    //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I", 30);
                    Control.Common.General.GetMensajeToList(230);

                    // MessageBoxTemporal.Show("No existen Artículos en lista", "Artículos", 30, true);
                    //MessageBox.Show(this, "No existen Artículos en lista", "Artículos", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ProcesarTarjetaDescuentoEspanola", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                // System.Windows.Forms.MessageBox.Show(this, "Por favor intente nuevamente", "Cantidad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                //textoMsj = $"Por favor intente nuevamente";
                //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I", 30);

                Control.Common.General.GetMensajeToList(231);


            }
            return true;
        }


        private void imprimirEspanola(string codigoTarjeta, string valor, string tipo, string fechaActivacion, string fechaExpiracion)
        {
            StringBuilder cadena = new StringBuilder();

            if (tipo == "P")
            {
                #region Impresión Asignación Primera Vez
                cadena.AppendLine("<footer>" + "**¡ASIGNACIÓN TARJ!**" + "</footer>");
                cadena.Append("\n");
                cadena.AppendLine(" ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("             COMPRA GRATIS  ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("Codigo Tarjeta  : " + codigoTarjeta);
                cadena.Append("\n");
                cadena.AppendLine("Valor Tarjeta   : " + valor);
                cadena.Append("\n");
                cadena.AppendLine("Fecha Activación: " + fechaActivacion);
                cadena.Append("\n");
                cadena.AppendLine("Fecha Caducidad : " + fechaExpiracion);
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("              ¡Gracias! ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");

                imprimir(cadena.ToString());
                #endregion
            }
            if (tipo == "A")
            {
                #region Impresión Acumulación de Tarjeta
                cadena.AppendLine("<footer>" + "**¡ACUMULACIÓN TARJ!**" + "</footer>");
                cadena.Append("\n");
                cadena.AppendLine(" ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("             COMPRA GRATIS  ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("Codigo Tarjeta : " + codigoTarjeta);
                cadena.Append("\n");
                cadena.AppendLine("Valor Acumulado: " + valor);
                cadena.Append("\n");
                cadena.AppendLine("Fecha Activación: " + fechaActivacion);
                cadena.Append("\n");
                cadena.AppendLine("Fecha Caducidad : " + fechaExpiracion);
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("              ¡Gracias! ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");

                imprimir(cadena.ToString());
                #endregion
            }
            if (tipo == "D")
            {
                #region Impresión Descuento Compra por Tarjeta
                cadena.AppendLine("<footer>" + "**¡DESCUENTO TARJ!**" + "</footer>");
                cadena.Append("\n");
                cadena.AppendLine(" ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("             COMPRA GRATIS  ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("Codigo Tarjeta: " + codigoTarjeta);
                cadena.Append("\n");
                cadena.AppendLine("Saldo Tarjeta : " + valor);
                cadena.Append("\n");
                cadena.AppendLine("Fecha Activación: " + fechaActivacion);
                cadena.Append("\n");
                cadena.AppendLine("Fecha Caducidad : " + fechaExpiracion);
                cadena.Append("\n");
                cadena.AppendLine("========================================");
                cadena.Append("\n");
                cadena.AppendLine("              ¡Gracias! ");
                cadena.Append("\n");
                cadena.AppendLine("========================================");

                imprimir(cadena.ToString());
                #endregion
            }
        }


        private bool verificarPromocionEspanola(string cliente)
        {
            flagPromocionPrimera = false;
            flagPromocionAcumulado = false;
            flagPromocionDescuento = false;
            flagProcesarDsct = false;

            core_TarjetaDescuento tarjeta = new core_TarjetaDescuento();
            core_TarjetaDescuento tarjetaDsct = new core_TarjetaDescuento();
            core_parametro parametro = new core_parametro();
            POSEntities db1 = new POSEntities();
            pos_customer customer = new pos_customer();

            tarjeta = db1.core_TarjetaDescuento.Where(x => x.codigoCliente == cliente).FirstOrDefault();
            customer = db1.pos_customer.Where(z => z.ACCOUNTNUM == cliente.Trim()).FirstOrDefault();

            parametro = db1.core_parametro.Where(y => y.identificador == "PROMO_ESPANOLA_FECHA_PROMO"
                                                   && y.parametro2 == this._factura.Establecimiento
                                                   && y.valor == "TRUE").FirstOrDefault();
            if (parametro != null)
            {
                #region Validación Cliente
                if (customer != null)
                {
                    if (customer.ACCOUNTNUM == "9999999999999")
                    {
                        //MessageBox.Show(this,"No aplica a la promoción", "Promoción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        flagPromocionDescuento = false;
                        flagPromocionPrimera = false;
                        flagPromocionAcumulado = false;
                        return false;
                    }
                }
                else
                {
                    flagPromocionDescuento = false;
                    flagPromocionPrimera = false;
                    flagPromocionAcumulado = false;
                    return false;
                }
                #endregion

                if (tarjeta == null)
                {
                    #region Asignación Primera vez
                    if (DateTime.Now.Date >= parametro.fecha_creacion && DateTime.Now.Date <= parametro.fecha_modificacion)
                    {
                        flagPromocionDescuento = false;
                        flagPromocionPrimera = true;
                        flagPromocionAcumulado = false;
                        usotarjetadscto = true;
                        return true;
                    }
                    else
                    {
                        //MessageBox.Show(this,"No es la fecha de promoción", "Promoción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        flagPromocionDescuento = false;
                        flagPromocionPrimera = false;
                        flagPromocionAcumulado = false;
                        return false;
                    }
                    #endregion
                }
                else
                {
                    #region Acumulación de Saldo a la tarjeta en fechas de promoción
                    if (parametro.fecha_creacion <= DateTime.Now.Date && parametro.fecha_modificacion >= DateTime.Now.Date)
                    {
                        flagPromocionDescuento = false;
                        flagPromocionPrimera = false;
                        flagPromocionAcumulado = true;
                        usotarjetadscto = true;
                        return true;
                    }
                    #endregion
                    #region Descuento de Tarjeta
                    if (tarjeta.saldo > 0 && tarjeta.activo)
                    {
                        if (tarjeta.fecha_expiracion >= DateTime.Now)
                        {
                            flagPromocionDescuento = true;
                            flagPromocionPrimera = false;
                            flagPromocionAcumulado = false;
                            return true;
                        }
                        else
                        {
                            //MessageBox.Show(this,"La tarjeta se encuentra expirada", "Promoción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            flagPromocionDescuento = false;
                            flagPromocionPrimera = false;
                            flagPromocionAcumulado = false;
                            return false;
                        }
                    }
                    else
                    {
                        //MessageBox.Show(this,"No tiene saldo en la tarjeta de promoción", "Promoción", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        flagPromocionDescuento = false;
                        flagPromocionPrimera = false;
                        flagPromocionAcumulado = false;
                        return false;
                    }
                    #endregion
                }
            }
            return true;
        }

        private void agregarDescuentoPromocionEspanola()
        {
            if (_factura != null && _factura.GetTotal() > 0)
            {
                POSEntities db = new POSEntities();
                //if (_factura.Pagos.Count() > 0)
                //{
                if (flagPromocionDescuento)
                {
                    var valor = 0M;
                    core_TarjetaDescuento tarj = new core_TarjetaDescuento();
                    core_parametro parametro = db.core_parametro.Where(x => x.identificador == "PROMO_ESPANOLA_PORCENTAJE" && x.parametro2 == _factura.Establecimiento).FirstOrDefault();
                    /*  aqui va la eliminacion de los descuentos BG*/
                    // decimal total = this._factura.getSubTotalSinDescuento();

                    tarj = db.core_TarjetaDescuento.Where(y => y.codigo == codigoTarjPromocion && y.codigoCliente == this._factura.ClienteIdentificacion).FirstOrDefault();

                    // decimal val = Math.Round((total * (decimal.Parse(parametro.valor) / 100)), 2, MidpointRounding.AwayFromZero);
                    if (tarj != null)
                    {
                        if (decimal.TryParse(parametro.valor, out valor) && valor > 0)
                        {
                            _factura.agregarPromocionEspanola(valor, tarj.saldo);
                            usotarjetadscto = true;
                            calcularFactura();
                        }

                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(232);

                        //string textoMsj = "Tarjeta no pertenece al Cliente";
                        //Control.Common.General.GetMensaje("POS - Promociones", textoMsj, "I");
                        //System.Windows.Forms.MessageBox.Show(this, "Tarjeta no pertenece al Cliente");
                    }
                }
                //}
            }
        }

        private void btnDsctoPaviPlan_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();
            //bool UsoTarjetaPavi = false;
            POSEntities db = new POSEntities();

            decimal dsctoPaviPlanProducto = 0;

            if (POS.Control.Common.Promo.PermiteUsarCuponDsctoPaviplan())
            {
                if (this._factura.Productos.Count > 0)
                {
                    Control.Pagos.DescuentoPromocion desc = new Control.Pagos.DescuentoPromocion("Descuento PAVIPLAN");

                    desc.ShowDialog();
                    codigoTarjPromocion = desc.codigo;
                    // POSEntities db = new POSEntities();

                    if (codigoTarjPromocion != "CLOSEFORM")
                    {
                        core_giftcard tarjeta = db.core_giftcard.Where(x => x.codigo == codigoTarjPromocion && x.bono == true && x.saldo == -1 && x.fecha_activacion <= DateTime.Now && x.fecha_expiracion >= DateTime.Now).FirstOrDefault();

                        if (tarjeta != null)
                        {
                            if (tarjeta.activo == true)
                            {
                                for (var i = 0; i < _factura.Productos.Count; i++)
                                {
                                    var item = _factura.Productos[i];

                                    dsctoPaviPlanProducto = Math.Round(item.SubtotalSinDescuento * 0.10M, 2);

                                    //item.DescuentoAX = (item.Subtotal * 0.10M);
                                    item.DescuentoAX = dsctoPaviPlanProducto;
                                    item.update();
                                    //var existente = getExistente(item.Id);
                                    //existente.update();

                                    item.DescuentoTarjetasPaviPlan += dsctoPaviPlanProducto;
                                }
                                calcularFactura();

                                tarjeta.activo = false;
                                tarjeta.fecha_desactivacion = DateTime.Now;
                                db.SaveChanges();
                                //UsoTarjetaPavi = true;
                            }
                            else
                            {
                                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[codigoTarjPromocion]", valor = codigoTarjPromocion });
                                parametros.Add(new ParametrosMensajes() { codigo = "[fecha_desactivacion]", valor = tarjeta.fecha_desactivacion.ToString() });
                                Control.Common.General.GetMensajeToList(233, parametros);

                                //string textoMsj = $"Tarjeta {codigoTarjPromocion} YA FUE UTILIZADA EL ";
                                //textoMsj = string.Concat(textoMsj, $" {tarjeta.fecha_desactivacion}");
                                //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                                //System.Windows.Forms.MessageBox.Show(this, "Tarjeta " + codigoTarjPromocion + " YA FUE UTILIZADA el " 
                                //    + tarjeta.fecha_desactivacion, "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }
                        }
                        else
                        {
                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[codigoTarjPromocion]", valor = codigoTarjPromocion });
                            Control.Common.General.GetMensajeToList(234, parametros);


                            //string textoMsj = $"Tarjeta {codigoTarjPromocion} NO EXISTE o EXPIRÓ ";
                            //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                            // System.Windows.Forms.MessageBox.Show(this, "Código de Tarjeta " + codigoTarjPromocion + " NO EXISTE o EXPIRÓ", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                else
                {
                    //string textoMsj = $"Para aplicar descuento de la tarjeta del cliente, la factura debe tener productos";
                    //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                    Control.Common.General.GetMensajeToList(235);

                    //System.Windows.Forms.MessageBox.Show(this, "Para aplicar descuento de la tarjeta del cliente, la factura debe tener productos");
                    return;
                }
            }
            else
            {
                //string textoMsj = $"Opción Descuento PaviPlan no se encuentra activa";
                //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                //System.Windows.Forms.MessageBox.Show(this, "Opción Descuento PaviPlan no se encuentra activa");

                Control.Common.General.GetMensajeToList(236);
            }
            agregaFormasPagoTmpFile();
        }

        private void btnDsctoEsp_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();
            //usotarjetadscto = false;
            POSEntities db = new POSEntities();

            //if (this._factura.Pagos.Any(x => x.Descripcion == "DSCT_PROMO"))
            //{
            //    MessageBox.Show(this,"Ya se Realizo el descuento con esta promoción");
            //    return;
            //}

            

            // Dscto con codigo de promocion. JM 29-11-2019
            VerificarAplicaDsctoCodigoPromocion(db);

            //Restricciones
            if (usotarjetadscto)
            {
                //No permitir uso de tarjetas durante periodo de Acumulacion (ambas variables se activan en el metodo verificarPromocionEspanola)
                if (flagPromocionAcumulado)
                {
                    
                    Control.Common.General.GetMensajeToList(237);

                    //string textoMsj = $"Recuerdele al cliente que estamos en temporada de Acumulacion y a partir de que esta finalice podra gozar de su saldo acumulado";
                    //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                    // System.Windows.Forms.MessageBox.Show(this, "Recuerdele al cliente que estamos en temporada de Acumulacion y a partir de que esta finalice podra gozar de su saldo acumulado", "Aviso");
                    return;
                }
                //No permitir uso consecutivo de tarjetas de descuento en la compra actual (la variable se activa al realizar Dcto. Promo)
                else
                {
                    Control.Common.General.GetMensajeToList(238);

                    //string textoMsj = $"Ya se empleó una tarjeta de descuento en esta compra";
                    //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                    // System.Windows.Forms.MessageBox.Show(this, "Ya se empleó una tarjeta de descuento en esta compra", "Aviso");
                    return;
                }
            }
            var parametro = db.core_parametro.Where(x => x.identificador == "PROMO_ESPANOLA" && x.valor == "TRUE" && x.parametro2 == this._factura.Establecimiento).FirstOrDefault();
            if (parametro == null)
            {
                Control.Common.General.GetMensajeToList(239);

                // System.Windows.Forms.MessageBox.Show(this, "Opcion Descuento Tarjeta CompraGratis no hablitada para este local");
                //string textoMsj = $"Opcion Descuento Tarjeta CompraGratis no hablitada para este local";
                //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                return;
            }
            if (flagPromocionDescuento)
            {
                if (this._factura.Productos.Count > 0)
                {
                    //if (MessageBox.Show(this,"¿Desea utilizar la Tarjeta de Descuento?", "Promoción", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    //{
                    Control.Pagos.DescuentoPromocion desc = new Control.Pagos.DescuentoPromocion("Descuento ESPECIAL");
                    desc.ShowDialog();
                    codigoTarjPromocion = desc.codigo;
                    // POSEntities db = new POSEntities();

                    if (codigoTarjPromocion != "CLOSEFORM")
                    {
                        core_TarjetaDescuento tarjeta = db.core_TarjetaDescuento.Where(x => x.codigo == codigoTarjPromocion).FirstOrDefault();

                        if (tarjeta != null)
                        {
                            agregarDescuentoPromocionEspanola();
                            flagProcesarDsct = true;
                        }
                        else
                        {

                            List<ParametrosMensajes>  parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[codigoTarjPromocion]", valor = codigoTarjPromocion });
                            Control.Common.General.GetMensajeToList(240, parametros);


                            //string textoMsj = $"Código de Tarjeta {codigoTarjPromocion}. NO EXISTE ";
                            //Control.Common.General.GetMensaje("POS - Tarjeta Descuento", textoMsj, "I");

                            // System.Windows.Forms.MessageBox.Show(this, "Código de Tarjeta " + codigoTarjPromocion + " NO EXISTE", "Tarjeta Descuento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }
                }
                else
                {
                    Control.Common.General.GetMensajeToList(241);

                    //string textoMsj = $"Para aplicar descuento de la tarjeta del cliente, la factura debe tener productos";
                    //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                    // System.Windows.Forms.MessageBox.Show(this, "Para aplicar descuento de la tarjeta del cliente, la factura debe tener productos");

                    return;
                }
            }
            else
            {
                if (!activoCodigoPromo)
                {
                    Control.Common.General.GetMensajeToList(242);
                    //string textoMsj = $"Cliente no posee tarjeta de descuento";
                    //Control.Common.General.GetMensaje("POS", textoMsj, "I");
                    //System.Windows.Forms.MessageBox.Show(this, "Cliente no posee tarjeta de descuento");
                }
                return;
            }
            agregaFormasPagoTmpFile();
        }

        private Boolean grabarPromocionEspanola(string msj_error)
        {

            if (msj_error == "" && usotarjetadscto == true)
            {
                POSEntities db = new POSEntities();
                core_parametro parametro = new core_parametro(); // Objeto para la validación - Tarjeta de Descuento Española
                //**** VERIFICA SI LA TIENDA ESTA HABILITADA PARA TARJETA DESCUENTO ************************************
                #region Tarjeta Descuento
                parametro = db.core_parametro.Where(x => x.identificador == "PROMO_ESPANOLA" && x.valor == "TRUE" && x.parametro2 == this._factura.Establecimiento).FirstOrDefault();
                string porcentaje = "", dias = "", numeroFactura = "";//, codigoTarjeta = "";
                decimal saldo = 0;
                POSEntities db1 = new POSEntities();

                if (parametro != null)
                {
                    //Verificar si es la primera factura del día
                    porcentaje = db.core_parametro.Where(x => x.identificador == "PROMO_ESPANOLA_PORCENTAJE" && x.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor;
                    dias = db.core_parametro.Where(x => x.identificador == "PROMO_ESPANOLA_DIAS" && x.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor;
                    numeroFactura = this._factura.GetNumeroFactura();
                    saldo = this._factura.getSubTotal();//getTotal();
                    string mensaje = "";

                    //Llamo al formulario para la asignación de la tarjeta de descuento
                    if (flagPromocionPrimera)
                        mensaje = "¿Asignar tarjeta de Promoción?";
                    else
                    {
                        if (flagPromocionDescuento)
                            mensaje = "¿Desea utilizar la Tarjeta de Descuento?";
                        else
                            mensaje = "";
                    }

                    if (flagProcesarDsct == false)
                    {
                        if (flagPromocionPrimera)
                        {
                            //if (MessageBox.Show(this,mensaje, "Promoción", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            //{
                            bool existe = false;
                            while (!existe)
                            {
                                Control.Pagos.DescuentoPromocion desc = new Control.Pagos.DescuentoPromocion("Descuento ESPECIAL");
                                desc.ShowDialog();
                                codigoTarjPromocion = desc.codigo;

                                if (codigoTarjPromocion != "CLOSEFORM")
                                {
                                    if (!procesarTarjetaDescuentoEspanola(codigoTarjPromocion, porcentaje, dias, numeroFactura, saldo, _factura.Cliente_codigo) == false)
                                    {
                                        existe = true;
                                    }
                                }
                            }
                            //}
                        }
                        else
                        {
                            if (flagPromocionDescuento)
                            {
                                if (procesarTarjetaDescuentoEspanola(codigoTarjPromocion, porcentaje, dias, numeroFactura, saldo, _factura.Cliente_codigo) == false)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (flagPromocionAcumulado)
                                {
                                    if (procesarTarjetaDescuentoEspanola(codigoTarjPromocion, porcentaje, dias, numeroFactura, saldo, _factura.Cliente_codigo) == false)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (procesarTarjetaDescuentoEspanola(codigoTarjPromocion, porcentaje, dias, numeroFactura, saldo, _factura.Cliente_codigo) == false)
                        {
                            return false;
                        }
                    }
                }
                #endregion
                // FIN TARJETA DE DESCUENTO***********************************************************************************
            }

            return true;
        }

        #endregion

        private void CreaVouchers()
        {
            if (_factura != null)
            {
                var db = new POSEntities();
                foreach (var fp in db.core_parametro.Where(x => x.identificador == "FORMA_PAGO_VOUCHER"))
                {
                    if (_factura.Pagos.Any(x => x.Descripcion == fp.valor))
                    {
                        _factura.prepararVoucher(fp.parametro2, fp.valor);
                    }
                }

            }
        }
        private void imprimir(string texto, int tipo = 1)
        {
            switch (tipo)
            {
                case 1:
                    printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                    break;
                case 2:
                    //printer.PrinterFont = new System.Drawing.Font("VERDANA", 7, FontStyle.Regular);
                    printer.PrinterFont = new System.Drawing.Font("VERDANA", 8, FontStyle.Bold);
                    break;
                default:
                    printer.PrinterFont = new System.Drawing.Font("COURIER NEW", 7, FontStyle.Bold);
                    break;
            }
            printer.TextToPrint = texto;
            printer.Print();
            
        }

        private void btnCheque_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();
            if (txtCedula.Text.Trim() == "")
            {
                Control.Common.General.GetMensajeToList(243);
                //string textoMsj = $"Debe ingresar datos de la factura";
                //Control.Common.General.GetMensaje("POS", textoMsj, "I");
                //System.Windows.Forms.MessageBox.Show(this, "Debe ingresar datos de la factura");
                return;
            }
            if (txtCedula.Text == "9999999999999")
            {
                // System.Windows.Forms.MessageBox.Show(this, "Cliente no debe ser consumidor Final");
                //string textoMsj = $"Cliente no debe ser consumidor Final";
                //Control.Common.General.GetMensaje("POS", textoMsj, "I");
                Control.Common.General.GetMensajeToList(244);

                return;
            }

            //if (revisarCambioEnTotal("CHEQUE", "CHEQUE"))
            //{
            //    return;
            //}

            string formaPago = "CHEQUE";
            if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional)
            {
                formaPago = "TAR PORTAL";
            }

            if (revisarCambioEnTotal(formaPago, formaPago))
            {
                return;
            }

            var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.Cheque, ref _factura, getValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();
            calcularFactura();
            agregaFormasPagoTmpFile();

        }
        //gasgsgag
        private void btnPagoGiftCard_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();

            //if (revisarCambioEnTotal("GIFT CARD", "GIFT CARD"))
            //{
            //    return;
            //}

            string formaPago = "GIFT CARD";
            if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional)
            {
                formaPago = "TAR PORTAL";
            }

            if (revisarCambioEnTotal(formaPago, formaPago))
            {
                return;
            }

            var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaRegalo, ref _factura, getValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();
            calcularFactura();
            for (int x = 0; x < this._factura.Pagos.Count; x++)
            {
                if (this._factura.Pagos[x].Descripcion == "GIFT CARD")
                {
                    this._factura.Pagos[x].Valor = sumardetalleGiftCard(this._factura.Pagos[x].Pagos);
                    //  this._factura.TmpGiftPagos.Add(this._factura.Pagos[x]);
                    // this._factura.Pagos.RemoveAt(x);
                }
            }
            if (f.DebeActualizarClienteFactura)
            {

                Control.Common.General.GetMensajeToList(245);
                //string textoMsj = $"Hemos detectado que está usando una(s) giftcard de GrupoCliente";
                //textoMsj = string.Concat(textoMsj, Environment.NewLine, Environment.NewLine);
                //textoMsj = string.Concat(textoMsj, $" - {f.NombreActualizarClienteFactura }" );
                //textoMsj = string.Concat(textoMsj, Environment.NewLine, Environment.NewLine);
                //textoMsj = string.Concat(textoMsj, $" por lo que la factura debe salir a nombre del cliente principal.");
                //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                //Control.Common.WinForm.ShowMessage("Hemos detectado que está usando una(s) giftcard de GrupoCliente "
                //    + Environment.NewLine + Environment.NewLine 
                //    + " -" + f.NombreActualizarClienteFactura 
                //    + Environment.NewLine + Environment.NewLine 
                //    + " por lo que la factura debe salir a nombre del cliente principal.");

                Control.Common.General.GetMensajeToList(246);

                //textoMsj = string.Empty;
                //textoMsj = string.Concat(textoMsj, $"Procediendo a cambiar la factura con el cliente principal del grupo :");
                //textoMsj = string.Concat(textoMsj, $"'{ f.IdentificacionActualizarClienteFactura}'");
                //Control.Common.General.GetMensaje("POS", textoMsj, "I");

                //Control.Common.WinForm.ShowMessage("Procediendo a cambiar la factura con el cliente principal del grupo : '" 
                //    + f.IdentificacionActualizarClienteFactura + "'");

                cambiarCliente(f.IdentificacionActualizarClienteFactura);
            }
            agregaFormasPagoTmpFile();
        }

        private decimal sumardetalleGiftCard(BindingList<PagoBase> pagos)
        {
            decimal respua = 0;
            foreach (var pgif in pagos)
            {
                respua = respua + pgif.Valor;

            }

            return respua;
        }

        private void btnCreditoInterno_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();

            ActivateDivisionEmployDiscounts(true);

            if (revisarCambioEnTotal("TAR PORTAL", "TAR PORTAL"))
            {
                return;
            }

            //decimal valorPago = decimal.Parse(_factura.GetTotal().ToString());
            //var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaInterna, ref _factura, getValorPago());
            var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.TarjetaInterna, ref _factura, getValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();

            if (!HasPayTipe("TAR PORTAL")) ActivateDivisionEmployDiscounts(false);

            calcularFactura();
            agregaFormasPagoTmpFile();
        }

        private void ActivateDivisionEmployDiscounts(bool activate, bool esEliminacionPago = false)
        {
            foreach (var item in _factura.Productos)
            {
                if (item.PorcDescuentoDivisionEmpleado > 0)
                {
                    if (activate == false || item.ActivadoPorcDctoDivisionEmpleado == false)
                    {
                        item.DescuentoAX = item.SubtotalSinDescuento * (item.PorcDescuentoDivisionEmpleado / 100) * (activate ? 1 : -1);
                        item.update();
                    }
                    item.ActivadoPorcDctoDivisionEmpleado = activate;
                }

                if (esEliminacionPago)
                {
                    item.ActivadoPorcDctoDivisionEmpleado = false;
                }
            }
            calcularFactura();
        }

        //***** FUNCION PARA AUTORIZACION DE USUARIOS PARA BORRAR LINEAS DE FACTURA *******//
        public bool ValidateAuthorizationUser(string authorize_code)
        {
            using (var db = new POSEntities())
            {
                var _almacen = db.core_establecimiento.FirstOrDefault(x => x.establecimiento == this._factura.Establecimiento).almacen;

                if (db.TblAuthorizeDeleteProducts.Any(x => x.CODE == authorize_code && x.LOCATIONID == _almacen && x.AUTHORIZE == true))
                {
                    return true;
                }
            }
            return false;
        } //***** FIN DE FUNCION *****//        


        public bool ValidateAuthorizationAuditor(string authorize_code)
        {
            using (var db = new POSEntities())
            {
                var _almacen = "AUDITOR";

                if (db.TblAuthorizeDeleteProducts.Any(x => x.CODE == authorize_code && x.LOCATIONID == _almacen && x.AUTHORIZE == true))
                {
                    return true;
                }
            }
            return false;
        } //***** FIN DE FUNCION *****//    

        private void DeletePromoProducts(string itemid)
        {


            //PROMOCIONES DE POR 4 LBS CARNE LLEVA 6 CERVEZA STELLA AL 20% DESC, PIAZZA 24 & VILLA CLUB 29
            using (var db = new POSEntities())
            {
                var parametro = db.core_parametro.Where(x => x.identificador == "PROMO_CARNES_CERVEZA" && x.valor == "TRUE").FirstOrDefault();

                if (parametro != null)
                {
                    if (parametro.fecha_creacion <= DateTime.Now.Date && parametro.fecha_modificacion >= DateTime.Now.Date)
                    {
                        if (_factura.Establecimiento == "024" || _factura.Establecimiento == "029")
                        {
                            var itemIdPromo = "PG-AB-000003";
                            var sumLibras = _factura.Productos.Where(x => POS.Control.Common.Promo.EsItemPromoCarneCerveza(x.Id) == true && x.Id != itemid).Sum(x => x.Cantidad);

                            var remainderLbs = Math.Truncate(sumLibras / 4);
                            var remainderCrvz = 0M;

                            if (_factura.Productos.Any(x => x.Id == itemIdPromo) && POS.Control.Common.Promo.EsItemPromoCarneCerveza(itemid))
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Al borrar el producto seleccionado, afectará a la promoción en el producto Six Pack de Stella");
                                Control.Common.General.GetMensajeToList(247);
                                var cervezaStella = _factura.Productos.FirstOrDefault(x => x.Id == itemIdPromo);

                                remainderCrvz = Math.Truncate(cervezaStella.Cantidad / 6M);
                                if (remainderCrvz > remainderLbs) remainderCrvz = remainderLbs;
                                cervezaStella.DescuentoAX = (remainderCrvz * 6M * cervezaStella.Pvp) * 0.2M;
                                cervezaStella.update();
                            }

                        }

                        //PROMOCION DE POR 4 LBS CARNE LLEVA 6 CERVEZA STELLA AL 20% DESC, LOS DEMAS

                        if (_factura.Establecimiento != "024" && _factura.Establecimiento != "029")
                        {
                            //SIX PACK DE PILSENER LIGHT : PG-AB-012961
                            var itemIdPromo = "PG-AB-012961";
                            var sumLibras = _factura.Productos.Where(x => POS.Control.Common.Promo.EsItemPromoCarneCerveza(x.Id) == true && x.Id != itemid).Sum(x => x.Cantidad);

                            var remainderLbs = Math.Truncate(sumLibras / 4);
                            var remainderCrvz = 0M;

                            if (_factura.Productos.Any(x => x.Id == itemIdPromo) && POS.Control.Common.Promo.EsItemPromoCarneCerveza(itemid))
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Al borrar el producto seleccionado, afectará a la promoción en el producto Six Pack de Pilsener Light");
                                Control.Common.General.GetMensajeToList(248);

                                var sixPilsenerLight = _factura.Productos.FirstOrDefault(x => x.Id == itemIdPromo);

                                remainderCrvz = Math.Truncate(sixPilsenerLight.Cantidad / 1M);
                                if (remainderCrvz > remainderLbs) remainderCrvz = remainderLbs;
                                sixPilsenerLight.DescuentoAX = (remainderCrvz * sixPilsenerLight.Pvp) * 0.2M;
                                sixPilsenerLight.update();
                            }

                        }
                    }
                }

            }

            var producto = new Producto();
            using (var db = new POSEntities())
            {
                decimal valor;
                var check = (from q in db.VW_CombosCaja
                             where q.ITEMRELATION == itemid
                             && (q.ACCOUNTRELATION == _factura.Establecimiento
                             || q.ACCOUNTRELATION == "")
                             && q.TODATE >= DateTime.Now
                             select q).FirstOrDefault();

                if (check != null)
                {
                    //DialogResult dr = MessageBox.Show(this,"Si borra el producto seleccionado, se podría anular la promoción en el producto: " + check.SUPPITEMNAME + ". ¿Desea continuar?", "Anular Promoción", MessageBoxButtons.YesNo);
                    //System.Windows.Forms.MessageBox.Show(this, "Si borra el producto seleccionado, anulará la promoción en el producto: " + check.SUPPITEMNAME);

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[SUPPITEMNAME]", valor = check.SUPPITEMNAME });
                    Control.Common.General.GetMensajeToList(249);

                    int qtySup = 0;
                    var check2 = (from q in db.VW_CombosCaja
                                  where (q.SUPPITEMCODE == check.SUPPITEMCODE)
                                  && (q.ACCOUNTRELATION == _factura.Establecimiento
                                  || q.ACCOUNTRELATION == "")
                                  && q.TODATE >= DateTime.Now
                                  orderby q.ACCOUNTRELATION descending
                                  select q).ToList();
                    foreach (VW_CombosCaja sq2 in check2)
                    {
                        foreach (var i in _factura.Productos)
                        {
                            foreach (var c in i.CodigosBarra)
                            {
                                if (c.codigo == sq2.ITEMRELATIONCODE && check.SUPPITEMCODE == sq2.SUPPITEMCODE)
                                {
                                    qtySup += int.Parse(Math.Truncate(i.Cantidad).ToString());
                                }
                            }
                        }
                    }

                    try { valor = qtySup / check.MULTIPLEQTY; }
                    catch { valor = 0; }
                    foreach (var i in _factura.Productos)
                    {
                        if (i.Id == check.SUPPITEMID)
                        {
                            //Reset descuento combo caja
                            i.DescuentoAX = -i.DescuentoPorComboCaja;
                            i.update();
                            i.DescuentoPorComboCaja = 0;

                            decimal dsctoPorCombo = 0;

                            valor = Math.Truncate(valor);
                            if (valor > 0 && i.Cantidad >= valor)
                                dsctoPorCombo = ((valor * i.Pvp) * check.SUPPITEMQTY);
                            else if (valor > 0)
                                dsctoPorCombo = ((Math.Truncate(i.Cantidad) * i.Pvp) * check.SUPPITEMQTY);
                            else
                                dsctoPorCombo = 0.00M;

                            i.DescuentoAX = Math.Round(dsctoPorCombo, 2, MidpointRounding.AwayFromZero);
                            i.DescuentoPorComboCaja += i.DescuentoAX;
                            i.update();
                            break;
                        }
                    }
                }

            }

        }

        private void DeletePromoProductoCantidadCupon(List<Descuento> Cupones)
        {
            if (Cupones!=null)
            {
                if (Cupones.Count > 0)
                {
                    if (_factura.EsUsoCuponPromocional)
                    {
                        foreach (var item in Cupones)
                        {
                            if (item.codigo == _factura.CuponPromocionalCodigo)
                            {
                                if (codigoCuponPromocional.codigo== item.codigo)
                                {
                                    codigoCuponPromocional = new core_TarjetaDescuento();
                                    _factura.EsUsoCuponPromocional = false;
                                }

                                _factura.CuponPromocionalPorcDesc = 0M;
                                _factura.CuponPromocionalCodigo = "";
                            }
                        }
                    }
                }
            }

        }
        private void btnBorrarProducto_Click(object sender, EventArgs e)
        {
            using (var db = new POSEntities())
            {
                if (gridItems.SelectedRows.Count > 0)
                {
                    //  if (MessageBox.Show(this,"Esta seguro de borrar producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    var item = gridItems.SelectedRows[0].DataBoundItem as POS.Models.Producto;

                    if (db.core_parametro.Where(x => x.identificador == "DELETE_FACT_PRO" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
                    {

                        if (Es2X_CONSULTA_POS || _factura.User.isSuperUser)//Si es modo Consulta 2X
                        {
                            _factura.Pagos.Clear();
                            if (!_factura.EsEmpleadoLiris)
                            {
                                _factura.Descuentos2.Clear();
                            }

                            gridItems.SelectedRows[0].Delete();

                            quitar_de_estructura(item);

                            promobonella(_factura, item);

                            promovino(_factura, item);

                            if (flagDescuentoBarra)//Tiene cupon de descuento Caducidad
                            {
                                try
                                {
                                    SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                                    using (conexion2)
                                    {
                                        conexion2.Open();
                                        String Query1 = "Update tblliquidacion set INACTIVO = 0, FacturaPos = '' where FacturaPos = '" + _factura.GetNumeroFactura() + "' and ItemId ='" + item.Id + "'";
                                        SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                        comandoupd.ExecuteNonQuery();
                                        conexion2.Close();
                                    }
                                }
                                catch (Exception error)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnBorrarProducto_Click", "No se pudo desenlazar los cupones de liquidacion al borrar de la factura '" + _factura.GetNumeroFactura() + "' el producto con ItemId '" + item.Id + "', verificar en tblliquidacion de la base Ax, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(error));
                                    //System.Windows.Forms.MessageBox.Show(this, "Ocurrio un error al actualizar barra de Descuento. Detalle del error: " + error.ToString());
                                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = error.ToString() });
                                    Control.Common.General.GetMensajeToList(250, parametros);

                                }
                            }

                            promojohnson(_factura, item);

                            promohotdog(_factura, item);

                            promochifle(_factura, item);
                            //promoiva(_factura, item);
                            agregaProductosTmpFile();  //agregaProductosTmp(item.Id);
                            DeletePromoProducts(item.Id);
                            DeletePromoProductoCantidadCupon(item.DescuentosCupon);

                            PromosPrecioPorCombinacion();

                            PromosDsctoPorSuplemento();

                            promoiva(_factura, null);
                            // Recalcular los descuentos.  22-02-2019 JM
                            //item.recalcularDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);
                            calcularFactura();

                            RefrescarGridItems();

                            CalcularParqueo();

                            AnulaDsctoCompraGratis();

                            return;
                        }

                        if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
                        {
                            Verifier = new VerificationForm(Data, _factura);
                            Verifier.Tag = POS.Control.Common.GlobalParameters.DeleteProductUsrAdm;
                            //Verifier.Tag = "usr";
                            if (_factura.User.isSuperUser)
                            {
                                verificador = DialogResult.OK;
                            }
                            else
                            {
                                verificador = Verifier.ShowDialog();
                            }

                            if (verificador == DialogResult.OK)
                            {
                                //DeletePromoProducts(item.Id, item.Cantidad,item.Pvp);
                                string auth_user = Verifier.Tag.ToString();
                                try
                                {
                                    TblDeletedProduct delete = new TblDeletedProduct();
                                    delete.INVOICEDATE = DateTime.Now;
                                    delete.ESTABLISHMENT = _factura.Establecimiento;
                                    delete.EMISIONSERIES = _factura.PtoEmision;
                                    delete.NUMBER = _factura.Secuencia;
                                    delete.CASHIER = _current_user.username;
                                    delete.ITEMID = item.Id;
                                    delete.QTY = item.Cantidad;
                                    delete.TOTAL = item.Total;
                                    delete.USERAUTHORIZE = auth_user;
                                    db.TblDeletedProducts.Add(delete);
                                    db.SaveChanges();
                                }
                                catch (Exception error)
                                {
                                    var errorMsg = error.ToString();
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnBorrarProducto_Click", "No se pudo registrar por EF la accion de borrar en la tabla TblDeletedProducts al realizar la acción en la factura '" + _factura.GetNumeroFactura() + "' sobre el producto con ItemId '" + item.Id + "', solicitado por '" + auth_user + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(error));
                                    if (!errorMsg.Contains("dbo.sp_send_dbmail"))
                                    {
                                        //System.Windows.Forms.MessageBox.Show(this, "Ocurrio un error al borrar linea. Detalle del error: " + errorMsg);

                                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = errorMsg.ToString() });
                                        Control.Common.General.GetMensajeToList(251, parametros);

                                    }
                                }

                                if (_factura.Descuentos2.Any())
                                {
                                    _factura.Descuentos2.Clear();
                                    _factura.Pagos.Clear();
                                }

                                gridItems.SelectedRows[0].Delete();
                                quitar_de_estructura(item);
                                promobonella(_factura, item);
                                promovino(_factura, item);

                                if (flagDescuentoBarra)//Tiene cupon de descuento Caducidad
                                {
                                    try
                                    {
                                        SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                                        using (conexion2)
                                        {
                                            conexion2.Open();
                                            String Query1 = "Update tblliquidacion set INACTIVO = 0, FacturaPos = '' where FacturaPos = '" + _factura.GetNumeroFactura() + "' and ItemId ='" + item.Id + "'";
                                            SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                            comandoupd.ExecuteNonQuery();
                                            conexion2.Close();
                                        }
                                    }
                                    catch (Exception error)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnBorrarProducto_Click", "No se pudo desenlazar los cupones de liquidacion al borrar de la factura '" + _factura.GetNumeroFactura() + "' el producto con ItemId '" + item.Id + "', verificar en tblliquidacion de la base Ax, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(error));
                                        //System.Windows.Forms.MessageBox.Show(this, "Ocurrio un error al actualizar barra de Descuento. Detalle del error: " + error.ToString());

                                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                        parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = error.ToString() });
                                        Control.Common.General.GetMensajeToList(253, parametros);


                                    }
                                }

                                promojohnson(_factura, item);
                                promohotdog(_factura, item);
                                promochifle(_factura, item);
                                //promoiva(_factura, item);
                                agregaProductosTmpFile();  //agregaProductosTmp(item.Id);
                                DeletePromoProducts(item.Id);
                                AnulaDsctoCompraGratis();
                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                Control.Common.General.GetMensajeToList(252);
                            }
                        }
                        else
                        {

                            // CODIGO CON FORMA DE CLAVE
                            try
                            {
                                DialogResult _authorize;
                                _inputFormAuthUser.setValue(String.Empty);
                                _inputFormAuthUser._txtInput.PasswordChar = '•';
                                _authorize = _inputFormAuthUser.ShowDialog();
                                focused = (System.Windows.Forms.Control)_inputFormAuthUser._txtInput;
                                //scanner.ControlToShowText = _inputFormAuthUser._txtInput;
                            }
                            catch (Exception ex)
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Por favor, intente nuevamente", "Advertencia Elimina Línea", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                Control.Common.General.GetMensajeToList(254);
                            }

                            if (focused.Text.ToString() != "")
                            {
                                if (ValidateAuthorizationUser(focused.Text.ToString()) == true)
                                {
                                    DeletePromoProducts(item.Id);


                                    string auth_user = db.TblAuthorizeDeleteProducts.Where(x => x.CODE == focused.Text.ToString()).First().USERID;

                                    try
                                    {
                                        TblDeletedProduct delete = new TblDeletedProduct();
                                        delete.INVOICEDATE = DateTime.Now;
                                        delete.ESTABLISHMENT = _factura.Establecimiento;
                                        delete.EMISIONSERIES = _factura.PtoEmision;
                                        delete.NUMBER = _factura.Secuencia;
                                        delete.CASHIER = _current_user.username;
                                        delete.ITEMID = item.Id;
                                        delete.QTY = item.Cantidad;
                                        delete.TOTAL = item.Total;
                                        delete.USERAUTHORIZE = auth_user;
                                        db.TblDeletedProducts.Add(delete);
                                        db.SaveChanges();
                                    }
                                    catch (Exception error)
                                    {
                                        var errorMsg = error.ToString();
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnBorrarProducto_Click", "No se pudo registrar por EF la accion de borrar en la tabla TblDeletedProducts al realizar la acción en la factura '" + _factura.GetNumeroFactura() + "' sobre el producto con ItemId '" + item.Id + "', solicitado por '" + auth_user + "', a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(error));
                                        if (!errorMsg.Contains("dbo.sp_send_dbmail"))
                                        {
                                            //System.Windows.Forms.MessageBox.Show(this, "Ocurrio un error al borrar linea. Detalle del error: " + errorMsg);
                                            Control.Common.General.GetMensajeToList(255);

                                        }
                                    }

                                    if (_factura.Descuentos2.Any())
                                    {
                                        _factura.Descuentos2.Clear();
                                        _factura.Pagos.Clear();
                                    }

                                    gridItems.SelectedRows[0].Delete();
                                    quitar_de_estructura(item);
                                    AnulaDsctoCompraGratis();
                                }
                                else
                                {
                                    //System.Windows.Forms.MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                    Control.Common.General.GetMensajeToList(256);

                                }

                            }
                            else
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Tiene que ingresar un código de autorización", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                                Control.Common.General.GetMensajeToList(257);


                            }
                        }
                    }
                    else
                    {
                        DeletePromoProducts(item.Id);


                        try
                        {
                            TblDeletedProduct delete = new TblDeletedProduct();
                            delete.INVOICEDATE = DateTime.Now;
                            delete.ESTABLISHMENT = _factura.Establecimiento;
                            delete.EMISIONSERIES = _factura.PtoEmision;
                            delete.NUMBER = _factura.Secuencia;
                            delete.CASHIER = _current_user.username;
                            delete.ITEMID = item.Id;
                            delete.QTY = item.Cantidad;
                            delete.TOTAL = item.Total;
                            delete.USERAUTHORIZE = null;
                            db.TblDeletedProducts.Add(delete);
                            db.SaveChanges();
                        }
                        catch (Exception error)
                        {
                            var errorMsg = error.ToString();
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnBorrarProducto_Click", "No se pudo registrar por EF la accion de borrar en la tabla TblDeletedProducts al realizar la acción en la factura '" + _factura.GetNumeroFactura() + "' sobre el producto con ItemId '" + item.Id + "' (borrar productos en el local no solicita autorizacion porque no cuenta con registro DELETE_FACT_PRO en core_parametro), a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(error));

                            if (!errorMsg.Contains("dbo.sp_send_dbmail"))
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Ocurrio un error al borrar linea. Detalle del error: " + errorMsg);

                                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[exception_error]", valor = errorMsg });
                                Control.Common.General.GetMensajeToList(251, parametros);

                            }
                        }

                        if (_factura.Descuentos2.Any())
                        {
                            _factura.Descuentos2.Clear();
                            _factura.Pagos.Clear();
                        }

                        gridItems.SelectedRows[0].Delete();
                        quitar_de_estructura(item);
                        AnulaDsctoCompraGratis();
                    }
                    // Recalcular los descuentos.  22-02-2019 JM

                    //item.recalcularDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);

                    /******************************************************************
                     * aqui promo budweiser bi=ucket corona.
                     * ********************************************************************/
                    promobucket(true);
                    /******************************************************************
                     *          hasta aqui promo budweiser bucket corona.**.
                     *******************************************************************/


                    PromosPrecioPorCombinacion();

                    PromosDsctoPorSuplemento();

                    promoiva(_factura, null);

                    calcularFactura();

                    RefrescarGridItems();
                }
                else
                {
                    Control.Common.General.GetMensajeToList(258);
                }
            }

            CalcularParqueo(); 
        }

        private void RefrescarGridItems()
        {
            gridItems.MasterTemplate.Refresh();
        }

        private void ScrollLastGridItems()
        {
            try
            {
                if (gridItems.Rows.Count > 0) gridItems.TableElement.ScrollToRow(gridItems.Rows.Last());
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ScrollLastGridItems", Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void btnRecarga_Click(object sender, EventArgs e)
        {

            if (esGiftCard)
            {
                POS.Control.POS.init(ref _factura);
                setTituloDocumento();
                _factura.Productos.Clear();
                calcularFactura();
                esRecarga = true;
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (scanner != null && scanner.IsOpen)
                {
                    scanner.Close();
                }

                if (scannerDL != null)
                {
                    scannerDL.DeviceEnabled = false;
                    scannerDL.ReleaseDevice();
                    scannerDL.Close();
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MainWindow_FormClosed", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }


        private void btnRetencion_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();
            if (_factura.ClienteIdentificacion == "9999999999999")
            {
                Control.Common.General.GetMensajeToList(259);
                //System.Windows.Forms.MessageBox.Show(this, "La Retencion no aplica a Consumidor Final", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_factura != null && _factura.GetTotal() > 0)
            {
                //if (revisarCambioEnTotal("RETCLIENTE", "RETCLIENTE"))
                //{
                //    return;
                //}

                string formaPago = "RETCLIENTE";
                if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional)
                {
                    formaPago = "TAR PORTAL";
                }

                if (revisarCambioEnTotal(formaPago, formaPago))
                {
                    return;
                }

                if (!_factura.Pagos.Any(x => x.Descripcion == "RETCLIENTE"))
                {
                    var f = new POS.Control.Pagos.RetencionesPagos(_factura); //var f = new POS.Control.Pagos.CalculoPago(_factura);
                    f.StartPosition = FormStartPosition.CenterScreen;
                    f.ShowDialog();
                }
                else
                {
                    Control.Common.General.GetMensajeToList(260);
                    //System.Windows.Forms.MessageBox.Show(this, "La Retencion ya se efectuo", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                txtPagoValor.Clear();
                calcularFactura();
                agregaFormasPagoTmpFile();
            }

        }

        #region Auxiliary class
        private class InputBoxDialog : Form
        {
            public string Value { get { return _txtInput.Text; } }
            public static bool SuppressEscape { get; set; }
            public bool activo;

            private Label _lblPrompt;
            public TextBox _txtInput;
            private Button _btnOk;
            private Button _btnCancel;

            #region Constructor
            public InputBoxDialog(string prompt, string title, string defaultValue = null, int? xPos = null, int? yPos = null)
            {
                if (xPos == null && yPos == null)
                {
                    StartPosition = FormStartPosition.CenterParent;
                }
                else
                {
                    StartPosition = FormStartPosition.Manual;

                    if (xPos == null) xPos = (Screen.PrimaryScreen.WorkingArea.Width - Width) >> 1;
                    if (yPos == null) yPos = (Screen.PrimaryScreen.WorkingArea.Height - Height) >> 1;

                    Location = new Point(xPos.Value, yPos.Value);
                }

                InitializeComponent();

                if (title == null) title = Application.ProductName;
                Text = title;
                this.TopMost = true;
                _lblPrompt.Text = prompt;
                Graphics graphics = CreateGraphics();
                _lblPrompt.Size = graphics.MeasureString(prompt, _lblPrompt.Font).ToSize();
                int promptWidth = _lblPrompt.Size.Width;
                int promptHeight = _lblPrompt.Size.Height;

                _txtInput.Location = new Point(8, 30 + promptHeight);
                int inputWidth = promptWidth < 206 ? 206 : promptWidth;
                _txtInput.Size = new Size(100, 21);
                _txtInput.Text = defaultValue;
                //_txtInput.PasswordChar = '•';               
                _txtInput.SelectAll();
                _txtInput.Focus();

                Height = 125 + promptHeight;
                Width = inputWidth + 23;

                _btnOk.Location = new Point(8, 60 + promptHeight);
                _btnOk.Size = new Size(100, 26);

                _btnCancel.Location = new Point(114, 60 + promptHeight);
                _btnCancel.Size = new Size(100, 26);

                return;
            }
            #endregion

            #region Methods
            protected void InitializeComponent()
            {
                InputBoxDialog.SuppressEscape = false;
                _lblPrompt = new Label();
                _lblPrompt.Location = new Point(12, 9);
                _lblPrompt.TabIndex = 0;
                _lblPrompt.BackColor = Color.Transparent;

                _txtInput = new TextBox();
                _txtInput.Size = new Size(156, 20);
                _txtInput.TabIndex = 1;

                _btnOk = new Button();
                _btnOk.TabIndex = 2;
                _btnOk.Size = new Size(75, 26);
                _btnOk.Text = "&OK";
                _btnOk.DialogResult = DialogResult.OK;

                _btnCancel = new Button();
                _btnCancel.TabIndex = 3;
                _btnCancel.Size = new Size(75, 26);
                _btnCancel.Text = "&Cancel";
                _btnCancel.DialogResult = DialogResult.Cancel;

                activo = false;
                AcceptButton = _btnOk;
                CancelButton = _btnCancel;

                Controls.Add(_lblPrompt);
                Controls.Add(_txtInput);
                Controls.Add(_btnOk);
                Controls.Add(_btnCancel);

                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                TopMost = true;

                return;
            }
            private void _txtInput_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar == (char)Keys.Escape)//.Enter)
                {

                }
            }
            public void setValue(string txt)
            {
                this._txtInput.Text = txt;
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                InputBoxDialog.SuppressEscape = (e.KeyCode == Keys.Escape);
                base.OnKeyUp(e);
            }
            #endregion

        }
        #endregion

        private void btnCreditoPavos_Click(object sender, EventArgs e)
        {
            var db = new POSEntities();

            if (POS.Control.Common.Promo.EstaActivaPromoPaviPlan())
            {
                ejecutaVentanaPaviplan();
            }
            else
            {
                Control.Common.General.GetMensajeToList(261);

                //System.Windows.Forms.MessageBox.Show(this, "La opción seleccionada ha sido desactivada.", "PaviPLAN", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ejecutaVentanaPaviplan(string id = "")
        {
            var pavos = new POS.Control.Pagos.CreditoPavos();
            pavos.Establecimiento = this._factura.Establecimiento;
            pavos.PtoEmision = this._factura.PtoEmision;
            pavos.Cajero = this._current_user.username;
            pavos.Left = 0;
            pavos.Top = 0;
            if (id != "")
            {
                pavos.setCedula(id);
            }

            pavos.ShowDialog();
        }

        private void btnQtyProduct_Click(object sender, EventArgs e)
        {
            SelectQtyProduct();

        }

        public void SelectQtyProduct()
        {
     
            try
            {
                if (_factura.Productos.Count() > 0)
                {
                    //_enterQty = _inputform.ShowDialog();
                    //focused = (System.Windows.Forms.Control)_inputform._txtInput;
                    QtyForm qty = new QtyForm();
                    qty.ShowDialog();
                    //var item = _factura.Productos.Last();
                    var item = gridItems.SelectedRows[0].DataBoundItem as POS.Models.Producto;

                    //if (focused.Text.ToString() != "")
                    if (qty.txtQty.Text != "")
                    {
                        if (item.Unidad.ToUpper() == "UND")
                        {
                            var producto = new Producto();
                            //item.Cantidad = int.Parse(focused.Text.ToString()) - 1;
                            //item.Unidades = int.Parse(focused.Text.ToString()) - 1;
                            if (qty.q > item.Cantidad)
                            {
                                item.Cantidad = qty.q - 1;
                                item.CantidadINEC = item.Cantidad;
                                item.Unidades = qty.q - 1;

                                getProducto(item.Id);
                            }
                            else
                            {
                                Control.Common.General.GetMensajeToList(262);
                                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Tiene que elegir cantidades mayores a la actual.", "POS - Cantidad de Artículos");
                                //System.Windows.Forms.MessageBox.Show(this, "Tiene que elegir cantidades mayores a la actual.", "Cantidad de Artículos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            Control.Common.General.GetMensajeToList(263);
                            //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " puede usar esta función porque el producto no esta en unidades", "POS - Cantidad de Artículos");
                            //System.Windows.Forms.MessageBox.Show(this, "No puede usar esta función porque el producto no esta en unidades", "Cantidad de Artículos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    Control.Common.General.GetMensajeToList(264);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "  existen Artículos en listas", "POS - Artículos", 30);
                    //MessageBoxTemporal.Show("No existen Artículos en lista", "Artículos", 30, true);
                    //MessageBox.Show(this, "No existen Artículos en lista", "Artículos", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                return;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "SelectQtyProduct", "Ocurrió una novedad durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //System.Windows.Forms.MessageBox.Show(this, "Por favor intente nuevamente", "Cantidad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Por favor intente nuevamente", "POS - Artículos", 30);
                //Control.Common.General.GetMensaje("POS - Artículos", $"Por favor intente nuevamente", "I", 30);
                Control.Common.General.GetMensajeToList(265);


            }
        }

        private void btnCFinal_Click(object sender, EventArgs e)
        {
            FinalClient();
            //ValidarMonedero(txtCedula.Text);
            ValidarMonederoCampania(txtCedula.Text);
            LimpiarClienteCompraGratis();
        }

        private void btnSearchPro_Click(object sender, EventArgs e)
        {
            //SearchProFunction();
            SearchProFunction3();
        }




        public bool ValidaConectividadPathImagenBusqProd()
        {
            //string url = "http://reportes.liris.com.ec/ImgAx/100/default.jpg";           
            ResponseBackground = string.Empty;
            //DebeCerrarFormBackground = false;
            On_Off_Controles(false);
            //PagoPinpadBackground = null;

            return RemoteFileExists(URLPATHIMGBUSQPROD);
        }

        private bool RemoteFileExists(string url)
        {
            try
            {
                //Creating the HttpWebRequest
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                //Setting the Request method HEAD, you can also use GET too.
                request.Method = "HEAD";
                //Getting the Web Response.
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }

        private void SearchProFunction()
        {

            if (txtCedula.Text.Length > 0 && txtCedula.Text.Length <= 13)
            {
                if (this.ValidaConectividadPathImagenBusqProd())
                {


                    SearchProductV2 sp = new SearchProductV2("", _factura.Establecimiento);
                    sp.Top = 80;
                    sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                    sp.ShowDialog();
                    txtCodigo.Text = sp.code;
                    getProducto(txtCodigo.Text);
                    txtCodigo.Clear();
                    txtCodigo.Focus();
                }
                else
                {
                    SearchProduct sp = new SearchProduct("", _factura.Establecimiento);
                    sp.Top = 80;
                    sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                    sp.ShowDialog();
                    txtCodigo.Text = sp.code;
                    getProducto(txtCodigo.Text);
                    txtCodigo.Clear();
                    txtCodigo.Focus();
                }
                // agregaProductosTmpFile(); eevv 20200408
            }
            else
            {
                // MessageBox.Show(this, "No existe cliente activo", "Búsqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                //Control.Common.WinForm.ShowMessage("No existe cliente activo");
                //MessageBoxTemporal.Show("No existe cliente activo.", "POS", 20, true);
                Control.Common.General.GetMensajeToList(579);
            }
        }

        private void SearchProFunction3()
        {
            try
            {
                if (txtCedula.Text.Length > 0 && txtCedula.Text.Length <= 13)
                {
                    //por background valida disponibilidad de la URL de las imagenes.
                    ProcesamientoBuscarItemBackGround();

                    //if (this.ValidaConectividadPathImagenBusqProd())
                    if (this.TieneConectividadPathImagenBusqProd)
                    {
                        SearchProductV3 sp = new SearchProductV3("", _factura.Establecimiento);
                        sp.Top = 20;
                        sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                        sp.TopMost = true;
                        sp.ShowDialog();
                        txtCodigo.Text = sp.code;
                        getProducto(txtCodigo.Text);
                        txtCodigo.Clear();
                        txtCodigo.Focus();
                    }
                    else
                    {
                        SearchProduct sp = new SearchProduct("", _factura.Establecimiento);
                        sp.Top = 80;
                        sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                        sp.ShowDialog();
                        txtCodigo.Text = sp.code;
                        getProducto(txtCodigo.Text);
                        txtCodigo.Clear();
                        txtCodigo.Focus();
                    }
                    // agregaProductosTmpFile(); eevv 20200408
                }
                else
                {
                    // MessageBox.Show(this, "No existe cliente activo", "Búsqueda de Artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    //Control.Common.WinForm.ShowMessage("No existe cliente activo");
                    //MessageBoxTemporal.Show("No existe cliente activo.", "POS", 20, true);
                    Control.Common.General.GetMensajeToList(266);

                }
            }
            catch (Exception ex)
            {

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "SearchProFunction3", "Ocurrió una excepción en la busqueda del item, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Si es modo Consulta 2X o la accion de cierre viene desde herramientas administrativas
            if (Es2X_CONSULTA_POS || POS.Control.Common.GlobalParameters.MustCloseApplication)
            {
                //Apagar la bandera luego de usarse
                POS.Control.Common.GlobalParameters.MustCloseApplication = false;

                //Permitir al proceso de cierre de programa continuar
                return;
            }

            //if (_factura.Productos.Count() > 0)
            //{
            var pos = new POSEntities();

            if (pos.core_parametro.Where(x => x.identificador == "CLOSE_POS" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
            {
                if (pos.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
                {
                    Verifier = new VerificationForm(Data, _factura);
                    Verifier.Tag = "adm";
                    verificador = Verifier.ShowDialog();
                    if (verificador == DialogResult.OK)
                    {
                        EliminaFacturaTmpFile();    //eliminaFacturatmp();
                        return;
                    }
                    else
                    {
                        Control.Common.General.GetMensajeToList(9001);

                        //System.Windows.Forms.MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        e.Cancel = true;
                    }
                }
                else
                {

                    try
                    {
                        DialogResult _authorize;
                        _inputFormAuthUser.setValue(String.Empty);
                        _inputFormAuthUser._txtInput.PasswordChar = '•';
                        _authorize = _inputFormAuthUser.ShowDialog();
                        focused = (System.Windows.Forms.Control)_inputFormAuthUser._txtInput;
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MainWindow_FormClosing", "Ocurrió una novedad en el cierre del formulario durante la validación manual de usuario empleando InputBoxDialog, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                        //System.Windows.Forms.MessageBox.Show(this, "Por favor, intente nuevamente", "Advertencia Cerrar POS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Control.Common.General.GetMensajeToList(267);

                    }

                    if (focused.Text.ToString() != "")
                    {
                        if (ValidateAuthorizationUser(focused.Text.ToString()) == true)
                        {
                            EliminaFacturaTmpFile();    //eliminaFacturatmp();
                            return;
                        }
                        else
                        {
                            
                            Control.Common.General.GetMensajeToList(9005);
                            //System.Windows.Forms.MessageBox.Show(this, "Código no válido ó Usuario no autorizado. Intente nuevamente", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(this, "Tiene que ingresar un código de autorización", "Código de Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Control.Common.General.GetMensajeToList(9004);
                        e.Cancel = true;
                    }
                }
            }
            //}

            POS.Control.Common.GlobalParameters.MustCloseApplication = false;
        }

        private void btnDescuentoEspecial_Click(object sender, EventArgs e)
        {
            Control.Pagos.DescuentoPromocion desc = new Control.Pagos.DescuentoPromocion("Descuento ESPECIAL");
            desc.ShowDialog();
            string codigo = desc.codigo;

            if (codigo != "CLOSEFORM")
            {
                VerifyDiscount(codigo);
            }
        }
        private void VerifyDiscount(string codigo)
        {
            try
            {
                if (_factura.Productos.Count() > 0)
                {
                    var db = new POSEntities();
                    bool flagDiscount = false;
                    string permite_almacen = db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor;

                    if (permite_almacen == "TRUE")
                    {
                        if (codigo != "" && codigo != null)
                        {
                            bool permitedsctocliente = db.TblDiscountCards.Any(x => x.card == codigo && x.itemid == this._factura.ClienteIdentificacion);
                            decimal pordesc = 0;
                            if (permitedsctocliente)
                            {

                                Decimal pordesc_max = Decimal.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_PORC_MAX" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);
                                Decimal pordesc_min = Decimal.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_PORC_MIN" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);
                                Decimal ventaminima = Decimal.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_VENTA_MINIMA" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);
                                Decimal ventamensual = Decimal.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_VENTA_MENSUAL" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);
                                Double periodoinicial = Double.Parse(db.core_parametro.Where(k => k.identificador == "LOCAL_TAR_DESC_PERIODO_INICIAL" && k.parametro2 == this._factura.Establecimiento).FirstOrDefault().valor);


                                DateTime Dia = (DateTime)db.TblDiscountCards.Where(x => x.card == codigo).FirstOrDefault().emision_date;
                                double numerodias = (DateTime.Now - Dia).TotalDays;
                                if (numerodias < periodoinicial)
                                {
                                    pordesc = pordesc_max;
                                }
                                else
                                {
                                    if (this._factura.GetTotal() >= ventaminima)
                                    {

                                        DateTime fechatemp = DateTime.Today;

                                        DateTime fecha1;
                                        DateTime fecha2;
                                        if (fechatemp.Month == 1)
                                        {
                                            fecha1 = new DateTime(fechatemp.Year - 1, 12, 1);
                                            fecha2 = new DateTime(fechatemp.Year, 1, 1);
                                        }
                                        else
                                        {
                                            fecha1 = new DateTime(fechatemp.Year, fechatemp.Month - 1, 1);
                                            fecha2 = new DateTime(fechatemp.Year, fechatemp.Month, 1);
                                        }

                                        Decimal ValorVendidoMensual = 0;
                                        try
                                        {
                                            ValorVendidoMensual = db.core_factura.Where(x => x.cliente == this._factura.ClienteIdentificacion
                                            && x.fecha_creacion >= fecha1 && x.fecha_creacion < fecha2).Sum(x => x.total);
                                            //MessageBox.Show(this,ValorVendidoMensual.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerifiyDiscount", "Ocurrió una novedad durante la consulta a la base del Valor Vendido Mensual, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                        }

                                        if (ValorVendidoMensual >= ventamensual)
                                        { pordesc = pordesc_max; }
                                        else
                                        { pordesc = pordesc_min; }
                                    }
                                }

                                foreach (var item in _factura.Productos)
                                {
                                    item.DescuentoAX = (pordesc / 100) * item.SubtotalSinDescuento;
                                    var existente = getExistente(item.Id);
                                    existente.update();
                                    calcularFactura();
                                    flagDiscount = true;
                                }
                                if (pordesc != 0)
                                {
                                    TblDiscountCardTran tran = new TblDiscountCardTran();
                                    tran.trans_date = DateTime.Now;
                                    tran.card = codigo;
                                    tran.invoiceid = this._factura.Establecimiento + this._factura.PtoEmision + this._factura.Secuencia;
                                    db.TblDiscountCardTrans.Add(tran);

                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerifyDiscount", "(Cuando SI permite dscto cliente) Ocurrio una novedad durante la persistencia de datos a traves de Entity Framework, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                        Console.WriteLine(ex);
                                    }

                                    Control.Common.General.GetMensajeToList(268);

                                    //System.Windows.Forms.MessageBox.Show(this, "Descuentos asignados correctamente.", "Descuento Especial", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                }
                            }
                            else
                            {
                                int how_times = int.Parse(db.core_parametro.Where(z => z.identificador == "TIMES_TAR_DESC").FirstOrDefault().valor);

                                var _list = (from t in db.TblDiscountCards
                                             where t.status == true
                                             && t.expiration_date >= DateTime.Now
                                             && t.times < how_times
                                             && t.card == codigo
                                             select t).ToList();

                                if (_list.Count > 0)
                                {
                                    foreach (var item in _factura.Productos)
                                    {
                                        for (int i = 0; i < _list.Count; i++)
                                        {
                                            TblDiscountCard discount = _list[i];

                                            if (item.Id == discount.itemid)
                                            {
                                                item.DescuentoAX = (decimal.Parse(discount.value.ToString()) / 100) * item.SubtotalSinDescuento;
                                                var existente = getExistente(item.Id);
                                                existente.update();
                                                calcularFactura();
                                                flagDiscount = true;
                                            }
                                        }
                                    }

                                    if (flagDiscount == true)
                                    {
                                        foreach (var q in _list.Where(w => w.card == codigo))
                                        {
                                            q.times = q.times + 1;
                                        }

                                        TblDiscountCardTran tran = new TblDiscountCardTran();
                                        tran.trans_date = DateTime.Now;
                                        tran.card = codigo;
                                        tran.invoiceid = this._factura.Establecimiento + this._factura.PtoEmision + this._factura.Secuencia;
                                        db.TblDiscountCardTrans.Add(tran);

                                        try
                                        {
                                            db.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerifyDiscount", "(Cuando NO permite dscto cliente) Ocurrio una novedad durante la persistencia de datos a traves de Entity Framework, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                            Console.WriteLine(ex);
                                        }
                                        Control.Common.General.GetMensajeToList(268);
                                        //System.Windows.Forms.MessageBox.Show(this, "Descuentos asignados correctamente.", "Descuento Especial", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    }
                                    else
                                    {
                                        Control.Common.General.GetMensajeToList(269);
                                        //System.Windows.Forms.MessageBox.Show(this, "Tarjeta Expirada o Desactivada. Consulte a administración.", "Información de Tarjeta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                    }
                                }
                                else
                                {
                                    Control.Common.General.GetMensajeToList(270);
                                    //System.Windows.Forms.MessageBox.Show(this, "No se obtuvo información de la tarjeta. Por favor deslice la tarjeta por el lector magnético.", "Información de Tarjeta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(this, "Tarjeta no válida para este local", "Tarjeta de Descuento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Control.Common.General.GetMensajeToList(271);
                    }
                }
                else
                {
                    Control.Common.General.GetMensajeToList(272);
                    //MessageBoxTemporal.Show("No existen Artículos en lista", "Artículos", 30, true);
                    //MessageBox.Show(this, "No existen Artículos en lista", "Artículos", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }

                return;
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerifyDiscount", "Ocurrio una novedad general durante la ejecución del método, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //System.Windows.Forms.MessageBox.Show(this, "Por favor intente nuevamente", "Cantidad", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Control.Common.General.GetMensajeToList(273);

            }

        }
        private void btnNC_Click(object sender, EventArgs e)
        {
            QuitarDescuentoPromocionTarjetaBines();
            if (_factura.ClienteIdentificacion == "9999999999999")
            {
                //System.Windows.Forms.MessageBox.Show(this, "La Nota de Crédito no aplica a Consumidor Final", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Control.Common.General.GetMensajeToList(274);
                return;
            }

            for (int x = 0; x < this._factura.Pagos.Count; x++)
            {
                if (this._factura.Pagos[x].Descripcion == "ANTCLIEN/C")
                {
                    this._factura.Pagos.RemoveAt(x);
                }
            }

            string formaPago = "ANTCLIEN/C";
            if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional)
            {
                formaPago = "TAR PORTAL";
            }

            if (revisarCambioEnTotal(formaPago, formaPago))
            {
                return;
            }

            //if (revisarCambioEnTotal("ANTCLIEN/C", "ANTCLIEN/C"))
            //{
            //    return;
            //}

            var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.NotaCredito, ref _factura, getValorPago());
            f.ShowDialog();
            txtPagoValor.Clear();
            calcularFactura();
            agregaFormasPagoTmpFile();
        }
        private void ValidaRecargaAdicional()
        {
            if (_factura == null) return;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();


            if (_factura.Productos.Count == 0)
            {
                tempo.Stop();
                SqlConnection conexion = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                string Query = null;
                SqlCommand comando = default(SqlCommand);
                using (conexion)
                {
                    try
                    {
                        conexion.Open();
                        Query = "select top 1 observacion, valor_base,recid from TBL_MONTO_APERTURA_HISTORIAL where cedula='" + _current_user.username + "' and cast(fecha as date)= cast( '" + DateTime.Now.Date.AddDays(0).ToString("yyyy-MM-dd") + "' as date) and leido=0  order by RECID desc";

                        comando = new SqlCommand(Query, conexion);
                        SqlDataReader dr = comando.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();



                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[usuario]", valor = _current_user.nombres });
                            parametros.Add(new ParametrosMensajes() { codigo = "[cantidad]", valor = string.Format("{0:C2}", dr.GetValue(1)) });
                            parametros.Add(new ParametrosMensajes() { codigo = "[MENSAJE_CAJA]", valor = POS.Properties.Settings.Default.MENSAJE_CAJA });

                            var result = Control.Common.General.GetMensajeToList(275, parametros);

                            if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                            {
                                try
                                {
                                    using (conexion2)
                                    {
                                        conexion2.Open();
                                        String Query1 = "UPDATE TBL_MONTO_APERTURA_HISTORIAL  SET leido= 1 WHERE recid=" + dr.GetValue(2).ToString();
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidaRecargaAdicional", "Usuario acepta una recarga adicional. Se ejecuta query de Aceptacion de recarga adicional: '" + Query1 + "'");

                                        SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                        comandoupd.ExecuteNonQuery();
                                        conexion2.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidaRecargaAdicional", "Ocurrio una novedad durante la solicitud de update a la tabla TBL_MONTO_APERTURA_HISTORIAL, para confirmar la recepcion de recarga al cajero. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                    //System.Windows.Forms.MessageBox.Show(this, ex.ToString(), "Mensaje");

                                    parametros = new List<ParametrosMensajes>();
                                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.ToString() });
                                    Control.Common.General.GetMensajeToList(273, parametros);



                                }
                            }
                            else {
                                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                                Application.Exit();
                            }
                          
                        }

                        dr.Close();
                        conexion.Close();
                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidaRecargaAdicional", "Ocurrio una novedad durante la consulta de tabla TBL_MONTO_APERTURA_HISTORIAL, para verificar si hay nueva recarga al cajero. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                        //MessageBox.Show(this, ex.ToString(), "Mensaje");
                    }


                }

                tempo.Start();
            }
        }
        private Decimal ObtenAvance(out Int64 registro)
        {
            SqlConnection conexion = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
            Decimal valor = 0;
            Int64 registroTmp = 0;
            string Query = null;
            SqlCommand comando = default(SqlCommand);
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();


            using (conexion)
            {
                conexion.Open();
                try
                {

                    Query = "select top 1 avance,recid from TBL_MONTOAPERTURA where cedula='" + _current_user.username + "' and cast(fecha as date)= cast( '" + DateTime.Now.Date.AddDays(0).ToString("yyyy-MM-dd") + "' as date)   order by RECID desc";
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ObtenAvance", "Solicitando Id del registro de monto apertura: " + Query);

                    comando = new SqlCommand(Query, conexion);
                    SqlDataReader dr = comando.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        valor = (decimal)dr.GetValue(0);
                        registroTmp = (Int64)dr.GetValue(1);
                    }
                    else
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ObtenAvance", "SqlDataReader no trajo registros a pesar de no existir excepciones durante la comunicacion a la base de datos, valor será retornado con 0");
                    }

                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ObtenAvance", "No fue posible obtener el registro de la tbl_montoapertura, valor será retornado con -1, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                    //System.Windows.Forms.MessageBox.Show(this, ex.ToString(), "Mensaje");

                    parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.ToString() });
                    Control.Common.General.GetMensajeToList(273, parametros);



                    conexion.Close();
                    valor = -1;
                    registroTmp = -1;

                }
                conexion.Close();
                registro = registroTmp;
                return (valor);
            }


        }
        private void AvanceCaja()
        {

            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            //Si Wallpaper de seguridad esta abierto no mostrar pantalla de avance

            if (MsgBoxCtrl.IsFormOpen(typeof(POS.Control.Security.Wallpaper)))
            {
                return;
            }

            //if (POS.Control.Common.WinForm.IsFormOpen(typeof(POS.Control.Security.Wallpaper)))
            //{
            //    return;
            //}

            tempo5min.Stop();

            Int64 registro;
            if (_factura.Productos.Count == 0)
            {

                using (var db = new POSEntities())
                {
                    decimal monto_avance = 499.00M;

                    if (db.core_parametro.Any(x => x.identificador == "MONTO_AVANCE" && x.parametro2 == establecimiento_inicio))
                    {
                        monto_avance = decimal.Parse(db.core_parametro.First(x => x.identificador == "MONTO_AVANCE" && x.parametro2 == establecimiento_inicio).valor);

                    }

                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"<plantillaPago>(.*)\</plantillaPago>");
                    StringBuilder pagos = new StringBuilder();


                    decimal PagoTotalValor = 0;
                    decimal PagoTotalCheque = 0;
                    try
                    {
                        var viewUs = db.ViewFacturaPagoUsuarios.Where(x => x.usuario == _current_user.username && x.establecimiento == _factura.Establecimiento && x.fecha.Value.Year == DateTime.Now.Year && x.fecha.Value.Month == DateTime.Now.Month && x.fecha.Value.Day == DateTime.Now.Day && (x.PAYMMODE == "EFECTIVO"));
                        foreach (var pago in viewUs)
                        {
                            PagoTotalValor += (decimal)pago.valor;
                        }

                    }
                    catch (Exception ex)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "AvanceCaja", "Ocurrio una novedad durante la consulta a la vista ViewFacturaPagoUsuarios. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    }

                    decimal valor = ObtenAvance(out registro);
                     if ((PagoTotalValor - valor) > monto_avance && valor != -1)
                    {

                        try
                        {
                            var viewUs = db.ViewFacturaPagoUsuarios.Where(x => x.usuario == _current_user.username && x.establecimiento == _factura.Establecimiento && x.fecha.Value.Year == DateTime.Now.Year && x.fecha.Value.Month == DateTime.Now.Month && x.fecha.Value.Day == DateTime.Now.Day && (x.PAYMMODE == "CHEQUE"));
                            foreach (var pago in viewUs)
                            {
                                PagoTotalCheque += (decimal)pago.valor;
                            }

                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "AvanceCaja", "Ocurrio una novedad durante la consulta a la vista viewUs. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                        }

                        parametros = new List<ParametrosMensajes>();
                        parametros.Add(new ParametrosMensajes() { codigo = "[usuario]", valor = _current_user.nombres });
                        parametros.Add(new ParametrosMensajes() { codigo = "[monto_avance]", valor = string.Format("{0:C2}", (monto_avance + 1)) });
                        parametros.Add(new ParametrosMensajes() { codigo = "[PagoTotalCheque]", valor = string.Format("{0:C2}", (PagoTotalCheque)) });
                        var result = Control.Common.General.GetMensajeToList(279, parametros);


                        //if (MessageBox.Show(this,"Usuario: " + _current_user.nombres + "\n\nTiene en caja la cantidad de : " + string.Format("{0:C2}", (PagoTotalValor - valor)) + " en EFECTIVO y/o CHEQUE.\n\nDesea realizar el avance de caja?", "ADVERTENCIA!!!!!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        //if (System.Windows.Forms.MessageBox.Show(this, "Usuario: " + _current_user.nombres + "\n\nTiene que realizar el avance de: " + string.Format("{0:C2}", (monto_avance + 1)) + " en EFECTIVO y " + string.Format("{0:C2}", (PagoTotalCheque)) + "  CHEQUE.\n\nDesea realizar el avance de caja?", "ADVERTENCIA!!!!!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        if(result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                        {
                            // MessageBox.Show(this,"Imprimir ticket y actualizar tabla");
                            if (db.core_recibo.Any(x => x.identificador == "AVANCE_CAJA"))
                            {
                                bool realizoUpdateAvance = false;
                                try
                                {
                                    SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                                    using (conexion2)
                                    {
                                        conexion2.Open();
                                        String Query1 = "UPDATE TBL_MONTOAPERTURA  SET avance= avance+" + (monto_avance + 1 + PagoTotalCheque) + ", contavance = contavance + 1 WHERE recid=" + registro;
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "AvanceCaja", "Usuario indica que desea realizar el avance de caja. Solicitando actualizar el avance en su registro de monto apertura: " + Query1);

                                        SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                        comandoupd.ExecuteNonQuery();
                                        conexion2.Close();
                                    }

                                    realizoUpdateAvance = true;

                                    String texto = db.core_recibo.First(x => x.identificador == "AVANCE_CAJA").cuerpo;

                                    texto = texto.Replace("<<oficina>>", _factura.Establecimiento_nombre);
                                    //    texto = texto.Replace("<<factura>>", _factura.getNumeroFactura());
                                    texto = texto.Replace("<<cajero>>", _factura.User.nombres);
                                    texto = texto.Replace("<<factura_fecha>>", DateTime.Now.ToString());

                                    System.Text.RegularExpressions.Regex regexfact = new System.Text.RegularExpressions.Regex(@"<plantillaFact>(.*)\</plantillaFact>");
                                    StringBuilder fact = new StringBuilder();

                                    fact.AppendLine("");
                                    fact.AppendLine("Punto Emision:" + _factura.PtoEmision);
                                    fact.AppendLine(string.Format("Valor del Anticipo Efectivo: $ {0}", (monto_avance + 1)));
                                    fact.AppendLine(string.Format("Valor del Anticipo Cheque: $ {0}", (PagoTotalCheque)));
                                    fact.AppendLine("--------------------------------------------");
                                    fact.AppendLine(string.Format("Valor Total del Anticipo: $ {0}", (monto_avance + 1) + PagoTotalCheque));

                                    fact.AppendLine("");
                                    texto = regexfact.Replace(texto, fact.ToString());
                                    this.imprimir(texto);
                                    this.imprimir(texto);

                                }
                                catch (Exception ex)
                                {
                                    if (realizoUpdateAvance)
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "AvanceCaja", "No pudo realizarse la impresion del avance de caja, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                        //System.Windows.Forms.MessageBox.Show(this, "Estimado usuario, su avance de caja si se registró pero no se pudo imprimir su comprobante, comuníquelo al administrador, solicite que sea comprobado en el sistema y realice el vaciado de su caja normalmente", "Mensaje");
                                        Control.Common.General.GetMensajeToList(280);

                                    }
                                    else
                                    {
                                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "AvanceCaja", "Imposible realizar la actualizacion del avance de caja en la base, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                                        System.Windows.Forms.MessageBox.Show(this, "Estimado usuario, en este momento no fue posible registrar su avance de caja, esto puede deberse a un problema temporal, por favor espere y dentro de unos instantes volveremos a intentarlo", "Mensaje");
                                        Control.Common.General.GetMensajeToList(281);

                                    }
                                }
                            }
                            else
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "AvanceCaja", "No pudo encontrarse en la tabla core_recibo un registro con identificador 'AVANCE_CAJA' designado para avances de caja, por favor verificar y proveer uno");
                                //System.Windows.Forms.MessageBox.Show(this, "Estimado usuario, no vamos a poder registrar por el momento su avance de caja porque faltan parámetros en el sistema. Comuníquelo al administrador y solicite que el incidente sea revisado", "Mensaje");
                                Control.Common.General.GetMensajeToList(282);

                            }

                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "AvanceCaja", "Usuario prefiere no realizar aún su avance de caja");
                        }

                    }
                }


            }
            tempo5min.Start();
        }
        private void tempo_Tick(object sender, EventArgs e)

        {
            tempo.Stop();
            ValidaRecargaAdicional();
            tempo.Start();
        }
        private void tempo5min_Tick(object sender, EventArgs e)
        {
            tempo.Stop();
            tempo5min.Stop();
           AvanceCaja();
            //evelasco  - aqui adicionar el cambio de carga del pos_voucher.
            ProcesaPosVoucherTmp();
            ProcesaPosConsumoBilleteraTmp();//Dinero Electrónico 
            ProcesaPosConsumoGiftCardTmp();//Gift Card 
            ProcesaPosConsumoCuponAppTmp();//Cupon App
            tempo.Start();
            tempo5min.Start();
        }

        private void ProcesaPosVoucherTmp()
        {
            string rutaCompleta = string.Empty;
            string scriptInsert = string.Empty;
            int numLineaInsert = 0;
            string Linea2Insert = string.Empty;
            string[] split;
            string fechaConsumo = string.Empty;
            string valorconsumo = string.Empty;
            string Autorizacion = string.Empty;
            string NumeroVoucher = string.Empty;
            string FacturaVoucher = string.Empty;
            bool Archivoprocesado = false;
            try
            {

                rutaCompleta = Control.Common.GlobalParameters.VoucherInsertPath;
                DirectoryInfo di = new DirectoryInfo(rutaCompleta);
                string[] files = Directory.GetFiles(rutaCompleta);
                // Console.WriteLine("Search pattern test?.txt returns:");
                foreach (var fi in di.GetFiles("F?.txt"))
                {

                }

                foreach (var item in files)
                {
                    //Console.WriteLine(fi.Name);
                    //MessageBox.Show(fi.);
                    //string contenido = File.ReadAllText(fi.FullName);
                    string contenido = File.ReadAllText(item);
                    scriptInsert = string.Empty;
                    numLineaInsert = 0;
                    Linea2Insert = string.Empty;
                    fechaConsumo = string.Empty;
                    valorconsumo = string.Empty;
                    Autorizacion = string.Empty;
                    NumeroVoucher = string.Empty;
                    FacturaVoucher = string.Empty;
                    Archivoprocesado = false;
                    using (StreamReader file = new StreamReader(item))
                    {
                        string ln;
                        // cargar cabecera  
                        while ((ln = file.ReadLine()) != null)
                        {
                            numLineaInsert++;
                            scriptInsert += ln.Replace("SCRIPT PARA INSERTAR:", string.Empty);
                            if (numLineaInsert == 3)
                            {
                                Linea2Insert = ln;
                                split = Linea2Insert.Split(',');

                                fechaConsumo = split[2].Replace("'", string.Empty);
                                valorconsumo = split[6].Replace("'", string.Empty);
                                Autorizacion = split[5].Replace("'", string.Empty);
                                NumeroVoucher = split[4].Replace("'", string.Empty);
                                FacturaVoucher = split[34].Replace("'", string.Empty);

                                if (!string.IsNullOrEmpty(fechaConsumo) && !string.IsNullOrEmpty(valorconsumo) && !string.IsNullOrEmpty(Autorizacion) && !string.IsNullOrEmpty(NumeroVoucher) && !string.IsNullOrEmpty(FacturaVoucher))
                                {
                                    if (!ExisteVoucher(fechaConsumo, valorconsumo, Autorizacion, NumeroVoucher, FacturaVoucher))
                                    {
                                        Archivoprocesado = InsertaVoucher(scriptInsert);
                                    }
                                    else
                                    {
                                        Archivoprocesado = true;
                                    }
                                }
                            }

                            // usotarjetadscto = false;
                        }
                        file.Close();

                    }
                    if (Archivoprocesado)
                    {
                        MoverArchivosProcesadorVoucher(rutaCompleta, item);
                    }

                    //MessageBox.Show(scriptInsert);
                    //fi.MoveTo(rutaCompleta + "/procesados");
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ProcesaPosVoucherTmp", "Ha ocurrido una excepción al leer los archivos Temporales de los Vouchers. A continuación se detalla: " + ex.Message);

            }
        }

        private void ProcesaPosConsumoBilleteraTmp()
        {
            string rutaCompleta = string.Empty;
            bool Archivoprocesado = false;
            try
            {

                rutaCompleta = Control.Common.GlobalParameters.ConsumoBilleteraInsertPath;
                DirectoryInfo di = new DirectoryInfo(rutaCompleta);
                if(!di.Exists)
                { return ; }
                

                    string[] files = Directory.GetFiles(rutaCompleta);
             
                foreach (var item in files)
                {
                   
                    string contenido = File.ReadAllText(item);

                  
                    Archivoprocesado = false;
                    using (StreamReader file = new StreamReader(item))
                    {
                        string ln;
                        while ((ln = file.ReadLine()) != null)
                        {
                            Archivoprocesado = InsertaConsumo(ln);
                        }
                        file.Close();
                    }
                    if (Archivoprocesado)
                    {
                        MoverArchivosProcesador(rutaCompleta, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ProcesaPosConsumoBilleteraTmp", "Ha ocurrido una excepción al leer los archivos Temporales de los Consumo Billetera. A continuación se detalla: " + ex.Message);
            }
        }

        private void ProcesaPosConsumoGiftCardTmp()
        {
            string rutaCompleta = string.Empty;
            bool Archivoprocesado = false;
            try
            {

                rutaCompleta = Control.Common.GlobalParameters.ConsumoGiftCardInsertPath;
                DirectoryInfo di = new DirectoryInfo(rutaCompleta);
                if (!di.Exists)
                { return; }


                string[] files = Directory.GetFiles(rutaCompleta);

                foreach (var item in files)
                {

                    string contenido = File.ReadAllText(item);


                    Archivoprocesado = false;
                    using (StreamReader file = new StreamReader(item))
                    {
                        string ln;
                        while ((ln = file.ReadLine()) != null)
                        {
                            Archivoprocesado = InsertaConsumo(ln);
                        }
                        file.Close();
                    }
                    if (Archivoprocesado)
                    {
                        MoverArchivosProcesador(rutaCompleta, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ProcesaPosConsumoBilleteraTmp", "Ha ocurrido una excepción al leer los archivos Temporales de los Consumo Billetera. A continuación se detalla: " + ex.Message);
            }
        }

        private void ProcesaPosConsumoCuponAppTmp()
        {
            string rutaCompleta = string.Empty;
            bool Archivoprocesado = false;
            try
            {

                rutaCompleta = Control.Common.GlobalParameters.ConsumoCuponAppInsertPath;
                DirectoryInfo di = new DirectoryInfo(rutaCompleta);
                if (!di.Exists)
                { return; }


                string[] files = Directory.GetFiles(rutaCompleta);

                foreach (var item in files)
                {

                    string contenido = File.ReadAllText(item);


                    Archivoprocesado = false;
                    using (StreamReader file = new StreamReader(item))
                    {
                        string ln;
                        while ((ln = file.ReadLine()) != null)
                        {
                            Archivoprocesado = InsertaConsumo(ln);
                        }
                        file.Close();
                    }
                    if (Archivoprocesado)
                    {
                        MoverArchivosProcesador(rutaCompleta, item);
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ProcesaPosConsumoBilleteraTmp", "Ha ocurrido una excepción al leer los archivos Temporales de los Consumo Billetera. A continuación se detalla: " + ex.Message);
            }
        }

        private bool ExisteVoucher(string fechaConsumo, string valorconsumo, string Autorizacion, string NumeroVoucher, string FacturaVoucher)
        {
            bool existe = false;
            try
            {
                using (POSEntities pos = new POSEntities())
                {
                    existe = pos.POS_VOUCHER.Any(x => x.FECHACONSUMO == fechaConsumo && x.VALORCONSUMO == valorconsumo && x.AUTORIZACION == Autorizacion && x.NUMEROVOUCHER == NumeroVoucher && x.FACTURA == FacturaVoucher);
                }
            }
            catch (Exception)
            {


            }
            return existe;
        }

        private bool InsertaVoucher(string Query1)
        {
            bool insertado = false;
            try
            {

                using (var ctx = new POSEntities())
                {
                    int noOfRowInserted = ctx.Database.ExecuteSqlCommand(Query1);
                    if (noOfRowInserted > 0)
                        insertado = true;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return insertado;
        }

        private bool InsertaConsumo(string Query1)
        {
            bool insertado = false;

            string cadenaCon = "";
            if (Control.Common.GlobalParameters.ConServerPuntos != "")
            {
                cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
            }
            else
            {
                return false;
            }

            if (cadenaCon != "")
            {
                SqlConnection conn = new SqlConnection(cadenaCon);
                SqlCommand select = new SqlCommand(Query1, conn);
                try
                {
                    conn.Open();
                    var result = select.ExecuteNonQuery();
                    conn.Close();
                    insertado = true;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    throw;
                }
            }


            return insertado;
        }

        private void MoverArchivosProcesadorVoucher(string targetPath, string fileName)
        {
            string sourceFile = targetPath + fileName;
            string copyto = string.Empty;
            string destFile = targetPath + "/procesados/" + fileName;
            copyto = targetPath + "procesados";
            string destinoNombre = string.Empty;
            string[] split1;
            try
            {
                // To copy a folder's contents to a new location:
                // Create a new target folder. 
                // If the directory already exists, this method does not create a new directory.
                System.IO.Directory.CreateDirectory(copyto);

                split1 = fileName.Split('\\');
                int i = 0;
                i = split1.Count();
                if (i > 0)
                {
                    destinoNombre = split1[i - 1];
                    copyto = copyto + "\\" + destinoNombre;
                }


                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                //System.IO.File.Copy(sourceFile, destFile, true);
                System.IO.File.Move(fileName, copyto);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MoverArchivosProcesadorVoucher", "Ha ocurrido una excepción al mover los archivos Temporales de los Vouchers. A continuación se detalla: " + ex.Message);
            }
        }

        private void MoverArchivosProcesador(string targetPath, string fileName)
        {
            string sourceFile = targetPath + fileName;
            string copyto = string.Empty;
            string destFile = targetPath + "/procesados/" + fileName;
            copyto = targetPath + "procesados";
            string destinoNombre = string.Empty;
            string[] split1;
            try
            {
                // To copy a folder's contents to a new location:
                // Create a new target folder. 
                // If the directory already exists, this method does not create a new directory.
                System.IO.Directory.CreateDirectory(copyto);

                split1 = fileName.Split('\\');
                int i = 0;
                i = split1.Count();
                if (i > 0)
                {
                    destinoNombre = split1[i - 1];
                    copyto = copyto + "\\" + destinoNombre;
                }


                // To copy a file to another location and 
                // overwrite the destination file if it already exists.
                //System.IO.File.Copy(sourceFile, destFile, true);
                System.IO.File.Move(fileName, copyto);
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "MoverArchivosProcesador", "Ha ocurrido una excepción al mover los archivos Temporales de"+ targetPath +" . A continuación se detalla: " + ex.Message);
            }
        }


        private AppData Data;                   // keeps application-wide data
        private VerificationForm Verifier;
        private DialogResult verificador;
        private Control.Pagos.FormaNotaCredito FrmNC;
        private Control.PINPAD.PagoPINPAD FrmPP;
        private Control.ReImprimirFactura.ReImprimirFactura FrmReImpFac; //JCanarte 11Ene2021

        private void btnLocal_Click(object sender, EventArgs e)
        {
            SeleccionaLocal f = new SeleccionaLocal();
            f.ShowDialog();
        }

        private void btnFrmNC_Click(object sender, EventArgs e)
        {
            Verifier = new VerificationForm(Data, _factura);
            Verifier.Tag = "adm";
            if (_factura.User.isSuperUser)
            {
                verificador = DialogResult.OK;
            }
            else
            {
                verificador = Verifier.ShowDialog();
            }

            if (verificador == DialogResult.OK)
            {
                FrmNC = new Control.Pagos.FormaNotaCredito(_factura);
                FrmNC.Top = 0;
                FrmNC.Left = 0;
                FrmNC.ShowDialog();
            }

        }
        private void promobucket(bool esborrar)
        {
            return;
            if (_factura.Establecimiento == "024" || _factura.Establecimiento == "011" || _factura.Establecimiento == "012")
            {
                var db = new POSEntities();
                if (db.core_parametro.Where(x => x.identificador == "PROMO_BUCKET" && x.valor == "TRUE").FirstOrDefault() != null)
                {
                    decimal cantbotella = 0;
                    bool estado = false;
                    foreach (var i in _factura.Productos)
                    {
                        //   MessageBox.Show(this,i._id);


                        switch (i.Id)
                        {
                            case "PG-AB-014056":
                                //CERVEZA CORONA EXTRA 355 ML.SIXPACK
                                cantbotella += (i.Cantidad * 6);
                                break;
                            case "PG-AB-005482":
                                //CERVEZA CORONA EXTRA 355ML.
                                cantbotella += i.Cantidad;
                                break;
                            case "PG-AB-014319":
                                estado = true;
                                break;
                        }
                    }

                    if (cantbotella >= 12 && estado != true)
                    {
                        System.Windows.Forms.MessageBox.Show(this, "EL CLIENTE LLEVA  " + Math.Truncate(cantbotella / 12).ToString() + "  Buckets Corona GRATIS");

                    }
                    foreach (var i in _factura.Productos)
                    {
                        if (/*cantbotella >= 12 &&*/ i.Id == "PG-AB-014319")
                        {
                            var canttmp = Math.Truncate(cantbotella == 0 ? 0 : cantbotella / 12);
                            if ((i.Cantidad < canttmp && esborrar != true) || esborrar == true)
                            {
                                i.Cantidad = Math.Truncate(cantbotella == 0 ? 0 : cantbotella / 12);
                                i.CantidadINEC = i.Cantidad;
                                i.Unidades = (int)Math.Truncate(cantbotella == 0 ? 0 : cantbotella / 12);
                            }
                            i.DescuentoAX = (Math.Truncate(cantbotella == 0 ? 0 : cantbotella / 12) * i.Pvp);
                            i.update();

                        }
                        else
                        {
                            //     MessageBox.Show(this,"Borrar Buckets");
                        }
                    }

                }
            }
        }
        private void promopavo(Factura _factura)
        {
            return;
            var db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PROMO_PAVO_2x1" && x.valor == "TRUE").FirstOrDefault() != null)
            {
                var cantpavos = 0;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PT-PA-000008":
                        case "PT-PA-000009":
                        case "PT-PA-000010":
                        case "PT-PA-000011":
                        case "PT-PA-000012":
                        case "PT-PA-000013":
                        case "PT-PA-000014":
                        case "PT-PA-000015":
                        case "PT-PA-000016":
                            cantpavos += item.Unidades;
                            break;

                    }
                }
                int cantpavosdesc = (cantpavos / 2) * 2;
                //for (var bg = 1; bg <= cantpavosdesc; bg++)
                //{
                //    MessageBox.Show(this,"Pavo " + bg.ToString());
                //    /*
                //    var descuento = item.getDescuento(_factura, c);
                //    item.calcularDescuento(descuento);
                //    item.update();
                //    */
                //}
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    var auxunidades = 0;

                    switch (item.Id)
                    {
                        case "PT-PA-000008":
                        case "PT-PA-000009":
                        case "PT-PA-000010":
                        case "PT-PA-000011":
                        case "PT-PA-000012":
                        case "PT-PA-000013":
                        case "PT-PA-000014":
                        case "PT-PA-000015":
                        case "PT-PA-000016":
                            //item.DescuentoAX = (decimal.Parse(discount.value.ToString()) / 100) * item.SubtotalSinDescuento;
                            if ((item.Unidades > cantpavosdesc) && (item.Unidades % 2) == 0)
                            {

                                auxunidades = item.Unidades;
                            }
                            else
                            {
                                auxunidades = cantpavosdesc;
                            }
                            cantpavosdesc = cantpavosdesc - auxunidades;
                            item.DescuentoAX = (((item.Cantidad / item.Unidades) * auxunidades) * item.Pvp) / 2;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            cantpavos += item.Unidades;
                            break;

                    }
                }

            }

        }
        private void promolonchera(Factura _factura, Producto productotmp)
        {
            return;
            var codprod = false;
            var db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PROMO_LONCHERA" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                var sumapromo = 0M;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PG-AB-005052":
                        case "PG-AB-005056":
                        case "PG-AB-005243":
                        case "PG-AB-013737":
                        case "PG-AB-001309":
                        case "PG-AB-010940":
                        case "PG-AB-010951":
                        case "PG-AB-014313":
                        case "PG-AB-004981":
                        case "PG-AB-005005":
                        case "PG-AB-005772":
                        case "PG-AB-013036":
                        case "PG-AB-014722":
                        case "PG-AB-014723":
                        case "PG-AB-014699":
                        case "PG-AB-014725":
                        case "PG-AB-014726":
                        case "PG-AB-014698":
                        case "PG-AB-014724":
                        case "PG-AB-014727":
                        case "PG-AB-001297":
                        case "PG-AB-014697":
                        case "PG-AB-001301":
                        case "PG-AB-001299":
                        case "PG-AB-001303":
                        case "PG-AB-001300":
                        case "PG-AB-001304":
                        case "PG-AB-001302":
                        case "PG-AB-001306":
                        case "PG-AB-005860":
                        case "PG-AB-001305":
                        case "PG-AB-001307":
                        case "PG-AB-005914":
                        case "PG-AB-010944":
                        case "PG-AB-005904":
                        case "PG-AB-005861":
                        case "PG-AB-014860":
                        case "PG-AB-006073":
                        case "PG-AB-014861":
                        case "PG-AB-010945":
                        case "PG-AB-010947":
                        case "PG-AB-010943":
                        case "PG-AB-014594":
                        case "PG-AB-014593":
                        case "PG-AB-010950":
                            sumapromo += item.Total;
                            if (!codprod && item.Id == productotmp.Id)
                            {
                                codprod = true;
                            }

                            if (sumapromo >= 10)
                                tienepromo = true;

                            break;

                    }
                }
                var cantidad = Math.Truncate(sumapromo / 10);
                if (tienepromo && codprod)
                {
                    System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar hasta " + cantidad.ToString() + " LONCHERA DE OREO");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];


                    switch (item.Id)
                    {
                        case "PG-AB-015558":
                            if (tienepromo)
                            {

                                //   item.DescuentoAX = item.Pvp;
                                if (item.Cantidad > cantidad)
                                {
                                    System.Windows.Forms.MessageBox.Show(this, "Solo puede facturarse hasta " + cantidad.ToString() + " unidad(es)");

                                    item.Cantidad = cantidad;
                                    item.CantidadINEC = item.Cantidad;

                                    // item.Unidades = cantidad;

                                }
                            }
                            else
                            {
                                item.Cantidad = 0;
                                item.CantidadINEC = item.Cantidad;
                                item.Unidades = 0;
                                System.Windows.Forms.MessageBox.Show(this, "Se va a eliminar el LONCHERA promocional x no cumplir los requisitos");
                            }
                            //item.update();
                            item.calcularDescuento(100);
                            //item.Descuento = item.Pvp * item.Cantidad;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            if (!tienepromo)
                            {
                                object sender = new object();
                                EventArgs e = new EventArgs();
                                btnBorrarProducto_Click(sender, e);
                            }
                            break;

                    }
                }

            }

        }
        private void promobonella(Factura _factura, Producto productotmp)
        {
            return;
            var codprod = false;
            var db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PROMO_BONELLA" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                var sumapromo = 0M;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PG-AB-002679":
                        case "PG-AB-004682":
                        case "PG-AB-011110":
                        case "PG-AB-011111":
                        case "PG-AB-011112":
                            sumapromo += item.Total;
                            if (!codprod && item.Id == productotmp.Id)
                            {
                                codprod = true;
                            }

                            if (sumapromo >= 3)
                                tienepromo = true;

                            break;

                    }
                }
                var cantidad = Math.Truncate(sumapromo / 3);
                if (tienepromo && codprod)
                {
                    System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar hasta " + cantidad.ToString() + " OLLA PROMOCION BONELLA A 5USD CADA UNA");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];


                    switch (item.Id)
                    {
                        case "PG-AB-016022":
                            //if (tienepromo)
                            //{
                            //    //                                var porcentaje = cantidad < item.Cantidad ? Math.Round((cantidad / item.Cantidad) * 100, 2, MidpointRounding.AwayFromZero) : 100;
                            //    var porcentaje = cantidad < item.Cantidad ? cantidad : item.Cantidad;
                            //    item.DescuentoAX = ((porcentaje) * (13.39M));

                            //    //item.calcularDescuento(porcentaje);

                            //}
                            //else
                            //{
                            //    item.DescuentoAX = 0M;
                            //    item.calcularDescuento(0);
                            //}
                            //item.update();
                            // item.calcularDescuento(100);
                            //item.Descuento = item.Pvp * item.Cantidad;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            break;

                    }
                }

            }

        }

        private void promovino(Factura _factura, Producto productotmp)
        {
            return;
            var codprod = false;
            var db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PROMO_VINO" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                var sumapromo = 0M;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PT-CR-000128":
                            sumapromo += item.Unidades;
                            if (!codprod && item.Id == productotmp.Id)
                            {
                                codprod = true;
                            }

                            if (sumapromo >= 1)
                                tienepromo = true;

                            break;

                    }
                }
                var cantidad = 1;// Math.Truncate(sumapromo);
                if (tienepromo && codprod)
                {
                    System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar hasta " + cantidad.ToString() + " VINO TINTO LA CHAMIZA POLO AMATEUR MALBEC 750 ML.A 8.50 USD C/U");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];


                    switch (item.Id)
                    {
                        case "PG-AB-015625":
                            if (tienepromo)
                            {
                                //                                var porcentaje = cantidad < item.Cantidad ? Math.Round((cantidad / item.Cantidad) * 100, 2, MidpointRounding.AwayFromZero) : 100;
                                var porcentaje = cantidad < item.Cantidad ? cantidad : item.Cantidad;
                                item.DescuentoAX = ((porcentaje) * 3.12M);// (item.Pvp *0.291M));

                                //item.calcularDescuento(porcentaje);

                            }
                            else
                            {
                                item.DescuentoAX = 0M;
                                item.calcularDescuento(0);
                            }
                            //item.update();
                            // item.calcularDescuento(100);
                            //item.Descuento = item.Pvp * item.Cantidad;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            break;

                    }
                }

            }

        }
        private void promojohnson(Factura _factura, Producto productotmp)
        {
            return;
            var codprod = false;
            var db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PROMO_JOHNSON" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                var sumapromo = 0M;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PG-AB-003696":
                        case "PG-AB-004715":
                        case "PG-AB-004961":
                        case "PG-AB-005801":
                        case "PG-AB-012454":
                        case "PG-AB-012455":
                        case "PG-AB-012456":
                        case "PG-AB-012457":
                        case "PG-AB-012458":
                        case "PG-AB-012461":
                        case "PG-AB-012462":
                        case "PG-AB-012463":
                        case "PG-AB-012464":
                        case "PG-AB-012465":
                        case "PG-AB-013061":
                        case "PG-AB-013766":
                        case "PG-AB-013768":
                        case "PG-AB-013769":
                        case "PG-AB-013770":
                        case "PG-AB-013772":
                        case "PG-AB-013774":
                        case "PG-AB-013775":
                        case "PG-AB-013776":
                        case "PG-AB-013777":
                        case "PG-AB-013778":
                        case "PG-AB-013787":
                        case "PG-AB-013788":
                        case "PG-AB-013789":
                        case "PG-AB-013790":
                        case "PG-AB-013791":
                        case "PG-AB-013792":
                        case "PG-AB-014735":
                        case "PG-AB-014736":
                        case "PG-AB-015214":
                        case "PG-AB-015429":
                        case "PG-AB-015430":
                        case "PG-AB-015431":
                        case "PG-AB-015432":
                        case "PG-AB-015433":
                        case "PG-AB-015434":
                        case "PG-AB-015435":
                        case "PG-AB-015436":
                        case "PG-AB-015437":
                        case "PG-AB-015438":
                        case "PG-AB-015439":
                        case "PG-AB-015440":
                        case "PG-AB-015441":
                            sumapromo += item.Total;
                            if (!codprod && item.Id == productotmp.Id)
                            {
                                codprod = true;
                            }

                            if (sumapromo >= 5)
                                tienepromo = true;

                            break;

                    }
                }
                var cantidad = Math.Truncate(sumapromo / 5);
                if (tienepromo && codprod)
                {
                    System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar hasta " + cantidad.ToString() + " STAYFREE ALAS X 10U (PAGUE 12 LLEVE 15)");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];


                    switch (item.Id)
                    {
                        case "PG-AB-012461":
                            if (tienepromo)
                            {
                                //                                var porcentaje = cantidad < item.Cantidad ? Math.Round((cantidad / item.Cantidad) * 100, 2, MidpointRounding.AwayFromZero) : 100;
                                var porcentaje = cantidad < item.Cantidad ? cantidad : item.Cantidad;
                                item.DescuentoAX = ((porcentaje) * (item.Pvp));

                                //item.calcularDescuento(porcentaje);

                            }
                            else
                            {
                                item.DescuentoAX = 0M;
                                item.calcularDescuento(0);
                            }
                            //item.update();
                            // item.calcularDescuento(100);
                            //item.Descuento = item.Pvp * item.Cantidad;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            break;

                    }
                }

            }

        }
        private void promopulpa(Factura _factura)
        {
            return;
            var db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PROMO_PULPA" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PT-CR-000130":
                            if (item.Cantidad >= 3)
                                tienepromo = true;

                            break;

                    }
                }

                if (tienepromo)
                {
                    System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar una malla de verdura gratis al 100%");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    var auxunidades = 0;

                    switch (item.Id)
                    {
                        case "PG-AB-015173":
                            if (tienepromo)
                            {

                                auxunidades = 1;
                            }
                            else
                            {
                                auxunidades = 0;
                            }

                            item.DescuentoAX = ((auxunidades) * item.Pvp);
                            var existente = getExistente(item.Id);
                            item.update();
                            existente.update();
                            calcularFactura();

                            break;

                    }
                }

            }

        }
        private void promohotdog(Factura _factura, Producto productotmp)
        {
            return;
            var db = new POSEntities();
            var canthotdog = 0M;
            var cantpanes = 0M;
            if (db.core_parametro.Where(x => x.identificador == "PROMO_HOTDOG" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PT-ER-000038":
                            canthotdog = canthotdog + item.Cantidad;
                            break;
                        case "PG-AB-015570":
                            cantpanes = cantpanes + item.Cantidad;
                            break;
                    }
                }
                if (canthotdog >= 2 && cantpanes >= 1)
                {
                    tienepromo = true;

                }
                var cantidad = 0M;
                if (tienepromo)
                {
                    if (Math.Truncate(canthotdog / 2) <= cantpanes)
                        cantidad = Math.Truncate(canthotdog / 2);
                    else
                        cantidad = cantpanes;

                    //cantidad = Math.Truncate(canthotdog / 2);
                    if (productotmp.Id == "PT-ER-000038" || productotmp.Id == "PG-AB-015570")
                        System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar hasta " + cantidad.ToString() + " COCA COLA 1.350 LT. GRATIS");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];


                    switch (item.Id)
                    {
                        case "PG-AB-015504":
                            if (tienepromo)
                            {
                                // var porcentaje = cantidad < item.Cantidad ? Math.Round((cantidad / item.Cantidad) * 100, 2, MidpointRounding.AwayFromZero) : 100;
                                var porcentaje = cantidad < item.Cantidad ? cantidad : item.Cantidad;
                                item.DescuentoAX = ((porcentaje) * item.Pvp);
                                //item.calcularDescuento(porcentaje);
                            }
                            else
                            {
                                item.DescuentoAX = 0M;
                                item.calcularDescuento(0);
                            }
                            //item.update();
                            // item.calcularDescuento(100);
                            //item.Descuento = item.Pvp * item.Cantidad;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            break;

                    }
                }

            }

        }
        private void promochifle(Factura _factura, Producto productotmp)
        {
            return;
            var db = new POSEntities();
            var cantchifle = 0M;
            if (db.core_parametro.Where(x => x.identificador == "PROMO_CHIFLE" && x.valor == "TRUE" && (x.parametro2 == _factura.Establecimiento || x.parametro2 == null)).FirstOrDefault() != null)
            {
                var tienepromo = false;
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    switch (item.Id)
                    {
                        case "PG-AB-013675":
                            cantchifle = cantchifle + item.Cantidad;
                            break;
                        case "PG-AB-013676":
                            cantchifle = cantchifle + item.Cantidad;
                            break;
                    }
                }
                if (cantchifle >= 2)
                {
                    tienepromo = true;

                }
                var cantidad = 0M;
                if (tienepromo)
                {
                    cantidad = Math.Truncate(cantchifle / 2);
                    if (productotmp.Id == "PG-AB-013675" || productotmp.Id == "PG-AB-013676")
                        System.Windows.Forms.MessageBox.Show(this, "Recuerde al Cliente que puede llevar hasta " + cantidad.ToString() + " CHIFLE DE 70G. GRATIS");
                }

                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];


                    switch (item.Id)
                    {
                        case "PG-AB-013677":
                            if (tienepromo)
                            {
                                //                                var porcentaje = cantidad < item.Cantidad ? Math.Round((cantidad / item.Cantidad) * 100, 2, MidpointRounding.AwayFromZero) : 100;
                                var porcentaje = cantidad < item.Cantidad ? cantidad : item.Cantidad;
                                item.DescuentoAX = ((porcentaje) * item.Pvp);

                                //item.calcularDescuento(porcentaje);

                            }
                            else
                            {
                                item.DescuentoAX = 0M;
                                item.calcularDescuento(0);
                            }
                            //item.update();
                            // item.calcularDescuento(100);
                            //item.Descuento = item.Pvp * item.Cantidad;
                            item.update();
                            var existente = getExistente(item.Id);
                            existente.update();
                            calcularFactura();
                            break;

                    }
                }

            }

        }

        private void promoiva(Factura _factura, Producto productotmp)
        {
            var db = new POSEntities();

            if (Control.Common.GlobalParameters.PROMO_IVA)
            {
               // var porcPromo = Decimal.Parse((db.core_parametro.First(x => x.identificador == "DESC_PROMO_IVA").parametro2));
                for (var i = 0; i < _factura.Productos.Count; i++)
                {
                    var item = _factura.Productos[i];
                    if (POS.Control.Common.Promo.EsDiaPromoIVA() && item.IvaProducto != 0 && item.EsExcluidoPromoIVA == false)
                    {
                        var descuentoPromoIVA = (item.Subtotal * item.IvaProducto);
                        item.DescuentoIVA = descuentoPromoIVA;
                    }

                }

            }

        }

        private void promoivaSingle(string codigo)
        {
            var db = new POSEntities();

            if (Control.Promos.PromoIVA.EsPromoIVA())
            {
                var item = GetItemExistente(codigo);
                if (item != null)
                {
                    if (item.IvaProducto != 0 && item.EsExcluidoPromoIVA == false)
                    {
                        var descuentoPromoIVA = (item.Subtotal * item.IvaProducto);
                        item.DescuentoIVA = descuentoPromoIVA;
                    }
                }
            }

        }


        private void promocuponapp()
        {
            
            if (_factura.ObjCuponApp != null)
            {
                if (_factura.ObjCuponApp.IdTblPremio >= 0)
                {
                    _factura.ObjCuponApp.SeUsoCuponApp = false;
                    if (_factura.ObjCuponApp.Lstitem.Count > 0)
                    {
                        int validarSoloUnItem = 0;

                        for (var i = 0; i < _factura.Productos.Count; i++)
                        {
                            var pro = _factura.Productos[i];
                            foreach (var c in _factura.ObjCuponApp.Lstitem)
                            {

                                if (pro.Id == c.ItemId && validarSoloUnItem == 0)
                                {

                                    var porcentaje = (_factura.ObjCuponApp.Valor / 100);
                                    pro.Descuento = ((porcentaje) * pro.Pvp);

                                    if (pro.Descuento > 0) {
                                        pro.Iva = (pro.Pvp - pro.Descuento) * pro.IvaProducto;
                                        pro.Total = pro.Pvp - pro.Descuento;
                                        
                                    }
                                

                                    _factura.ObjCuponApp.SeUsoCuponApp = true;

                                    if (_factura.ObjCuponApp.CantAplicar == 1)
                                    {
                                        validarSoloUnItem = 1;
                                    }
                                    //break;
                                }
                            }
                        }
                    }
                }
            }
        }


        private void MainWindow_Deactivate(object sender, EventArgs e)
        {
            // CloseTaskManager();
            // StartHiddenTaskManager();
            // HideTaskManager();

            if (!Es2X_CONSULTA_POS)
            {
                this.WindowState = FormWindowState.Maximized;
                SetForegroundWindow(this.Handle);
            }
        }

        private void btnGarancheck_Click(object sender, EventArgs e)
        {
            GarancheckForm gr = new GarancheckForm();
            gr.ShowDialog();
        }


        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (!Es2X_CONSULTA_POS)
            {
                this.Left = 0;
                this.Top = 0;
                this.Height = Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = Screen.PrimaryScreen.WorkingArea.Width;
            }
            //this.WindowState = FormWindowState.Maximized;
        }


        private void btnKbd_Click(object sender, EventArgs e)
        {
            //pnlKbd.Visible = !pnlKbd.Visible;
            Control.Common.General.TecladoPantalla();
        }

        private void btnBusqProd_Click(object sender, EventArgs e)
        {
            if (txtCedula.Text.Length > 0 && txtCedula.Text.Length <= 13)
            {
                SearchProductV3 sp = new SearchProductV3("", _factura.Establecimiento);
                sp.Top = 20;
                sp.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2) - (sp.Width / 2);
                sp.TopMost = true;
                sp.ShowDialog();
                txtCodigo.Text = sp.code;
                getProducto(txtCodigo.Text);
                txtCodigo.Clear();
                txtCodigo.Focus();
            }
            else
            {
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No existe cliente activo.", "POS", 20, false);
                Control.Common.General.GetMensajeToList(155);
            }
        }

        private void f1()
        {
            POSEntities db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "AUTHORIZE_F1" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
            {

                if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
                {
                    Verifier = new VerificationForm(Data, _factura);
                    Verifier.Tag = "aud";
                    verificador = Verifier.ShowDialog();
                    if (verificador == DialogResult.OK)
                    {
                        OpcionesAuditor optAud = new OpcionesAuditor();
                        DialogResult verOptAud = optAud.ShowDialog();

                        switch (optAud.Tag.ToString())
                        {
                            case "1":
                                _factura.prepararCorte();

                                if (_factura.ReciboCorte != null)
                                {
                                    imprimir(_factura.ReciboCorte);

                                }
                                break;
                            case "2":
                                string cadena = "Codigo/Descripcion\n            Cantidad                 Total \n-------------------------------------------------\n";


                                foreach (var prod in db.BG_Lst_POS_no_sub_AX.Where(x => x.establecimiento == this._factura.Establecimiento))
                                {
                                    cadena += String.Concat(prod.item_id, "\n", prod.item_nombre, "\n                 ", prod.cantidad, "           ", prod.total, "\n");

                                }

                                imprimir(cadena);
                                break;
                        }

                    }
                    else
                    {
                        //MsgBoxCtrl.MessageBoxResult objMsj = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Auditor no autorizado", "POS - Autorización");
                        //System.Windows.Forms.MessageBox.Show(this, "Auditor no autorizado", "Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Control.Common.General.GetMensajeToList(9002);

                    }
                }
                else

                {
                    try
                    {
                        DialogResult _authorize;
                        _inputFormAuthUser.setValue(String.Empty);
                        _inputFormAuthUser._txtInput.PasswordChar = '•';
                        _authorize = _inputFormAuthUser.ShowDialog();
                        focused = (System.Windows.Forms.Control)_inputFormAuthUser._txtInput;
                    }
                    catch (Exception ex)
                    {
                        // MsgBoxCtrl.MessageBoxResult objMsj = msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Por favor, intente nuevamente", "POS - Cuadre de Caja");
                        Control.Common.General.GetMensajeToList(156);

                        //System.Windows.Forms.MessageBox.Show(this, "Por favor, intente nuevamente", "Cuadre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }

                    if (focused.Text.ToString() != "")
                    {
                        if (ValidateAuthorizationAuditor(focused.Text.ToString()) == true)
                        {
                            _factura.prepararCorte();

                            if (_factura.ReciboCorte != null)
                            {
                                imprimir(_factura.ReciboCorte);

                            }
                        }
                        else
                        {
                            //System.Windows.Forms.MessageBox.Show(this, "Auditor no autorizado", "Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                            //MsgBox msgBox = new MsgBox("info", "Auditor no autorizado", "POS - Autorización");
                            //DialogResult dialogResult = msgBox.ShowDialog();
                            
                            // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " Auditor no autorizado ", " POS - Autorización");
                            Control.Common.General.GetMensajeToList(9002);

                        }
                    }
                    else
                    {

                    }
                }
            }

        }
        private void f2()
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "f2", "Cierre de caja solicitado. Usuario logon: " + (Control.Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Control.Common.GlobalParameters.UserObj.username));
            List<ParametrosMensajes> ParametrosMensajes = new List<ParametrosMensajes>();
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            
            POSEntities db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
            {
                Verifier = new VerificationForm(Data, _factura);
                Verifier.Tag = "usr";
                verificador = Verifier.ShowDialog();

                ParametrosMensajes = new List<ParametrosMensajes>();
                ParametrosMensajes.Add(new ParametrosMensajes() { codigo = "[NOMBRE_USUARIO]", valor = _current_user.nombres });
                

                if (verificador == DialogResult.OK)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "f2", "Se ha verificado la identiidad Indentidad con éxito");

                    var result = Control.Common.General.GetMensajeToList(283, ParametrosMensajes);

                    //if (System.Windows.Forms.MessageBox.Show(this, "Usuario: " + _current_user.nombres + "\n\nDesea cerrar la caja?\nEste proceso no se puede revertir", "ADVERTENCIA!!!!!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                    {
                        try
                        {
                            if (db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                            {
                                //Control.Common.General.GetMensaje(43, ParametrosMensajes);
                                Control.Common.General.GetMensajeToList(284);
                                
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " Se va a imprimir el lote de transacciones de Tarjetas de Crédito ", " POS - Cierre de Lote");
                                //System.Windows.Forms.MessageBox.Show(this, "Se va a imprimir el lote de transacciones de Tarjetas de Crédito");

                                _factura.prepararCorteLote();

                                if (_factura.ReciboCorteLote != null)
                                {
                                    imprimir(_factura.ReciboCorteLote);

                                }
                            }
                            SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                            using (conexion2)
                            {
                                Int64 registro;
                                decimal valor = ObtenAvance(out registro);

                                conexion2.Open();
                                String Query1 = "UPDATE TBL_MONTOAPERTURA  SET PRE_CIERRE= 1 WHERE recid=" + registro;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "f2", "Enviando a actualizar registro de monto apertura: " + Query1);

                                SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                comandoupd.ExecuteNonQuery();
                                comandoupd.CommandTimeout = 5000;
                                conexion2.Close();
                                imprimir("Caja Cerrada\nUsuario: " + _current_user.nombres);

                            }
                        }
                        catch (Exception ex)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "f2", "No fue posible modificar el campo PRE_CIERRE de la tbl_montoapertura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                            //System.Windows.Forms.MessageBox.Show(this, ex.ToString(), "Mensaje");

                            ParametrosMensajes = new List<ParametrosMensajes>();
                            ParametrosMensajes.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.ToString() });
                            Control.Common.General.GetMensajeToList(285, ParametrosMensajes);
                        }

                        Control.Common.General.GetMensajeToList(286);
                        //System.Windows.Forms.MessageBox.Show(this, "Caja Cerrada\nSaliendo del programa");
                        POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                        Application.Exit();
                    }
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show(this, "Usuario no autorizado", "Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    Control.Common.General.GetMensajeToList(9007);
                }

            }
            else
            {
                try
                {
                    DialogResult _authorize;
                    _inputFormAuthUser = new InputBoxDialog("Ingrese código de autorización", "Cierre de Caja");
                    _inputFormAuthUser.setValue(String.Empty);
                    _inputFormAuthUser._txtInput.PasswordChar = '•';
                    _authorize = _inputFormAuthUser.ShowDialog();
                    focused = (System.Windows.Forms.Control)_inputFormAuthUser._txtInput;
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "f2", "No fue posible realizar la validacion manual del usuario logueado, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                    //System.Windows.Forms.MessageBox.Show(this, "Por favor, intente nuevamente", "Cuadre de Caja", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Control.Common.General.GetMensajeToList(9006);
                    

                }

                if (focused.Text.ToString() != "")
                {
                    if (ValidateAuthorizationUser(focused.Text.ToString()) == true)
                    {

                        ParametrosMensajes = new List<ParametrosMensajes>();
                        ParametrosMensajes.Add(new ParametrosMensajes() { codigo = "[NOMBRE_USUARIO]", valor = _current_user.nombres });
                        var result = Control.Common.General.GetMensajeToList(283, ParametrosMensajes);


                        //if (System.Windows.Forms.MessageBox.Show(this, "Usuario: " + _current_user.nombres + "\n\nDesea cerrar la caja?\nEste proceso no se puede revertir", "ADVERTENCIA!!!!!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                        {
                            try
                            {
                                SqlConnection conexion2 = new SqlConnection(POS.Properties.Settings.Default.CONECTA_AX);
                                using (conexion2)
                                {
                                    Int64 registro;
                                    decimal valor = ObtenAvance(out registro);

                                    conexion2.Open();
                                    String Query1 = "UPDATE TBL_MONTOAPERTURA  SET PRE_CIERRE= 1 WHERE recid=" + registro;

                                    SqlCommand comandoupd = new SqlCommand(Query1, conexion2);
                                    comandoupd.ExecuteNonQuery();
                                    conexion2.Close();
                                    imprimir("Caja Cerrada\nUsuario: " + _current_user.nombres);
                                }
                            }
                            catch (Exception ex)
                            {
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "f2", "No fue posible modificar el campo PRE_CIERRE de la tbl_montoapertura, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                                //System.Windows.Forms.MessageBox.Show(this, ex.ToString(), "Mensaje");

                                parametros = new List<ParametrosMensajes>();
                                parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.ToString() });
                                Control.Common.General.GetMensajeToList(285, parametros);
                            }

                            Control.Common.General.GetMensajeToList(286);
                            //System.Windows.Forms.MessageBox.Show(this, "Caja Cerrada\nSaliendo del programa");
                            POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                            Application.Exit();
                        }
                    }
                    else
                    {
                        //System.Windows.Forms.MessageBox.Show(this, "Usuario no autorizado", "Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Control.Common.General.GetMensajeToList(9006);

                    }
                }
            }
        }

        private void f4()
        {
            Verifier = new VerificationForm(Data, _factura);
            Verifier.Tag = "adm";
            if (_factura.User.isSuperUser)
            {
                verificador = DialogResult.OK;
            }
            else
            {
                verificador = Verifier.ShowDialog();
            }
            if (verificador == DialogResult.OK)
            {
                FrmPP = new Control.PINPAD.PagoPINPAD(ref _factura);
                FrmPP.ShowDialog();
            }

        }

        private void f8()
        {
            //JCanarte 11Enero2021 Reimpresión de Facturas
            Verifier = new VerificationForm(Data, _factura);
            Verifier.Tag = "adm";
            if (_factura.User.isSuperUser)
            {
                verificador = DialogResult.OK;
            }
            else
            {
                verificador = Verifier.ShowDialog();
            }
            if (verificador == DialogResult.OK)
            {
                FrmReImpFac = new Control.ReImprimirFactura.ReImprimirFactura(ref _factura);
                FrmReImpFac.ShowDialog();
            }

        }

        private void f10()
        {
            bool autorizado = false;
            using (Control.Auth.CredentialAuth frmAuth = new Control.Auth.CredentialAuth("Ud. está viendo esta pantalla porque pulsó F10 para abrir herramientas administrativas. Si no era su opción deseada, pulse el botón Cancelar"))
            {
                frmAuth.ShowDialog();
                autorizado = frmAuth.EsAutorizado;
            }

            if (autorizado)
            {
                Control.ToolBox.ToolBoxMenu frmToolBox = new Control.ToolBox.ToolBoxMenu(this);
                frmToolBox.ShowDialog();
            }
        }

        private void f6()
        {
            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "F6", "Impresión de Resumen de Cobros solicitado. Usuario logon: " + (Control.Common.GlobalParameters.UserObj == null ? "No hay logon de usuario en objeto UserObj" : Control.Common.GlobalParameters.UserObj.username));

            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
            


            POSEntities db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "FINGERPRINT" && x.parametro2 == establecimiento_inicio).First().valor == "TRUE")
            {
                Verifier = new VerificationForm(Data, _factura);
                Verifier.Tag = "usr";
                verificador = Verifier.ShowDialog();
                if (verificador == DialogResult.OK)
                {

                    parametros.Add(new ParametrosMensajes() { codigo = "[nombre_usuario]", valor = _current_user.nombres });
                    var result = Control.Common.General.GetMensajeToList(287, parametros);

                    //if (System.Windows.Forms.MessageBox.Show(this, "Usuario: " + _current_user.nombres + "\n\nDesea imprimir el resumen de cobros de tarjetas de crédito?\nNota:   No es reporte definitivo..", "INFORMATIVO!!!!!!!!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                    {
                        try
                        {
                            if (db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
                            {
                                //System.Windows.Forms.MessageBox.Show(this, "Se va a imprimir el lote de transacciones de Tarjetas de Crédito");
                                Control.Common.General.GetMensajeToList(288);

                                _factura.prepararCorteLoteNoDefinitivo();

                                if (_factura.ReciboCorteLote != null)
                                {
                                    imprimir(_factura.ReciboCorteLote);
                                }
                            }                            
                        }
                        catch (Exception ex)
                        {
                            parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.ToString() });
                            Control.Common.General.GetMensajeToList(289, parametros);

                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "f6", "No fue posible imprimir el resumen de cobros, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                            //System.Windows.Forms.MessageBox.Show(this, ex.ToString(), "Mensaje");
                        }   
                    }
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show(this, "Usuario no autorizado", "Autorización", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    Control.Common.General.GetMensajeToList(9007);
                }
            }
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnF1_Click(object sender, EventArgs e)
        {
            f1();
        }

        private void btnF2_Click(object sender, EventArgs e)
        {
            f2();
        }

        private void btnF4_Click(object sender, EventArgs e)
        {
            POSEntities db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "PINPAD" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
            {
                f4();
            }
            else
            {
                Control.Common.General.GetMensajeToList(169);
                //System.Windows.Forms.MessageBox.Show(this, "No tiene habilitada esta opcion");

            }
        }

        private void btnF8_Click(object sender, EventArgs e)
        {
            //JCanarte 11Ene2021
            POSEntities db = new POSEntities();
            if (db.core_parametro.Where(x => x.identificador == "REIMPRIME_FACTURA" && x.parametro2 == this._factura.Establecimiento).First().valor == "TRUE")
            {
                f8();
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show(this, "No tiene habilitada esta opcion");
                Control.Common.General.GetMensajeToList(169);

            }
        }

        private void BTN_DIREC_1_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = BotonDirec1;
            txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));
        }

        private void BTN_DIREC_2_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = BotonDirec2;
            txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));
        }

        private void BTN_DIREC_3_Click(object sender, EventArgs e)
        {
            if (!Es2X_CONSULTA_POS)
            {
                txtCodigo.Text = BotonDirec3;
                txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));
            }
            else
            { this.Close(); }
        }

        private void BTN_DIREC_4_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = BotonDirec4;
            txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));
        }

        private void BTN_DIREC_5_Click(object sender, EventArgs e)
        {
            txtCodigo.Text = BotonDirec5;
            txtCodigo_KeyPress(this, new KeyPressEventArgs((char)(Keys.Enter)));
        }

        private void cmbLocalConsulta_TextChanged(object sender, EventArgs e)
        {
            string CodigoLocal = "";
            switch (cmbLocalConsulta.Text)
            {
                case "PIAZZA":
                    CodigoLocal = "024";
                    break;
                case "ORELLANA":
                    CodigoLocal = "011";
                    break;
                case "ALBORADA":
                    CodigoLocal = "007";
                    break;
                case "GRAN MANZANA":
                    CodigoLocal = "012";
                    break;
                case "PPG":
                    CodigoLocal = "006";
                    break;
                case "GOMEZ RENDON":
                    CodigoLocal = "008";
                    break;
                case "VILLA CLUB":
                    CodigoLocal = "029";
                    break;
                case "LA JOYA":
                    CodigoLocal = "037";
                    break;
            }

            var pos = new POSEntities();
            var PuntoEmision = pos.core_puntoemision.Single(x => x.ip_address == _factura.Ip_address);
            PuntoEmision.establecimiento_id = CodigoLocal;
            pos.SaveChanges();

            CargarMainWindow();
            this.Text = "POS DE CONSULTAS 2X";
        }

        private void btnCorresponsal_Click(object sender, EventArgs e)
        {
            //Si los parametros no estan correctamente cargados, intentar consultarlos de nuevo ahora
            if (!Control.CorrBan.ClsCorrBan.EsCorresponsalActivo)
            {
                var result = Control.Common.General.GetMensajeToList(167);

                if (result == MsgBoxCtrl.MessageBoxResult.Ok || result == MsgBoxCtrl.MessageBoxResult.Yes)
                {
                    Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();
                }

                //MsgBox msgBox = new MsgBox("question", " No habilitado para el local. ¿Desea intentar recargarlos ahora?", "Red Activa");
                //DialogResult dialogResult = msgBox.ShowDialog();
                //if (dialogResult == DialogResult.Yes || dialogResult == DialogResult.OK)
                //{
                //    Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();
                //}

                //if (System.Windows.Forms.MessageBox.Show("No habilitado para el local. ¿Desea intentar recargarlos ahora?", "Red Activa", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //    Control.CorrBan.ClsCorrBan.CargarParametrosRedActiva();
                //}
            }

            //Abrir el menu de corresponsal solo si los parametros estan correctamente cargados
            if (Control.CorrBan.ClsCorrBan.EsCorresponsalActivo)
            {
                POS.Control.CorrBan.MenuPrincipal _frm = new POS.Control.CorrBan.MenuPrincipal(this);
                _frm.ShowDialog();
            }
            else
            {
                Control.Common.General.GetMensajeToList(168);
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "  No habilitado para el local o bien los parámetros de Red Activa no se cargaron correctamente.", " POS - Red Activa ");

                //// System.Windows.Forms.MessageBox.Show("No habilitado para el local o bien los parámetros de Red Activa no se cargaron correctamente.");
                //MsgBox msgBox = new MsgBox("info", " No habilitado para el local o bien los parámetros de Red Activa no se cargaron correctamente.", "Red Activa");
                //DialogResult dialogResult = msgBox.ShowDialog();

            }

        }

        private void btnPrecio_Click(object sender, EventArgs e)
        {
            MainWindowV1 clonedMainWindow = (MainWindowV1)this.MemberwiseClone();

            POS.Control.Main.PriceViewer frmPriceViewer = new Control.Main.PriceViewer(clonedMainWindow);
            frmPriceViewer.ShowDialog();
            GC.Collect();
        }

        private void btnWallet_Click(object sender, EventArgs e)
        {
            Control.WalletPoints.MenuPrincipal frm = new Control.WalletPoints.MenuPrincipal(cliente_actual != null ? cliente_actual.ACCOUNTNUM : string.Empty);
            frm.ShowDialog();
        }

        public bool HasPayTipe(string type)
        {
            if (_factura == null) return false;

            if (_factura.Pagos == null) return false;

            return _factura.Pagos.Any(x => x.Descripcion == type);
        }

        private void btnCliente_Click(object sender, EventArgs e)
        {
            if (cliente_actual != null)
            {
                if (cliente_actual.ACCOUNTNUM != POS.Control.Common.GlobalParameters.IdConsumidorFinal)
                {
                    var f = new POS.Control.Clientes.ClienteForm();
                    f._cliente = cliente_actual;
                    f.DeseaPermitirCambioBasico = true;
                    f.ShowDialog();
                    if (f._cliente != null)
                    {
                        cliente_actual = f._cliente;
                        setClienteData();
                        txtCodigo.Focus();
                        txtCodigo.SelectAll();
                    }
                }
            }
        }


        BackgroundWorker bgw;
        BackgroundWorker bgwGrabaDocumento;
        
  
        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

       
        public void EstresarPOSBackGround()
        {
            if (bgw == null)
            {
                bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            }
            bgw.WorkerReportsProgress = true;
            bgw.WorkerSupportsCancellation = true;
            bgw.RunWorkerAsync();
            System.Threading.Thread.Sleep(500);
        }
        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            EstresarPOS();
        }

 
        private void EstresarPOS()
        {
            try
            {
                for (int i = 1; i <= 50; i++)
                {
                    txtCodigo.Clear();
                    txtCodigo.Text = "PG-AB-" + i.ToString().PadLeft(6, '0');
                    txtCodigo.Focus();

                    if (clienteActivo())
                    {
                        var val = txtCodigo.Text.Trim();

                        bool encontroCupon = false;
                        if (AplicarDescuentoCupon(val, ref encontroCupon))
                        {
                            txtCodigo.Clear();
                            txtCodigo.Focus();
                            return;
                        }
                        else if (encontroCupon)
                        {
                            txtCodigo.Clear();
                            txtCodigo.Focus();
                            return;
                        }

                        if (_factura.Documento == "F")
                        {

                            if (_factura.Establecimiento == "029")
                            {
                                getProductoCP(val);
                                calcularFactura();
                                txtCodigo.Clear();
                                txtCodigo.Focus();
                                //    return;


                            }

                            getProducto(val);
                        }
                        else
                        {
                            getGiftCard(val);
                        }
                        txtCodigo.Clear();
                        txtCodigo.Focus();
                    }
                    else
                    {
                        string textAlerta = "No ha escogido un cliente!";
                        ShowDesktopAlert("Acción no valida", textAlerta, position: AlertScreenPosition.TopCenter, autoCloseDelay: 2);
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "EstresarPOS", textAlerta);

                        txtCedula.Focus();
                        txtCedula.SelectAll();
                    }


                    txtCodigo.Clear();
                    txtCodigo.Focus();

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {


                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "EstresarPOS", "Ocurrió un incidente mientras se estresaba POS, se enviará a cerrar el aplicativo. A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //POS.Control.Common.WinForm.ShowMessage("El aplicativo o uno de sus componentes gráficos tuvo problemas durante la operacion, se enviará a cerrar POS para evitar cualquier descuadre");

                Control.Common.General.GetMensajeToList(290);

                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        //public void EjecutarPistoleoMetricas()
        //{
        //    pnlPistoleoMetrica.Visible = !pnlPistoleoMetrica.Visible;
        //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "EjecutarPistoleoMetricas", "Las metricas acaban de ser '" + (pnlPistoleoMetrica.Visible ? "Habilitadas" : "Deshabilitadas") + "'");
        //}

        private void btnParqueo_Click(object sender, EventArgs e)
        {
            if (Control.Common.GlobalParameters.Parking_TienePermiso)
            {
                SolicitarParqueo();
            }
        }

        private void SolicitarParqueo()
        {
    

            if (cliente_actual != null)
            {
                if (_factura.Productos.Any(x => Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(y => y.Equals(x.Id))))
                {
                    Control.Common.General.GetMensajeToList(291);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No puede solicitar ítem de parqueo porque ya hay un ítem de pérdida en la factura ", " POS - Parqueo ");
                    //Control.Common.WinForm.ShowMessage("No puede solicitar ítem de parqueo porque ya hay un ítem de pérdida en la factura");
                    return;
                }

                var frmParqueo = new POS.Control.Parking.CapturaParqueo();
                frmParqueo.ShowDialog();
                if (frmParqueo.ObjParking != null)
                {
                    if (frmParqueo.ObjParking.EstaConfirmado)
                    {
                        _factura.ObjParking = frmParqueo.ObjParking.Clone();
                        CalcularParqueo();
                    }
                }
                else if (frmParqueo.NoTieneTicket)
                {
                    _factura.ObjParking = null;
                    var listaItems = _factura.Productos.Where(x => Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(y => y.Equals(x.Id))).ToList();
                    if (listaItems.Count > 0)
                    {
                        foreach (var prod in listaItems)
                        {
                            _factura.Productos.Remove(prod);
                        }
                        calcularFactura();
                    }
                }
                frmParqueo.Dispose();
                GC.Collect();
            }
        }

        private void SolicitarCuponApp()
        {
            if (cliente_actual != null)
            {
                if (_factura.ObjCuponApp!=null)
                {
                    Control.Common.General.GetMensajeToList(292);
                    //Control.Common.General.GetMensaje("POS - CuponApp", "Solo puede aplicar 1 cupón por producto. Ya existe un cupón agregado al producto", "I");
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Solo puede aplicar 1 cupón por producto. Ya existe un cupón agregado al producto", " POS - CuponApp ");
                    //Control.Common.WinForm.ShowMessage("Solo puede aplicar 1 cupón por producto. Ya existe un cupón agregado al producto");
                    return;
                }


                string lblIdClienteApp = this.lblIdClienteApp.Text;
                var frmCupon = new POS.Control.WalletPoints.CapturaCupon(lblIdClienteApp);
                
                frmCupon.ShowDialog();
                

                if (frmCupon.ObjCuponApp  != null)
                {
                    if (frmCupon.ObjCuponApp.EstaConfirmado)
                    {
                        _factura.ObjCuponApp = frmCupon.ObjCuponApp.Clone();
                        promocuponapp();
                        calcularFactura();
                    }
                }
                else if (frmCupon.NoTieneTicket)
                {
                    _factura.ObjCuponApp = null;
                   
                        calcularFactura();
                }
                frmCupon.Dispose();
                GC.Collect();
            }
        }

        private void GenerarSalidaPorPerdida()
        {
            var itemPerdida = _factura.Productos.Where(x => x.Id == Control.Common.GlobalParameters.Parking_ItemPerdidaTicket).FirstOrDefault();
            if (itemPerdida != null)
            {
                string codigo = string.Empty;
                if (Control.Common.GlobalParameters.Parking_PathSalida == "API")
                {
                    WebMethods wbser = new WebMethods();
                    var res = wbser.GeneraTicket(Control.Common.GlobalParameters.Parking_WSGetTicket);
                    if (res == null || res == "")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GenerarSalidaPorPerdida|wbser.GeneraTicket", "El api " + Control.Common.GlobalParameters.Parking_WSGetTicket + " respondio null");
                    }
                    else
                    {

                       codigo = res;
                        _factura.ObjParkingLost = new Models.Parking.clsParking() { Codigo = res };

                        Models.Parking.ParkingRequest requestData = new Models.Parking.ParkingRequest();
                        requestData.url = codigo;
                        requestData.full_name = _factura.Cliente_nombre;
                        requestData.identification = _factura.ClienteIdentificacion;
                        requestData.phone = _factura.Cliente_telefono;
                        requestData.address = _factura.Cliente_direccion;
                        requestData.email = "";
                        requestData.total_value = itemPerdida.Total;

                        var resSal = wbser.PagarTicketParqueo(requestData, Control.Common.GlobalParameters.Parking_WSSalida);
                        if (resSal == null)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GenerarSalidaPorPerdida|wbser.PagarTicketParqueo", "El api " + Control.Common.GlobalParameters.Parking_WSSalida + " respondio null");
                        }
                    }

                }
                else
                {
                    var rnd = new Random();

                    bool esCodeDisponible = false;
                    
                    string path = string.Empty;

                    while (!esCodeDisponible)
                    {
                        codigo = string.Empty;
                        while (codigo.Length < 12)
                        {
                            codigo += rnd.Next(1, 9).ToString();
                        }
                        path = System.IO.Path.Combine(Control.Common.GlobalParameters.Parking_PathSalida, DateTime.Now.Date.ToString("yyyyMMdd"), codigo + ".txt");
                        //Si archivo no existe
                        if (!System.IO.File.Exists(path))
                        {
                            esCodeDisponible = true;
                        }
                    }

                    _factura.ObjParkingLost = new Models.Parking.clsParking() { Codigo = codigo };

                    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    //Crear archivo con hora de salida
                    System.IO.FileStream fileStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    TextWriterTraceListener listener = new TextWriterTraceListener(fileStream);
                    Trace.Listeners.Add(listener);
                    Trace.Write(string.Concat(new string[]
                    {
                        DateTime.Now.AddMinutes(Control.Common.GlobalParameters.Parking_MinutosGracia).ToString("HH:mm")
                    }));
                    Trace.Flush();
                    Trace.Close();
                    fileStream.Close();


                    //-----------------------------------------Genera nueva ENTRADA
                    path = System.IO.Path.Combine(Control.Common.GlobalParameters.Parking_PathIngreso, DateTime.Now.Date.ToString("yyyyMMdd"), codigo + ".txt");

                    directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    //Si archivo ya existe, eliminarlo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    //Crear archivo con hora de salida
                    fileStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    listener = new TextWriterTraceListener(fileStream);
                    Trace.Listeners.Add(listener);
                    Trace.Write(string.Concat(new string[]
                    {
                        DateTime.Now.ToString("HH:mm")
                    }));
                    Trace.Flush();
                    Trace.Close();
                    fileStream.Close();
                }

                if (Control.Common.GlobalParameters.Parking_PathSalida != "API")
                {
                    Barcode bc = new Barcode();
                    codigo = bc.encodeString(codigo);
                }
                   

                //Eliminar cualquier ticket previo que se haya creado (Cuando la facturacion no se completa por breve inconveniente)
                _factura.Cupon.RemoveAll(x => x.Referencia == "PERDIDAPARQUEO");

                //Agregar cupon desprendible
                _factura.Cupon.Add(new Cupones()
                {
                    Texto = Control.Common.GlobalParameters.Parking_ReciboPerdida
                                                    .Replace("<<codigo>>", codigo )
                                                    .Replace("<<aaaa-MM-dd>>", DateTime.Now.ToString("yyyy-MM-dd"))
                                                    .Replace("<<hh:mm:ss>>", DateTime.Now.ToString("HH:mm:ss"))
                                                    .Replace("<<mensaje>>", Control.Common.GlobalParameters.ParkingReciboPerdidaLeyenda)
                                                    .Replace("<<nombreLocal>>", Control.Common.GlobalParameters.EstablecimientoNombre)
                                                    .Replace("<<nroFactura>>", _factura.GetNumeroFactura())
                                                    .Replace("<<cajera>>", Control.Common.GlobalParameters.UserObj.nombres)
                                                    .Replace("<<horaDesde>>", Control.Common.GlobalParameters.Parking_HoraDesde)
                                                    .Replace("<<horaHasta>>", Control.Common.GlobalParameters.Parking_HoraHasta)
                                                    .Replace("<<fechaSalida>>", DateTime.Now.AddMinutes(Control.Common.GlobalParameters.Parking_MinutosGracia).ToString("dddd, dd MMMM yyyy H:mm")),
                    Valor = 0,
                    Unico = true,
                    Giftcard = false,
                    Valorgiftcard = 0,
                    Referencia = "PERDIDAPARQUEO"
                });
            
            }
        }

        private void GenerarSalidaParqueo()
        {
            if (_factura.ObjParking != null)
            {
                var fechaMomento = DateTime.Now;
                DateTime fechaIngresoNueva = fechaMomento, fechaSalidaNueva = new DateTime();
                if (_factura.ObjParking.TotalFracciones > 0)
                {
                    fechaSalidaNueva = fechaMomento.AddMinutes(_factura.ObjParking.MinutosGracia);
                }
                else
                {
                    if (_factura.ObjParking.TieneTiempoGraciaPorCompra)
                    {
                        TimeSpan diferenciaMinutos;
                        decimal totalPorParqueoActual = _factura.Productos.Where(x => Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(y => y.Equals(x.Id))).Sum(x => (decimal?)x.Total) ?? 0M;
                        decimal totalFac = _factura.ObjParking.DebeEnlazarFactura ? _factura.ObjParking.FacValorTotal : (_factura.GetTotal() - totalPorParqueoActual);

                        if (Control.Common.GlobalParameters.Parking_PathSalida == "API")
                        {
                            diferenciaMinutos = _factura.ObjParking.FechaIngreso.AddMinutes(totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2 ? _factura.ObjParking.MinutosLibreMaxPorCompra2 : (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ? _factura.ObjParking.MinutosLibreMaxPorCompra:0)) - fechaMomento;
                        }
                        else
                        {
                            diferenciaMinutos = _factura.ObjParking.FechaIngreso.AddMinutes(_factura.ObjParking.MinutosLibreMaxPorCompra) - fechaMomento;
                        }
                         
                        decimal diff = (diferenciaMinutos.TotalMinutes > 0 ? (int)diferenciaMinutos.TotalMinutes : 0);
                        if (diff < _factura.ObjParking.MinutosGracia)
                        {
                            fechaSalidaNueva = fechaMomento.AddMinutes(_factura.ObjParking.MinutosGracia);
                        }
                        else
                        {
                            if (Control.Common.GlobalParameters.Parking_PathSalida == "API")
                            {
                                fechaSalidaNueva = _factura.ObjParking.FechaIngreso.AddMinutes(totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2 ? _factura.ObjParking.MinutosLibreMaxPorCompra2: (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ? _factura.ObjParking.MinutosLibreMaxPorCompra : 0));
                            }
                            else
                            {
                                fechaSalidaNueva = _factura.ObjParking.FechaIngreso.AddMinutes(_factura.ObjParking.MinutosLibreMaxPorCompra);
                            }
                        }
                    }
                    else
                        fechaSalidaNueva = fechaMomento.AddMinutes(_factura.ObjParking.MinutosGracia);
                }

                if (_factura.ObjParking.PathSalida == "API")
                {
                    WebMethods wbser = new WebMethods();
                    Models.Parking.ParkingRequest requestData = new Models.Parking.ParkingRequest();
                    requestData.url = _factura.ObjParking.Codigo;
                    requestData.full_name = _factura.Cliente_nombre;
                    requestData.identification = _factura.ClienteIdentificacion;
                    requestData.phone = _factura.Cliente_telefono;
                    requestData.address = _factura.Cliente_direccion;
                    requestData.email = "";
                    requestData.total_value = _factura.ObjParking.FacValorTotal;
                    
                    var res = wbser.PagarTicketParqueo(requestData, Control.Common.GlobalParameters.Parking_WSSalida);
                    if (res == null)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "GenerarSalidaParqueo", "El api "+ Control.Common.GlobalParameters.Parking_WSSalida + " respondio null" );
                    }
                }
                else
                {


                    string path, pathDestino;
                    System.IO.DirectoryInfo directoryInfo;
                    System.IO.FileStream fileStream;
                    TextWriterTraceListener listener;

                    //----------------------------------------Respalda ENTRADA
                    //Path ingreso original
                    path = System.IO.Path.Combine(_factura.ObjParking.PathIngreso, DateTime.Now.Date.ToString("yyyyMMdd"), _factura.ObjParking.Codigo + ".txt");

                    pathDestino = System.IO.Path.Combine(_factura.ObjParking.PathIngreso,
                                                                        DateTime.Now.Date.ToString("yyyyMMdd"),
                                                                        "PROCESADO",
                                                                        _factura.ObjParking.Codigo +
                                                                            "_" +
                                                                            DateTime.Now.Hour.ToString("00") +
                                                                            DateTime.Now.Minute.ToString("00") +
                                                                            DateTime.Now.Second.ToString("00") +
                                                                            ".txt");
                    directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(pathDestino));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }

                    System.IO.File.Move(path, pathDestino);

                    //-----------------------------------------Genera nueva ENTRADA
                    directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    //Si archivo ya existe, eliminarlo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    //Crear archivo con hora de salida
                    fileStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    listener = new TextWriterTraceListener(fileStream);
                    Trace.Listeners.Add(listener);
                    Trace.Write(string.Concat(new string[]
                    {
                    fechaIngresoNueva.ToString("HH:mm")
                    }));
                    Trace.Flush();
                    Trace.Close();
                    fileStream.Close();


                    //----------------------------------------Archivo SALIDA
                    path = System.IO.Path.Combine(_factura.ObjParking.PathSalida, DateTime.Now.Date.ToString("yyyyMMdd"), _factura.ObjParking.Codigo + ".txt");

                    directoryInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(path));
                    //Si no existe directorio, crearlo
                    if (!directoryInfo.Exists)
                    {
                        directoryInfo.Create();
                    }
                    //Si archivo ya existe, eliminarlo
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    //Crear archivo con hora de salida
                    fileStream = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write);
                    listener = new TextWriterTraceListener(fileStream);
                    Trace.Listeners.Add(listener);
                    Trace.Write(string.Concat(new string[]
                    {
                    fechaSalidaNueva.ToString("HH:mm")
                    }));
                    Trace.Flush();
                    Trace.Close();
                    fileStream.Close();
                }
            }
        }

        private void CalcularParqueo()
        {
            if (_factura.ObjParking != null)
            {
                decimal totalPorParqueoActual = _factura.Productos.Where(x => Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(y => y.Equals(x.Id))).Sum(x => (decimal?)x.Total) ?? 0M;

                decimal totalFac = _factura.ObjParking.DebeEnlazarFactura ? _factura.ObjParking.FacValorTotal : (_factura.GetTotal() - totalPorParqueoActual);
                //Si lleva items asi sea que no llegue al minimo de compra para minutos libres
                bool tieneComprasAdicionales = _factura.Productos.Where(x => !Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(y => y.Equals(x.Id))).ToList().Count > 0;

                DateTime esteMomento = DateTime.Now;
                
                TimeSpan diferenciaMinutos;
                bool tendriaTiempoGraciaPorCompra;
                if (Control.Common.GlobalParameters.Parking_PathIngreso =="API")
                {
                    diferenciaMinutos = esteMomento - _factura.ObjParking.FechaIngreso.AddMinutes(
                                                                                    totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                                                                                        (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2 ? _factura.ObjParking.MinutosLibreMaxPorCompra2 : _factura.ObjParking.MinutosLibreMaxPorCompra)
                                                                                        :

                                                                                        ((esteMomento - _factura.ObjParking.FechaIngreso).TotalMinutes > _factura.ObjParking.MinutosGracia ? 0: _factura.ObjParking.MinutosGracia)
                                                                                        );

                    tendriaTiempoGraciaPorCompra = (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre || totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2);
                }
                else
                {
                    diferenciaMinutos = esteMomento - _factura.ObjParking.FechaIngreso.AddMinutes(
                                                                                    totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                                                                                        _factura.ObjParking.MinutosLibreMaxPorCompra
                                                                                        :
                                                                                        _factura.ObjParking.MinutosGracia);

                    tendriaTiempoGraciaPorCompra = (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre);
                }
                                
                 

                if (tendriaTiempoGraciaPorCompra != _factura.ObjParking.TieneTiempoGraciaPorCompra)
                {
                    //Para que vuelva a avisar cuanto tiene de gracia
                    _factura.ObjParking.DebeRealizarAvisoTiempoGracia = true;
                }
                _factura.ObjParking.TieneTiempoGraciaPorCompra = tendriaTiempoGraciaPorCompra;

                decimal diff = (diferenciaMinutos.TotalMinutes > 0 ? (int)diferenciaMinutos.TotalMinutes : 0);
                decimal minutosFraccion = (tieneComprasAdicionales ? _factura.ObjParking.MinutosFraccion : _factura.ObjParking.MinutosFraccionSinCompra);

                int totalFracciones = (int)Math.Ceiling((diff / minutosFraccion));

                if (totalFracciones > 0)
                {
                    //Para que vuelva a avisar la siguiente vez que no tenga fracciones
                    _factura.ObjParking.DebeRealizarAvisoTiempoGracia = true;

                    string itemIdParqueo = tieneComprasAdicionales ? _factura.ObjParking.ItemIdParqueo : _factura.ObjParking.ItemIdParqueoSinCompra;
                    //Borrar si hubieren items de parqueo por casos contrarios (Si tiene compras o no)
                    var itemParqueoNoDeseado = _factura.Productos.Where(x => x.Id.Equals(tieneComprasAdicionales ? _factura.ObjParking.ItemIdParqueoSinCompra : _factura.ObjParking.ItemIdParqueo)).FirstOrDefault();
                    if (itemParqueoNoDeseado != null)
                    {
                        _factura.Productos.Remove(itemParqueoNoDeseado);
                        calcularFactura();
                    }

                    var itemParqueo = _factura.Productos.Where(x => x.Id == itemIdParqueo).FirstOrDefault();
                    if (itemParqueo == null)
                    {
                        getProducto(itemIdParqueo, false);
                        itemParqueo = _factura.Productos.Where(x => x.Id == itemIdParqueo).FirstOrDefault();
                    }
                    else if (totalFracciones < itemParqueo.Cantidad)
                    {
                        _factura.Productos.Remove(itemParqueo);
                        getProducto(itemIdParqueo, false);
                        itemParqueo = _factura.Productos.Where(x => x.Id == itemIdParqueo).FirstOrDefault();
                    }

                    if (totalFracciones > itemParqueo.Cantidad)
                    {
                        itemParqueo.Cantidad = totalFracciones - 1;
                        itemParqueo.CantidadINEC = totalFracciones - 1;
                        itemParqueo.Unidades = totalFracciones - 1;
                        getProducto(itemIdParqueo, false);
                    }
                }
                else
                {
                    var listaItems = _factura.Productos.Where(x => x.Id == _factura.ObjParking.ItemIdParqueo || x.Id == _factura.ObjParking.ItemIdParqueoSinCompra).ToList();
                    if (listaItems.Count > 0)
                    {
                        foreach (var prod in listaItems)
                        {
                            _factura.Productos.Remove(prod);
                        }
                        calcularFactura();
                    }
                    if (_factura.ObjParking.DebeRealizarAvisoTiempoGracia)
                    {

                        List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

                        if (Control.Common.GlobalParameters.Parking_PathIngreso == "API")
                        {
                            if((totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                                                                                          (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2 ? _factura.ObjParking.MinutosLibreMaxPorCompra2 : _factura.ObjParking.MinutosLibreMaxPorCompra)
                                                                                           :
                                                                                           ((esteMomento - _factura.ObjParking.FechaIngreso).TotalMinutes > _factura.ObjParking.MinutosGracia ? 0 : _factura.ObjParking.MinutosGracia)) > 15){

                                Control.Common.General.GetMensajeToList(293);
                                //Control.Common.WinForm.ShowMessage("El cliente se encuentra dentro del periodo de gracia. Después de la emisión de la factura dispone de 15 minutos para salir.");

                            }
                            else
                            {
                                
                                _factura.ObjParking.FechaIngreso.AddMinutes(totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                                                                                              (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2 ? _factura.ObjParking.MinutosLibreMaxPorCompra2 : _factura.ObjParking.MinutosLibreMaxPorCompra)
                                                                                               : ((esteMomento - _factura.ObjParking.FechaIngreso).TotalMinutes > _factura.ObjParking.MinutosGracia ? 0 : _factura.ObjParking.MinutosGracia)).ToString("HH:mm");


                                parametros.Add(new ParametrosMensajes() { codigo = "[tiempo_maximo]", valor = _factura.ObjParking.FechaIngreso.ToString() });
                                Control.Common.General.GetMensajeToList(294, parametros);

                               // Control.Common.WinForm.ShowMessage("Indíquele al cliente que al momento se encuentra dentro del período de gracia y puede salir con normalidad máximo hasta las " +
                               //_factura.ObjParking.FechaIngreso.AddMinutes(totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                               //                                                               (totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre2 ? _factura.ObjParking.MinutosLibreMaxPorCompra2 : _factura.ObjParking.MinutosLibreMaxPorCompra)
                               //                                                                : ((esteMomento - _factura.ObjParking.FechaIngreso).TotalMinutes > _factura.ObjParking.MinutosGracia ? 0 : _factura.ObjParking.MinutosGracia)).ToString("HH:mm"));

                            }
                        }
                        else
                        {
                            _factura.ObjParking.FechaIngreso.AddMinutes(totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                                                                                            _factura.ObjParking.MinutosLibreMaxPorCompra
                                                                                            : _factura.ObjParking.MinutosGracia).ToString("HH:mm");

                            parametros.Add(new ParametrosMensajes() { codigo = "[tiempo_maximo]", valor = _factura.ObjParking.FechaIngreso.ToString() });
                            Control.Common.General.GetMensajeToList(295, parametros);


                            //Control.Common.WinForm.ShowMessage("Indíquele al cliente que al momento se encuentra dentro del período de gracia y puede salir con normalidad máximo hasta las " +
                            //_factura.ObjParking.FechaIngreso.AddMinutes(totalFac >= _factura.ObjParking.ValorMinCompraParaMinutosLibre ?
                            //                                                                _factura.ObjParking.MinutosLibreMaxPorCompra
                            //                                                                : _factura.ObjParking.MinutosGracia).ToString("HH:mm"));
                        }
                        _factura.ObjParking.DebeRealizarAvisoTiempoGracia = false;
                    }
                }
            }
        }

        private void CalcularPremioCupon()
        {
            if (_factura.ObjCuponApp != null)
            {

                calcularFactura();
            }
        }

        private bool PermiteAgregarItemPorParqueo(string itemId)
        {
            bool respuesta = true;
            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();

            using (POSEntities db = new POSEntities())
            {
                var codigoBarra = db.pos_itembarra.Where(x => x.ITEMBARCODE == itemId).FirstOrDefault();
                if (codigoBarra != null)
                {
                    itemId = codigoBarra.ITEMID;
                }
            }

            //RecargarParametrosParking();

            parametros = new List<ParametrosMensajes>();
            parametros.Add(new ParametrosMensajes() { codigo = "[itemId]", valor = itemId });

            if (Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(x => x.ToUpper().Equals(itemId)) && !Control.Common.GlobalParameters.Parking_TienePermiso)
            {
                //Control.Common.WinForm.ShowMessage("El item de perdida de ticket '" + itemId + "' no se puede seleccionar porque local no tiene habilitado parqueo");
                Control.Common.General.GetMensajeToList(296, parametros);

                respuesta = false;
            }
            if (Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(x => x.ToUpper().Equals(itemId)) && _factura.ObjParking == null)
            {
                //Control.Common.WinForm.ShowMessage("No puede haber ítem de parqueo '" + itemId + "' antes de realizar el paso de Solicitar el ítem de parqueo");
                Control.Common.General.GetMensajeToList(297, parametros);

                respuesta = false;
            }
            if (Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(x => x.ToUpper().Equals(itemId)) && _factura.Productos.Any(x => Control.Common.GlobalParameters.Parking_ListaItemParqueo.Any(y => y.Equals(x.Id))))
            {
                //Control.Common.WinForm.ShowMessage("No puede agregar ítem de perdida '" + itemId + "' cuando ya hay un ítem de parqueo en la factura");
                Control.Common.General.GetMensajeToList(298, parametros);
                respuesta = false;
            }
            if (Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(x => x.ToUpper().Equals(itemId)) && _factura.Productos.Where(x => Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(y => y.Equals(x.Id))).ToList().Count > 0)
            {
                //Control.Common.WinForm.ShowMessage("No puede agregar mas de un ítem de perdida '" + itemId + "' a la factura");
                Control.Common.General.GetMensajeToList(299, parametros);
                respuesta = false;
            }

            return respuesta;
        }

        private bool SolicitaParqueoEmergente()
        {
            if (Control.Common.GlobalParameters.Parking_TienePermiso && _factura.ObjParking == null)
            {
                if (!_factura.Productos.Any(x => Control.Common.GlobalParameters.Parking_ListaItemPerdida.Any(y => y.Equals(x.Id))))
                {
                    SolicitarParqueo();
                    if (_factura.ObjParking != null) return true;
                }
            }

            return false;
        }

        private void gridItems_RowFormatting(object sender, RowFormattingEventArgs e)
        {
            SetGlobalRowFormatting(sender, e);
        }

        private void gridPagos_RowFormatting(object sender, RowFormattingEventArgs e)
        {
            SetGlobalRowFormatting(sender, e);
        }

        private void SetGlobalRowFormatting(object sender, RowFormattingEventArgs e)
        {
            e.RowElement.DrawFill = true;
            e.RowElement.GradientStyle = Telerik.WinControls.GradientStyles.Solid;
            e.RowElement.BackColor = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.RowElement.BackColor2 = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.RowElement.BackColor3 = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.RowElement.BackColor4 = Control.Common.GlobalParameters.Color_GridViewBackground;
        }

        private void gridPagos_CellFormatting(object sender, CellFormattingEventArgs e)
        {
            SetGlobalCellFormatting(sender, e, ref gridPagos);
        }

        private void SetGlobalCellFormatting(object sender, CellFormattingEventArgs e, ref RadGridView refGridView)
        {
            e.CellElement.DrawFill = true;
            e.CellElement.BackColor = Control.Common.GlobalParameters.Color_GridViewBackground;
            e.CellElement.NumberOfColors = 1;
            if (refGridView.CurrentCell != null)
            {
                refGridView.CurrentCell.DrawFill = true;
                refGridView.CurrentCell.BackColor = Control.Common.GlobalParameters.Color_GridViewSelectedCell;
            }
        }

        private void SetWindowPalette()
        {
            ((FillPrimitive)btnFactura.ButtonElement.GetChildrenByType(typeof(FillPrimitive))[0]).BackColor = Control.Common.GlobalParameters.Color_ButtonPulsed;
            panelPagoSecundario.BackColor = Control.Common.GlobalParameters.Color_PanelBackground;
            panelgrid.BackColor = Control.Common.GlobalParameters.Color_PanelBackground;
            panel10.BackColor = Control.Common.GlobalParameters.Color_PanelBackground;
            panel11.BackColor = Control.Common.GlobalParameters.Color_PanelBackground;
            panel7.BackColor = Control.Common.GlobalParameters.Color_PanelBackground;
            foreach (var c in panel3.Controls)
            {
                if (c is Button)
                {
                    if (((Button)c).Name.Contains("BTN_DIREC"))
                        ((Button)c).BackColor = Control.Common.GlobalParameters.Color_ButtonPulsed;
                }
            }
        }

        private void MainWindow_Shown(object sender, EventArgs e)
        {
            SetWindowPalette();
        }

        private void btnRetencionElect_Click(object sender, EventArgs e)
        {
            var f = new POS.Control.Pagos.FrmRetencionElectronica();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        private void btnTarjetaEmpre_Click(object sender, EventArgs e)
        {
            var f = new POS.Control.CrediEmpre.TarjeEmprePagos();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog();
        }

        public void RecargarParametrosLiquidacion()
        {
            string[] FormasPagoPermitidas;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    //------------------------LIQUIDACION ACTIVO--------------------------------------------------------

                    var EsLiquidacionActivo = db.core_parametro.Any(x => x.identificador.Equals("POS_LIQUIDACION_" + Control.Common.GlobalParameters.EstablecimientoAxCode) && x.valor.Equals("TRUE"));

                    if (EsLiquidacionActivo)
                    {
                        var opciones = db.core_parametro.Where(x => x.identificador.Equals("POS_LIQUIDACION_" + Control.Common.GlobalParameters.EstablecimientoAxCode)).FirstOrDefault().parametro2;
                        if (opciones != null)
                            FormasPagoPermitidas = opciones.Split(';');
                        else
                            FormasPagoPermitidas = null;
                    }
                    else
                        FormasPagoPermitidas = null;

                    if (FormasPagoPermitidas != null)
                    {
                        btnEfectivo.Visible = (FormasPagoPermitidas.Contains("E"));
                        btnTCredito.Visible = (FormasPagoPermitidas.Contains("TC"));
                        btnCheque.Visible = (FormasPagoPermitidas.Contains("CH"));
                        btnNC.Visible = (FormasPagoPermitidas.Contains("NC"));
                        btnDsctoEsp.Visible = (FormasPagoPermitidas.Contains("DE"));
                        btnPagoGiftCard.Visible = (FormasPagoPermitidas.Contains("GC"));
                        btnCreditoInterno.Visible = (FormasPagoPermitidas.Contains("TP"));
                        btnRetencion.Visible = (FormasPagoPermitidas.Contains("RE"));
                        btnDsctoPaviPlan.Visible = (FormasPagoPermitidas.Contains("DP"));
                    }
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "RecargarParametrosTarjetaEmpresarial", "El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. A continuacion las excepciones encontradas " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                //System.Windows.Forms.MessageBox.Show("El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS");
                Control.Common.General.GetMensajeToList(153);

                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS ", " POS - Parametros Generales");
                //MsgBox msgBox = new MsgBox("info", " El aplicativo no pudo tomar uno o varios parámetros básicos para su normal funcionamiento, por lo que, el aplicativo se cerrará para prevenir inconsistencias en la facturación. Reabrir POS", "POS");
                //DialogResult dialogResult = msgBox.ShowDialog();

                POS.Control.Common.GlobalParameters.MustCloseApplication = true;
                Application.Exit();
            }
        }

        public void RecargarImagenes()
        {
            string ruta = Properties.Settings.Default.PATH_IMG;
            try
            {
                if (File.Exists(@Control.Common.GlobalParameters.WallpaperLocal))
                {
                    FileInfo x = new FileInfo(@Control.Common.GlobalParameters.WallpaperLocal);
                    if (!Directory.Exists(ruta))
                        Directory.CreateDirectory(ruta);

                    if (File.Exists(ruta + @"\wallpaper.jpg"))
                        File.Delete(ruta + @"\wallpaper.jpg");

                    x.CopyTo(ruta + @"\wallpaper.jpg");
                    Control.Common.GlobalParameters.WallpaperLocal = ruta + @"\wallpaper.jpg";
                }
                else
                {
                    if (File.Exists(ruta + @"\wallpaper.jpg"))
                    {
                        Control.Common.GlobalParameters.WallpaperLocal = ruta + @"\wallpaper.jpg";
                    }
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(ruta + @"\wallpaper.jpg"))
                {
                    Control.Common.GlobalParameters.WallpaperLocal = ruta + @"\wallpaper.jpg";
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "RecargarImagenes", " wallpaper a continuacion la excepcion encontrada " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }

            
        }

        private void ValidarMonedero(string identificacion)
        {
            decimal saldoPuntos = 0;          
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    if (db.TblPuntosCab.Any(x => x.AccountNum == identificacion && x.Estado == 1))
                    {
                        var puntoscab = db.TblPuntosCab.Where(x => x.AccountNum == identificacion && x.Estado == 1).ToList();                        
                        if (puntoscab != null && puntoscab.Count > 0)
                        {
                            saldoPuntos = puntoscab.Sum(x => x.Saldo);
                            if (saldoPuntos > 0)
                            {
                                _factura.SaldoPuntos = saldoPuntos * Control.WalletPoints.ClsPoints.FactorCanje;
                                btnMonedero.Visible = (_factura.EsUsoAppMovil && POS.Control.Common.GlobalParameters.MonederoConsumoActivo);                                
                            }
                            else
                            {
                                btnMonedero.Visible = false;
                            }
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonedero", "CI:" + identificacion + ", saldo de puntos TblPuntosCab:" + saldoPuntos.ToString());//stefany                           
                        }
                        else
                        {
                            btnMonedero.Visible = false;
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidarMonedero", "A continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }


        }
        /// <summary>
        /// Obtiene el saldo del monedero del cliente por campaña.
        /// </summary>
        /// <param name="identificacion"></param>
        /// <param name="idCampania"></param>
        private void ValidarMonederoCampania(string identificacion)
        {
            int idCampania = 0;
            decimal saldoPuntos = 0.0M;
            Boolean _tieneCampania = true;

            using (POSEntities db = new POSEntities())
            {

                _tieneCampania = false;
                if (db.core_parametro.Any(x => x.identificador == "CAMPANIA_MONEDERO"))
                {
                    string valor = db.core_parametro.Where(x => x.identificador == "CAMPANIA_MONEDERO").FirstOrDefault().valor;
                    if (!string.IsNullOrEmpty(valor))
                    {

                        _tieneCampania = false;
                        if (valor != "0")
                        {
                            idCampania = Convert.ToInt32(valor);
                            _tieneCampania = true;
                        }
                    }
                }
                

                if (_tieneCampania)
                {
                    
                    if (identificacion == "" || identificacion == "9999999999999")
                    {
                        btnMonedero.Visible = false;
                        btnCuponApp.Visible = false;
                    }
                    else
                    {
                        string cadenaCon = "";
                      
                        if (Control.Common.GlobalParameters.ConServerPuntos != "")
                        {
                            cadenaCon = Control.Common.GlobalParameters.ConServerPuntos;
                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "ValidarMonederoCampania", "No hay parametro 'CON_SERVER_PUNTOS' para este local o esta vacío ");
                            Control.Common.General.GetMensajeToList(159);

                            //System.Windows.Forms.MessageBox.Show(this, "La Billetera Electrónica esta fuera de línea y no se puede utilizar (No hay parametro 'CON_SERVER_PUNTOS' para este local), intente más tarde", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, " La Billetera Electrónica esta fuera de línea y no se puede utilizar (No hay parametro 'CON_SERVER_PUNTOS' para este local), intente más tarde", "POS - Billetera Electrónica");
                            // MsgBox msgBox = new MsgBox("info", " La Billetera Electrónica esta fuera de línea y no se puede utilizar (No hay parametro 'CON_SERVER_PUNTOS' para este local), intente más tarde", "POS - Billetera Electrónica ");
                            // DialogResult dialogResult = msgBox.ShowDialog();
                        }

                        if (cadenaCon !="")
                        {
                            SqlConnection conn = new SqlConnection(cadenaCon);
                            try
                            {
                                SqlParameter paramResult = new SqlParameter("@respuesta", SqlDbType.VarChar, -1);
                                paramResult.IsNullable = true;
                                paramResult.Direction = System.Data.ParameterDirection.Output;
                                var addParameters = new List<SqlParameter>
                                 {
                                    new SqlParameter("@AccountNum", identificacion),
                                    paramResult
                                 };

                                SqlCommand select = new SqlCommand("Exec PtsCliente.spValidarHistoricoPuntosGen @AccountNum, @respuesta out", conn);
                                select.Parameters.AddRange(addParameters.ToArray());
                                conn.Open();
                                select.ExecuteNonQuery();
                                conn.Close();
                                string saldoPuntos_ = (string)paramResult.Value;
                                decimal.TryParse(paramResult.Value.ToString(), out saldoPuntos);

                                if (saldoPuntos > 0.0M)
                                {
                                    _factura.SaldoPuntos = Math.Round(saldoPuntos * Control.WalletPoints.ClsPoints.FactorCanje, 2);
                                    btnMonedero.Visible = (_factura.EsUsoAppMovil && POS.Control.Common.GlobalParameters.MonederoConsumoActivo);                                   
                                }
                                else
                                {
                                    btnMonedero.Visible = false;
                                }

                            }
                            catch (Exception ex)
                            {
                                conn.Close();
                                _factura.EsUsoAppMovil = false;

                                Control.Common.General.GetMensajeToList(160);


                                //msgBoxCtrl = new MsgBoxCtrl();
                                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "La Billetera Electrónica esta fuera de línea y no se puede utilizar, intente más tarde", "POS - Billetera Electrónica");

                                //MsgBox msgBox = new MsgBox("info", " La Billetera Electrónica esta fuera de línea y no se puede utilizar, intente más tarde", "POS - Billetera Electrónica");
                                //DialogResult dialogResult = msgBox.ShowDialog();
                                //System.Windows.Forms.MessageBox.Show(this, "La Billetera Electrónica esta fuera de línea y no se puede utilizar, intente más tarde", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);

                                btnMonedero.Visible = false;
                                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "ValidarMonederoCampania", "Billetera Electrónica esta fuera de línea, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                            }
                        }

                                               
                    }
                }
                else
                {
                    ValidarMonedero(identificacion);
                }

            }
        }
       
        private void btnMonedero_Click(object sender, EventArgs e)
        {
            try
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnMonedero_Click", "El cajero ha pulsado el botón Billetera Electrónica del cliente: " + this._factura.ClienteIdentificacion + "  " + this._factura.Cliente_nombre);
                                
                if (_factura.Pagos.Where(x => x.Descripcion == "DINE ELECT").Sum(x => x.Valor) > 0)
                {
                    Control.Common.General.GetMensajeToList(160);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Dinero Electrónico ya fué aplicado a esta compra, para aplicar debe borrar el pago actual", "POS - Dinero Electrónica");
                    //MsgBox msgBox = new MsgBox("info", " Dinero Electrónico ya fué aplicado a esta compra, para aplicar debe borrar el pago actual", "POS - Dinero Electrónica");
                    //DialogResult dialogResult = msgBox.ShowDialog();
                    // System.Windows.Forms.MessageBox.Show(this, "Dinero Electrónico ya fué aplicado a esta compra, para aplicar debe borrar el pago actual", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                QuitarDescuentoPromocionTarjetaBines();

                //if (revisarCambioEnTotal("DINE ELECT", "DINE ELECT"))
                //{
                //    return;
                //}

                string formaPago = "DINE ELECT";
                if (_factura.EsEmpleadoLiris || _factura.EsTarjetaCreditoInterno || _factura.EsTarjetaCreditoInternoAdicional)
                {
                    formaPago = "TAR PORTAL";
                }

                if (revisarCambioEnTotal(formaPago, formaPago))
                {
                    return;
                }


                var f = new POS.Control.Pagos.BasePagos(Control.Pagos.BasePagos.PagoTipo.DineroElectronico, ref _factura, getValorPago());
                f.ShowDialog();
                txtPagoValor.Clear();
                calcularFactura();
                agregaFormasPagoTmpFile();               
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnMonedero_Click", "No fue posible aplicar Billetera Electrónica, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }

            // Logica para Promocion Compra Gratis
            /*try
            {
                POSEntities db = new POSEntities();

                if (!POS.Control.Common.GlobalParameters.CompraGratis)
                {
                    MessageBox.Show(this, "Opción Billetera Electrónica no hablitada para este local", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "btnMonedero_Click", "El cajero ha pulsado el botón Billetera Electrónica del cliente: " + this._factura.ClienteIdentificacion + "  " + this._factura.Cliente_nombre);

                core_TarjetaDescuento tarjeta = db.core_TarjetaDescuento.Where(x => x.codigo == ClienteCompraGratis).FirstOrDefault();

                if (tarjeta != null)
                {
                    if (tarjeta.saldo <= 0)
                    {
                        MessageBox.Show(this, "El saldo $ " + tarjeta.saldo + " de la Billetera Electrónica es insuficiente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (this._factura.Productos.Count > 0)
                    {
                        agregarDescuentoCompraGratis(db);
                    }
                    else
                    {
                        MessageBox.Show(this, "Para usar la Billetera Electrónica, la factura debe tener productos", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(this, "La Billetera Electrónica no es válida, por favor pase la tarjeta nuevamente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnMonedero_Click", "No fue posible aplicar Billetera Electrónica, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
            }*/
        }

        // Carga la temporal.  JM 11-09-2019 
        public void CargaFacturaTmpFile()
        {
            string tipoCab = string.Empty, tipoDet = string.Empty, tipoPag = string.Empty;
            tipoCab = "Cab.txt";
            tipoDet = "Det.txt";
            tipoPag = "Pag.txt";

            if (NoGrabaTMP == "FALSE" && !Es2X_CONSULTA_POS)
            {
                // Cargar la cabecera temporal
                try
                {
                    string txtfilecab = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Cab.txt";
                    //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                    txtfilecab = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoCab);
                    bool EsEmpleadoLiris = false;
                    
                    if (File.Exists(txtfilecab))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturaTmpFile", "Se ha encontrado datos de factura previa en la base temporal, se cargarán los datos a la pantalla");
                        using (StreamReader file = new StreamReader(txtfilecab))
                        {
                            string ln;
                            // cargar cabecera  
                            while ((ln = file.ReadLine()) != null)
                            {
                                var lineCab = ln.Split('|');
                                codigocliente = lineCab[0];
                                EsEmpleadoLiris = lineCab[1] == "True"? true: false;

                                //codigocliente = ln;
                                //usotarjetadscto = false;
                            }
                            file.Close();

                            cambiarCliente(codigocliente);
                            txtCedula.Text = codigocliente;
                            _factura.EsEmpleadoLiris = EsEmpleadoLiris;

                        }
                    }
                }
                catch (Exception ex)
                {


                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.StackTrace });
                    

                    string errorMsj = "No fue posible cargar los datos de Cabecera de la factura temporal, a continuacion las excepciones encontradas - " 
                        + Control.Common.ExceptionHandler.GetExceptionMessages(ex) + "StackTrace: " + ex.StackTrace;

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmpFile", errorMsj);
                    Control.Common.General.GetMensajeToList(300, parametros);

                    //Manejo de error 
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);
                    // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, ex.Message, "POS");
                }

                // Cargar los Productos temporales
                try
                {

                    string txtfiledet = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Det.txt";
                    //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                    txtfiledet = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoDet);

                    if (File.Exists(txtfiledet))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturaTmpFile", "Se han encontrado datos de productos de una factura previa en la base temporal, se cargarán los datos a la pantalla");

                        using (StreamReader file = new StreamReader(txtfiledet))
                        {
                            string ln;
                            // cargar cabecera y detalle
                            while ((ln = file.ReadLine()) != null)
                            {
                                try
                                {

                                    //sw.WriteLine(prod.Id + "||" + prod.Cantidad + "||" + prod.Unidades + "||" + prod.Subtotal + "||" + prod.Descuento + "||" + prod.Iva + "||" + prod.Total + "||" + prod.Unidad + "||" + prod.Costo + "||" + prod.Pvp);
                                    var item = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                    Producto itm = new Producto();

                                    if(cliente_actual == null) { cliente_actual = new pos_customer(); }

                                    itm.getProducto(item[0] /*item_id*/, _factura, cliente_actual);
                                    itm.CantidadINEC = decimal.Parse(item[1]); //cantidad,
                                    itm.Cantidad = decimal.Parse(item[1]); //cantidad,
                                    itm.Unidades = int.Parse(item[2]); //unidades,
                                    itm.Subtotal = decimal.Parse(item[3]); //subtotal,
                                    itm.Descuento = decimal.Parse(item[4]); //descuento,
                                    itm.Iva = decimal.Parse(item[5]); //iva,
                                    itm.Total = decimal.Parse(item[6]); //total,
                                    itm.Unidad = item[7]; //unidad,
                                    itm.Costo = decimal.Parse(item[8]);  //costo,
                                    itm.Pvp = decimal.Parse(item[9]);  //precio

                                    existeEnListaDescuento(itm.Id); //Verifica si esta en lista de descuentos AX
                                    if (POS.Control.Common.Promo.PuedeConjuntoClienteRecibirDescGestor(cliente_actual.CUSTGROUP))//cliente_actual.CUSTGROUP != "07" && cliente_actual.CUSTGROUP != "09" /*&& cliente_actual.CUSTGROUP != "EM"*/ && cliente_actual.CUSTGROUP != "CE")
                                    {
                                        itm.actualizarDescuentoPromocionAX(_factura.PromocionesActuales, (cliente_actual == null ? string.Empty : cliente_actual.ACCOUNTNUM), _factura);
                                    }

                                    //itm.update();
                                    _factura.Productos.Add(itm);
                                }
                                catch (Exception)
                                {

                                    throw;
                                }


                            }
                            file.Close();
                        }

                        PromosPrecioPorCombinacion();
                        PromosDsctoPorSuplemento();

                        promoiva(_factura, null);
                        calcularFactura();

                        RefrescarGridItems();

                        if (Control.Common.GlobalParameters.UserObj == null)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturaTmpFile", "Se detectaron items temporales pero no se pudo levantar la pantalla para la aceptacion del cajero porque el objeto de usuario estaba nulo");
                        }
                        else
                        {
                            if (_factura.Productos.Count > 0 && !Control.Common.GlobalParameters.UserObj.isSuperUser)
                            {
                                tieneProductosTmp = true;
                                Control.Main.TempInvoiceAlert frm = new Control.Main.TempInvoiceAlert();
                                frm.ShowDialog();
                            }

                            if (_factura.Productos.Count > 0)
                            {
                                tieneProductosTmp = true;
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmpFile", "No fue posible cargar los datos de Detalle de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    // MessageBox.Show(this, comando);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, ex.Message, "POS");
                    //Control.Common.General.GetMensaje("POS", "No fue posible cargar los datos de Detalle de la factura temporal. Error: " + ex.Message, "I");

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.StackTrace });
                    Control.Common.General.GetMensajeToList(302, parametros);

                }

                //Cargar las formas de pago temporales.     JM   20-11-2020
                try
                {

                    string txtfilepag = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Pag.txt";
                    //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                    txtfilepag = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoPag);

                    if (File.Exists(txtfilepag))
                    {
                        string formaPagoDesc =string.Empty;
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "MainWindow", "CargaFacturaTmpFile", "Se han encontrado datos de formas de pago de una factura previa en la base temporal, se cargarán los datos a la pantalla");

                        using (StreamReader filepag = new StreamReader(txtfilepag))
                        {
                            string ln;
                            // cargar cabecera y detalle                       
                            while ((ln = filepag.ReadLine()) != null)
                            {
                                var formaPago = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                if (!formaPago[1].Contains("@") && !formaPago[0].Contains("@"))
                                { formaPagoDesc=formaPago[1];}
                                
                                switch (formaPagoDesc)
                                {
                                    case "T. CREDITO":
                                        var cred = new PagoTarjetaCredito() ;
                                        if (!formaPago[1].Contains("@") && !formaPago[0].Contains("@"))
                                        {
                                            while ((ln = filepag.ReadLine()) != null)
                                            {
                                                var fpTarjetaCredito = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                                cred.Banco = fpTarjetaCredito[0];
                                                cred.BinDescripcion = fpTarjetaCredito[1];
                                                cred.Codigo = fpTarjetaCredito[2];
                                                cred.Marca = fpTarjetaCredito[3];
                                                cred.Nombre = fpTarjetaCredito[4];
                                                cred.TipoPos = fpTarjetaCredito[5];
                                                cred.Valor = decimal.Parse(fpTarjetaCredito[6]);
                                                _factura.AgregarPagoTarjetaCredito(cred.Valor, cred.Banco, cred.Nombre, cred.Marca, cred.TipoPos);
                                                formaPagoDesc = "T. CREDITO";
                                               break;
                                            }
                                        }
                                        else
                                        {
                                            var fpTarjetaCredito = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                            cred.Banco = fpTarjetaCredito[0];
                                            cred.BinDescripcion = fpTarjetaCredito[1];
                                            cred.Codigo = fpTarjetaCredito[2];
                                            cred.Marca = fpTarjetaCredito[3];
                                            cred.Nombre = fpTarjetaCredito[4];
                                            cred.TipoPos = fpTarjetaCredito[5];
                                            cred.Valor = decimal.Parse(fpTarjetaCredito[6]);
                                            _factura.AgregarPagoTarjetaCredito(cred.Valor, cred.Banco, cred.Nombre, cred.Marca, cred.TipoPos);
                                            formaPagoDesc = "T. CREDITO";
                                            break;
                                        }
                                           
                                        break;
                                    case "CHEQUE":
                                        var cheq = new PagoCheque();
                                        if (!formaPago[1].Contains("@") && !formaPago[0].Contains("@"))
                                        {
                                            while ((ln = filepag.ReadLine()) != null)
                                            {
                                                var fpCheque = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                                cheq.Banco = fpCheque[0];
                                                cheq.Codigo = fpCheque[1];
                                                cheq.Cuenta = fpCheque[2];
                                                cheq.Numero = fpCheque[3];
                                                cheq.Valor = decimal.Parse(fpCheque[4]);
                                                _factura.AgregarPagoCheque(cheq.Valor, cheq.Banco, cheq.Numero, cheq.Cuenta);
                                                formaPagoDesc = "CHEQUE";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            var fpCheque = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                            cheq.Banco = fpCheque[0];
                                            cheq.Codigo = fpCheque[1];
                                            cheq.Cuenta = fpCheque[2];
                                            cheq.Numero = fpCheque[3];
                                            cheq.Valor = decimal.Parse(fpCheque[4]);
                                            _factura.AgregarPagoCheque(cheq.Valor, cheq.Banco, cheq.Numero, cheq.Cuenta);
                                            formaPagoDesc = "CHEQUE";
                                            break;
                                        }
                                        break;
                                    case "GIFT CARD":
                                        var gift = new PagoGiftCard();
                                        if (!formaPago[1].Contains("@") && !formaPago[0].Contains("@"))
                                        {
                                            while ((ln = filepag.ReadLine()) != null)
                                            {
                                                var fpGift = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                                gift.Codigo = fpGift[0];
                                                gift.EstaAsociadaGrupoCliente = bool.Parse(fpGift[1]);
                                                gift.IdentificacionGrupoCliente = fpGift[2];
                                                gift.NombreGrupoCliente = fpGift[3];
                                                gift.Saldo = decimal.Parse(fpGift[4]);
                                                gift.Valor = decimal.Parse(fpGift[5]);
                                                _factura.AgregarPagoTarjetaRegalo(gift.Valor, gift.Codigo, gift.Saldo, gift.EstaAsociadaGrupoCliente, gift.IdentificacionGrupoCliente, gift.NombreGrupoCliente);
                                                formaPagoDesc = "GIFT CARD";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            var fpGift = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                            gift.Codigo = fpGift[0];
                                            gift.EstaAsociadaGrupoCliente = bool.Parse(fpGift[1]);
                                            gift.IdentificacionGrupoCliente = fpGift[2];
                                            gift.NombreGrupoCliente = fpGift[3];
                                            gift.Saldo = decimal.Parse(fpGift[4]);
                                            gift.Valor = decimal.Parse(fpGift[5]);
                                            _factura.AgregarPagoTarjetaRegalo(gift.Valor, gift.Codigo, gift.Saldo, gift.EstaAsociadaGrupoCliente, gift.IdentificacionGrupoCliente, gift.NombreGrupoCliente);
                                            formaPagoDesc = "GIFT CARD";
                                            break;
                                        }
                                        break;
                                    case "GIFT CARDV":
                                        var giftv = new PagoGiftCard();
                                        if (!formaPago[1].Contains("@") && !formaPago[0].Contains("@"))
                                        {
                                            while ((ln = filepag.ReadLine()) != null)
                                            {
                                                var fpGift = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                                giftv.Codigo = fpGift[0];
                                                giftv.EstaAsociadaGrupoCliente = bool.Parse(fpGift[1]);
                                                giftv.IdentificacionGrupoCliente = fpGift[2];
                                                giftv.NombreGrupoCliente = fpGift[3];
                                                giftv.Saldo = decimal.Parse(fpGift[4]);
                                                giftv.Valor = decimal.Parse(fpGift[5]);
                                                _factura.AgregarPagoTarjetaRegalo(giftv.Valor, giftv.Codigo, giftv.Saldo, giftv.EstaAsociadaGrupoCliente, giftv.IdentificacionGrupoCliente, giftv.NombreGrupoCliente);
                                                formaPagoDesc = "GIFT CARDV";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            var fpGiftv = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                            giftv.Codigo = fpGiftv[0];
                                            giftv.EstaAsociadaGrupoCliente = bool.Parse(fpGiftv[1]);
                                            giftv.IdentificacionGrupoCliente = fpGiftv[2];
                                            giftv.NombreGrupoCliente = fpGiftv[3];
                                            giftv.Saldo = decimal.Parse(fpGiftv[4]);
                                            giftv.Valor = decimal.Parse(fpGiftv[5]);
                                            _factura.AgregarPagoTarjetaRegalo(giftv.Valor, giftv.Codigo, giftv.Saldo, giftv.EstaAsociadaGrupoCliente, giftv.IdentificacionGrupoCliente, giftv.NombreGrupoCliente);
                                            formaPagoDesc = "GIFT CARDV";
                                            break;
                                        }
                                        break;
                                    case "TAR PORTAL":
                                        var inte = new PagoTarjetaInterna();
                                        if (!formaPago[1].Contains("@") && !formaPago[0].Contains("@"))
                                        {
                                            while ((ln = filepag.ReadLine()) != null)
                                            {
                                                var fpGift = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                                inte.Codigo = fpGift[0];
                                                inte.Titular = fpGift[1];
                                                inte.Valor = decimal.Parse(fpGift[2]);
                                                _factura.AgregarPagoTarjetaInterna(inte.Valor, inte.Codigo, inte.Titular);
                                                formaPagoDesc = "TAR PORTAL";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            var fpGift = ln.Split(new string[] { "||" }, StringSplitOptions.None);
                                            inte.Codigo = fpGift[0];
                                            inte.Titular = fpGift[1];
                                            inte.Valor = decimal.Parse(fpGift[2]);
                                            _factura.AgregarPagoTarjetaInterna(inte.Valor, inte.Codigo, inte.Titular);
                                            formaPagoDesc = "TAR PORTAL";
                                            break;
                                        }
                                        break;
                                    default:
                                        break;
                                } 
                                 
                            }
                            filepag.Close();
                        } 
                         
                        calcularFactura();
                         
                    }
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "CargaFacturaTmpFile", "No fue posible cargar los datos de Formas de Pago de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    // System.Windows.Forms.MessageBox.Show(this, ex.Message);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, ex.Message, "POS");

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.Message });
                    Control.Common.General.GetMensajeToList(303, parametros);


                    //Control.Common.General.GetMensaje("POS", "No fue posible carar los datos de Formas de Pago de la factura temporal. Error: " + ex.Message, "I");
                }


                //string itendifacionCompleta = txtCedula.Text;
                //validaClienteSp(itendifacionCompleta);

            }
        }


        static void UnlockFile(string filePath)
        {
            try
            {
                // Ejecuta Handle.exe para encontrar el proceso que usa el archivo
                Process process = new Process();

                process.StartInfo.FileName = string.Concat(POS.Control.Common.GlobalParameters.DBIdCaja, @"Handle\Handle.exe");
                // process.StartInfo.FileName = @"C:\Tools\Handle.exe";
                process.StartInfo.Arguments = $"\"{filePath}\" /accepteula";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                // Buscar el proceso que bloquea el archivo
                var lines = output.Split('\n').Where(l => l.Contains("pid:")).ToList();
                foreach (var line in lines)
                {
                    string pid = new string(line.Where(char.IsDigit).ToArray());
                    if (int.TryParse(pid, out int processId))
                    {
                        try
                        {
                            Process.GetProcessById(processId).Kill();
                            Console.WriteLine($"Proceso {processId} cerrado para liberar el archivo.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al cerrar el proceso {processId}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al desbloquear el archivo: {ex.Message}");
            }
        }


        //Agregar el cliente a la temporal. JM 11-09-2019
        public void insertaCabeceraFile()
        {
            string textFile = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Cab.txt";
            string tipoCab = string.Empty;
            tipoCab = "Cab.txt";
            try
            {
                //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                textFile = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoCab);

                if (string.IsNullOrEmpty(codigocliente))
                    codigocliente = txtCedula.Text;

                if (File.Exists(textFile))
                {
                    File.Delete(textFile);
                }

                using (StreamWriter sw = File.CreateText(textFile))
                {
                    string line = string.Empty;
                    line = string.Concat(line, codigocliente);
                    line = string.Concat(line, "|", _factura.EsEmpleadoLiris);

                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();
                }
         
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "insertaCabeceraFile", "No fue posible guardar los datos de cabecera de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                try
                {
                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "No fue posible guardar los datos de cabecera de la factura temporal",
                        String.Format("El POS del siguiente punto de emision no pudo guardar los datos de cabecera de la factura temporal.  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    POS.Control.Common.GlobalParameters.Establecimiento,
                                    POS.Control.Common.GlobalParameters.PuntoEmision,
                                    POS.Control.Common.GlobalParameters.IpMaquina,
                                    POS.Control.Common.GlobalParameters.UsuarioNombre,
                                    POS.Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex)),
                        false,
                        String.Empty);

                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaProductosTmpFile", "No se pudo enviar notificacion del problema al guardar los datos de cabecera de la factura temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                    }
                }
                catch (Exception exMail)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaProductosTmpFile", "Excepcion grave al llamar a la clase de envio de email, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exMail));
                }
            }
        }


        //Agregar los items pistoleados a la temporal.  JM 11-09-2019
        public void agregaProductosTmpFile()
        {
            string tipoDet = string.Empty;
            tipoDet = "Det.txt";
            string textFile = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Det.txt";

            try
            {
                //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                textFile = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoDet);
                if (File.Exists(textFile))
                {
                    File.Delete(textFile);
                }

                var buffer = new StringBuilder();
                (from c in _factura.Productos
                 select c).ToList().ForEach(item => buffer.AppendLine(String.Format("{0}||{1}||{2}||{3}||{4}||{5}||{6}||{7}||{8}||{9}", item.Id, item.Cantidad, item.Unidades, item.Subtotal, item.Descuento, item.Iva, item.Total, item.Unidad, item.Costo, item.Pvp)));
                File.WriteAllText(textFile, buffer.ToString());

                // try
                //  {
                //si no tiene productos, entonces asigna a false variable para itemstemporales
                if (_factura.Productos.Count == 0)
                {
                    tieneProductosTmp = false;
                }
                ////  }
                //  catch (Exception)
                //  {

                //   }

                //using (StreamWriter sw = File.CreateText(textFile))
                //{
                //    foreach (Producto prod in _factura.Productos)
                //    {
                //        sw.WriteLine(prod.Id + "||" + prod.Cantidad + "||" + prod.Unidades + "||" + prod.Subtotal + "||" + prod.Descuento + "||" + prod.Iva + "||" + prod.Total + "||" + prod.Unidad + "||" + prod.Costo + "||" + prod.Pvp);
                //    }
                //    sw.Flush();
                //    sw.Close();
                //}
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaProductosTmpFile", "No fue posible guardar los datos de los productos de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                try
                {
                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "No fue posible guardar los datos de los productos de la factura temporal",
                        String.Format("El POS del siguiente punto de emision no pudo guardar los datos de los productos de la factura temporal.  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    POS.Control.Common.GlobalParameters.Establecimiento,
                                    POS.Control.Common.GlobalParameters.PuntoEmision,
                                    POS.Control.Common.GlobalParameters.IpMaquina,
                                    POS.Control.Common.GlobalParameters.UsuarioNombre,
                                    POS.Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex)),
                        false,
                        String.Empty);

                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaProductosTmpFile", "No se pudo enviar notificacion del problema al guardar los datos de los productos de la factura temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                    }
                }
                catch (Exception exMail)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaProductosTmpFile", "Excepcion grave al llamar a la clase de envio de email, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exMail));
                }
            }
        }

        //Agregar las formas de pagos a la temporal.  JM 21-08-2020
        public void agregaFormasPagoTmpFile()
        {
            string tipoDet = string.Empty;
            tipoDet = "Pag.txt";
            string textFile = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Pag.txt";

            try
            {
                //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                textFile = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoDet);
                if (File.Exists(textFile))
                {
                    File.Delete(textFile);
                }

                var buffer = new StringBuilder();
                /*(from c in _factura.Pagos
                 select c).ToList().ForEach(pago =>
                 {
                    buffer.AppendLine(String.Format("{0}||{1}||{2}", pago.Cliente, pago.Descripcion, pago.Valor));
                    (from d in pago.Pagos
                    select d) 
                    .AsEnumerable()
                    .ToList().ForEach(pag => buffer.AppendLine(String.Format("@{0}||{1}", pag.Codigo, pag.Valor)));
                 } );*/

                /*if (t.credo = objetopago.tipo)
                    var respu = (clstiptarjeto)objeto;
                  */

                foreach (var pcab in _factura.Pagos)
                {
                    buffer.AppendLine(String.Format("{0}||{1}||{2}", pcab.Cliente, pcab.Descripcion, pcab.Valor));
                 
                    foreach (var pdet in pcab.Pagos)
                    {
                        switch (pdet.GetType().Name)
                        {
                            case "PagoTarjetaCredito": 
                                var cred = (PagoTarjetaCredito)pdet;                                
                                buffer.AppendLine(String.Format("@{0}||{1}||{2}||{3}||{4}||{5}||{6}", cred.Banco,cred.BinDescripcion, cred.Codigo, cred.Marca,cred.Nombre,cred.TipoPos, cred.Valor));
                                break;
                            case "PagoCheque": 
                                var cheq = (PagoCheque)pdet;
                                buffer.AppendLine(String.Format("@{0}||{1}||{2}||{3}||{4}", cheq.Banco, cheq.Codigo, cheq.Cuenta, cheq.Numero, cheq.Valor));
                                break;
                            case "PagoGiftCard":
                                var gift = (PagoGiftCard)pdet;
                                buffer.AppendLine(String.Format("@{0}||{1}||{2}||{3}||{4}||{5}", gift.Codigo, gift.EstaAsociadaGrupoCliente, gift.IdentificacionGrupoCliente, gift.NombreGrupoCliente, gift.Saldo, gift.Valor));
                                break;
                            case "PagoTarjetaInterna":
                                var inte = (PagoTarjetaInterna)pdet;
                                buffer.AppendLine(String.Format("@{0}||{1}||{2}", inte.Codigo, inte.Titular, inte.Valor));
                                break;
                            default:
                                break;
                        }
                        
                    }
                }
                File.WriteAllText(textFile, buffer.ToString());                                           
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaFormasPagoTmpFile", "No fue posible guardar los datos de formas de pago de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                try
                {
                    var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "No fue posible guardar los datos de formas de pago de la factura temporal",
                        String.Format("El POS del siguiente punto de emision no pudo guardar los datos de formas de pago de la factura temporal.  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5}",
                                    POS.Control.Common.GlobalParameters.Establecimiento,
                                    POS.Control.Common.GlobalParameters.PuntoEmision,
                                    POS.Control.Common.GlobalParameters.IpMaquina,
                                    POS.Control.Common.GlobalParameters.UsuarioNombre,
                                    POS.Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex)),
                        false,
                        String.Empty);

                    if (!xmlRespuesta.DocumentElement.GetAttribute("CodError").Equals("0"))
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaFormasPagoTmpFile", "No se pudo enviar notificacion del problema al guardar los datos de formas de pago de la factura temporal, a continuacion las excepciones encontradas - " + xmlRespuesta.DocumentElement.GetAttribute("MsgError"));
                    }
                }
                catch (Exception exMail)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "agregaFormasPagoTmpFile", "Excepcion grave al llamar a la clase de envio de email, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(exMail));
                }
            }
        }

        private void EliminaFacturaTmpFile()
        {
            string tipoCab = string.Empty, tipoDet = string.Empty, tipoPag = string.Empty;
            if (NoGrabaTMP == "FALSE" && !Es2X_CONSULTA_POS)
            {
                try
                {
                    tipoCab = "Cab.txt";
                    tipoDet = "Det.txt";
                    tipoPag = "Pag.txt";
                    //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                    string textFilecab = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + tipoCab;
                    textFilecab = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoCab);
                    if (File.Exists(textFilecab))
                    {
                        File.Delete(textFilecab);
                    }
                    //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                    string textFiledet = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Det.txt";
                    textFiledet = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoDet);
                    if (File.Exists(textFiledet))
                    {
                        File.Delete(textFiledet);
                    }
                    tieneProductosTmp = false;
                    //Verifica conectividad al recurso compartido, si no existe conectividad, entonces que tome los parametros del recurso Local.
                    string textFilePag = POS.Control.Common.GlobalParameters.DBIdCaja + Program.ID_Caja_POS + "Pag.txt";
                    textFilePag = ConectividadSharedTmpFile(POS.Control.Common.GlobalParameters.DBIdCaja, Program.ID_Caja_POS, tipoPag);
                    if (File.Exists(textFilePag))
                    {
                        File.Delete(textFilePag);
                    }
                   
                }
                catch (Exception ex)
                {
                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "EliminaFacturaTmpFile", "No fue posible eliminar los datos de la factura temporal, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);
                    //System.Windows.Forms.MessageBox.Show(this, ex.Message);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, ex.Message, "POS");
                    //Control.Common.General.GetMensaje("POS", "No fue posible eliminar los datos de la factura temporal. Error: " + ex.Message, "I");

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[error_exception]", valor = ex.Message });
                    Control.Common.General.GetMensajeToList(304, parametros);


                }
            }
        }

        private string ConectividadSharedTmpFile(string shared, string idPOS, string tipo)
        {
            bool recursoCompartidoDisponible = true;
            string txtFile = string.Empty;
            try
            {
                //bool recursoCompartidoDisponible = QuickBestGuessAboutAccessibilityOfNetworkPath(shared);                
                recursoCompartidoDisponible = Directory.Exists(@shared);
                if (recursoCompartidoDisponible)
                {
                    return shared + idPOS + tipo;
                }
                else
                {
                    POS.Control.Common.GlobalParameters.DBIdCaja = POS.Control.Common.GlobalParameters.DBIdCajaLocal;
                    return POS.Control.Common.GlobalParameters.DBIdCajaLocal + idPOS + tipo;
                }

            }
            catch (Exception)
            {
                //Si existe error desconocido o no tratado, entonces que obtenga la ruta Local.
                POS.Control.Common.GlobalParameters.DBIdCaja = POS.Control.Common.GlobalParameters.DBIdCajaLocal;
                return POS.Control.Common.GlobalParameters.DBIdCajaLocal + idPOS + tipo;
            }
        }

        private bool QuickBestGuessAboutAccessibilityOfNetworkPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            string pathRoot = Path.GetPathRoot(path);
            if (string.IsNullOrEmpty(pathRoot)) return false;
            ProcessStartInfo pinfo = new ProcessStartInfo("net", "use");
            pinfo.CreateNoWindow = true;
            pinfo.RedirectStandardOutput = true;
            pinfo.UseShellExecute = false;
            string output;
            using (Process p = Process.Start(pinfo))
            {
                output = p.StandardOutput.ReadToEnd();
            }
            foreach (string line in output.Split('\n'))
            {
                if (line.Contains(pathRoot) && line.Contains("OK"))
                {
                    return true; // shareIsProbablyConnected
                }
            }
            return false;
        }

        public void VerificarAplicaDsctoCodigoPromocion(POSEntities db)
        {
            if (this._factura.ClienteIdentificacion == "9999999999999")
            {
                // System.Windows.Forms.MessageBox.Show(this, "No aplica para consumidor final", "Código Promocional", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "No aplica para consumidor final", "POS");

                //Control.Common.General.GetMensaje("POS", "No aplica para consumidor final",  "I");
                Control.Common.General.GetMensajeToList(305);


                //MsgBox msgBox = new MsgBox("info", "No aplica para consumidor final", "POS");
                //DialogResult dialogResult = msgBox.ShowDialog();

                return;
            }
            var parametro = db.core_parametro.Where(x => x.identificador == "CODIGO_PROMO_DSCTO_" + Control.Common.GlobalParameters.EstablecimientoAxCode && x.valor == "TRUE").FirstOrDefault();
            if (parametro == null)
            {
                activoCodigoPromo = false;
                return;
            }
            activoCodigoPromo = true;

            if (this._factura.Productos.Count > 0)
            {
                Control.Pagos.DescuentoPromocion desc = new Control.Pagos.DescuentoPromocion("Descuento ESPECIAL");
                desc.ShowDialog();
                codigoPromocion = desc.codigo;

                if (codigoPromocion != "CLOSEFORM")
                {
                    core_TarjetaDescuento tarjeta = db.core_TarjetaDescuento.Where(x => x.codigo == codigoPromocion && x.codigoCliente == this._factura.ClienteIdentificacion).FirstOrDefault();

                    if (tarjeta == null)
                    {
                        agregarDescuentoCodigoPromocion();
                    }
                    else
                    {
                        // System.Windows.Forms.MessageBox.Show(this, "El código de promoción ya fue aplicado a este cliente", "Código Promocional", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El código de promoción ya fue aplicado a este cliente", "POS - Dscto Promoción");

                        //Control.Common.General.GetMensaje("POS", "El código de promoción ya fue aplicado a este cliente", "I");
                        Control.Common.General.GetMensajeToList(306);
                        return;
                    }
                }
            }
            else
            {
                // System.Windows.Forms.MessageBox.Show(this, "Para aplicar descuento del código de promoción, la factura debe tener productos");
                // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Para aplicar descuento del código de promoción, la factura debe tener productos", "POS - Dscto Promoción" );
                //Control.Common.General.GetMensaje("POS - Dscto Promoción", "Para aplicar descuento del código de promoción, la factura debe tener productos", "I");
                Control.Common.General.GetMensajeToList(307);

                return;
            }
        }

        private void agregarDescuentoCodigoPromocion()
        {
            if (_factura != null && _factura.GetTotal() > 0)
            {
                POSEntities db = new POSEntities();

                var dsctopromo = 0M;
                core_TarjetaDescuento tarj = new core_TarjetaDescuento();
                core_parametro parametro = db.core_parametro.Where(x => x.identificador == "CODIGO_PROMO_DSCTO_" + Control.Common.GlobalParameters.EstablecimientoAxCode && x.valor == "TRUE" && x.parametro2 == codigoPromocion).FirstOrDefault();
                if (parametro == null)
                {
                    // System.Windows.Forms.MessageBox.Show(this, "El código de promoción no es válido", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "El código de promoción no es válido", "POS - Dscto Promoción");
                    //Control.Common.General.GetMensaje("POS - Dscto Promoción", "El código de promoción no es válido", "I");
                    Control.Common.General.GetMensajeToList(308);
                    return;
                }

                if (decimal.TryParse(parametro.documento, out dsctopromo) && dsctopromo > 0)
                {
                    core_descuento dscto = new core_descuento
                    {
                        valor = dsctopromo
                    };
                    _factura.agregarPromocionCodigo(dscto, "NA", codigoPromocion); // dsctopromo
                    usoCodigoPromo = true;
                    calcularFactura();
                }
            }
        }

        private Boolean grabarPromocionCodigo(string msj_error)
        {
            if (msj_error == "" && usoCodigoPromo == true)
            {
                POSEntities db = new POSEntities();
                core_parametro parametro = new core_parametro(); // Objeto para la validación - Descuento Codigo Promocional
                //**** VERIFICA SI LA TIENDA ESTA HABILITADA PARA CODIGO PROMOCIONAL ************************************                
                parametro = db.core_parametro.Where(x => x.identificador == "CODIGO_PROMO_DSCTO_" + Control.Common.GlobalParameters.EstablecimientoAxCode && x.valor == "TRUE").FirstOrDefault();
                string numeroFactura = "";

                if (parametro != null)
                {
                    numeroFactura = this._factura.GetNumeroFactura();

                    core_TarjetaDescuento ctd = new core_TarjetaDescuento();
                    ctd.codigo = codigoPromocion;
                    ctd.fecha_creacion = DateTime.Now;
                    ctd.fecha_modificacion = DateTime.Now;
                    ctd.saldo = 0;
                    ctd.activo = true;
                    ctd.numeroFactura = this._factura.IdFacturaPOS;
                    ctd.codigoCliente = _factura.Cliente_codigo;
                    db.core_TarjetaDescuento.Add(ctd);
                    db.SaveChanges();

                }
                usoCodigoPromo = false;
            }
            return true;
        }

        private void btnGlovo_Click(object sender, EventArgs e)
        {
            /* try
             {
                 PedidosOtrasApp pedido;
                 if (_factura.PedidoOtraApp != null)
                 {
                     pedido = new PedidosOtrasApp(_factura.PedidoOtraApp.Pedido);
                     pedido.ShowDialog();

                     _factura.EsPedidoOtraApp= pedido.TienePedidoOtrApp;
                     _factura.PedidoOtraApp.Pedido = pedido.NumeroPedido;
                     _factura.PedidoOtraApp.Tipo = (byte)pedido.Tipo;

                     if(_factura.EsPedidoOtraApp)
                     {
                         lblPedidoOtrasApp.Text = _factura.PedidoOtraApp.Pedido;
                         lblPedidoOtrasApp.Visible = true;
                     }
                     else
                     {
                         lblPedidoOtrasApp.Text = string.Empty;
                         lblPedidoOtrasApp.Visible = false;
                     }
                 }
                 else
                 {
                     pedido = new PedidosOtrasApp();
                     pedido.ShowDialog();
                     _factura.PedidoOtraApp = new TblPedido()
                     {
                         Pedido = pedido.TienePedidoOtrApp ? pedido.NumeroPedido : string.Empty,
                         Tipo = pedido.TienePedidoOtrApp ? (byte)pedido.Tipo : (byte) 0
                     };
                 }   

             }
             catch (Exception ex)
             {
                 Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "btnGlovo_Click", "No fue posible abrir formulario para Pedidos de Otras App, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex), "StackTrace: " + ex.StackTrace);                
             }*/
        }

        private void btnMenuInicial_Click(object sender, EventArgs e)
        {
            try
            {
                if (_factura.Productos.Count > 0)
                {
                    //System.Windows.Forms.MessageBox.Show(this, "No puede cambiar el tipo de pedido, primero debe borrar los items del carrito", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "No puede cambiar el tipo de pedido, primero debe borrar los items del carrito", "POS");
                    //Control.Common.General.GetMensaje("POS - Dscto Promoción", "No puede cambiar el tipo de pedido, primero debe borrar los items del carrito", "ER");
                    Control.Common.General.GetMensajeToList(309);

                    return;
                }

                llamaMenuInicial();
            }
            catch (Exception)
            {

                //throw;
            }
        }

        public void VerificarSaldoCompraGratis()
        {
            decimal saldoCompraGratis = 0;
            using (POSEntities db = new POSEntities())
            {
                var tarj = db.core_TarjetaDescuento.Where(y => y.codigo == ClienteCompraGratis && y.activo).FirstOrDefault();
                if (tarj != null)
                {
                    saldoCompraGratis = tarj.saldo;
                    //System.Windows.Forms.MessageBox.Show(this, "El saldo actual de la Billetera Electrónica es $ " + saldoCompraGratis, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "El saldo actual de la Billetera Electrónica es $ " + saldoCompraGratis, "POS");
                    //Control.Common.General.GetMensaje("POS", $"El saldo actual de la Billetera Electrónica es ${saldoCompraGratis}", "I");

                    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "MainWindow", "VerificarSaldoCompraGratis"
                        , "El saldo actual de la Billetera Electrónica es $ " + saldoCompraGratis.ToString());

                    List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                    parametros.Add(new ParametrosMensajes() { codigo = "[saldoCompraGratis]", valor = saldoCompraGratis.ToString() });
                    Control.Common.General.GetMensajeToList(310, parametros);


                    btnMonedero.Visible = true;
                }
            }            
        }
         

        public void agregarDescuentoCompraGratis(POSEntities db)
        {
            if (_factura != null && _factura.getSubTotal() > 0)
            {
                var valor = 0M;
                core_TarjetaDescuento tarj = new core_TarjetaDescuento();
                core_parametro parametro = db.core_parametro.Where(x => x.identificador == "COMPRA_GRATIS_PORCENTAJE" ).FirstOrDefault();
                core_parametro fecha_inicio = db.core_parametro.Where(x => x.identificador == "COMPRA_GRATIS_FECHA_INICIO_CONSUMO" ).FirstOrDefault();
                core_parametro fecha_fin = db.core_parametro.Where(x => x.identificador == "COMPRA_GRATIS_FECHA_FIN_CONSUMO").FirstOrDefault();                
                activaConsumoCompraGratis = DateTime.Parse(fecha_inicio.valor);
                expiraConsumoCompraGratis = DateTime.Parse(fecha_fin.valor);
                if (!(DateTime.Now >= activaConsumoCompraGratis && DateTime.Now <= expiraConsumoCompraGratis))
                {
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "Billetera Electrónica, el consumo no está vigente", "POS - Billetera Electrónica");
                    // System.Windows.Forms.MessageBox.Show(this, "Billetera Electrónica, el consumo no está vigente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //Control.Common.General.GetMensaje("POS - Billetera Electrónica", $"Billetera Electrónica, el consumo no está vigente", "I");
                    Control.Common.General.GetMensajeToList(311);
                    return;
                }                

                tarj = db.core_TarjetaDescuento.Where(y => y.codigo == ClienteCompraGratis && y.activo).FirstOrDefault();
                if (tarj != null)
                {
                    if (decimal.TryParse(parametro.valor, out valor) && valor > 0)
                    {
                        decimal dsctoTotal = decimal.Round(decimal.Round((valor / 100M), 2) * _factura.getSubTotal(), 2);
                        if (tarj.saldo < dsctoTotal)
                        {
                            valor = (tarj.saldo / _factura.getSubTotal()) * 100 ;
                            //MessageBox.Show(this, "Tarjeta Compra Gratis, no puede usar el " + valor + " % de su compra porque su saldo actual $ " + tarj.saldo + " es inferior", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //return;
                        }

                        if (_factura.usoTarjetaCompraGratis)
                        {
                            // System.Windows.Forms.MessageBox.Show(this, "$ " + montoCompraGratis + " de la Billetera Electrónica, ya fué aplicado a esta compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "$ " + montoCompraGratis + " de la Billetera Electrónica, ya fué aplicado a esta compra", "POS - Billetera Electrónica");
                            //Control.Common.General.GetMensaje("POS - Billetera Electrónica", $" ${montoCompraGratis},  de la Billetera Electrónica, ya fué aplicado a esta compra", "I");


                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[montoCompraGratis]", valor = decimal.Round(porcenDsctoCompraGratis, 2).ToString() });
                            Control.Common.General.GetMensajeToList(312, parametros);

                            
                            //MessageBox.Show(this, "Descuento Compra Gratis " + decimal.Round(porcenDsctoCompraGratis, 2) + " %, ya fué aplicado a esta compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        bool usoCompraGratis;
                        _factura.agregarDescuentoCompraGratis(valor, tarj.saldo, out usoCompraGratis, out saldoCompraGratis, out montoCompraGratis);
                        _factura.usoTarjetaCompraGratis = usoCompraGratis;

                        if (_factura.usoTarjetaCompraGratis)
                        {
                            porcenDsctoCompraGratis = valor;
                            calcularFactura();


                            List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                            parametros.Add(new ParametrosMensajes() { codigo = "[montoCompraGratis]", valor = montoCompraGratis.ToString() });
                            Control.Common.General.GetMensajeToList(313, parametros);

                            //System.Windows.Forms.MessageBox.Show(this, "$ " + montoCompraGratis + " de la Billetera Electrónica, ha sido usado en la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            //MessageBox.Show(this, "Descuento Compra Gratis $ " + montoCompraGratis + ", ha sido aplicado a la compra", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    //System.Windows.Forms.MessageBox.Show(this, "La Billetera Electrónica, está inactiva", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show(this, "Tarjeta Compra Gratis, está inactiva", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Control.Common.General.GetMensajeToList(314);

                    return;
                }
            }
        }
        public bool grabarAcumulaCompraGratis(string msj_error, Factura factura)
        {
            if (msj_error == "" && ClienteCompraGratis != "")
            {
                using (POSEntities db = new POSEntities())
                {
                    SqlParameter paramResult = new SqlParameter("@respuesta",SqlDbType.VarChar, -1);
                    paramResult.Direction = System.Data.ParameterDirection.Output;

                    var addParameters = new List<SqlParameter>
                     {
                        new SqlParameter("@idFactura", factura.IdFacturaPOS),
                        paramResult
                     };

                    db.Database.ExecuteSqlCommand("PtsCliente.spAcumularCompraGratis @idFactura, @respuesta out", addParameters.ToArray());
                    string response = (string)paramResult.Value;

                    if (!string.IsNullOrEmpty(response))
                    {
                        factura.Recibo = factura.Recibo.Replace("<<COMPRAGRATIS>>", "");
                        return false;
                    }

                    var plantillaCompra = db.core_recibo.Where(x => x.identificador == "COMPRA_GRATIS").FirstOrDefault();

                    var puntoscab = db.TblPuntosCab.Where(x => x.AccountNum == ClienteCompraGratis && x.Estado == 1).ToList();
                    if (puntoscab != null && puntoscab.Count > 0)
                    {
                        int idptocab = puntoscab.FirstOrDefault().IdTblPuntosCab;
                        decimal saldoPuntos = puntoscab.Sum(x => x.Saldo); 
                        var acumulaFactura = db.TblPuntos.Where(x => x.IdTblPuntosCab == idptocab && x.Id_Factura == factura.IdFacturaPOS).ToList();
                        string mensaje = "";
                        if (acumulaFactura != null && acumulaFactura.Count > 0)
                        {
                            mensaje = plantillaCompra.cuerpo;

                            mensaje = mensaje.Replace("<<PTOSNUEVOS>>", acumulaFactura.FirstOrDefault().Saldo.ToString("N2"));
                            mensaje = mensaje.Replace("<<PTOSACUMULADOS>>", saldoPuntos.ToString("N2"));
                        }
                        factura.Recibo = factura.Recibo.Replace("<<COMPRAGRATIS>>", mensaje);
                    }
                }
            }
            factura.Recibo = factura.Recibo.Replace("<<COMPRAGRATIS>>", "");
            return true;
        }

        private Boolean grabarConsumoCompraGratis(string msj_error, string codigoCompraGratis)
        {
            if (msj_error == "" && _factura.usoTarjetaCompraGratis == true)
            {
                POSEntities db = new POSEntities();
                SqlParameter paramResult = new SqlParameter("@respuesta", "");
                paramResult.Direction = System.Data.ParameterDirection.Output;
                string xmlconsumo = "<root><req idf='"+_factura .IdFacturaPOS+"' cliente='"+ _factura.ClienteIdentificacion+ "' monto='"+ montoCompraGratis + "' fecha='"+DateTime.Now.ToString("yyyyMMdd")+"' /></root>";
                var addParameters = new List<SqlParameter>
                     {
                        new SqlParameter("@xmlRequest", xmlconsumo),
                        paramResult
                     };

                db.Database.ExecuteSqlCommand("PtsCliente.spConsumoCompraGratis @xmlRequest, @respuesta out", addParameters.ToArray());
                string response = (string)paramResult.Value;

                if (!string.IsNullOrEmpty(response))
                    return false;

                //core_parametro parametro = new core_parametro();
                //core_TarjetaDescuento ctd;
                //ctd = db.core_TarjetaDescuento.Where(x => x.codigo == codigoCompraGratis).FirstOrDefault();
                //ctd.fecha_modificacion = DateTime.Now;
                //ctd.saldo = saldoCompraGratis;
                //ctd.fecha_activacion = activaConsumoCompraGratis;
                //ctd.fecha_expiracion = expiraConsumoCompraGratis;
                //ctd.codigoCliente = _factura.Cliente_codigo;
                //db.SaveChanges();
                 
            }            
            return true;
        }

        public void AnulaDsctoCompraGratis()
        {
            //Si ya aplicó descuento compra gratis se anula en todos los productos para que lo vuelva aplicar.   JM  1/7/2020
            if (_factura.usoTarjetaCompraGratis)
            {
                bool usoComprGratis;
                _factura.quitarDescuentoCompraGratis(porcenDsctoCompraGratis, out usoComprGratis);
                _factura.usoTarjetaCompraGratis = usoComprGratis;
                
                calcularFactura();
                // System.Windows.Forms.MessageBox.Show(this, "$ " + montoCompraGratis + " de la Billetera Electrónica, ha sido anulado de la compra, por favor volver a presionar Billetera Electrónica", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Information, "$ " + montoCompraGratis + " de la Billetera Electrónica, ha sido anulado de la compra, por favor volver a presionar Billetera Electrónica", "POS - Billetera Electrónica");
                //Control.Common.General.GetMensaje("POS - Billetera Electrónica", $" ${montoCompraGratis},  de la Billetera Electrónica, ha sido anulado de la compra, por favor volver a presionar Billetera Electrónica", "I");
                

                List<ParametrosMensajes> parametros = new List<ParametrosMensajes>();
                parametros.Add(new ParametrosMensajes() { codigo = "[montoCompraGratis]", valor = montoCompraGratis.ToString() });
                Control.Common.General.GetMensajeToList(315, parametros);



                //MessageBox.Show(this, "Descuento Compra Gratis $ " + montoCompraGratis + ", ha sido anulado de la compra, por favor volver a presionar Compra Gratis", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);                 
            }
        }

        public void LimpiarClienteCompraGratis()
        {
            ClienteCompraGratis = "";
            btnMonedero.Visible = false;
            btnCuponApp.Visible = false;
            _factura.usoTarjetaCompraGratis = false;
            porcenDsctoCompraGratis = 0;
            montoCompraGratis = 0;
        }

        private void tempo666_Tick(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(ClienteCompraGratis) && txtCedula.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
            {
                txtCedula.Text = "";
            }
            if (string.IsNullOrEmpty(ClienteCompraGratis) && txtCodigo.Text.Trim().StartsWith(POS.Control.Common.GlobalParameters.AppMovil_PrefijoUsaApp))
            {
                txtCodigo.Text = "";
            }

            tempo666.Stop();
        }
         
        //private void txtCedula_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.End || e.KeyCode == Keys.Home 
        //        || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
        //    { 
        //        e.SuppressKeyPress = true;
        //    }
        //} 

        public void ActivaFormaPagoDelivery()
        {
            btnOrdenDelivery.Visible = true;
            btnOrdenDelivery.Text = this.FacturaActual.Delivery;
            btnEfectivo.Enabled = false;
            btnTCredito.Enabled = false;
            btnCheque.Enabled = false;
            btnNC.Enabled = false;
            btnDsctoEsp.Enabled = false;
            btnMonedero.Enabled = false;
            btnCuponApp.Enabled  = false;
            btnPagoGiftCard.Enabled = false;
            btnCreditoInterno.Enabled = false;
            btnRetencion.Enabled = false;
            btnDsctoPaviPlan.Enabled = false;
             
        }

        public void InactivaFormaPagoDelivery()
        {
            btnOrdenDelivery.Visible = false;
            btnOrdenDelivery.Text = "";
            btnEfectivo.Enabled = true;
            btnTCredito.Enabled = true;
            btnCheque.Enabled = true;
            btnNC.Enabled = true;
            btnDsctoEsp.Enabled = true;
            btnMonedero.Enabled = true;
            btnCuponApp.Enabled  = true;
            btnPagoGiftCard.Enabled = true;
            btnCreditoInterno.Enabled = true;
            btnRetencion.Enabled = true;
            btnDsctoPaviPlan.Enabled = true;                       
        }

        private void btnOrdenDelivery_Click(object sender, EventArgs e)
        {            
            //if (revisarCambioEnTotal(this.FacturaActual.DeliveryFormaPago, this.FacturaActual.DeliveryFormaPago))
            //{
            //    return; 
            //}

            if (_factura != null && _factura.GetTotal() > 0)
            {
                var valor = 0M;
                if (decimal.TryParse(txtPagoValor.Text, out valor) && valor > 0)
                {
                    _factura.agregarPagoDelivery(valor);
                    txtPagoValor.Clear();
                    calcularFactura();
                    agregaFormasPagoTmpFile();
                }
            }
        }

        private Boolean grabarCodigoCuponPromocional(string msj_error)
        {
            if (msj_error == "" && _factura.EsUsoCuponPromocional)
            {
                POSEntities db = new POSEntities();
                var cupon = db.core_TarjetaDescuento.Where(x => x.codigo == codigoCuponPromocional.codigo && x.numeroFactura == -1).FirstOrDefault();
                cupon.activo = false;
                cupon.fecha_desactivacion = DateTime.Now;
                cupon.codigoCliente = _factura.getNumeroFacturaOpcional();
                db.SaveChanges();

                //Cambia el estado de los N# cupones usados 
                foreach (var producto in _factura.Productos)
                {
                    if (producto.DescuentosCupon != null)
                    {
                        foreach (var cuponUsado in producto.DescuentosCupon)
                        {
                            var cuponItem = db.core_TarjetaDescuento.Where(x => x.codigo == cuponUsado.codigo && x.numeroFactura == -1).FirstOrDefault();
                            cuponItem.activo = false;
                            cuponItem.fecha_desactivacion = DateTime.Now;
                            cuponItem.codigoCliente = _factura.getNumeroFacturaOpcional();
                            db.SaveChanges();
                        }
                    }

                }
            }
            return true;
        }

        private void clienteGroup_Click(object sender, EventArgs e)
        {

        }

        private void tempo2minutos_Tick(object sender, EventArgs e)
        {

            FindWindow(null, "Mensaje");
            if (FindWindow(null, "Mensaje") != 0)
            {
                // Ha encontrado el MessageBox, busca ahora el texto


            }
        }

        
        private void txtCedula_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {

                txtCedula.Text = "";
            }
        }


        private void txtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Home || e.KeyCode == Keys.Up)
            {
                txtCodigo.Text = "";
            }
        }

        private void btnCuponApp_Click(object sender, EventArgs e)
        {
            SolicitarCuponApp();
        }

        //private void pnlKbd_Paint(object sender, PaintEventArgs e)
        //{

        //}

        private void btnPagoEnter_Click(object sender, EventArgs e)
        {

        }

        //private void pnlKbd_Paint_1(object sender, PaintEventArgs e)
        //{

        //}

        private void txtCodigo_TextChanged(object sender, EventArgs e)
        {

        }

        //private void pnlKbd_Paint_2(object sender, PaintEventArgs e)
        //{

        //}
  

        private void pbLogoPOS_Click(object sender, EventArgs e)
        {
            try
            {
                if (_factura.Productos.Count > 0)
                {
                    Control.Common.General.GetMensajeToList(316);
                    //Control.Common.General.GetMensaje("POS", " No puede cambiar el tipo de pedido, primero debe borrar los items del carrito", "I");
                    //msgBoxCtrl.ShowMessage(MsgBoxCtrl.MessageType.Error, "No puede cambiar el tipo de pedido, primero debe borrar los items del carrito", "POS");
                    //System.Windows.Forms.MessageBox.Show(this, "No puede cambiar el tipo de pedido, primero debe borrar los items del carrito", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                llamaMenuInicial();
            }
            catch (Exception)
            {

                //throw;
            }
        }

        private void txtCedula_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnPagoEnter_Click_1(object sender, EventArgs e)
        {

        }


        //private void btnPrueba_Click(object sender, EventArgs e)
        //{
        //    MsgBox m = new MsgBox("warning", "Este es un ejemplo de un message box personalizado");
        //    DialogResult dg = m.ShowDialog();
        //    //label1.Text = dg.ToString();
        //}


    }
}
