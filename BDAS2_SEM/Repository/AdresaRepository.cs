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
    public class AdresaRepository : IAdresaRepository
    {
        private readonly string connection;

        public AdresaRepository(string connection)
        {
            this.connection = connection;
        }

        public async Task<int> AddNewAdresa(ADRESA adresa)
        {
            using (var db = new OracleConnection(this.connection))
            {
                var sqlQuery = @"
                    INSERT INTO ADRESA (ID_ADRESA, STAT, MESTO, PSC, ULICE, CISLO_POPISNE) 
                    VALUES (ADRESA_SEQ.NEXTVAL, :Stat, :Mesto, :PSC, :Ulice, :CisloPopisne) 
                    RETURNING ID_ADRESA INTO :IdAdresa";

                var parameters = new DynamicParameters();
                parameters.Add("Stat", adresa.Stat, DbType.String);
                parameters.Add("Mesto", adresa.Mesto, DbType.String);
                parameters.Add("PSC", adresa.PSC, DbType.Int32);
                parameters.Add("Ulice", adresa.Ulice, DbType.String);
                parameters.Add("CisloPopisne", adresa.CisloPopisne, DbType.Int32);
                parameters.Add("IdAdresa", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sqlQuery, parameters);

                return parameters.Get<int>("IdAdresa");
            }
        }

        public async Task UpdateAdresa(int id, ADRESA adresa)
        {
            using (var db = new OracleConnection(this.connection))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_adresa", id, DbType.Int32);
                parameters.Add("p_stat", adresa.Stat, DbType.String);
                parameters.Add("p_mesto", adresa.Mesto, DbType.String);
                parameters.Add("p_psc", adresa.PSC, DbType.Int32);
                parameters.Add("p_ulice", adresa.Ulice, DbType.String);
                parameters.Add("p_cislo_popisne", adresa.CisloPopisne, DbType.Int32);

                await db.ExecuteAsync("UPDATE_ADRESA", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<ADRESA> GetAdresaById(int id)
        {
            using (var db = new OracleConnection(this.connection))
            {
                var sqlQuery = @"
                    SELECT id_adresa AS IdAdresa, 
                           stat AS Stat, 
                           mesto AS Mesto, 
                           psc AS PSC, 
                           ulice AS Ulice, 
                           cislo_popisne AS CisloPopisne 
                    FROM ADRESA 
                    WHERE id_adresa = :Id";

                return await db.QueryFirstOrDefaultAsync<ADRESA>(sqlQuery, new { Id = id });
            }
        }

        public async Task<IEnumerable<ADRESA>> GetAllAddresses()
        {
            using (var db = new OracleConnection(this.connection))
            {
                var sqlQuery = @"
                    SELECT id_adresa AS IdAdresa, 
                           stat AS Stat, 
                           mesto AS Mesto, 
                           psc AS PSC, 
                           ulice AS Ulice, 
                           cislo_popisne AS CisloPopisne 
                    FROM ADRESA";

                return await db.QueryAsync<ADRESA>(sqlQuery);
            }
        }

        public async Task DeleteAdresa(int id)
        {
            using (var db = new OracleConnection(this.connection))
            {
                var sqlQuery = "DELETE FROM ADRESA WHERE ID_ADRESA = :Id";
                await db.ExecuteAsync(sqlQuery, new { Id = id });
            }
        }
    }
}
