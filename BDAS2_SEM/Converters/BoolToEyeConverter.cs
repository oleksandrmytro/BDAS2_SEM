using System;
using System.Globalization;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    public class BoolToEyeConverter : IValueConverter
    {
        // Ви можете змінити ці значення на іконки або зображення
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isPasswordVisible = (bool)value;
            return isPasswordVisible ? "🙈" : "👁"; // Або використовуйте Path, Image тощо
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}