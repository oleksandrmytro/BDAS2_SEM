using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    // Převádí hodnoty bool na opačnou viditelnost (Visibility).
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = !(bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }
}
