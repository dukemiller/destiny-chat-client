using System.Windows.Controls;
using System.Windows.Input;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using destiny_chat_client.Services;

namespace destiny_chat_client.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for Debug.xaml
    /// </summary>
    public partial class Debug
    {
        private readonly bool _isSet;

        public Debug()
        {
            InitializeComponent();
            Combo.ItemsSource = Extensions.GetValues<DebugScenario>();
            Combo.SelectedItem = MockChatService.DebugScenario;
            _isSet = true;
        }

        private void ComboChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isSet) return;
            MockChatService.DebugScenario = (DebugScenario)Combo.SelectedItem;
            Close();
        }

        private void Debug_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
