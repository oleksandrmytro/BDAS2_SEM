using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using BDAS2_SEM.Repository.Interfaces;

namespace BDAS2_SEM.Repository
{
    public class StatusRepository : IStatusRepository
    {
        private readonly string connectionString;

        public StatusRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddStatus(STATUS status)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_status";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String);
                parameters.Add("p_id_status", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", status.Nazev, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_status");
            }
        }

        public async Task UpdateStatus(STATUS status)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_status";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String);
                parameters.Add("p_id_status", status.IdStatus, DbType.Int32, direction: ParameterDirection.Input);
                parameters.Add("p_nazev", status.Nazev, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public Task<STATUS> GetStatusById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<STATUS>> GetAllStatuses()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT ID_STATUS AS IdStatus,
                           nazev AS Nazev
                    FROM Status";

                return await db.QueryAsync<STATUS>(sqlQuery);
            }
        }

        public async Task DeleteStatus(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM STATUS WHERE ID_STATUS = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
