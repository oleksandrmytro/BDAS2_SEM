using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class DiagnozaRepository : IDiagnozaRepository
    {
        private readonly string _connectionString;

        public DiagnozaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Метод для додавання нового діагнозу
        public async Task<int> AddDiagnoza(DIAGNOZA diagnoza)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_diagnoza", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_nazev", diagnoza.Nazev, DbType.String, ParameterDirection.Input);
                parameters.Add("p_popis", diagnoza.Popis, DbType.String, ParameterDirection.Input);

                await db.ExecuteAsync(
                    "manage_diagnoza",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                // Отримуємо згенерований ID
                diagnoza.IdDiagnoza = parameters.Get<int>("p_id_diagnoza");
                return diagnoza.IdDiagnoza;
            }
        }

        // Метод для оновлення існуючого діагнозу
        public async Task UpdateDiagnoza(DIAGNOZA diagnoza)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_diagnoza", diagnoza.IdDiagnoza, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_nazev", diagnoza.Nazev, DbType.String, ParameterDirection.Input);
                parameters.Add("p_popis", diagnoza.Popis, DbType.String, ParameterDirection.Input);

                await db.ExecuteAsync(
                    "manage_diagnoza",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<DIAGNOZA> GetDiagnozaById(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_DIAGNOZA AS IdDiagnoza, 
                           NAZEV AS Nazev, 
                           POPIS AS Popis 
                    FROM DIAGNOZA 
                    WHERE ID_DIAGNOZA = :Id";

                return await db.QueryFirstOrDefaultAsync<DIAGNOZA>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<DIAGNOZA>> GetAllDiagnozy()
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = @"
                    SELECT ID_DIAGNOZA AS IdDiagnoza, 
                           NAZEV AS Nazev, 
                           POPIS AS Popis 
                    FROM DIAGNOZA";

                return await db.QueryAsync<DIAGNOZA>(sql);
            }
        }

        public async Task DeleteDiagnoza(int id)
        {
            using (var db = new OracleConnection(_connectionString))
            {
                string sql = "DELETE FROM DIAGNOZA WHERE ID_DIAGNOZA = :Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}