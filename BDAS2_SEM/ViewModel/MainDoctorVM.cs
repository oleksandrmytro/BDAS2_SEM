// ViewModel/MainDoctorVM.cs
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BDAS2_SEM.ViewModel
{
    public class MainDoctorVM : INotifyPropertyChanged
    {
        private readonly IPacientRepository _pacientRepository;
        private ObservableCollection<PACIENT> _patients;

        public ObservableCollection<PACIENT> Patients
        {
            get => _patients;
            set
            {
                _patients = value;
                OnPropertyChanged();
            }
        }

        public MainDoctorVM(IPacientRepository pacientRepository)
        {
            _pacientRepository = pacientRepository;
            LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            var allPacienti = await _pacientRepository.GetAllPacienti();
            Patients = new ObservableCollection<PACIENT>(allPacienti);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}