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
    public class ZamestnanecRepository : IZamestnanecRepository
    {
        private readonly string connectionString;

        public ZamestnanecRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddZamestnanec(ZAMESTNANEC zamestnanec)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO ZAMESTNANEC 
                        (ID_ZAMESTNANEC, Jmeno, Prijmeni, Telefon, ZAMESTNANEC_ID_ZAMESTNANEC, ADRESA_ID_ADRESA, POZICE_ID_POZICE, UZIVATEL_DATA_ID_UZIVATEL_DATA) 
                    VALUES 
                        (ZAMESTNANEC_SEQ.NEXTVAL, :Jmeno, :Prijmeni, :Telefon, :NadrazenyZamestnanecId, :AdresaId, :PoziceId, :UserDataId) 
                    RETURNING ID_ZAMESTNANEC INTO :IdZamestnanec";

                var parameters = new DynamicParameters();
                parameters.Add("Jmeno", zamestnanec.Jmeno, DbType.String);
                parameters.Add("Prijmeni", zamestnanec.Prijmeni, DbType.String);
                parameters.Add("Telefon", zamestnanec.Telefon, DbType.Int64);
                parameters.Add("NadrazenyZamestnanecId", zamestnanec.NadrazenyZamestnanecId, DbType.Int32);
                parameters.Add("AdresaId", zamestnanec.AdresaId, DbType.Int32);
                parameters.Add("PoziceId", zamestnanec.PoziceId, DbType.Int32);
                parameters.Add("UserDataId", zamestnanec.UserDataId, DbType.Int32);
                parameters.Add("IdZamestnanec", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdZamestnanec");
            }
        }

        public async Task UpdateZamestnanec(ZAMESTNANEC zamestnanec)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE ZAMESTNANEC 
                    SET 
                        Jmeno = :Jmeno, 
                        Prijmeni = :Prijmeni, 
                        Telefon = :Telefon, 
                        NadrazenyZamestnanecId = :NadrazenyZamestnanecId, 
                        AdresaId = :AdresaId, 
                        PoziceId = :PoziceId, 
                        UserDataId = :UserDataId 
                    WHERE 
                        ID_ZAMESTNANEC = :IdZamestnanec";

                var parameters = new DynamicParameters();
                parameters.Add("Jmeno", zamestnanec.Jmeno, DbType.String);
                parameters.Add("Prijmeni", zamestnanec.Prijmeni, DbType.String);
                parameters.Add("Telefon", zamestnanec.Telefon, DbType.Int64);
                parameters.Add("NadrazenyZamestnanecId", zamestnanec.NadrazenyZamestnanecId, DbType.Int32);
                parameters.Add("AdresaId", zamestnanec.AdresaId, DbType.Int32);
                parameters.Add("PoziceId", zamestnanec.PoziceId, DbType.Int32);
                parameters.Add("UserDataId", zamestnanec.UserDataId, DbType.Int32);
                parameters.Add("IdZamestnanec", zamestnanec.IdZamestnanec, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<ZAMESTNANEC> GetZamestnanecById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_ZAMESTNANEC AS IdZamestnanec, 
                        Jmeno AS Jmeno, 
                        Prijmeni AS Prijmeni, 
                        Telefon AS Telefon, 
                        NadrazenyZamestnanecId AS NadrazenyZamestnanecId, 
                        AdresaId AS AdresaId, 
                        PoziceId AS PoziceId, 
                        UserDataId AS UserDataId 
                    FROM 
                        ZAMESTNANEC 
                    WHERE 
                        ID_ZAMESTNANEC = :Id";

                return await db.QueryFirstOrDefaultAsync<ZAMESTNANEC>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<ZAMESTNANEC>> GetAllZamestnanci()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_ZAMESTNANEC AS IdZamestnanec, 
                        Jmeno AS Jmeno, 
                        Prijmeni AS Prijmeni, 
                        Telefon AS Telefon, 
                        ZAMESTNANEC_ID_ZAMESTNANEC AS NadrazenyZamestnanecId, 
                        ADRESA_ID_ADRESA AS AdresaId, 
                        POZICE_ID_POZICE AS PoziceId, 
                        UZIVATEL_DATA_ID_UZIVATEL_DATA AS UserDataId 
                    FROM 
                        ZAMESTNANEC";

                return await db.QueryAsync<ZAMESTNANEC>(sql);
            }
        }

        public async Task DeleteZamestnanec(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM ZAMESTNANEC WHERE ID_ZAMESTNANEC = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
