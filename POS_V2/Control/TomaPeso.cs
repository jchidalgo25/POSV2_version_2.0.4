using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace POS.Control
{
    public class TomaPeso
    {
        #region Variables privadas

        private SerialPort serial;
        private BalanzaMarcas _marca_balanza = BalanzaMarcas.CAS;
        private decimal _peso;
        private string _comPort = "COM1";
        private UnidadMedida _unidadMedida = UnidadMedida.LB;
        private bool usarScanner =false;
        private bool tomarPeso = false;
        private bool pesoLibre = false;

        

        #endregion Variables privadas

        public enum UnidadMedida
        {
            LB, KG
        }

        public enum BalanzaMarcas
        {
            CAS, METTLER_TOLEDO, RICE_LAKE, DATALOGIC
        }

        #region Propiedades Publicas

        public BalanzaMarcas BalanzaMarca
        {
            get { return this._marca_balanza; }
            set
            {
                this._marca_balanza = value;
                if (value != BalanzaMarcas.DATALOGIC)
                {
                    //MessageBox.Show(this,"Class: tomaPeso != Datalogic");
                    if (this.serial.IsOpen)
                    {
                        Close();
                    }
                    Open();
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                if (serial != null)
                    return this.serial.IsOpen;
                else
                    return false;
            }
        }

        public SerialPort Serial
        { get { return serial; } }

        public decimal Peso
        {
            get { return this._peso; }
        }

        public decimal setPeso
        {
            set { this._peso = value; }
        }

        public System.Windows.Forms.Control ControlToShowText
        {
            get;
            set;
        }

        public UnidadMedida MedidaActual { get; set; }

        public UnidadMedida UnidadDeMedidad
        {
            get { return _unidadMedida; }
            set { _unidadMedida = value; }
        }

        public string ComPort
        {
            get { return this._comPort; }
            set { this._comPort = value; }
        }

        #endregion Propiedades Publicas

        /// <summary>
        /// Identifica si el texto enviado es un codigo de barras.
        /// </summary>
        /// <param name="data">Texto a validar</param>
        /// <returns>True or False</returns>
        public bool isBarcode(string data)
        {
            Regex inicio = new Regex("^08\\w");
            if (inicio.Match(data).Success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Toma el codigo de barra de la informacion enviada desde la balanza/escaner Solo EAN13
        /// </summary>
        /// <param name="data">Texto a verificar</param>
        /// <returns>Codigo de barra EAN13</returns>
        public string ReadEAN13serial(string data)
        {
            Regex inicio = new Regex("^08\\w");
            Regex fin = new Regex("+(\\w*\\W*|\\W*\\w*)$");
            return fin.Replace(inicio.Replace(data, ""), "");
        }

        public bool isWeight(string data)
        {
            Regex inicio = new Regex("^11\\w");
            if (inicio.Match(data).Success)
                return true;
            return false;
        }

        public string ReadWeight(string data)
        {
            Regex inicio = new Regex("^11");
            Regex fin = new Regex("+(\\w*\\W*|\\W*\\w*)$");
            decimal result = decimal.Parse(fin.Replace(inicio.Replace(data, ""), ""));
            return (result / 100M).ToString();
        }

        public event EventHandler ScannerDataReceived;

        public delegate void ScannerDataReceivedHandler(object sender, EventArgs args);

        public TomaPeso(string comPort, BalanzaMarcas balanza, bool scanner, bool requestPeso, bool requestPesoLibre)
        {
            this._comPort = comPort;
            this._marca_balanza = balanza;
            this.usarScanner = scanner;
            this.tomarPeso = requestPeso;
            this.pesoLibre = requestPesoLibre;
        }

        #region Eventos

        public delegate void UpdateControlText(string msg);

        private void UpdateText(string msg)
        {
            ControlToShowText.Text = msg;
            if (usarScanner)
            {
                if (this.ScannerDataReceived != null)
                {
                    this.ScannerDataReceived(this, new EventArgs());
                }                    
            }
        }

        public void UpdateText2(string msg)
        {
            ControlToShowText.Text = msg;
            //if (usarScanner)
            //{
            //    if (this.ScannerDataReceived != null)
            //    {
            //        this.ScannerDataReceived(this, new EventArgs());
            //    }
            //}
        }

        private void serial_DataReceivedPPG(object sender, SerialDataReceivedEventArgs e)
        {
            //MessageBox.Show(this,"Data receiver PPG"); //DATARECEIVER
            try
            {
                if (serial.IsOpen)
                {
                    string s = this.serial.ReadLine();

                    if (s.Length == 21)
                    {
                        s = s.Substring(9, 9);
                        _peso = decimal.Parse(s);
                        ControlToShowText.BeginInvoke(new UpdateControlText(this.UpdateText), new object[] { s });
                    }
                }
            }
            catch
            {
            }
        }

        private void serial_DataReceivedScanner(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //MessageBox.Show(this,"Data receive Scanner"); //DATARECEIVER SCANNER
            try
            {
                if (serial.IsOpen)
                {
                    Thread.Sleep(50);
                    string s = this.serial.ReadExisting();
                    this.serial.DiscardInBuffer();
                    if (isBarcode(s))
                    {
                        ControlToShowText.BeginInvoke(new UpdateControlText(this.UpdateText), new object[] { ReadEAN13serial(s) });
                    }
                }
            }
            // catch { }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TomaPeso", "serial_DataReceivedScanner", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

               /* var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "Scanner no pudo recibir Data",
                        String.Format("El POS del siguiente punto de emision no pudo leer la data desde el scanner:  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} ------------\nClass: {6} \nMethod: {7} \nStackTrace: {8}",
                                    Control.Common.GlobalParameters.Establecimiento,
                                    Control.Common.GlobalParameters.PuntoEmision,
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.UsuarioNombre,
                                    Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                    "TomaPeso",
                                    "serial_DataReceivedScanner",
                                    ex.StackTrace
                                    ),
                        false,
                        String.Empty);*/


            }
            finally
            {
                GC.Collect();
            }
        }

        private void serial_DataReceivedPOSPeso(object sender, SerialDataReceivedEventArgs e)
        {
            //MessageBox.Show(this,"Datareceive POS Peso"); //DATARECEIVER
            try
            {
                if (serial.IsOpen)
                {
                    Thread.Sleep(50);
                    string s = this.serial.ReadExisting();
                    this.serial.DiscardInBuffer();
                    if (isWeight(s))
                    {
                        _peso = decimal.Parse(ReadWeight(s).ToString());
                        ControlToShowText.BeginInvoke(new UpdateControlText(this.UpdateText), new object[] { _peso.ToString("N2") });
                    }
                }
            }
            catch(Exception ex) {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TomaPeso", "serial_DataReceivedPOSPeso", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

                /*var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "Caja no pudo obtener peso de la Balanza.",
                        String.Format("El POS del siguiente punto de emision no pudo obtener el peso de la balanza:  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} ------------\nClass: {6} \nMethod: {7} \nStackTrace: {8}",
                                    Control.Common.GlobalParameters.Establecimiento,
                                    Control.Common.GlobalParameters.PuntoEmision,
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.UsuarioNombre,
                                    Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                    "TomaPeso",
                                    "serial_DataReceivedPOSPeso",
                                    ex.StackTrace
                                    ),
                        false,
                        String.Empty);*/

               
            }
            finally
            {
                GC.Collect();
            }
        }

        private void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                if (serial.IsOpen)
                {
                    string s = serial.ReadLine();

                    if (this._marca_balanza == BalanzaMarcas.CAS && s.Length == 21)
                    {
                        //MessageBox.Show(this,"Serial_DataReceive CAS");//DATARECEIVER
                        s = s.Substring(9, 11);
                        this._peso = (decimal)float.Parse(s.Substring(0, s.Length - 2).Trim());
                    }
                    else if (this._marca_balanza == BalanzaMarcas.METTLER_TOLEDO)
                    {                        
                        s = s.Substring(5, 5);
                        if (UnidadDeMedidad == UnidadMedida.LB)
                        {
                            this._peso = (decimal.Parse(s) / 10M);
                        }
                        else
                        {
                            this._peso = (decimal.Parse(s));
                        }
                    }
                    else if (this._marca_balanza == BalanzaMarcas.RICE_LAKE)
                    {
                        //"  24.440LG ";
                        var medida = s.Substring(9, 1);
                        s = s.Substring(1, 8);

                        if (medida == "L")
                        {
                            this.MedidaActual = UnidadMedida.LB;
                        }
                        else
                        {
                            this.MedidaActual = UnidadMedida.KG;
                        }
                        this._peso = decimal.Parse(s);
                    }
                    else if (this._marca_balanza == BalanzaMarcas.DATALOGIC)
                    {
                        s = s.Substring(5, 5);

                        if (UnidadDeMedidad == UnidadMedida.LB)
                        {
                            this._peso = (decimal.Parse(s) / 10M);
                        }
                        else
                        {
                            this._peso = (decimal.Parse(s));
                        }
                    }
                    if (ControlToShowText != null)
                    {
                        ControlToShowText.BeginInvoke(new UpdateControlText(this.UpdateText), new object[] { this._peso.ToString("N2") + " " + UnidadDeMedidad.ToString() });
                    }
                }
            }            
            catch (Exception ex)
            {
                MessageBox.Show("Existió un problema con la balanza. Detalle: " + ex.Message.ToString());
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TomaPeso", "serial_DataReceived", "Ha ocurrido una excepción, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

               /* var xmlRespuesta = Control.Common.Mail.EnviaCorreo(
                        Properties.Settings.Default.MAILERROR_FROM,
                        Properties.Settings.Default.MAILERROR_ALIAS,
                        Properties.Settings.Default.MAILERROR_DESTINO,
                        Properties.Settings.Default.MAILERROR_CC,
                        "Problema con la balanza",
                        String.Format("El POS del siguiente punto de emision tiene problemas con la balanza:  \n\nEstablecimiento: {0} \nPto Emision: {1} \nIpMaquina: {2} \nCajeroNombre: {3} \nCajeroIdentificacion: {4} \nExcepcion: {5} ------------\nClass: {6} \nMethod: {7} \nStackTrace: {8}",
                                    Control.Common.GlobalParameters.Establecimiento,
                                    Control.Common.GlobalParameters.PuntoEmision,
                                    Control.Common.GlobalParameters.IpMaquina,
                                    Control.Common.GlobalParameters.UsuarioNombre,
                                    Control.Common.GlobalParameters.Usuario,
                                    Control.Common.ExceptionHandler.GetExceptionMessages(ex),
                                    "TomaPeso",
                                    "serial_DataReceived",
                                    ex.StackTrace
                                    ),
                        false,
                        String.Empty);*/


            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion Eventos

        public void Open()
        {
            try
            {
                
                if (this._marca_balanza == BalanzaMarcas.DATALOGIC)
                {

                }
                else
                {
                    SerialPortFixer.Execute(this._comPort);
                    serial = new SerialPort(this._comPort, 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    serial.PortName = this._comPort;
                    serial.RtsEnable = true;
                    serial.Open();

                    if (serial.IsOpen)
                    {
                        if (this._marca_balanza == BalanzaMarcas.CAS)
                        {
                            serial.NewLine = "\n";
                            serial.WriteLine("P");
                            //MessageBox.Show(this,"Serial Newline CAS");
                        }
                        else if (this._marca_balanza == BalanzaMarcas.METTLER_TOLEDO)
                        {
                            serial.NewLine = "\r";
                            
                        }
                        else if (this._marca_balanza == BalanzaMarcas.DATALOGIC) //ADD
                        {
                            serial.NewLine = "\r";
                        }

                        if (tomarPeso)
                        {
                            if (this._marca_balanza == BalanzaMarcas.CAS)
                            {                                
                                serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceivedPPG);
                                //MessageBox.Show(this,"serial_DataReceivedPPG");
                            }
                            else
                            {
                                serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceivedPOSPeso);
                                //MessageBox.Show(this,"serial_DataReceivedPOSPeso");
                            }
                        }
                        else
                        {
                            if (usarScanner)
                            {
                                serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceivedScanner);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TomaPeso", "Open", "Ha ocurrido una excepción al momento de abrir el Puerto COM, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));
                throw;
            }
        }

        void serial_CaptureWeightDL(int Status)
        {
            
        }

        public void Close()
        {
            try
            {
                if (serial.IsOpen)
                    this.serial.Close();
                if (ControlToShowText != null)
                    ControlToShowText.BeginInvoke(new UpdateControlText(this.UpdateText), new object[] { "" });
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TomaPeso", "Close", "Ha ocurrido una excepción al momento de cerrar el Puerto COM, a continuacion las excepciones encontradas - " + Control.Common.ExceptionHandler.GetExceptionMessages(ex));

            }
            finally
            {
                GC.Collect();
            }
           
        }
    }
}
