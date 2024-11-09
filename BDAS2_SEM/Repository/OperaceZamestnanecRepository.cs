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
    public class OperaceZamestnanecRepository : IOperaceZamestnanecRepository
    {
        private readonly string connectionString;

        public OperaceZamestnanecRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task AddOperaceZamestnanec(OPERACE_ZAMESTNANEC operaceZamestnanec)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO OPERACE_ZAMESTNANEC (OPERACE_ID, ZAMESTNANEC_ID) 
                    VALUES (:OperaceId, :ZamestnanecId)";

                var parameters = new DynamicParameters();
                parameters.Add("OperaceId", operaceZamestnanec.OperaceId, DbType.Int32);
                parameters.Add("ZamestnanecId", operaceZamestnanec.ZamestnanecId, DbType.Int32);

                await db.ExecuteAsync("INSERT_OPERACE_ZAMESTNANEC", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<OPERACE_ZAMESTNANEC> GetOperaceZamestnanec(int operaceId, int zamestnanecId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT OPERACE_ID AS OperaceId, 
                           ZAMESTNANEC_ID AS ZamestnanecId 
                    FROM OPERACE_ZAMESTNANEC 
                    WHERE OPERACE_ID = :OperaceId AND ZAMESTNANEC_ID = :ZamestnanecId";

                return await db.QueryFirstOrDefaultAsync<OPERACE_ZAMESTNANEC>(sql, new { OperaceId = operaceId, ZamestnanecId = zamestnanecId });
            }
        }

        public async Task<IEnumerable<OPERACE_ZAMESTNANEC>> GetAllOperaceZamestnanec()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT OPERACE_ID AS OperaceId, 
                           ZAMESTNANEC_ID AS ZamestnanecId 
                    FROM OPERACE_ZAMESTNANEC";

                return await db.QueryAsync<OPERACE_ZAMESTNANEC>(sql);
            }
        }

        public async Task DeleteOperaceZamestnanec(int operaceId, int zamestnanecId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM OPERACE_ZAMESTNANEC 
                    WHERE OPERACE_ID = :OperaceId AND ZAMESTNANEC_ID = :ZamestnanecId";

                var parameters = new
                {
                    OperaceId = operaceId,
                    ZamestnanecId = zamestnanecId
                };

                await db.ExecuteAsync("DELETE_OPERACE_ZAMESTNANEC", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
