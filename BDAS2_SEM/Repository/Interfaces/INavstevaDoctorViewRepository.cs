using BDAS2_SEM.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface INavstevaDoctorViewRepository
    {
        Task<IEnumerable<NAVSTEVA_DOCTOR_VIEW>> GetNavstevaDoctorViewByPatientId(int pacientId);
    }
}