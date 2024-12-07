using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task<int> AddRole(ROLE role);
        Task UpdateRole(ROLE role);
        Task<ROLE> GetRoleById(int id);
        Task<IEnumerable<ROLE>> GetAllRoles();
        Task DeleteRole(int id);
    }
}
