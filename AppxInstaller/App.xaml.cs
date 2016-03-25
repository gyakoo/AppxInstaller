using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace AppxInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Target;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                ProcessStartInfo startInfo;

                startInfo = new ProcessStartInfo();
                startInfo.FileName = this.GetType().Assembly.Location;
                startInfo.Arguments = e.Args.ElementAtOrDefault(0); // if you need to pass any command line arguments to your stub, enter them here
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                Process.Start(startInfo);
                Process.GetCurrentProcess().Kill();
            }
            Target = e.Args.ElementAtOrDefault(0);
            base.OnStartup(e);
        }
    }
}
