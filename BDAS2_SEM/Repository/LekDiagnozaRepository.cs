using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

public class LekDiagnozaRepository : ILekDiagnozaRepository
{
    private readonly string connectionString;

    public LekDiagnozaRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task AddLekDiagnoza(LEK_DIAGNOZA lekDiagnoza)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    INSERT INTO LEK_DIAGNOZA (LEK_ID, DIAGNOZA_ID) 
                    VALUES (:LekId, :DiagnozaId)";

            var parameters = new DynamicParameters();
            parameters.Add("LekId", lekDiagnoza.LekId, DbType.Int32);
            parameters.Add("DiagnozaId", lekDiagnoza.DiagnozaId, DbType.Int32);

            await db.ExecuteAsync(sql, parameters);
        }
    }

    public async Task UpdateLekDiagnoza(LEK_DIAGNOZA lekDiagnoza)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    UPDATE LEK_DIAGNOZA 
                    SET LEK_ID = :LekId, DIAGNOZA_ID = :DiagnozaId 
                    WHERE LEK_ID = :OldLekId AND DIAGNOZA_ID = :OldDiagnozaId";

            var parameters = new DynamicParameters();
            parameters.Add("LekId", lekDiagnoza.LekId, DbType.Int32);
            parameters.Add("DiagnozaId", lekDiagnoza.DiagnozaId, DbType.Int32);
            parameters.Add("OldLekId", lekDiagnoza.LekId, DbType.Int32); // Adjust if you have original values
            parameters.Add("OldDiagnozaId", lekDiagnoza.DiagnozaId, DbType.Int32); // Adjust if you have original values

            await db.ExecuteAsync(sql, parameters);
        }
    }

    public async Task<LEK_DIAGNOZA> GetLekDiagnoza(int diagnozaId, int lekId)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    SELECT LEK_ID AS LekId, 
                           DIAGNOZA_ID AS DiagnozaId 
                    FROM LEK_DIAGNOZA 
                    WHERE DIAGNOZA_ID = :DiagnozaId AND LEK_ID = :LekId";

            return await db.QueryFirstOrDefaultAsync<LEK_DIAGNOZA>(sql, new { DiagnozaId = diagnozaId, LekId = lekId });
        }
    }

    public async Task<IEnumerable<LEK_DIAGNOZA>> GetAllLekDiagnoza()
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    SELECT LEK_ID AS LekId, 
                           DIAGNOZA_ID AS DiagnozaId 
                    FROM LEK_DIAGNOZA";

            return await db.QueryAsync<LEK_DIAGNOZA>(sql);
        }
    }

    public async Task DeleteLekDiagnoza(int diagnozaId, int lekId)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    DELETE FROM LEK_DIAGNOZA 
                    WHERE DIAGNOZA_ID = :DiagnozaId AND LEK_ID = :LekId";

            var parameters = new
            {
                DiagnozaId = diagnozaId,
                LekId = lekId
            };

            await db.ExecuteAsync(sql, parameters);
        }
    }
}