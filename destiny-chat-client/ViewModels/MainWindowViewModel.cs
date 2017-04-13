using System;
using System.IO;
using GalaSoft.MvvmLight;

namespace destiny_chat_client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string ApplicationPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "destiny_chat_client");
    }
}