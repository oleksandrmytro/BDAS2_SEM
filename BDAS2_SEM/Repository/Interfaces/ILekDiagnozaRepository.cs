using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface ILekDiagnozaRepository
    {
        Task AddLekDiagnoza(LEK_DIAGNOZA lekDiagnoza);
        Task UpdateLekDiagnoza(LEK_DIAGNOZA lekDiagnoza);
        Task<LEK_DIAGNOZA> GetLekDiagnoza(int diagnozaId, int lekId);
        Task<IEnumerable<LEK_DIAGNOZA>> GetAllLekDiagnoza();
        Task DeleteLekDiagnoza(int diagnozaId, int lekId);
    }
}
