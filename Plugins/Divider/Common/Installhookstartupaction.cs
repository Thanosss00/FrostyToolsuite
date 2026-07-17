using System;
using System.Windows;
using System.Windows.Threading;
using Frosty.Core;
using FrostySdk.Interfaces;
using ModCategories.Common;
 
namespace ModCategories
{
    public class InstallHookStartupAction : StartupAction
    {
        public override Action<ILogger> Action => (logger) =>
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => WaitAndInstall(logger)));
        };
 
        private static void WaitAndInstall(ILogger logger)
        {
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            timer.Tick += (s, e) =>
            {
                Window mainWindow = Application.Current?.MainWindow;
                if (mainWindow != null && mainWindow.IsLoaded)
                {
                    timer.Stop();
                    ModListHook.Install(mainWindow, logger);
                }
            };
            timer.Start();
        }
    }
}