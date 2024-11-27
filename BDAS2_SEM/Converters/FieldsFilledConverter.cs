using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Converters
{
    using System;
using System.Globalization;
using System.Windows.Data;

public class FieldsFilledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        // Перевіряємо, чи всі значення не порожні або не нульові
        foreach (var value in values)
        {
            if (value == null)
                return false;

            if (value is string strValue)
            {
                if (string.IsNullOrWhiteSpace(strValue))
                    return false;
            }
            else if (value is int intValue)
            {
                if (intValue <= 0)
                    return false;
            }
            else if (value is long longValue)
            {
                if (longValue <= 0)
                    return false;
            }
            else if (value is DateTime dateValue)
            {
                if (dateValue == DateTime.MinValue)
                    return false;
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
}
