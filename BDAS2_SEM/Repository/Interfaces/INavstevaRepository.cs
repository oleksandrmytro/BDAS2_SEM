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
        Task<IEnumerable<NAVSTEVA>> GetAllNavstevas();
        Task<IEnumerable<NAVSTEVA>> GetFutureNavstevy();
        Task DeleteNavsteva(int id);
        Task<IEnumerable<NAVSTEVA>> GetAppointmentsByDoctorId(int doctorId);
        Task<IEnumerable<NAVSTEVA>> GetAppointmentsByDoctorIdDateAndRoom(int doctorId, DateTime date, int room);
        Task<bool> IsTimeSlotAvailable(int doctorId, DateTime dateTime, int room, int? excludeAppointmentId = null);
        Task<bool> ExistsNavstevaForDoctorAndPatientAsync(int doctorId, int patientId);
        Task<List<(int Room, string TimeSlot)>> GetAvailableRoomsAndTimes(int ordinaceId, DateTime date);
        Task<(string FirstName, string LastName)> GetPatientNameByAppointmentId(int appointmentId);
    }
}