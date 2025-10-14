using System;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using POS.Models;
using MensajesLibrary;

namespace POS.Control.Peso
{
    public partial class TomaPesoUI : RadForm
    {
        public bool correcto;
        public string _estab;
        public string _modeloBalanza;
        private OposScale_CCO.OPOSScale _scaleDL;
        private ServiceOPOSLib.ServiceOPOSScale _scaleDL9800;
        private bool esSuperUsuario;
        private string _ptoemisionorigen = "";

        public decimal Peso
        {
            get;
            set;
        }

        private TomaPeso balanza;

        public TomaPesoUI(string nombre_producto, Factura factura)
        {
            InitializeComponent();
            this.lblNombreProducto.Text = nombre_producto;
            //MessageBox.Show(this,"Ponga el producto en la balanza", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //balanza = new TomaPeso(factura.PuertoBalanza, TomaPeso.BalanzaMarcas.CAS, false, true, false);

            TomaPeso.BalanzaMarcas marca_balanza;
            _ptoemisionorigen = factura.PtoEmisionOrigen;

            //MessageBox.Show(this,"Factura Balanza: " + factura.MarcaBalanza.ToString());
            //txtPeso.Visible = true;
            //esSuperUsuario = true;

            txtPeso.Visible = false;
            esSuperUsuario = false;

            if (factura.User.isSuperUser)
            {
                txtPeso.Visible = true;
                esSuperUsuario = true;
            }


            if (Control.Common.GlobalParameters.PESO_MANUAL)
            {
                if (factura.MarcaBalanza == "MS2420")
                {
                    factura.MarcaBalanza = "METTER_TOLEDO";
                }

                if (factura.MarcaBalanza == "CAS")
                {
                    marca_balanza = TomaPeso.BalanzaMarcas.CAS;
                }
                else if (factura.MarcaBalanza == "METTER_TOLEDO")
                {
                    marca_balanza = TomaPeso.BalanzaMarcas.METTLER_TOLEDO;
                    //MessageBox.Show(this,"Marca balanza METTER TOLEDO");
                }
                else
                {
                    marca_balanza = TomaPeso.BalanzaMarcas.DATALOGIC;
                }
                //balanza = new TomaPeso(factura.PuertoBalanza, marca_balanza, false, true, false);
                balanza = new TomaPeso(factura.PuertoBalanza, marca_balanza, false, true, false);
            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var db = new POSEntities();
            object sender = new object();
            EventArgs e = new EventArgs();

            switch (keyData)
            {
                case Keys.Escape:
                    this.Close();
                    break;

                case Keys.Enter:
                    btnOk_Click(sender, e);
                    break;

            }


            return base.ProcessCmdKey(ref msg, keyData);
        }


        public delegate void UpdateLabel(string msg);

        private void PesoBalanza_Load(object sender, EventArgs e)
        {
            if (!esSuperUsuario)
            {
                //if (balanza == null) { balanza = new TomaPeso); }
                balanza.ControlToShowText = lblPeso;
                balanza.UnidadDeMedidad = TomaPeso.UnidadMedida.LB;
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  - PesoBalanza_Load");

                if (!String.IsNullOrEmpty(_modeloBalanza))
                {
                    string[] parametrosbalanza = _modeloBalanza.Split('|');
                    if (balanza.BalanzaMarca == TomaPeso.BalanzaMarcas.DATALOGIC && parametrosbalanza[2] != "")
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  -  establecimiento:" + _estab + " driver balanza:" + parametrosbalanza[2].ToString() + " balanza.BalanzaMarca == TomaPeso.BalanzaMarcas.DATALOGIC");
                        // MessageBox.Show(this, "Ponga el producto en la balanza (DL)", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Control.Common.General.GetMensajeToList(527);

                        _scaleDL = new OposScale_CCO.OPOSScale();
                        _scaleDL.Open(parametrosbalanza[2].ToString());
                        _scaleDL.ClaimDevice(0);
                        _scaleDL.DeviceEnabled = true;
                        _scaleDL.DataEventEnabled = true;
                        _scaleDL.AutoDisable = false;

                        tmrDL.Enabled = true;
                    }
                    else
                    {
                        balanza.Open();

                        var result = Control.Common.General.GetMensajeToList(528);
                        if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                        {
                            this.balanza.Serial.WriteLine(string.Format("{0}11{1}{2}", Convert.ToChar(2), Convert.ToChar(3), Convert.ToChar(3)));
                        }
                    }
                }
                else
                {

                    if ((_estab == "024" || (_estab == "007" && _ptoemisionorigen == "007") || (_estab == "011" && _ptoemisionorigen == "008"))
                        && balanza.BalanzaMarca == TomaPeso.BalanzaMarcas.DATALOGIC)
                    {
                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  - _estab == '024' && balanza.BalanzaMarca == TomaPeso.BalanzaMarcas.DATALOGIC");
                        //MessageBox.Show(this, "Ponga el producto en la balanza (DL)", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Control.Common.General.GetMensajeToList(527);

                        if (_ptoemisionorigen == "004" || (_estab == "024" && _ptoemisionorigen == "007")) //|| _ptoemisionorigen == "006")
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  -   if (_ptoemisionorigen == '004')");
                            _scaleDL = new OposScale_CCO.OPOSScale();
                            _scaleDL.Open("USBScale");
                            _scaleDL.ClaimDevice(0);
                            _scaleDL.DeviceEnabled = true;
                            _scaleDL.DataEventEnabled = true;
                            _scaleDL.AutoDisable = false;

                        }
                        else
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  -   else del -if (_ptoemisionorigen == '004')-");
                            _scaleDL = new OposScale_CCO.OPOSScale();
                            _scaleDL.Open("MagellanSC");
                            _scaleDL.ClaimDevice(0);
                            _scaleDL.DeviceEnabled = true;
                            _scaleDL.DataEventEnabled = true;
                            _scaleDL.AutoDisable = false;

                        }
                        tmrDL.Enabled = true;
                    }
                    else if ((_estab == "029" || _estab == "035" || _estab == "037" || _estab == "040" || _estab == "011") && balanza.BalanzaMarca == TomaPeso.BalanzaMarcas.DATALOGIC)
                    {
                        //MessageBox.Show(this, "Ponga el producto en la balanza (DL)", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Control.Common.General.GetMensajeToList(527);

                        Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  -   else del -if ((_estab =='029' || _estab == '035' || _estab == '037' || _estab == '040' || _estab == '011') && balanza.BalanzaMarca == TomaPeso.BalanzaMarcas.DATALOGIC)");
                        _scaleDL = new OposScale_CCO.OPOSScale();
                        _scaleDL.Open("USBScale");
                        _scaleDL.ClaimDevice(0);
                        _scaleDL.DeviceEnabled = true;
                        _scaleDL.DataEventEnabled = true;
                        _scaleDL.AutoDisable = false;

                        /*
                        _scaleDL9800.OpenService("USBScale", "DLS Magellan 9800i",null);
                        _scaleDL9800.ClaimDevice(0);
                        */

                        tmrDL.Enabled = true;
                    }
                    else
                    {
                        balanza.Open();

                        var result = Control.Common.General.GetMensajeToList(528);
                        if (result == MsgBoxCtrl.MessageBoxResult.Yes || result == MsgBoxCtrl.MessageBoxResult.Ok)
                        {
                            Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  -   else ");
                            this.balanza.Serial.WriteLine(string.Format("{0}11{1}{2}", Convert.ToChar(2), Convert.ToChar(3), Convert.ToChar(3)));
                        }


                        //if (MessageBox.Show(this, "Ponga el producto en la balanza", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                        //{
                        //    Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Info, "PesoBalanza_Load", "PesoBalanza_Load", " ingresa  -   else ");
                        //    //MessageBox.Show(this,"Here Trouble");
                        //    this.b1alanza.Serial.WriteLine(string.Format("{0}11{1}{2}", Convert.ToChar(2), Convert.ToChar(3), Convert.ToChar(3)));
                        //    //MessageBox.Show(this,"End Trouble");
                        //}
                    }
                }
            }
        }

        private void PesoBalanza_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                tmrDL.Enabled = false;
                _scaleDL.DeviceEnabled = false;
                _scaleDL.ReleaseDevice();
                _scaleDL.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public void btnOk_Click(object sender, EventArgs e)
        {
            if (balanza.Peso > 0)
            {
                Peso = balanza.Peso;
                correcto = true;
            }

            this.Close();
        }

        private void UpdateText(string msg)
        {
            //this.lblPeso.Text = msg;
            this.lblPeso.Text = decimal.Parse(msg).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " lb";
            this.lblpesokl.Text = (decimal.Parse(msg) * 0.453592M).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";
        }

        private void PesoBalanza_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!esSuperUsuario)
            {
                if (balanza.IsOpen)
                    balanza.Close();
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            if (esSuperUsuario)
            {
                decimal valor = 0M;
                decimal.TryParse(txtPeso.Text.Trim(), out valor);
                Peso = valor;
                correcto = true;
            }
            else
            {
                if (balanza.Peso > 0)
                {
                    Peso = balanza.Peso;
                    correcto = true;
                }
            }
            this.Close();
        }

        private void tmrDL_Tick(object sender, EventArgs e)
        {
            try
            {
                int peso = 0;
                int peso2 = 0;
                decimal p2 = 0.00M;
                decimal mil = 1000M;
                decimal pfinal = 0.00M;
                peso = _scaleDL.ReadWeight(out peso2, 1000);
                p2 = decimal.Parse(peso2.ToString());
                if (peso2 > 0)
                {
                    pfinal = decimal.Round(p2 / mil, Common.GlobalParameters.CantidadDecimalesBascula);
                }
                balanza.setPeso = pfinal;
                _scaleDL.DataEventEnabled = true;
                lblPeso.Text = pfinal.ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " lb";
                lblpesokl.Text = (pfinal * 0.453592M).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";
            }
            catch (Exception ex)
            {
                Control.Common.Logger.LogMessage(Control.Common.Enum.LogTypes.Error, "TomaPesoUI", "tmrDL_Tick", ex.Message + ex.StackTrace);
                //MessageBox.Show(this, "El peso no fué capturado, vuelva a poner el producto en la balanza (DL)", "Peso no capturado..!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Control.Common.General.GetMensajeToList(529);
            }
        }

        private void btnMostarKL_Click(object sender, EventArgs e)
        {
            lblpesokl.Text = (decimal.Parse(lblPeso.Text.ToLower().Replace("lb", "")) * 0.453592M).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";
            this.Height = 447;
        }

        private void txtPeso_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lblPeso.Text = decimal.Parse(txtPeso.Text).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " lb";
                lblpesokl.Text = (decimal.Parse(txtPeso.Text) * 0.453592M).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";
            }
            catch
            {
                lblPeso.Text = (0).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " lb";
                lblpesokl.Text = (0).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";
            }

        }

        private void lblPeso_TextChanged(object sender, EventArgs e)
        {
            this.lblPeso.Text = decimal.Parse(lblPeso.Text.ToLower().Replace("lb", "")).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " lb";
            this.lblpesokl.Text = (decimal.Parse(lblPeso.Text.ToLower().Replace("lb", "")) * 0.453592M).ToString("N" + Common.GlobalParameters.CantidadDecimalesBascula.ToString()) + " kg";
        }
    }
}