using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IZamestnanecNavstevaRepository
    {
        Task<IEnumerable<ZAMESTNANEC_NAVSTEVA>> GetAllZamestnanecNavstevas();
        Task<ZAMESTNANEC_NAVSTEVA> GetZamestnanecNavstevaById(int zamestnanecId, int navstevaId);
        Task InsertZamestnanecNavsteva(ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva);
        Task UpdateZamestnanecNavsteva(ZAMESTNANEC_NAVSTEVA zamestnanecNavsteva);
        Task<ZAMESTNANEC_NAVSTEVA> GetZamestnanecNavstevaByNavstevaId(int navstevaId); // Новый метод
        Task DeleteZamestnanecNavsteva(int zamestnanecId, int navstevaId);
    }
}
