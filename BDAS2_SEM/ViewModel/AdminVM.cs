﻿// ViewModel/AdminVM.cs

using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using BDAS2_SEM.View;
using BDAS2_SEM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using BDAS2_SEM.View.AdminViews;

namespace BDAS2_SEM.ViewModel
{
    public class AdminVM : INotifyPropertyChanged
    {
        private readonly IServiceProvider _serviceProvider;

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

        public AdminVM(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            Tabs = new ObservableCollection<TabItemVM>
            {
                new TabItemVM { 
                    Name = "Analyze",
                    Content = _serviceProvider.GetRequiredService<AnalyzeView>()    
                },
                new TabItemVM {
                    Name = "New Users",
                    Content = _serviceProvider.GetRequiredService<NewUsersView>()
                },
                new TabItemVM
                {
                    Name = "Simulate",
                    Content = _serviceProvider.GetRequiredService<SimulateView>()
                },
                new TabItemVM
                {
                    Name = "All tables",
                    Content = _serviceProvider.GetRequiredService<AllTablesView>()
                },
                new TabItemVM
                {
                    Name = "Log",
                    Content = _serviceProvider.GetRequiredService<LogView>()
                },
                new TabItemVM
                {
                    Name = "System Catalog",
                    Content = _serviceProvider.GetRequiredService<SystemCatalogView>()
                }
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