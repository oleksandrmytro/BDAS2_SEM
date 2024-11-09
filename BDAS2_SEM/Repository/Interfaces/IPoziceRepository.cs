using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IPoziceRepository
    {
        Task<int> AddPozice(POZICE pozice);
        Task UpdatePozice(POZICE pozice);
        Task<POZICE> GetPoziceById(int id);
        Task<IEnumerable<POZICE>> GetAllPozice();
        Task DeletePozice(int id);
    }
}
