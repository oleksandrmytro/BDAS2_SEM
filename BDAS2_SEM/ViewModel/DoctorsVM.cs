using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View.DoctorViews;
using BDAS2_SEM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using BDAS2_SEM.View;
using System.Windows;

public class DoctorsVM : INotifyPropertyChanged
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDoctorContextService _doctorContextService;
    private ZAMESTNANEC _zamestnanec;
    private readonly IBlobTableRepository _blobRepository;
    private readonly IZamestnanecRepository _zamestnanecRepository;
    private readonly IPriponaRepository _priponaRepository;

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
            if (_employeeImage != value)
            {
                _employeeImage = value;
                OnPropertyChanged(nameof(EmployeeImage));
                Console.WriteLine("EmployeeImage property changed.");
            }
        }
    }

    // Command to handle logout action
    public ICommand LogoutCommand { get; }

    public DoctorsVM(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _doctorContextService = _serviceProvider.GetRequiredService<IDoctorContextService>();
        _blobRepository = _serviceProvider.GetRequiredService<IBlobTableRepository>();
        _zamestnanecRepository = _serviceProvider.GetRequiredService<IZamestnanecRepository>();
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
            var uzivatelRepository = _serviceProvider.GetRequiredService<IUzivatelDataRepository>();
            var blobRepository = _serviceProvider.GetRequiredService<IBlobTableRepository>();
            var priponaRepository = _serviceProvider.GetRequiredService<IPriponaRepository>();
            var adresaRepository = _serviceProvider.GetRequiredService<IAdresaRepository>();
            var windowService = _serviceProvider.GetRequiredService<IWindowService>();

            var settingsVM = new DSettingsVM(zamestnanecRepository, uzivatelRepository, blobRepository, priponaRepository, adresaRepository, windowService, null);
            return new DSettingsView(settingsVM);
        }

    // Set the employee (doctor) information
    public async void SetEmployee(ZAMESTNANEC zamestnanec)
    {
        try
        {
            _zamestnanec = zamestnanec;
            EmployeeName = $"{_zamestnanec.Jmeno} {_zamestnanec.Prijmeni}";
            Console.WriteLine($"Setting employee: {EmployeeName}");

            _doctorContextService.CurrentDoctor = zamestnanec;

            // Load the employee image
            if (_zamestnanec.BlobId != 0)
            {
                await LoadEmployeeImage(_zamestnanec.BlobId);
            }
            else
            {
                EmployeeImage = null;
            }

            // Pass the doctor to AppointmentsVM
            var mainTab = Tabs.FirstOrDefault(t => t.Name == "Main");
            var appointmentsTab = Tabs.FirstOrDefault(t => t.Name == "Appointments");
            var diagnosesTab = Tabs.FirstOrDefault(t => t.Name == "Diagnoses");
            if (mainTab != null && mainTab.Content is MainDoctorView mainView)
            {
                if (mainView.DataContext is MainDoctorVM mainVM)
                {
                    mainVM.SetDoctor(_zamestnanec);
                }
            }
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
                    settingsVM.SetDoctor(_zamestnanec);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting employee: {ex.Message}");
        }
    }

    // Load the employee image from the database
    private async Task LoadEmployeeImage(int blobId)
    {
        try
        {
            var blob = await _blobRepository.GetBlobById(blobId);
            if (blob != null && blob.Obsah != null)
            {
                using (var ms = new MemoryStream(blob.Obsah))
                {
                    try
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                        image.Freeze();
                        EmployeeImage = image;
                        Console.WriteLine("Employee image loaded successfully.");
                    }
                    catch (NotSupportedException)
                    {
                        Console.WriteLine("Unsupported image format.");
                        EmployeeImage = null;
                    }
                }
            }
            else
            {
                Console.WriteLine("Avatar not found, setting EmployeeImage to null.");
                EmployeeImage = null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading employee image: {ex}");
            EmployeeImage = null;
        }
    }

    // Logout command implementation
    private void Logout(object obj)
    {
        var authWindow = _serviceProvider.GetRequiredService<AuthWindow>();
        Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        authWindow.Show();
    }

    // Implementation of INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}