using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IDiagnozaRepository
    {
        Task<int> AddDiagnoza(DIAGNOZA diagnoza);
        Task UpdateDiagnoza(DIAGNOZA diagnoza);
        Task<DIAGNOZA> GetDiagnozaById(int id);
        Task<IEnumerable<DIAGNOZA>> GetAllDiagnoses();
        Task DeleteDiagnoza(int id);
    }
}
