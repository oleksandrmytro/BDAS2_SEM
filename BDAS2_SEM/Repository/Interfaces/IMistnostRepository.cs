using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IMistnostRepository
    {
        Task<int> AddMistnost(MISTNOST mistnost);
        Task UpdateMistnost(MISTNOST mistnost);
        Task<MISTNOST> GetMistnostById(int? id);
        Task<MISTNOST> GetMistnostByNumber(int roomNumber);
        Task<IEnumerable<MISTNOST>> GetAllMistnosti();
        Task DeleteMistnost(int id);
    }
}
