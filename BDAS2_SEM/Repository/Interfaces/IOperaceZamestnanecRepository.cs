using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IOperaceZamestnanecRepository
    {
        Task AddOperaceZamestnanec(OPERACE_ZAMESTNANEC operaceZamestnanec);
        Task<OPERACE_ZAMESTNANEC> GetOperaceZamestnanec(int operaceId, int zamestnanecId);
        Task<IEnumerable<OPERACE_ZAMESTNANEC>> GetAllOperaceZamestnanecs();
        Task DeleteOperaceZamestnanec(int operaceId, int zamestnanecId);
    }
}
