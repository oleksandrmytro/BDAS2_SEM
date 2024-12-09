using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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
        public ICommand LoadUsersCommand => _loadUsersCommand ??= new RelayCommand<object>(async (o) => await LoadUsersAsync());
        public ICommand SimulateCommand => _simulateCommand ??= new RelayCommand<dynamic>(async (dynamicUser) => await SimulateAsync(dynamicUser));
        public ICommand RefreshDataCommand => _refreshDataCommand ??= new RelayCommand<object>(async (o) => await RefreshDataAsync());

        private ICommand _loadUsersCommand;
        private ICommand _simulateCommand;
        private ICommand _refreshDataCommand;

        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly IWindowService _windowService;
        private readonly IRoleRepository _roleRepository;

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
            _roleRepository = roleRepository;

            Users = new ObservableCollection<dynamic>();
            AvailableRoles = new ObservableCollection<ROLE>();

            LoadRolesAsync();
            LoadUsersCommand.Execute(null);
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _roleRepository.GetAllRoles();
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

                var view = CollectionViewSource.GetDefaultView(Users);
                if (view != null)
                {
                    view.Filter = UserFilter;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to upload users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RefreshDataAsync()
        {
            await LoadRolesAsync();
            await LoadUsersAsync();
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
                        MessageBox.Show("User role is not supported for simulation.", "Information.", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"The simulation failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}