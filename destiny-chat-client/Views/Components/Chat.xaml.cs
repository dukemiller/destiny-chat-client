using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace destiny_chat_client.Views.Components
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public static bool AutoScroll = true;

        public static bool NewUnreadMessages;

        public static bool UserScrolled;

        public static bool ViewAtBottom;

        private readonly ScrollViewer _scrollViewer;

        public Chat()
        {
            InitializeComponent();
            _scrollViewer = (ScrollViewer)GetDescendantByType(this, typeof(ScrollViewer));
            _scrollViewer.ScrollChanged += ScrollViewer_OnScrollChanged;
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var lbi = sender as ListBoxItem;
                if (lbi != null)
                {
                    lbi.IsSelected = true;
                    lbi.Focus();
                    // MessageBox.Show("WEHAWE");
                    // lb.SelectedItems.Add(lbi);
                }
            }
        }

        // http://stackoverflow.com/questions/16683477/change-scrollviewer-template-in-listbox
        // http://stackoverflow.com/questions/2984803/how-to-automatically-scroll-scrollviewer-only-if-the-user-did-not-change-scrol
        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            NewUnreadMessages = Math.Abs(e.ExtentHeightChange) > 0.1;
            UserScrolled = Math.Abs(e.ExtentHeightChange) < 0.1;
            ViewAtBottom = Math.Abs(_scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight) < 0.1;

            if (UserScrolled)
            {
                if (ViewAtBottom)
                {
                    NewUnreadMessages = false;
                    MoreMessagesPane.Visibility = Visibility.Collapsed;
                }
                AutoScroll = ViewAtBottom;
            }

            if (AutoScroll && NewUnreadMessages)
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ExtentHeight);

            if (NewUnreadMessages)
                MoreMessagesPane.Visibility = ViewAtBottom ? Visibility.Collapsed : Visibility.Visible;
        }

        // http://stackoverflow.com/questions/10293236/accessing-the-scrollviewer-of-a-listbox-from-c-sharp
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
                return null;

            if (element.GetType() == type)
                return element;

            Visual foundElement = null;

            (element as FrameworkElement)?.ApplyTemplate();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }

        private void GoToBottom(object sender, MouseButtonEventArgs e)
        {
            MoreMessagesPane.Visibility = Visibility.Collapsed;
            AutoScroll = true;
            _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ExtentHeight);
        }
    }
}
