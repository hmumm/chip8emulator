using System;
using System.IO;
using Terminal.Gui;

namespace HJM.Chip8.MonoGameUI.Terminal
{
    public class GameSelectWindow
    {
        public static string getSelectedFilename()
        {
            Application.Init();

            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            var ok = new Button(3, 14, "Ok");

            ok.Clicked += new Action(() =>
            {
                Application.RequestStop();
            });

            var dialog = new Dialog("Select Program", 60, 18, ok);

            var entry = new TextField()
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = 1
            };

            entry.Text = homeDirectory;

            dialog.Add(entry);

            Application.Run(dialog);

            Application.Shutdown();

            string? result = entry.Text.ToString();

            if (result == null)
            {
                result = string.Empty;
            }

            return result;
        }
    }
}
