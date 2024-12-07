using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    public class MistnostConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return "Unknown";

            var mistnostId = values[0] as int?;
            var mistnostList = values[1] as IEnumerable<MISTNOST>;

            if (mistnostId == null || mistnostList == null)
                return "Unknown";

            var mistnost = mistnostList.FirstOrDefault(m => m.IdMistnost == mistnostId);
            return mistnost?.Cislo.ToString() ?? "Unknown";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
