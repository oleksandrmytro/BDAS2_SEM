using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum; // Include the Status enum
using BDAS2_SEM.Repository.Interfaces;
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
        private ZAMESTNANEC _doctor;

        public ObservableCollection<NAVSTEVA> Appointments { get; set; }

        public ICommand AcceptAppointmentCommand { get; }
        public ICommand CancelAppointmentCommand { get; }

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

        public AppointmentsVM(INavstevaRepository navstevaRepository)
        {
            _navstevaRepository = navstevaRepository;
            Appointments = new ObservableCollection<NAVSTEVA>();

            AcceptAppointmentCommand = new RelayCommand(AcceptAppointment);
            CancelAppointmentCommand = new RelayCommand(CancelAppointment);
        }

        public void SetDoctor(ZAMESTNANEC doctor)
        {
            _doctor = doctor;
            LoadAppointments();
        }

        private async void LoadAppointments()
        {
            //var navstevy = await _navstevaRepository.GetAppointmentsByDoctorId(_doctor.IdZamestnanec);
            //Appointments.Clear();
            //if (navstevy != null && navstevy.Any())
            //{
            //    foreach (var navsteva in navstevy)
            //    {
            //        Appointments.Add(navsteva);
            //    }
            //    NoAppointmentsMessage = string.Empty; // Clear any previous message
            //}
            //else
            //{
            //    NoAppointmentsMessage = "Лікар не має запланованих прийомів."; // "The doctor has no scheduled appointments."
            //}
        }

        private async void AcceptAppointment(object parameter)
        {
            if (parameter is NAVSTEVA navsteva)
            {
                navsteva.Status = Status.Accepted;
                await _navstevaRepository.UpdateNavsteva(navsteva);
                OnPropertyChanged(nameof(Appointments));
            }
        }

        private async void CancelAppointment(object parameter)
        {
            if (parameter is NAVSTEVA navsteva)
            {
                navsteva.Status = Status.Cancelled;
                await _navstevaRepository.UpdateNavsteva(navsteva);
                OnPropertyChanged(nameof(Appointments));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}