using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface ILekRepository
    {
        Task<int> AddLek(LEK lek);
        Task UpdateLek(LEK lek);
        Task<LEK> GetLekById(int id);
        Task<IEnumerable<LEK>> GetAllLeks();
        Task DeleteLek(int id);
    }
}
