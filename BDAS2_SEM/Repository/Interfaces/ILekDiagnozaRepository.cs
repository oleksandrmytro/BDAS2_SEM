using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface ILekDiagnozaFKRepository
    {
        Task AddLekDiagnoza(LEK_DIAGNOZA_FK lekDiagnoza);
        Task UpdateLekDiagnoza(LEK_DIAGNOZA_FK lekDiagnoza);
        Task<LEK_DIAGNOZA_FK> GetLekDiagnoza(int diagnozaId, int lekId);
        Task<IEnumerable<LEK_DIAGNOZA_FK>> GetAllLekDiagnoza();
        Task DeleteLekDiagnoza(int diagnozaId, int lekId);
    }
}
