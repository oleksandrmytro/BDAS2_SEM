using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class PDiagnosesVM : INotifyPropertyChanged
    {
        private readonly INavstevaDoctorViewRepository _navstevaDoctorViewRepository;
        private readonly IWindowService _windowService;
        private readonly IPatientContextService _patientContextService;

        private ObservableCollection<NAVSTEVA_DOCTOR_VIEW> _pastAppointments;
        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> PastAppointments
        {
            get => _pastAppointments;
            set
            {
                _pastAppointments = value;
                OnPropertyChanged();
            }
        }

        public ICommand ViewDiagnosisCommand { get; }
        public ICommand RefreshCommand { get; }

        public PDiagnosesVM(
            IWindowService windowService,
            IPatientContextService patientContextService,
            INavstevaDoctorViewRepository navstevaDoctorViewRepository)
        {
            _windowService = windowService;
            _patientContextService = patientContextService;
            _navstevaDoctorViewRepository = navstevaDoctorViewRepository;

            ViewDiagnosisCommand = new AsyncRelayCommand(ViewDiagnosisAsync);
            RefreshCommand = new AsyncRelayCommand(LoadPastAppointmentsAsync);

            // Ініціалізація завантаження призначень
            LoadPastAppointmentsAsync();
        }

        private async Task LoadPastAppointmentsAsync(object parameter = null)
        {
            var currentPatient = _patientContextService.CurrentPatient;
            if (currentPatient == null)
            {
                // Обробка випадку, коли поточний пацієнт не встановлений
                PastAppointments = new ObservableCollection<NAVSTEVA_DOCTOR_VIEW>();
                return;
            }

            var pacientId = currentPatient.IdPacient;
            var allAppointments = await _navstevaDoctorViewRepository.GetNavstevaDoctorViewByPatientId(pacientId);
            var now = DateTime.Now;

            var pastAppointmentsList = new ObservableCollection<NAVSTEVA_DOCTOR_VIEW>();

            foreach (var appointment in allAppointments)
            {
                var appointmentDateTime = appointment.VisitDate;

                if (appointmentDateTime < now)
                {
                    pastAppointmentsList.Add(appointment);
                }
            }

            PastAppointments = pastAppointmentsList;
        }

        private async Task ViewDiagnosisAsync(object parameter)
        {
            if (parameter is NAVSTEVA_DOCTOR_VIEW appointment)
            {
                var navsteva = new NAVSTEVA
                {
                    IdNavsteva = appointment.NavstevaId,
                    Datum = appointment.VisitDate,
                    PacientId = appointment.PacientId,
                    DoktorJmeno = appointment.DoctorFullName
                };

                // Відкриття вікна діагнозу асинхронно, якщо необхідно
                await Task.Run(() => _windowService.OpenViewDiagnosisWindow(navsteva));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}