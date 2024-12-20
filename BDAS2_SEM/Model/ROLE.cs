﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class ROLE : INotifyPropertyChanged
    {
        private int idRole;
        private string nazev;

        public int IdRole
        {
            get { return idRole; }
            set
            {
                if (idRole != value)
                {
                    idRole = value;
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
