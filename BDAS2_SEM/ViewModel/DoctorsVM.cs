// DoctorsVM.cs
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Threading.Tasks;

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsVM : INotifyPropertyChanged
    {
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IPoziceRepository _poziceRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly int _currentDoctorId;

        private ObservableCollection<PACIENT> _patients;
        private string _searchQuery;
        private int _totalDoctors;
        private int _appointmentsToday;
        private int _activeDoctors;

        public ObservableCollection<PACIENT> Patients
        {
            get => _patients;
            set
            {
                _patients = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterPatients();
            }
        }

        public int TotalDoctors
        {
            get => _totalDoctors;
            set
            {
                _totalDoctors = value;
                OnPropertyChanged();
            }
        }

        public int AppointmentsToday
        {
            get => _appointmentsToday;
            set
            {
                _appointmentsToday = value;
                OnPropertyChanged();
            }
        }

        public int ActiveDoctors
        {
            get => _activeDoctors;
            set
            {
                _activeDoctors = value;
                OnPropertyChanged();
            }
        }

        public DoctorsVM(IZamestnanecRepository zamestnanecRepository, IPoziceRepository poziceRepository, IPacientRepository pacientRepository, int currentDoctorId)
        {
            _zamestnanecRepository = zamestnanecRepository;
            _poziceRepository = poziceRepository;
            _pacientRepository = pacientRepository;
            _currentDoctorId = currentDoctorId;
            LoadData();
        }

        private async void LoadData()
        {
            var allZamestnanci = await _zamestnanecRepository.GetAllZamestnanci();
            var allPozice = await _poziceRepository.GetAllPozice();

            var doctorPozice = allPozice.FirstOrDefault(p => p.Nazev.Equals("Doctor", System.StringComparison.OrdinalIgnoreCase));
            if (doctorPozice == null)
            {
                TotalDoctors = 0;
                AppointmentsToday = 0;
                ActiveDoctors = 0;
                Patients = new ObservableCollection<PACIENT>();
                return;
            }

            int doctorPoziceId = doctorPozice.IdPozice;
            var doctors = allZamestnanci.Where(z => z.PoziceId == doctorPoziceId);
            TotalDoctors = doctors.Count();

            AppointmentsToday = await GetAppointmentsTodayAsync();
            ActiveDoctors = await GetActiveDoctorsAsync();

            await LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            var allPacienti = await _pacientRepository.GetAllPacienti();
            var doctorPacienti = allPacienti.Where(p => p.UserDataId == _currentDoctorId);
            Patients = new ObservableCollection<PACIENT>(doctorPacienti);
        }

        private void FilterPatients()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                LoadData();
            }
            else
            {
                var filteredPatients = Patients
                    .Where(p => $"{p.Jmeno} {p.Prijmeni}".Contains(SearchQuery, System.StringComparison.OrdinalIgnoreCase));
                Patients = new ObservableCollection<PACIENT>(filteredPatients);
            }
        }

        private Task<int> GetAppointmentsTodayAsync()
        {
            return Task.FromResult(0);
        }

        private Task<int> GetActiveDoctorsAsync()
        {
            return Task.FromResult(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}