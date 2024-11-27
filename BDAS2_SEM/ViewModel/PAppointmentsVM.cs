using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class PAppointmentsVM : INotifyPropertyChanged
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IWindowService _windowService;
        private readonly IPatientContextService _patientContextService;

        public ObservableCollection<NAVSTEVA> PastAppointments { get; set; }
        public ObservableCollection<NAVSTEVA> FutureAppointments { get; set; }

        public ICommand BookAppointmentCommand { get; }

        public PAppointmentsVM(INavstevaRepository navstevaRepository, IWindowService windowService, IPatientContextService patientContextService)
        {
            _navstevaRepository = navstevaRepository;
            _windowService = windowService;
            _patientContextService = patientContextService;

            BookAppointmentCommand = new RelayCommand(BookAppointment);
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            var pacientId = _patientContextService.CurrentPatient.IdPacient;
            var allAppointments = await _navstevaRepository.GetAllNavstevy();
            var now = DateTime.Now;

            PastAppointments = new ObservableCollection<NAVSTEVA>();
            FutureAppointments = new ObservableCollection<NAVSTEVA>();

            foreach (var appointment in allAppointments)
            {
                var appointmentDateTime = appointment.Datum;
                if (appointmentDateTime < now)
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