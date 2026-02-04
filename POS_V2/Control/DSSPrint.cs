using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ZXing;
using ZXing.Common;

namespace DSS.Controles.Impresion
{
    public class DSSPrint : System.Drawing.Printing.PrintDocument
    {
        #region Propiedades y Campos
        private Font _font;
        private Font _font_bold;
        private string _text;
        private string[] _lineas;
        private int lineas_impresas;

        // ✅ PROPIEDADES RESTAURADAS PARA COMPATIBILIDAD
        private int? _yLineSpacing;
        public int? YLineSpacing { get => _yLineSpacing; set => _yLineSpacing = value; }

        private int _font_size_bold;
        public int FontSizeBold
        {
            get { if (this._font_size_bold == 0) return 2; return this._font_size_bold; }
            set { this._font_size_bold = value; }
        }

        public Font PrinterFont
        {
            get { return _font; }
            set { _font = value; _font_bold = new Font(_font.Name, _font.Size + this.FontSizeBold, FontStyle.Bold); }
        }
        // FIN DE PROPIEDADES RESTAURADAS

        public string TextToPrint { get => _text; set => _text = value; }
        #endregion

        #region Clases Internas y Constructores
        private class ParsedTag
        {
            public string Name { get; set; }
            public string Content { get; set; }
            public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public string GetAttribute(string key) => Attributes.TryGetValue(key, out string value) ? value : string.Empty;
        }

        private struct ImgAttributes { public int Width, Height; public int? CustomX; public string Align; }
        public DSSPrint() : base() { _text = string.Empty; }
        public DSSPrint(string value) : base() { _text = value; }
        #endregion

        #region Lógica de Impresión Principal
        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            if (_font == null)
            {
                _font = new Font("Courier New", 10);
                _font_bold = new Font("Courier New", 10, FontStyle.Bold);
            }
            this._lineas = this._text.Split(new char[] { '\n' });
            for (int i = 0; i < this._lineas.Length; i++) { this._lineas[i] = this._lineas[i].TrimEnd('\r'); }
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            int x = 3;
            int y = 5;
            int ticketWidth = 275;

            while (lineas_impresas < this._lineas.Length)
            {
                if (lineas_impresas >= _lineas.Length) break;
                string linea_a_imprimir = this._lineas[lineas_impresas++];

                if (string.IsNullOrWhiteSpace(linea_a_imprimir)) { y += 10; continue; }

                if (linea_a_imprimir.StartsWith("<img")) { ProcesarImagen_MetodoAntiguo(e, linea_a_imprimir, x, ref y); }
                else if (linea_a_imprimir.StartsWith("<zoom:")) { ProcesarZoom_MetodoAntiguo(e, linea_a_imprimir, x, y); y += (int)e.Graphics.MeasureString("M", _font).Height; }
                else if (linea_a_imprimir.StartsWith("<logowithinfo>")) { ProcesarLogoConInfo(e, linea_a_imprimir, x, ticketWidth, ref y); }
                else
                {
                    ParsedTag parsedTag = ParseFullTag(linea_a_imprimir);
                    if (parsedTag != null)
                    {
                        string alignAttribute = parsedTag.GetAttribute("align");
                        switch (parsedTag.Name.ToLower())
                        {
                            case "titulo": DrawStringWrapped(e.Graphics, parsedTag.Content, new Font(_font.Name, 14, FontStyle.Bold), ticketWidth, ref y, alignAttribute); break;
                            case "info": DrawStringWrapped(e.Graphics, parsedTag.Content, new Font(_font.Name, 10, FontStyle.Regular), ticketWidth, ref y, alignAttribute); break;
                            case "legales": DrawStringWrapped(e.Graphics, parsedTag.Content, new Font(_font.Name, 8, FontStyle.Regular), ticketWidth, ref y, alignAttribute); break;
                            case "logo": ProcesarLogo_MetodoNuevo(e, parsedTag, x, ticketWidth, ref y); break;
                            case "barcode":
                                ProcesarBarcode(e, parsedTag, x, ticketWidth, ref y);
                                break;
                            case "b": e.Graphics.DrawString(parsedTag.Content, _font_bold, Brushes.Black, x, y); y += (int)_font_bold.GetHeight(e.Graphics); break;
                            default: e.Graphics.DrawString(linea_a_imprimir, _font, Brushes.Black, x, y); y += (int)_font.GetHeight(e.Graphics); break;
                        }
                    }
                    else
                    {
                        e.Graphics.DrawString(linea_a_imprimir, _font, Brushes.Black, x, y);
                        y += (int)_font.GetHeight(e.Graphics);
                    }
                }

                y += 5;
                if (y >= e.MarginBounds.Bottom) { e.HasMorePages = true; return; }
            }
            lineas_impresas = 0;
            e.HasMorePages = false;
        }
        #endregion

