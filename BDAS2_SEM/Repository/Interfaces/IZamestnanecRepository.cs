using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IZamestnanecRepository
    {
        Task<int> AddZamestnanec(ZAMESTNANEC zamestnanec);
        Task UpdateZamestnanec(ZAMESTNANEC zamestnanec);
        Task<ZAMESTNANEC> GetZamestnanecById(int id);
        Task<IEnumerable<ZAMESTNANEC>> GetAllZamestnanci();
        Task DeleteZamestnanec(int id);
    }
}
