using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IDoctorInfoRepository
    {
        Task<IEnumerable<DOCTOR_INFO>> GetAllDoctors();
    }
}
