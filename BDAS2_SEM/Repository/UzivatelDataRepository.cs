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
                        ID_UZIVATEL_DATA AS Id, 
                        EMAIL AS Email,
                        HESLO AS Heslo,
                        ROLE_ID_ROLE AS RoleUzivatel,
                        ZAMESTNANEC_ID_C AS ZamestnanecId
                    FROM 
                        UZIVATEL_DATA 
                    WHERE 
                        EMAIL = :Email AND HESLO = :Heslo";

                var user = await db.QueryFirstOrDefaultAsync<UZIVATEL_DATA>(sql, new { Email = email, Heslo = heslo });

                return user;
            }
        }

        public async Task<int> RegisterNewUserData(string email, string heslo)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            INSERT INTO UZIVATEL_DATA 
                (ID_UZIVATEL_DATA, EMAIL, HESLO, PACIENT_ID_C, ZAMESTNANEC_ID_C, ROLE_ID_ROLE) 
            VALUES 
                (uzivatel_data_seq.NEXTVAL, :Email, :Heslo, NULL, NULL, 1) 
            RETURNING ID_UZIVATEL_DATA INTO :Id";

                var parameters = new DynamicParameters();
                parameters.Add("Email", email, DbType.String);
                parameters.Add("Heslo", heslo, DbType.String);
                parameters.Add("Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                try
                {
                    await db.ExecuteAsync(sql, parameters);
                    return parameters.Get<int>("Id");
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException ex) when (ex.Number == 1)
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
        public async Task<IEnumerable<UZIVATEL_DATA>> GetUsersWithUndefinedRole()
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            SELECT 
                ID_UZIVATEL_DATA AS Id, 
                EMAIL AS Email, 
                HESLO AS Heslo, 
                ROLE_ID_ROLE AS RoleUzivatel 
            FROM 
                UZIVATEL_DATA 
            WHERE 
                ROLE_ID_ROLE = :UndefinedRole";

                var parameters = new { UndefinedRole = (int)Role.NEOVERENY };
                var users = await db.QueryAsync<UZIVATEL_DATA>(sql, parameters);
                return users;
            }
        }

        public async Task UpdateUserRole(int userId, Role newRole)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
            UPDATE UZIVATEL_DATA 
            SET ROLE_ID_ROLE = :RoleId 
            WHERE ID_UZIVATEL_DATA = :UserId";

                var parameters = new DynamicParameters();
                parameters.Add("RoleId", (int)newRole, DbType.Int32);
                parameters.Add("UserId", userId, DbType.Int32);

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

        public async Task<UZIVATEL_DATA> GetUserByEmailAsync(string email)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_UZIVATEL_DATA AS Id, 
                        EMAIL AS Email, 
                        ROLE_ID_ROLE AS RoleUzivatel 
                    FROM 
                        UZIVATEL_DATA 
                    WHERE 
                        LOWER(EMAIL) = LOWER(:Email)";

                return await db.QueryFirstOrDefaultAsync<UZIVATEL_DATA>(sql, new { Email = email.Trim() });
            }
        }

        public async Task UpdateUserData(UZIVATEL_DATA userData)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    UPDATE UZIVATEL_DATA 
                    SET 
                        EMAIL = :Email, 
                        HESLO = :Heslo, 
                        PACIENT_ID_C = :PacientId, 
                        ZAMESTNANEC_ID_C = :ZamestnanecId, 
                        ROLE_ID_ROLE = :RoleUzivatel 
                    WHERE 
                        ID_UZIVATEL_DATA = :Id";

                var parameters = new DynamicParameters();
                parameters.Add("Email", userData.Email, DbType.String);
                parameters.Add("Heslo", userData.Heslo, DbType.String);
                parameters.Add("PacientId", userData.pacientId, DbType.Int32);
                parameters.Add("ZamestnanecId", userData.zamestnanecId, DbType.Int32);
                parameters.Add("RoleUzivatel", (int)userData.RoleUzivatel, DbType.Int32);
                parameters.Add("Id", userData.Id, DbType.Int32);

                await db.ExecuteAsync(sql, parameters);
            }
        }

    }
}
