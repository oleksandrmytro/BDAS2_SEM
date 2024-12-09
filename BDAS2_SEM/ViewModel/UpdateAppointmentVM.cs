// UpdateAppointmentVM.cs
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class UpdateAppointmentVM : INotifyPropertyChanged
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IOrdinaceZamestnanecRepository _ordinaceZamestnanecRepository;
        private readonly IMistnostRepository _mistnostRepository;
        private NAVSTEVA _appointment;
        private Func<NAVSTEVA, Task> _callback;
        private int _doctorId;
        private int _ordinaceId; 

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
                    _ = InitializeAvailableRoomsAndTimesAsync();
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

        private int? _selectedRoom;
        public int? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (_selectedRoom != value)
                {
                    _selectedRoom = value;
                    OnPropertyChanged();
                    _ = UpdateAvailableTimesAsync();
                }
            }
        }

        public ObservableCollection<string> AvailableTimes { get; set; }
        public ObservableCollection<int> AvailableRooms { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public UpdateAppointmentVM(
            INavstevaRepository navstevaRepository,
            IOrdinaceZamestnanecRepository ordinaceZamestnanecRepository,
            IMistnostRepository mistnostRepository)
        {
            _navstevaRepository = navstevaRepository;
            _ordinaceZamestnanecRepository = ordinaceZamestnanecRepository;
            _mistnostRepository = mistnostRepository;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => Cancel());

            AvailableTimes = new ObservableCollection<string>();
            AvailableRooms = new ObservableCollection<int>();
        }

        public async void Initialize(NAVSTEVA appointment, Func<NAVSTEVA, Task> callback, int doctorId)
        {
            _appointment = appointment;
            _callback = callback;
            _doctorId = doctorId;

            var ordinaceZamestnanec = await _ordinaceZamestnanecRepository.GetOrdinaceZamestnanecByZamestnanecId(_doctorId);
            if (ordinaceZamestnanec != null)
            {
                _ordinaceId = ordinaceZamestnanec.OrdinaceId;
            }
            else
            {
                MessageBox.Show("We could not find a residency for this doctor.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SelectedDate = _appointment.Datum?.Date;
            SelectedTime = _appointment.Datum?.ToString("HH:mm");
            SelectedRoom = _appointment.MistnostId;

            await InitializeAvailableRoomsAndTimesAsync(); 
        }

        private async Task InitializeAvailableRoomsAndTimesAsync()
        {
            if (SelectedDate.HasValue)
            {
                try
                {
                    var availableRoomsTimes = await _navstevaRepository.GetAvailableRoomsAndTimes(_ordinaceId, SelectedDate.Value);

                    if (availableRoomsTimes == null || !availableRoomsTimes.Any())
                    {
                        MessageBox.Show("No rooms available for the selected date.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        AvailableRooms.Clear();
                        AvailableTimes.Clear();
                        return;
                    }

                    var rooms = availableRoomsTimes.Select(rt => rt.Room).Distinct();

                    AvailableRooms.Clear();
                    foreach (var room in rooms)
                    {
                        AvailableRooms.Add(room);
                    }

                    if (AvailableRooms.Any())
                    {
                        if (SelectedRoom == null || !AvailableRooms.Contains(SelectedRoom.Value))
                        {
                            SelectedRoom = AvailableRooms.First();
                        }
                    }
                    else
                    {
                        SelectedRoom = null;
                    }

                    await UpdateAvailableTimesAsync(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when getting available rooms and times: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                AvailableRooms.Clear();
                AvailableTimes.Clear();
            }
        }

        private async Task UpdateAvailableTimesAsync()
        {
            if (SelectedDate.HasValue && SelectedRoom.HasValue)
            {
                try
                {
                    var availableRoomsTimes = await _navstevaRepository.GetAvailableRoomsAndTimes(_ordinaceId, SelectedDate.Value);

                    if (availableRoomsTimes == null || !availableRoomsTimes.Any())
                    {
                        MessageBox.Show("No available time slots for the selected date and room.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        AvailableTimes.Clear();
                        SelectedTime = null;
                        return;
                    }

                    var times = availableRoomsTimes
                        .Where(rt => rt.Room == SelectedRoom.Value)
                        .Where(rt => !string.IsNullOrEmpty(rt.TimeSlot))
                        .Select(rt => rt.TimeSlot)
                        .Distinct()
                        .OrderBy(t => t);

                    AvailableTimes.Clear();
                    foreach (var time in times)
                    {
                        AvailableTimes.Add(time);
                    }

                    if (!AvailableTimes.Contains(SelectedTime))
                    {
                        SelectedTime = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when getting available time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                AvailableTimes.Clear();
            }
        }

        private async Task SaveAsync()
        {
            if (SelectedDate.HasValue && !string.IsNullOrEmpty(SelectedTime) && SelectedRoom.HasValue)
            {
                if (TimeSpan.TryParseExact(SelectedTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan time))
                {
                    DateTime newAppointmentDateTime = SelectedDate.Value.Date + time;

                    try
                    {
                        var isAvailable = await _navstevaRepository.IsTimeSlotAvailable(_doctorId, newAppointmentDateTime, SelectedRoom.Value, _appointment.IdNavsteva);

                        if (isAvailable)
                        {
                            var mistnost = await _mistnostRepository.GetMistnostByNumber(SelectedRoom.Value);

                            if (mistnost != null)
                            {
                                _appointment.Datum = newAppointmentDateTime;
                                _appointment.MistnostId = mistnost.IdMistnost; 
                                await _callback(_appointment);
                                CloseWindow();
                            }
                            else
                            {
                                MessageBox.Show("The room with the specified number could not be found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The selected time slot is already taken. Please select another time.", "Clock slot inaccessible", MessageBoxButton.OK, MessageBoxImage.Warning);
                            await InitializeAvailableRoomsAndTimesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving a record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The time format is incorrect. Please select the correct time.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a date, time and room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}