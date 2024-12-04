using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace BDAS2_SEM.Repository
{
    public class SystemCatalogRepository : ISystemCatalogRepository
    {
        private readonly string connectionString;

        public SystemCatalogRepository(string connection)
        {
            this.connectionString = connection;
        }

        public async Task<IEnumerable<SystemCatalog>> GetSystemCatalog()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT object_name AS ObjectName,
                           object_type AS ObjectType
                    FROM all_objects WHERE OWNER = 'ST69616'";

                return await db.QueryAsync<SystemCatalog>(sqlQuery);
            }
        }
    }
}
