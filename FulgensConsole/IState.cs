using System;
using System.Collections.Generic;
using System.Text;

namespace FulgensConsole
{
    public interface IState
    {
        /// <summary>
        /// Occurs once at startup.  Use this to load resources and initiate game state.
        /// </summary>
        void OnInit(IShell shell);

        /// <summary>
        /// Occurs each frame.
        /// </summary>
        void OnDraw(IShell shell);

        /// <summary>
        /// Occurs on a separate thread.  Use this to update game state.
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// Occurs once at shutdown.  Use this to release any resources.
        /// </summary>
        void OnClosing(IShell shell);

        void OnKeyUp(Key key);

        void OnKeyDown(Key key);

        void OnTextInput(string text);
    }
}
