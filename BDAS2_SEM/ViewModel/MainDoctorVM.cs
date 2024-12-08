// ViewModel/MainDoctorVM.cs
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BDAS2_SEM.ViewModel
{
    public class MainDoctorVM : INotifyPropertyChanged
    {
        private ZAMESTNANEC _doctor;
        private readonly IAnalyzeRepository _analyzeRepository;

        // Свойства для отображения информации о враче


        public void SetDoctor(ZAMESTNANEC doctor)
        {
            if (doctor == null) throw new ArgumentNullException(nameof(doctor));
            _doctor = doctor;

            FirstName = _doctor.Jmeno;
            LastName = _doctor.Prijmeni;
            Phone = _doctor.Telefon;

            LoadDoctorActivityDataAsync();
        }


        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged(nameof(FirstName));
                }
            }
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged(nameof(LastName));
                }
            }
        }

        private long _phone;
        public long Phone
        {
            get => _phone;
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }

        // Свойства для привязки активности в UI
        private int _totalVisits;
        public int TotalVisits
        {
            get => _totalVisits;
            set
            {
                if (_totalVisits != value)
                {
                    _totalVisits = value;
                    OnPropertyChanged(nameof(TotalVisits));
                }
            }
        }

        private DateTime? _lastVisitDate;
        public DateTime? LastVisitDate
        {
            get => _lastVisitDate;
            set
            {
                if (_lastVisitDate != value)
                {
                    _lastVisitDate = value;
                    OnPropertyChanged(nameof(LastVisitDate));
                }
            }
        }

        private int _totalOperations;
        public int TotalOperations
        {
            get => _totalOperations;
            set
            {
                if (_totalOperations != value)
                {
                    _totalOperations = value;
                    OnPropertyChanged(nameof(TotalOperations));
                }
            }
        }

        private DateTime? _lastOperationDate;
        public DateTime? LastOperationDate
        {
            get => _lastOperationDate;
            set
            {
                if (_lastOperationDate != value)
                {
                    _lastOperationDate = value;
                    OnPropertyChanged(nameof(LastOperationDate));
                }
            }
        }

        private int _totalMedicines;
        public int TotalMedicines
        {
            get => _totalMedicines;
            set
            {
                if (_totalMedicines != value)
                {
                    _totalMedicines = value;
                    OnPropertyChanged(nameof(TotalMedicines));
                }
            }
        }

        private string _analysisResult;
        public string AnalysisResult
        {
            get => _analysisResult;
            set
            {
                if (_analysisResult != value)
                {
                    _analysisResult = value;
                    OnPropertyChanged(nameof(AnalysisResult));
                }
            }
        }

        public MainDoctorVM(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            _analyzeRepository = serviceProvider.GetRequiredService<IAnalyzeRepository>();


            // Загрузка данных активности врача
        }

        private async Task LoadDoctorActivityDataAsync()
        {
            int doctorId = _doctor.IdZamestnanec;

            try
            {
                // Вызов метода, возвращающего DoctorActivityResult
                DoctorActivityResult activityResult = await _analyzeRepository.AnalyzeDoctorActivityAsync(doctorId);

                if (activityResult != null)
                {
                    // Присвоение значений свойствам для UI
                    TotalVisits = activityResult.TotalVisits;
                    LastVisitDate = activityResult.LastVisitDate;
                    TotalOperations = activityResult.TotalOperations;
                    LastOperationDate = activityResult.LastOperationDate;
                    TotalMedicines = activityResult.TotalMedicines;
                    
                    // Формирование полного текста результата, если необходимо
                    AnalysisResult = $"Анализ активности врача завершён успешно.\n" +
                                     $"Всего посещений: {TotalVisits}\n" +
                                     $"Дата последнего посещения: {LastVisitDate?.ToString("dd.MM.yyyy") ?? "N/A"}\n" +
                                     $"Всего операций: {TotalOperations}\n" +
                                     $"Дата последней операции: {LastOperationDate?.ToString("dd.MM.yyyy") ?? "N/A"}\n" +
                                     $"Всего назначенных лекарств: {TotalMedicines}";
                }
                else
                {
                    AnalysisResult = "Данные отсутствуют.";
                }
            }
            catch (Exception ex)
            {
                AnalysisResult = $"Ошибка при загрузке данных активности: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}