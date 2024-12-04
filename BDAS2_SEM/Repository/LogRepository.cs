using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace BDAS2_SEM.Repository
{
    public class LogRepository : ILogRepository
    {

        private readonly string connectionString;

        public LogRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddLog(LOG log)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_log";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String);
                parameters.Add("p_id_log", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_table_name", log.TableName, DbType.String);
                parameters.Add("p_operation_typr", log.OperationType, DbType.String);
                parameters.Add("p_operation_data", log.OperationData, DbType.String);
                parameters.Add("p_old_values", log.OldValues, DbType.String);
                parameters.Add("p_new_values", log.NewValues, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_status");
            }
        }

        public async Task UpdateLog(LOG log)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_log", log.IdLog, DbType.Int32);
                parameters.Add("p_table_name", log.TableName, DbType.Date);
                parameters.Add("p_operation_typr", log.OperationType, DbType.Int32);
                parameters.Add("p_operation_data", log.OperationData, DbType.Int32);
                parameters.Add("p_old_values", log.OldValues, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_new_values", log.NewValues, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync("manage_log", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<LOG>> GetAllLogs()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                SELECT id_log AS IdLog,
                    table_name AS TableName,
                    operation_typr as OperationType,
                    operation_data as OperationData,
                    old_values as OldValues,
                    new_values as NewValues
                FROM LOG_TABLE";

                return await db.QueryAsync<LOG>(sqlQuery);
            }
        }

        public async Task DeleteLog(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM LOG_TABLE WHERE ID_LOG = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
