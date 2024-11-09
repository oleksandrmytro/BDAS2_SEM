﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class NAVSTEVA : INotifyPropertyChanged
    {
        private int idNavsteva;
        private DateTime datum;
        private DateTime cas;
        private int mistnost;
        private int pacientId;

        public int IdNavsteva
        {
            get { return idNavsteva; }
            set
            {
                if (idNavsteva != value)
                {
                    idNavsteva = value;
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

        public DateTime Cas
        {
            get { return cas; }
            set
            {
                if (cas != value)
                {
                    cas = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Mistnost
        {
            get { return mistnost; }
            set
            {
                if (mistnost != value)
                {
                    mistnost = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PacientId
        {
            get { return pacientId; }
            set
            {
                if (pacientId != value)
                {
                    pacientId = value;
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