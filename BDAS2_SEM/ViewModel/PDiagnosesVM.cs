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

        public ObservableCollection<NAVSTEVA_DOCTOR_VIEW> PastAppointments { get; set; }

        public ICommand ViewDiagnosisCommand { get; }

        public PDiagnosesVM(
            IWindowService windowService,
            IPatientContextService patientContextService,
            INavstevaDoctorViewRepository navstevaDoctorViewRepository)
        {
            _windowService = windowService;
            _patientContextService = patientContextService;
            _navstevaDoctorViewRepository = navstevaDoctorViewRepository;

            ViewDiagnosisCommand = new RelayCommand(ViewDiagnosis);
            LoadPastAppointments();
        }

        private async void LoadPastAppointments()
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

            foreach (var appointment in allAppointments)
            {
                var appointmentDateTime = appointment.VisitDate;

                if (appointmentDateTime < now)
                {
                    PastAppointments.Add(appointment);
                }
            }

            OnPropertyChanged(nameof(PastAppointments));
        }

        private void ViewDiagnosis(object parameter)
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

                _windowService.OpenViewDiagnosisWindow(navsteva);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}