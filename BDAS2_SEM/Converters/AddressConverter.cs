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
    // Třída AddressConverter převádí ID adresy a seznam adres na čitelný formát adresy.
    public class AddressConverter : IMultiValueConverter
    {
        // Metoda Convert převádí vstupní hodnoty na formátovanou adresu.
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return "Unknown Address";

            if (!(values[0] is int adresaId))
                return "Unknown Address";

            var adresaList = values[1] as ObservableCollection<ADRESA>;
            if (adresaList == null)
                return "Unknown Address";

            var adresa = adresaList.FirstOrDefault(a => a.IdAdresa == adresaId);
            if (adresa == null)
                return "Unknown Address";

            return $"{adresa.Ulice} {adresa.CisloPopisne}, {adresa.Mesto}, {adresa.Stat}";
        }

        // Metoda ConvertBack není implementována, protože zpětný převod není potřeba.
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
