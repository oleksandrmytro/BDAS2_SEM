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
                        // Используем IsDBNull() перед чтением каждого столбца
                        var doctorId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                        var avatarId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                        var firstName = reader.IsDBNull(2) ? null : reader.GetString(2);
                        var surname = reader.IsDBNull(3) ? null : reader.GetString(3);
                        var phone = reader.IsDBNull(4) ? 0L : reader.GetInt64(4);
                        var department = reader.IsDBNull(5) ? null : reader.GetString(5);

                        var doctor = new DOCTOR_INFO
                        {
                            DoctorId = doctorId,
                            AvatarId = avatarId,
                            FirstName = firstName,
                            Surname = surname,
                            Phone = phone,
                            Department = department
                        };

                        // Загрузка аватарки, если есть AvatarId
                        if (doctor.AvatarId != 0)
                        {
                            using (var avatarCommand = new OracleCommand("SELECT OBSAH FROM BLOB_TABLE WHERE ID_BLOB = :BlobId", connection))
                            {
                                avatarCommand.Parameters.Add(new OracleParameter("BlobId", doctor.AvatarId));
                                var avatarData = await avatarCommand.ExecuteScalarAsync() as byte[];

                                if (avatarData != null && avatarData.Length > 0)
                                {
                                    doctor.Avatar = avatarData;
                                }
                                else
                                {
                                    // Если нет данных или пусто - аватарка не устанавливается (останется null)
                                }
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
