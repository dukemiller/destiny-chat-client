using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using destiny_chat_client.Enums;
using destiny_chat_client.Repositories.Interfaces;
using GalaSoft.MvvmLight.Ioc;

namespace destiny_chat_client.Classes
{

    public class BoolOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? 1.0 : 0.4;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToBoolean(value) ? "True" : "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => System.Convert
            .ToBoolean(value)
            ? Visibility.Visible
            : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture) => value != null && ((Visibility)value) == Visibility.Visible;
    }


    public class EmoteConverter : IValueConverter
    {
        private static readonly IEmoteRepository EmoteRepository = SimpleIoc.Default.GetInstance<IEmoteRepository>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EmoteRepository.GetSourceUri((Emote)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}