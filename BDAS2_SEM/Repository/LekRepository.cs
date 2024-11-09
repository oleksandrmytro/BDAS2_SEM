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
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    INSERT INTO LEK (NAZEV, MNOZSTVI, CENA, TYP_LEK_ID) 
                    VALUES (:Nazev, :Mnozstvi, :Cena, :TypLekId) 
                    RETURNING ID_LEK INTO :IdLek";

                var parameters = new DynamicParameters();
                parameters.Add("Nazev", lek.Nazev, DbType.String);
                parameters.Add("Mnozstvi", lek.Mnozstvi, DbType.Int32);
                parameters.Add("Cena", lek.Cena, DbType.Decimal);
                parameters.Add("TypLekId", lek.TypLekId, DbType.Int32);
                parameters.Add("IdLek", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdLek");
            }
        }

        public async Task UpdateLek(LEK lek)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    UPDATE LEK 
                    SET NAZEV = :Nazev, 
                        MNOZSTVI = :Mnozstvi, 
                        CENA = :Cena, 
                        TYP_LEK_ID = :TypLekId 
                    WHERE ID_LEK = :IdLek";

                var parameters = new
                {
                    Nazev = lek.Nazev,
                    Mnozstvi = lek.Mnozstvi,
                    Cena = lek.Cena,
                    TypLekId = lek.TypLekId,
                    IdLek = lek.IdLek
                };

                await db.ExecuteAsync(sql, parameters);
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
                string sql = @"
                    SELECT ID_LEK AS IdLek, 
                           NAZEV AS Nazev, 
                           MNOZSTVI AS Mnozstvi, 
                           CENA AS Cena, 
                           TYP_LEK_ID AS TypLekId 
                    FROM LEK";

                return await db.QueryAsync<LEK>(sql);
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
