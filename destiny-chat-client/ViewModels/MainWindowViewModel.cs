using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using destiny_chat_client.Views.Dialogs;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using UserControl = System.Windows.Controls.UserControl;

namespace destiny_chat_client.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string ApplicationPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "destiny_chat_client");

        public static bool AutoCorrecting;

        // Fields

        private ISettingsRepository _settingsRepository;

        private string _message;

        private bool _loggedIn;

        private readonly ChatViewModel _chatViewModel;

        private bool _isShowingPopup;

        private readonly List<string> _commandHistory = new List<string>();

        private int _commandHistoryIndex;

        private UserControl _popupView;

        private IChatService _chatService;

        private IEmoteRepository _emoteRepository;

        private bool _isShowingUserlist;

        private bool _isShowingSettings;

        private bool _isShowingEmotes;

        private bool _isShowingLogin;

        // Constructor

        public MainWindowViewModel(IChatService chatService,
            ISettingsRepository settingsRepository,
            IEmoteRepository emoteRepository)
        {
            ChatService = chatService;
            EmoteRepository = emoteRepository;
            _chatViewModel = SimpleIoc.Default.GetInstance<ChatViewModel>();
            SettingsRepository = settingsRepository;
            SettingsRepository.PropertyChanged += async (sender, args) =>
            {
                await SettingsRepository.Save();
                if (args.PropertyName.Equals("Sid") || args.PropertyName.Equals("RememberMe") ||
                    args.PropertyName == "LoggedIn")
                {
                    LoginCommand.RaiseCanExecuteChanged();
                    RetrieveDetailsCommand.RaiseCanExecuteChanged();
                }
            };

            SendCommand = new RelayCommand(SendMessage);
            LoginCommand = new RelayCommand(
                Login, 
                () => !SettingsRepository.LoggedIn && (SettingsRepository.Sid.Length > 0 || SettingsRepository.RememberMe.Length > 0)
            );
            LogoutCommand = new RelayCommand(Logout);
            ClearCommand = new RelayCommand(() => Message = "");

            SettingsCommand = new RelayCommand(() =>
            {
                if (!IsShowingSettings)
                {
                    PopupView = new Settings();
                    IsShowingSettings = true;
                    IsShowingPopup = true;
                }

                else
                    IsShowingPopup = false;
            });

            UserlistCommand = new RelayCommand(() =>
            {
                PopupView = new Users();
                IsShowingUserlist = true;
                IsShowingPopup = true;
            });

            EmotesCommand = new RelayCommand(() =>
            {
                PopupView = new Emotes();
                IsShowingEmotes = true;
                IsShowingPopup = true;
            });

            ShowLoginCommand = new RelayCommand(() =>
            {
                PopupView = new Login();
                IsShowingSettings = false;
                IsShowingLogin = true;
                IsShowingPopup = true;
            });

            UseEmoteCommand = new RelayCommand<Emote>(emote =>
            {
                if (_settingsRepository.LoggedIn)
                    Message += emote.Description() + " ";
            });

            OpenWebsiteCommand = new RelayCommand<string>(name =>
            {
                switch (name)
                {
                    case "destiny":
                        Process.Start("https://www.destiny.gg/bigscreen");
                        break;
                    case "twitter":
                        Process.Start("https://twitter.com/omnidestiny?lang=en");
                        break;
                    case "reddit":
                        Process.Start("https://www.reddit.com/r/Destiny/");
                        break;
                    case "youtube":
                        Process.Start("https://www.youtube.com/user/destiny");
                        break;
                    case "twitch":
                        Process.Start("https://www.twitch.tv/destiny");
                        break;
                }
            });

            RetrieveDetailsCommand = new RelayCommand(() => ChatService.FindDetails(), () => !SettingsRepository.LoggedIn);
            TrayCommand = new RelayCommand(() => new Tray(SimpleIoc.Default.GetInstance<ISettingsRepository>()));
            AutoCompleteCommand = new RelayCommand(AutoComplete);

            SaveCommand = new RelayCommand(() =>
            {
                IsShowingSettings = false;
                SettingsRepository.Save();
            });

            ClosePopupCommand = new RelayCommand(() => IsShowingPopup = false);

            LoggedIn = SettingsRepository.LoggedIn;
            PreviousMessageCommand = new RelayCommand(PreviousMessage);
            NextMessageCommand = new RelayCommand(NextMessage);
            
        }
        
        // Popup logic

        public bool IsShowingPopup
        {
            get => _isShowingPopup;
            set
            {
                Set(() => IsShowingPopup, ref _isShowingPopup, value);
                if (!IsShowingPopup)
                {
                    IsShowingUserlist = false;
                    IsShowingSettings = false;
                    IsShowingEmotes = false;
                    IsShowingLogin = false;
                }
            }
        }

        public bool IsShowingUserlist
        {
            get => _isShowingUserlist;
            set => Set(() => IsShowingUserlist, ref _isShowingUserlist, value);
        }

        public bool IsShowingSettings
        {
            get => _isShowingSettings;
            set => Set(() => IsShowingSettings, ref _isShowingSettings, value);
        }

        public bool IsShowingEmotes
        {
            get => _isShowingEmotes;
            set => Set(() => IsShowingEmotes, ref _isShowingEmotes, value);
        }

        public bool IsShowingLogin
        {
            get => _isShowingLogin;
            set => Set(() => IsShowingLogin, ref _isShowingLogin, value);
        }

        // Services as public properties

        public IChatService ChatService
        {
            get => _chatService;
            set => Set(() => ChatService, ref _chatService, value);
        }

        public IEmoteRepository EmoteRepository
        {
            get => _emoteRepository;
            set => Set(() => EmoteRepository, ref _emoteRepository, value);
        }

        public ISettingsRepository SettingsRepository
        {
            get => _settingsRepository;
            set => Set(() => SettingsRepository, ref _settingsRepository, value);
        }

        // Chatbar commands

        public string Message
        {
            get => _message;
            set => Set(() => Message, ref _message, value);
        }

        public bool LoggedIn
        {
            get => _loggedIn;
            set
            {
                Set(() => LoggedIn, ref _loggedIn, value);
                RaisePropertyChanged(nameof(ChatHint));
            }
        }

        public UserControl PopupView
        {
            get => _popupView;
            set => Set(() => PopupView, ref _popupView, value);
        }

        public string ChatHint => LoggedIn ? $"Write something {SettingsRepository.Username} ..." : "Log in to type in chat.";

        // Commands

        public RelayCommand ClearCommand { get; set; }

        public RelayCommand SendCommand { get; set; }

        public RelayCommand AutoCompleteCommand { get; set; }

        public RelayCommand SettingsCommand { get; set; }

        public RelayCommand UserlistCommand { get; set; }

        public RelayCommand LoginCommand { get; set; }

        public RelayCommand LogoutCommand { get; set; }

        public RelayCommand ShowLoginCommand { get; set; }

        public RelayCommand SaveCommand { get; set; }

        public RelayCommand RetrieveDetailsCommand { get; set; }

        public RelayCommand TrayCommand { get; set; }

        public RelayCommand PreviousMessageCommand { get; set; }

        public RelayCommand NextMessageCommand { get; set; }

        public RelayCommand EmotesCommand { get; set; }

        public RelayCommand<string> OpenWebsiteCommand { get; set; }

        public RelayCommand<Emote> UseEmoteCommand { get; set; }

        public RelayCommand ClosePopupCommand { get; set; }

        // Method logic

        private void PreviousMessage()
        {
            if (_commandHistoryIndex - 1 >= 0)
                Message = _commandHistory[--_commandHistoryIndex];
        }

        private void NextMessage()
        {

            if (_commandHistoryIndex >= _commandHistory.Count - 1)
            {
                Message = "";
                if (_commandHistoryIndex == _commandHistory.Count)
                    _commandHistoryIndex++;
            }
            else Message = _commandHistory[++_commandHistoryIndex];
        }

        private async void AutoComplete()
        {
            Message = await WordSuggestor.CorrectMessage(Message);
        }

        private async void Logout()
        {
            _settingsRepository.Username = "";
            _settingsRepository.Sid = "";
            _settingsRepository.RememberMe = "";
            LoggedIn = false;
            await SettingsRepository.Save();
        }

        private async void Login()
        {
            if (_settingsRepository.Username.Length == 0)
            {
                (var successful, var username) = await ChatService.GetUsername();
                if (successful)
                {
                    _settingsRepository.Username = username;
                    ChatService.Login(SettingsRepository.Sid, SettingsRepository.RememberMe);
                    LoggedIn = true;
                }
            }
            await SettingsRepository.Save();
        }

        private void SendMessage()
        {
            if (!string.IsNullOrEmpty(Message))
            {
                ChatService.SendMessage(Message);
                _commandHistory.Add(Message);
                _commandHistoryIndex = _commandHistory.Count;
            }
            Message = "";
        }
    }
}