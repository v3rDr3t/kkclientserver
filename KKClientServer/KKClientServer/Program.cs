using KKClientServer.Networking;
using System;
using System.Windows.Forms;

namespace KKClientServer
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

            // setup core components
            MainView mainView = new MainView();
            new Controller(mainView);

            // run
            Application.Run(mainView);
        }
    }
}