        #region Métodos Auxiliares

        private void DrawStringWrapped(Graphics g, string text, Font font, int maxWidth, ref int y, string align = "left", int startX = 3)
        {
            if (string.IsNullOrEmpty(text))
            {
                // Si el texto es nulo o vacío, salimos inmediatamente.
                return;
            }

            // ✅ 1. Decodificar el marcador temporal del SP a un salto de línea real (\n)
            // Esto asegura que la intención de salto de línea del usuario sea respetada.
            text = text.Replace("@@NL@@", Environment.NewLine);
            text = text.Replace("@@NL@@", Environment.NewLine);

            // 2. Dividir el texto en líneas basándose en los saltos de línea del usuario
            // (Ahora que el marcador @@NL@@ fue convertido a \n)
            string[] userLines = text.Split(new char[] { '\n' }, StringSplitOptions.None);

            // Lista final de líneas a imprimir
            var finalLines = new List<string>();

            // 3. Para cada línea introducida por el usuario, aplicar Word Wrap si es necesario
            foreach (var userLine in userLines)
            {
                // WrapText ahora solo se usa para envolver líneas demasiado largas
                finalLines.AddRange(WrapText(g, userLine, font, maxWidth));
            }

            // 4. Dibujar cada línea final (ahora en finalLines)
            foreach (var line in finalLines)
            {
                int x = startX;

                // Calcular la posición X para la alineación
                if (align == "center")
                {
                    var size = g.MeasureString(line, font);
                    // x se calcula para centrar la línea dentro del ancho disponible (maxWidth)
                    x = startX + (maxWidth - (int)size.Width) / 2;
                    if (x < startX) x = startX; // Asegurar que no se salga del margen izquierdo
                }

                // Dibujar la línea
                g.DrawString(line, font, Brushes.Black, x, y);

                // Mover la posición Y hacia abajo para la siguiente línea
                y += (int)g.MeasureString(line, font).Height;
            }
        }

