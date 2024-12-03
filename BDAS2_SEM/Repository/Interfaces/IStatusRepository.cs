using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model.Enum;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IStatusRepository
    {
        Task<int> AddStatus(STATUS pozice);
        Task UpdateStatus(STATUS pozice);
        Task<STATUS> GetStatusById(int id);
        Task<IEnumerable<STATUS>> GetAllStatuses();
        Task DeleteStatus(int id);
    }
}
