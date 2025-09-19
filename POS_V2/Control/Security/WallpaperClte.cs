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


namespace POS.Control.Security
{
    public partial class WallpaperClte : Telerik.WinControls.UI.RadForm
    {

        private int currentImageIndex = 0;
        private static List<Image> _cachedImages = new List<Image>();
        private static bool _isLoaded = false;

        public static bool IsLoaded => _isLoaded;

        public WallpaperClte()
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
            try
            {
                var imagePaths = Control.Common.GlobalParameters.ListWallPapers.Select(w => w.Ruta).ToList();

                if (!WallpaperClte.IsLoaded)
                {
                    WallpaperClte.LoadImages(imagePaths); // Cargar por primera vez
                }

                if (WallpaperClte.Count > 0)
                {
                    currentImageIndex = 0;
                    pbWallpaperClte.Image?.Dispose();
                    pbWallpaperClte.Image = new Bitmap(WallpaperClte.GetImage(currentImageIndex));
                }
                else
                {
                    pbWallpaperClte.Image = pbWallpaperClte.InitialImage;
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.WallpaperClte", "InitializeImages"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                pbWallpaperClte.Image = pbWallpaperClte.InitialImage;
            }
        }

        private void ImageTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Incrementar índice
                currentImageIndex++;

                // Verificar si llegamos al final
                if (currentImageIndex >= Wallpaper.Count)
                {
                    currentImageIndex = 0; // Reiniciar desde el inicio
                }

                // Cargar imagen desde la caché en memoria
                LoadImage();
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.WallpaperClte", "ImageTimer_Tick"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }
        }


        public static void LoadImages(List<string> imagePaths)
        {
            try
            {
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
                if (WallpaperClte.Count == 0) return;

                var newImage = new Bitmap(WallpaperClte.GetImage(currentImageIndex)); // Copia para evitar problemas
                pbWallpaperClte.Image?.Dispose(); // Liberar la imagen anterior
                pbWallpaperClte.Image = newImage;
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "LoadImage"
                    , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                pbWallpaperClte.Image = pbWallpaperClte.InitialImage;
            }


            /// pbWallpaper.ImageLocation = imagePaths[currentImageIndex];
        }


        private bool CanClose { get; set; }

        private void pbWallpaper_Click(object sender, EventArgs e)
        {
            try
            {
                VerifyCanClose();
                this.Close();
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.WallpaperClte", "pbWallpaperClte_Click"
                     , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }

        }

        private void VerifyCanClose()
        {

            try
            {
                if (Control.Common.GlobalParameters.UserObj.isSuperUser)
                {
                    CanClose = true;
                }
                else
                {
                    Fingerprint.VerificationForm Verifier = new Fingerprint.VerificationForm(Control.Common.GlobalParameters.DataForFingerprint, null);
                    DialogResult verificador;
                    Verifier.Tag = "usr";
                    verificador = Verifier.ShowDialog();
                    if (verificador == DialogResult.OK)
                    {
                        CanClose = true;
                    }
                    else
                    {
                        CanClose = false;
                        Control.Common.General.GetMensajeToList(541);
                        //MessageBox.Show("Estimado usuario, POS solo puede ser desbloqueado por la persona que aperturo la caja. Si ud es quien aperturó por favor inténtelo nuevamente");
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "VerifyCanClose"
                     , Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
            }

        }

        private void WallpaperClte_KeyDown(object sender, KeyEventArgs e)
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

        private void WallpaperClte_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CanClose)
            {
                e.Cancel = true;
                return;
            }

            pbWallpaperClte.Image?.Dispose(); // Solo limpiamos lo local, la caché persiste
        }
        private void WallpaperClte_Load(object sender, EventArgs e)
        {
            // GetWallpaper();
            LoadImage();

        }

        private void GetWallpaper()
        {
            try
            {
                //Debemos controlar que el archivo no quede tomado para poderlo reemplazar con facilidad

                //Limpiar referencias
                var image = pbWallpaperClte.Image;
                pbWallpaperClte.Image = null;
                image.Dispose();

                //Prevenir que el constructor, y por tanto el aplicativo, retenga referencia al archivo fisico.
                //Esto se realiza pasandole la imagen por stream
                //var img = Image.FromStream(new System.IO.MemoryStream(File.ReadAllBytes(@"\\srvatila\Shares\Compartido\ImgPOS\Wallpaper.jpg")));

                var img = Image.FromStream(new System.IO.MemoryStream(File.ReadAllBytes(@Control.Common.GlobalParameters.WallpaperLocal)));
                pbWallpaperClte.Image = img;


            }
            catch (Exception ex)
            {
                Common.Logger.LogMessage(Common.Enum.LogTypes.Error, "POS.Control.Security.Wallpaper", "GetWallpaper", Common.ExceptionHandler.GetExceptionMessages(ex), string.Empty);
                pbWallpaperClte.Image = pbWallpaperClte.InitialImage;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

    }
}
