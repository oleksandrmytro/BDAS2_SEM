using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsListVM : INotifyPropertyChanged
    {
        private readonly IZamestnanecRepository _zamestnanecRepository;

        public ObservableCollection<ZAMESTNANEC> Doctors { get; set; }

        public DoctorsListVM(IZamestnanecRepository zamestnanecRepository)
        {
            _zamestnanecRepository = zamestnanecRepository;
            LoadDoctors();
        }

        private async void LoadDoctors()
        {
            var doctors = await _zamestnanecRepository.GetAllZamestnanci();
            Doctors = new ObservableCollection<ZAMESTNANEC>(doctors);
            OnPropertyChanged(nameof(Doctors));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}