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

        public async Task<int> AddAdresa(ADRESA adresa)
        {
            using (var db = new OracleConnection(this.connection))
            {
                var procedureName = "manage_adresa";

                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_adresa", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_stat", adresa.Stat, DbType.String, ParameterDirection.Input);
                parameters.Add("p_mesto", adresa.Mesto, DbType.String, ParameterDirection.Input);
                parameters.Add("p_psc", adresa.PSC, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_ulice", adresa.Ulice, DbType.String, ParameterDirection.Input);
                parameters.Add("p_cislo_popisne", adresa.CisloPopisne, DbType.Int32, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                    int newIdAdresa = parameters.Get<int>("p_id_adresa");

                    return newIdAdresa;
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

        public async Task UpdateAdresa(int id, ADRESA adresa)
        {
            using (var db = new OracleConnection(this.connection))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE",DbType.String);
                parameters.Add("p_id_adresa", id, DbType.Int32);
                parameters.Add("p_stat", adresa.Stat, DbType.String);
                parameters.Add("p_mesto", adresa.Mesto, DbType.String);
                parameters.Add("p_psc", adresa.PSC, DbType.Int32);
                parameters.Add("p_ulice", adresa.Ulice, DbType.String);
                parameters.Add("p_cislo_popisne", adresa.CisloPopisne, DbType.Int32);

                await db.ExecuteAsync("manage_adresa", parameters, commandType: CommandType.StoredProcedure);
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
