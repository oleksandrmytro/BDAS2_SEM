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
    public class PAppointmentsVM : INotifyPropertyChanged
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

        private ObservableCollection<NAVSTEVA_DOCTOR_VIEW> _futureAppointments;
        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> FutureAppointments
        {
            get => _futureAppointments;
            set
            {
                _futureAppointments = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<NAVSTEVA_DOCTOR_VIEW> _unconfirmedAppointments;
        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> UnconfirmedAppointments
        {
            get => _unconfirmedAppointments;
            set
            {
                _unconfirmedAppointments = value;
                OnPropertyChanged();
            }
        }

        public ICommand BookAppointmentCommand { get; }
        public ICommand RefreshCommand { get; }

        public PAppointmentsVM(
            IWindowService windowService,
            IPatientContextService patientContextService,
            INavstevaDoctorViewRepository navstevaDoctorViewRepository)
        {
            _windowService = windowService;
            _patientContextService = patientContextService;
            _navstevaDoctorViewRepository = navstevaDoctorViewRepository;

            BookAppointmentCommand = new AsyncRelayCommand(BookAppointmentAsync);
            RefreshCommand = new AsyncRelayCommand(RefreshAppointmentsAsync);
            LoadAppointmentsAsync();
        }

        private async Task LoadAppointmentsAsync()
        {
            var currentPatient = _patientContextService.CurrentPatient;
            if (currentPatient == null)
            {
                return;
            }

            var pacientId = currentPatient.IdPacient;
            var allAppointments = await _navstevaDoctorViewRepository.GetNavstevaDoctorViewByPatientId(pacientId);
            var now = DateTime.Now;

            PastAppointments = new ObservableCollection<NAVSTEVA_DOCTOR_VIEW>();
            FutureAppointments = new ObservableCollection<NAVSTEVA_DOCTOR_VIEW>();
            UnconfirmedAppointments = new ObservableCollection<NAVSTEVA_DOCTOR_VIEW>();

            foreach (var appointment in allAppointments)
            {
                var appointmentDateTime = appointment.VisitDate;

                if (appointment.NavstevaStatus.Trim().Equals("očekávání", StringComparison.OrdinalIgnoreCase))
                {
                    UnconfirmedAppointments.Add(appointment);
                }
                else if (appointmentDateTime < now)
                {
                    PastAppointments.Add(appointment);
                }
                else
                {
                    FutureAppointments.Add(appointment);
                }
            }

            // Повідомлення про зміни відбувається через сеттери властивостей
        }

        private async Task BookAppointmentAsync(object parameter)
        {
            _windowService.OpenDoctorsListWindow();
            await LoadAppointmentsAsync();
        }

        private async Task RefreshAppointmentsAsync(object parameter)
        {
            await LoadAppointmentsAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}