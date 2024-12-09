using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace BDAS2_SEM.Converters
{
    // Převádí hodnoty typu decimal? na řetězec a naopak.
    public class DecimalNullableConverter : IValueConverter
    {
        // Převádí hodnotu typu decimal? na řetězec.
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString(); // Null vrací prázdný řetězec.
        }

        // Převádí hodnotu z řetězce zpět na decimal?.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
                return null;

            if (decimal.TryParse(value.ToString(), out var result))
                return result;

            return DependencyProperty.UnsetValue; // Vrátí neplatnou hodnotu, pokud převod selže.
        }
    }
}
