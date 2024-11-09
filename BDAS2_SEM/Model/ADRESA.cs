using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class ADRESA : INotifyPropertyChanged
    {
        private int id_adresa;
        private string stat;
        private string mesto;
        private int psc;
        private string ulice;
        private int cislo_popisne;

        public int IdAdresa
        {
            get { return id_adresa; }
            set
            {
                if (id_adresa != value)
                {
                    id_adresa = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Stat
        {
            get { return stat; }
            set
            {
                if (stat != value)
                {
                    stat = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Mesto
        {
            get { return mesto; }
            set
            {
                if (mesto != value)
                {
                    mesto = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PSC
        {
            get { return psc; }
            set
            {
                if (psc != value)
                {
                    psc = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Ulice
        {
            get { return ulice; }
            set
            {
                if (ulice != value)
                {
                    ulice = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CisloPopisne
        {
            get { return cislo_popisne; }
            set
            {
                if (cislo_popisne != value)
                {
                    cislo_popisne = value;
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
