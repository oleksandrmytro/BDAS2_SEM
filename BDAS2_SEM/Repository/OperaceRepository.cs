using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class OperaceRepository : IOperaceRepository
    {
        private readonly string _connectionString;

        public OperaceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddOperace(OPERACE operace)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_nazev", operace.Nazev, DbType.String, ParameterDirection.Input);
                parameters.Add("p_datum", operace.Datum, DbType.Date, ParameterDirection.Input);
                parameters.Add("p_diagnoza_id", operace.DiagnozaId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_id_operace", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("manage_operace", parameters, commandType: CommandType.StoredProcedure);

                // Retrieve the newly generated ID_OPERACE
                int newOperaceId = parameters.Get<int>("p_id_operace");
                return newOperaceId;
            }
        }

        public async Task UpdateOperace(OPERACE operace)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_operace", operace.IdOperace, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_nazev", operace.Nazev, DbType.String, ParameterDirection.Input);
                parameters.Add("p_datum", operace.Datum, DbType.Date, ParameterDirection.Input);
                parameters.Add("p_diagnoza_id", operace.DiagnozaId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync("manage_operace", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<OPERACE> GetOperaceById(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        ID_OPERACE AS IdOperace, 
                        NAZEV AS Nazev, 
                        DATUM AS Datum, 
                        DIAGNOZA_ID_DIAGNOZA AS DiagnozaIdDiagnoza 
                    FROM OPERACE 
                    WHERE ID_OPERACE = :id";

                return await db.QueryFirstOrDefaultAsync<OPERACE>(query, new { id });
            }
        }

        public async Task<IEnumerable<OPERACE>> GetAllOperace()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var query = @"
                    SELECT 
                        ID_OPERACE AS IdOperace, 
                        NAZEV AS Nazev, 
                        DATUM AS Datum, 
                        DIAGNOZA_ID_DIAGNOZA AS DiagnozaIdDiagnoza 
                    FROM OPERACE";

                return await db.QueryAsync<OPERACE>(query);
            }
        }

        public async Task DeleteOperace(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_operace", id, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync("DELETE_OPERACE", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}