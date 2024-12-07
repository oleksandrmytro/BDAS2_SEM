using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using BDAS2_SEM.View.AdminViews;
using BDAS2_SEM.Repository;

namespace BDAS2_SEM.ViewModel
{
    public class SimulateVM : INotifyPropertyChanged
    {
        private ObservableCollection<dynamic> _users;
        public ObservableCollection<dynamic> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();

                // Налаштовуємо фільтрацію
                var view = CollectionViewSource.GetDefaultView(_users);
                if (view != null)
                {
                    view.Filter = UserFilter;
                }
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();

                // Оновлюємо фільтр при зміні пошукового запиту
                CollectionViewSource.GetDefaultView(Users)?.Refresh();
            }
        }

        private ObservableCollection<ROLE> _availableRoles;
        public ObservableCollection<ROLE> AvailableRoles
        {
            get => _availableRoles;
            set
            {
                _availableRoles = value;
                OnPropertyChanged();
            }
        }


        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly IWindowService _windowService;

        private ICommand _loadUsersCommand;
        public ICommand LoadUsersCommand => _loadUsersCommand ??= new RelayCommand(async (a) => await LoadUsersAsync());

        private ICommand _simulateCommand;
        public ICommand SimulateCommand => _simulateCommand ??= new RelayCommand<dynamic>(async (dynamicUser) => await SimulateAsync(dynamicUser));

        public SimulateVM(
            IUzivatelDataRepository uzivatelDataRepository,
            IZamestnanecRepository zamestnanecRepository,
            IPacientRepository pacientRepository,
            IWindowService windowService,
            IRoleRepository roleRepository)
        {
            _uzivatelDataRepository = uzivatelDataRepository;
            _zamestnanecRepository = zamestnanecRepository;
            _pacientRepository = pacientRepository;
            _windowService = windowService;

            Users = new ObservableCollection<dynamic>();
            LoadRolesAsync(roleRepository);
            LoadUsersCommand.Execute(null);
        }

        private async void LoadRolesAsync(IRoleRepository roleRepository)
        {
            var roles = await roleRepository.GetAllRoles();
            AvailableRoles = new ObservableCollection<ROLE>(roles);
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await _uzivatelDataRepository.GetAllUzivatelDatas();

                var usersWithNames = new ObservableCollection<dynamic>();

                foreach (var user in users)
                {
                    string firstName = null;
                    string lastName = null;

                    if (user.RoleId == 2 && user.pacientId.HasValue)
                    {
                        var pacient = await _pacientRepository.GetPacientByUserDataId(user.Id);
                        firstName = pacient.Jmeno;
                        lastName = pacient.Prijmeni;

                        dynamic dynamicUser = new ExpandoObject();
                        dynamicUser.User = user;
                        dynamicUser.FirstName = firstName;
                        dynamicUser.LastName = lastName;

                        usersWithNames.Add(dynamicUser);
                    }
                    else if (user.RoleId == 3 && user.zamestnanecId.HasValue)
                    {
                        var zamestnanec = await _zamestnanecRepository.GetZamestnanecByUserDataId(user.Id);
                        firstName = zamestnanec.Jmeno;
                        lastName = zamestnanec.Prijmeni;


                        dynamic dynamicUser = new ExpandoObject();
                        dynamicUser.User = user;
                        dynamicUser.FirstName = firstName;
                        dynamicUser.LastName = lastName;

                        usersWithNames.Add(dynamicUser);
                    }

                }

                Users = usersWithNames;

                // Налаштовуємо фільтр після встановлення Users
                var view = CollectionViewSource.GetDefaultView(Users);
                if (view != null)
                {
                    view.Filter = UserFilter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалось завантажити користувачів: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool UserFilter(object item)
        {
            if (string.IsNullOrEmpty(SearchQuery))
                return true;

            dynamic dynamicUser = item as dynamic;
            if (dynamicUser == null)
                return false;

            string search = SearchQuery.ToLower();

            string firstName = dynamicUser.FirstName ?? "";
            string lastName = dynamicUser.LastName ?? "";
            string email = dynamicUser.User.Email ?? "";

            // Знаходимо назву ролі
            string roleName = "Unknown Role";
            if (dynamicUser.User.RoleId is int roleId && AvailableRoles != null)
            {
                var role = AvailableRoles.FirstOrDefault(r => r.IdRole == roleId);
                if (role != null)
                    roleName = role.Nazev ?? "Unknown Role";
            }

            return firstName.ToLower().Contains(search)
                   || lastName.ToLower().Contains(search)
                   || email.ToLower().Contains(search)
                   || roleName.ToLower().Contains(search);
        }


        private async Task SimulateAsync(dynamic dynamicUser)
        {
            try
            {
                var user = dynamicUser.User as UZIVATEL_DATA;
                if (user != null)
                {
                    if (user.RoleId == 4)
                    {
                        _windowService.OpenAdminWindow();
                    }
                    else if (user.RoleId == 3)
                    {
                        var zamestnanec = await _zamestnanecRepository.GetZamestnanecByUserDataId(user.Id);
                        if (zamestnanec != null)
                        {
                            _windowService.OpenDoctorWindow(zamestnanec);
                        }
                    }
                    else if (user.RoleId == 2)
                    {
                        var pacient = await _pacientRepository.GetPacientByUserDataId(user.Id);
                        if (pacient != null)
                        {
                            _windowService.OpenPatientWindow(pacient);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Роль користувача не підтримується для симуляції.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Симуляція не вдалася: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseAdminWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is AdminWindow)
                {
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}