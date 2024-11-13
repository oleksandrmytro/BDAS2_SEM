using BDAS2_SEM.Model.Enum;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            // Ensure the value is an enum
            if (!Enum.IsDefined(typeof(Role), value))
                return value.ToString().ToLower();

            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi == null)
                return value.ToString().ToLower();

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString().ToLower();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Role.NEOVERENY;

            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                string description = RoleService.GetRoleName(role);
                if (description.Equals(value.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    return role;
            }
            return Role.NEOVERENY;
        }
    }
}