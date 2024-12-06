// ViewModel/AssignAppointmentVM.cs

using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using System.Globalization;
using BDAS2_SEM.Services.Interfaces;

namespace BDAS2_SEM.ViewModel
{
    public class AssignAppointmentVM : INotifyPropertyChanged
    {
        private readonly NAVSTEVA _navsteva;
        private readonly IMistnostRepository _mistnostRepository;
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IOrdinaceZamestnanecRepository _ordinaceZamestnanecRepository;
        private readonly IDoctorContextService _doctorContextService;

        private int _doctorId;
        private int _ordinaceId;

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
                    _ = LoadAvailableRoomsAndTimesAsync();
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

        public ObservableCollection<string> AvailableTimes { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<int> AvailableRooms { get; set; } = new ObservableCollection<int>();

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

        private Dictionary<int, int> _roomMapping = new Dictionary<int, int>();

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AssignAppointmentVM(NAVSTEVA navsteva,
                                   IMistnostRepository mistnostRepository,
                                   INavstevaRepository navstevaRepository,
                                   IOrdinaceZamestnanecRepository ordinaceZamestnanecRepository,
                                   IDoctorContextService doctorContextService)
        {
            _navsteva = navsteva;
            _mistnostRepository = mistnostRepository;
            _navstevaRepository = navstevaRepository;
            _ordinaceZamestnanecRepository = ordinaceZamestnanecRepository;
            _doctorContextService = doctorContextService;

            _doctorId = _doctorContextService.CurrentDoctor.IdZamestnanec;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(Cancel);

            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            var ordinaceZamestnanec = await _ordinaceZamestnanecRepository.GetOrdinaceZamestnanecByZamestnanecId(_doctorId);
            if (ordinaceZamestnanec != null)
            {
                _ordinaceId = ordinaceZamestnanec.OrdinaceId;
            }
            else
            {
                MessageBox.Show("Не вдалося знайти ординатуру для даного лікаря.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            await LoadAvailableRoomsAndTimesAsync();
        }

        private async Task LoadAvailableRoomsAndTimesAsync()
        {
            AvailableRooms.Clear();
            AvailableTimes.Clear();
            _roomMapping.Clear();

            if (SelectedDate.HasValue)
            {
                try
                {
                    var availableRoomsAndTimes = await _navstevaRepository.GetAvailableRoomsAndTimes(_ordinaceId, SelectedDate.Value);

                    if (availableRoomsAndTimes == null || !availableRoomsAndTimes.Any())
                    {
                        MessageBox.Show("Немає доступних кімнат для вибраної дати.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var rooms = availableRoomsAndTimes.Select(rt => rt.Room).Distinct();

                    foreach (var room in rooms)
                    {
                        AvailableRooms.Add(room);
                    }

                    if (AvailableRooms.Any())
                    {
                        SelectedRoom = AvailableRooms.First();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при отриманні доступних кімнат: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task UpdateAvailableTimesAsync()
        {
            AvailableTimes.Clear();

            if (SelectedDate.HasValue && SelectedRoom.HasValue)
            {
                try
                {
                    var availableRoomsAndTimes = await _navstevaRepository.GetAvailableRoomsAndTimes(_ordinaceId, SelectedDate.Value);

                    var times = availableRoomsAndTimes
                        .Where(rt => rt.Room == SelectedRoom.Value)
                        .Select(rt => rt.TimeSlot)
                        .Distinct()
                        .OrderBy(t => t);

                    foreach (var time in times)
                    {
                        AvailableTimes.Add(time);
                    }

                    if (AvailableTimes.Any())
                    {
                        SelectedTime = AvailableTimes.First();
                    }
                    else
                    {
                        SelectedTime = null;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при отриманні доступного часу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async Task SaveAsync()
        {
            if (SelectedDate.HasValue && !string.IsNullOrEmpty(SelectedTime) && SelectedRoom.HasValue)
            {
                if (TimeSpan.TryParseExact(SelectedTime, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan time))
                {
                    DateTime appointmentDateTime = SelectedDate.Value.Date + time;

                    try
                    {
                        var isAvailable = await _navstevaRepository.IsTimeSlotAvailable(_doctorId, appointmentDateTime, SelectedRoom.Value);

                        if (isAvailable)
                        {
                            var mistnost = await _mistnostRepository.GetMistnostByNumber(SelectedRoom.Value);

                            if (mistnost != null)
                            {
                                _navsteva.Datum = appointmentDateTime;
                                _navsteva.MistnostId = mistnost.IdMistnost;
                                _navsteva.StatusId = 1;

                                CloseAction?.Invoke(_navsteva);
                            }
                            else
                            {
                                MessageBox.Show("Не вдалося знайти кімнату з вказаним номером.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Обраний часовий слот вже зайнятий. Будь ласка, виберіть інший час.", "Часовий слот недоступний", MessageBoxButton.OK, MessageBoxImage.Warning);
                            await LoadAvailableRoomsAndTimesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при збереженні запису: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Неправильний формат часу. Будь ласка, виберіть коректний час.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть дату, час та кімнату.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
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