using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class NavstevaDiagnozaRepository : INavstevaDiagnozaRepository
    {
        private readonly string connectionString;

        public NavstevaDiagnozaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task AddNavstevaDiagnoza(NAVSTEVA_DIAGNOZA navstevaDiagnoza)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO NAVSTEVA_DIAGNOZA (NAVSTEVA_ID_NAVSTEVA, DIAGNOZA_ID_DIAGNOZA) 
                    VALUES (:NavstevaId, :DiagnozaId)";

                var parameters = new DynamicParameters();
                parameters.Add("NavstevaId", navstevaDiagnoza.NavstevaId, DbType.Int32);
                parameters.Add("DiagnozaId", navstevaDiagnoza.DiagnozaId, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<IEnumerable<DIAGNOZA>> GetDiagnozyByNavstevaIdAsync(int navstevaId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT d.ID_DIAGNOZA, d.NAZEV, d.POPIS
                    FROM DIAGNOZA d
                    JOIN NAVSTEVA_DIAGNOZA nd ON d.ID_DIAGNOZA = nd.DIAGNOZA_ID_DIAGNOZA
                    WHERE nd.NAVSTEVA_ID_NAVSTEVA = :NavstevaId";

                return await db.QueryAsync<DIAGNOZA>(sql, new { NavstevaId = navstevaId });
            }
        }

        public async Task<IEnumerable<NAVSTEVA_DIAGNOZA>> GetAllNavstevaDiagnozas()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT NAVSTEVA_ID_NAVSTEVA AS NavstevaId,
                           DIAGNOZA_ID_DIAGNOZA AS DiagnozaId
                    FROM NAVSTEVA_DIAGNOZA";

                return await db.QueryAsync<NAVSTEVA_DIAGNOZA>(sqlQuery);
            }
        }

        public async Task DeleteNavstevaDiagnoza(int navstevaId, int diagnozaId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM NAVSTEVA_DIAGNOZA 
                    WHERE NAVSTEVA_ID = :NavstevaId AND DIAGNOZA_ID = :DiagnozaId";

                var parameters = new
                {
                    NavstevaId = navstevaId,
                    DiagnozaId = diagnozaId
                };

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateNavstevaDiagnoza(NAVSTEVA_DIAGNOZA navstevaDiagnoza)
        {
                using (var db = new OracleConnection(connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                    parameters.Add("p_id_navsteva", navstevaDiagnoza.NavstevaId, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("p_id_diagnoza", navstevaDiagnoza.DiagnozaId, DbType.String, ParameterDirection.Input);

                    await db.ExecuteAsync(
                        "manage_navsteva_diagnoza",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                }
        }
    }
}
