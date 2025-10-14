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
using POS.Control.Common;

namespace POS.Control.Encuestas
{
    public partial class Encuesta : Telerik.WinControls.UI.RadForm
    {
        #region Private Attributes

        private LstPregunta objPregunta;
        private LstEncuesta objEncuesta;
        private TblEncuestaRealizada objEncRealizada;
                
        private int minSiguientePregunta = 1;
        private int nroRadioBtn = 1;
        private bool esPrimeraPregunta = true;

        #endregion

        #region Constructors

        public Encuesta(LstEncuesta encuesta)
        {
            objEncuesta = encuesta;
            InitializeComponent();
        }

        #endregion

        #region Controls Methods

        private void Encuesta_Load(object sender, EventArgs e)
        {
            try
            {
                this.FormElement.TitleBar.CloseButton.Visibility = ElementVisibility.Hidden;
                btnAbandonar.Visible = objEncuesta.PermiteAbandonar;
                ContinuarCuestionario();
                esPrimeraPregunta = false;
            }
            catch (Exception)
            {
                this.Close();
            }
        }

        private void btnAbandonar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            GrabarRespuesta();
        }

        private void Encuesta_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {

            switch (keyData)
            {

                case Keys.Escape:
                    this.Close();
                    break;


            }
            return base.ProcessCmdKey(ref msg, keyData);
        }



        #endregion

        #region Private Methods

        private void AbrirEncuesta()
        {
            objEncRealizada = new TblEncuestaRealizada();
            objEncRealizada.FechaCreacion = DateTime.Now;
            objEncRealizada.IdEncuesta = objEncuesta.IdEncuesta;
        }

        private bool GetNextPregunta()
        {
            //El minSiguientePregunta variara de acuerdo a la respuesta dada a la ultima pregunta 

            //Si el usuario seleccionó previamente una respuesta con indicador -1, el sistema debe terminar la encuesta en ese momento
            if (minSiguientePregunta == -1)
                return false;
            
            using (POSEntities db = new POSEntities())
            {
                objPregunta = db.LstPregunta.Where(x => x.LstEncuesta.IdEncuesta == objEncuesta.IdEncuesta
                                                        && x.Estado == true
                                                        && x.Orden >= minSiguientePregunta)
                                            .OrderBy(x => x.Orden)
                                            .FirstOrDefault();
            }
            if (objPregunta != null)
            {
                minSiguientePregunta = objPregunta.Orden + 1;
                return true;
            }
            else
                return false;
        }

        private string GetScriptInicial()
        {
            string response = string.Empty;

            try
            {
                using (POSEntities db = new POSEntities())
                {
                    response = db.core_parametro.Where(x => x.identificador == "ENCUESTA_SCRIPTINICIAL" 
                                                                && x.parametro2 == objEncuesta.Descripcion)
                                                .FirstOrDefault()
                                                .valor + System.Environment.NewLine;
                }
            }
            catch (Exception)
            {
                response = string.Empty;
            }

            return response;
        }

        private void ArmarControlesPregunta()
        {
            LimpiarPanelControles();
            int maxWidthControl = 800;
            txtRespuesta.Visible = false;
            nroRadioBtn = 1;
            lblPregunta.Text = (!esPrimeraPregunta ? "" : GetScriptInicial()) + objPregunta.Descripcion;
            //Hack para tener inmediatamente el size del label con el texto recien asignado
            Application.DoEvents();
            //Obtener el nuevo alto del label de pregunta
            int heightLblPregunta = lblPregunta.Size.Height;
            switch (objPregunta.TipoPregunta)
            {
                //Cerrada, una respuesta multiples opciones
                case 1:
                    using (POSEntities db = new POSEntities())
                    {
                        var listOptions = db.LstOpcionesRespuesta.Where(x => x.Estado == true
                                                                                && x.IdPregunta == objPregunta.IdPregunta)
                                                                 .ToList();

                        foreach (var option in listOptions)
                        {
                            System.Windows.Forms.RadioButton rad = new System.Windows.Forms.RadioButton();
                            rad.Top = (nroRadioBtn * 35) + heightLblPregunta;
                            rad.Left = 100;
                            rad.Size = new Size(maxWidthControl, 35);
                            rad.Tag = option;
                            rad.Text = option.Descripcion;

                            pnlControls.Controls.Add(rad);

                            nroRadioBtn += 1;
                        }
                    }
                    break;
                //Abierta, texto libre
                case 2:
                    txtRespuesta.Visible = true;
                    txtRespuesta.Focus();
                    break;
                //Cerrada, si/no
                case 3:
                    System.Windows.Forms.RadioButton radYes = new System.Windows.Forms.RadioButton();
                    radYes.Top = (nroRadioBtn * 35) + heightLblPregunta;
                    radYes.Left = 100;
                    radYes.Size = new Size(maxWidthControl, 35);
                    radYes.Tag = new LstOpcionesRespuesta() { IdOpcionRespuesta = 1 };
                    radYes.Text = "Si";

                    pnlControls.Controls.Add(radYes);

                    nroRadioBtn += 1;

                    System.Windows.Forms.RadioButton radNo = new System.Windows.Forms.RadioButton();
                    radNo.Top = (nroRadioBtn * 35) + heightLblPregunta;
                    radNo.Left = 100;
                    radNo.Size = new Size(maxWidthControl, 35);
                    radNo.Tag = new LstOpcionesRespuesta() { IdOpcionRespuesta = 0 };
                    radNo.Text = "No";

                    pnlControls.Controls.Add(radNo);

                    break;
                //Cerrada, multiples respuestas multiples opciones
                case 4:
                    using (POSEntities db = new POSEntities())
                    {
                        var listOptions = db.LstOpcionesRespuesta.Where(x => x.Estado == true
                                                                                && x.IdPregunta == objPregunta.IdPregunta)
                                                                 .ToList();

                        foreach (var option in listOptions)
                        {
                            System.Windows.Forms.CheckBox chk = new System.Windows.Forms.CheckBox();
                            chk.Top = (nroRadioBtn * 35) + heightLblPregunta;
                            chk.Left = 100;
                            chk.Size = new Size(maxWidthControl, 35);
                            chk.Tag = option;
                            chk.Text = option.Descripcion;

                            pnlControls.Controls.Add(chk);

                            nroRadioBtn += 1;
                        }
                    }
                    break;
                default:
                    break;
            }


        }

