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

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsListVM : INotifyPropertyChanged
    {
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IPatientContextService _patientContextService;
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IZamestnanecNavstevaRepository _zamestnanecNavstevaRepository;
        private readonly IOrdinaceZamestnanecRepository _ordinaceZamestnanecRepository;
        private readonly IOrdinaceRepository _ordinaceRepository;

        public ObservableCollection<ZAMESTNANEC> Doctors { get; set; }
        public ICommand CreateAppointmentCommand { get; }

        public DoctorsListVM(
            IZamestnanecRepository zamestnanecRepository,
            IPatientContextService patientContextService,
            INavstevaRepository navstevaRepository,
            IZamestnanecNavstevaRepository zamestnanecNavstevaRepository,
            IOrdinaceZamestnanecRepository ordinaceZamestnanecRepository,
            IOrdinaceRepository ordinaceRepository)
        {
            _zamestnanecRepository = zamestnanecRepository;
            _patientContextService = patientContextService;
            _navstevaRepository = navstevaRepository;
            _zamestnanecNavstevaRepository = zamestnanecNavstevaRepository;
            _ordinaceZamestnanecRepository = ordinaceZamestnanecRepository;
            _ordinaceRepository = ordinaceRepository;

            CreateAppointmentCommand = new RelayCommand(CreateAppointment);
            LoadDoctors();
        }

        private async void LoadDoctors()
        {
            var doctors = await _zamestnanecRepository.GetAllZamestnanci();
            var ordinaceZamestnanci = await _ordinaceZamestnanecRepository.GetAllOrdinaceZamestnanecs();
            var ordinaceList = await _ordinaceRepository.GetAllOrdinaces();

            foreach (var doctor in doctors)
            {
                var ordinaceZamestnanec = ordinaceZamestnanci.Where(o => o.ZamestnanecId == doctor.IdZamestnanec);
                var ordinaceNames = ordinaceZamestnanec
                    .Select(o => ordinaceList.FirstOrDefault(ord => ord.IdOrdinace == o.OrdinaceId)?.Nazev)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                if (ordinaceNames.Any())
                {
                }
            }

            Doctors = new ObservableCollection<ZAMESTNANEC>(doctors);
            OnPropertyChanged(nameof(Doctors));
        }

        private async void CreateAppointment(object parameter)
        {
            var selectedDoctor = parameter as ZAMESTNANEC;
            if (selectedDoctor != null)
            {
                var pacientId = _patientContextService.CurrentPatient.IdPacient;

                var newAppointment = new NAVSTEVA
                {
                    PacientId = pacientId,
                    Status = Status.Pending
                };

                var newAppointmentId = await _navstevaRepository.AddNavsteva(newAppointment);
                newAppointment.IdNavsteva = newAppointmentId;

                var zamestnanecNavsteva = new ZAMESTNANEC_NAVSTEVA
                {
                    ZamestnanecId = selectedDoctor.IdZamestnanec,
                    NavstevaId = newAppointmentId
                };

                await _zamestnanecNavstevaRepository.InsertZamestnanecNavsteva(zamestnanecNavsteva);

                MessageBox.Show("Appointment successfully created!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}