using Dapper;
using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace BDAS2_SEM.Repository
{
    public class PlatbaRepository : IPlatbaRepository
    {
        private readonly string connectionString;

        public PlatbaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddPlatba(PLATBA platba)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var procedureName = "manage_payment";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_platba", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_castka", platba.Castka, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_datum", platba.Datum, DbType.Date, ParameterDirection.Input);
                parameters.Add("p_typ_platby", platba.TypPlatby, DbType.String, ParameterDirection.Input);
                parameters.Add("p_navsteva_id", platba.NavstevaId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_cislo_karty", platba.CisloKarty, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_prijato", platba.Prijato, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_vraceno", platba.Vraceno, DbType.Decimal, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
                    return parameters.Get<int>("p_id_platba");
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


        public async Task UpdatePlatba(PLATBA platba)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var procedureName = "manage_payment";
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_platba", platba.IdPlatba, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_castka", platba.Castka, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_datum", platba.Datum, DbType.Date, ParameterDirection.Input);
                parameters.Add("p_typ_platby", platba.TypPlatby, DbType.String, ParameterDirection.Input);
                parameters.Add("p_navsteva_id", platba.NavstevaId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_cislo_karty", platba.CisloKarty, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_prijato", platba.Prijato, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_vraceno", platba.Vraceno, DbType.Decimal, ParameterDirection.Input);

                await db.OpenAsync();

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
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


        public async Task<PLATBA> GetPlatbaById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_PLATBA AS IdPlatba, 
                        CASTKA AS Castka, 
                        DATUM AS Datum, 
                        TYP_PLATBY AS TypPlatby, 
                        NAVSTEVA_ID AS NavstevaId 
                    FROM 
                        PLATBA 
                    WHERE 
                        ID_PLATBA = :Id";

                return await db.QueryFirstOrDefaultAsync<PLATBA>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<PLATBA>> GetAllPlatbas()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT id_platba AS IdPlatba,
                           castka AS Castka,
                           datum AS Datum,
                           typ_platby AS TypPlatby,
                           navsteva_id_navsteva AS NavstevaId
                    FROM PLATBA";

                return await db.QueryAsync<PLATBA>(sqlQuery);
            }
        }

        public async Task DeletePlatba(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM PLATBA WHERE ID_PLATBA = :Id";

                await db.ExecuteAsync(sql, new { Id = id });
            }
        }

        public async Task<int> ManagePaymentAsync(
            string action,
            int? idPlatba,
            decimal castka,
            DateTime datum,
            string typPlatby,
            int navstevaId,
            long? cisloKarty = null,
            decimal? prijato = null,
            decimal? vraceno = null)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", action, DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_platba", idPlatba, DbType.Int32, ParameterDirection.InputOutput);
                parameters.Add("p_castka", castka, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_datum", datum, DbType.Date, ParameterDirection.Input);
                parameters.Add("p_typ_platby", typPlatby, DbType.String, ParameterDirection.Input);
                parameters.Add("p_navsteva_id", navstevaId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_cislo_karty", cisloKarty, DbType.Int64, ParameterDirection.Input);
                parameters.Add("p_prijato", prijato, DbType.Decimal, ParameterDirection.Input);
                parameters.Add("p_vraceno", vraceno, DbType.Decimal, ParameterDirection.Input);

                await db.ExecuteAsync("manage_payment", parameters, commandType: CommandType.StoredProcedure);

                // Отримання значення OUT параметра p_id_platba
                var generatedId = parameters.Get<int?>("p_id_platba");
                if (generatedId.HasValue)
                {
                    return generatedId.Value;
                }
                else
                {
                    throw new Exception("Не вдалося отримати ID платежу.");
                }
            }
        }
    }
}
