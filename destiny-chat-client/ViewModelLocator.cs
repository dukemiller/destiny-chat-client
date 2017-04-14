using destiny_chat_client.Repositories;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services;
using destiny_chat_client.Services.Interfaces;
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

            SimpleIoc.Default.Register<ISettingsRepository>(SettingsRepository.Load);
            SimpleIoc.Default.Register<IEmoteRepository, EmoteRepository>();
            SimpleIoc.Default.Register<IFlairRepository, FlairRepository>();
            SimpleIoc.Default.Register<ISoundRepository, SoundRepository>();

            SimpleIoc.Default.Register<IChatService, ChatService>();
            SimpleIoc.Default.Register<IDataConverterService, DataConverterService>();
            SimpleIoc.Default.Register<ICookieFinderService, CookieFinderService>();

            SimpleIoc.Default.Register<MainWindowViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
        }

        public static MainWindowViewModel Main => ServiceLocator.Current.GetInstance<MainWindowViewModel>();

        public static ChatViewModel Chat => ServiceLocator.Current.GetInstance<ChatViewModel>();
    }
}