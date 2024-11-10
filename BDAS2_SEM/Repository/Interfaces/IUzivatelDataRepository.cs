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
        Task UpdateUserEmail(int id, string newEmail);
        Task UpdateUserPassword(int id, string newPassword);
        Task<UZIVATEL_DATA> GetUzivatelById(int id);
        Task<UZIVATEL_DATA> GetUserByEmailAsync(string email);
    }
}
