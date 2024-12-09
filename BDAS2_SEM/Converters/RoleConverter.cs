using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Converters
{
    // Převádí ID role na název role na základě seznamu rolí.
    public class RoleConverter : IMultiValueConverter
    {
        // Metoda Convert převádí ID role na název role.
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Ověření počtu hodnot a jejich platnosti
            if (values.Length < 2)
                return "Unknown Role";

            var roleIdValue = values[0];
            var rolesValue = values[1] as IEnumerable<ROLE>;

            if (roleIdValue == null || rolesValue == null)
                return "Unknown Role";

            // Vyhledání role na základě ID
            if (roleIdValue is int roleId)
            {
                var role = rolesValue.FirstOrDefault(r => r.IdRole == roleId);
                if (role != null)
                    return role.Nazev;
            }

            return "Unknown Role";
        }

        // Metoda ConvertBack převádí název role zpět na ID role.
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value is string roleName && parameter is IEnumerable<ROLE> roles)
            {
                var role = roles.FirstOrDefault(r => r.Nazev.Equals(roleName, StringComparison.OrdinalIgnoreCase));
                if (role != null)
                    return new object[] { role.IdRole };
            }

            return new object[] { Binding.DoNothing };
        }
    }
}
