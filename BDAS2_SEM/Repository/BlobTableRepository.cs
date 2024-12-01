using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Data;

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

        public async Task<int> AddBlobContent(BLOB_TABLE content)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = @"
                    INSERT INTO blob (nazev_souboru, typ_souboru, pripona_souboru, obsah, datum_nahrani, operace_provedl, popis_operace)
                    VALUES (:NazevSouboru, :TypSouboru, :PriponaSouboru, :Obsah, :DatumNahrani, :OperaceProvedl, :PopisOperace)
                    RETURNING id_blob INTO :IdBlob";

                var parameters = new DynamicParameters();
                parameters.Add("NazevSouboru", content.NazevSouboru);
                parameters.Add("TypSouboru", content.TypSouboru);
                parameters.Add("PriponaSouboru", content.PriponaSouboru);
                parameters.Add("Obsah", content.Obsah, DbType.Binary);
                parameters.Add("DatumNahrani", content.DatumNahrani);
                parameters.Add("OperaceProvedl", content.OperaceProvedl);
                parameters.Add("PopisOperace", content.PopisOperace);
                parameters.Add("IdBlob", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(sql, parameters);
                return parameters.Get<int>("IdBlob");
            }
        }

        public async Task UpdateBlobContent(BLOB_TABLE content)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                string sql = @"
                    UPDATE blob
                    SET nazev_souboru = :NazevSouboru,
                        typ_souboru = :TypSouboru,
                        pripona_souboru = :PriponaSouboru,
                        obsah = :Obsah,
                        datum_modifikace = :DatumModifikace,
                        operace_provedl = :OperaceProvedl,
                        popis_operace = :PopisOperace
                    WHERE id_blob = :IdBlob";

                var parameters = new DynamicParameters();
                parameters.Add("NazevSouboru", content.NazevSouboru);
                parameters.Add("TypSouboru", content.TypSouboru);
                parameters.Add("PriponaSouboru", content.PriponaSouboru);
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
    }
}