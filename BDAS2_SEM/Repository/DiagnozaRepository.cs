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
    public class DiagnozaRepository : IDiagnozaRepository
    {
        private readonly string connectionString;

        public DiagnozaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddDiagnoza(DIAGNOZA diagnoza)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    INSERT INTO DIAGNOZA (NAZEV, POPIS) 
                    VALUES (:Nazev, :Popis) 
                    RETURNING ID_DIAGNOZA INTO :IdDiagnoza";

                var parameters = new DynamicParameters();
                parameters.Add("Nazev", diagnoza.Nazev, DbType.String);
                parameters.Add("Popis", diagnoza.Popis, DbType.String);
                parameters.Add("IdDiagnoza", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await db.ExecuteAsync(sql, parameters);

                return parameters.Get<int>("IdDiagnoza");
            }
        }

        public async Task UpdateDiagnoza(DIAGNOZA diagnoza)
        {
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = @"
                    UPDATE DIAGNOZA 
                    SET NAZEV = :Nazev, POPIS = :Popis 
                    WHERE ID_DIAGNOZA = :IdDiagnoza";

                var parameters = new
                {
                    Nazev = diagnoza.Nazev,
                    Popis = diagnoza.Popis,
                    IdDiagnoza = diagnoza.IdDiagnoza
                };

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<DIAGNOZA> GetDiagnozaById(int id)
        {
            using (var db = new OracleConnection(this.connectionString))
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
            using (var db = new OracleConnection(this.connectionString))
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
            using (var db = new OracleConnection(this.connectionString))
            {
                string sql = "DELETE FROM DIAGNOZA WHERE ID_DIAGNOZA = :Id";
                await db.ExecuteAsync(sql, new { Id = id });
            }
        }
    }
}
