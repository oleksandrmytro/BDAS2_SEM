using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface INavstevaDiagnozaRepository
    {
        Task AddNavstevaDiagnoza(NAVSTEVA_DIAGNOZA navstevaDiagnoza);
        Task<NAVSTEVA_DIAGNOZA> GetNavstevaDiagnoza(int navstevaId, int diagnozaId);
        Task<IEnumerable<NAVSTEVA_DIAGNOZA>> GetAllNavstevaDiagnoza();
        Task DeleteNavstevaDiagnoza(int navstevaId, int diagnozaId);
    }
}
