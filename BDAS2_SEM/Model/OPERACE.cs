using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class OPERACE : INotifyPropertyChanged
    {
        private int idOperace;
        private string nazev;
        private DateTime datum;
        private int diagnozaId;

        public int IdOperace
        {
            get { return idOperace; }
            set
            {
                if (idOperace != value)
                {
                    idOperace = value;
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

        public DateTime Datum
        {
            get { return datum; }
            set
            {
                if (datum != value)
                {
                    datum = value;
                    OnPropertyChanged();
                }
            }
        }

        public int DiagnozaId
        {
            get { return diagnozaId; }
            set
            {
                if (diagnozaId != value)
                {
                    diagnozaId = value;
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