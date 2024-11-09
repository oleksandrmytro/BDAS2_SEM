using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IKartaRepository
    {
        Task<int> AddKarta(KARTA karta);
        Task UpdateKarta(KARTA karta);
        Task<KARTA> GetKartaById(int id);
        Task<IEnumerable<KARTA>> GetAllKarty();
        Task DeleteKarta(int id);
    }
}