        private void GrabarRespuesta()
        {
            try
            {
                //Identificar opcion seleccionada
                RadioButton radSelected = null;

                foreach (var ctrl in pnlControls.Controls)
                {
                    if (ctrl.GetType() == typeof(System.Windows.Forms.RadioButton))
                    {
                        if (((System.Windows.Forms.RadioButton)ctrl).Checked)
                        {
                            radSelected = (System.Windows.Forms.RadioButton)ctrl;
                            break;
                        }
                    }
                }

                //Si la pregunta no es de respuesta abierta ni opcion multiple, obligar a seleccionar un elemento
                if (objPregunta.TipoPregunta != 2 && objPregunta.TipoPregunta != 4)
                {
                    if (radSelected == null)
                    {
                        //MessageBox.Show("Seleccione una respuesta antes de continuar");
                        Control.Common.General.GetMensajeToList(376);
                        return;
                    }
                }

                //Si la pregunta es de tipo abierta el cajero debe haber escrito texto
                if (objPregunta.TipoPregunta == 2)
                {
                    if (txtRespuesta.Text.Trim().Length <= 4)
                    {
                        //MessageBox.Show("Recuerde ser descriptivo y escriba la respuesta recibida en la caja de texto antes de continuar");
                        Control.Common.General.GetMensajeToList(377);
                        return;
                    }
                }

                //Abrir encuesta
                if (objEncRealizada == null) AbrirEncuesta();

                if (objPregunta.TipoPregunta != 4)
                {
                    var obj = new TblEncuestaDetalle();
                    obj.IdOpcionRespuesta = (radSelected != null) ? ((LstOpcionesRespuesta)radSelected.Tag).IdOpcionRespuesta : (short)0;
                    obj.IdPregunta = objPregunta.IdPregunta;
                    obj.Respuesta = (radSelected != null) ? radSelected.Text : txtRespuesta.Text.Trim();

                    objEncRealizada.TblEncuestaDetalle.Add(obj);

                    //Modificar el indicador de siguiente pregunta de acuerdo a la opcion seleccionada
                    if (radSelected != null)
                    {
                        var optionSelected = (LstOpcionesRespuesta)radSelected.Tag;

                        if (optionSelected.OrdenPreguntaSig != null)
                        {
                            if (optionSelected.OrdenPreguntaSig != 0) minSiguientePregunta = (int)optionSelected.OrdenPreguntaSig;
                        }
                    }
                }
                else
                {
                    foreach (var ctrl in pnlControls.Controls)
                    {
                        if (ctrl.GetType() == typeof(System.Windows.Forms.CheckBox))
                        {
                            var ctrlSelected = (CheckBox)ctrl;
                            if (ctrlSelected.Checked)
                            {
                                var obj = new TblEncuestaDetalle();
                                obj.IdOpcionRespuesta = ((LstOpcionesRespuesta)ctrlSelected.Tag).IdOpcionRespuesta;
                                obj.IdPregunta = objPregunta.IdPregunta;
                                obj.Respuesta = ctrlSelected.Text;

                                objEncRealizada.TblEncuestaDetalle.Add(obj);
                            }
                        }
                    }
                }

                ContinuarCuestionario();
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

        private void ContinuarCuestionario()
        {
            if (GetNextPregunta())
                ArmarControlesPregunta();
            else
                Despedirse();
        }

        #endregion

        #region Support Methods

        private void Despedirse()
        {
            EncuestaHandler.EncuestaEnMemoria = objEncRealizada;

            var msjFinal = string.Empty;
            try
            {
                using (POSEntities db = new POSEntities())
                {
                    msjFinal = db.core_parametro.Where(x => x.identificador == "ENCUESTA_MENSAJEFINAL").FirstOrDefault().valor;
                }
            }
            catch (Exception)
            {
                msjFinal = "Gracias por sus respuestas, su ayuda es valiosa para nosotros";
            }
            MessageBox.Show(msjFinal);
            this.Close();
        }

        private void LimpiarPanelControles()
        {
            List<RadioButton> lst = new List<RadioButton>();
            List<CheckBox> lstChks = new List<CheckBox>();
            foreach (var ctrl in pnlControls.Controls)
            {
                if (ctrl.GetType() == typeof(RadioButton))
                {
                    lst.Add((RadioButton)ctrl);
                }
                else if (ctrl.GetType() == typeof(CheckBox))
                {
                    lstChks.Add((CheckBox)ctrl);
                }
            }

            foreach (RadioButton uc in lst)
            {
                uc.Dispose();
            }
            foreach (CheckBox uc in lstChks)
            {
                uc.Dispose();
            }
        }

        #endregion
    }
}
