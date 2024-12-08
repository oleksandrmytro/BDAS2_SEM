using BDAS2_SEM.Model;
using System;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IAnalyzeRepository
    {
        Task<string> AnalyzeDiagnosesAndMedicines(DateTime startDate, DateTime endDate);
        Task<string> AnalyzeEmployeeHierarchy(int managerId);
        Task<string> AnalyzeIncomeByPaymentType(DateTime startDate, DateTime endDate);
        Task<string> AnalyzeMedicineExpenses();
        Task<DoctorActivityResult> AnalyzeDoctorActivityAsync(int doctorId);
    }
}