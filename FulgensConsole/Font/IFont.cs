using System;

namespace FulgensConsole.Font
{
    public interface IFont
    {
        IntPtr NativePtr { get; }
        (int width, int height) TextSize(string contents);
    }
}
