using Dapper;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class PlatbaRepository : IPlatbaRepository
    {
        private readonly string connectionString;

        public PlatbaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddPlatba(PLATBA platba)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO PLATBA 
                        (CASTKA, DATUM, TYP_PLATBY, NAVSTEVA_ID) 
                    VALUES 
                        (:Castka, :Datum, :TypPlatby, :NavstevaId) 
                    RETURNING ID_PLATBA INTO :IdPlatba";

                var parameters = new DynamicParameters();
                parameters.Add("Castka", platba.Castka, DbType.Decimal);
                parameters.Add("Datum", platba.Datum, DbType.Date);
                parameters.Add("TypPlatby", platba.TypPlatby, DbType.String);
                parameters.Add("NavstevaId", platba.NavstevaId, DbType.Int32);
                parameters.Add("IdPlatba", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdPlatba");
            }
        }

        public async Task UpdatePlatba(PLATBA platba)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE PLATBA 
                    SET 
                        CASTKA = :Castka, 
                        DATUM = :Datum, 
                        TYP_PLATBY = :TypPlatby, 
                        NAVSTEVA_ID = :NavstevaId 
                    WHERE 
                        ID_PLATBA = :IdPlatba";

                var parameters = new DynamicParameters();
                parameters.Add("Castka", platba.Castka, DbType.Decimal);
                parameters.Add("Datum", platba.Datum, DbType.Date);
                parameters.Add("TypPlatby", platba.TypPlatby, DbType.String);
                parameters.Add("NavstevaId", platba.NavstevaId, DbType.Int32);
                parameters.Add("IdPlatba", platba.IdPlatba, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<PLATBA> GetPlatbaById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_PLATBA AS IdPlatba, 
                        CASTKA AS Castka, 
                        DATUM AS Datum, 
                        TYP_PLATBY AS TypPlatby, 
                        NAVSTEVA_ID AS NavstevaId 
                    FROM 
                        PLATBA 
                    WHERE 
                        ID_PLATBA = :Id";

                return await db.QueryFirstOrDefaultAsync<PLATBA>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<PLATBA>> GetAllPlatby()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_PLATBA AS IdPlatba, 
                        CASTKA AS Castka, 
                        DATUM AS Datum, 
                        TYP_PLATBY AS TypPlatby, 
                        NAVSTEVA_ID AS NavstevaId 
                    FROM 
                        PLATBA";

                return await db.QueryAsync<PLATBA>(sql);
            }
        }

        public async Task DeletePlatba(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM PLATBA WHERE ID_PLATBA = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
