using BDAS2_SEM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IBlobTableRepository
    {
        Task<int> AddBlob(BLOB_TABLE blob);
        Task UpdateBlob(BLOB_TABLE blob);
        Task DeleteBlob(int id);
        Task<BLOB_TABLE> GetBlobById(int id);
        Task<IEnumerable<BLOB_TABLE>> GetAllBlobTables();
    }
}