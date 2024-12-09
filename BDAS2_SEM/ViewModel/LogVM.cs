using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows; 
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;

namespace BDAS2_SEM.ViewModel
{
    public class LogVM : INotifyPropertyChanged
    {
        private readonly ILogRepository _repository;

        private ObservableCollection<LOG> _logs;
        public ObservableCollection<LOG> Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                OnPropertyChanged(nameof(Logs));
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
                    FilterLogs();
                }
            }
        }

        private ObservableCollection<LOG> _allLogs;

        public event PropertyChangedEventHandler PropertyChanged;

        public LogVM(ILogRepository repository)
        {
            _repository = repository;
            Logs = new ObservableCollection<LOG>();
            _allLogs = new ObservableCollection<LOG>();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                var data = await _repository.GetAllLogs();

                _allLogs.Clear();
                foreach (var entry in data)
                {
                    _allLogs.Add(entry);
                }
                FilterLogs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error downloading logs: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterLogs()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                Logs = new ObservableCollection<LOG>(_allLogs);
            }
            else
            {
                var filtered = new ObservableCollection<LOG>();
                foreach (var entry in _allLogs)
                {
                    if ((entry.TableName != null && entry.TableName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (entry.OperationType != null && entry.OperationType.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        filtered.Add(entry);
                    }
                }
                Logs = filtered;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}