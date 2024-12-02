using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IHotovostRepository
    {
        Task<int> AddHotovost(HOTOVOST hotovost);
        Task UpdateHotovost(HOTOVOST hotovost);
        Task<HOTOVOST> GetHotovostById(int id);
        Task<IEnumerable<HOTOVOST>> GetAllHotovost();
        Task DeleteHotovost(int id);
    }
}
