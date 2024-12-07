using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace BDAS2_SEM.Repository;

public class PDiagnosesRepository : IPDiagnosesRepository
{
    private readonly string _connectionString;

    public PDiagnosesRepository(string connectionString)
    {
        _connectionString = connectionString;
    }


    public async Task<IEnumerable<PDiagnosesDetail>> GetPDiagnosesByPatientIdAsync(int pacientId)
    {
        var query = @"
                SELECT 
                    ID_NAVSTEVA AS IdNavsteva,
                    DATUM AS Datum,
                    PACIENT_ID_PACIENT AS PacientIdPacient,
                    STATUS_ID_STATUS AS StatusIdStatus,
                    MISTNOST_ID AS MistnostId,
                    DiagnosisName,
                    DiagnosisDescription,
                    PrescribedMedications,
                    TotalCost
                FROM 
                    PDiagnosesView
                WHERE 
                    PACIENT_ID_PACIENT = :pacientId";

        var parameters = new { pacientId };

        using (var connection = new OracleConnection(_connectionString))
        {
            try
            {
                var result = await connection.QueryAsync<PDiagnosesDetail>(query, parameters);
                Console.WriteLine($"Found {result.AsList().Count} diagnoses for patient {pacientId}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching diagnoses: {ex.Message}");
                return Enumerable.Empty<PDiagnosesDetail>();
            }
        }
    }
}