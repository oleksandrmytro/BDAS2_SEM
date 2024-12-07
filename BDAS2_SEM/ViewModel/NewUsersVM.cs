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
        public ObservableCollection<UZIVATEL_DATA> NewUsers { get; set; }
        public ObservableCollection<ROLE> AvailableRoles { get; set; }
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
            OnPropertyChanged(nameof(NewUsers));
        }

        private async Task LoadRolesAsync()
        {
            var roles = await _roleRepository.GetAllRoles();
            AvailableRoles = new ObservableCollection<ROLE>(roles);
            OnPropertyChanged(nameof(AvailableRoles));
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
                                //await _uzivatelDataRepository.UpdateUserRole(user.Id, user.RoleId);
                                await _uzivatelDataRepository.UpdateUserData(user);

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    NewUsers.Remove(user);
                                    OnPropertyChanged(nameof(NewUsers));
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
                        //await _uzivatelDataRepository.UpdateUserRole(user.Id, user.RoleId);
                        await _uzivatelDataRepository.UpdateUserData(user);

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            NewUsers.Remove(user);
                            OnPropertyChanged(nameof(NewUsers));
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
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}