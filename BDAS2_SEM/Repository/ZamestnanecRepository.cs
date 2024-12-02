using System.Data;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace BDAS2_SEM.Repository;

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
            var procedureName = "manage_zamestnanec";

            var parameters = new DynamicParameters();
            parameters.Add("p_action", "INSERT", DbType.String);
            parameters.Add("p_id_zamestnanec", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("p_jmeno", zamestnanec.Jmeno, DbType.String);
            parameters.Add("p_prijmeni", zamestnanec.Prijmeni, DbType.String);
            parameters.Add("p_telefon", zamestnanec.Telefon, DbType.Int64);
            parameters.Add("p_zamestnanec_id", zamestnanec.NadrazenyZamestnanecId, DbType.Int32);
            parameters.Add("p_adresa_id", zamestnanec.AdresaId, DbType.Int32);
            parameters.Add("p_pozice_id", zamestnanec.PoziceId, DbType.Int32);
            parameters.Add("p_uzivatel_data_id", zamestnanec.UserDataId, DbType.Int32);

            await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

            var newZamestnanecId = parameters.Get<int?>("p_id_zamestnanec");
            if (newZamestnanecId.HasValue)
                return newZamestnanecId.Value;
            throw new Exception("The procedure did not return a valid employee ID.");
        }
    }


    public async Task UpdateZamestnanec(ZAMESTNANEC zamestnanec)
    {
        using (var db = new OracleConnection(connectionString))
        {
            var sql = @"
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
            var sql = @"
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
            var sql = @"
                    SELECT 
                        ID_ZAMESTNANEC AS IdZamestnanec, 
                        JMENO AS Jmeno, 
                        PRIJMENI AS Prijmeni, 
                        TELEFON AS Telefon, 
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
            var sql = "DELETE FROM ZAMESTNANEC WHERE ID_ZAMESTNANEC = :Id";

            await db.ExecuteAsync(sql, new { Id = id });
        }
    }

    public async Task<ZAMESTNANEC> GetZamestnanecByUserDataId(int userDataId)
    {
        using (var db = new OracleConnection(connectionString))
        {
            var sql = @"
            SELECT 
                ID_ZAMESTNANEC AS IdZamestnanec, 
                JMENO AS Jmeno, 
                PRIJMENI AS Prijmeni, 
                TELEFON AS Telefon, 
                ZAMESTNANEC_ID_ZAMESTNANEC AS NadrazenyZamestnanecId, 
                ADRESA_ID_ADRESA AS AdresaId, 
                POZICE_ID_POZICE AS PoziceId, 
                UZIVATEL_DATA_ID_UZIVATEL_DATA AS UserDataId 
            FROM 
                ZAMESTNANEC 
            WHERE UZIVATEL_DATA_ID_UZIVATEL_DATA = :UserDataId";

            return await db.QueryFirstOrDefaultAsync<ZAMESTNANEC>(sql, new { UserDataId = userDataId });
        }
    }
}