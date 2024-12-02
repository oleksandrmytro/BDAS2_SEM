using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class LekDiagnozaRepository : ILekDiagnozaRepository
    {
        private readonly string _connectionString;

        public LekDiagnozaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Метод для додавання зв'язку між ліком та діагнозом
        public async Task AddLekDiagnoza(LEK_DIAGNOZA lekDiagnoza)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_diagnoza", lekDiagnoza.DiagnozaId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_id_lek", lekDiagnoza.LekId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync(
                    "manage_lek_diagnoza_fk",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        // Метод для оновлення зв'язку між ліком та діагнозом
        public async Task UpdateLekDiagnoza(LEK_DIAGNOZA lekDiagnoza)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_diagnoza", lekDiagnoza.DiagnozaId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_id_lek", lekDiagnoza.LekId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync(
                    "manage_lek_diagnoza_fk",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        // Метод для отримання конкретного зв'язку між ліком та діагнозом
        public async Task<LEK_DIAGNOZA> GetLekDiagnoza(int diagnozaId, int lekId)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT LEK_ID AS LekId, 
                           DIAGNOZA_ID AS DiagnozaId 
                    FROM LEK_DIAGNOZA 
                    WHERE DIAGNOZA_ID = :DiagnozaId AND LEK_ID = :LekId";

                return await db.QueryFirstOrDefaultAsync<LEK_DIAGNOZA>(sql, new { DiagnozaId = diagnozaId, LekId = lekId });
            }
        }

        // Метод для отримання всіх зв'язків між ліками та діагнозами
        public async Task<IEnumerable<LEK_DIAGNOZA>> GetAllLekDiagnoza()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var sqlQuery = @"
                    SELECT LEK_ID_LEK AS LekId,
                           DIAGNOZA_ID_DIAGNOZA AS DiagnozaId
                    FROM LEK_DIAGNOZA";

                return await db.QueryAsync<LEK_DIAGNOZA>(sqlQuery);
            }
        }

        // Метод для видалення зв'язку між ліком та діагнозом
        public async Task DeleteLekDiagnoza(int diagnozaId, int lekId)
        {
            using (var db = new OracleConnection(_connectionString))
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
}