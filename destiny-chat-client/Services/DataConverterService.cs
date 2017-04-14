using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using destiny_chat_client.Models;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using destiny_chat_client.ViewModels;
using destiny_chat_client.Views;
using destiny_chat_client.Views.Components;
using GalaSoft.MvvmLight.Ioc;
using Message = destiny_chat_client.Models.Message;

namespace destiny_chat_client.Services
{
    /// <summary>
    ///     A service to convert a chat message into a formatted paragraph.
    /// </summary>
    public class DataConverterService : IDataConverterService
    {
        private static ISettingsRepository _settingsRepository;

        private static IChatService _chatService;

        private static IFlairRepository _flairRepository;

        private static IEmoteRepository _emoteRepository;

        private readonly ISoundRepository _soundRepository;

        private static readonly ChatViewModel ChatViewModel = SimpleIoc.Default.GetInstance<ChatViewModel>();

        private static MainWindow MainWindow => (MainWindow)Application.Current.MainWindow;

        private readonly FlashWindowHelper _helper;

        // 

        public DataConverterService(ISettingsRepository settingsRepository,
            IChatService chatService,
            IFlairRepository flairRepository,
            IEmoteRepository emoteRepository,
            ISoundRepository soundRepository
        )
        {
            _settingsRepository = settingsRepository;
            _chatService = chatService;
            _flairRepository = flairRepository;
            _emoteRepository = emoteRepository;
            _soundRepository = soundRepository;
            _helper = new FlashWindowHelper(Application.Current);
        }

        // The construction

        public Paragraph MessageToParagraph(Message message)
        {
            if (_settingsRepository.LoggedIn && _settingsRepository.Username.Equals(message.Username))
                message.IsUser = true;

            // The holder of the message block
            var paragraph = new Paragraph();

            // ## COMPONENT 1: timestamp
            paragraph.Inlines.Add(Timestamp(message));
            // ##

            // ## COMPONENT 2 & 3: flair and username
            // Squash username if the last message is from the same person, also dont show flair
            if (LastMessage != null
                && ((Message)LastMessage.DataContext).Username.Equals(message.Username))
            {
                paragraph.Inlines.Add(new Run(">") { Foreground = Brushes.DarkSlateGray });
                paragraph.Inlines.Add(new Run(" ") { Foreground = Brushes.White });
            }

            else
            {
                foreach (var flair in Flair(message))
                    paragraph.Inlines.Add(flair);
                paragraph.Inlines.Add(Username(message));
                paragraph.Inlines.Add(new Run(": ") { Foreground = Brushes.White });
            }
            // ##

            // ## COMPONENT 4: parsed message
            paragraph.Inlines.Add(Message(message));
            // ##

            // Set a highlight
            if (_settingsRepository.HighlightWordsList.Any(word => message.Text.ToLower().Contains(word.ToLower())))
                message.Mention = true;

            // Settings notifications
            if (message.Mention && MainWindow.WindowState == WindowState.Minimized)
            {
                if (_settingsRepository.FlashOnMention)
                    _helper.FlashApplicationWindow();

                if (_settingsRepository.SoundOnMention)
                    _soundRepository.PlayNotificationSound();
            }

            paragraph.KeepTogether = false;
            return paragraph;
        }

        public Views.Components.Message LastMessage { get; set; }

        public ComboMessage Combo { get; set; }

        // The models main elements

        private static Inline Timestamp(UserData model)
        {
            var date = DateTimeOffset
                .FromUnixTimeMilliseconds(model.Timestamp)
                .DateTime.ToLocalTime();

            var timestamp = new Run($"{date:t}  ")
            {
                Foreground = new SolidColorBrush { Color = Colors.DimGray },
                ToolTip = date.ToString("R"),
                Tag = "Timestamp"
            };

            BindingOperations.SetBinding(timestamp, TextElement.FontSizeProperty, TimeStampVisible);

            return timestamp;
        }

        private static IEnumerable<Inline> Flair(UserData model)
        {
            var flairs = new List<Flair>();

            foreach (var feature in model.Features)
                if (_flairRepository.TryGetFlair(feature, out var flair))
                    flairs.Add(flair);

            if (flairs.Contains(Enums.Flair.minitwitch) && flairs.Contains(Enums.Flair.subscriber))
                flairs.Remove(Enums.Flair.subscriber);

            if (flairs.Count(flair => flair.ToString().StartsWith("subscriber")) > 1)
                foreach (var flair in flairs.GroupBy(flair => flair.ToString().StartsWith("subscriber"))
                    .First(group => group.Key)
                    .OrderByDescending(flair => (int)flair)
                    .Skip(1))
                    flairs.Remove(flair);

            return flairs.Select(Flair);
        }

