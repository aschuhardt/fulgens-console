using System;
using System.Runtime.InteropServices;

namespace FulgensConsole
{
    [StructLayout(LayoutKind.Sequential, Pack=8)]
    public struct Color
    {
        public int R, G, B, A;

        public static Color White = new Color(0xFF, 0xFF, 0xFF);
        public static Color Transparent = new Color(0x00, 0x00, 0x00, 0x00);

        public Color(double r, double g, double b, double a = 1.0f)
        {
            R = Convert.ToInt32(Math.Truncate(r * 0xFF));
            G = Convert.ToInt32(Math.Truncate(g * 0xFF));
            B = Convert.ToInt32(Math.Truncate(b * 0xFF));
            A = Convert.ToInt32(Math.Truncate(a * 0xFF));
        }

        public Color(byte r, byte g, byte b, byte a = 0xFF)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
