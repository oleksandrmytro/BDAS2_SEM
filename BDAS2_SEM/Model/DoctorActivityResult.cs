using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Model
{
    public class DoctorActivityResult
    {
        public int TotalVisits { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public int TotalOperations { get; set; }
        public DateTime? LastOperationDate { get; set; }
        public int TotalMedicines { get; set; }
    }
}
