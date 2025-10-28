using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Text;
using ZXing;
using ZXing.Common;

namespace DSS.Controles.Impresion
{
    public class DSSPrint : System.Drawing.Printing.PrintDocument
    {
        private int _barcodeWidth = 300;
        private int _barcodeHeight = 100;
        private readonly object _imageLock = new object();
        private Image _cachedImage = null;
        private string _cachedImagePath = null;


        public int BarcodeWidth
        {
            get { return _barcodeWidth; }
            set { _barcodeWidth = value; }
        }

        public int BarcodeHeight
        {
            get { return _barcodeHeight; }
            set { _barcodeHeight = value; }
        }

        private Font _font;

        private Font _font_bold;

        private string _text;

        private string[] _lineas;

        private int lineas_impresas;

        private int? _yLineSpacing;

        public int? YLineSpacing
        {
            get { return _yLineSpacing; }
            set { _yLineSpacing = value; }
        }

        public string TextToPrint
        {
            get { return _text; }
            set { _text = value; }
        }

        private int _font_size_bold;

        public int FontSizeBold
        {
            get
            {
                if (this._font_size_bold == 0)
                    return 2;
                return this._font_size_bold;
            }
            set { this._font_size_bold = value; }
        }

        public Font PrinterFont
        {
            get { return _font; }
            set { _font = value; _font_bold = new Font(_font.Name, _font.Size + this.FontSizeBold, FontStyle.Bold); }
        }

        public DSSPrint()
            : base()
        {
            _text = string.Empty;
        }

        public DSSPrint(string value)
            : base()
        {
            _text = value;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);

            if (_font == null)
            {
                _font = new Font("Times New Roman", 10);
                _font_bold = new Font("Times New Roman", 10, FontStyle.Bold);
            }

            char[] param = { '\n' };

            this._lineas = this._text.Split(param);

            int i = 0;
            char[] trimParam = { '\r' };
            foreach (string s in this._lineas)
            {
                this._lineas[i++] = s.TrimEnd(trimParam);
            }


        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);

            int x = 3;
            int y = e.MarginBounds.Top;

