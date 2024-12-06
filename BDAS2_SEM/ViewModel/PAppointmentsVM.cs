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

        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> PastAppointments { get; set; }
        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> FutureAppointments { get; set; }
        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> UnconfirmedAppointments { get; set; }

        public ICommand BookAppointmentCommand { get; }

        public PAppointmentsVM(IWindowService windowService, IPatientContextService patientContextService, INavstevaDoctorViewRepository navstevaDoctorViewRepository)
        {
            _windowService = windowService;
            _patientContextService = patientContextService;
            _navstevaDoctorViewRepository = navstevaDoctorViewRepository;

            BookAppointmentCommand = new RelayCommand(BookAppointment);
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            var currentPatient = _patientContextService.CurrentPatient;
            if (currentPatient == null)
            {
                // Обработка случая, когда текущий пациент не установлен
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

                if (appointment.NavstevaStatus == "očekávání")
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

            OnPropertyChanged(nameof(PastAppointments));
            OnPropertyChanged(nameof(FutureAppointments));
            OnPropertyChanged(nameof(UnconfirmedAppointments));
        }

        private void BookAppointment(object parameter)
        {
            _windowService.OpenDoctorsListWindow();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}