using BDAS2_SEM.Model;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Repository.Interfaces;

namespace BDAS2_SEM.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string connectionString;

        public RoleRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddRole(ROLE role)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_role";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String);
                parameters.Add("p_id_role", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", role.Nazev, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_role");
            }
        }

        public async Task UpdateRole(ROLE role)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_role";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String);
                parameters.Add("p_id_role", role.IdRole, DbType.Int32, direction: ParameterDirection.Input);
                parameters.Add("p_nazev", role.Nazev, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public Task<ROLE> GetRoleById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ROLE>> GetAllRoles()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT ID_ROLE AS IdRole,
                           nazev AS Nazev
                    FROM Role";

                return await db.QueryAsync<ROLE>(sqlQuery);
            }
        }

        public async Task DeleteRole(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM ROLE WHERE ID_ROLE = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
