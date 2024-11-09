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
    public class OperaceRepository : IOperaceRepository
    {
        private readonly string connectionString;

        public OperaceRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddOperace(OPERACE operace)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_nazev", operace.Nazev, DbType.String);
                parameters.Add("p_datum", operace.Datum, DbType.Date);
                parameters.Add("p_diagnoza_id", operace.DiagnozaId, DbType.Int32);
                parameters.Add("p_id_operace", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync("INSERT_OPERACE", parameters, commandType: CommandType.StoredProcedure);

                return parameters.Get<int>("p_id_operace");
            }
        }

        public async Task UpdateOperace(OPERACE operace)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_operace", operace.IdOperace, DbType.Int32);
                parameters.Add("p_nazev", operace.Nazev, DbType.String);
                parameters.Add("p_datum", operace.Datum, DbType.Date);
                parameters.Add("p_diagnoza_id", operace.DiagnozaId, DbType.Int32);

                await db.ExecuteAsync("UPDATE_OPERACE", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<OPERACE> GetOperaceById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = "SELECT ID_OPERACE AS IdOperace, NAZEV AS Nazev, DATUM AS Datum, DIAGNOZA_ID AS DiagnozaId FROM OPERACE WHERE ID_OPERACE = :id";
                return await db.QueryFirstOrDefaultAsync<OPERACE>(query, new { id });
            }
        }

        public async Task<IEnumerable<OPERACE>> GetAllOperace()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = "SELECT ID_OPERACE AS IdOperace, NAZEV AS Nazev, DATUM AS Datum, DIAGNOZA_ID AS DiagnozaId FROM OPERACE";
                return await db.QueryAsync<OPERACE>(query);
            }
        }

        public async Task DeleteOperace(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_operace", id, DbType.Int32);

                await db.ExecuteAsync("DELETE_OPERACE", parameters, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
