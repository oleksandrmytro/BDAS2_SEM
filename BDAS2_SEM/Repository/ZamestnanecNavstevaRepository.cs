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
    public class ZamestnanecNavstevaRepository : IZamestnanecNavstevaRepository
    {
        private readonly string connectionString;

        public ZamestnanecNavstevaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<ZAMESTNANEC_NAVSTEVA>> GetAllZamestnanecNavsteva()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ZAMESTNANEC_ID AS ZamestnanecId, 
                        NAVSTEVA_ID AS NavstevaId 
                    FROM 
                        ZAMESTNANEC_NAVSTEVA";

                return await db.QueryAsync<ZAMESTNANEC_NAVSTEVA>(sql);
            }
        }

        public async Task<ZAMESTNANEC_NAVSTEVA> GetZamestnanecNavstevaById(int zamestnanecId, int navstevaId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ZAMESTNANEC_ID AS ZamestnanecId, 
                        NAVSTEVA_ID AS NavstevaId 
                    FROM 
                        ZAMESTNANEC_NAVSTEVA 
                    WHERE 
                        ZAMESTNANEC_ID = :ZamestnanecId AND NAVSTEVA_ID = :NavstevaId";

                return await db.QueryFirstOrDefaultAsync<ZAMESTNANEC_NAVSTEVA>(sql, new { ZamestnanecId = zamestnanecId, NavstevaId = navstevaId });
            }
        }

        public async Task InsertZamestnanecNavsteva(ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO ZAMESTNANEC_NAVSTEVA (ZAMESTNANEC_ID_ZAMESTNANEC, NAVSTEVA_ID_NAVSTEVA) 
                    VALUES (:ZamestnanecId, :NavstevaId)";

                var parameters = new DynamicParameters();
                parameters.Add("ZamestnanecId", zamestnanecNavsteva.ZamestnanecId, DbType.Int32);
                parameters.Add("NavstevaId", zamestnanecNavsteva.NavstevaId, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateZamestnanecNavsteva(ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE ZAMESTNANEC_NAVSTEVA 
                    SET NAVSTEVA_ID = :NewNavstevaId 
                    WHERE ZAMESTNANEC_ID = :ZamestnanecId AND NAVSTEVA_ID = :NavstevaId";

                var parameters = new DynamicParameters();
                parameters.Add("NewNavstevaId", zamestnanecNavsteva.NavstevaId, DbType.Int32);
                parameters.Add("ZamestnanecId", zamestnanecNavsteva.ZamestnanecId, DbType.Int32);
                parameters.Add("NavstevaId", zamestnanecNavsteva.NavstevaId, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task DeleteZamestnanecNavsteva(int zamestnanecId, int navstevaId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM ZAMESTNANEC_NAVSTEVA 
                    WHERE ZAMESTNANEC_ID = :ZamestnanecId AND NAVSTEVA_ID = :NavstevaId";

                var parameters = new DynamicParameters();
                parameters.Add("ZamestnanecId", zamestnanecId, DbType.Int32);
                parameters.Add("NavstevaId", navstevaId, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }
    }
}
