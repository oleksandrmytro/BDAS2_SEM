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
    public class PoziceRepository : IPoziceRepository
    {
        private readonly string connectionString;

        public PoziceRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddPozice(POZICE pozice)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO POZICE (ID_POZICE, NAZEV) 
                    VALUES (POZICE_SEQ.NEXTVAL, :Nazev) 
                    RETURNING ID_POZICE INTO :IdPozice";

                var parameters = new DynamicParameters();
                parameters.Add("Nazev", pozice.Nazev, DbType.String);
                parameters.Add("IdPozice", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdPozice");
            }
        }

        public async Task UpdatePozice(POZICE pozice)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE POZICE 
                    SET NAZEV = :Nazev 
                    WHERE ID_POZICE = :IdPozice";

                var parameters = new DynamicParameters();
                parameters.Add("Nazev", pozice.Nazev, DbType.String);
                parameters.Add("IdPozice", pozice.IdPozice, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<POZICE> GetPoziceById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_POZICE AS IdPozice, 
                        NAZEV AS Nazev 
                    FROM 
                        POZICE 
                    WHERE 
                        ID_POZICE = :Id";

                return await db.QueryFirstOrDefaultAsync<POZICE>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<POZICE>> GetAllPozice()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_POZICE AS IdPozice, 
                        NAZEV AS Nazev 
                    FROM 
                        POZICE";

                return await db.QueryAsync<POZICE>(sql);
            }
        }

        public async Task DeletePozice(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM POZICE WHERE ID_POZICE = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
