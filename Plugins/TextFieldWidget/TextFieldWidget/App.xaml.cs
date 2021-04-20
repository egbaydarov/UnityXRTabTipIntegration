using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TextFieldWidget
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppExit(object sender, ExitEventArgs e)
        {
            AppMainWindow.IsSharedMomeryReachable = false;
            AppMainWindow.mmf.Dispose();
            AppMainWindow.TextChangedEvent.Dispose();
            AppMainWindow.TextFieldClearEvent.Dispose();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1)
                AppMainWindow.SIZE = int.Parse(e.Args[0]);
        }
    }
}
