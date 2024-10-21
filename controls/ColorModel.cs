using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace drawingApp.controls
{
    public abstract class ColorModel
    {
        public abstract void UpdateFromColor(System.Windows.Media.Color color);
        public abstract System.Windows.Media.Color ToColor();
    }
    public class RGBModel : ColorModel
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public override void UpdateFromColor(System.Windows.Media.Color color)
        {
            Red = color.R;
            Green = color.G;
            Blue = color.B;
        }

        public override System.Windows.Media.Color ToColor()
        {
            return System.Windows.Media.Color.FromRgb((byte)Red, (byte)Green, (byte)Blue);
        }

        // Konwersja RGB -> CMYK
        public CMYKModel ToCMYKModel()
        {
            CMYKModel cmyk = new CMYKModel();
            float r = Red / 255f;
            float g = Green / 255f;
            float b = Blue / 255f;

            cmyk.Black = 1 - Math.Max(r, Math.Max(g, b));
            if (cmyk.Black < 1)
            {
                cmyk.Cyan = (1 - r - cmyk.Black) / (1 - cmyk.Black);
                cmyk.Magenta = (1 - g - cmyk.Black) / (1 - cmyk.Black);
                cmyk.Yellow = (1 - b - cmyk.Black) / (1 - cmyk.Black);
            }
            else
            {
                cmyk.Cyan = cmyk.Magenta = cmyk.Yellow = 0;
            }
            return cmyk;
        }
    }

    public class CMYKModel : ColorModel
    {
        public float Cyan { get; set; }
        public float Magenta { get; set; }
        public float Yellow { get; set; }
        public float Black { get; set; }

        public override void UpdateFromColor(System.Windows.Media.Color color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            Black = 1 - Math.Max(r, Math.Max(g, b));
            if (Black < 1)
            {
                Cyan = (1 - r - Black) / (1 - Black);
                Magenta = (1 - g - Black) / (1 - Black);
                Yellow = (1 - b - Black) / (1 - Black);
            }
            else
            {
                Cyan = Magenta = Yellow = 0;
            }
        }

        public override System.Windows.Media.Color ToColor()
        {
            byte r = (byte)(255 * (1 - Cyan) * (1 - Black));
            byte g = (byte)(255 * (1 - Magenta) * (1 - Black));
            byte b = (byte)(255 * (1 - Yellow) * (1 - Black));
            return System.Windows.Media.Color.FromRgb(r, g, b);
        }

        // Konwersja CMYK -> RGB
        public RGBModel ToRGBModel()
        {
            RGBModel rgb = new RGBModel();
            rgb.Red = (byte)(255 * (1 - Cyan) * (1 - Black));
            rgb.Green = (byte)(255 * (1 - Magenta) * (1 - Black));
            rgb.Blue = (byte)(255 * (1 - Yellow) * (1 - Black));
            return rgb;
        }
    }
   
}