        private static Inline Username(UserData model)
        {
            var run = new Run($"{model.Username}")
            {
                FontWeight = FontWeights.SemiBold,
                FontFamily = Styling.UsernameFont,
                Foreground = Styling.UsernameDefault,
                Tag = "Username"
            };

            // Changing foreground

            var features = model.Features;

            if (features.Contains(Feature.Admin))
                run.Foreground = Styling.UsernameAdmin;

            else if (features.Contains(Feature.Bot))
                run.Foreground = Styling.UsernameBot;

            else if (features.Contains(Feature.SubscriberT4))
                run.Foreground = Styling.UsernameT4;

            else if (features.Contains(Feature.SubscriberT3))
                run.Foreground = Styling.UsernameT3;

            else if (features.Contains(Feature.Subscriber))
                run.Foreground = Styling.UsernameSubOrT1;

            else if (features.Contains(Feature.SubscriberT2))
                run.Foreground = Styling.UsernameT2;

            else if (features.Contains(Feature.SubscriberT1)
                     || features.Contains(Feature.SubscriberT0))
                run.Foreground = Styling.UsernameSubOrT1;

            run.PreviewMouseDown += (sender, e) =>
            {
                ChatViewModel.HighlightCommand.Execute(model.Username);
                e.Handled = true;
            };

            var hyperlink = new Hyperlink(run)
            {
                Foreground = Styling.TextColor,
                TextDecorations = null
            };

            hyperlink.MouseEnter += (sender, args) => hyperlink.TextDecorations = Styling.Underline;
            hyperlink.MouseLeave += (sender, args) => hyperlink.TextDecorations = null;

            return hyperlink;
        }

        private static Inline Message(Message model)
        {
            var tokenized = new Queue<(Token Token, string Word)>(
                model.Text.Trim()
                    .Split(' ')
                    .Select(word => new ValueTuple<Token, string>(TokenParse(word), word))
            );

            var builder = new StringBuilder();
            var token = Token.None;
            var span = new Span { Tag = "Message" };

            while (tokenized.Count > 0)
            {
                var phrase = tokenized.Dequeue();

                if (phrase.Token == Token.Text)
                {
                    // if not the start of the sentence, append a space before the next word
                    if (token != Token.None)
                        builder.Append(" ");
                    builder.Append(phrase.Word);
                }

                else
                {
                    // If the special token is following something else, add a space to
                    // the builder but not if its the start of the sentence
                    if (token != Token.None)
                        builder.Append(" ");

                    if (builder.Length > 0)
                    {
                        span.Inlines.Add(Text(model, builder.ToString()));
                        builder.Clear();
                    }

                    span.Inlines.Add(CreateSpecialLine(phrase.Token, model, model.Username, phrase.Word));
                }

                token = phrase.Token;
            }

            span.Inlines.Add(Text(model, builder.ToString()));

            return span;
        }

        // Parsing

        private static Token TokenParse(string fragment)
        {
            lock (_chatService.Users)
            {
                if (_chatService.Users.ToList().Any(user => user.Username.Equals(fragment)))
                    return Token.Username;
            }

            // make rules less strict
            if (fragment.StartsWith("http"))
                return Token.Hyperlink;

            if (_emoteRepository.HasEmote(fragment))
                return Token.Emoticon;

            return Token.Text;
        }

        private static Inline CreateSpecialLine(Token token, Message model, string username, string text)
        {
            switch (token)
            {
                case Token.Username:
                    return InlineUsername(model, username, text);

                case Token.Hyperlink:
                    return Hyperlink(text);

                case Token.Emoticon:
                    return Emoticon(text);

                default:
                    return new Run(text);
            }
        }

        // Creating the elements

        private static Inline Text(Message model, string word)
        {
            var run = new Run(word);
            run.MouseDown += TextMouseDown;
            BindingOperations.SetBinding(run, TextElement.ForegroundProperty, Highlighted(model));
            return run;
        }

        private static void TextMouseDown(object sender, MouseButtonEventArgs args) => ChatViewModel.RemoveHighlightCommand.Execute(null);

