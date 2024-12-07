using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IPlatbaRepository
    {
        Task<int> AddPlatba(PLATBA platba);
        Task UpdatePlatba(PLATBA platba);
        Task<PLATBA> GetPlatbaById(int id);
        Task<IEnumerable<PLATBA>> GetAllPlatbas();
        Task DeletePlatba(int id);

        Task<int> ManagePaymentAsync(
            string action,
            int? idPlatba,
            decimal castka,
            DateTime datum,
            string typPlatby,
            int navstevaId,
            long? cisloKarty = null,
            decimal? prijato = null,
            decimal? vraceno = null);
    }
}
