using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Converters
{
    public class StatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is STATUS status && parameter is string targetStatus)
            {
                return status == value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}