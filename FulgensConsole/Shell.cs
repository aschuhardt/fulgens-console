using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using FulgensConsole.Font;

namespace FulgensConsole
{
    public class Shell : IShell
    {
        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "initialize",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr NativeInitialize(int width, int height, string title);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "dispose",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeCleanup(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "quitting",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern bool NativeQuitting(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "update",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeUpdate(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "quit",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeQuit(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "resize",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeResize(IntPtr shell, int width, int height);

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
            int x, int y, int r, int g, int b, int a);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "clear",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeClear(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "flip_buffer",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeFlipBuffer(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "disposed",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern bool NativeDisposed(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "get_key_down",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int NativeGetKeyDown(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "get_key_up",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern int NativeGetKeyUp(IntPtr shell);

        [DllImport("FulgensConsole.Native.dll",
            EntryPoint = "get_input_text",
            ExactSpelling = false,
            CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeGetInputText(IntPtr shell, StringBuilder text);

        private IntPtr _nativeShell;
        private Queue<DrawOperation> _drawOps;
        private static object _stateLock;

        public Shell(int width, int height, string title)
        {
            _nativeShell = NativeInitialize(width, height, title);
            if (_nativeShell == IntPtr.Zero)
                throw new Exception("Failed to initialize the shell!");

            _drawOps = new Queue<DrawOperation>();
            _stateLock = new object();
        }

        public void Run(IState state)
        {
            bool quitting = false;

            state.OnInit(this);

            var updateThread = new Thread(new ThreadStart(() =>
            {
                while (!quitting)
                {
                    Thread.Sleep(10);
                    lock (_stateLock)
                    {
                        state.OnUpdate();
                    }
                }
            }));

            updateThread.Start();

            while (!NativeQuitting(_nativeShell))
            {
                NativeUpdate(_nativeShell);

                SendKeyDown(state);
                SendKeyUp(state);
                SendInputText(state);

                lock (_stateLock)
                {
                    state.OnDraw(this);
                }

                NativeClear(_nativeShell);
                while (_drawOps.Count > 0)
                {
                    var op = _drawOps.Dequeue();
                    NativeDrawText(_nativeShell, op.Font.NativePtr, op.Contents,
                        op.X, op.Y, op.Color.R, op.Color.G, op.Color.B, op.Color.A);
                }
                NativeFlipBuffer(_nativeShell);

                Thread.Sleep(33);
            }

            quitting = true;
            updateThread.Join(30000);

            state.OnClosing(this);
        }

        public void Close()
        {
            NativeQuit(_nativeShell);
        }

        public IFont LoadTrueTypeFont(string path, int size)
        {
            var nativeFont = NativeLoadTrueTypeFont(_nativeShell, path, size);
            if (nativeFont == IntPtr.Zero)
                throw new Exception($"Failed to load TrueType font at {path}!");
            return new TrueTypeFont(nativeFont);
        }

        public void Resize(int width, int height)
        {
            NativeResize(_nativeShell, width, height);
        }

        public void Write(string contents, int x, int y, IFont font, Color color)
        {
            if (string.IsNullOrEmpty(contents))
                return;
            _drawOps.Enqueue(new DrawOperation
            {
                X = x,
                Y = y,
                Color = color,
                Contents = contents,
                Font = font
            });
        }

        private void SendKeyDown(IState state)
        {
            var key = Key.NONE;
            do
            {
                key = (Key)NativeGetKeyDown(_nativeShell);
                if (key != Key.NONE)
                    state.OnKeyDown(key);
            } while (key != Key.NONE);
        }

        private void SendKeyUp(IState state)
        {
            var key = Key.NONE;
            do
            {
                key = (Key)NativeGetKeyUp(_nativeShell);
                if (key != Key.NONE)
                    state.OnKeyUp(key);
            } while (key != Key.NONE);
        }

        private void SendInputText(IState state)
        {
            var sb = new StringBuilder(256);
            NativeGetInputText(_nativeShell, sb);
            var text = sb.ToString();
            if (!string.IsNullOrEmpty(text))
                state.OnTextInput(text);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                NativeCleanup(_nativeShell);

                // wait for SDL to finish cleaning itself up
                do { } while (!NativeDisposed(_nativeShell));

                disposedValue = true;
            }
        }

        ~Shell()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
