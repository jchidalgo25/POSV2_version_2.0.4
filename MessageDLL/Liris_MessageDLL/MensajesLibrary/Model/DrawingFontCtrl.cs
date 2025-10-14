using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liris_MenssageDLL.Model
{

    public class DrawingFontCtrl
    {
        public FontStyle fontStyle { get; set; }
        public FontFamily fontFamily { get; set; }
        public float emSize { get; set; }

        public DrawingFontCtrl()
        {
            // Valores por defecto
            fontStyle = FontStyle.Bold;
            fontFamily = new FontFamily("Segoe UI");
            emSize = 20f;

            /*
             Control.Common.GlobalParameters.fontFamily = new FontFamily("Segoe UI");
                    Control.Common.GlobalParameters.fontStyle = FontStyle.Bold;
                    Control.Common.GlobalParameters.emSize = 20f;
             */
        }

        public Font GetFont()
        {
            return new Font(fontFamily, emSize, fontStyle);
        }
    }
}
