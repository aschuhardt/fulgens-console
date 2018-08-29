using FulgensConsole.Font;
using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace FulgensConsole.Example
{
    public class Game : IState
    {
        private IFont _font;
        private string _inputText;
        private bool _toggleColor;

        public void OnInit(IShell shell)
        {
            _font = shell.LoadTrueTypeFont("MorePerfectDOSVGA.ttf", 36);
            _toggleColor = false;
        }

        public void OnClosing(IShell shell)
        {

        }

        public void OnDraw(IShell shell)
        {
            if (_toggleColor)
                shell.Write(_inputText, 100, 100, _font, Color.White);
            else
                shell.Write(_inputText, 100, 100, _font, new Color(255, 0, 0));
        }

        public void OnUpdate()
        {

        }

        public void OnKeyUp(Key key)
        {

        }

        public void OnKeyDown(Key key)
        {
            if (key == Key.SPACE)
                _toggleColor = !_toggleColor;

            if (key == Key.BACKSPACE && !string.IsNullOrEmpty(_inputText))
                _inputText = _inputText.Substring(0, _inputText.Length - 1);
        }

        public void OnTextInput(string text)
        {
            _inputText += text;
        }
    }
}
