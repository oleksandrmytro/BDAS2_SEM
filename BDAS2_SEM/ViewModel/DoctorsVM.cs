// ViewModel/DoctorsVM.cs
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View.DoctorViews;
using BDAS2_SEM.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel
{
    public class DoctorsVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private ZAMESTNANEC _zamestnanec;

        public ObservableCollection<TabItemVM> Tabs { get; set; }
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

        public DoctorsVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<TabItemVM>
            {
                new TabItemVM {
                    Name = "Main",
                    Content = _serviceProvider.GetRequiredService<MainDoctorView>()
                },
                new TabItemVM {
                    Name = "Appointments",
                    Content = _serviceProvider.GetRequiredService<AppointmentsView>()
                },
                // Add more tabs as needed
            };
            OnPropertyChanged(nameof(Tabs));
            SelectedTab = Tabs.FirstOrDefault();
        }

        public void SetEmployee(ZAMESTNANEC zamestnanec)
        {
            _zamestnanec = zamestnanec;
            // Pass the doctor to AppointmentsVM
            var appointmentsTab = Tabs.FirstOrDefault(t => t.Name == "Appointments");
            if (appointmentsTab != null && appointmentsTab.Content is AppointmentsView appointmentsView)
            {
                if (appointmentsView.DataContext is AppointmentsVM appointmentsVM)
                {
                    appointmentsVM.SetDoctor(_zamestnanec);
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