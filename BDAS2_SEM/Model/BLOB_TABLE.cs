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
        private string priponaSouboru;
        private byte[] obsah;
        private DateTime datumNahrani;
        private DateTime datumModifikace;
        private string operaceProvedl;
        private string popisOperace;

        public int IdBlob
        {
            get { return idBlob; }
            set
            {
                idBlob = value;
                OnPropertyChanged();
            }
        }

        public string NazevSouboru
        {
            get { return nazevSouboru; }
            set
            {
                nazevSouboru = value;
                OnPropertyChanged();
            }
        }

        public string TypSouboru
        {
            get { return typSouboru; }
            set
            {
                typSouboru = value;
                OnPropertyChanged();
            }
        }

        public string PriponaSouboru
        {
            get { return priponaSouboru; }
            set
            {
                priponaSouboru = value;
                OnPropertyChanged();
            }
        }

        public byte[] Obsah
        {
            get { return obsah; }
            set
            {
                obsah = value;
                OnPropertyChanged();
            }
        }

        public DateTime DatumNahrani
        {
            get { return datumNahrani; }
            set
            {
                datumNahrani = value;
                OnPropertyChanged();
            }
        }

        public DateTime DatumModifikace
        {
            get { return datumModifikace; }
            set
            {
                datumModifikace = value;
                OnPropertyChanged();
            }
        }

        public string OperaceProvedl
        {
            get { return operaceProvedl; }
            set
            {
                operaceProvedl = value;
                OnPropertyChanged();
            }
        }

        public string PopisOperace
        {
            get { return popisOperace; }
            set
            {
                popisOperace = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}