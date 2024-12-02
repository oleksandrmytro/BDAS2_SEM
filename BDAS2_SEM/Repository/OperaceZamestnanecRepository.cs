using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class OperaceZamestnanecRepository : IOperaceZamestnanecRepository
    {
        private readonly string _connectionString;

        public OperaceZamestnanecRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddOperaceZamestnanec(OPERACE_ZAMESTNANEC operaceZamestnanec)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_operace", operaceZamestnanec.OperaceId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_id_zamestnanec", operaceZamestnanec.ZamestnanecId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync("manage_operace_zamestnanec", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task UpdateOperaceZamestnanec(OPERACE_ZAMESTNANEC operaceZamestnanec)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_operace", operaceZamestnanec.OperaceId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_id_zamestnanec", operaceZamestnanec.ZamestnanecId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync("manage_operace_zamestnanec", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<OPERACE_ZAMESTNANEC> GetOperaceZamestnanec(int operaceId, int zamestnanecId)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        OPERACE_ID_OPERACE AS OperaceIdOperace, 
                        ZAMESTNANEC_ID_ZAMESTNANEC AS ZamestnanecIdZamestnanec 
                    FROM OPERACE_ZAMESTNANEC 
                    WHERE OPERACE_ID_OPERACE = :operaceId AND ZAMESTNANEC_ID_ZAMESTNANEC = :zamestnanecId";

                return await db.QueryFirstOrDefaultAsync<OPERACE_ZAMESTNANEC>(query, new { operaceId, zamestnanecId });
            }
        }

        public async Task<IEnumerable<OPERACE_ZAMESTNANEC>> GetAllOperaceZamestnanecs()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var sqlQuery = @"
                    SELECT operace_id_operace AS OperaceId,
                           zamestnanec_id_zamestnanec AS ZamestnanecId
                    FROM OPERACE_ZAMESTNANEC";

                return await db.QueryAsync<OPERACE_ZAMESTNANEC>(sqlQuery);
            }
        }

        public async Task DeleteOperaceZamestnanec(int operaceId, int zamestnanecId)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "DELETE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_operace", operaceId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_id_zamestnanec", zamestnanecId, DbType.Int32, ParameterDirection.Input);

                // Assuming you have a stored procedure for deletion similar to manage_operace_zamestnanec
                await db.ExecuteAsync("DELETE_OPERACE_ZAMESTNANEC", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}