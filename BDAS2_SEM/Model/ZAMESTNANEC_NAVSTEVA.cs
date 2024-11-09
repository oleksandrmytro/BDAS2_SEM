using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class ZAMESTNANEC_NAVSTEVA : INotifyPropertyChanged
    {
        private int zamestnanecId;
        private int navstevaId;

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