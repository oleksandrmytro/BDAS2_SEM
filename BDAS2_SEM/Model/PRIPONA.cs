using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class PRIPONA : INotifyPropertyChanged
    {
        private int idPripona;
        private string typ;

        public int IdPripona
        {
            get { return idPripona; }
            set
            {
                if (idPripona != value)
                {
                    idPripona = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Typ
        {
            get { return typ; }
            set
            {
                if (typ != value)
                {
                    typ = value;
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