using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface ILogRepository
    {
        Task<int> AddLog(LOG log);
        Task UpdateLog(LOG log);
        Task<IEnumerable<LOG>> GetAllLogs();
        Task DeleteLog(int id);
    }
}
