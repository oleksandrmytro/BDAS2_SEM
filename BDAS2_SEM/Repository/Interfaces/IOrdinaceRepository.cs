using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IOrdinaceRepository
    {
        Task<int> AddOrdinace(ORDINACE ordinace);
        Task UpdateOrdinace(ORDINACE ordinace);
        Task<ORDINACE> GetOrdinaceById(int id);
        Task<IEnumerable<ORDINACE>> GetAllOrdinaces();
        Task DeleteOrdinace(int id);
    }
}
