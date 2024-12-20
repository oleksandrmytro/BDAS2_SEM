﻿using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsListVM : INotifyPropertyChanged
    {
        private readonly IDoctorInfoRepository _doctorInfoRepository;
        private readonly IPatientContextService _patientContextService;
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IZamestnanecNavstevaRepository _zamestnanecNavstevaRepository;
        private readonly IWindowService _windowService;

        private ObservableCollection<DOCTOR_INFO> _allDoctors;
        private ObservableCollection<DOCTOR_INFO> _doctors;
        public ObservableCollection<DOCTOR_INFO> Doctors
        {
            get => _doctors;
            set
            {
                _doctors = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateAppointmentCommand { get; }
        public ICommand RefreshCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterDoctors();
            }
        }

        public DoctorsListVM(
            IDoctorInfoRepository doctorInfoRepository,
            IPatientContextService patientContextService,
            INavstevaRepository navstevaRepository,
            IZamestnanecNavstevaRepository zamestnanecNavstevaRepository,
            IWindowService windowService)
        {
            _doctorInfoRepository = doctorInfoRepository;
            _patientContextService = patientContextService;
            _navstevaRepository = navstevaRepository;
            _zamestnanecNavstevaRepository = zamestnanecNavstevaRepository;
            _windowService = windowService;

            Doctors = new ObservableCollection<DOCTOR_INFO>();
            _allDoctors = new ObservableCollection<DOCTOR_INFO>();

            CreateAppointmentCommand = new RelayCommand(CreateAppointment);
            RefreshCommand = new AsyncRelayCommand(RefreshDoctorsAsync);

            LoadDoctorsAsync();
        }

        // Конструктор по замовчуванню для XAML
        public DoctorsListVM() : this(
            App.ServiceProvider.GetRequiredService<IDoctorInfoRepository>(),
            App.ServiceProvider.GetRequiredService<IPatientContextService>(),
            App.ServiceProvider.GetRequiredService<INavstevaRepository>(),
            App.ServiceProvider.GetRequiredService<IZamestnanecNavstevaRepository>(),
            App.ServiceProvider.GetRequiredService<IWindowService>())
        {
        }

        private async Task LoadDoctorsAsync()
        {
            try
            {
                var doctors = await _doctorInfoRepository.GetAllDoctors();
                _allDoctors = new ObservableCollection<DOCTOR_INFO>(doctors);
                Doctors = new ObservableCollection<DOCTOR_INFO>(_allDoctors);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when uploading doctors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterDoctors()
        {
            if (_allDoctors == null)
                return;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Doctors = new ObservableCollection<DOCTOR_INFO>(_allDoctors);
            }
            else
            {
                var filteredDoctors = _allDoctors.Where(d =>
                    (d.FirstName != null && d.FirstName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (d.Surname != null && d.Surname.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                    (d.Phone.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase)) || 
                    (d.Department != null && d.Department.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                ).ToList();

                Doctors = new ObservableCollection<DOCTOR_INFO>(filteredDoctors);
            }
        }

        private async Task RefreshDoctorsAsync(object parameter)
        {
            await LoadDoctorsAsync();
        }

        private async void CreateAppointment(object parameter)
        {
            if (parameter != null)
            {
                var selectedDoctor = parameter as DOCTOR_INFO;
                if (selectedDoctor != null)
                {
                    var pacientId = _patientContextService.CurrentPatient.IdPacient;

                    var newAppointment = new NAVSTEVA
                    {
                        PacientId = pacientId,
                        StatusId = 3
                    };

                    try
                    {
                        var newAppointmentId = await _navstevaRepository.AddNavsteva(newAppointment);
                        newAppointment.IdNavsteva = newAppointmentId;

                        var zamestnanecNavsteva = new ZAMESTNANEC_NAVSTEVA
                        {
                            ZamestnanecId = selectedDoctor.DoctorId,
                            NavstevaId = newAppointmentId
                        };

                        await _zamestnanecNavstevaRepository.AddZamestnanecNavsteva(zamestnanecNavsteva);

                        MessageBox.Show("Record successfully created!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error creating a record: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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