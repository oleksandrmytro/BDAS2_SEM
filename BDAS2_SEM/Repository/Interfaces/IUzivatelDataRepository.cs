using BDAS2_SEM.Model;
using BDAS2_SEM.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IUzivatelDataRepository
    {
        Task<UZIVATEL_DATA> CheckCredentials(string email, string heslo);
        Task<int> RegisterNewUserData(string email, string heslo);
        Task<UZIVATEL_DATA> GetUzivatelById(int id);
        Task<UZIVATEL_DATA> GetUserByEmailAsync(string email);
        Task<IEnumerable<UZIVATEL_DATA>> GetUsersWithUndefinedRole();
        Task UpdateUserData(UZIVATEL_DATA userData);
        Task<IEnumerable<UZIVATEL_DATA>> GetAllUzivatelDatas();

    }
}
