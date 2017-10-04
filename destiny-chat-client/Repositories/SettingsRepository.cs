using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.ViewModels;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace destiny_chat_client.Repositories
{
    [Serializable]
    public class SettingsRepository : ObservableObject, ISettingsRepository
    {
        private static readonly string SavePath = Path.Combine(MainWindowViewModel.ApplicationPath, "settings.json");

        private string _rememberMe = "";

        private string _sid = "";

        private string _username = "";

        private string _highlightWords = "";

        private string _highlightNames = "Destiny";

        private bool _showFlair = true;

        private bool _showTimestamp;

        private bool _soundOnMention;

        private bool _flashOnMention;

        private bool _exitToTray;

        private bool _trayAlwaysOpen = true;

        private bool _minimizeToTray;

        private bool _toastOnMention;

        // 

        [JsonProperty("exit_to_tray")]
        public bool ExitToTray
        {
            get => _exitToTray;
            set => Set(() => ExitToTray, ref _exitToTray, value);
        }

        [JsonProperty("minimize_to_tray")]
        public bool MinimizeToTray
        {
            get => _minimizeToTray;
            set => Set(() => MinimizeToTray, ref _minimizeToTray, value);
        }

        [JsonProperty("tray_always_open")]
        public bool TrayAlwaysOpen
        {
            get => _trayAlwaysOpen;
            set => Set(() => TrayAlwaysOpen, ref _trayAlwaysOpen, value);
        }

        [JsonProperty("sid")]
        public string Sid
        {
            get => _sid;
            set
            {
                Set(() => Sid, ref _sid, value);
                RaisePropertyChanged(nameof(LoggedIn));
            }
        }

        [JsonProperty("remember_me")]
        public string RememberMe
        {
            get => _rememberMe;
            set
            {
                Set(() => RememberMe, ref _rememberMe, value);
                RaisePropertyChanged(nameof(LoggedIn));
            }
        }

        [JsonProperty("show_flair")]
        public bool ShowFlair
        {
            get => _showFlair;
            set => Set(() => ShowFlair, ref _showFlair, value);
        }

        [JsonProperty("show_timestamp")]
        public bool ShowTimestamp
        {
            get => _showTimestamp;
            set => Set(() => ShowTimestamp, ref _showTimestamp, value);
        }

        [JsonProperty("flash_on_mention")]
        public bool FlashOnMention
        {
            get => _flashOnMention;
            set => Set(() => FlashOnMention, ref _flashOnMention, value);
        }

        [JsonProperty("sound_on_mention")]
        public bool SoundOnMention
        {
            get => _soundOnMention;
            set => Set(() => SoundOnMention, ref _soundOnMention, value);
        }

        [JsonProperty("toast_on_mention")]
        public bool ToastOnMention
        {
            get => _toastOnMention;
            set => Set(() => ToastOnMention, ref _toastOnMention, value);
        }

        [JsonProperty("username")]
        public string Username
        {
            get => _username;
            set
            {
                Set(() => Username, ref _username, value);
                RaisePropertyChanged(nameof(LoggedIn));
            }
        }

        [JsonProperty("highlight_words")]
        public string HighlightWords
        {
            get => _highlightWords;
            set => Set(() => HighlightWords, ref _highlightWords, value);
        }

        [JsonProperty("highlight_names")]
        public string HighlightNames
        {
            get => _highlightNames;
            set => Set(() => HighlightNames, ref _highlightNames, value);
        }

        [JsonIgnore]
        public IEnumerable<string> HighlightWordsList => HighlightWords
            .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
        
        [JsonIgnore]
        public IEnumerable<string> HighlightNamesList => HighlightNames
            .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);

        [JsonIgnore]
        public bool LoggedIn => (!string.IsNullOrEmpty(Sid) || !string.IsNullOrEmpty(RememberMe))
                                && !string.IsNullOrEmpty(Username);

        // 

        public async Task Save()
        {
            using (var stream = new StreamWriter(SavePath))
                await stream.WriteAsync(JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static SettingsRepository Load()
        {
            if (!Directory.Exists(MainWindowViewModel.ApplicationPath))
                Directory.CreateDirectory(MainWindowViewModel.ApplicationPath);

            if (File.Exists(SavePath))
                using (var stream = new StreamReader(SavePath))
                    return JsonConvert.DeserializeObject<SettingsRepository>(stream.ReadToEnd());

            return new SettingsRepository();
        }
    }
}