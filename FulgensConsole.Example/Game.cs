using FulgensConsole.Font;
using System;
using System.Collections.Generic;
using System.Text;

namespace FulgensConsole.Example
{
    public class Game : IState
    {
        private IFont _font;
        private string _inputText;
        private bool _toggleColor;

        private float _currentZ;

        private enum ElementStyle
        {
            Large,
            Small
        }

        private ElementStyle[,] _elements;
        private int _width;
        private int _height;
        private FastNoise _noise;
        private int _charWidth;
        private int _charHeight;
        private const float NoiseScale = 10.0f;
        private bool _shouldQuit;

        public void OnInit(IShell shell)
        {
            _font = shell.LoadTrueTypeFont("MorePerfectDOSVGA.ttf", 32);
            (_charWidth, _charHeight) = _font.TextSize("M");
            _toggleColor = false;
            _currentZ = 0.00001f;
            _width = 800 / _charWidth;
            _height = 600 / _charHeight;
            _elements = new ElementStyle[_width, _height];
            _noise = new FastNoise(new Random().Next());
            _shouldQuit = false;
        }

        public void OnClosing(IShell shell)
        {
            Console.WriteLine("Goodbye!");
        }

        public void OnDraw(IShell shell)
        {
            if (_shouldQuit)
            {
                shell.Close();
                return;
            }
            
            var sb = new StringBuilder(_width);
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    var element = _elements[x, y];
                    var visual = element == ElementStyle.Large ? "#" : ".";
                    sb.Append(visual);
                }

                shell.Write(sb.ToString(), 0, y * _charHeight, _font, _toggleColor ? Color.White : new Color(255, 0, 0));
                sb.Clear();
            }
        }

        public void OnUpdate()
        {
            _currentZ += 0.1f;
            for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    _elements[x, y] = _noise.GetPerlinFractal(x * NoiseScale, y * NoiseScale, _currentZ) > 0.0f ? ElementStyle.Large : ElementStyle.Small;
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.ESCAPE)
            {
                _shouldQuit = true;
            }
        }

        public void OnKeyDown(Key key)
        {
            switch (key)
            {
                case Key.SPACE:
                    _toggleColor = !_toggleColor;
                    break;
                case Key.BACKSPACE when !string.IsNullOrEmpty(_inputText):
                    _inputText = _inputText.Substring(0, _inputText.Length - 1);
                    break;
            }
        }

        public void OnTextInput(string text)
        {
            _inputText += text;
        }
    }
}
