using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BDAS2_SEM.Repository;
using System.Threading.Tasks;

namespace BDAS2_SEM.ViewModel
{
    public class NewEmployeeVM : INotifyPropertyChanged
    {
        private readonly IZamestnanecRepository _zamestnanecRepository;
        private readonly IAdresaRepository _adresaRepository;
        private readonly IPoziceRepository _poziceRepository;
        private readonly IUzivatelDataRepository _uzivatelDataRepository;
        private readonly IOrdinaceRepository _ordinaceRepository;
        private readonly IOrdinaceZamestnanecRepository _ordinaceZamestnanecRepository;
        private readonly UZIVATEL_DATA _userData;
        private readonly IWindowService _windowService;
        private readonly Action<bool> _onClosed;

        public ObservableCollection<ADRESA> Addresses { get; set; }
        public ObservableCollection<ZAMESTNANEC> Supervisors { get; set; }
        public ObservableCollection<POZICE> Positions { get; set; }
        public ObservableCollection<ORDINACE> Departments { get; set; }

        // Employee properties
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public long Telefon { get; set; }
        public int? NadrazenyZamestnanecId { get; set; }
        public int AdresaId { get; set; }
        public int PoziceId { get; set; }
        public int SelectedDepartmentId { get; set; }

        // Commands
        public ICommand SaveCommand { get; }
        public ICommand AddAddressCommand { get; }
        public ICommand AddPositionCommand { get; }
        public ICommand AddDepartmentCommand { get; }

        public NewEmployeeVM(UZIVATEL_DATA userData, IWindowService windowService, IServiceProvider serviceProvider, Action<bool> onClosed)
        {
            _userData = userData;
            _windowService = windowService;
            _onClosed = onClosed;

            _zamestnanecRepository = serviceProvider.GetRequiredService<IZamestnanecRepository>();
            _adresaRepository = serviceProvider.GetRequiredService<IAdresaRepository>();
            _poziceRepository = serviceProvider.GetRequiredService<IPoziceRepository>();
            _uzivatelDataRepository = serviceProvider.GetRequiredService<IUzivatelDataRepository>();
            _ordinaceRepository = serviceProvider.GetRequiredService<IOrdinaceRepository>();
            _ordinaceZamestnanecRepository = serviceProvider.GetRequiredService<IOrdinaceZamestnanecRepository>();

            SaveCommand = new RelayCommand(Save);
            AddAddressCommand = new RelayCommand(AddAddress);
            AddPositionCommand = new RelayCommand(AddPosition);
            AddDepartmentCommand = new RelayCommand(AddDepartment);

            LoadAddresses();
            LoadSupervisors();
            LoadPositions();
            LoadDepartments();
        }

        private async void LoadAddresses()
        {
            var addresses = await _adresaRepository.GetAllAddresses();
            Addresses = new ObservableCollection<ADRESA>(addresses);
            OnPropertyChanged(nameof(Addresses));
        }

        private async void LoadSupervisors()
        {
            var supervisors = await _zamestnanecRepository.GetAllZamestnanci();
            Supervisors = new ObservableCollection<ZAMESTNANEC>(supervisors);
            OnPropertyChanged(nameof(Supervisors));
        }

        private async void LoadPositions()
        {
            var positions = await _poziceRepository.GetAllPozices();
            Positions = new ObservableCollection<POZICE>(positions);
            OnPropertyChanged(nameof(Positions));
        }

        private async void LoadDepartments()
        {
            var departments = await _ordinaceRepository.GetAllOrdinaces();
            Departments = new ObservableCollection<ORDINACE>(departments);
            OnPropertyChanged(nameof(Departments));
        }

        private async void Save(object parameter)
        {
            var zamestnanec = new ZAMESTNANEC
            {
                Jmeno = this.Jmeno,
                Prijmeni = this.Prijmeni,
                Telefon = this.Telefon,
                NadrazenyZamestnanecId = this.NadrazenyZamestnanecId,
                AdresaId = this.AdresaId,
                PoziceId = this.PoziceId,
                UserDataId = _userData.Id
            };

            try
            {
                // Добавление Zamestnanec и получение нового ID
                int newZamestnanecId = await _zamestnanecRepository.AddZamestnanec(zamestnanec);

                // Обновление UZIVATEL_DATA с новым Zamestnanec ID
                _userData.zamestnanecId = newZamestnanecId;
                await _uzivatelDataRepository.UpdateUserData(_userData);

                // Создание связи с отделением
                var ordinaceZamestnanec = new ORDINACE_ZAMESTNANEC
                {
                    OrdinaceId = this.SelectedDepartmentId,
                    ZamestnanecId = newZamestnanecId
                };
                await _ordinaceZamestnanecRepository.AddOrdinaceZamestnanec(ordinaceZamestnanec);

                _onClosed?.Invoke(true);

                _windowService.CloseWindow(() =>
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is BDAS2_SEM.View.AdminViews.NewEmployeeWindow)
                        {
                            window.Close();
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show($"Произошла ошибка при сохранении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAddress(object parameter)
        {
            _windowService.OpenAddAddressWindow(OnAddressAdded);
        }

        private void OnAddressAdded(ADRESA newAddress)
        {
            Addresses.Add(newAddress);
            AdresaId = newAddress.IdAdresa;
            OnPropertyChanged(nameof(Addresses));
            OnPropertyChanged(nameof(AdresaId));
        }

        private void AddPosition(object parameter)
        {
            _windowService.OpenAddPositionWindow(OnPositionAdded);
        }

        private void OnPositionAdded(POZICE newPosition)
        {
            Positions.Add(newPosition);
            PoziceId = newPosition.IdPozice;
            OnPropertyChanged(nameof(Positions));
            OnPropertyChanged(nameof(PoziceId));
        }

        private void AddDepartment(object parameter)
        {
            _windowService.OpenAddDepartmentWindow(OnDepartmentAdded);
        }

        private void OnDepartmentAdded(ORDINACE newDepartment)
        {
            Departments.Add(newDepartment);
            SelectedDepartmentId = newDepartment.IdOrdinace;
            OnPropertyChanged(nameof(Departments));
            OnPropertyChanged(nameof(SelectedDepartmentId));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void InvokeOnClosed(bool isSaved)
        {
            _onClosed?.Invoke(isSaved);
        }
    }
}