using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Services.Interfaces
{
    public interface IDoctorContextService
    {
        ZAMESTNANEC CurrentDoctor { get; set; }
    }
}
