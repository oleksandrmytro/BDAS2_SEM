using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface INavstevaRepository
    {
        Task<int> AddNavsteva(NAVSTEVA navsteva);
        Task UpdateNavsteva(NAVSTEVA navsteva);
        Task<NAVSTEVA> GetNavstevaById(int id);
        Task<IEnumerable<NAVSTEVA>> GetAllNavstevy();
        Task DeleteNavsteva(int id);
        Task<IEnumerable<NAVSTEVA>> GetAppointmentsByDoctorId(int doctorId);
    }
}
