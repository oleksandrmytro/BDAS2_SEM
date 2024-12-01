// ViewModel/DDiagnosesVM.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Repository;
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

        private ZAMESTNANEC _doctor;

        private string _noAppointmentsMessage;

        public ICommand AssignDiagnosisCommand { get; }

        public DDiagnosesVM(IServiceProvider serviceProvider, IWindowService windowService)
        {
            _navstevaRepository = serviceProvider.GetRequiredService<INavstevaRepository>();
            _windowService = windowService;
            _navstevaDiagnozaRepository = serviceProvider.GetRequiredService<INavstevaDiagnozaRepository>();
            _pacientRepository = serviceProvider.GetRequiredService<IPacientRepository>();

            PastAppointments = new ObservableCollection<dynamic>();

            AssignDiagnosisCommand = new RelayCommand(AssignDiagnosis);
        }

        private void AssignDiagnosis(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = (int)appointment.IdNavsteva,
                    PacientId = (int)appointment.PacientId,
                    Status = (Status)(int)appointment.Status,
                    Datum = appointment.Datum,
                    Mistnost = appointment.Mistnost
                };
                // Явне приведення лямбда-виразу до Func<NAVSTEVA, Task>
                Func<NAVSTEVA, Task> callback = async (updatedAppointment) =>
                {
                    if (updatedAppointment != null)
                    {
                        // Завантажуємо діагнози для цього візиту
                        var diagnozy = await _navstevaDiagnozaRepository.GetDiagnozyByNavstevaIdAsync(updatedAppointment.IdNavsteva);
                        // Оновлюємо список зустрічей
                        LoadAppointments(); 
                        MessageBox.Show("Діагноз успішно призначено.", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                };

                _windowService.OpenAssignDiagnosisWindow(navsteva, _doctor.IdZamestnanec, callback);
            }
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
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            var appointments = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
            PastAppointments.Clear();

            if (appointments != null && appointments.Any())
            {
                foreach (var appointment in appointments)
                {
                    var pacient = await _navstevaRepository.GetPatientNameByAppointmentId(appointment.IdNavsteva);
                    var status = (int)appointment.Status;

                    if (status.Equals((int)Status.Accepted) && appointment.Datum < DateTime.Now)
                    {
                        // Завантажуємо діагнози для цього візиту
                        var diagnozy = await _navstevaDiagnozaRepository.GetDiagnozyByNavstevaIdAsync((int)appointment.IdNavsteva);

                        // Використовуємо цикл для збору назв діагнозів
                        List<string> diagnozyList = new List<string>();
                        foreach (var d in diagnozy)
                        {
                            diagnozyList.Add(d.Nazev);
                        }

                        // Створюємо анонімний об'єкт з діагнозами
                        var appointmentWithDiagnozy = new
                        {
                            appointment.IdNavsteva,
                            appointment.PacientId,
                            appointment.Datum,
                            appointment.Mistnost,
                            appointment.Status,
                            PACIENTJmeno = pacient.FirstName,
                            PACIENTPrijmeni = pacient.LastName,
                            Diagnozy = diagnozyList
                        };

                        PastAppointments.Add(appointmentWithDiagnozy);
                    }
                }

                if (!PastAppointments.Any())
                {
                    NoAppointmentsMessage = "Лікар не має пройдених зустрічей.";
                }
                else
                {
                    NoAppointmentsMessage = string.Empty;
                }
            }
            else
            {
                NoAppointmentsMessage = "Лікар не має пройдених зустрічей.";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
