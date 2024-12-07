using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
        public class TypLekMultiConverter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (values.Length < 2)
                    return string.Empty;

                var idValue = values[0];
                var typLeks = values[1] as ObservableCollection<TYP_LEK>;

                if (idValue == null || typLeks == null)
                    return string.Empty;

                if (idValue is int id)
                {
                    var typLek = typLeks.FirstOrDefault(t => t.IdTypLek == id);
                    return typLek?.Nazev ?? string.Empty;
                }

                return string.Empty;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
                => throw new NotImplementedException();
        }
  

}
