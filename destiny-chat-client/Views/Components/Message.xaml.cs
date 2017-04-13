using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using destiny_chat_client.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace destiny_chat_client.Views.Components
{
    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message
    {
        private static readonly IDataConverterService DataService = SimpleIoc.Default.GetInstance<IDataConverterService>();

        private static readonly IEmoteRepository EmoteRepository = SimpleIoc.Default.GetInstance<IEmoteRepository>();

        private static readonly ChatViewModel ChatViewModel = SimpleIoc.Default.GetInstance<ChatViewModel>();

        private static bool _inCombo;

        private static int _comboCount = 1;

        // 

        public Message()
        {
            InitializeComponent();
            FadeIn();
        }

        /// <summary>
        ///     In an effort to reduce the grossly large memory leak, i'm doing all this here
        /// </summary>
        ~Message()
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                var border = (Border)Content;
                var textbox = (RichTextBox)border.Child;

                DataContextChanged -= Message_SetModel;
                textbox.PreviewMouseDown -= RichTextBoxDefocus;
                textbox.CommandBindings.Clear();
                border.PreviewMouseDown -= BorderDefocus;

                DataContext = null;
                textbox = null;
                border = null;
            });
        }

        // 

        private void Message_SetModel(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext != null)
            {
                var message = (Models.Message)((Message)sender).DataContext;
                var textbox = (RichTextBox)((Border)((Message)sender).Content).Child;
                var previous = DataService.LastMessage?.DataContext as Models.Message;

                EmoteComboCheck();
                textbox.Document.Blocks.Clear();
                textbox.Document.Blocks.Add(DataService.MessageToParagraph(message));
                DataService.LastMessage = this;

                // 

                void EmoteComboCheck()
                {
                    if (IsCascadingEmote())
                    {
                        // Add to known combo spot
                        if (_inCombo)
                        {
                            DataService.Combo.Run.Text = $"{++_comboCount}x";
                            DataService.Combo.Run.FontSize = Math.Min(12 + (0.25 * (_comboCount) - (2 * 0.25)), 20);
                        }

                        // start the combo
                        else
                        {
                            _inCombo = true;
                            _comboCount = 2;
                            var last = (RichTextBox)((Border)DataService.LastMessage.Content).Child;
                            var paragraph = (Paragraph)last.Document.Blocks.FirstBlock;
                            // var paragraph = ((RichTextBox) (Paragraph) DataService.LastMessage).Document.Blocks.FirstBlock;
                            DataService.Combo = new ComboMessage { Run = { Text = $"{_comboCount}x" } };

                            // Skip timestamp
                            var content = new Stack<Inline>(paragraph.Inlines.Skip(1));

                            // Don't remove last thing (emote)
                            content.Pop();

                            // Remove everything else
                            while (content.Count > 0)
                                paragraph.Inlines.Remove(content.Pop());

                            paragraph.Inlines.Add(DataService.Combo);
                        }

                        Visibility = Visibility.Collapsed;
                    }

                    // no combo
                    else
                    {
                        _inCombo = false;
                        _comboCount = 1;
                        DataService.Combo = null;
                    }

                    bool IsCascadingEmote()
                    {
                        if (previous == null)
                            return false;
                        var currentIsEmotion = EmoteRepository.HasEmote(message.Text);
                        var previousIsEmoticon = EmoteRepository.HasEmote(previous.Text);
                        var currentEqualsPrevious = previous.Text.Equals(message.Text);
                        return currentIsEmotion && previousIsEmoticon && currentEqualsPrevious;
                    }
                }
            }
        }

        private async void FadeIn()
        {
            for (var i = 0.10; i <= 1.00; i += 0.10)
            {
                Opacity = i;
                await Task.Delay(15);
            }
        }

        private void RichTextBoxDefocus(object sender, MouseButtonEventArgs e)
        {
            var element = sender as RichTextBox;
            var position = element?.GetPositionFromPoint(Mouse.GetPosition(element), false);
            var overText = position?.IsAtInsertionPosition ?? false;
            if (!overText)
            {
                ChatViewModel.RemoveHighlightCommand.Execute(null);
                e.Handled = true;
            }
        }

        private void BorderDefocus(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.DirectlyOver is Border)
                ChatViewModel.RemoveHighlightCommand.Execute(null);
        }

        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/e897e946-7845-48f9-b9f2-ac2ab1dd3d24/copy-image-tag-as-text-from-richtextbox?forum=wpf
        private void CommandBinding_OnExecuted(object sender, RoutedEventArgs e)
        {
            var selection = ((RichTextBox)sender).Selection;
            var navigator = selection.Start.GetPositionAtOffset(0, LogicalDirection.Forward);
            var end = selection.End;
            var buffer = new StringBuilder();
            var offset = 0;

            do
            {
                if (navigator == null)
                    continue;

                offset = navigator.GetOffsetToPosition(end);
                var context = navigator.GetPointerContext(LogicalDirection.Forward);

                if (context == TextPointerContext.Text)
                    buffer.Append(navigator.GetTextInRun(LogicalDirection.Forward), 0,
                        Math.Min(offset, navigator.GetTextRunLength(LogicalDirection.Forward)));

                else if (context == TextPointerContext.EmbeddedElement)
                {
                    var container = navigator.Parent as InlineUIContainer;
                    if (container?.Child is Image img)
                        buffer.Append(((string)img.Tag).Split('~')[1]);
                }

                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            } while (offset > 0);

            Clipboard.SetText(buffer.ToString());
        }
    }
}
