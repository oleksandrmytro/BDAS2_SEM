using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace BDAS2_SEM.Repository
{
    public class TypLekRepository : ITypLekRepository
    {

        private readonly string connectionString;

        public TypLekRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddTypLek(TYP_LEK typLek)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_typ_lek";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String);
                parameters.Add("p_id_typ_lek", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", typLek.Nazev, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_typ_lek");
            }
        }

        public async Task UpdateTypLek(TYP_LEK typLek)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_typ_lek";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String);
                parameters.Add("p_id_typ_lek", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", typLek.Nazev, DbType.String);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public Task<POZICE> GetTypLekById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TYP_LEK>> GetAllTypLekes()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT id_typ_lek AS IdTypLek,
                           nazev AS Nazev
                    FROM typ_lek";

                return await db.QueryAsync<TYP_LEK>(sqlQuery);
            }
        }

        public async Task DeleteTypLek(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM TYP_LEK WHERE id_typ_lek = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
