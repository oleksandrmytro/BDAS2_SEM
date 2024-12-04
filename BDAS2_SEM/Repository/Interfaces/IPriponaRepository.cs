using BDAS2_SEM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IPriponaRepository
    {
        Task<int> AddPripona(PRIPONA pripona);
        Task UpdatePripona(PRIPONA pripona);
        Task<PRIPONA> GetPriponaById(int id);
        Task<IEnumerable<PRIPONA>> GetAllPriponas();
        Task DeletePripona(int id);
    }
}