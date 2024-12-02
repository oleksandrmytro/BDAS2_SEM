using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.Model;
using BDAS2_SEM.Commands;
using BDAS2_SEM.View.DoctorViews;
using Microsoft.Extensions.DependencyInjection;
using BDAS2_SEM.Repository.Interfaces;

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private ZAMESTNANEC _zamestnanec;
        private readonly IBlobTableRepository _blobRepository;

        // Collection of tabs to display in the navigation panel
        public ObservableCollection<TabItemVM> Tabs { get; set; }

        // Currently selected tab
        private TabItemVM _selectedTab;
        public TabItemVM SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        // Full name of the employee for display
        private string _employeeName;
        public string EmployeeName
        {
            get => _employeeName;
            set
            {
                _employeeName = value;
                OnPropertyChanged();
            }
        }

        // Employee image for display
        private ImageSource _employeeImage;
        public ImageSource EmployeeImage
        {
            get => _employeeImage;
            set
            {
                _employeeImage = value;
                OnPropertyChanged();
            }
        }

        // Command to handle logout action
        public ICommand LogoutCommand { get; }

        public DoctorsVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _blobRepository = _serviceProvider.GetRequiredService<IBlobTableRepository>();
            InitializeTabs();

            // Initialize commands
            LogoutCommand = new RelayCommand(Logout);
        }

        // Initialize the tabs displayed in the navigation panel
        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<TabItemVM>
            {
                new TabItemVM
                {
                    Name = "Main",
                    Content = _serviceProvider.GetRequiredService<MainDoctorView>()
                },
                new TabItemVM
                {
                    Name = "Appointments",
                    Content = _serviceProvider.GetRequiredService<AppointmentsView>()
                },
                new TabItemVM
                {
                    Name = "Diagnoses",
                    Content = _serviceProvider.GetRequiredService<DDiagnosesView>()
                },
                // Settings Tab
                new TabItemVM
                {
                    Name = "Settings",
                    Content = CreateSettingsView()
                }
            };
            OnPropertyChanged(nameof(Tabs));
            SelectedTab = Tabs.FirstOrDefault();
        }

        // Create the settings view and pass the necessary dependencies
        private DSettingsView CreateSettingsView()
        {
            var zamestnanecRepository = _serviceProvider.GetRequiredService<IZamestnanecRepository>();
            var blobRepository = _serviceProvider.GetRequiredService<IBlobTableRepository>();
            var windowService = _serviceProvider.GetRequiredService<IWindowService>();

            var settingsVM = new DSettingsVM(_zamestnanec, zamestnanecRepository, blobRepository, windowService, UpdateAvatar);
            return new DSettingsView(settingsVM);
        }

        // Set the employee (doctor) information
        public void SetEmployee(ZAMESTNANEC zamestnanec)
        {
            _zamestnanec = zamestnanec;
            EmployeeName = $"{_zamestnanec.Jmeno} {_zamestnanec.Prijmeni}";

            // Load the employee image
            LoadEmployeeImage(_zamestnanec.IdZamestnanec);

            // Pass the doctor to AppointmentsVM
            var appointmentsTab = Tabs.FirstOrDefault(t => t.Name == "Appointments");
            var diagnosesTab = Tabs.FirstOrDefault(t => t.Name == "Diagnoses");
            if (appointmentsTab != null && appointmentsTab.Content is AppointmentsView appointmentsView)
            {
                if (appointmentsView.DataContext is AppointmentsVM appointmentsVM)
                {
                    appointmentsVM.SetDoctor(_zamestnanec);
                }
            }
            if (diagnosesTab != null && diagnosesTab.Content is DDiagnosesView diagnosesView)
            {
                if (diagnosesView.DataContext is DDiagnosesVM diagnosesVM)
                {
                    diagnosesVM.SetDoctor(_zamestnanec);
                }
            }

            // Update the settings view with the current doctor
            var settingsTab = Tabs.FirstOrDefault(t => t.Name == "Settings");
            if (settingsTab != null && settingsTab.Content is DSettingsView settingsView)
            {
                if (settingsView.DataContext is DSettingsVM settingsVM)
                {
                    settingsVM.Doctor = _zamestnanec;
                }
            }
        }

        // Load the employee image from the database
        private async void LoadEmployeeImage(int zamestnanecId)
        {
            var blob = await _blobRepository.GetBlobByZamestnanecId(zamestnanecId);
            if (blob != null && blob.Obsah != null)
            {
                using (var ms = new MemoryStream(blob.Obsah))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    EmployeeImage = image;
                }
            }
            else
            {
                // Return a default image if no image is found
                EmployeeImage = new BitmapImage(new Uri("pack://application:,,,/Images/default-user.png"));
            }
        }

        // Update the avatar image
        private void UpdateAvatar(byte[] imageData)
        {
            using (var ms = new MemoryStream(imageData))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                EmployeeImage = image;
            }
        }

        // Logout command implementation
        private void Logout(object obj)
        {
            // Implement logout logic here
            // For example, navigate back to the login view or close the application
            // If using a navigation service:
            // _navigationService.NavigateTo("LoginView");

            // If you want to close the application:
            System.Windows.Application.Current.Shutdown();
        }

        // Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}