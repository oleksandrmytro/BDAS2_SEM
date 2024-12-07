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
                var procedureName = "manage_blob_table";

                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_blob", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev_souboru", blob.NazevSouboru, DbType.String, ParameterDirection.Input);
                parameters.Add("p_typ_souboru", blob.TypSouboru, DbType.String, ParameterDirection.Input);
                parameters.Add("p_obsah", blob.Obsah, DbType.Binary, ParameterDirection.Input);
                parameters.Add("p_operace_provedl", blob.OperaceProvedl, DbType.String, ParameterDirection.Input);
                parameters.Add("p_popis_operace", blob.PopisOperace, DbType.String, ParameterDirection.Input);
                parameters.Add("p_pripona_id", blob.PriponaId, DbType.Int32, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                    int newIdBlob = parameters.Get<int>("p_id_blob");
                    return newIdBlob;
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Database error occurred: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred: {ex.Message}", ex);
                }
            }
        }


        public async Task UpdateBlob(BLOB_TABLE blob)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var procedureName = "manage_blob_table";

                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_blob", blob.IdBlob, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_nazev_souboru", blob.NazevSouboru, DbType.String, ParameterDirection.Input);
                parameters.Add("p_typ_souboru", blob.TypSouboru, DbType.String, ParameterDirection.Input);
                parameters.Add("p_obsah", blob.Obsah, DbType.Binary, ParameterDirection.Input);
                parameters.Add("p_operace_provedl", blob.OperaceProvedl, DbType.String, ParameterDirection.Input);
                parameters.Add("p_popis_operace", blob.PopisOperace, DbType.String, ParameterDirection.Input);
                parameters.Add("p_pripona_id", blob.PriponaId, DbType.Int32, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                    Console.WriteLine($"Blob with ID {blob.IdBlob} updated successfully.");
                }
                catch (OracleException ex)
                {
                    throw new Exception($"Database error occurred: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred: {ex.Message}", ex);
                }
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
