using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using destiny_chat_client.Enums;
using destiny_chat_client.Models;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace destiny_chat_client.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        public ISettingsRepository SettingsRepository { get; }

        // 

        private bool _highlighting;

        private string _highlightTarget;

        private int _messagesReceived;

        private ObservableCollection<string> _highlightedUsernames;

        private ObservableCollection<string> _highlightedMentionedUsernames;

        // 

        public ChatViewModel(IChatService chatService, ISettingsRepository settingsRepository)
        {
            SettingsRepository = settingsRepository;
            chatService.MessageReceived = AddMessage;
            chatService.ErrorReceived = ErrorMessage;

            HighlightedUsernames = new ObservableCollection<string>();
            HighlightedMentionedUsernames = new ObservableCollection<string>();

            HighlightCommand = new RelayCommand<string>(Highlight);
            RemoveHighlightCommand = new RelayCommand(Unhighlight);
            HighlightUsernameCommand = new RelayCommand<ValueTuple<string, string>>(HighlightUsername);

            Chat = new ObservableCollection<Message>();

            if (!IsInDesignModeStatic)
                chatService.StartReceivingMessages();
        }

        // 

        public RelayCommand RemoveHighlightCommand { get; set; }

        public RelayCommand<string> HighlightCommand { get; set; }

        public RelayCommand<ValueTuple<string, string>> HighlightUsernameCommand { get; set; }

        public ObservableCollection<string> HighlightedUsernames
        {
            get => _highlightedUsernames;
            set => Set(() => HighlightedUsernames, ref _highlightedUsernames, value);
        }

        public ObservableCollection<string> HighlightedMentionedUsernames
        {
            get => _highlightedMentionedUsernames;
            set => Set(() => HighlightedMentionedUsernames, ref _highlightedMentionedUsernames, value);
        }

        public bool Highlighting
        {
            get => _highlighting;
            set => Set(() => Highlighting, ref _highlighting, value);
        }

        public string HighlightTarget
        {
            get => _highlightTarget;
            set => Set(() => HighlightTarget, ref _highlightTarget, value);
        }

        private ObservableCollection<Message> _chat;

        public ObservableCollection<Message> Chat
        {
            get => _chat;
            set => Set(() => Chat, ref _chat, value);
        }

        public IEnumerable<string> ActiveUsers => Chat
            .OrderByDescending(message => message.Timestamp)
            .Select(message => message.Username)
            .Distinct();

        // 

        private void AddMessage(Data data)
        {
            Application.Current.Dispatcher.Invoke(async () =>
            {
                //if (Views.Components.Chat.AutoScroll && Chat.Count >= 189)
                //{
                //    if (Chat.Count > 200)
                //        foreach (var _ in Enumerable.Range(0, 5))
                //            Chat.RemoveAt(0);

                //    Chat.RemoveAt(0);
                //}

                if (Highlighting)
                {
                    if (HighlightedUsernames?.Count > 0)
                    {
                        if (HighlightedUsernames.Contains(data.Message.Username)
                            || HighlightedMentionedUsernames.Contains(data.Message.Username)
                            || (HighlightedMentionedUsernames.Contains(data.Message.Username) && HighlightedUsernames.Any(dim => data.Message.Text.Contains(dim))))
                            data.Message.Dimmed = false;
                        else
                            data.Message.Dimmed = true;
                    }

                    else if (!data.Message.Username.Equals(HighlightTarget))
                        data.Message.Dimmed = true;
                }

                Chat.Add(data.Message);
                _messagesReceived++;

                // Force a garbage collection every 500 messages
                // TODO: This will keep memory ~(250, 300) MB which is still terrible,
                // TODO: find the cause of this delayed garbage collection and
                // TODO: work on reducing the use of memory
                if (_messagesReceived % 500 == 0)
                {
                    await Task.Run(() =>
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    });
                    _messagesReceived = 0;
                }
            });
        }

        private static void ErrorMessage(ServerError serverError)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(serverError.Reason());
            });
        }

        // 

        private void Unhighlight()
        {
            HighlightedUsernames = new ObservableCollection<string>();
            HighlightedMentionedUsernames = new ObservableCollection<string>();
            HighlightTarget = null;
            Highlighting = false;
            foreach (var message in Chat)
                message.Dimmed = false;
        }

        private void Highlight(string username)
        {
            // Undim if clicking the same target or passing null
            if (username == null || HighlightTarget?.Equals(username) == true)
                Unhighlight();

            else
            {
                // If switching, first undim
                if (!HighlightTarget?.Equals(username) == true)
                    Unhighlight();

                // Then dim
                Highlighting = true;
                HighlightTarget = username;
                foreach (var message in Chat)
                {
                    if (!message.Username.Equals(username))
                        message.Dimmed = true;
                }
            }
        }

        private void HighlightUsername(ValueTuple<string, string> highlightCall)
        {
            // something weird might be going on here for new messages

            if (!HighlightedUsernames.Contains(highlightCall.Item1))
                HighlightedUsernames.Add(highlightCall.Item1);

            if (!HighlightedMentionedUsernames.Contains(highlightCall.Item2))
                HighlightedMentionedUsernames.Add(highlightCall.Item2);

            else
            {
                HighlightedMentionedUsernames.Remove(highlightCall.Item2);
                foreach (var message in Chat)
                    if (message.Username.Equals(highlightCall.Item2))
                        message.Dimmed = true;
            }

            Highlighting = true;

            foreach (var message in Chat)
            {
                if (HighlightedUsernames.Contains(message.Username)
                    || HighlightedMentionedUsernames.Contains(message.Username)
                    || (HighlightedMentionedUsernames.Contains(message.Username) && HighlightedUsernames.Any(dim => message.Text.Contains(dim))))
                    message.Dimmed = false;
                else
                    message.Dimmed = true;
            }
        }
    }
}