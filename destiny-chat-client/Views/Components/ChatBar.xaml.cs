using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace destiny_chat_client.Views.Components
{
    /// <summary>
    /// Interaction logic for ChatBar.xaml
    /// </summary>
    public partial class ChatBar : UserControl
    {
        public ChatBar()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                e.Handled = true;
        }

        private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // selection change on auto complete (I guess it works)
            if (e.Changes.Sum(change => change.AddedLength) > 1)
            {
                var textbox = (TextBox)sender;
                textbox.CaretIndex = textbox.Text.Length;
            }
        }
    }
}
