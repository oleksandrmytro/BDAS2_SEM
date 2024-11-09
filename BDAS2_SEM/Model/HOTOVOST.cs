using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class HOTOVOST : INotifyPropertyChanged
    {
        private int idPlatba;
        private decimal prijato;
        private decimal vraceno;

        public int IdPlatba
        {
            get { return idPlatba; }
            set
            {
                if (idPlatba != value)
                {
                    idPlatba = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Prijato
        {
            get { return prijato; }
            set
            {
                if (prijato != value)
                {
                    prijato = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Vraceno
        {
            get { return vraceno; }
            set
            {
                if (vraceno != value)
                {
                    vraceno = value;
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