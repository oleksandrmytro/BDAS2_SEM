using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IOperaceRepository
    {
        Task<int> AddOperace(OPERACE operace);
        Task UpdateOperace(OPERACE operace);
        Task<OPERACE> GetOperaceById(int id);
        Task<IEnumerable<OPERACE>> GetAllOperaces();
        Task DeleteOperace(int id);
    }
}
