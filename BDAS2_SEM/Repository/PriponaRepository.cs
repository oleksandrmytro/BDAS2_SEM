using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BDAS2_SEM.Repository
{
    public class PriponaRepository : IPriponaRepository
    {
        private readonly string _connectionString;

        public PriponaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddPripona(PRIPONA pripona)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var procedureName = "MANAGE_PRIPONA";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_pripona", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_typ", pripona.Typ, DbType.String, ParameterDirection.Input);

                await db.OpenAsync();
                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_pripona");
            }
        }

        public async Task UpdatePripona(PRIPONA pripona)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var procedureName = "MANAGE_PRIPONA";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_pripona", pripona.IdPripona, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_typ", pripona.Typ, DbType.String, ParameterDirection.Input);

                await db.OpenAsync();
                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<PRIPONA> GetPriponaById(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_PRIPONA AS IdPripona,
                           TYP AS Typ
                    FROM PRIPONA
                    WHERE ID_PRIPONA = :IdPripona";

                return await db.QueryFirstOrDefaultAsync<PRIPONA>(sql, new { IdPripona = id });
            }
        }

        public async Task<PRIPONA> GetPriponaByTyp(string typ)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
            SELECT ID_PRIPONA AS IdPripona,
                   TYP AS Typ
            FROM PRIPONA
            WHERE TYP = :Typ";

                return await db.QueryFirstOrDefaultAsync<PRIPONA>(sql, new { Typ = typ });
            }
        }

        public async Task<IEnumerable<PRIPONA>> GetAllPriponas()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_PRIPONA AS IdPripona,
                           TYP AS Typ
                    FROM PRIPONA";

                return await db.QueryAsync<PRIPONA>(sql);
            }
        }

        public async Task DeletePripona(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = "DELETE FROM PRIPONA WHERE ID_PRIPONA = :IdPripona";
                await db.ExecuteAsync(sql, new { IdPripona = id });
            }
        }
    }
}