        private List<string> WrapText(Graphics g, string text, Font font, float maxWidth)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(text)) return lines;
            var words = text.Split(' ');
            var currentLine = new StringBuilder();
            foreach (var word in words)
            {
                var testLine = currentLine.Length > 0 ? currentLine.ToString() + " " + word : word;
                if (g.MeasureString(testLine, font).Width > maxWidth)
                {
                    if (currentLine.Length > 0) lines.Add(currentLine.ToString());
                    currentLine = new StringBuilder(word);
                }
                else { if (currentLine.Length > 0) currentLine.Append(" "); currentLine.Append(word); }
            }
            if (currentLine.Length > 0) lines.Add(currentLine.ToString());
            return lines;
        }

        private void ProcesarLogoConInfo(PrintPageEventArgs e, string rawContent, int defaultX, int ticketWidth, ref int y)
        {
            Match logoMatch = Regex.Match(rawContent, @"<logo>(.*?)</logo>");
            Match infoMatch = Regex.Match(rawContent, @"<info>(.*?)</info>");
            if (!logoMatch.Success || !infoMatch.Success) return;

            string logoContent = logoMatch.Groups[1].Value;
            string infoContent = infoMatch.Groups[1].Value;

            try
            {
                using (Image logoImg = LoadImage(logoContent))
                {
                    if (logoImg == null) return;

                    // --- ✅ LÓGICA DE ESCALADO Y ALINEACIÓN FINAL ---

                    // 1. Definir tamaño deseado del logo y calcular su nuevo ancho
                    int desiredLogoHeight = 50; // Altura fija para el logo
                    double aspectRatio = (double)logoImg.Width / logoImg.Height;
                    int scaledLogoWidth = (int)(desiredLogoHeight * aspectRatio);

                    // 2. Definir área de trabajo para el texto
                    Font infoFont = new Font(_font.Name, 10, FontStyle.Regular);
                    int textX = defaultX + scaledLogoWidth + 10;
                    int textMaxWidth = ticketWidth - textX;
                    int startY = y;

                    // 3. Medir la altura total del texto
                    var textLines = WrapText(e.Graphics, infoContent, infoFont, textMaxWidth);
                    float totalTextHeight = 0;
                    foreach (var line in textLines) { totalTextHeight += e.Graphics.MeasureString(line, infoFont).Height; }

                    // 4. Calcular el bloque y las posiciones verticales
                    float blockHeight = Math.Max(desiredLogoHeight, totalTextHeight);
                    int logoDrawY = startY + (int)((blockHeight - desiredLogoHeight) / 2);
                    int textDrawY = startY + (int)((blockHeight - totalTextHeight) / 2);

                    // 5. Dibujar el logo ESCALADO en su posición correcta
                    e.Graphics.DrawImage(logoImg, new Rectangle(defaultX, logoDrawY, scaledLogoWidth, desiredLogoHeight));

                    // 6. Dibujar el texto en su posición correcta
                    int currentTextY = textDrawY;
                    foreach (var line in textLines)
                    {
                        e.Graphics.DrawString(line, infoFont, Brushes.Black, textX, currentTextY);
                        currentTextY += (int)e.Graphics.MeasureString(line, infoFont).Height;
                    }

                    // 7. Actualizar la posición 'y' global
                    y = startY + (int)blockHeight;
                }
            }
            catch { e.Graphics.DrawString("[Error en LogoConInfo]", _font, Brushes.Red, defaultX, y); }
        }

        private Image LoadImage(string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;
            bool isBase64 = (content.Length % 4 == 0) && Regex.IsMatch(content, @"^[a-zA-Z0-9\+/]*={0,3}$");
            if (isBase64)
            {
                byte[] imageBytes = Convert.FromBase64String(content);
                return Image.FromStream(new MemoryStream(imageBytes));
            }
            if (File.Exists(content)) { return Image.FromFile(content); }
            return null;
        }

        private void ProcesarLogo_MetodoNuevo(PrintPageEventArgs e, ParsedTag parsedTag, int defaultX, int ticketWidth, ref int y)
        {
            try
            {
                using (Image img = LoadImage(parsedTag.Content))
                {
                    if (img == null) return;
                    int logoX = defaultX;
                    if (parsedTag.GetAttribute("align") == "center" && img.Width < ticketWidth)
                    {
                        logoX = (ticketWidth - img.Width) / 2;
                    }
                    e.Graphics.DrawImage(img, new Rectangle(logoX, y, img.Width, img.Height));
                    y += img.Height;
                }
            }
            catch { e.Graphics.DrawString("[Error al cargar logo]", _font, Brushes.Red, defaultX, y); }
        }

        private ParsedTag ParseFullTag(string line)
        {
            if (string.IsNullOrEmpty(line) || !line.StartsWith("<") || !line.EndsWith(">")) return null;
            Match tagMatch = Regex.Match(line, @"^<(?<name>\w+)(?<attrs>(?:\s+\w+\s*=\s*""[^""]*"")*)\s*>(?<content>.*)</\k<name>>$");
            if (!tagMatch.Success) return null;
            ParsedTag parsed = new ParsedTag { Name = tagMatch.Groups["name"].Value, Content = tagMatch.Groups["content"].Value };
            MatchCollection attrMatches = Regex.Matches(tagMatch.Groups["attrs"].Value, @"(\w+)\s*=\s*""([^""]*)""");
            foreach (Match match in attrMatches) { parsed.Attributes[match.Groups[1].Value] = match.Groups[2].Value; }
            return parsed;
        }

        private void ProcesarImagen_MetodoAntiguo(PrintPageEventArgs e, string linea_inicial, int x, ref int y)
        {
            string tagPart = linea_inicial; string contenido = "";
            if (linea_inicial.Contains("</img>"))
            {
                int startContent = linea_inicial.IndexOf('>') + 1; int endContent = linea_inicial.LastIndexOf("</img>");
                if (endContent > startContent) contenido = linea_inicial.Substring(startContent, endContent - startContent).Trim();
            }
            else
            {
                while (lineas_impresas < _lineas.Length && !_lineas[lineas_impresas].Contains("</img>")) { contenido += _lineas[lineas_impresas++].Trim(); }
                if (lineas_impresas < _lineas.Length && _lineas[lineas_impresas].Contains("</img>"))
                {
                    string finalPart = _lineas[lineas_impresas].Substring(0, _lineas[lineas_impresas].IndexOf("</img>"));
                    contenido += finalPart.Trim(); lineas_impresas++;
                }
            }
            contenido = contenido.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();
            if (File.Exists(contenido))
            {
                using (Image img = Image.FromFile(contenido))
                {
                    ImgAttributes parsed = ParseImgAttributes(tagPart, img); int imgX = parsed.CustomX ?? x;
                    if (!parsed.CustomX.HasValue)
                    {
                        if (parsed.Align == "center") imgX = (e.MarginBounds.Width - parsed.Width) / 2;
                        else if (parsed.Align == "right") imgX = e.MarginBounds.Width - parsed.Width - 5;
                    }
                    e.Graphics.DrawImage(img, new Rectangle(imgX, y, parsed.Width, parsed.Height)); y += parsed.Height;
                }
            }
            else { e.Graphics.DrawString("[img no encontrada]", _font, Brushes.Red, x, y); }
        }

        private ImgAttributes ParseImgAttributes(string tagPart, Image img)
        {
            ImgAttributes attrs = new ImgAttributes { Width = img.Width, Height = img.Height, CustomX = null, Align = "left" };
            Match widthMatch = Regex.Match(tagPart, "width\\s*=\\s*\"(\\d+)\"");
            if (widthMatch.Success && int.TryParse(widthMatch.Groups[1].Value, out int w) && w > 0) attrs.Width = w;
            Match heightMatch = Regex.Match(tagPart, "height\\s*=\\s*\"(\\d+)\"");
            if (heightMatch.Success && int.TryParse(heightMatch.Groups[1].Value, out int h) && h > 0) attrs.Height = h;
            else if (widthMatch.Success) attrs.Height = (int)((double)img.Height * attrs.Width / img.Width);
            Match xMatch = Regex.Match(tagPart, "x\\s*=\\s*\"(\\d+)\"");
            if (xMatch.Success && int.TryParse(xMatch.Groups[1].Value, out int xVal)) attrs.CustomX = xVal;
            Match alignMatch = Regex.Match(tagPart, "align\\s*=\\s*\"(\\w+)\"");
            if (alignMatch.Success) attrs.Align = alignMatch.Groups[1].Value.ToLower();
            return attrs;
        }

        private void ProcesarZoom_MetodoAntiguo(PrintPageEventArgs e, string linea_a_imprimir, int x, int y)
        {
            string strValue = "";
            if ((linea_a_imprimir.IndexOf(">") + 1) > linea_a_imprimir.IndexOf(":"))
                strValue = linea_a_imprimir.Substring(linea_a_imprimir.IndexOf(":") + 1, (linea_a_imprimir.IndexOf(">") - 1) - linea_a_imprimir.IndexOf(":"));

            string tagInicio = "<zoom:" + strValue + ">";
            int.TryParse(strValue, out int value);
            linea_a_imprimir = linea_a_imprimir.Replace(tagInicio, "").Replace("</zoom>", "");
            e.Graphics.DrawString(linea_a_imprimir, new Font(_font.Name, _font.Size + value, _font.Style), Brushes.Black, x, y);
        }

        private void ProcesarBarcode(PrintPageEventArgs e, ParsedTag parsedTag, int defaultX, int ticketWidth, ref int y)
        {
            try
            {
                // Configurar el generador de código de barras (Code 128 es el más estándar)
                var writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = 60, // Altura de las barras
                        Width = 250,  // Ancho de las barras
                        Margin = 2,
                        PureBarcode = false // True si NO quieres que aparezca el número abajo
                    }
                };

                using (Bitmap bitmap = writer.Write(parsedTag.Content.Trim()))
                {
                    int barcodeX = defaultX;
                    // Alinear al centro si se solicita
                    if (parsedTag.GetAttribute("align") == "center" || true) // Por defecto centrado
                    {
                        barcodeX = (ticketWidth - bitmap.Width) / 2;
                    }

                    e.Graphics.DrawImage(bitmap, new Rectangle(barcodeX, y, bitmap.Width, bitmap.Height));
                    y += bitmap.Height + 5; // Espacio después del código
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString("[Error Barcode: " + ex.Message + "]", _font, Brushes.Red, defaultX, y);
                y += 15;
            }
        }

        #endregion
    }
}