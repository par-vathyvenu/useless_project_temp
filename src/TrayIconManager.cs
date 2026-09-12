using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace AnnoyingCat
{
    public class TrayIconManager : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;
        private readonly MainWindow _mainWindow;

        public TrayIconManager(MainWindow mainWindow, string baseDir)
        {
            _mainWindow = mainWindow;
            _notifyIcon = new NotifyIcon();

            string iconPath = Path.Combine(baseDir, "assets", "sprites", "icon.ico");
            if (File.Exists(iconPath))
            {
                try
                {
                    _notifyIcon.Icon = new Icon(iconPath);
                }
                catch
                {
                    _notifyIcon.Icon = SystemIcons.Application;
                }
            }
            else
            {
                _notifyIcon.Icon = SystemIcons.Application;
            }

            _notifyIcon.Text = "ശല്യക്കാരൻ പൂച്ച (Annoying Cat)";
            _notifyIcon.Visible = true;

            // Double click: summon to screen center
            _notifyIcon.DoubleClick += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() =>
                {
                    _mainWindow.Controller.TriggerBlockScreenCenter();
                });
            };

            // Tray context menu
            var contextMenu = new ContextMenuStrip();

            var miCenter = new ToolStripMenuItem("🎯 ഇവിടെ വാ (Summon to Center)");
            miCenter.Click += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() => _mainWindow.Controller.TriggerBlockScreenCenter());
            };

            var miNap = new ToolStripMenuItem("💤 ശല്യം ചെയ്യല്ലേ (Take a Nap)");
            miNap.Click += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() => _mainWindow.Controller.TriggerSnooze(120.0));
            };

            var miQuip = new ToolStripMenuItem("💬 ഒരു ഡയലോഗ് പറ (Say Something)");
            miQuip.Click += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() => _mainWindow.Controller.SayRandomQuip());
            };

            var miDance = new ToolStripMenuItem("💃 ഡാൻസ് കളിക്ക് (Dance!)");
            miDance.Click += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() => _mainWindow.Controller.TriggerDance());
            };

            var miDisappear = new ToolStripMenuItem("👻 പോ അവിടുന്ന് (Disappear / Shoo)");
            miDisappear.Click += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() => _mainWindow.Controller.TriggerDisappear());
            };

            var miExit = new ToolStripMenuItem("❌ വിട പറയുക (Exit)");
            miExit.Click += (s, e) =>
            {
                _mainWindow.Dispatcher.Invoke(() => System.Windows.Application.Current.Shutdown());
            };

            contextMenu.Items.Add(miCenter);
            contextMenu.Items.Add(miNap);
            contextMenu.Items.Add(miQuip);
            contextMenu.Items.Add(miDance);
            contextMenu.Items.Add(miDisappear);
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(miExit);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        public void Dispose()
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = false;
                _notifyIcon.Dispose();
            }
        }
    }
}
