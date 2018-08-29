using System;
using System.Timers;
using FulgensConsole;

namespace FulgensConsole.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var shell = new Shell(800, 600, "Fulgens"))
                shell.Run(new Game());
        }
    }
}
