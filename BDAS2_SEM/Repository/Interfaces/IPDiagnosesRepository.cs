using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IPDiagnosesRepository
    {
        Task<IEnumerable<PDiagnosesDetail>> GetPDiagnosesByPatientIdAsync(int pacientId);
    }
}
