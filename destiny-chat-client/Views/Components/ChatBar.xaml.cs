using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using destiny_chat_client.Classes;

namespace destiny_chat_client.Views.Components
{
    /// <summary>
    /// Interaction logic for ChatBar.xaml
    /// </summary>
    public partial class ChatBar
    {
        public ChatBar()
        {
            InitializeComponent();
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                e.Handled = true;

            else if (WordSuggestor.Suggesting)
                WordSuggestor.Suggesting = false;
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
