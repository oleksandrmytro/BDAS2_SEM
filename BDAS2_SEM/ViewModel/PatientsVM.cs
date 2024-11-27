using BDAS2_SEM.Model;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View.PatientViews;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BDAS2_SEM.ViewModel
{
    public class PatientsVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IPatientContextService _patientContextService;

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

        public PatientsVM(IServiceProvider serviceProvider, IPatientContextService patientContextService)
        {
            _serviceProvider = serviceProvider;
            _patientContextService = patientContextService;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<TabItemVM>
            {
                new TabItemVM {
                    Name = "Appointments",
                    Content = _serviceProvider.GetRequiredService<PAppointmentsView>()
                },
            };
            OnPropertyChanged(nameof(Tabs));
            SelectedTab = Tabs.FirstOrDefault();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}