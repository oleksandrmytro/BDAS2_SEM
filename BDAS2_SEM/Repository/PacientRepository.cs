using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class PacientRepository : IPacientRepository
    {
        private readonly string connectionString;

        public PacientRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddPacient(PACIENT pacient)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_pacient";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String);
                parameters.Add("p_id_pacient", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_jmeno", pacient.Jmeno, DbType.String);
                parameters.Add("p_prijmeni", pacient.Prijmeni, DbType.String);
                parameters.Add("p_rodne_cislo", pacient.RodneCislo, DbType.Int32);
                parameters.Add("p_telefon", pacient.Telefon, DbType.Int64);
                parameters.Add("p_datum_narozeni", pacient.DatumNarozeni, DbType.Date);
                parameters.Add("p_pohlavi", pacient.Pohlavi, DbType.String);
                parameters.Add("p_adresa_id_adresa", pacient.AdresaId, DbType.Int32);
                parameters.Add("p_uzivatel_data_id", pacient.UserDataId, DbType.Int32);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                var newPacientId = parameters.Get<int?>("p_id_pacient");
                if (newPacientId.HasValue)
                {
                    return newPacientId.Value;
                }
                else
                {
                    throw new Exception("The procedure did not return a valid patient ID.");
                }
            }
        }

        public async Task UpdatePacient(PACIENT pacient)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string procedureName = "manage_pacient";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String);
                parameters.Add("p_id_pacient", pacient.IdPacient, DbType.Int32);
                parameters.Add("p_jmeno", pacient.Jmeno, DbType.String);
                parameters.Add("p_prijmeni", pacient.Prijmeni, DbType.String);
                parameters.Add("p_rodne_cislo", pacient.RodneCislo, DbType.Int32);
                parameters.Add("p_telefon", pacient.Telefon, DbType.Int64);
                parameters.Add("p_datum_narozeni", pacient.DatumNarozeni, DbType.Date);
                parameters.Add("p_pohlavi", pacient.Pohlavi, DbType.String);
                parameters.Add("p_adresa_id_adresa", pacient.AdresaId, DbType.Int32);
                parameters.Add("p_uzivatel_data_id", pacient.UserDataId, DbType.Int32);

                Debug.WriteLine("Executing stored procedure: " + procedureName);
                Debug.WriteLine("Parameters: " + string.Join(", ", parameters.ParameterNames.Select(name => $"{name}={parameters.Get<object>(name)}")));

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                    Debug.WriteLine("Stored procedure executed successfully");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error executing stored procedure: " + ex.Message);
                    throw;
                }
            }
        }


        public async Task<PACIENT> GetPacientById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            SELECT 
                ID_PACIENT AS IdPacient, 
                JMENO AS Jmeno, 
                PRIJMENI AS Prijmeni, 
                RODNE_CISLO AS RodneCislo, 
                TELEFON AS Telefon, 
                DATUM_NAROZENI AS DatumNarozeni, 
                POHLAVI AS Pohlavi, 
                ADRESA_ID_ADRESA AS AdresaId, 
                UZIVATEL_DATA_ID_UZIVATEL_DATA AS UserDataId 
            FROM 
                PACIENT 
            WHERE 
                ID_PACIENT = :Id";

                return await db.QueryFirstOrDefaultAsync<PACIENT>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<PACIENT>> GetAllPacienti()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            SELECT 
                ID_PACIENT AS IdPacient, 
                JMENO AS Jmeno, 
                PRIJMENI AS Prijmeni, 
                RODNE_CISLO AS RodneCislo, 
                TELEFON AS Telefon, 
                DATUM_NAROZENI AS DatumNarozeni, 
                POHLAVI AS Pohlavi, 
                ADRESA_ID_ADRESA AS AdresaId, 
                UZIVATEL_DATA_ID AS UserDataId 
            FROM 
                PACIENT";

                return await db.QueryAsync<PACIENT>(sql);
            }
        }

        public async Task DeletePacient(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM PACIENT WHERE ID_PACIENT = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<PACIENT> GetPacientByUserDataId(int userDataId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            SELECT 
                ID_PACIENT AS IdPacient,
                JMENO AS Jmeno,
                PRIJMENI AS Prijmeni,
                POHLAVI AS Pohlavi,
                DATUM_NAROZENI AS DatumNarozeni,
                RODNE_CISLO AS RodneCislo,
                TELEFON AS Telefon,
                UZIVATEL_DATA_ID_UZIVATEL_DATA AS UserDataId,
                ADRESA_ID_ADRESA AS AdresaId
            FROM PACIENT 
            WHERE UZIVATEL_DATA_ID_UZIVATEL_DATA = :UserDataId";

                return await db.QueryFirstOrDefaultAsync<PACIENT>(sql, new { UserDataId = userDataId });
            }
        }

    }
}