            while (lineas_impresas < this._lineas.Length)
            {
                string linea_a_imprimir = this._lineas[lineas_impresas++];


                if (linea_a_imprimir.StartsWith("<b>") && linea_a_imprimir.EndsWith("</b>"))
                {
                    linea_a_imprimir = linea_a_imprimir.Replace("<b>", "").Replace("</b>", "");
                    e.Graphics.DrawString(linea_a_imprimir, this._font_bold, Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<bfactura>") && linea_a_imprimir.EndsWith("</bfactura>"))
                {
                    linea_a_imprimir = linea_a_imprimir.Replace("<bfactura>", "").Replace("</bfactura>", "");
                    e.Graphics.DrawString(linea_a_imprimir, new Font("Arial", 12, FontStyle.Bold), Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<barcode>") && linea_a_imprimir.EndsWith("</barcode>"))
                {
                    string codigo = linea_a_imprimir.Replace("<barcode>", "").Replace("</barcode>", "").Trim();
                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        try
                        {
                            var barcodeWriter = new BarcodeWriter
                            {
                                Format = BarcodeFormat.CODE_128,
                                Options = new EncodingOptions { Width = 250, Height = 50, Margin = 3 }
                            };

                            Bitmap barcodeBitmap = barcodeWriter.Write(codigo);
                            e.Graphics.DrawImage(barcodeBitmap, x, y, barcodeBitmap.Width, barcodeBitmap.Height);
                            y += barcodeBitmap.Height + 5;
                        }
                        catch (Exception ex)
                        {
                            e.Graphics.DrawString("[Error al generar código de barras: " + ex.Message + "]", this._font, Brushes.Red, x, y);
                            y += (YLineSpacing == null) ? 15 : (int)YLineSpacing;
                        }
                    }
                    else
                    {
                        e.Graphics.DrawString("[Código de barras vacío]", this._font_bold, Brushes.Red, x, y);
                        y += (YLineSpacing == null) ? 15 : (int)YLineSpacing;
                    }
                }
                else if (linea_a_imprimir.StartsWith("<footer>") && linea_a_imprimir.EndsWith("</footer>"))
                {
                    linea_a_imprimir = linea_a_imprimir.Replace("<footer>", "").Replace("</footer>", "");
                    e.Graphics.DrawString(linea_a_imprimir, new Font("COURIER NEW", 16, FontStyle.Bold), Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<h2>") && linea_a_imprimir.EndsWith("</h2>"))
                {
                    linea_a_imprimir = linea_a_imprimir.Replace("<h2>", "").Replace("</h2>", "");
                    e.Graphics.DrawString(linea_a_imprimir, new Font("Eras Bold ITC", 38, FontStyle.Italic), Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<h1>") && linea_a_imprimir.EndsWith("</h1>"))
                {
                    linea_a_imprimir = linea_a_imprimir.Replace("<h1>", "").Replace("</h1>", "");
                    e.Graphics.DrawString(linea_a_imprimir, new Font("Eras Bold ITC", 30, FontStyle.Bold), Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<zoom:") && linea_a_imprimir.EndsWith("</zoom>"))
                {
                    string strValue = "";
                    if ((linea_a_imprimir.IndexOf(">") + 1) > linea_a_imprimir.IndexOf(":"))
                        strValue = linea_a_imprimir.Substring(linea_a_imprimir.IndexOf(":") + 1,
                            (linea_a_imprimir.IndexOf(">") - 1) - linea_a_imprimir.IndexOf(":"));

                    string tagInicio = "<zoom:" + strValue + ">";
                    int value = 0;
                    int.TryParse(strValue, out value);

                    linea_a_imprimir = linea_a_imprimir.Replace(tagInicio, "").Replace("</zoom>", "");
                    e.Graphics.DrawString(linea_a_imprimir,
                        new Font(_font.Name, _font.Size + value, _font.Style), Brushes.Black, x, y);
                }

                else if (linea_a_imprimir.StartsWith("<titulo") && linea_a_imprimir.EndsWith("</titulo>"))
                {
                    string contenido = ExtraerContenido(linea_a_imprimir, "titulo");
                    if (!string.IsNullOrWhiteSpace(contenido))
                        e.Graphics.DrawString(contenido, this._font_bold, Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<info") && linea_a_imprimir.EndsWith("</info>"))
                {
                    string contenido = ExtraerContenido(linea_a_imprimir, "info");
                    if (!string.IsNullOrWhiteSpace(contenido))
                        e.Graphics.DrawString(contenido, this._font, Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<legales") && linea_a_imprimir.EndsWith("</legales>"))
                {
                    string contenido = ExtraerContenido(linea_a_imprimir, "legales");
                    if (!string.IsNullOrWhiteSpace(contenido))
                        e.Graphics.DrawString(contenido, new Font(_font.Name, _font.Size - 2, FontStyle.Regular), Brushes.Black, x, y);
                }
                else if (linea_a_imprimir.StartsWith("<img"))
                {
                    string tagPart = linea_a_imprimir;  // apertura con atributos
                    string contenido = "";

                    // Caso 1: apertura y cierre en la MISMA línea
                    if (linea_a_imprimir.Contains("</img>"))
                    {
                        int startContent = linea_a_imprimir.IndexOf('>') + 1;
                        int endContent = linea_a_imprimir.LastIndexOf("</img>");
                        contenido = linea_a_imprimir.Substring(startContent, endContent - startContent).Trim();
                    }
                    else
                    {
                        // leer líneas hasta encontrar </img>
                        while (lineas_impresas < _lineas.Length && !_lineas[lineas_impresas].Contains("</img>"))
                        {
                            contenido += _lineas[lineas_impresas++].Trim();
                        }

                        // avanzar la línea de cierre </img>
                        if (lineas_impresas < _lineas.Length && _lineas[lineas_impresas].Contains("</img>"))
                            lineas_impresas++;
                    }


                    contenido = contenido.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

                    if (File.Exists(contenido))
                    {
                        using (Image img = Image.FromFile(contenido))
                        {
                            ImgAttributes parsed = ParseImgAttributes(tagPart, img);

                            int imgX = parsed.CustomX.HasValue ? parsed.CustomX.Value : x;
                            if (!parsed.CustomX.HasValue)
                            {
                                if (parsed.Align == "center")
                                    imgX = (e.MarginBounds.Width - parsed.Width) / 2;
                                else if (parsed.Align == "right")
                                    imgX = e.MarginBounds.Width - parsed.Width - 5;
                            }

                            e.Graphics.DrawImage(img, new Rectangle(imgX, y, parsed.Width, parsed.Height));
                            y += parsed.Height + 5;
                        }
                    }
                    else
                    {
                        e.Graphics.DrawString("[img: no encontrada]", _font, Brushes.Red, x, y);
                        y += 20;
                    }


                }

                else if (linea_a_imprimir.StartsWith("<logo"))
                {
                    string contenido = "";

                    // Caso 1: apertura y cierre en la misma línea
                    if (linea_a_imprimir.Contains("</logo>"))
                    {
                        contenido = ExtraerContenido(linea_a_imprimir, "logo");
                    }
                    else
                    {
                        // Caso 2: multilínea
                        StringBuilder sb = new StringBuilder();
                        while (lineas_impresas < _lineas.Length && !_lineas[lineas_impresas].Contains("</logo>"))
                        {
                            sb.AppendLine(_lineas[lineas_impresas].Trim());
                            lineas_impresas++;
                        }
                        if (lineas_impresas < _lineas.Length && _lineas[lineas_impresas].Contains("</logo>"))
                            lineas_impresas++;

                        contenido = sb.ToString().Trim();
                    }

                    if (!string.IsNullOrWhiteSpace(contenido))
                    {
                        try
                        {
                            Image img = null;

                            // 🔹 Validamos primero si es Base64 (solo caracteres válidos + longitud múltiplo de 4)
                            bool esBase64 = (contenido.Length % 4 == 0) &&
                                            System.Text.RegularExpressions.Regex.IsMatch(contenido, @"^[a-zA-Z0-9\+/]*={0,3}$");

                            if (esBase64)
                            {
                                try
                                {
                                    byte[] imageBytes = Convert.FromBase64String(contenido);
                                    using (var ms = new MemoryStream(imageBytes))
                                    {
                                        img = Image.FromStream(ms);
                                    }
                                }
                                catch
                                {
                                    e.Graphics.DrawString("[Logo Base64 inválido]", this._font_bold, Brushes.Red, x, y);
                                }
                            }
                            else if (File.Exists(contenido))
                            {
                                img = Image.FromFile(contenido);
                            }
                            else
                            {
                                e.Graphics.DrawString("[Ruta de logo inválida]", this._font_bold, Brushes.Red, x, y);
                            }

                            if (img != null)
                            {
                                PrintImage(img, e.Graphics, ref y, e.MarginBounds.Width, x, 0, 0);
                                img.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            e.Graphics.DrawString("[Error al cargar logo: " + ex.Message + "]", this._font, Brushes.Red, x, y);
                        }
                    }
                }

                else
                {
                    e.Graphics.DrawString(linea_a_imprimir, this._font, Brushes.Black, x, y);
                }

                // Avanzar verticalmente
                y += (YLineSpacing == null) ? 15 : (int)YLineSpacing;

                if (y >= e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }

            }

            lineas_impresas = 0;
            e.HasMorePages = false;
        }

        // --------------------------------------------------------------------
        // Estructura para compatibilidad con .NET 3.5 (en lugar de tuplas)
        // --------------------------------------------------------------------
        private struct ImgAttributes
        {
            public int Width;
            public int Height;
            public int? CustomX;
            public string Align;
        }

        private ImgAttributes ParseImgAttributes(string tagPart, Image img)
        {
            ImgAttributes attrs = new ImgAttributes();
            attrs.Width = img.Width;
            attrs.Height = img.Height;
            attrs.CustomX = null;
            attrs.Align = "left";

            System.Text.RegularExpressions.Match widthMatch = System.Text.RegularExpressions.Regex.Match(tagPart, "width\\s*=\\s*\"(\\d+)\"");
            int w;
            if (widthMatch.Success && int.TryParse(widthMatch.Groups[1].Value, out w) && w > 0)
                attrs.Width = w;

            System.Text.RegularExpressions.Match heightMatch = System.Text.RegularExpressions.Regex.Match(tagPart, "height\\s*=\\s*\"(\\d+)\"");
            int h;
            if (heightMatch.Success && int.TryParse(heightMatch.Groups[1].Value, out h) && h > 0)
                attrs.Height = h;
            else if (widthMatch.Success)
                attrs.Height = (int)((double)img.Height * attrs.Width / img.Width);

            System.Text.RegularExpressions.Match xMatch = System.Text.RegularExpressions.Regex.Match(tagPart, "x\\s*=\\s*\"(\\d+)\"");
            int xVal;
            if (xMatch.Success && int.TryParse(xMatch.Groups[1].Value, out xVal))
                attrs.CustomX = xVal;

            System.Text.RegularExpressions.Match alignMatch = System.Text.RegularExpressions.Regex.Match(tagPart, "align\\s*=\\s*\"(\\w+)\"");
            if (alignMatch.Success)
                attrs.Align = alignMatch.Groups[1].Value.ToLower();

            return attrs;
        }







        private Image GetCachedImage(string path)
        {
            lock (_imageLock) // Seguro para múltiples hilos (opcional si imprimes en UI thread)
            {
                try
                {
                    // Si la ruta es diferente, liberar caché anterior
                    if (_cachedImage != null && _cachedImagePath != path)
                    {
                        _cachedImage.Dispose();
                        _cachedImage = null;
                        _cachedImagePath = null;
                    }

                    // Si no está en caché, cargarla
                    if (_cachedImage == null)
                    {
                        byte[] imageBytes = File.ReadAllBytes(path);
                        var ms = new MemoryStream(imageBytes);
                        _cachedImage = Image.FromStream(ms);
                        _cachedImagePath = path;
                    }

                    // Devolver clon para evitar "El objeto está ocupado en otro proceso"
                    return (Image)_cachedImage.Clone();
                }
                catch
                {
                    return null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedImage?.Dispose();
                _cachedImage = null;
                _cachedImagePath = null;
            }
            base.Dispose(disposing);
        }

        private void PrintImage(Image img, Graphics g, ref int y, int maxWidth, int x, int customWidth = 0, int customHeight = 0)
        {
            int imgWidth = customWidth > 0 ? customWidth : img.Width;
            int imgHeight = customHeight > 0 ? customHeight : img.Height;

            // Escalado si no se dio tamaño personalizado
            if (customWidth == 0 && imgWidth > maxWidth)
            {
                double ratio = (double)maxWidth / imgWidth;
                imgWidth = maxWidth;
                imgHeight = (int)(imgHeight * ratio);
            }

            g.DrawImage(img, new Rectangle(x, y, imgWidth, imgHeight));
            y += imgHeight + 10;
        }


        private string ExtraerContenido(string linea, string tagName)
        {
            try
            {
                // Buscar la posición de cierre del ">"
                int inicioContenido = linea.IndexOf(">") + 1;
                int finContenido = linea.LastIndexOf("</" + tagName + ">");
                if (inicioContenido >= 0 && finContenido > inicioContenido)
                {
                    return linea.Substring(inicioContenido, finContenido - inicioContenido).Trim();
                }
            }
            catch { }
            return string.Empty;
        }




    }
}