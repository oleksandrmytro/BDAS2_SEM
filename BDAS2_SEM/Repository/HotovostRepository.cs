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
    public class HotovostRepository : IHotovostRepository
    {
        private readonly string connectionString;

        public HotovostRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddHotovost(HOTOVOST hotovost)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    INSERT INTO HOTOVOST (PRIJATO, VRACENO) 
                    VALUES (:Prijato, :Vraceno) 
                    RETURNING ID_PLATBA INTO :IdPlatba";

                var parameters = new DynamicParameters();
                parameters.Add("Prijato", hotovost.Prijato, DbType.Decimal);
                parameters.Add("Vraceno", hotovost.Vraceno, DbType.Decimal);
                parameters.Add("IdPlatba", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdPlatba");
            }
        }

        public async Task UpdateHotovost(HOTOVOST hotovost)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    UPDATE HOTOVOST 
                    SET PRIJATO = :Prijato, VRACENO = :Vraceno 
                    WHERE ID_PLATBA = :IdPlatba";

                var parameters = new
                {
                    Prijato = hotovost.Prijato,
                    Vraceno = hotovost.Vraceno,
                    IdPlatba = hotovost.IdPlatba
                };

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<HOTOVOST> GetHotovostById(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    SELECT ID_PLATBA AS IdPlatba, 
                           PRIJATO AS Prijato, 
                           VRACENO AS Vraceno 
                    FROM HOTOVOST 
                    WHERE ID_PLATBA = :Id";

                return await db.QueryFirstOrDefaultAsync<HOTOVOST>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<HOTOVOST>> GetAllHotovosti()
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    SELECT ID_PLATBA AS IdPlatba, 
                           PRIJATO AS Prijato, 
                           VRACENO AS Vraceno 
                    FROM HOTOVOST";

                return await db.QueryAsync<HOTOVOST>(sql);
            }
        }

        public async Task DeleteHotovost(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = "DELETE FROM HOTOVOST WHERE ID_PLATBA = :Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<HOTOVOST>> GetAllHotovost()
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                var sqlQuery = @"
                    SELECT id_platba AS IdPlatba,
                           prijato AS Prijato,
                           vraceno AS Vraceno
                    FROM HOTOVOST";

                return await db.QueryAsync<HOTOVOST>(sqlQuery);
            }
        }
    }
}
