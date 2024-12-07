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
    public class StatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return "Unknown";

            var statusId = values[0] as int?;
            var statusList = values[1] as IEnumerable<STATUS>;

            if (statusList == null)
                return "Status list is null";
            if (!statusList.Any())
                return "Status list is empty";
            if (statusId == null)
                return "Status ID is null";

            var status = statusList.FirstOrDefault(s => s.IdStatus == statusId);
            if (status == null)
                return $"Status not found for ID {statusId}";

            return status.Nazev;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
