using System.Threading.Tasks;

namespace destiny_chat_client.Views.Components
{
    /// <summary>
    /// Interaction logic for ComboMessage.xaml
    /// </summary>
    public partial class ComboMessage
    {
        public ComboMessage()
        {
            InitializeComponent();
            FadeIn();
        }

        public async void FadeIn()
        {
            for (var i = 0.1; i <= 1.0; i += 0.1)
            {
                Opacity = i;
                await Task.Delay(40);
            }
        }
    }
}
