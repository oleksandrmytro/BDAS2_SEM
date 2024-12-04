using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class ZAMESTNANEC : INotifyPropertyChanged
    {
        private int idZamestnanec;
        private string jmeno;
        private string prijmeni;
        private long telefon;
        private int? nadrazenyZamestnanecId;
        private int adresaId;
        private int poziceId;
        private int userDataId;
        private int blobId; // Новое свойство

        public int IdZamestnanec
        {
            get { return idZamestnanec; }
            set
            {
                if (idZamestnanec != value)
                {
                    idZamestnanec = value;
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

        public int? NadrazenyZamestnanecId
        {
            get { return nadrazenyZamestnanecId; }
            set
            {
                if (nadrazenyZamestnanecId != value)
                {
                    nadrazenyZamestnanecId = value;
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

        public int PoziceId
        {
            get { return poziceId; }
            set
            {
                if (poziceId != value)
                {
                    poziceId = value;
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

        public int BlobId
        {
            get { return blobId; }
            set
            {
                if (blobId != value)
                {
                    blobId = value;
                    OnPropertyChanged();
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}