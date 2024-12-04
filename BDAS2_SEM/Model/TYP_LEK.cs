using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model.Enum
{
    public class TYP_LEK : INotifyPropertyChanged
    {
        private int idTypLek;
        private string nazev;

        public int IdTypLek
        {
            get { return idTypLek; }
            set
            {
                if (idTypLek != value)
                {
                    idTypLek = value;
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