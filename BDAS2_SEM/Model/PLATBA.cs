﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class PLATBA : INotifyPropertyChanged
    {
        private int idPlatba;
        private decimal castka;
        private DateTime datum;
        private string typPlatby;
        private int navstevaId;

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

        public decimal Castka
        {
            get { return castka; }
            set
            {
                if (castka != value)
                {
                    castka = value;
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

        public string TypPlatby
        {
            get { return typPlatby; }
            set
            {
                if (typPlatby != value)
                {
                    typPlatby = value;
                    OnPropertyChanged();
                }
            }
        }

        public int NavstevaId
        {
            get { return navstevaId; }
            set
            {
                if (navstevaId != value)
                {
                    navstevaId = value;
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
