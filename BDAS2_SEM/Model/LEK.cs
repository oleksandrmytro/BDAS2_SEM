using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class LEK : INotifyPropertyChanged
    {
        private int idLek;
        private string nazev;
        private int mnozstvi;
        private decimal cena;
        private int typLekId;

        public int IdLek
        {
            get { return idLek; }
            set
            {
                if (idLek != value)
                {
                    idLek = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Nazev
        {
            get { return nazev; }
            set
            {
                if (nazev != value)
                {
                    nazev = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Mnozstvi
        {
            get { return mnozstvi; }
            set
            {
                if (mnozstvi != value)
                {
                    mnozstvi = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Cena
        {
            get { return cena; }
            set
            {
                if (cena != value)
                {
                    cena = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TypLekId
        {
            get { return typLekId; }
            set
            {
                if (typLekId != value)
                {
                    typLekId = value;
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
