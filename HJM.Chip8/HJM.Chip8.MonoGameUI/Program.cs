using System;
using Serilog;

namespace HJM.Chip8.MonoGameUI
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            InitializeLogging();

            using Game1? game = new Game1();
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
