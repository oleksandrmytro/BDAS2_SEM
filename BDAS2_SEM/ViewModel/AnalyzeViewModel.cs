using BDAS2_SEM.Commands;
using BDAS2_SEM.Repository.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BDAS2_SEM.ViewModel
{
    public class MedicineExpenseResult
    {
        public int DiagnosisId { get; set; }
        public string DiagnosisName { get; set; }
        public decimal TotalExpense { get; set; }
        public int MedicineCount { get; set; }
        public decimal AvgMedicineCost { get; set; }
        public decimal MinMedicineCost { get; set; }
        public decimal MaxMedicineCost { get; set; }
        public string MedicinesList { get; set; }
        public string ExpenseCategory { get; set; }
        public int TotalDiagnoses { get; set; }
        public int NoMedicineDiagnoses { get; set; }
    }

    public class AnalyzeViewModel : INotifyPropertyChanged
    {
        private readonly IAnalyzeRepository _analyzeRepository;

        private string _selectedProcedure;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private string _result;
        private int? _managerId;

        private ObservableCollection<MedicineExpenseResult> _medicineExpenseResults;

        public ObservableCollection<string> Procedures { get; set; }

        public string SelectedProcedure
        {
            get => _selectedProcedure;
            set
            {
                if (_selectedProcedure != value)
                {
                    _selectedProcedure = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? ManagerId
        {
            get => _managerId;
            set
            {
                if (_managerId != value)
                {
                    _managerId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Result
        {
            get => _result;
            set
            {
                if (_result != value)
                {
                    _result = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<MedicineExpenseResult> MedicineExpenseResults
        {
            get => _medicineExpenseResults;
            set
            {
                if (_medicineExpenseResults != value)
                {
                    _medicineExpenseResults = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ExecuteProcedureCommand { get; }

        public AnalyzeViewModel(IAnalyzeRepository analyzeRepository)
        {
            _analyzeRepository = analyzeRepository ?? throw new ArgumentNullException(nameof(analyzeRepository));

            Procedures = new ObservableCollection<string>
            {
                "ANALYZE_DIAGNOSES_AND_MEDICINES",
                "ANALYZE_EMPLOYEE_HIERARCHY",
                "ANALYZE_INCOME_BY_PAYMENT_TYPE",
                "ANALYZE_MEDICINE_EXPENSES"
            };

            ExecuteProcedureCommand = new RelayCommand(async _ => await ExecuteProcedureAsync(), CanExecuteProcedure);

            // Установка начального SelectedProcedure
            SelectedProcedure = Procedures.FirstOrDefault();
        }

        private bool CanExecuteProcedure(object parameter)
        {
            // Здесь можно добавить логику для включения/отключения кнопки "Выполнить" 
            // Например, проверку наличия необходимых данных для выбранной процедуры
            return true;
        }

        public async Task ExecuteProcedureAsync()
        {
            Result = string.Empty; // Очистка предыдущих результатов
            MedicineExpenseResults = new ObservableCollection<MedicineExpenseResult>();

            switch (SelectedProcedure)
            {
                case "ANALYZE_DIAGNOSES_AND_MEDICINES":
                    await ExecuteAnalyzeDiagnosesAndMedicinesAsync();
                    break;
                case "ANALYZE_EMPLOYEE_HIERARCHY":
                    await ExecuteAnalyzeEmployeeHierarchyAsync();
                    break;
                case "ANALYZE_INCOME_BY_PAYMENT_TYPE":
                    await ExecuteAnalyzeIncomeByPaymentTypeAsync();
                    break;
                case "ANALYZE_MEDICINE_EXPENSES":
                    await ExecuteAnalyzeMedicineExpensesAsync();
                    break;
                default:
                    Result = $"Процедура '{SelectedProcedure}' не реализована.";
                    break;
            }
        }

        private async Task ExecuteAnalyzeDiagnosesAndMedicinesAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                Result = "Пожалуйста, выберите начальную и конечную даты.";
                return;
            }

            try
            {
                var analysisResult = await _analyzeRepository.AnalyzeDiagnosesAndMedicines(StartDate.Value, EndDate.Value);
                Result = analysisResult;

                // Дополнительно вывод в консоль (опционально)
                Console.WriteLine(analysisResult);
            }
            catch (Exception ex)
            {
                Result = $"Ошибка при выполнении процедуры: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeEmployeeHierarchyAsync()
        {
            if (!ManagerId.HasValue || ManagerId <= 0)
            {
                Result = "Пожалуйста, введите корректный ID менеджера.";
                return;
            }

            try
            {
                var analysisResult = await _analyzeRepository.AnalyzeEmployeeHierarchy(ManagerId.Value);
                Result = analysisResult;

                // Дополнительно вывод в консоль (опционально)
                Console.WriteLine(analysisResult);
            }
            catch (Exception ex)
            {
                Result = $"Ошибка при выполнении процедуры: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeIncomeByPaymentTypeAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                Result = "Пожалуйста, выберите начальную и конечную даты.";
                return;
            }

            try
            {
                var incomeResults = await _analyzeRepository.AnalyzeIncomeByPaymentType(StartDate.Value, EndDate.Value);
                Result = incomeResults;

                // Дополнительно вывод в консоль (опционально)
                Console.WriteLine(incomeResults);
            }
            catch (Exception ex)
            {
                Result = $"Ошибка при выполнении процедуры: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeMedicineExpensesAsync()
        {
            try
            {
                var expenseResults = await _analyzeRepository.AnalyzeMedicineExpenses();
                Result = expenseResults;

                // Парсинг строкового результата и заполнение коллекции для DataGrid
                MedicineExpenseResults = ParseMedicineExpensesResults(expenseResults);

                // Дополнительно вывод в консоль (опционально)
                Console.WriteLine(expenseResults);
            }
            catch (Exception ex)
            {
                Result = $"Ошибка при выполнении процедуры: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private ObservableCollection<MedicineExpenseResult> ParseMedicineExpensesResults(string expenseResults)
        {
            var results = new ObservableCollection<MedicineExpenseResult>();

            var lines = expenseResults.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Пример строки:
                // "Diagnosis ID: 65, Name: sdf, Total Expense: 37700, Medicine Count: 2, Avg Medicine Cost: 102, Min Medicine Cost: 52, Max Medicine Cost: 152, Medicines List: FDSe, KTR, Expense Category: High, Total Diagnoses: 43, Diagnoses without Medicines: 26"

                var parts = line.Split(',');

                if (parts.Length >= 11)
                {
                    try
                    {
                        string diagnosisIdPart = parts[0].Trim();
                        string diagnosisNamePart = parts[1].Trim();
                        string totalExpensePart = parts[2].Trim();
                        string medicineCountPart = parts[3].Trim();
                        string avgMedicineCostPart = parts[4].Trim();
                        string minMedicineCostPart = parts[5].Trim();
                        string maxMedicineCostPart = parts[6].Trim();
                        string medicinesListPart = parts[7].Trim();
                        string expenseCategoryPart = parts[8].Trim();
                        string totalDiagnosesPart = parts[9].Trim();
                        string noMedicineDiagnosesPart = parts[10].Trim();

                        int diagnosisId = int.Parse(diagnosisIdPart.Split(':')[1].Trim());
                        string diagnosisName = diagnosisNamePart.Split(':')[1].Trim();
                        decimal totalExpense = decimal.Parse(totalExpensePart.Split(':')[1].Trim());
                        int medicineCount = int.Parse(medicineCountPart.Split(':')[1].Trim());
                        decimal avgMedicineCost = decimal.Parse(avgMedicineCostPart.Split(':')[1].Trim());
                        decimal minMedicineCost = decimal.Parse(minMedicineCostPart.Split(':')[1].Trim());
                        decimal maxMedicineCost = decimal.Parse(maxMedicineCostPart.Split(':')[1].Trim());
                        string medicinesList = medicinesListPart.Split(':')[1].Trim();
                        string expenseCategory = expenseCategoryPart.Split(':')[1].Trim();
                        int totalDiagnoses = int.Parse(totalDiagnosesPart.Split(':')[1].Trim());
                        int noMedicineDiagnoses = int.Parse(noMedicineDiagnosesPart.Split(':')[1].Trim());

                        results.Add(new MedicineExpenseResult
                        {
                            DiagnosisId = diagnosisId,
                            DiagnosisName = diagnosisName,
                            TotalExpense = totalExpense,
                            MedicineCount = medicineCount,
                            AvgMedicineCost = avgMedicineCost,
                            MinMedicineCost = minMedicineCost,
                            MaxMedicineCost = maxMedicineCost,
                            MedicinesList = medicinesList,
                            ExpenseCategory = expenseCategory,
                            TotalDiagnoses = totalDiagnoses,
                            NoMedicineDiagnoses = noMedicineDiagnoses
                        });
                    }
                    catch
                    {
                        // Игнорируем строки с неправильным форматом
                        // Возможно, стоит логировать эти случаи
                    }
                }
            }

            return results;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}