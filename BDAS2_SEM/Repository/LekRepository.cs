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
    public class LekRepository : ILekRepository
    {
        private readonly string connectionString;

        public LekRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddLek(LEK lek)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var procedureName = "manage_lek";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_lek", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", lek.Nazev, DbType.String, ParameterDirection.Input);
                parameters.Add("p_mnozstvi", lek.Mnozstvi, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_cena", lek.Cena, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_typ_lek_id", lek.TypLekId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                return parameters.Get<int>("p_id_lek");
            }
        }

        public async Task UpdateLek(LEK lek)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var procedureName = "manage_lek";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_lek", lek.IdLek, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_nazev", lek.Nazev, DbType.String, ParameterDirection.Input);
                parameters.Add("p_mnozstvi", lek.Mnozstvi, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_cena", lek.Cena, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_typ_lek_id", lek.TypLekId, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }


        public async Task<LEK> GetLekById(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    SELECT ID_LEK AS IdLek, 
                           NAZEV AS Nazev, 
                           MNOZSTVI AS Mnozstvi, 
                           CENA AS Cena, 
                           TYP_LEK_ID AS TypLekId 
                    FROM LEK 
                    WHERE ID_LEK = :Id";

                return await db.QueryFirstOrDefaultAsync<LEK>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<LEK>> GetAllLeks()
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                var sqlQuery = @"
                    SELECT id_lek AS IdLek,
                           nazev AS Nazev,
                           mnozstvi AS Mnozstvi,
                           cena AS Cena,
                           TYP_LEK_ID_TYP_LEK AS TypLekId
                    FROM LEK";

                return await db.QueryAsync<LEK>(sqlQuery);
            }
        }

        public async Task DeleteLek(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = "DELETE FROM LEK WHERE ID_LEK = :Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
