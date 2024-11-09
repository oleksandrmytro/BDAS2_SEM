﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class KARTA : INotifyPropertyChanged
    {
        private int idPlatba;
        private long cisloKarty;

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

        public long CisloKarty
        {
            get { return cisloKarty; }
            set
            {
                if (cisloKarty != value)
                {
                    cisloKarty = value;
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