using BDAS2_SEM.Model.Enum;
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
    public class UzivatelDataRepository : IUzivatelDataRepository
    {
        private readonly string connectionString;

        public UzivatelDataRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<UZIVATEL_DATA> CheckCredentials(string email, string heslo)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID AS Id, 
                        EMAIL AS Email, 
                        ROLE AS RoleUzivatel 
                    FROM 
                        UZIVATEL_DATA 
                    WHERE 
                        EMAIL = :Email AND HESLO = :Heslo";

                var user = await db.QueryFirstOrDefaultAsync<UZIVATEL_DATA>(sql, new { Email = email, Heslo = heslo });

                return user;
            }
        }

        public async Task<int> RegisterNewUserData(string email, string heslo, Role role)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO UZIVATEL_DATA (EMAIL, HESLO, ROLE) 
                    VALUES (:Email, :Heslo, :Role) 
                    RETURNING ID INTO :Id";

                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                parameters.Add("Heslo", heslo, DbType.String);
                parameters.Add("Role", (int)role, DbType.Int32);
                parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                try
                {
                    await db.ExecuteAsync(sql, parameters);
                    return parameters.Get<int>("Id");
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException ex) when (ex.Number == 1) // Unique constraint violation
                {
                    throw new Exception("User already exists.");
                }
            }
        }

        public async Task UpdateUserEmail(int id, string newEmail)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE UZIVATEL_DATA 
                    SET EMAIL = :NewEmail 
                    WHERE ID = :Id";

                var parameters = new DynamicParameters();
                parameters.Add("NewEmail", newEmail, DbType.String);
                parameters.Add("Id", id, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateUserPassword(int id, string newPassword)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE UZIVATEL_DATA 
                    SET HESLO = :NewPassword 
                    WHERE ID = :Id";

                var parameters = new DynamicParameters();
                parameters.Add("NewPassword", newPassword, DbType.String);
                parameters.Add("Id", id, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<UZIVATEL_DATA> GetUzivatelById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID AS Id, 
                        EMAIL AS Email, 
                        ROLE AS RoleUzivatel 
                    FROM 
                        UZIVATEL_DATA 
                    WHERE 
                        ID = :Id";

                return await db.QueryFirstOrDefaultAsync<UZIVATEL_DATA>(sql, new { Id = id });
            }
        }
    }
}
