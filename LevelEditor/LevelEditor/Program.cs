using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LevelEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            splashScreen aSplashScreen = new splashScreen();
            aSplashScreen.ShowDialog();
            Application.Run(new levelEditor());
        }
    }
}
