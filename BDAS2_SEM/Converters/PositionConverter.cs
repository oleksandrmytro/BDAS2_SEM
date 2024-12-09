using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace BDAS2_SEM.Converters
{
    // Převádí ID pozice a seznam pozic na název pozice.
    public class PositionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return "Unknown Position";

            if (!(values[0] is int poziceId))
                return "Unknown Position";

            var poziceList = values[1] as ObservableCollection<POZICE>;
            if (poziceList == null)
                return "Unknown Position";

            var pozice = poziceList.FirstOrDefault(p => p.IdPozice == poziceId);
            if (pozice == null)
                return "Unknown Position";

            return pozice.Nazev;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
