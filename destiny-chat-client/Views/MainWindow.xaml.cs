using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using static destiny_chat_client.Classes.WindowHandler;

namespace destiny_chat_client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if (AlreadyOpen)
            {
                FocusChat();
                Close();
            }

            else
                InitializeComponent();
        }

        private static bool AlreadyOpen
        {
            get
            {
                var name = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
                return Process.GetProcessesByName(name).Length > 1;
            }
        }
    }
}
