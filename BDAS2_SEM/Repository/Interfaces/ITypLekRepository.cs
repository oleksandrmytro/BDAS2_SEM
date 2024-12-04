using BDAS2_SEM.Model.Enum;
using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface ITypLekRepository
    {
        Task<int> AddTypLek(TYP_LEK typLek);
        Task UpdateTypLek(TYP_LEK typLek);
        Task<POZICE> GetTypLekById(int id);
        Task<IEnumerable<TYP_LEK>> GetAllTypLekes();
        Task DeleteTypLek(int id);
    }
}
