using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IBlobTableRepository
    {
        Task<BLOB_TABLE> GetBlobContentByDoctorId(int doctorId);
        Task<int> AddBlobContent(BLOB_TABLE content);
        Task UpdateBlobContent(BLOB_TABLE content);
        Task DeleteBlobContent(int id);
        Task<BLOB_TABLE> GetBlobByZamestnanecId(int zamestnanecId);
        Task<IEnumerable<BLOB_TABLE>> GetAllBlobTables();
    }
}
