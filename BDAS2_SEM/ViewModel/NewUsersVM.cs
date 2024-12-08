using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;

namespace BDAS2_SEM.ViewModel
{
    public class NewUsersVM : INotifyPropertyChanged
    {
        private ObservableCollection<UZIVATEL_DATA> _newUsers;
        public ObservableCollection<UZIVATEL_DATA> NewUsers
        {
            get { return _newUsers; }
            set
            {
                if (_newUsers != value)
                {
                    _newUsers = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<ROLE> _availableRoles;
        public ObservableCollection<ROLE> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                if (_availableRoles != value)
                {
                    _availableRoles = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AssignRoleCommand { get; }

        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IWindowService _windowService;

        public NewUsersVM(IUzivatelDataRepository uzivatelDataRepository, IWindowService windowService, IRoleRepository roleRepository)
        {
            _uzivatelDataRepository = uzivatelDataRepository;
            _windowService = windowService;
            _roleRepository = roleRepository;
            AssignRoleCommand = new RelayCommand(AssignRole);
            LoadRolesAsync();
            LoadNewUsers();
        }

        private async void LoadNewUsers()
        {
            var users = await _uzivatelDataRepository.GetUsersWithUndefinedRole();
            NewUsers = new ObservableCollection<UZIVATEL_DATA>(users);
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _roleRepository.GetAllRoles();
            AvailableRoles = new ObservableCollection<ROLE>(roles);
            OnPropertyChanged(nameof(AvailableRoles));

            // Отладочный вывод
            foreach (var role in AvailableRoles)
            {
                Console.WriteLine($"Role Loaded: {role.IdRole} - {role.Nazev}");
            }
        }

        private async void AssignRole(object parameter)
        {
            if (parameter is UZIVATEL_DATA user)
            {
                if (user.RoleId == 2 || user.RoleId == 3)
                {
                    if (user.RoleId == 3)
                    {
                        _windowService.OpenNewEmployeeWindow(user, async (bool isSaved) =>
                        {
                            if (isSaved)
                            {
                                await _uzivatelDataRepository.UpdateUserData(user);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    NewUsers.Remove(user);
                                });
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    user.RoleId = 1;
                                    OnPropertyChanged(nameof(user.RoleId));
                                });
                            }
                        });
                    }
                    else
                    {
                        await _uzivatelDataRepository.UpdateUserData(user);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            NewUsers.Remove(user);
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Please select a role before assigning.", "ROLE Not Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}