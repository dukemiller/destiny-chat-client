using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Views;
using Application = System.Windows.Application;

namespace destiny_chat_client.Classes
{
    /// <summary>
    ///     The underlying logic for a tray icon
    /// </summary>
    public class Tray
    {
        /// <summary>
        ///     In the case that I have both to tray on minimize and to tray on exit,
        ///     exit will call the window state change "minimizing" the window twice
        /// </summary>
        private static bool _minimizing;

        private readonly ISettingsRepository _settingsRepository;

        private NotifyIcon _trayIcon;

        private ContextMenu _trayContextMenu;

        private static MainWindow MainWindow => (MainWindow)Application.Current.MainWindow;

        public Tray(ISettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
            _settingsRepository.PropertyChanged += PropertyChanged;
            MainWindow.Closing += WindowIsClosing;
            MainWindow.StateChanged += WindowStateChanged;

            InitTray();
            InitContextMenu();

            Visible = _settingsRepository.TrayAlwaysOpen;
        }

        // 

        private bool FullExit { get; set; }

        private bool Visible
        {
            get => _trayIcon.Visible;
            set => _trayIcon.Visible = value;
        }

        // 

        private void InitTray()
        {
            // get the image from the program
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("destiny_chat_client.Resources.icon.ico");
            Debug.Assert(stream != null, "stream != null");

            _trayIcon = new NotifyIcon
            {
                Icon = new Icon(stream)
            };

            _trayIcon.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                    if (MainWindow.WindowState == WindowState.Minimized)
                    {
                        MainWindow.Show();
                        MainWindow.WindowState = WindowState.Normal;
                        _minimizing = false;
                    }
                    else if (MainWindow.WindowState == WindowState.Normal)
                    {
                        MainWindow.WindowState = WindowState.Minimized;
                        _minimizing = true;
                    }
            };

            stream.Close();
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_settingsRepository.TrayAlwaysOpen)
                Visible = true;

            else
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                    Visible = true;
                else if (MainWindow.WindowState == WindowState.Normal)
                    if (Visible)
                        Visible = false;
            }
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {

            switch (MainWindow.WindowState)
            {
                case WindowState.Normal:
                    Visible = _settingsRepository.TrayAlwaysOpen;
                    MainWindow.Show();
                    break;

                case WindowState.Minimized:
                    if (_settingsRepository.MinimizeToTray && !_minimizing)
                    {
                        _minimizing = true;
                        if (_settingsRepository.TrayAlwaysOpen)
                            Visible = true;
                        MainWindow.Hide();
                    }
                    break;
            }
        }

        private void WindowIsClosing(object sender, CancelEventArgs e)
        {
            // The program is closing
            if (FullExit)
                Visible = false;

            // It's not closing
            else if (_settingsRepository.ExitToTray)
            {
                if (!_settingsRepository.TrayAlwaysOpen)
                    Visible = true;
                _minimizing = true;
                MainWindow.Hide();
                MainWindow.WindowState = WindowState.Minimized;
                e.Cancel = true;
            }

            // Who knows how I got here
            else
                Visible = false;
        }

        private void InitContextMenu()
        {
            _trayContextMenu = new ContextMenu();

            // "Header"
            _trayContextMenu.MenuItems.Add(new MenuItem("Destiny.gg Chat Client")
            {
                Enabled = false
            });

            _trayContextMenu.MenuItems.Add("-");

            // Maybe add setting changes here later
            _trayContextMenu.MenuItems.Add(
                new MenuItem("E&xit", (sender, args) =>
                {
                    _trayIcon.Visible = false;
                    FullExit = true;
                    Application.Current.Shutdown();
                }));

            _trayIcon.ContextMenu = _trayContextMenu;
        }
    }
}