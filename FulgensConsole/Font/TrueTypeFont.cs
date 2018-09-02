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

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "load_ttf_font",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NativeLoadTrueTypeFont(IntPtr shell, string path, int size);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "draw_text",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeDrawText(IntPtr shell, IntPtr font, string contents,
            int x, int y, Color foreColor, Color backColor);

        public (int width, int height) TextSize(string contents)
        {
            int width = 0, height = 0;
            NativeTextSize(_nativeResource, contents, ref width, ref height);
            return (width, height);
        }

        private IntPtr _nativeResource;

        public void Draw(IntPtr nativeShell, int x, int y, string contents, Color foreColor, Color? backColor = null)
        {
            NativeDrawText(nativeShell, _nativeResource, contents, x, y,
                foreColor, backColor ?? Color.Transparent);
        }

        public TrueTypeFont(IntPtr nativeShell, string path, int size)
        {
            _nativeResource = NativeLoadTrueTypeFont(nativeShell, path, size);
        }
    }
}
