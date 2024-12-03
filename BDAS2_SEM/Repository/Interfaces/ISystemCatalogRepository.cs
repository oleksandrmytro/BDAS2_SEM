using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BDAS2_SEM.Model;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface ISystemCatalogRepository
    {
        Task<IEnumerable<SystemCatalog>> GetSystemCatalog();
    }
}
