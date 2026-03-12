using System;
using System.IO;
using System.Windows;

namespace AssaultCubeExternal
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Catch any unhandled crash and write to a log file next to the exe
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
            {
                string msg = ex.ExceptionObject?.ToString() ?? "unknown error";
                File.WriteAllText("crash.log", msg);
                MessageBox.Show(msg, "ACTrainer Crash", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            DispatcherUnhandledException += (s, ex) =>
            {
                string msg = ex.Exception?.ToString() ?? "unknown error";
                File.WriteAllText("crash.log", msg);
                MessageBox.Show(msg, "ACTrainer Crash", MessageBoxButton.OK, MessageBoxImage.Error);
                ex.Handled = true;
            };

            base.OnStartup(e);
        }
    }
}
