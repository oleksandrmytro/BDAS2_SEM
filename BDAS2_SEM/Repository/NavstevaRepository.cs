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
                parameters.Add("p_pacient_id", navsteva.PacientId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_status_id", navsteva.StatusId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("p_mistnost_id", navsteva.MistnostId, DbType.Int32, ParameterDirection.Input);

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
                parameters.Add("p_pacient_id", navsteva.PacientId, DbType.Int32);
                parameters.Add("p_status_id", navsteva.StatusId, DbType.Int32); // Add status parameter
                parameters.Add("p_mistnost_id", navsteva.MistnostId, DbType.Int32, ParameterDirection.Input);

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

        public async Task<IEnumerable<NAVSTEVA>> GetAppointmentsByDoctorId(int doctorId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"
            SELECT 
                n.ID_NAVSTEVA AS IdNavsteva,
                n.PACIENT_ID_PACIENT AS PacientId,
                n.STATUS_ID_STATUS AS StatusId,
                n.DATUM AS Datum,
                n.MISTNOST_ID AS MistnostId
            FROM NAVSTEVA n
            JOIN ZAMESTNANEC_NAVSTEVA zn ON n.ID_NAVSTEVA = zn.NAVSTEVA_ID_NAVSTEVA
            WHERE zn.ZAMESTNANEC_ID_ZAMESTNANEC = :doctorId
        ";

                var parameters = new { doctorId };
                var result = await db.QueryAsync<NAVSTEVA>(query, parameters);
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

        public async Task<IEnumerable<NAVSTEVA>> GetAppointmentsByDoctorIdDateAndRoom(int doctorId, DateTime date, int room)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"
                    SELECT n.*
                    FROM NAVSTEVA n
                    JOIN ZAMESTNANEC_NAVSTEVA zn ON n.ID_NAVSTEVA = zn.NAVSTEVA_ID_NAVSTEVA
                    WHERE zn.ZAMESTNANEC_ID_ZAMESTNANEC = :doctorId
                    AND TRUNC(n.DATUM) = :selectedDate
                    AND n.MISTNOST = :room
                    AND n.STATUS_ID_STATUS IN (:accepted)
                ";

                var parameters = new
                {
                    doctorId,
                    selectedDate = date.Date,
                    room,
                    accepted = 1
                };

                var result = await db.QueryAsync<NAVSTEVA>(query, parameters);
                return result;
            }
        }


        public async Task<bool> IsTimeSlotAvailable(int doctorId, DateTime dateTime, int room, int? excludeAppointmentId = null)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"
                    SELECT COUNT(*)
                    FROM NAVSTEVA n
                    JOIN ZAMESTNANEC_NAVSTEVA zn ON n.ID_NAVSTEVA = zn.NAVSTEVA_ID_NAVSTEVA
                    WHERE zn.ZAMESTNANEC_ID_ZAMESTNANEC = :doctorId
                    AND n.DATUM = :dateTime
                    AND n.MISTNOST_ID = :room
                    AND n.STATUS_ID_STATUS IN (:accepted)
                    AND (:excludeId IS NULL OR n.ID_NAVSTEVA != :excludeId)
                ";

                var parameters = new
                {
                    doctorId,
                    dateTime,
                    room,
                    accepted = 1,
                    excludeId = excludeAppointmentId
                };

                var count = await db.ExecuteScalarAsync<int>(query, parameters);
                return count == 0;
            }
        }
        public async Task<List<(int Room, string TimeSlot)>> GetAvailableRoomsAndTimes(int ordinaceId, DateTime date)
        {
            var results = new List<(int Room, string TimeSlot)>();

            using (var db = new OracleConnection(connectionString))
            using (var cmd = db.CreateCommand())
            {
                cmd.CommandText = "GetAvailableRoomsAndTimes";
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("p_ordinace_id", OracleDbType.Int32).Value = ordinaceId;
                cmd.Parameters.Add("p_date", OracleDbType.Date).Value = date;
                cmd.Parameters.Add("p_available_rooms_times", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                await db.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int room = reader.GetInt32(0);
                        string timeSlot = reader.GetString(1);
                        results.Add((room, timeSlot));
                    }
                }
            }

            return results;
        }

        public async Task<(string FirstName, string LastName)> GetPatientNameByAppointmentId(int appointmentId)
        {
            using (var db = new OracleConnection(connectionString))
            {
                var query = @"
            SELECT 
                p.JMENO AS FirstName,
                p.PRIJMENI AS LastName
            FROM NAVSTEVA n
            JOIN PACIENT p ON n.PACIENT_ID_PACIENT = p.ID_PACIENT
            WHERE n.ID_NAVSTEVA = :appointmentId";
                var result = await db.QueryFirstOrDefaultAsync<(string FirstName, string LastName)>(query, new { appointmentId });
                return result;
            }
        }

        public async Task<IEnumerable<NAVSTEVA>> GetAllNavstevas()
        {
            using (var db = new OracleConnection(connectionString))
            {
                var sqlQuery = @"
                    SELECT id_navsteva AS IdNavsteva,
                           datum AS Datum,
                           pacient_id_pacient AS PacientId,
                           status_id_status AS Status,
                           MISTNOST_ID As MistnostId
                    FROM NAVSTEVA";

                return await db.QueryAsync<NAVSTEVA>(sqlQuery);
            }
        }

    }
}