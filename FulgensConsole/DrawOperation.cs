using FulgensConsole.Font;
using System;
using System.Collections.Generic;
using System.Text;

namespace FulgensConsole
{
    internal class DrawOperation
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color Color { get; set; }
        public string Contents { get; set; }
        public IFont Font { get; set; }
    }
}
