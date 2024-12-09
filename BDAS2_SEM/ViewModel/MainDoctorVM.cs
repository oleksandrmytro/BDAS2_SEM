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
        }

        private async Task LoadDoctorActivityDataAsync()
        {
            int doctorId = _doctor.IdZamestnanec;

            try
            {
                DoctorActivityResult activityResult = await _analyzeRepository.AnalyzeDoctorActivityAsync(doctorId);

                if (activityResult != null)
                {
                    TotalVisits = activityResult.TotalVisits;
                    LastVisitDate = activityResult.LastVisitDate;
                    TotalOperations = activityResult.TotalOperations;
                    LastOperationDate = activityResult.LastOperationDate;
                    TotalMedicines = activityResult.TotalMedicines;

                    AnalysisResult = $"Physician activity analysis completed successfully.\n" +
                                     $"Total visits: {TotalVisits}\n" +
                                     $"Date of last visit: {LastVisitDate?.ToString("dd.MM.yyyy") ?? "N/A"}\n" +
                                     $"Total operations: {TotalOperations}\n" +
                                     $"Date of last operation: {LastOperationDate?.ToString("dd.MM.yyyy") ?? "N/A"}\n" +
                                     $"Total prescribed medicines: {TotalMedicines}";
                }
                else
                {
                    AnalysisResult = "No data available.";
                }
            }
            catch (Exception ex)
            {
                AnalysisResult = $"Error when loading activity data: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}