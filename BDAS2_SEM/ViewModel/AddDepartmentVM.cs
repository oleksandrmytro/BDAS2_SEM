// AddDepartmentVM.cs
using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BDAS2_SEM.Repository.Interfaces;

namespace BDAS2_SEM.ViewModel
{
    public class AddDepartmentVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly IOrdinaceRepository _departmentRepository;
        private readonly IWindowService _windowService;
        private readonly Action<ORDINACE> _onDepartmentAdded;

        private string _departmentName;
        public string DepartmentName
        {
            get => _departmentName;
            set
            {
                _departmentName = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddDepartmentVM(Action<ORDINACE> onDepartmentAdded, IWindowService windowService, IOrdinaceRepository departmentRepository)
        {
            _onDepartmentAdded = onDepartmentAdded;
            _windowService = windowService;
            _departmentRepository = departmentRepository;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private async void Save(object parameter)
        {
            var newDepartment = new ORDINACE
            {
                Nazev = this.DepartmentName
            };

            await _departmentRepository.AddOrdinace(newDepartment);
            _onDepartmentAdded?.Invoke(newDepartment);
            CloseWindow();
        }

        private bool CanSave(object parameter)
        {
            return !string.IsNullOrWhiteSpace(DepartmentName);
        }

        private void Cancel(object parameter)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.DialogResult = true;
                    window.Close();
                    break;
                }
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(DepartmentName))
                {
                    if (string.IsNullOrWhiteSpace(DepartmentName))
                        return "Department name is required.";
                }
                return null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}