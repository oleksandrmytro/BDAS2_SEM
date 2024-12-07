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
    public class OrdinaceRepository : IOrdinaceRepository
    {
        private readonly string connectionString;

        public OrdinaceRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddOrdinace(ORDINACE ordinace)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var procedureName = "manage_ordinace";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_ordinace", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", ordinace.Nazev, DbType.String, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                    int newIdOrdinace = parameters.Get<int>("p_id_ordinace");
                    return newIdOrdinace;
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Database error occurred: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred: {ex.Message}", ex);
                }
            }
        }

        public async Task UpdateOrdinace(ORDINACE ordinace)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var procedureName = "manage_ordinace";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_ordinace", ordinace.IdOrdinace, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_nazev", ordinace.Nazev, DbType.String, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Database error occurred: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred: {ex.Message}", ex);
                }
            }
        }


        public async Task<ORDINACE> GetOrdinaceById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_ORDINACE AS IdOrdinace, 
                        NAZEV AS Nazev 
                    FROM ORDINACE 
                    WHERE ID_ORDINACE = :Id";

                return await db.QueryFirstOrDefaultAsync<ORDINACE>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<ORDINACE>> GetAllOrdinaces()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT id_ordinace AS idOrdinace,
                           nazev AS Nazev
                    FROM ORDINACE";

                return await db.QueryAsync<ORDINACE>(sqlQuery);
            }
        }

        public async Task DeleteOrdinace(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM ORDINACE WHERE ID_ORDINACE = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
