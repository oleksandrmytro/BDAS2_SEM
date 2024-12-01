// ViewModel/DoctorsVM.cs
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
using Microsoft.Extensions.DependencyInjection; // Ensure you have a RelayCommand or similar command implementation

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private ZAMESTNANEC _zamestnanec;

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
                }
                // Add more tabs as needed
            };
            OnPropertyChanged(nameof(Tabs));
            SelectedTab = Tabs.FirstOrDefault();
        }

        // Set the employee (doctor) information
        public void SetEmployee(ZAMESTNANEC zamestnanec)
        {
            _zamestnanec = zamestnanec;
            EmployeeName = $"{_zamestnanec.Jmeno} {_zamestnanec.Prijmeni}";

            // Load the employee image
            //EmployeeImage = LoadEmployeeImage(_zamestnanec.ImagePath);

            // Pass the doctor to AppointmentsVM
            var appointmentsTab = Tabs.FirstOrDefault(t => t.Name == "Appointments");
            var disgnosesTab = Tabs.FirstOrDefault(t => t.Name == "Diagnoses");
            if (appointmentsTab != null && appointmentsTab.Content is AppointmentsView appointmentsView)
            {
                if (appointmentsView.DataContext is AppointmentsVM appointmentsVM)
                {
                    appointmentsVM.SetDoctor(_zamestnanec);
                }
            }
            if (disgnosesTab != null && disgnosesTab.Content is DDiagnosesView disgnosesView)
            {
                if (disgnosesView.DataContext is DDiagnosesVM disgnosesVM)
                {
                    disgnosesVM.SetDoctor(_zamestnanec);
                }
            }
        }

        // Load the employee image from the specified path
        private ImageSource LoadEmployeeImage(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    // Return a default image if no image path is provided or file doesn't exist
                    return new BitmapImage(new Uri("pack://application:,,,/Images/default-user.png"));
                }
            }
            catch
            {
                // Handle exceptions and return a default image
                return new BitmapImage(new Uri("pack://application:,,,/Images/default-user.png"));
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
