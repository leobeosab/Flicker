using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace Flicker
{
    public partial class App : Application
    {
        private TaskbarIcon? notifyIcon;
        private KeyboardHandler? keyboardHandler;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            notifyIcon = (TaskbarIcon)FindResource("FlickerIcon");
            keyboardHandler = new KeyboardHandler();
            keyboardHandler.Init();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon?.Dispose();
            keyboardHandler?.Destroy();
            base.OnExit(e);
        }
    }
}