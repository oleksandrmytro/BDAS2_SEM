using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class AppointmentsVM : INotifyPropertyChanged
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IWindowService _windowService;
        private ZAMESTNANEC _doctor;

        public ObservableCollection<dynamic> AppointmentRequests { get; set; }
        public ObservableCollection<dynamic> FutureAppointments { get; set; }
        public ObservableCollection<dynamic> PastAppointments { get; set; }

        public ICommand AcceptAppointmentCommand { get; }
        public ICommand CancelAppointmentCommand { get; }
        public ICommand AssignAppointmentCommand { get; }
        public ICommand UpdateAppointmentCommand { get; }

        private string _noAppointmentsMessage;
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

        public AppointmentsVM(IServiceProvider serviceProvider, IWindowService windowService)
        {
            _navstevaRepository = serviceProvider.GetRequiredService<INavstevaRepository>();
            _windowService = windowService;
            AppointmentRequests = new ObservableCollection<dynamic>();
            FutureAppointments = new ObservableCollection<dynamic>();
            PastAppointments = new ObservableCollection<dynamic>();

            AcceptAppointmentCommand = new RelayCommand(AcceptAppointment);
            CancelAppointmentCommand = new RelayCommand(CancelAppointment);
            AssignAppointmentCommand = new RelayCommand(AssignAppointment);
            UpdateAppointmentCommand = new RelayCommand(UpdateAppointment);
        }

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            var appointments = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
            AppointmentRequests.Clear();
            FutureAppointments.Clear();
            PastAppointments.Clear();

            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    int status = (int)appointment.StatusId;

                    // Отримуємо ім'я та прізвище пацієнта за ID візиту
                    var pacient = await _navstevaRepository.GetPatientNameByAppointmentId(appointment.IdNavsteva);

                    // Створюємо новий об'єкт з необхідними властивостями
                    var appointmentWithPatient = new
                    {
                        appointment.IdNavsteva,
                        appointment.PacientId,
                        appointment.Datum,
                        appointment.MistnostId,
                        appointment.StatusId,
                        PACIENTJMENO = pacient.FirstName,
                        PACIENTPRIJMENI = pacient.LastName
                    };

                    // Перевіряємо статус та дату візиту
                    if (status == 3)
                    {
                        AppointmentRequests.Add(appointmentWithPatient);
                    }
                    else if (status == 1)
                    {
                        DateTime? appointmentDate = appointment.Datum;

                        if (appointmentDate.HasValue)
                        {
                            if (appointmentDate.Value >= DateTime.Now)
                            {
                                FutureAppointments.Add(appointmentWithPatient);
                            }
                            else
                            {
                                PastAppointments.Add(appointmentWithPatient);
                            }
                        }
                        else
                        {
                            FutureAppointments.Add(appointmentWithPatient);
                        }
                    }
                }
                NoAppointmentsMessage = string.Empty;
            }
            else
            {
                NoAppointmentsMessage = "Лікар не має запланованих зустрічей.";
            }
        }

        private async void AcceptAppointment(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = (int)appointment.IdNavsteva,
                    PacientId = (int)appointment.PacientId,
                    StatusId = (int)appointment.Status,
                    Datum = appointment.Datum,
                    MistnostId = appointment.Mistnost
                };

                await _navstevaRepository.UpdateNavsteva(navsteva);
                LoadAppointments();
            }
        }

        private async void CancelAppointment(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = appointment.IDNAVSTEVA,
                    PacientId = appointment.PACIENTID,
                    StatusId = 2
                };

                await _navstevaRepository.UpdateNavsteva(navsteva);
                LoadAppointments();
            }
        }

        private void AssignAppointment(object parameter)
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
                    MistnostId = appointment.MistnostId
                };

                _windowService.OpenAssignAppointmentWindow(navsteva, async (updatedAppointment) =>
                {
                    if (updatedAppointment != null)
                    {
                        await _navstevaRepository.UpdateNavsteva(updatedAppointment);
                        LoadAppointments();
                    }
                });
            }
        }

        private void UpdateAppointment(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = (int)appointment.IdNavsteva,
                    PacientId = (int)appointment.PacientId,
                    StatusId = (int)appointment.Status,
                    Datum = appointment.Datum,
                    MistnostId = appointment.Mistnost != null ? Convert.ToInt32(appointment.Mistnost) : (int?)null
                };

                _windowService.UpdateAppointmentWindow(navsteva, async (updatedAppointment) =>
                {
                    if (updatedAppointment != null)
                    {
                        await _navstevaRepository.UpdateNavsteva(updatedAppointment);
                        LoadAppointments();
                    }
                }, _doctor.IdZamestnanec); // Pass the doctor ID here
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
