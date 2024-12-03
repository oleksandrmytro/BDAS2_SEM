using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class MISTNOST : INotifyPropertyChanged
    {
        private int idMistnost;
        private int cislo;

        public int IdMistnost
        {
            get { return idMistnost; }
            set
            {
                if (idMistnost != value)
                {
                    idMistnost = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Cislo
        {
            get { return cislo; }
            set
            {
                if (cislo != value)
                {
                    cislo = value;
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