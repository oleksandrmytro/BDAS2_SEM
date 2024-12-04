using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection.Metadata;

namespace BDAS2_SEM.Repository
{
    public class BlobTableRepository : IBlobTableRepository
    {
        private readonly string _connectionString;

        public BlobTableRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<BLOB_TABLE> GetBlobContentByDoctorId(int doctorId)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = "SELECT * FROM BLOB_TABLE WHERE id_blob = :DoctorId";
                return await connection.QueryFirstOrDefaultAsync<BLOB_TABLE>(sql, new { DoctorId = doctorId });
            }
        }

        public async Task<int> AddBlobContent(BLOB_TABLE blob)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO BLOB_TABLE (ID_BLOB, NAZEV_SOUBORU, TYP_SOUBORU, PRIPONA_SOUBORU, OBSAH, DATUM_NAHRANI, DATUM_MODIFIKACE, OPERACE_PROVEDL, POPIS_OPERACE, ZAMESTNANEC_ID)
                    VALUES (BLOB_SEQ.NEXTVAL, :NazevSouboru, :TypSouboru, :PriponaSouboru, :Obsah, :DatumNahrani, :DatumModifikace, :OperaceProvedl, :PopisOperace, :ZamestnanecId)
                    RETURNING ID_BLOB INTO :IdBlob";

                var parameters = new DynamicParameters();
                parameters.Add("NazevSouboru", blob.NazevSouboru, DbType.String);
                parameters.Add("TypSouboru", blob.TypSouboru, DbType.String);
                parameters.Add("PriponaSouboru", blob.PriponaSouboru, DbType.String);
                parameters.Add("Obsah", blob.Obsah, DbType.Binary);
                parameters.Add("DatumNahrani", blob.DatumNahrani, DbType.Date);
                parameters.Add("DatumModifikace", blob.DatumModifikace, DbType.Date);
                parameters.Add("OperaceProvedl", blob.OperaceProvedl, DbType.String);
                parameters.Add("PopisOperace", blob.PopisOperace, DbType.String);
                parameters.Add("IdBlob", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);
                return parameters.Get<int>("IdBlob");
            }
        }

        public async Task<BLOB_TABLE> GetBlobByZamestnanecId(int zamestnanecId)
        {
            //    using (var db = new OracleConnection(_connectionString))
        //    {
        //        string sql = @"
        //SELECT ID_BLOB AS IdBlob,
        //       NAZEV_SOUBORU AS NazevSouboru,
        //       TYP_SOUBORU AS TypSouboru,
        //       PRIPONA_SOUBORU AS PriponaSouboru,
        //       OBSAH AS Obsah,
        //       DATUM_NAHRANI AS DatumNahrani,
        //       DATUM_MODIFIKACE AS DatumModifikace,
        //       OPERACE_PROVEDL AS OperaceProvedl,
        //       POPIS_OPERACE AS PopisOperace,
        //       ZAMESTNANEC_ID AS ZamestnanecId
        //FROM BLOB_TABLE
        //WHERE ZAMESTNANEC_ID = :ZamestnanecId";

        //        return await db.QueryFirstOrDefaultAsync<BLOB_TABLE>(sql, new { ZamestnanecId = zamestnanecId });
        //    }
        return null;
        }

        public async Task UpdateBlobContent(BLOB_TABLE content)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = @"
            UPDATE BLOB_TABLE
            SET NAZEV_SOUBORU = :NazevSouboru,
                TYP_SOUBORU = :TypSouboru,
                PRIPONA_SOUBORU = :PriponaSouboru,
                OBSAH = :Obsah,
                DATUM_MODIFIKACE = :DatumModifikace,
                OPERACE_PROVEDL = :OperaceProvedl,
                POPIS_OPERACE = :PopisOperace
            WHERE ID_BLOB = :IdBlob";

                var parameters = new DynamicParameters();
                parameters.Add("NazevSouboru", content.NazevSouboru, DbType.String);
                parameters.Add("TypSouboru", content.TypSouboru, DbType.String);
                parameters.Add("PriponaSouboru", content.PriponaSouboru, DbType.String);
                parameters.Add("Obsah", content.Obsah, DbType.Binary);
                parameters.Add("DatumModifikace", content.DatumModifikace);
                parameters.Add("OperaceProvedl", content.OperaceProvedl);
                parameters.Add("PopisOperace", content.PopisOperace);
                parameters.Add("IdBlob", content.IdBlob);

                await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task DeleteBlobContent(int id)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = "DELETE FROM blob WHERE id_blob = :IdBlob";
                await connection.ExecuteAsync(sql, new { IdBlob = id });
            }
        }

        public async Task<IEnumerable<BLOB_TABLE>> GetAllBlobTables()
        {
            using (var db = new OracleConnection(this._connectionString))
            {
                var sqlQuery = @"
                    SELECT id_blob AS IdBlob,
                           nazev_souboru AS NazevSouboru,
                           typ_souboru AS TypSouboru,
                           obsah AS Obsah,
                           datum_nahrani AS DatumNahrani,
                           datum_modifikace AS DatumModifikace,
                           operace_provedl AS OperaceProvedl,
                           popis_operace AS PopisOperace,
                           pripona_id AS PriponaId
                    FROM BLOB_TABLE";

                return await db.QueryAsync<BLOB_TABLE>(sqlQuery);
            }
        }
    }
}