using BDAS2_SEM.Model;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IPacientRepository
    {
        Task<int> AddPacient(PACIENT pacient);
        Task UpdatePacient(PACIENT pacient);
        Task<PACIENT> GetPacientById(int id);
        Task<IEnumerable<PACIENT>> GetAllPacients();
        Task DeletePacient(int id);
        Task<PACIENT> GetPacientByUserDataId(int userDataId);
    }
}