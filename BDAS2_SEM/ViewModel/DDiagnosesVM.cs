using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel
{
    public class DDiagnosesVM : INotifyPropertyChanged
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly IWindowService _windowService;
        private readonly INavstevaDiagnozaRepository _navstevaDiagnozaRepository;
        private readonly IMistnostRepository _mistnostRepository;

        private ZAMESTNANEC _doctor;

        private string _noAppointmentsMessage;
        private string _noDiagnosesMessage;

        public ICommand AssignDiagnosisCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewDiagnosisCommand { get; }

        public DDiagnosesVM(IServiceProvider serviceProvider, IWindowService windowService)
        {
            _navstevaRepository = serviceProvider.GetRequiredService<INavstevaRepository>();
            _windowService = windowService;
            _navstevaDiagnozaRepository = serviceProvider.GetRequiredService<INavstevaDiagnozaRepository>();
            _pacientRepository = serviceProvider.GetRequiredService<IPacientRepository>();
            _mistnostRepository = serviceProvider.GetRequiredService<IMistnostRepository>();

            PastAppointments = new ObservableCollection<dynamic>();
            Diagnoses = new ObservableCollection<dynamic>();

            AssignDiagnosisCommand = new RelayCommand<object>(async (param) => await AssignDiagnosisAsync(param));
            RefreshCommand = new RelayCommand<object>(async (param) => await LoadAppointmentsAsync());
            ViewDiagnosisCommand = new RelayCommand<object>(ViewDiagnosis);
        }

        public ObservableCollection<dynamic> PastAppointments { get; set; }
        public ObservableCollection<dynamic> Diagnoses { get; set; }

        public string NoAppointmentsMessage
        {
            get => _noAppointmentsMessage;
            set
            {
                if (_noAppointmentsMessage != value)
                {
                    _noAppointmentsMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NoDiagnosesMessage
        {
            get => _noDiagnosesMessage;
            set
            {
                if (_noDiagnosesMessage != value)
                {
                    _noDiagnosesMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadAppointmentsAsync();
        }

        private async Task LoadAppointmentsAsync()
        {
            var appointments = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
            PastAppointments.Clear();
            Diagnoses.Clear();

            if (appointments != null && appointments.Any())
            {
                foreach (var appointment in appointments)
                {
                    var pacient = await _navstevaRepository.GetPatientNameByAppointmentId(appointment.IdNavsteva);
                    var status = (int)appointment.StatusId;
                    var mistnost = await _mistnostRepository.GetMistnostById(appointment.MistnostId);
                    var diagnozy = await _navstevaDiagnozaRepository.GetDiagnozyByNavstevaIdAsync(appointment.IdNavsteva);

                    if (appointment.Datum < DateTime.Now)
                    {
                        if (diagnozy.Any())
                        {
                            var appointmentWithDiagnosis = new
                            {
                                appointment.IdNavsteva,
                                appointment.PacientId,
                                appointment.Datum,
                                appointment.MistnostId,
                                appointment.StatusId,
                                PACIENTJmeno = pacient.FirstName,
                                PACIENTPrijmeni = pacient.LastName,
                                DIAGNOZA = string.Join(", ", diagnozy.Select(d => d.Nazev)),
                                MISTNOST = mistnost?.Cislo ?? (object)null
                            };

                            Diagnoses.Add(appointmentWithDiagnosis);
                        }
                        else
                        {
                            var appointmentWithDiagnozy = new
                            {
                                appointment.IdNavsteva,
                                appointment.PacientId,
                                appointment.Datum,
                                appointment.MistnostId,
                                appointment.StatusId,
                                PACIENTJmeno = pacient.FirstName,
                                PACIENTPrijmeni = pacient.LastName,
                                Diagnozy = diagnozy.Select(d => d.Nazev).ToList(),
                                MISTNOST = mistnost?.Cislo ?? (object)null
                            };

                            PastAppointments.Add(appointmentWithDiagnozy);
                        }
                    }
                }

                NoAppointmentsMessage = PastAppointments.Any() ? string.Empty : "The doctor has no past appointments.";
                NoDiagnosesMessage = Diagnoses.Any() ? string.Empty : "The doctor has no assigned diagnoses.";
            }
            else
            {
                NoAppointmentsMessage = "The doctor has no past appointments.";
                NoDiagnosesMessage = "The doctor has no assigned diagnoses.";
            }
        }

        private async Task AssignDiagnosisAsync(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = (int)appointment.IdNavsteva,
                    PacientId = (int)appointment.PacientId,
                    StatusId = (int)appointment.StatusId,
                    Datum = appointment.Datum,
                    MistnostId = (int?)appointment.MistnostId
                };
                _windowService.OpenAssignDiagnosisWindow(navsteva, _doctor.IdZamestnanec);

                await LoadAppointmentsAsync();
            }
        }

        private void ViewDiagnosis(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = (int)appointment.IdNavsteva,
                    PacientId = (int)appointment.PacientId,
                    StatusId = (int)appointment.StatusId,
                    Datum = appointment.Datum,
                    MistnostId = (int?)appointment.MistnostId
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