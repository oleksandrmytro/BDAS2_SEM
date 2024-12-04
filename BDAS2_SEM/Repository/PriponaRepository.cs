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
                string sql = @"
                    INSERT INTO PRIPONA (ID_PRIPONA, TYP)
                    VALUES (PRIPONA_SEQ.NEXTVAL, :Typ)
                    RETURNING ID_PRIPONA INTO :IdPripona";

                var parameters = new DynamicParameters();
                parameters.Add("Typ", pripona.Typ, DbType.String);
                parameters.Add("IdPripona", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);
                return parameters.Get<int>("IdPripona");
            }
        }

        public async Task UpdatePripona(PRIPONA pripona)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    UPDATE PRIPONA
                    SET TYP = :Typ
                    WHERE ID_PRIPONA = :IdPripona";

                var parameters = new DynamicParameters();
                parameters.Add("Typ", pripona.Typ, DbType.String);
                parameters.Add("IdPripona", pripona.IdPripona, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
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