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

        public ObservableCollection<dynamic> Appointments { get; set; }

        public ICommand AcceptAppointmentCommand { get; }
        public ICommand CancelAppointmentCommand { get; }
        public ICommand AssignAppointmentCommand { get; }

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
            Appointments = new ObservableCollection<dynamic>();

            AcceptAppointmentCommand = new RelayCommand(AcceptAppointment);
            CancelAppointmentCommand = new RelayCommand(CancelAppointment);
            AssignAppointmentCommand = new RelayCommand(AssignAppointment);
        }

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            var appointments = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
            Appointments.Clear();
            if (appointments != null)
            {
                foreach (var appointment in appointments)
                {
                    Appointments.Add(appointment);
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
                    IdNavsteva = appointment.IDNAVSTEVA,
                    PacientId = appointment.PACIENTIDPACIENT,
                    Status = Status.Accepted
                };

                await _navstevaRepository.UpdateNavsteva(navsteva);
                OnPropertyChanged(nameof(Appointments));
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
                    PacientId = appointment.PACIENTIDPACIENT,
                    Status = Status.Cancelled
                };

                await _navstevaRepository.UpdateNavsteva(navsteva);
                Appointments.Remove(appointment);
            }
        }

        private void AssignAppointment(object parameter)
        {
            if (parameter != null)
            {
                dynamic appointment = parameter;
                NAVSTEVA navsteva = new NAVSTEVA
                {
                    IdNavsteva = (int)appointment.IDNAVSTEVA,
                    PacientId = (int)appointment.PACIENTID,
                    Status = (Status)(int)appointment.STATUS
                };

                _windowService.OpenAssignAppointmentWindow(navsteva, async (updatedAppointment) =>
                {
                    if (updatedAppointment != null)
                    {
                        await _navstevaRepository.UpdateNavsteva(updatedAppointment);
                        Appointments.Remove(appointment);
                    }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}