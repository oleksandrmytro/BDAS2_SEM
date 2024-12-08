using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
                const string procedureName = "manage_blob_table";

                await db.OpenAsync();

                using (var command = new OracleCommand(procedureName, db))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Добавление параметров
                    command.Parameters.Add("p_action", OracleDbType.Varchar2, ParameterDirection.Input).Value = "INSERT";
                    command.Parameters.Add("p_id_blob", OracleDbType.Int32, ParameterDirection.Output);
                    command.Parameters.Add("p_nazev_souboru", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.NazevSouboru;
                    command.Parameters.Add("p_typ_souboru", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.TypSouboru;
                    command.Parameters.Add("p_obsah", OracleDbType.Blob, ParameterDirection.Input).Value = blob.Obsah ?? (object)DBNull.Value;
                    command.Parameters.Add("p_operace_provedl", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.OperaceProvedl;
                    command.Parameters.Add("p_popis_operace", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.PopisOperace ?? (object)DBNull.Value;
                    command.Parameters.Add("p_pripona_id", OracleDbType.Int32, ParameterDirection.Input).Value = blob.PriponaId;

                    try
                    {
                        // Выполнение команды
                        await command.ExecuteNonQueryAsync();

                        // Получение выходного параметра
                        int newIdBlob = Convert.ToInt32(command.Parameters["p_id_blob"].Value.ToString());

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
        }


        public async Task UpdateBlob(BLOB_TABLE blob)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                const string procedureName = "manage_blob_table";

                await db.OpenAsync();

                using (var command = new OracleCommand(procedureName, db))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Добавление параметров
                    command.Parameters.Add("p_action", OracleDbType.Varchar2, ParameterDirection.Input).Value = "UPDATE";
                    command.Parameters.Add("p_id_blob", OracleDbType.Int32, ParameterDirection.Input).Value = blob.IdBlob;
                    command.Parameters.Add("p_nazev_souboru", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.NazevSouboru;
                    command.Parameters.Add("p_typ_souboru", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.TypSouboru;
                    command.Parameters.Add("p_obsah", OracleDbType.Blob, ParameterDirection.Input).Value = blob.Obsah ?? (object)DBNull.Value;
                    command.Parameters.Add("p_operace_provedl", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.OperaceProvedl;
                    command.Parameters.Add("p_popis_operace", OracleDbType.Varchar2, ParameterDirection.Input).Value = blob.PopisOperace ?? (object)DBNull.Value;
                    command.Parameters.Add("p_pripona_id", OracleDbType.Int32, ParameterDirection.Input).Value = blob.PriponaId;

                    try
                    {
                        // Выполнение команды
                        await command.ExecuteNonQueryAsync();
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
