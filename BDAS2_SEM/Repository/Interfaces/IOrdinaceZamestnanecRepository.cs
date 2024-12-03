using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IOrdinaceZamestnanecRepository
    {
        Task AddOrdinaceZamestnanec(ORDINACE_ZAMESTNANEC ordinaceZamestnanec);

        Task UpdateOrdinaceZamestnanec(ORDINACE_ZAMESTNANEC ordinaceZamestnanec);
        Task<ORDINACE_ZAMESTNANEC> GetOrdinaceZamestnanecByZamestnanecId(int zamestnanecId);
        Task<IEnumerable<ORDINACE_ZAMESTNANEC>> GetAllOrdinaceZamestnanecs();
        Task DeleteOrdinaceZamestnanec(int ordinaceId, int zamestnanecId);
    }
}
