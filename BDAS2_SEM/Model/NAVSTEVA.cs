using BDAS2_SEM.Model.Enum;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class NAVSTEVA : INotifyPropertyChanged
    {
        private int idNavsteva;
        private DateTime? datum;
        private int pacientId;
        private Status status;
        private string doktorJmeno; // Новое свойство
        private int zamestnanecId; // Новое свойство
        private MISTNOST mistnost; // Новое свойство

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

        public DateTime? Datum
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

        public Status Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DoktorJmeno
        {
            get { return doktorJmeno; }
            set
            {
                if (doktorJmeno != value)
                {
                    doktorJmeno = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ZamestnanecId
        {
            get { return zamestnanecId; }
            set
            {
                if (zamestnanecId != value)
                {
                    zamestnanecId = value;
                    OnPropertyChanged();
                }
            }
        }

        public MISTNOST Mistnost
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}