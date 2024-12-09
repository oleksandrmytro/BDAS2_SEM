using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Converters
{
    // Převádí objekt typu STATUS na hodnotu bool.
    public class StatusToBooleanConverter : IValueConverter
    {
        // Metoda Convert porovnává status s cílovým parametrem a vrací Visibility.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is STATUS status && parameter is string targetStatus)
            {
                return status == value ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        // Zpětný převod není implementován
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
