﻿// ViewModel/AssignDiagnosisVM.cs

using BDAS2_SEM.Commands;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class AssignDiagnosisVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IDiagnozaRepository _diagnozaRepository;
        private readonly ILekRepository _lekRepository;
        private readonly ILekDiagnozaRepository _lekDiagnozaRepository;
        private readonly INavstevaDiagnozaRepository _navstevaDiagnozaRepository;
        private readonly IOperaceRepository _operaceRepository;
        private readonly IOperaceZamestnanecRepository _operaceZamestnanecRepository;
        private NAVSTEVA _appointment;
        private int _employeeId;

        private string _newDiagnozaNazev;
        public string NewDiagnozaNazev
        {
            get => _newDiagnozaNazev;
            set
            {
                if (_newDiagnozaNazev != value)
                {
                    _newDiagnozaNazev = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _newDiagnozaPopis;
        public string NewDiagnozaPopis
        {
            get => _newDiagnozaPopis;
            set
            {
                if (_newDiagnozaPopis != value)
                {
                    _newDiagnozaPopis = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _newOperationName;
        public string NewOperationName
        {
            get => _newOperationName;
            set
            {
                if (_newOperationName != value)
                {
                    _newOperationName = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private DateTime? _newOperationDate;
        public DateTime? NewOperationDate
        {
            get => _newOperationDate;
            set
            {
                if (_newOperationDate != value)
                {
                    _newOperationDate = value;
                    OnPropertyChanged();
                    UpdateDatum();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _newOperationTimeString;
        public string NewOperationTimeString
        {
            get => _newOperationTimeString;
            set
            {
                if (_newOperationTimeString != value)
                {
                    _newOperationTimeString = value;
                    OnPropertyChanged();
                    UpdateOperationTime();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private TimeSpan? _newOperationTime;
        public TimeSpan? NewOperationTime
        {
            get => _newOperationTime;
            set
            {
                if (_newOperationTime != value)
                {
                    _newOperationTime = value;
                    OnPropertyChanged();
                    UpdateDatum();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private DateTime? _datum;
        public DateTime? Datum
        {
            get => _datum;
            private set
            {
                if (_datum != value)
                {
                    _datum = value;
                    OnPropertyChanged();
                }
            }
        }

        private DIAGNOZA _selectedDiagnoza;
        public DIAGNOZA SelectedDiagnoza
        {
            get => _selectedDiagnoza;
            set
            {
                if (_selectedDiagnoza != value)
                {
                    _selectedDiagnoza = value;
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private LEK _selectedLek;
        public LEK SelectedLek
        {
            get => _selectedLek;
            set
            {
                if (_selectedLek != value)
                {
                    _selectedLek = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DIAGNOZA> Diagnozy { get; set; }
        public ObservableCollection<LEK> Leks { get; set; }
        public ObservableCollection<LEK> SelectedLeks { get; set; }

        public ICommand AssignDiagnosisCommand { get; }
        public ICommand AddNewDiagnozaCommand { get; }
        public ICommand AddLekToDiagnosisCommand { get; }
        public ICommand RemoveLekFromDiagnosisCommand { get; }
        public ICommand CancelCommand { get; }

        public AssignDiagnosisVM(
            INavstevaRepository navstevaRepository,
            IDiagnozaRepository diagnozaRepository,
            ILekRepository lekRepository,
            ILekDiagnozaRepository lekDiagnozaRepository,
            INavstevaDiagnozaRepository navstevaDiagnozaRepository,
            IOperaceRepository operaceRepository,
            IOperaceZamestnanecRepository operaceZamestnanecRepository,
            NAVSTEVA appointment,
            int employeeId)
        {

            _navstevaRepository = navstevaRepository;
            _diagnozaRepository = diagnozaRepository;
            _lekRepository = lekRepository;
            _lekDiagnozaRepository = lekDiagnozaRepository;
            _navstevaDiagnozaRepository = navstevaDiagnozaRepository;
            _operaceRepository = operaceRepository;
            _operaceZamestnanecRepository = operaceZamestnanecRepository;
            _employeeId = employeeId;

            _appointment = appointment;

            Diagnozy = new ObservableCollection<DIAGNOZA>();
            Leks = new ObservableCollection<LEK>();
            SelectedLeks = new ObservableCollection<LEK>();

            AssignDiagnosisCommand = new RelayCommand(async _ => await AssignDiagnosis(), _ => CanAssignDiagnosis());
            AddNewDiagnozaCommand = new RelayCommand(async _ => await AddNewDiagnoza(), _ => CanAddNewDiagnoza());
            AddLekToDiagnosisCommand = new RelayCommand(_ => AddLekToDiagnosis(), _ => SelectedLek != null);
            RemoveLekFromDiagnosisCommand = new RelayCommand<LEK>(RemoveLekFromDiagnosis, lek => lek != null);
            CancelCommand = new RelayCommand(_ => Cancel());

            LoadDiagnozy();
            LoadLeks();
        }

        private async void LoadDiagnozy()
        {
            try
            {
                var diagnozy = await _diagnozaRepository.GetAllDiagnoses();
                Diagnozy.Clear();
                foreach (var diagnoza in diagnozy)
                {
                    Diagnozy.Add(diagnoza);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when uploading diagnoses: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadLeks()
        {
            try
            {
                var leky = await _lekRepository.GetAllLeks();
                Leks.Clear();
                foreach (var lek in leky)
                {
                    Leks.Add(lek);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when uploading medication: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddLekToDiagnosis()
        {
            if (SelectedLek != null && !SelectedLeks.Contains(SelectedLek))
            {
                SelectedLeks.Add(SelectedLek);
            }
        }

        private void RemoveLekFromDiagnosis(LEK lek)
        {
            if (lek != null && SelectedLeks.Contains(lek))
            {
                SelectedLeks.Remove(lek);
            }
        }

        private bool CanAssignDiagnosis()
        {
            bool isDiagnosisValid = SelectedDiagnoza != null || !string.IsNullOrWhiteSpace(NewDiagnozaNazev);
            bool isOperationValid = !string.IsNullOrWhiteSpace(NewOperationName) && NewOperationDate.HasValue && NewOperationTime.HasValue;

            return isDiagnosisValid; 
        }

        private async Task AssignDiagnosis()
        {
            try
            {
                if (SelectedDiagnoza == null && !string.IsNullOrWhiteSpace(NewDiagnozaNazev))
                {
                    await AddNewDiagnoza();
                }

                if (SelectedDiagnoza == null)
                {
                    MessageBox.Show("Please select or enter a diagnosis.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var navstevaDiagnoza = new NAVSTEVA_DIAGNOZA
                {
                    NavstevaId = _appointment.IdNavsteva,
                    DiagnozaId = SelectedDiagnoza.IdDiagnoza
                };

                await _navstevaDiagnozaRepository.AddNavstevaDiagnoza(navstevaDiagnoza);

                foreach (var lek in SelectedLeks)
                {
                    var lekDiagnoza = new LEK_DIAGNOZA
                    {
                        DiagnozaId = SelectedDiagnoza.IdDiagnoza,
                        LekId = lek.IdLek
                    };
                    await _lekDiagnozaRepository.AddLekDiagnoza(lekDiagnoza);
                }

                if (!string.IsNullOrWhiteSpace(NewOperationName) && Datum.HasValue)
                {
                    var newOperace = new OPERACE
                    {
                        Nazev = NewOperationName,
                        Datum = Datum.Value,
                        DiagnozaId = SelectedDiagnoza.IdDiagnoza
                    };

                    int newOperaceId = await _operaceRepository.AddOperace(newOperace);

                    var operaceZamestnanec = new OPERACE_ZAMESTNANEC
                    {
                        OperaceId = newOperaceId,
                        ZamestnanecId = _employeeId
                    };
                    await _operaceZamestnanecRepository.AddOperaceZamestnanec(operaceZamestnanec);
                }

                MessageBox.Show("Diagnosis, medication and surgery successfully prescribed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when assigning data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddNewDiagnoza()
        {
            return !string.IsNullOrWhiteSpace(NewDiagnozaNazev);
        }

        private async Task AddNewDiagnoza()
        {
            try
            {
                var newDiagnoza = new DIAGNOZA
                {
                    Nazev = NewDiagnozaNazev,
                    Popis = NewDiagnozaPopis
                };

                int newDiagnozaId = await _diagnozaRepository.AddDiagnoza(newDiagnoza);
                newDiagnoza.IdDiagnoza = newDiagnozaId;
                Diagnozy.Add(newDiagnoza);
                SelectedDiagnoza = newDiagnoza;

                NewDiagnozaNazev = string.Empty;
                NewDiagnozaPopis = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when adding a diagnosis: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }

        private void UpdateDatum()
        {
            if (NewOperationDate.HasValue && NewOperationTime.HasValue)
            {
                Datum = NewOperationDate.Value.Date + NewOperationTime.Value;
            }
            else if (NewOperationDate.HasValue)
            {
                Datum = NewOperationDate.Value.Date;
            }
            else
            {
                Datum = null;
            }
        }

        private void UpdateOperationTime()
        {
            if (TimeSpan.TryParseExact(_newOperationTimeString, new[] { @"hh\:mm", @"hh\:mm\:ss" }, CultureInfo.InvariantCulture, out TimeSpan parsedTime))
            {
                NewOperationTime = parsedTime;
            }
            else
            {
                NewOperationTime = null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                string result = null;
                if (columnName == nameof(NewOperationTimeString))
                {
                    if (string.IsNullOrWhiteSpace(NewOperationTimeString))
                    {
                        result = "Час є обов'язковим.";
                    }
                    else if (!TimeSpan.TryParseExact(NewOperationTimeString, new[] { @"hh\:mm", @"hh\:mm\:ss" }, CultureInfo.InvariantCulture, out _))
                    {
                        result = "The time format is incorrect. Use HH:MM or HH:MM:SS.";
                    }
                }
                else if (columnName == nameof(NewOperationName))
                {
                    if (string.IsNullOrWhiteSpace(NewOperationName))
                    {
                        result = "Назва операції є обов'язковою.";
                    }
                }
                else if (columnName == nameof(NewOperationDate))
                {
                    if (!NewOperationDate.HasValue)
                    {
                        result = "The date of the transaction is mandatory.";
                    }
                }
                return result;
            }
        }

        public string Error => null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}