        private static Inline Emoticon(string text)
        {
            var emote = _emoteRepository.GetEmote(text);
            var bitmap = new BitmapImage(_emoteRepository.GetSourceUri(emote));
            var image = new Image
            {
                Source = bitmap,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = emote,
                Tag = $"Emote~{emote}",
                Height = 30, //bitmap.Height,
                Width = 30 //bitmap.Width
            };
            return new InlineUIContainer(image) { BaselineAlignment = BaselineAlignment.Center };
        }

        private static Inline Hyperlink(string text)
        {
            var run = new Run(text) { Foreground = Styling.HyperlinkUnfocusedColor };
            run.MouseEnter += (sender, args) => run.Foreground = Styling.HyperlinkFocusedColor;
            run.MouseLeave += (sender, args) => run.Foreground = Styling.HyperlinkUnfocusedColor;
            run.MouseDown += (sender, args) => Process.Start(text);
            var hyperlink = new Hyperlink(run) { TextDecorations = null };
            return hyperlink;
        }

        private static Inline InlineUsername(Message model, string username, string text)
        {
            // if they say your name, highlight the message
            if (text.Equals(_settingsRepository.Username))
                model.Mention = true;

            var run = new Run(text) { Foreground = Styling.TextColor };

            BindingOperations.SetBinding(run, TextElement.ForegroundProperty, Highlighted(model));
            run.MouseEnter += (sender, args) => run.TextDecorations = Styling.Underline;
            run.MouseLeave += (sender, args) => run.TextDecorations = null;
            run.MouseDown += (sender, args) => ChatViewModel.HighlightUsernameCommand.Execute((username, text));

            return run;
        }

        private static Inline Flair(Flair flair)
        {
            var bitmap = new BitmapImage(_flairRepository.GetSourceUri(flair));
            var image = new Image
            {
                Source = bitmap,
                VerticalAlignment = VerticalAlignment.Center,
                ToolTip = flair,
                Tag = "Flair",
                Margin = new Thickness(0, 0, 4, 0),
                Height = 20, //bitmap.Height,
                Width = 20 //bitmap.Width
            };
            BindingOperations.SetBinding(image, UIElement.VisibilityProperty, ShowFlair);
            return new InlineUIContainer(image) { BaselineAlignment = BaselineAlignment.Center };
        }

        // Bindings

        private static Binding Highlighted(Message model)
        {
            var binding = new Binding
            {
                Source = model,
                Path = new PropertyPath("Mention"),
                Converter = new FontColorConverter(),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };

            return binding;
        }

        private static Binding _timeStampVisible;

        private static Binding TimeStampVisible => _timeStampVisible ?? (_timeStampVisible = new Binding
        {
            Source = _settingsRepository,
            Path = new PropertyPath("ShowTimestamp"),
            Converter = new TimestampFontConverter(),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });

        private static Binding _showFlair;

        private static Binding ShowFlair => _showFlair ?? (_showFlair = new Binding
        {
            Source = _settingsRepository,
            Path = new PropertyPath("ShowFlair"),
            Converter = new BoolVisibilityConverter(),
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
        });
    }

    internal static class Styling
    {
        public static readonly FontFamily UsernameFont = new FontFamily("Trebuchet MS");

        // 

        public static readonly TextDecorationCollection Underline = new TextDecorationCollection(new[]
            {TextDecorations.Underline.FirstOrDefault()});

        // 

        public static readonly SolidColorBrush UsernameDefault = (SolidColorBrush)new BrushConverter().ConvertFrom("#CACACA");

        public static readonly SolidColorBrush UsernameAdmin = (SolidColorBrush)new BrushConverter().ConvertFrom("#B91010");

        public static readonly SolidColorBrush UsernameBot = Brushes.YellowGreen;

        public static readonly SolidColorBrush UsernameT4 =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#A427D6");

        public static readonly SolidColorBrush UsernameT3 =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#0060FF");

        public static readonly SolidColorBrush UsernameT2 = Brushes.SlateBlue;

        public static readonly SolidColorBrush UsernameSubOrT1 = Brushes.CornflowerBlue;

        // 

        // public static readonly SolidColorBrush Text

        public static readonly SolidColorBrush TextColor =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#759999");

        public static readonly SolidColorBrush HyperlinkUnfocusedColor =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#03C2FF");

        public static readonly SolidColorBrush HyperlinkFocusedColor =
            (SolidColorBrush)new BrushConverter().ConvertFrom("#0088CC");
    }
}