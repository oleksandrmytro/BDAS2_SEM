using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class BlobTableRepository : IBlobTableRepository
    {
        private readonly string _connectionString;

        public BlobTableRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<BLOB_TABLE> GetBlobById(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_BLOB AS IdBlob,
                           NAZEV_SOUBORU AS NazevSouboru,
                           TYP_SOUBORU AS TypSouboru,
                           OBSAH AS Obsah,
                           DATUM_NAHRANI AS DatumNahrani,
                           DATUM_MODIFIKACE AS DatumModifikace,
                           OPERACE_PROVEDL AS OperaceProvedl,
                           POPIS_OPERACE AS PopisOperace,
                           PRIPONA_ID AS PriponaId
                    FROM BLOB_TABLE
                    WHERE ID_BLOB = :IdBlob";

                return await db.QueryFirstOrDefaultAsync<BLOB_TABLE>(sql, new { IdBlob = id });
            }
        }

        public async Task<int> AddBlob(BLOB_TABLE blob)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                // Проверяем, существует ли PriponaId в таблице Pripona
                string checkPriponaSql = "SELECT COUNT(1) FROM PRIPONA WHERE ID_PRIPONA = :PriponaId";
                var priponaExists = await db.ExecuteScalarAsync<int>(checkPriponaSql, new { PriponaId = blob.PriponaId }) > 0;

                string sql = @"
                    INSERT INTO BLOB_TABLE (ID_BLOB, NAZEV_SOUBORU, TYP_SOUBORU, OBSAH, DATUM_NAHRANI, DATUM_MODIFIKACE, OPERACE_PROVEDL, POPIS_OPERACE, PRIPONA_ID)
                    VALUES (BLOB_SEQ.NEXTVAL, :NazevSouboru, :TypSouboru, :Obsah, :DatumNahrani, :DatumModifikace, :OperaceProvedl, :PopisOperace, :PriponaId)
                    RETURNING ID_BLOB INTO :IdBlob";

                var parameters = new DynamicParameters();
                parameters.Add("NazevSouboru", blob.NazevSouboru, DbType.String);
                parameters.Add("TypSouboru", blob.TypSouboru, DbType.String);
                parameters.Add("Obsah", blob.Obsah, DbType.Binary);
                parameters.Add("DatumNahrani", blob.DatumNahrani, DbType.Date);
                parameters.Add("DatumModifikace", blob.DatumModifikace, DbType.Date);
                parameters.Add("OperaceProvedl", blob.OperaceProvedl, DbType.String);
                parameters.Add("PopisOperace", blob.PopisOperace, DbType.String);
                parameters.Add("PriponaId", priponaExists ? (object)blob.PriponaId : DBNull.Value, DbType.Int32);
                parameters.Add("IdBlob", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);
                return parameters.Get<int>("IdBlob");
            }
        }

        public async Task UpdateBlob(BLOB_TABLE blob)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    UPDATE BLOB_TABLE
                    SET NAZEV_SOUBORU = :NazevSouboru,
                        TYP_SOUBORU = :TypSouboru,
                        OBSAH = :Obsah,
                        DATUM_MODIFIKACE = :DatumModifikace,
                        OPERACE_PROVEDL = :OperaceProvedl,
                        POPIS_OPERACE = :PopisOperace,
                        PRIPONA_ID = :PriponaId
                    WHERE ID_BLOB = :IdBlob";

                var parameters = new DynamicParameters();
                parameters.Add("NazevSouboru", blob.NazevSouboru, DbType.String);
                parameters.Add("TypSouboru", blob.TypSouboru, DbType.String);
                parameters.Add("Obsah", blob.Obsah, DbType.Binary);
                parameters.Add("DatumModifikace", blob.DatumModifikace, DbType.Date);
                parameters.Add("OperaceProvedl", blob.OperaceProvedl, DbType.String);
                parameters.Add("PopisOperace", blob.PopisOperace, DbType.String);
                parameters.Add("PriponaId", blob.PriponaId, DbType.Int32);
                parameters.Add("IdBlob", blob.IdBlob, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task DeleteBlob(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = "DELETE FROM BLOB_TABLE WHERE ID_BLOB = :IdBlob";
                await db.ExecuteAsync(sql, new { IdBlob = id });
            }
        }

        public async Task<IEnumerable<BLOB_TABLE>> GetAllBlobTables()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_BLOB AS IdBlob,
                           NAZEV_SOUBORU AS NazevSouboru,
                           TYP_SOUBORU AS TypSouboru,
                           OBSAH AS Obsah,
                           DATUM_NAHRANI AS DatumNahrani,
                           DATUM_MODIFIKACE AS DatumModifikace,
                           OPERACE_PROVEDL AS OperaceProvedl,
                           POPIS_OPERACE AS PopisOperace,
                           PRIPONA_ID AS PriponaId
                    FROM BLOB_TABLE";

                return await db.QueryAsync<BLOB_TABLE>(sql);
            }
        }

      
    }
}
