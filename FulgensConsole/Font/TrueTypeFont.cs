using System;
using System.Runtime.InteropServices;

namespace FulgensConsole.Font
{
    internal class TrueTypeFont : IFont
    {
        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "ttf_text_size",
            ExactSpelling = false,
            CallingConvention = CallingConvention.StdCall)]
        private static extern void NativeTextSize(IntPtr fnt, string contents, ref int width, ref int height);

        public (int width, int height) TextSize(string contents)
        {
            int width = 0, height = 0;
            NativeTextSize(NativePtr, contents, ref width, ref height);
            return (width, height);
        }

        public IntPtr NativePtr { get; }
        
        public TrueTypeFont(IntPtr ptr)
        {
            NativePtr = ptr;
        }
    }
}
