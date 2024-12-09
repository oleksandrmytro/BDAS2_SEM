using System;
using System.Globalization;
using System.Windows.Data;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Repository;
using BDAS2_SEM.Model;
using System.Collections.Generic;

namespace BDAS2_SEM.Converters
{
    // Převádí ID pacienta na jeho jméno nebo příjmení.
    public class PacientIdToNameConverter : IValueConverter
    {
        private static IPacientRepository _pacientRepository;
        private static Dictionary<int, PACIENT> _pacientCache = new Dictionary<int, PACIENT>();

        public PacientIdToNameConverter()
        {
            if (_pacientRepository == null)
            {
                // Inicializace repozitáře pacientů (přizpůsobte svému prostředí)
                _pacientRepository = new PacientRepository("YourConnectionString");
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int pacientId)
            {
                if (!_pacientCache.TryGetValue(pacientId, out var pacient))
                {
                    pacient = _pacientRepository.GetPacientById(pacientId).Result;
                    if (pacient != null)
                    {
                        _pacientCache[pacientId] = pacient;
                    }
                }

                if (pacient != null)
                {
                    switch (parameter as string)
                    {
                        case "FirstName":
                            return pacient.Jmeno;
                        case "LastName":
                            return pacient.Prijmeni;
                    }
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
