using BDAS2_SEM.Model.Enum;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    public class StatusToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Status status && parameter is string targetStatus)
            {
                if (Enum.TryParse(targetStatus, out Status target))
                {
                    return status == target ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}