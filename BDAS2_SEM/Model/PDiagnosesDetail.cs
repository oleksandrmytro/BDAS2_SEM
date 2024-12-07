using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Model
{
    public class PDiagnosesDetail
    {
        public int IdNavsteva { get; set; }
        public DateTime Datum { get; set; }
        public int PacientIdPacient { get; set; }
        public int StatusIdStatus { get; set; }
        public int MistnostId { get; set; }
        public string DiagnosisName { get; set; }
        public string DiagnosisDescription { get; set; }
        public string PrescribedMedications { get; set; }
        public decimal TotalCost { get; set; }
    }
}
