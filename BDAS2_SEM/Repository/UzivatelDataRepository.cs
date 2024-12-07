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
                string procedureName = "manage_uzivatel_data";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "INSERT", DbType.String);
                parameters.Add("p_id_uzivatel_data", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_email", email, DbType.String);
                parameters.Add("p_heslo", heslo, DbType.String);
                parameters.Add("p_pacient_id_c", dbType: DbType.Int32, value: null);
                parameters.Add("p_zamestnanec_id_c", dbType: DbType.Int32, value: null);
                parameters.Add("p_role_id_role", 1, DbType.Int32);

                try
                {
                    await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);

                    var userId = parameters.Get<int?>("p_id_uzivatel_data");

                    if (userId.HasValue)
                    {
                        return userId.Value;
                    }
                    else
                    {
                        throw new Exception("The procedure did not return a valid user ID.");
                    }
                }
                catch (Oracle.ManagedDataAccess.Client.OracleException ex) when (ex.Number == 1)
                {
                    throw new Exception("User already exists.");
                }
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

        public async Task<UZIVATEL_DATA> GetUzivatelById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT 
                        ID_UZIVATEL_DATA AS Id, 
                        EMAIL AS Email, 
                        heslo as Heslo,
                        PACIENT_ID_C as pacientId,
                        ZAMESTNANEC_ID_C as zamestnanecId,
                        ROLE_ID_ROLE AS RoleUzivatel 
                    FROM 
                        UZIVATEL_DATA 
                    WHERE 
                        ID_UZIVATEL_DATA = :Id";

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
                string procedureName = "manage_uzivatel_data";

                var parameters = new DynamicParameters();
                parameters.Add("p_action", "UPDATE", DbType.String);
                parameters.Add("p_id_uzivatel_data", userData.Id, DbType.Int32);
                parameters.Add("p_email", userData.Email, DbType.String);
                parameters.Add("p_heslo", userData.Heslo, DbType.String);
                parameters.Add("p_pacient_id_c", userData.pacientId, DbType.Int32);
                parameters.Add("p_zamestnanec_id_c", userData.zamestnanecId, DbType.Int32);
                parameters.Add("p_role_id_role", (int?)userData.RoleUzivatel, DbType.Int32);

                await db.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<IEnumerable<UZIVATEL_DATA>> GetAllUzivatelDatas()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT ID_UZIVATEL_DATA AS Id,
                           email AS Email,
                           heslo AS Heslo,
                           pacient_id_c AS pacientId,
                           zamestnanec_id_c AS zamestnanecId,
                           ROLE_ID_ROLE AS RoleUzivatel
                    FROM UZIVATEL_DATA";

                return await db.QueryAsync<UZIVATEL_DATA>(sqlQuery);
            }
        }

        public async Task DeleteUzivatelData(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = "DELETE FROM UZIVATEL_DATA WHERE ID_UZIVATEL_DATA = :Id";
                await db.ExecuteAsync(sqlQuery, new { Id = id });
            }
        }

    }
}
