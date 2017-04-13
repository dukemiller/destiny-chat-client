using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using destiny_chat_client.Views.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace destiny_chat_client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string ApplicationPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "destiny_chat_client");


        public MainWindowViewModel()
        {
            TrayCommand = new RelayCommand(() => new Tray(SimpleIoc.Default.GetInstance<ISettingsRepository>()));
        }

        public RelayCommand TrayCommand { get; set; }

    }
}