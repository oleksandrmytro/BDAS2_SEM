using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class PACIENT : INotifyPropertyChanged, ICloneable
    {
        private int idPacient;
        private string jmeno;
        private string prijmeni;
        private int? rodneCislo;
        private long telefon;
        private DateTime datumNarozeni;
        private string pohlavi;
        private int adresaId;
        private int userDataId;

        public int IdPacient
        {
            get { return idPacient; }
            set
            {
                if (idPacient != value)
                {
                    idPacient = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Jmeno
        {
            get { return jmeno; }
            set
            {
                if (jmeno != value)
                {
                    jmeno = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Prijmeni
        {
            get { return prijmeni; }
            set
            {
                if (prijmeni != value)
                {
                    prijmeni = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? RodneCislo
        {
            get { return rodneCislo; }
            set
            {
                if (rodneCislo != value)
                {
                    rodneCislo = value;
                    OnPropertyChanged();
                }
            }
        }

        public long Telefon
        {
            get { return telefon; }
            set
            {
                if (telefon != value)
                {
                    telefon = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DatumNarozeni
        {
            get { return datumNarozeni; }
            set
            {
                if (datumNarozeni != value)
                {
                    datumNarozeni = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Pohlavi
        {
            get { return pohlavi; }
            set
            {
                if (pohlavi != value)
                {
                    pohlavi = value;
                    OnPropertyChanged();
                }
            }
        }

        public int AdresaId
        {
            get { return adresaId; }
            set
            {
                if (adresaId != value)
                {
                    adresaId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int UserDataId
        {
            get { return userDataId; }
            set
            {
                if (userDataId != value)
                {
                    userDataId = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (obj is PACIENT other)
            {
                return IdPacient == other.IdPacient &&
                       Jmeno == other.Jmeno &&
                       Prijmeni == other.Prijmeni &&
                       RodneCislo == other.RodneCislo &&
                       Telefon == other.Telefon &&
                       DatumNarozeni == other.DatumNarozeni &&
                       Pohlavi == other.Pohlavi &&
                       AdresaId == other.AdresaId &&
                       UserDataId == other.UserDataId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = HashCode.Combine(IdPacient, Jmeno, Prijmeni, RodneCislo, Telefon);
            hash = HashCode.Combine(hash, DatumNarozeni, Pohlavi, AdresaId, UserDataId);
            return hash;
        }
    }
}