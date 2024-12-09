using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows; 
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;

namespace BDAS2_SEM.ViewModel
{
    public class SystemCatalogVM : INotifyPropertyChanged
    {
        private readonly ISystemCatalogRepository _repository;

        private ObservableCollection<SYSTEM_CATALOG> _systemCatalog;
        public ObservableCollection<SYSTEM_CATALOG> SystemCatalog
        {
            get { return _systemCatalog; }
            set
            {
                _systemCatalog = value;
                OnPropertyChanged(nameof(SystemCatalog));
            }
        }

        private string _searchQuery;
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    FilterCatalogEntries();
                }
            }
        }

        private ObservableCollection<SYSTEM_CATALOG> _allEntries;

        public event PropertyChangedEventHandler PropertyChanged;
        public SystemCatalogVM(ISystemCatalogRepository repository)
        {
            _repository = repository;
            SystemCatalog = new ObservableCollection<SYSTEM_CATALOG>();
            _allEntries = new ObservableCollection<SYSTEM_CATALOG>();
            LoadDataAsync();
        }
        private async void LoadDataAsync()
        {
            try
            {
                var data = await _repository.GetSystemCatalog();

                _allEntries.Clear();
                foreach (var entry in data)
                {
                    _allEntries.Add(entry);
                }

                FilterCatalogEntries();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterCatalogEntries()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                SystemCatalog = new ObservableCollection<SYSTEM_CATALOG>(_allEntries);
            }
            else
            {
                var filtered = new ObservableCollection<SYSTEM_CATALOG>();
                foreach (var entry in _allEntries)
                {
                    if ((entry.ObjectName != null && entry.ObjectName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (entry.ObjectType != null && entry.ObjectType.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        filtered.Add(entry);
                    }
                }
                SystemCatalog = filtered;
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}