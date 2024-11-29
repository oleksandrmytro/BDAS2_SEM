using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum; // Include the Status enum
using BDAS2_SEM.Repository.Interfaces;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class NavstevaRepository : INavstevaRepository
    {
        private readonly string connectionString;

        public NavstevaRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<int> AddNavsteva(NAVSTEVA navsteva)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "INSERT", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_navsteva", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("p_datum", navsteva.Datum, DbType.DateTime, ParameterDirection.Input);
                parameters.Add("p_mistnost", navsteva.Mistnost, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_pacient_id", navsteva.PacientId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_status_id", (int)navsteva.Status, DbType.Int32, ParameterDirection.Input);

                await db.ExecuteAsync("manage_navsteva", parameters, commandType: CommandType.StoredProcedure);

                int newId = parameters.Get<int>("p_id_navsteva");
                return newId;
            }
        }

        public async Task UpdateNavsteva(NAVSTEVA navsteva)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();

                parameters.Add("p_action", "UPDATE", DbType.String, ParameterDirection.Input);
                parameters.Add("p_id_navsteva", navsteva.IdNavsteva, DbType.Int32);
                parameters.Add("p_datum", navsteva.Datum, DbType.Date);
                parameters.Add("p_mistnost", navsteva.Mistnost, DbType.Int32);
                parameters.Add("p_pacient_id", navsteva.PacientId, DbType.Int32);
                parameters.Add("p_status_id", (int)navsteva.Status, DbType.Int32); // Add status parameter

                await db.ExecuteAsync("manage_navsteva", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<NAVSTEVA> GetNavstevaById(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"SELECT 
                                ID_NAVSTEVA AS IdNavsteva,
                                DATUM AS Datum,
                                CAS AS Cas,
                                MISTNOST AS Mistnost,
                                PACIENT_ID_PACIENT AS PacientId,
                                STATUS_ID_STATUS AS Status
                              FROM NAVSTEVA WHERE ID_NAVSTEVA = :id";
                return await db.QueryFirstOrDefaultAsync<NAVSTEVA>(query, new { id });
            }
        }

        public async Task<IEnumerable<NAVSTEVA>> GetAllNavstevy()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"SELECT 
                                ID_NAVSTEVA AS IdNavsteva,
                                DATUM AS Datum,
                                MISTNOST AS Mistnost,
                                PACIENT_ID_PACIENT AS PacientId,
                                STATUS_ID_STATUS AS Status
                              FROM NAVSTEVA";
                return await db.QueryAsync<NAVSTEVA>(query);
            }
        }

        public async Task<IEnumerable<NAVSTEVA>> GetFutureNavstevy()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"SELECT 
                        ID_NAVSTEVA AS IdNavsteva,
                        DATUM AS Datum,
                        CAS AS Cas,
                        MISTNOST AS Mistnost,
                        PACIENT_ID_PACIENT AS PacientId,
                        STATUS_ID_STATUS AS Status
                      FROM NAVSTEVA
                      WHERE DATUM >= :currentDate";
                return await db.QueryAsync<NAVSTEVA>(query, new { currentDate = DateTime.Now });
            }
        }

        public async Task DeleteNavsteva(int id)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_id_navsteva", id, DbType.Int32);

                await db.ExecuteAsync("DELETE_NAVSTEVA", parameters, commandType: CommandType.StoredProcedure);
            }
        }

        // NavstevaRepository.cs
        public async Task<IEnumerable<dynamic>> GetAppointmentsByDoctorId(int doctorId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"
            SELECT 
                n.ID_NAVSTEVA AS IdNavsteva,
                n.PACIENT_ID_PACIENT AS PacientId,
                n.STATUS_ID_STATUS AS Status,
                p.JMENO AS PacientJmeno,
                p.PRIJMENI AS PacientPrijmeni
            FROM NAVSTEVA n
            JOIN PACIENT p ON n.PACIENT_ID_PACIENT = p.ID_PACIENT
            WHERE n.STATUS_ID_STATUS = :pendingStatus
            AND n.ID_NAVSTEVA IN (
                SELECT NAVSTEVA_ID_NAVSTEVA FROM ZAMESTNANEC_NAVSTEVA WHERE ZAMESTNANEC_ID_ZAMESTNANEC = :doctorId
            )
        ";

                var parameters = new { doctorId, pendingStatus = (int)Status.Pending };
                var result = await db.QueryAsync(query, parameters);
                return result;
            }
        }


        public async Task<bool> ExistsNavstevaForDoctorAndPatientAsync(int doctorId, int patientId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                string sql = @"
                    SELECT COUNT(*) FROM NAVSTEVA N
                    INNER JOIN ZAMESTNANEC_NAVSTEVA ZN ON N.ID_NAVSTEVA = ZN.NAVSTEVA_ID_NAVSTEVA
                    WHERE ZN.ZAMESTNANEC_ID = :DoctorId
                    AND N.PACIENT_ID_PACIENT = :PatientId";

                int count = await db.ExecuteScalarAsync<int>(sql, new { DoctorId = doctorId, PatientId = patientId });
                return count > 0;
            }
        }
    }
}