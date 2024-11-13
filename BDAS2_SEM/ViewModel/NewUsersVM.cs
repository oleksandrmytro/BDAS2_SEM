﻿using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class NewUsersVM : INotifyPropertyChanged
    {
        public ObservableCollection<UZIVATEL_DATA> NewUsers { get; set; }
        public ObservableCollection<Role> AvailableRoles { get; set; }
        public ICommand AssignRoleCommand { get; }

        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IWindowService _windowService;

        public NewUsersVM(IUzivatelDataRepository uzivatelDataRepository, IWindowService windowService)
        {
            _uzivatelDataRepository = uzivatelDataRepository;
            _windowService = windowService;

            AssignRoleCommand = new RelayCommand(AssignRole);
            LoadRoles();
            LoadNewUsers();
        }

        private async void LoadNewUsers()
        {
            var users = await _uzivatelDataRepository.GetUsersWithUndefinedRole();
            NewUsers = new ObservableCollection<UZIVATEL_DATA>(users);
            OnPropertyChanged(nameof(NewUsers));
        }

        private void LoadRoles()
        {
            AvailableRoles = new ObservableCollection<Role>
            {
                Role.PACIENT,
                Role.ZAMESTNANEC
            };
            OnPropertyChanged(nameof(AvailableRoles));
        }

        private async void AssignRole(object parameter)
        {
            if (parameter is UZIVATEL_DATA user)
            {
                await _uzivatelDataRepository.UpdateUserRole(user.Id, user.RoleUzivatel);
                NewUsers.Remove(user);
                OnPropertyChanged(nameof(NewUsers));

                if (user.RoleUzivatel == Role.ZAMESTNANEC)
                {
                    _windowService.OpenNewEmployeeWindow(user);
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