using System;
using Serilog;
using Serilog.Configuration;

namespace HJM.Chip8.MonoGameUI
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            InitializeLogging();

            using var game = new Game1();
            game.Run();
        }

        private static void InitializeLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}
