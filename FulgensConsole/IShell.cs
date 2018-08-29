using FulgensConsole.Font;
using System;

namespace FulgensConsole
{
    public interface IShell : IDisposable
    {
        /// <summary>
        /// Loads a TrueType font from a location on the disk
        /// </summary>
        IFont LoadTrueTypeFont(string path, int size);

        /// <summary>
        /// Writes text to the shell at the specified location and using the
        /// <see cref="IFont"/> instance provided
        /// </summary>
        void Write(string contents, int x, int y, IFont font, Color color);
        
        /// <summary>
        /// Resizes the shell window to the given parameters
        /// </summary>
        void Resize(int width, int height);

        /// <summary>
        /// Closes the shell window and triggers resource cleanup
        /// </summary>
        void Close();
    }
}
