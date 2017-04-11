using destiny_chat_client.ViewModels;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace destiny_chat_client
{
    public class ViewModelLocator : ViewModelBase
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
        }

        public static MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();

        public static ChatViewModel Chat => ServiceLocator.Current.GetInstance<ChatViewModel>();
    }
}