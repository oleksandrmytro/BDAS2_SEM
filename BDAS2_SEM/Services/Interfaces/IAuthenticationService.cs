using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(string email, string password);
        Task<UZIVATEL_DATA> LoginAsync(string email, string password);
        Task<bool> CheckUserExistsAsync(string email);
    }
}
