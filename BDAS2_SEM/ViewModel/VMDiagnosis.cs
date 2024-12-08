using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Commands;

namespace BDAS2_SEM.ViewModel
{
    public class VMDiagnosis : INotifyPropertyChanged
    {
        private readonly INavstevaRepository _navstevaRepository;
        private readonly IDiagnozaRepository _diagnozaRepository;
        private readonly ILekRepository _lekRepository;
        private readonly INavstevaDiagnozaRepository _navstevaDiagnozaRepository;
        private readonly ILekDiagnozaRepository _lekDiagnozaRepository;
        private readonly IOperaceRepository _operaceRepository;
        private NAVSTEVA _appointment;

        // Diagnosis Properties
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

        // Operation Properties
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

        public ObservableCollection<LEK> SelectedLeks { get; set; }

        public ICommand CloseCommand { get; }

        public VMDiagnosis(
            INavstevaRepository navstevaRepository,
            IDiagnozaRepository diagnozaRepository,
            ILekRepository lekRepository,
            ILekDiagnozaRepository lekDiagnozaRepository,
            INavstevaDiagnozaRepository navstevaDiagnozaRepository,
            IOperaceRepository operaceRepository,
            NAVSTEVA appointment)
        {
            _navstevaRepository = navstevaRepository;
            _diagnozaRepository = diagnozaRepository;
            _lekDiagnozaRepository = lekDiagnozaRepository;
            _lekRepository = lekRepository;
            _navstevaDiagnozaRepository = navstevaDiagnozaRepository;
            _operaceRepository = operaceRepository;
            _appointment = appointment;

            SelectedLeks = new ObservableCollection<LEK>();
            CloseCommand = new RelayCommand(Close);

            LoadDiagnosisData();
        }

        private async void LoadDiagnosisData()
        {
            try
            {
                var diagnozy = await _navstevaDiagnozaRepository.GetDiagnozyByNavstevaIdAsync(_appointment.IdNavsteva);
                if (diagnozy != null)
                {
                    var diagnoza = diagnozy.FirstOrDefault();
                    if (diagnoza != null)
                    {
                        NewDiagnozaNazev = diagnoza.Nazev;
                        NewDiagnozaPopis = diagnoza.Popis;
                        Console.WriteLine($"Loaded Diagnosis: {NewDiagnozaNazev}, {NewDiagnozaPopis}");

                        // Загрузка лекарств, связанных с диагнозом
                        var lekDiagnozy = await _lekDiagnozaRepository.GetLeksByDiagnozaId(diagnoza.IdDiagnoza);
                        SelectedLeks.Clear();
                        foreach (var lek in lekDiagnozy)
                        {
                            SelectedLeks.Add(lek);
                        }

                        // Загрузка данных операции, связанной с диагнозом
                        var operace = await _operaceRepository.GetOperaceByDiagnozaId(diagnoza.IdDiagnoza);
                        if (operace != null)
                        {
                            NewOperationName = operace.Nazev;
                            NewOperationDate = operace.Datum.Date;
                            NewOperationTime = operace.Datum.TimeOfDay;
                            NewOperationTimeString = NewOperationTime?.ToString(@"hh\:mm");
                            Console.WriteLine($"Loaded Operation: {NewOperationName}, {NewOperationDate}, {NewOperationTimeString}");
                        }
                        else
                        {
                            // Если операции нет, очищаем поля
                            NewOperationName = string.Empty;
                            NewOperationDate = null;
                            NewOperationTime = null;
                            NewOperationTimeString = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
            }
        }

        private void Close(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}