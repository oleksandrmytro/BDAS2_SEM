using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.Model
{
    public class ORDINACE_ZAMESTNANEC : INotifyPropertyChanged
    {
        private int ordinaceId;
        private int zamestnanecId;

        public int OrdinaceId
        {
            get { return ordinaceId; }
            set
            {
                if (ordinaceId != value)
                {
                    ordinaceId = value;
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