using BDAS2_SEM.Repository.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

public class DoctorInfoRepository : IDoctorInfoRepository
{
    private readonly string _connectionString;

    public DoctorInfoRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<DOCTOR_INFO>> GetAllDoctors()
    {
        var doctors = new List<DOCTOR_INFO>();

        using (var connection = new OracleConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new OracleCommand("SELECT * FROM DOCTOR_INFO_VIEW", connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var doctor = new DOCTOR_INFO
                        {
                            DoctorId = reader.GetInt32(0),
                            AvatarId = reader.GetInt32(1),
                            FirstName = reader.GetString(2),
                            Surname = reader.GetString(3),
                            Phone = reader.GetInt64(4),
                            Department = reader.GetString(5)
                        };

                        // Загрузка аватарки
                        if (doctor.AvatarId != 0)
                        {
                            using (var avatarCommand = new OracleCommand("SELECT OBSAH FROM BLOB_TABLE WHERE ID_BLOB = :BlobId", connection))
                            {
                                avatarCommand.Parameters.Add(new OracleParameter("BlobId", doctor.AvatarId));
                                var avatarData = await avatarCommand.ExecuteScalarAsync() as byte[];
                                doctor.Avatar = avatarData;
                            }
                        }

                        doctors.Add(doctor);
                    }
                }
            }
        }

        return doctors;
    }
}