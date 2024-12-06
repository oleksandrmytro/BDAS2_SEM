using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BDAS2_SEM.Repository
{
    public class MistnostRepository : IMistnostRepository
    {
        private readonly string _connectionString;

        public MistnostRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddMistnost(MISTNOST mistnost)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO MISTNOST (ID_MISTNOST, CISLO)
                    VALUES (MISTNOST_SEQ.NEXTVAL, :Cislo)
                    RETURNING ID_MISTNOST INTO :IdMistnost";

                var parameters = new DynamicParameters();
                parameters.Add("Cislo", mistnost.Cislo, DbType.Int32);
                parameters.Add("IdMistnost", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);
                return parameters.Get<int>("IdMistnost");
            }
        }

        public async Task UpdateMistnost(MISTNOST mistnost)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    UPDATE MISTNOST
                    SET CISLO = :Cislo
                    WHERE ID_MISTNOST = :IdMistnost";

                var parameters = new DynamicParameters();
                parameters.Add("Cislo", mistnost.Cislo, DbType.Int32);
                parameters.Add("IdMistnost", mistnost.IdMistnost, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<MISTNOST> GetMistnostById(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_MISTNOST AS IdMistnost,
                           CISLO AS Cislo
                    FROM MISTNOST
                    WHERE ID_MISTNOST = :IdMistnost";

                return await db.QueryFirstOrDefaultAsync<MISTNOST>(sql, new { IdMistnost = id });
            }
        }

        public async Task<IEnumerable<MISTNOST>> GetAllMistnosti()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_MISTNOST AS IdMistnost,
                           CISLO AS Cislo
                    FROM MISTNOST";

                return await db.QueryAsync<MISTNOST>(sql);
            }
        }

        public async Task<MISTNOST> GetMistnostByNumber(int roomNumber)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_MISTNOST AS IdMistnost,
                           CISLO AS Cislo
                    FROM MISTNOST
                    WHERE CISLO = :RoomNumber";

                var parameters = new DynamicParameters();
                parameters.Add("RoomNumber", roomNumber, DbType.Int32);

                return await db.QueryFirstOrDefaultAsync<MISTNOST>(sql, parameters);
            }
        }

        public async Task DeleteMistnost(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = "DELETE FROM MISTNOST WHERE ID_MISTNOST = :IdMistnost";
                await db.ExecuteAsync(sql, new { IdMistnost = id });
            }
        }
    }
}