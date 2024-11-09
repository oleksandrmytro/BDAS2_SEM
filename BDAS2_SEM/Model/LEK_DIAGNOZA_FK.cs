using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class LEK_DIAGNOZA_FK : INotifyPropertyChanged
    {
        private int diagnozaId;
        private int lekId;

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

        public int LekId
        {
            get { return lekId; }
            set
            {
                if (lekId != value)
                {
                    lekId = value;
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