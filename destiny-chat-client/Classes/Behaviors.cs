using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace destiny_chat_client.Classes
{
    // http://stackoverflow.com/questions/1977929/wpf-listbox-with-a-listbox-ui-virtualization-and-scrolling
    public static class PixelBasedScrollingBehavior
    {
        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PixelBasedScrollingBehavior), new UIPropertyMetadata(false, HandleIsEnabledChanged));

        private static void HandleIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vsp = d as VirtualizingStackPanel;

            if (vsp == null)
                return;

            var property = typeof(VirtualizingStackPanel).GetProperty("IsPixelBased",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (property == null)
                throw new InvalidOperationException("Pixel-based scrolling behaviour hack no longer works!");

            property.SetValue(vsp, (bool) e.NewValue, new object[0]);
        }
    }
}