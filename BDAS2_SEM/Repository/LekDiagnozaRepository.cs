using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

public class LekDiagnozaFKRepository : ILekDiagnozaFKRepository
{
    private readonly string connectionString;

    public LekDiagnozaFKRepository(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task AddLekDiagnoza(LEK_DIAGNOZA_FK lekDiagnoza)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    INSERT INTO LEK_DIAGNOZA_FK (LEK_ID, DIAGNOZA_ID) 
                    VALUES (:LekId, :DiagnozaId)";

            var parameters = new DynamicParameters();
            parameters.Add("LekId", lekDiagnoza.LekId, DbType.Int32);
            parameters.Add("DiagnozaId", lekDiagnoza.DiagnozaId, DbType.Int32);

            await db.ExecuteAsync(sql, parameters);
        }
    }

    public async Task UpdateLekDiagnoza(LEK_DIAGNOZA_FK lekDiagnoza)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    UPDATE LEK_DIAGNOZA_FK 
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

    public async Task<LEK_DIAGNOZA_FK> GetLekDiagnoza(int diagnozaId, int lekId)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    SELECT LEK_ID AS LekId, 
                           DIAGNOZA_ID AS DiagnozaId 
                    FROM LEK_DIAGNOZA_FK 
                    WHERE DIAGNOZA_ID = :DiagnozaId AND LEK_ID = :LekId";

            return await db.QueryFirstOrDefaultAsync<LEK_DIAGNOZA_FK>(sql, new { DiagnozaId = diagnozaId, LekId = lekId });
        }
    }

    public async Task<IEnumerable<LEK_DIAGNOZA_FK>> GetAllLekDiagnoza()
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    SELECT LEK_ID AS LekId, 
                           DIAGNOZA_ID AS DiagnozaId 
                    FROM LEK_DIAGNOZA_FK";

            return await db.QueryAsync<LEK_DIAGNOZA_FK>(sql);
        }
    }

    public async Task DeleteLekDiagnoza(int diagnozaId, int lekId)
    {
        using (var db = new OracleConnection(this.connectionString))
        {
            string sql = @"
                    DELETE FROM LEK_DIAGNOZA_FK 
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