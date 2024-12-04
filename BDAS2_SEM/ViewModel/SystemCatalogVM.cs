using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows; // Для MessageBox
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

        // Конструктор з ін'єкцією залежностей
        public SystemCatalogVM(ISystemCatalogRepository repository)
        {
            _repository = repository;
            SystemCatalog = new ObservableCollection<SYSTEM_CATALOG>();
            _allEntries = new ObservableCollection<SYSTEM_CATALOG>();
            LoadDataAsync();
        }

        // Асинхронний метод для завантаження даних
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

                // Ініціалізуємо відображувану колекцію
                FilterCatalogEntries();
            }
            catch (Exception ex)
            {
                // Обробка виключень (можна замінити на відображення повідомлення користувачу)
                MessageBox.Show($"Помилка при завантаженні даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод фільтрації даних за запитом пошуку
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

        // Метод виклику події зміни властивості
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}