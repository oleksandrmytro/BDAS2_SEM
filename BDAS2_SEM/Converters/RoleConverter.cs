using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Converters
{
    public class RoleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return "Unknown Role";

            var roleIdValue = values[0];
            var rolesValue = values[1] as ObservableCollection<ROLE>;

            if (roleIdValue == null || rolesValue == null)
                return "Unknown Role";

            if (int.TryParse(roleIdValue.ToString(), out int roleId))
            {
                var role = rolesValue.FirstOrDefault(r => r.IdRole == roleId);
                if (role != null)
                    return role.Nazev;
            }

            return "Unknown Role";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
