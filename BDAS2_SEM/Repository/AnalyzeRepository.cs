// Repository/AnalyzeRepository.cs
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class AnalyzeRepository : IAnalyzeRepository
    {
        private readonly string _connectionString;

        public AnalyzeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> AnalyzeDiagnosesAndMedicines(DateTime startDate, DateTime endDate)
        {
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("analyze_diagnoses_and_medicines", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Включаем привязку по имени
                    command.BindByName = true;

                    // Добавление входных параметров
                    command.Parameters.Add("p_start_date", OracleDbType.Date).Value = startDate;
                    command.Parameters.Add("p_end_date", OracleDbType.Date).Value = endDate;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("p_results", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        // Используем ExecuteReaderAsync для выполнения процедуры и получения курсора
                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            var analysisList = new List<DiagnosisAnalysis>();

                            while (await reader.ReadAsync())
                            {
                                var diagnosis = new DiagnosisAnalysis
                                {
                                    DiagnosisId = reader.IsDBNull(reader.GetOrdinal("diagnosis_id")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("diagnosis_id")),
                                    DiagnosisName = reader.IsDBNull(reader.GetOrdinal("diagnosis_name")) ? "N/A" : reader.GetString(reader.GetOrdinal("diagnosis_name")),
                                    VisitCount = reader.IsDBNull(reader.GetOrdinal("visit_count")) ? 0 : reader.GetInt32(reader.GetOrdinal("visit_count")),
                                    Medicines = reader.IsDBNull(reader.GetOrdinal("medicines")) ? "No medicines" : reader.GetString(reader.GetOrdinal("medicines"))
                                };
                                analysisList.Add(diagnosis);
                            }

                            // Формирование результата
                            if (analysisList.Count > 0)
                            {
                                // Проверяем, есть ли первая запись с информацией о периоде
                                var firstRecord = analysisList[0];
                                if (firstRecord.DiagnosisId == null && firstRecord.DiagnosisName == "Analysis Period")
                                {
                                    resultBuilder.AppendLine(firstRecord.Medicines); // 'From YYYY-MM-DD to YYYY-MM-DD'
                                    analysisList.RemoveAt(0);
                                }
                                else
                                {
                                    resultBuilder.AppendLine("Diagnosis and Medicines Analysis:");
                                    resultBuilder.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                                }

                                foreach (var diagnosis in analysisList)
                                {
                                    resultBuilder.AppendLine($"Diagnosis ID: {diagnosis.DiagnosisId}, Name: {diagnosis.DiagnosisName}, Visits: {diagnosis.VisitCount}, Medicines: {diagnosis.Medicines}");
                                }

                                resultBuilder.AppendLine("Analysis completed.");
                            }
                            else
                            {
                                resultBuilder.AppendLine("Diagnosis and Medicines Analysis:");
                                resultBuilder.AppendLine($"Period: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
                                resultBuilder.AppendLine("No data found for the given period.");
                                resultBuilder.AppendLine("Analysis completed.");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        public async Task<string> AnalyzeEmployeeHierarchy(int managerId)
        {
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("analyze_employee_hierarchy", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление входных параметров
                    command.Parameters.Add("p_manager_id", OracleDbType.Int32).Value = managerId;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("p_result", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            while (await reader.ReadAsync())
                            {
                                int hierarchyLevel = reader.IsDBNull(reader.GetOrdinal("hierarchy_level"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("hierarchy_level"));

                                int employeeId = reader.IsDBNull(reader.GetOrdinal("employee_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("employee_id"));

                                string firstName = reader.IsDBNull(reader.GetOrdinal("first_name"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("first_name"));

                                string lastName = reader.IsDBNull(reader.GetOrdinal("last_name"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("last_name"));

                                string phone = reader.IsDBNull(reader.GetOrdinal("phone"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("phone"));

                                int rootManagerId = reader.IsDBNull(reader.GetOrdinal("root_manager_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("root_manager_id"));

                                int managerIdValue = reader.IsDBNull(reader.GetOrdinal("manager_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("manager_id"));

                                resultBuilder.AppendLine($"Level: {hierarchyLevel}, Employee ID: {employeeId}, Name: {firstName} {lastName}, Phone: {phone}, Root Manager ID: {rootManagerId}, Manager ID: {managerIdValue}");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        public async Task<string> AnalyzeIncomeByPaymentType(DateTime startDate, DateTime endDate)
        {
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("analyze_income_by_payment_type", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление входных параметров
                    command.Parameters.Add("p_start_date", OracleDbType.Date).Value = startDate;
                    command.Parameters.Add("p_end_date", OracleDbType.Date).Value = endDate;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("p_results", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            while (await reader.ReadAsync())
                            {
                                string paymentType = reader.IsDBNull(reader.GetOrdinal("payment_type"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("payment_type"));

                                decimal totalIncome = reader.IsDBNull(reader.GetOrdinal("total_income"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("total_income"));

                                decimal avgIncome = reader.IsDBNull(reader.GetOrdinal("avg_income"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("avg_income"));

                                decimal minPayment = reader.IsDBNull(reader.GetOrdinal("min_payment"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("min_payment"));

                                decimal maxPayment = reader.IsDBNull(reader.GetOrdinal("max_payment"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("max_payment"));

                                string period = reader.IsDBNull(reader.GetOrdinal("period"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("period"));

                                resultBuilder.AppendLine($"Payment Type: {paymentType}, Total Income: {totalIncome}, Avg Income: {avgIncome}, Min Payment: {minPayment}, Max Payment: {maxPayment}, Period: {period}");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        public async Task<string> AnalyzeMedicineExpenses()
        {
            // Реализация метода для процедуры ANALYZE_MEDICINE_EXPENSES
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("analyze_medicine_expenses", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("p_result", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        using (var reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            while (await reader.ReadAsync())
                            {
                                int diagnosisId = reader.IsDBNull(reader.GetOrdinal("diagnosis_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("diagnosis_id"));

                                string diagnosisName = reader.IsDBNull(reader.GetOrdinal("diagnosis_name"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("diagnosis_name"));

                                decimal totalExpense = reader.IsDBNull(reader.GetOrdinal("total_expense"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("total_expense"));

                                int medicineCount = reader.IsDBNull(reader.GetOrdinal("medicine_count"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("medicine_count"));

                                decimal avgMedicineCost = reader.IsDBNull(reader.GetOrdinal("avg_medicine_cost"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("avg_medicine_cost"));

                                decimal minMedicineCost = reader.IsDBNull(reader.GetOrdinal("min_medicine_cost"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("min_medicine_cost"));

                                decimal maxMedicineCost = reader.IsDBNull(reader.GetOrdinal("max_medicine_cost"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("max_medicine_cost"));

                                string medicinesList = reader.IsDBNull(reader.GetOrdinal("medicines_list"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("medicines_list"));

                                string expenseCategory = reader.IsDBNull(reader.GetOrdinal("expense_category"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("expense_category"));

                                int totalDiagnoses = reader.IsDBNull(reader.GetOrdinal("total_diagnoses"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("total_diagnoses"));

                                int noMedicineDiagnoses = reader.IsDBNull(reader.GetOrdinal("no_medicine_diagnoses"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("no_medicine_diagnoses"));

                                resultBuilder.AppendLine($"Diagnosis ID: {diagnosisId}, Name: {diagnosisName}, Total Expense: {totalExpense}, Medicine Count: {medicineCount}, Avg Medicine Cost: {avgMedicineCost}, Min Medicine Cost: {minMedicineCost}, Max Medicine Cost: {maxMedicineCost}, Medicines List: {medicinesList}, Expense Category: {expenseCategory}, Total Diagnoses: {totalDiagnoses}, Diagnoses without Medicines: {noMedicineDiagnoses}");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        public async Task<DoctorActivityResult> AnalyzeDoctorActivityAsync(int doctorId)
        {
            StringBuilder resultBuilder = new StringBuilder();
            DoctorActivityResult activityResult = null;

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("analyze_doctor_activity_fn", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление входного параметра
                    command.Parameters.Add("p_doctor_id", OracleDbType.Int32).Value = doctorId;

                    // Параметр RETURN_VALUE для функции
                    var refCursor = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        // Выполнение функции
                        await command.ExecuteNonQueryAsync();

                        // Получение курсора
                        using (var reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                        {
                            if (await reader.ReadAsync())
                            {
                                activityResult = new DoctorActivityResult
                                {
                                    TotalVisits = reader["total_visits"] != DBNull.Value ? Convert.ToInt32(reader["total_visits"]) : 0,
                                    LastVisitDate = reader["last_visit_date"] != DBNull.Value ? Convert.ToDateTime(reader["last_visit_date"]) : (DateTime?)null,
                                    TotalOperations = reader["total_operations"] != DBNull.Value ? Convert.ToInt32(reader["total_operations"]) : 0,
                                    LastOperationDate = reader["last_operation_date"] != DBNull.Value ? Convert.ToDateTime(reader["last_operation_date"]) : (DateTime?)null,
                                    TotalMedicines = reader["total_medicines"] != DBNull.Value ? Convert.ToInt32(reader["total_medicines"]) : 0
                                };
                            }
                            else
                            {
                                // Если данных нет
                                return null;
                            }
                        }

                        // Дополнительно вывод в консоль
                        Console.WriteLine(resultBuilder.ToString());
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return activityResult;
        }

        public async Task<string> AnalyzeEmployeeEfficiency(int employeeId)
        {
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("Analyze_Employee_Efficiency", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление входного параметра
                    command.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = employeeId;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        // Выполнение функции
                        await command.ExecuteNonQueryAsync();

                        // Получение курсора
                        using (var reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                int employeeIdValue = reader.IsDBNull(reader.GetOrdinal("employee_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("employee_id"));

                                string employeeName = reader.IsDBNull(reader.GetOrdinal("employee_name"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("employee_name"));

                                int totalVisits = reader.IsDBNull(reader.GetOrdinal("total_visits"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("total_visits"));

                                int totalOperations = reader.IsDBNull(reader.GetOrdinal("total_operations"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("total_operations"));

                                decimal avgInteractionDays = reader.IsDBNull(reader.GetOrdinal("avg_interaction_days"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("avg_interaction_days"));

                                resultBuilder.AppendLine($"Employee ID: {employeeIdValue}, Name: {employeeName}, Total Visits: {totalVisits}, Total Operations: {totalOperations}, Avg Interaction Days: {avgInteractionDays}");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        public async Task<string> AnalyzeTopEmployeesEfficiency()
        {
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("Analyze_Top_Employees_Efficiency", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        // Выполнение функции
                        await command.ExecuteNonQueryAsync();

                        // Получение курсора
                        using (var reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                int employeeId = reader.IsDBNull(reader.GetOrdinal("employee_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("employee_id"));

                                string employeeName = reader.IsDBNull(reader.GetOrdinal("employee_name"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("employee_name"));

                                int totalVisits = reader.IsDBNull(reader.GetOrdinal("total_visits"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("total_visits"));

                                int totalOperations = reader.IsDBNull(reader.GetOrdinal("total_operations"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("total_operations"));

                                int rank = reader.IsDBNull(reader.GetOrdinal("rank"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("rank"));

                                resultBuilder.AppendLine($"Rank: {rank}, Employee ID: {employeeId}, Name: {employeeName}, Total Visits: {totalVisits}, Total Operations: {totalOperations}");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        public async Task<string> AnalyzeTopPayingPatients(DateTime startDate, DateTime endDate, int topCount)
        {
            StringBuilder resultBuilder = new StringBuilder();

            using (var connection = new OracleConnection(_connectionString))
            {
                using (var command = new OracleCommand("Analyze_Top_Paying_Patients", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.BindByName = true;

                    // Добавление входных параметров
                    command.Parameters.Add("p_start_date", OracleDbType.Date).Value = startDate;
                    command.Parameters.Add("p_end_date", OracleDbType.Date).Value = endDate;
                    command.Parameters.Add("p_top_count", OracleDbType.Int32).Value = topCount;

                    // Добавление выходного параметра - SYS_REFCURSOR
                    var refCursor = new OracleParameter("RETURN_VALUE", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.ReturnValue
                    };
                    command.Parameters.Add(refCursor);

                    try
                    {
                        await connection.OpenAsync();

                        // Выполнение функции
                        await command.ExecuteNonQueryAsync();

                        // Получение курсора
                        using (var reader = ((OracleRefCursor)refCursor.Value).GetDataReader())
                        {
                            while (await reader.ReadAsync())
                            {
                                int patientId = reader.IsDBNull(reader.GetOrdinal("patient_id"))
                                    ? 0
                                    : reader.GetInt32(reader.GetOrdinal("patient_id"));

                                string patientName = reader.IsDBNull(reader.GetOrdinal("patient_name"))
                                    ? "N/A"
                                    : reader.GetString(reader.GetOrdinal("patient_name"));

                                decimal totalPaid = reader.IsDBNull(reader.GetOrdinal("total_paid"))
                                    ? 0
                                    : reader.GetDecimal(reader.GetOrdinal("total_paid"));

                                resultBuilder.AppendLine($"Patient ID: {patientId}, Name: {patientName}, Total Paid: {totalPaid}");
                            }

                            // Дополнительно вывод в консоль
                            Console.WriteLine(resultBuilder.ToString());
                        }
                    }
                    catch (OracleException ex)
                    {
                        var errorMessage = $"Database error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                    catch (Exception ex)
                    {
                        var errorMessage = $"An error occurred: {ex.Message}";
                        resultBuilder.AppendLine(errorMessage);
                        Console.WriteLine(errorMessage);
                    }
                }
            }

            return resultBuilder.ToString();
        }

        // Класс для маппинга данных диагностики
        private class DiagnosisAnalysis
        {
            public int? DiagnosisId { get; set; }
            public string DiagnosisName { get; set; }
            public int VisitCount { get; set; }
            public string Medicines { get; set; }
        }

        
    }
}