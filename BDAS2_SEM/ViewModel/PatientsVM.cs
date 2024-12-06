using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View;
using BDAS2_SEM.View.PatientViews;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace BDAS2_SEM.ViewModel
{
    public class PatientsVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPatientContextService _patientContextService;

        public ObservableCollection<TabItemVM> Tabs { get; set; }
        private TabItemVM _selectedTab;
        public ICommand LogoutCommand { get; }

        private string _patientName;
        public string PatientName
        {
            get => _patientName;
            set
            {
                _patientName = value;
                OnPropertyChanged();
            }
        }

        private ImageSource _patientImage;
        public ImageSource PatientImage
        {
            get => _patientImage;
            set
            {
                _patientImage = value;
                OnPropertyChanged();
            }
        }

        public TabItemVM SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                OnPropertyChanged();
            }
        }

        public PatientsVM(IServiceProvider serviceProvider, IPatientContextService patientContextService)
        {
            _serviceProvider = serviceProvider;
            _patientContextService = patientContextService;

            LogoutCommand = new RelayCommand(Logout);

            InitializeTabs();
            LoadPatientInfo();
        }

        // Конструктор по умолчанию для XAML
        public PatientsVM() : this(
            App.ServiceProvider.GetRequiredService<IServiceProvider>(),
            App.ServiceProvider.GetRequiredService<IPatientContextService>())
        {
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<TabItemVM>
            {
                new TabItemVM {
                    Name = "Appointments",
                    Content = _serviceProvider.GetRequiredService<PAppointmentsView>()
                },
                new TabItemVM {
                    Name = "Diagnoses",
                    Content = _serviceProvider.GetRequiredService<PDiagnosesView>()

                },
                new TabItemVM {
                    Name = "Settings",
                    Content = _serviceProvider.GetRequiredService<PSettingsView>()
                }
                
            };
            OnPropertyChanged(nameof(Tabs));
            SelectedTab = Tabs.FirstOrDefault();
        }

        private void LoadPatientInfo()
        {
            var patient = _patientContextService.CurrentPatient;
            if (patient != null)
            {
                PatientName = $"{patient.Jmeno} {patient.Prijmeni}";
            }
        }

        private void Logout(object parameter)
        {
            var currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
            authWindow.Show();
            currentWindow?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}