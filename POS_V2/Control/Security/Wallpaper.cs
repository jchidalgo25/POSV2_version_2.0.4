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
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace POS.Control.Security
{
    public partial class Wallpaper : Telerik.WinControls.UI.RadForm
    {
        private int currentImageIndex = 0;
        private static List<Image> _cachedImages = new List<Image>();
        private static bool _isLoaded = false;

        public static bool IsLoaded => _isLoaded;

        public Wallpaper()
        {
            InitializeComponent();

            imageTimer = new Timer();
            imageTimer.Interval = 10000; // Cambiar imagen cada 2 segundos (2000 ms)
            imageTimer.Tick += ImageTimer_Tick;
            imageTimer.Start();

            CanClose = false;

            InitializeImages();

        }

        private void InitializeImages()
        {

            Control.Common.Logger.LogMessage(
                        Control.Common.Enum.LogTypes.Info,
                        "Wallpaper",
                        "InitializeImages",
                        "Inicializa Imagenes");


            try
            {
                var imagePaths = Control.Common.GlobalParameters.ListWallPapers.Select(w => w.Ruta).ToList();

                if (!Wallpaper.IsLoaded)
                {


                    Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                "Wallpaper",
                                "InitializeImages",
                                "Wallpaper.IsLoaded - Carga Imagen");


                    Wallpaper.LoadImages(imagePaths); // Cargar por primera vez
                }

                if (Wallpaper.Count > 0)
                {
                    currentImageIndex = 0;
                    pbWallpaper.Image?.Dispose();
                    pbWallpaper.Image = new Bitmap(Wallpaper.GetImage(currentImageIndex));
                }
                else
                {
                    pbWallpaper.Image = pbWallpaper.InitialImage;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "InitializeImages"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                pbWallpaper.Image = pbWallpaper.InitialImage;
            }
        }

        private void ImageTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                currentImageIndex = (currentImageIndex + 1) % Wallpaper.Count;
                LoadImage();
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "Wallpaper", "ImageTimer_Tick", ex.Message, ex.StackTrace);
                // Intenta con la siguiente
                currentImageIndex = (currentImageIndex + 1) % Wallpaper.Count;
            }

            //try
            //{
            //    // Incrementar índice
            //    currentImageIndex++;

            //    // Verificar si llegamos al final
            //    if (currentImageIndex >= Wallpaper.Count)
            //    {
            //        currentImageIndex = 0; // Reiniciar desde el inicio
            //    }

            //    // Cargar imagen desde la caché en memoria
            //    LoadImage();
            //}
            //catch (Exception ex)
            //{
            //    Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "ImageTimer_Tick"
            //        , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            //}
        }


        public static void LoadImages(List<string> imagePaths)
        {
            try
            {
                Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                "Wallpaper",
                                "InitializeImages",
                                "Limpiar caché previa");


                Clear(); // Limpiar caché previa

                foreach (var path in imagePaths)
                {
                    byte[] imageData = File.ReadAllBytes(path);
                    using (var ms = new MemoryStream(imageData))
                    {
                        var img = Image.FromStream(ms);
                        _cachedImages.Add(img);
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "WallpaperCache", "LoadImages"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);

                Clear();
                _isLoaded = false;
            }
        }
        public static void LoadImages(List<RutaImagen> imagePaths)
        {
            try
            {
                Control.Common.Logger.LogMessage(
                                Control.Common.Enum.LogTypes.Info,
                                "Wallpaper",
                                "InitializeImages",
                                "Limpiar caché previa");


                Clear(); // Limpiar caché previa

                foreach (var path in imagePaths)
                {



                    byte[] imageData = File.ReadAllBytes(path.Ruta);
                    using (var ms = new MemoryStream(imageData))
                    {
                        var img = Image.FromStream(ms);
                        _cachedImages.Add(img);
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "WallpaperCache", "LoadImages"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);

                Clear();
                _isLoaded = false;
            }
        }

        public static Image GetImage(int index)
        {
            if (!_isLoaded || index < 0 || index >= _cachedImages.Count)
                return null;

            return _cachedImages[index];
        }

        public static int Count => _cachedImages.Count;

        public static void Clear()
        {
            foreach (var img in _cachedImages)
            {
                img.Dispose();
            }
            _cachedImages.Clear();
            _isLoaded = false;
        }

        private void LoadImage()
        {


            try
            {
                if (Wallpaper.Count == 0) return;

                var newImage = new Bitmap(Wallpaper.GetImage(currentImageIndex)); // Copia para evitar problemas
                pbWallpaper.Image?.Dispose(); // Liberar la imagen anterior
                pbWallpaper.Image = newImage;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "LoadImage"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                pbWallpaper.Image = pbWallpaper.InitialImage;
            }


            /// pbWallpaper.ImageLocation = imagePaths[currentImageIndex];
        }


        private bool CanClose { get; set; }

        private void pbWallpaper_Click(object sender, EventArgs e)
        {
            try
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "POS.Control.Security.Wallpaper", "pbWallpaper_Click",
                                  "Ejcuta evento pbWallpaper_Click");


                // Si ya estamos verificando, ignoramos nuevas solicitudes
                if (_isVerifying)
                {
                    return;
                }

                // Bloquear nuevas ejecuciones
                _isVerifying = true;


                VerifyCanClose();
                this.Close();
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "pbWallpaper_Click"
                     , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }

        }



        private bool _isVerifying = false;

        private void TryStartVerification()
        {
            if (_isVerifying) return;

            _isVerifying = true;

            try
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "Wallpaper", "TryStartVerification", "Iniciando desbloqueo");
                VerifyCanClose();
            }
            catch
            {
                _isVerifying = false;
                throw;
            }
        }

      

  
        

        private void VerifyCanClose()
        {
            try
            {

                //Control.Common.GlobalParameters.UserObj.isSuperUser = true;

                if (Control.Common.GlobalParameters.UserObj?.isSuperUser == true)
                {
                    CanClose = true;
                    _isVerifying = false; // ✅ Aquí sí liberamos para nuevos intentos
                    return;
                }

                Thread t = new Thread(() =>
                {
                    DialogResult resultado = DialogResult.Cancel;
                    try
                    {
                        using (var verifier = new Fingerprint.VerificationForm(Control.Common.GlobalParameters.DataForFingerprint, null))
                        {
                            verifier.Tag = "usr";
                            verifier.StartPosition = FormStartPosition.CenterScreen;
                            resultado = verifier.ShowDialog(); // Ahora en STA, debería funcionar
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "VerificationThread", "Run", ex.ToString(), "");
                    }

                    // ✅ Usa Invoke para actualizar CanClose desde el hilo principal
                    this.Invoke(new Action(() =>
                    {
                        if (resultado == DialogResult.OK)
                        {
                            CanClose = true;
                            _isVerifying = false; // ✅ Aquí sí liberamos para nuevos intentos

                        }
                        else
                        {
                            CanClose = false;
                            _isVerifying = false; // ✅ Aquí sí liberamos para nuevos intentos
                            Control.Common.General.GetMensajeToList(541);
                        }

                        // Y ahora sí permite cerrar
                        this.Close(); // O desbloquea el cierre
                    }));
                });

                t.SetApartmentState(ApartmentState.STA);
                t.Start();

           

            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "Wallpaper", "VerifyCanClose", ex.ToString(), ex.StackTrace);
            }
        }


        //private void VerifyCanClose()
        //{

        //    try
        //    {
        //        if (Control.Common.GlobalParameters.UserObj.isSuperUser)
        //        {
        //            CanClose = true;
        //        }
        //        else
        //        {
        //            Fingerprint.VerificationForm Verifier = new Fingerprint.VerificationForm(Control.Common.GlobalParameters.DataForFingerprint, null);
        //            DialogResult verificador;
        //            Verifier.Tag = "usr";
        //            verificador = Verifier.ShowDialog();
        //            if (verificador == DialogResult.OK)
        //            {

        //                CanClose = true;
        //            }
        //            else
        //            {
        //                CanClose = false;

        //                Common.Logger.LogMessage(Common.Enum.LogTypes.Info, "POS.Control.Security.Wallpaper", "VerifyCanClose",
        //                           "Estimado usuario, POS solo puede ser desbloqueado por la persona que aperturo la caja. Si ud es quien aperturó por favor inténtelo nuevament");


        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "VerifyCanClose",
        //                   $"Error: {ex.Message}, {ex.StackTrace}");
        //    }

        //}



        //private void VerifyCanClose()
        //{
        //    try
        //    {
        //        if (Control.Common.GlobalParameters.UserObj.isSuperUser)
        //        {
        //            CanClose = true;
        //        }
        //        else
        //        {
        //            DialogResult resultado = DialogResult.None;

        //            // Invocar directamente el formulario de verificación en el mismo hilo de la UI
        //            using (var verifier = new Fingerprint.VerificationForm(Control.Common.GlobalParameters.DataForFingerprint, null))
        //            {
        //                verifier.Tag = "usr";
        //                resultado = verifier.ShowDialog();
        //            }

        //            if (resultado == DialogResult.OK)
        //            {
        //                CanClose = true;
        //            }
        //            else
        //            {
        //                CanClose = false;
        //                Control.Common.General.GetMensajeToList(541);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "VerifyCanClose",
        //            Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
        //    }
        //}


        private void Wallpaper_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Alt && e.KeyCode == Keys.F4)
                    VerifyCanClose();

                //Para que staff de sistemas pueda cerrar el wallpaper
                if (e.Control && e.KeyCode == Keys.F10)
                {
                    bool autorizado = false;
                    using (Control.Auth.CredentialAuth frmAuth = new Control.Auth.CredentialAuth("Ingrese credenciales administrativas para confirmar su identidad"))
                    {
                        frmAuth.ShowDialog();
                        autorizado = frmAuth.EsAutorizado;
                    }

                    if (autorizado)
                    {
                        CanClose = true;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "Wallpaper_KeyDown"
                     , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }

        }

        private void Wallpaper_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                if (!CanClose)
                {
                    e.Cancel = true;
                    return;
                }

                pbWallpaper.Image?.Dispose(); // Solo limpiamos lo local, la caché persiste
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "Wallpaper_FormClosing"
                     , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }

        }
        private void Wallpaper_Load(object sender, EventArgs e)
        {
            try
            {
                LoadImage();

            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "Wallpaper_Load"
                     , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }
            // GetWallpaper();

        }

        //private void GetWallpaper()
        //{
        //    try
        //    {
        //        //Debemos controlar que el archivo no quede tomado para poderlo reemplazar con facilidad

        //        //Limpiar referencias
        //        var image = pbWallpaper.Image;
        //        pbWallpaper.Image = null;
        //        image.Dispose();

        //        //Prevenir que el constructor, y por tanto el aplicativo, retenga referencia al archivo fisico.
        //        //Esto se realiza pasandole la imagen por stream
        //        //var img = Image.FromStream(new System.IO.MemoryStream(File.ReadAllBytes(@"\\srvatila\Shares\Compartido\ImgPOS\Wallpaper.jpg")));

        //        var img = Image.FromStream(new System.IO.MemoryStream(File.ReadAllBytes(@Control.Common.GlobalParameters.WallpaperLocal)));
        //        pbWallpaper.Image = img;


        //    }
        //    catch (Exception ex)
        //    {
        //        Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "GetWallpaper", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
        //        pbWallpaper.Image = pbWallpaper.InitialImage;
        //    }
        //}

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            var db = new POSEntities();
            var strtecla = keyData.ToString();
            var parkey = db.core_parametro
                .FirstOrDefault(x => x.identificador == "POS_KEYBOARD" && x.valor == strtecla);

            if (parkey == null) return false;

            switch (parkey.parametro2)
            {
                case "GRABARFACTURA":
                    return true;

                case "EFECTIVOEXACTO":
                    return true;

                case "REIMPRIMIRVOUCHER":
                    return true;

                case "REIMPRIMIRFACTURA":
                    return true;

                case "CONSULTAPRECIO":
                    return true;

                case "CANTIDADPRODUCTO":
                    return true;

                case "PAGOTCREDITO":
                    return true;

                case "CONSUMIDORFINAL":
                    return true;

                case "PAGOCHEQUE":
                    return true;

                case "PAGOSERVICIO":
                    return true;

                case "FORMAPAGO":
                    return true;

                case "ADMINISTRADOR":
                    return true;
                default:
                    return false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

    }
}
