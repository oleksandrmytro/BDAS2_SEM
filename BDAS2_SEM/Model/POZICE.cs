using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class POZICE : INotifyPropertyChanged
    {
        private int idPozice;
        private string nazev;

        public int IdPozice
        {
            get { return idPozice; }
            set
            {
                if (idPozice != value)
                {
                    idPozice = value;
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}