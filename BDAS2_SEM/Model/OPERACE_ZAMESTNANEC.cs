using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class OPERACE_ZAMESTNANEC : INotifyPropertyChanged
    {
        private int operaceId;
        private int zamestnanecId;

        public int OperaceId
        {
            get { return operaceId; }
            set
            {
                if (operaceId != value)
                {
                    operaceId = value;
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

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
