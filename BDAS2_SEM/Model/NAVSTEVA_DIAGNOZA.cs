using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class NAVSTEVA_DIAGNOZA : INotifyPropertyChanged
    {
        private int navstevaId;
        private int diagnozaId;

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