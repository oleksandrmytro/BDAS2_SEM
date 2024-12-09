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
                "ANALYZE_MEDICINE_EXPENSES",
                "ANALYZE_EMPLOYEE_EFFICIENCY", // Новая процедура
                "ANALYZE_TOP_EMPLOYEES_EFFICIENCY", // Новая процедура
                "ANALYZE_TOP_PAYING_PATIENTS", // Новая процедура
                "GROUP_PATIENTS_BY_AGE" // Новая процедура
            };

            ExecuteProcedureCommand = new RelayCommand(async _ => await ExecuteProcedureAsync(), CanExecuteProcedure);

            // Set the initial SelectedProcedure
            SelectedProcedure = Procedures.FirstOrDefault();
        }

        private bool CanExecuteProcedure(object parameter)
        {
            // Add logic here to enable/disable the "Execute" button
            // For example, check if the necessary data for the selected procedure is available
            return true;
        }

        public async Task ExecuteProcedureAsync()
        {
            Result = string.Empty; // Clear previous results
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
                case "ANALYZE_EMPLOYEE_EFFICIENCY": // Новая процедура
                    await ExecuteAnalyzeEmployeeEfficiencyAsync();
                    break;
                case "ANALYZE_TOP_EMPLOYEES_EFFICIENCY": // Новая процедура
                    await ExecuteAnalyzeTopEmployeesEfficiencyAsync();
                    break;
                case "ANALYZE_TOP_PAYING_PATIENTS": // Новая процедура
                    await ExecuteAnalyzeTopPayingPatientsAsync();
                    break;
                case "GROUP_PATIENTS_BY_AGE": // Новая процедура
                    await ExecuteGroupPatientsByAgeAsync();
                    break;
                default:
                    Result = $"Procedure '{SelectedProcedure}' is not implemented.";
                    break;
            }
        }

        private async Task ExecuteAnalyzeDiagnosesAndMedicinesAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                Result = "Please select the start and end dates.";
                return;
            }

            try
            {
                var analysisResult = await _analyzeRepository.AnalyzeDiagnosesAndMedicines(StartDate.Value, EndDate.Value);
                Result = analysisResult;

                // Optional: Output to console
                Console.WriteLine(analysisResult);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeEmployeeHierarchyAsync()
        {
            if (!ManagerId.HasValue || ManagerId <= 0)
            {
                Result = "Please enter a valid manager ID.";
                return;
            }

            try
            {
                var analysisResult = await _analyzeRepository.AnalyzeEmployeeHierarchy(ManagerId.Value);
                Result = analysisResult;

                // Optional: Output to console
                Console.WriteLine(analysisResult);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeIncomeByPaymentTypeAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                Result = "Please select the start and end dates.";
                return;
            }

            try
            {
                var incomeResults = await _analyzeRepository.AnalyzeIncomeByPaymentType(StartDate.Value, EndDate.Value);
                Result = incomeResults;

                // Optional: Output to console
                Console.WriteLine(incomeResults);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeMedicineExpensesAsync()
        {
            try
            {
                var expenseResults = await _analyzeRepository.AnalyzeMedicineExpenses();
                Result = expenseResults;

                // Parse the string result and populate the collection for DataGrid
                MedicineExpenseResults = ParseMedicineExpensesResults(expenseResults);

                // Optional: Output to console
                Console.WriteLine(expenseResults);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private ObservableCollection<MedicineExpenseResult> ParseMedicineExpensesResults(string expenseResults)
        {
            var results = new ObservableCollection<MedicineExpenseResult>();

            var lines = expenseResults.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Example line:
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
                        // Ignore lines with incorrect format
                        // It might be worth logging these cases
                    }
                }
            }

            return results;
        }

        private async Task ExecuteAnalyzeEmployeeEfficiencyAsync()
        {
            if (!ManagerId.HasValue || ManagerId <= 0)
            {
                Result = "Please enter a valid employee ID.";
                return;
            }

            try
            {
                var efficiencyResult = await _analyzeRepository.AnalyzeEmployeeEfficiency(ManagerId.Value);
                Result = efficiencyResult;

                // Optional: Output to console
                Console.WriteLine(efficiencyResult);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeTopEmployeesEfficiencyAsync()
        {
            try
            {
                var efficiencyResult = await _analyzeRepository.AnalyzeTopEmployeesEfficiency();
                Result = efficiencyResult;

                // Optional: Output to console
                Console.WriteLine(efficiencyResult);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteAnalyzeTopPayingPatientsAsync()
        {
            if (!StartDate.HasValue || !EndDate.HasValue)
            {
                Result = "Please select the start and end dates.";
                return;
            }

            try
            {
                var topCount = 5; // Например, топ 5 пациентов
                var topPayingPatientsResult = await _analyzeRepository.AnalyzeTopPayingPatients(StartDate.Value, EndDate.Value, topCount);
                Result = topPayingPatientsResult;

                // Optional: Output to console
                Console.WriteLine(topPayingPatientsResult);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

        private async Task ExecuteGroupPatientsByAgeAsync()
        {
            try
            {
                var groupResult = await _analyzeRepository.GroupPatientsByAge();
                Result = groupResult;

                // Optional: Output to console
                Console.WriteLine(groupResult);
            }
            catch (Exception ex)
            {
                Result = $"Error executing procedure: {ex.Message}";
                Console.WriteLine(Result);
            }
        }

       
    

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}