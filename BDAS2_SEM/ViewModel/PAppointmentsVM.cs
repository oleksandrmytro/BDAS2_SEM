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
using System.Threading.Tasks;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class PAppointmentsVM : INotifyPropertyChanged
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IWindowService _windowService;
        private readonly IPatientContextService _patientContextService;
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IZamestnanecNavstevaRepository _zamestnanecNavstevaRepository;

        public ObservableCollection<NAVSTEVA> PastAppointments { get; set; }
        public ObservableCollection<NAVSTEVA> FutureAppointments { get; set; }
        public ObservableCollection<NAVSTEVA> UnconfirmedAppointments { get; set; }

        public ICommand BookAppointmentCommand { get; }

        public PAppointmentsVM(INavstevaRepository navstevaRepository, IWindowService windowService, IPatientContextService patientContextService, IZamestnanecRepository zamestnanecRepository, IZamestnanecNavstevaRepository zamestnanecNavstevaRepository)
        {
            _navstevaRepository = navstevaRepository;
            _windowService = windowService;
            _patientContextService = patientContextService;
            _zamestnanecRepository = zamestnanecRepository;
            _zamestnanecNavstevaRepository = zamestnanecNavstevaRepository;

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
            var allAppointments = await _navstevaRepository.GetAllNavstevy();
            var now = DateTime.Now;

            PastAppointments = new ObservableCollection<NAVSTEVA>();
            FutureAppointments = new ObservableCollection<NAVSTEVA>();
            UnconfirmedAppointments = new ObservableCollection<NAVSTEVA>();

            foreach (var appointment in allAppointments)
            {
                var appointmentDateTime = appointment.Datum;
                var zamestnanecNavsteva = await _zamestnanecNavstevaRepository.GetZamestnanecNavstevaByNavstevaId(appointment.IdNavsteva);
                if (zamestnanecNavsteva != null)
                {
                    var doctor = await _zamestnanecRepository.GetZamestnanecById(zamestnanecNavsteva.ZamestnanecId);
                    if (doctor != null)
                    {
                        appointment.DoktorJmeno = $"{doctor.Jmeno} {doctor.Prijmeni}";
                    }
                    else
                    {
                        appointment.DoktorJmeno = "Unknown";
                    }
                }
                else
                {
                    appointment.DoktorJmeno = "Unknown";
                }

                if (appointment.Status == Status.Pending)
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