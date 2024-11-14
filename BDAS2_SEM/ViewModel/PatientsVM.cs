using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.ViewModel
{
    public class PatientsVM : INotifyPropertyChanged
    {
        private string welcomeMessage = "Welcome to the Patient Dashboard!";

        public string WelcomeMessage
        {
            get => welcomeMessage;
            set
            {
                if (welcomeMessage != value)
                {
                    welcomeMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        // Додайте додаткові властивості та команди за потреби

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}