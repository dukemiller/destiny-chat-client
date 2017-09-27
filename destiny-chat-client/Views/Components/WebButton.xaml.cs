using System.Windows;

namespace destiny_chat_client.Views.Components
{
    /// <summary>
    /// Interaction logic for WebButton.xaml
    /// </summary>
    public partial class WebButton
    {
        public WebButton() => InitializeComponent();

        public string Kind
        {
            get => (string) GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.Register("Kind", typeof(string), typeof(WebButton), new PropertyMetadata(""));
    }
}
