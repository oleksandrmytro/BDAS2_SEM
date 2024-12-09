using System;
using System.Globalization;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    // Třída BoolToEyeConverter převádí hodnotu typu bool na symbol nebo ikonu, která reprezentuje viditelnost hesla.
    public class BoolToEyeConverter : IValueConverter
    {
        // Metoda Convert převádí hodnotu true/false na odpovídající symbol.
        // Např. true → "🙈" (heslo skryté), false → "👁" (heslo viditelné).
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPasswordVisible = (bool)value;
            return isPasswordVisible ? "🙈" : "👁"; // Může být nahrazeno cestou k obrázku nebo ikonou.
        }

        // Metoda ConvertBack není implementována, protože zpětný převod není potřeba.
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
