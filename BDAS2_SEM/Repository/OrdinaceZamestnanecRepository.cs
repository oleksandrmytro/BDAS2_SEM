using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class OrdinaceZamestnanecRepository : IOrdinaceZamestnanecRepository
    {
        private readonly string connectionString;

        public OrdinaceZamestnanecRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task AddOrdinaceZamestnanec(ORDINACE_ZAMESTNANEC ordinaceZamestnanec)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO ORDINACE_ZAMESTNANEC (ORDINACE_ID_ORDINACE, ZAMESTNANEC_ID_ZAMESTNANEC) 
                    VALUES (:OrdinaceId, :ZamestnanecId)";

                var parameters = new DynamicParameters();
                parameters.Add("OrdinaceId", ordinaceZamestnanec.OrdinaceId, DbType.Int32);
                parameters.Add("ZamestnanecId", ordinaceZamestnanec.ZamestnanecId, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<ORDINACE_ZAMESTNANEC> GetOrdinaceZamestnanecByZamestnanecId(int zamestnanecId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            SELECT 
                ORDINACE_ID_ORDINACE AS OrdinaceId, 
                ZAMESTNANEC_ID_ZAMESTNANEC AS ZamestnanecId 
            FROM ORDINACE_ZAMESTNANEC 
            WHERE ZAMESTNANEC_ID_ZAMESTNANEC = :ZamestnanecId";

                return await db.QueryFirstOrDefaultAsync<ORDINACE_ZAMESTNANEC>(sql, new { ZamestnanecId = zamestnanecId });
            }
        }

        public async Task<IEnumerable<ORDINACE_ZAMESTNANEC>> GetAllOrdinaceZamestnanec()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ORDINACE_ID_ORDINACE AS OrdinaceId, 
                        ZAMESTNANEC_ID_ZAMESTNANEC AS ZamestnanecId 
                    FROM ORDINACE_ZAMESTNANEC";

                return await db.QueryAsync<ORDINACE_ZAMESTNANEC>(sql);
            }
        }

        public async Task DeleteOrdinaceZamestnanec(int ordinaceId, int zamestnanecId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    DELETE FROM ORDINACE_ZAMESTNANEC 
                    WHERE ORDINACE_ID_ORDINACE = :OrdinaceId AND ZAMESTNANEC_ID_ZAMESTNANEC = :ZamestnanecId";

                await db.ExecuteAsync(sql, new { OrdinaceId = ordinaceId, ZamestnanecId = zamestnanecId });
            }
        }
    }
}