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
    public class KartaRepository : IKartaRepository
    {
        private readonly string connectionString;

        public KartaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddKarta(KARTA karta)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    INSERT INTO KARTA (CISLO_KARTY) 
                    VALUES (:CisloKarty) 
                    RETURNING ID_PLATBA INTO :IdPlatba";

                var parameters = new DynamicParameters();
                parameters.Add("CisloKarty", karta.CisloKarty, DbType.Int64);
                parameters.Add("IdPlatba", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdPlatba");
            }
        }

        public async Task UpdateKarta(KARTA karta)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    UPDATE KARTA 
                    SET CISLO_KARTY = :CisloKarty 
                    WHERE ID_PLATBA = :IdPlatba";

                var parameters = new
                {
                    CisloKarty = karta.CisloKarty,
                    IdPlatba = karta.IdPlatba
                };

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<KARTA> GetKartaById(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    SELECT ID_PLATBA AS IdPlatba, 
                           CISLO_KARTY AS CisloKarty 
                    FROM KARTA 
                    WHERE ID_PLATBA = :Id";

                return await db.QueryFirstOrDefaultAsync<KARTA>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<KARTA>> GetAllKarta()
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                var sqlQuery = @"
                    SELECT ID_PLATBA AS IdPlatba,
                           CISLO_KARTY AS Cislo
                    FROM KARTA";

                return await db.QueryAsync<KARTA>(sqlQuery);
            }
        }

        public async Task DeleteKarta(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = "DELETE FROM KARTA WHERE ID_PLATBA = :Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
