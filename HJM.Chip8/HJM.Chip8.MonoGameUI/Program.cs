using System;

namespace HJM.Chip8.MonoGameUI
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Chip8Game();
            game.Run();
        }
    }
}
