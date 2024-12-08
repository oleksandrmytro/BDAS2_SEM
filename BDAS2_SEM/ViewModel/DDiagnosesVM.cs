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

        public ICommand AssignDiagnosisCommand { get; }
        public ICommand RefreshCommand { get; } // Новa команда

        public DDiagnosesVM(IServiceProvider serviceProvider, IWindowService windowService)
        {
            _navstevaRepository = serviceProvider.GetRequiredService<INavstevaRepository>();
            _windowService = windowService;
            _navstevaDiagnozaRepository = serviceProvider.GetRequiredService<INavstevaDiagnozaRepository>();
            _pacientRepository = serviceProvider.GetRequiredService<IPacientRepository>();
            _mistnostRepository = serviceProvider.GetRequiredService<IMistnostRepository>();

            PastAppointments = new ObservableCollection<dynamic>();

            AssignDiagnosisCommand = new RelayCommand<object>(async (param) => await AssignDiagnosisAsync(param));
            RefreshCommand = new RelayCommand<object>(async (param) => await LoadAppointmentsAsync());
        }

        public ObservableCollection<dynamic> PastAppointments { get; set; }

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

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadAppointmentsAsync();
        }

        private async Task LoadAppointmentsAsync()
        {
            var appointments = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
            PastAppointments.Clear();

            if (appointments != null && appointments.Any())
            {
                foreach (var appointment in appointments)
                {
                    var pacient = await _navstevaRepository.GetPatientNameByAppointmentId(appointment.IdNavsteva);
                    var status = (int)appointment.StatusId;

                    if (status.Equals(4) && appointment.Datum < DateTime.Now)
                    {
                        // Завантаження діагнозів для цієї зустрічі
                        var diagnozy = await _navstevaDiagnozaRepository.GetDiagnozyByNavstevaIdAsync(appointment.IdNavsteva);

                        // Збирання назв діагнозів
                        List<string> diagnozyList = diagnozy.Select(d => d.Nazev).ToList();

                        // Отримання інформації про кімнату
                        var mistnost = await _mistnostRepository.GetMistnostById(appointment.MistnostId);

                        // Створення анонімного об'єкта з інформацією про кімнату
                        var appointmentWithDiagnozy = new
                        {
                            appointment.IdNavsteva,
                            appointment.PacientId,
                            appointment.Datum,
                            appointment.MistnostId,
                            appointment.StatusId,
                            PACIENTJmeno = pacient.FirstName,
                            PACIENTPrijmeni = pacient.LastName,
                            Diagnozy = diagnozyList,
                            MISTNOST = mistnost?.Cislo ?? (object)null
                        };

                        PastAppointments.Add(appointmentWithDiagnozy);
                    }
                }

                NoAppointmentsMessage = PastAppointments.Any() ? string.Empty : "Лікар не має пройдених зустрічей.";
            }
            else
            {
                NoAppointmentsMessage = "Лікар не має пройдених зустрічей.";
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
                // Відкриваємо вікно без колбеку
                _windowService.OpenAssignDiagnosisWindow(navsteva, _doctor.IdZamestnanec);

                // Після закриття вікна оновлюємо дані
                await LoadAppointmentsAsync();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}