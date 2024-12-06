using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class NavstevaDoctorViewRepository : INavstevaDoctorViewRepository
    {
        private readonly string _connectionString;

        public NavstevaDoctorViewRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<NAVSTEVA_DOCTOR_VIEW>> GetNavstevaDoctorViewByPatientId(int pacientId)
        {
            var query = @"
                SELECT 
                    NAVSTEVA_ID AS NavstevaId,
                    VISIT_DATE AS VisitDate,
                    DOCTOR_FULL_NAME AS DoctorFullName,
                    NAVSTEVA_STATUS AS NavstevaStatus,
                    PACIENT_ID AS PacientId
                FROM NAVSTEVA_DOCTOR_VIEW
                WHERE PACIENT_ID = :pacientId";

            var parameters = new { pacientId };

            using (var connection = new OracleConnection(_connectionString))
            {
                try
                {
                    var result = await connection.QueryAsync<NAVSTEVA_DOCTOR_VIEW>(query, parameters);
                    Console.WriteLine($"Found {result.AsList().Count} appointments for patient {pacientId}");
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching appointments: {ex.Message}");
                    return Enumerable.Empty<NAVSTEVA_DOCTOR_VIEW>();
                }
            }
        }
    }
}