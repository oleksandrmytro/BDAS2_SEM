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
                string sql = @"
                    INSERT INTO ORDINACE (NAZEV) 
                    VALUES (:Nazev) 
                    RETURNING ID_ORDINACE INTO :IdOrdinace";

                var parameters = new DynamicParameters();
                parameters.Add("Nazev", ordinace.Nazev, DbType.String);
                parameters.Add("IdOrdinace", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdOrdinace");
            }
        }

        public async Task UpdateOrdinace(ORDINACE ordinace)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE ORDINACE 
                    SET NAZEV = :Nazev 
                    WHERE ID_ORDINACE = :IdOrdinace";

                var parameters = new DynamicParameters();
                parameters.Add("Nazev", ordinace.Nazev, DbType.String);
                parameters.Add("IdOrdinace", ordinace.IdOrdinace, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
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
