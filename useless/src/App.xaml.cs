using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace AnnoyingCat
{
    public partial class App : Application
    {
        private static Mutex _mutex;
        private TrayIconManager _trayManager;

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appMutexName = "AnnoyingCatDesktopOverlay_SingleInstanceMutex";
            bool createdNew;
            _mutex = new Mutex(true, appMutexName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("പൂച്ച ഇതിനകം ഡെസ്ക്ടോപ്പിൽ ഓടുന്നുണ്ട്!\n(The cat is already roaming on your desktop!)",
                                "ശല്യക്കാരൻ പൂച്ച",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                Shutdown();
                return;
            }

            base.OnStartup(e);

            string baseDir = AssetManager.ResolveBaseDirectory();
            Console.WriteLine("[App] Starting with baseDir: " + baseDir);

            MainWindow mainWindow = new MainWindow(baseDir);
            _trayManager = new TrayIconManager(mainWindow, baseDir);

            this.MainWindow = mainWindow;
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_trayManager != null)
            {
                _trayManager.Dispose();
            }

            if (_mutex != null)
            {
                try
                {
                    _mutex.ReleaseMutex();
                    _mutex.Dispose();
                }
                catch { }
            }

            base.OnExit(e);
        }
    }
}
