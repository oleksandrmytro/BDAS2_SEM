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
    public class NavstevaRepository : INavstevaRepository
    {
        private readonly string connectionString;

        public NavstevaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddNavsteva(NAVSTEVA navsteva)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_datum", navsteva.Datum, DbType.Date);
                parameters.Add("p_cas", navsteva.Cas, DbType.DateTime);
                parameters.Add("p_mistnost", navsteva.Mistnost, DbType.Int32);
                parameters.Add("p_pacient_id", navsteva.PacientId, DbType.Int32);
                parameters.Add("p_id_navsteva", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("INSERT_NAVSTEVA", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_navsteva");
            }
        }

        public async Task UpdateNavsteva(NAVSTEVA navsteva)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_navsteva", navsteva.IdNavsteva, DbType.Int32);
                parameters.Add("p_datum", navsteva.Datum, DbType.Date);
                parameters.Add("p_cas", navsteva.Cas, DbType.DateTime);
                parameters.Add("p_mistnost", navsteva.Mistnost, DbType.Int32);
                parameters.Add("p_pacient_id", navsteva.PacientId, DbType.Int32);

                await db.ExecuteAsync("UPDATE_NAVSTEVA", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<NAVSTEVA> GetNavstevaById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = "SELECT * FROM NAVSTEVA WHERE ID_NAVSTEVA = :id";
                return await db.QueryFirstOrDefaultAsync<NAVSTEVA>(query, new { id });
            }
        }

        public async Task<IEnumerable<NAVSTEVA>> GetAllNavstevy()
        {
            using (var db = new OracleConnection(connectionString))
            {
                return await db.QueryAsync<NAVSTEVA>("SELECT * FROM NAVSTEVA");
            }
        }

        public async Task DeleteNavsteva(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_navsteva", id, DbType.Int32);

                await db.ExecuteAsync("DELETE_NAVSTEVA", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
