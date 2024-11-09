using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class DIAGNOZA : INotifyPropertyChanged
    {
        private int id_diagnoza;
        private string nazev;
        private string popis;

        public int IdDiagnoza
        {
            get { return id_diagnoza; }
            set
            {
                if (id_diagnoza != value)
                {
                    id_diagnoza = value;
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

        public string Popis
        {
            get { return popis; }
            set
            {
                if (popis != value)
                {
                    popis = value;
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