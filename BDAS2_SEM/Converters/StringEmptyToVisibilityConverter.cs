using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    // Převádí prázdný řetězec na Visibility.
    public class StringEmptyToVisibilityConverter : IValueConverter
    {
        // Metoda Convert kontroluje, zda je řetězec prázdný, a nastavuje Visibility.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;
            return string.IsNullOrEmpty(str) ? Visibility.Visible : Visibility.Collapsed;
        }

        // Zpětný převod není implementován
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
