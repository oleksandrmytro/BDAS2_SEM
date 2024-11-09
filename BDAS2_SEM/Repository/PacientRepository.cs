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
                string sql = @"
                    INSERT INTO PACIENT 
                        (Jmeno, Prijmeni, RodneCislo, Telefon, DatumNarozeni, Pohlavi, AdresaId, UserDataId) 
                    VALUES 
                        (:Jmeno, :Prijmeni, :RodneCislo, :Telefon, :DatumNarozeni, :Pohlavi, :AdresaId, :UserDataId) 
                    RETURNING ID_PACIENT INTO :IdPacient";

                var parameters = new DynamicParameters();
                parameters.Add("Jmeno", pacient.Jmeno, DbType.String);
                parameters.Add("Prijmeni", pacient.Prijmeni, DbType.String);
                parameters.Add("RodneCislo", pacient.RodneCislo, DbType.Int32);
                parameters.Add("Telefon", pacient.Telefon, DbType.Int64);
                parameters.Add("DatumNarozeni", pacient.DatumNarozeni, DbType.Date);
                parameters.Add("Pohlavi", pacient.Pohlavi, DbType.String);
                parameters.Add("AdresaId", pacient.AdresaId, DbType.Int32);
                parameters.Add("UserDataId", pacient.UserDataId, DbType.Int32);
                parameters.Add("IdPacient", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdPacient");
            }
        }

        public async Task UpdatePacient(PACIENT pacient)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE PACIENT 
                    SET 
                        Jmeno = :Jmeno, 
                        Prijmeni = :Prijmeni, 
                        RodneCislo = :RodneCislo, 
                        Telefon = :Telefon, 
                        DatumNarozeni = :DatumNarozeni, 
                        Pohlavi = :Pohlavi, 
                        AdresaId = :AdresaId, 
                        UserDataId = :UserDataId 
                    WHERE 
                        ID_PACIENT = :IdPacient";

                var parameters = new DynamicParameters();
                parameters.Add("Jmeno", pacient.Jmeno, DbType.String);
                parameters.Add("Prijmeni", pacient.Prijmeni, DbType.String);
                parameters.Add("RodneCislo", pacient.RodneCislo, DbType.Int32);
                parameters.Add("Telefon", pacient.Telefon, DbType.Int64);
                parameters.Add("DatumNarozeni", pacient.DatumNarozeni, DbType.Date);
                parameters.Add("Pohlavi", pacient.Pohlavi, DbType.String);
                parameters.Add("AdresaId", pacient.AdresaId, DbType.Int32);
                parameters.Add("UserDataId", pacient.UserDataId, DbType.Int32);
                parameters.Add("IdPacient", pacient.IdPacient, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<PACIENT> GetPacientById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_PACIENT AS IdPacient, 
                        Jmeno, 
                        Prijmeni, 
                        RodneCislo, 
                        Telefon, 
                        DatumNarozeni, 
                        Pohlavi, 
                        AdresaId, 
                        UserDataId 
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
                        Jmeno, 
                        Prijmeni, 
                        RodneCislo, 
                        Telefon, 
                        DatumNarozeni, 
                        Pohlavi, 
                        AdresaId, 
                        UserDataId 
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
    }
}
