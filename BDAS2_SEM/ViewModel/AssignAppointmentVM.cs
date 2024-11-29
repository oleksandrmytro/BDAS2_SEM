// ViewModel/AssignAppointmentVM.cs
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class AssignAppointmentVM : INotifyPropertyChanged
    {
        private readonly NAVSTEVA _navsteva;

        public Action<NAVSTEVA> CloseAction { get; set; }

        private DateTime? _selectedDate;
        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedTime;
        public string SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime != value)
                {
                    _selectedTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> AvailableTimes { get; set; }
        public ObservableCollection<int> Rooms { get; set; }

        private int _selectedRoom;
        public int SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (_selectedRoom != value)
                {
                    _selectedRoom = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AssignAppointmentVM(NAVSTEVA navsteva)
        {
            _navsteva = navsteva;

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);

            // Initialize rooms (fetch from repository if needed)
            Rooms = new ObservableCollection<int> { 101, 102, 103, 104, 105 };

            // Initialize available times (e.g., every 30 minutes from 8 AM to 5 PM)
            AvailableTimes = new ObservableCollection<string>();
            for (int hour = 8; hour <= 17; hour++)
            {
                AvailableTimes.Add($"{hour:D2}:00");
                AvailableTimes.Add($"{hour:D2}:30");
            }
        }

        private void Save(object obj)
        {
            if (SelectedDate.HasValue && !string.IsNullOrEmpty(SelectedTime))
            {
                if (TimeSpan.TryParse(SelectedTime, out TimeSpan time))
                {
                    DateTime appointmentDateTime = SelectedDate.Value.Date.Add(time);
                    _navsteva.Datum = appointmentDateTime;
                    _navsteva.Mistnost = SelectedRoom;
                    _navsteva.Status = Status.Accepted;

                    CloseAction?.Invoke(_navsteva);
                }
                else
                {
                    // Handle invalid time format
                    System.Windows.MessageBox.Show("Invalid time format. Please select a valid time.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
            else
            {
                // Handle missing date or time
                System.Windows.MessageBox.Show("Please select both date and time.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void Cancel(object obj)
        {
            CloseAction?.Invoke(null);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
