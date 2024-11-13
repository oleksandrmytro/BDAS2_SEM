using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IAdresaRepository
    {
        Task<int> AddNewAdresa(ADRESA adresa);
        Task UpdateAdresa(int id, ADRESA adresa);
        Task<ADRESA> GetAdresaById(int id);
        Task<IEnumerable<ADRESA>> GetAllAddresses();
        Task DeleteAdresa(int id);
    }
}
