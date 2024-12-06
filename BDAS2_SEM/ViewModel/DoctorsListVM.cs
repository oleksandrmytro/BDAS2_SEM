using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BDAS2_SEM.Model.Enum;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsListVM : INotifyPropertyChanged
    {
        private readonly IDoctorInfoRepository _doctorInfoRepository;
        private readonly IPatientContextService _patientContextService;
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IZamestnanecNavstevaRepository _zamestnanecNavstevaRepository;
        private readonly IWindowService _windowService;

        private ObservableCollection<DOCTOR_INFO> _allDoctors;
        public ObservableCollection<DOCTOR_INFO> Doctors { get; set; }
        public ICommand CreateAppointmentCommand { get; }
        public ICommand SearchCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterDoctors();
            }
        }

        public DoctorsListVM(
            IDoctorInfoRepository doctorInfoRepository,
            IPatientContextService patientContextService,
            INavstevaRepository navstevaRepository,
            IZamestnanecNavstevaRepository zamestnanecNavstevaRepository,
            IWindowService windowService)
        {
            _doctorInfoRepository = doctorInfoRepository;
            _patientContextService = patientContextService;
            _navstevaRepository = navstevaRepository;
            _zamestnanecNavstevaRepository = zamestnanecNavstevaRepository;
            _windowService = windowService;

            CreateAppointmentCommand = new RelayCommand(CreateAppointment);
            SearchCommand = new RelayCommand(_ => FilterDoctors());
            LoadDoctors();
        }

        // Конструктор по умолчанию для XAML
        public DoctorsListVM() : this(
            App.ServiceProvider.GetRequiredService<IDoctorInfoRepository>(),
            App.ServiceProvider.GetRequiredService<IPatientContextService>(),
            App.ServiceProvider.GetRequiredService<INavstevaRepository>(),
            App.ServiceProvider.GetRequiredService<IZamestnanecNavstevaRepository>(),
            App.ServiceProvider.GetRequiredService<IWindowService>())
        {
        }

        private async void LoadDoctors()
        {
            var doctors = await _doctorInfoRepository.GetAllDoctors();
            _allDoctors = new ObservableCollection<DOCTOR_INFO>(doctors);
            Doctors = new ObservableCollection<DOCTOR_INFO>(_allDoctors);
            OnPropertyChanged(nameof(Doctors));
        }

        private void FilterDoctors()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Doctors = new ObservableCollection<DOCTOR_INFO>(_allDoctors);
            }
            else
            {
                var filteredDoctors = _allDoctors.Where(d =>
                    (d.FirstName != null && d.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (d.Surname != null && d.Surname.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (d.Phone.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (d.Department != null && d.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                Doctors = new ObservableCollection<DOCTOR_INFO>(filteredDoctors);
            }
            OnPropertyChanged(nameof(Doctors));
        }

        private async void CreateAppointment(object parameter)
        {
            if (parameter != null)
            {
                var selectedDoctor = parameter as DOCTOR_INFO;
                if (selectedDoctor != null)
                {
                    var pacientId = _patientContextService.CurrentPatient.IdPacient;

                    var newAppointment = new NAVSTEVA
                    {
                        PacientId = pacientId,
                        StatusId = 3
                    };

                    var newAppointmentId = await _navstevaRepository.AddNavsteva(newAppointment);
                    newAppointment.IdNavsteva = newAppointmentId;

                    var zamestnanecNavsteva = new ZAMESTNANEC_NAVSTEVA
                    {
                        ZamestnanecId = selectedDoctor.DoctorId, // Используем DoctorId вместо AvatarId
                        NavstevaId = newAppointmentId
                    };

                    await _zamestnanecNavstevaRepository.AddZamestnanecNavsteva(zamestnanecNavsteva);

                    MessageBox.Show("Appointment successfully created!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}