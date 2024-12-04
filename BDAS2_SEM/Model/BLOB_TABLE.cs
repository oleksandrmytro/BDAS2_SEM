using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class BLOB_TABLE : INotifyPropertyChanged
    {
        private int idBlob;
        private string nazevSouboru;
        private string typSouboru;
        private byte[] obsah;
        private DateTime datumNahrani;
        private DateTime datumModifikace;
        private string operaceProvedl;
        private string popisOperace;
        private int priponaId; // Новое свойство

        public int IdBlob
        {
            get { return idBlob; }
            set
            {
                if (idBlob != value)
                {
                    idBlob = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NazevSouboru
        {
            get { return nazevSouboru; }
            set
            {
                if (nazevSouboru != value)
                {
                    nazevSouboru = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypSouboru
        {
            get { return typSouboru; }
            set
            {
                if (typSouboru != value)
                {
                    typSouboru = value;
                    OnPropertyChanged();
                }
            }
        }

        public byte[] Obsah
        {
            get { return obsah; }
            set
            {
                if (obsah != value)
                {
                    obsah = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DatumNahrani
        {
            get { return datumNahrani; }
            set
            {
                if (datumNahrani != value)
                {
                    datumNahrani = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime DatumModifikace
        {
            get { return datumModifikace; }
            set
            {
                if (datumModifikace != value)
                {
                    datumModifikace = value;
                    OnPropertyChanged();
                }
            }
        }

        public string OperaceProvedl
        {
            get { return operaceProvedl; }
            set
            {
                if (operaceProvedl != value)
                {
                    operaceProvedl = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PopisOperace
        {
            get { return popisOperace; }
            set
            {
                if (popisOperace != value)
                {
                    popisOperace = value;
                    OnPropertyChanged();
                }
            }
        }

        public int PriponaId
        {
            get { return priponaId; }
            set
            {
                if (priponaId != value)
                {
                    priponaId = value;
                    OnPropertyChanged();
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}