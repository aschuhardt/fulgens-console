using System;

namespace FulgensConsole.Font
{
    public interface IFont
    {
        void Draw(IntPtr nativeShell, int x, int y, string contents, Color foreColor, Color? backColor = null);
        (int width, int height) TextSize(string contents);
    }
}
