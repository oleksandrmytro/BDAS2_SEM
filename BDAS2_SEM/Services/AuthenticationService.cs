using BDAS2_SEM.Model;
using BDAS2_SEM.Repository.Interfaces;
using BDAS2_SEM.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUzivatelDataRepository _uzivatelRepository;
        private readonly IPacientRepository _pacientRepository;
        private readonly IRoleRepository _roleRepository;

        public AuthenticationService(IUzivatelDataRepository uzivatelRepository, IPacientRepository pacientRepository, IRoleRepository roleRepository)
        {
            _uzivatelRepository = uzivatelRepository;
            _pacientRepository = pacientRepository;
            _roleRepository = roleRepository;
        }

        public async Task<bool> RegisterUserAsync(string email, string password)
        {
            try
            {
                var existingUser = await _uzivatelRepository.GetUserByEmailAsync(email);
                if (existingUser != null)
                {
                    return false;
                }

                int newUserId = await _uzivatelRepository.RegisterNewUserData(email, HashPassword(password));

                return newUserId > 0;
            }
            catch (Oracle.ManagedDataAccess.Client.OracleException ex) when (ex.Number == 1)
            {
                throw new Exception("User already exists.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UZIVATEL_DATA> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _uzivatelRepository.CheckCredentials(email, HashPassword(password));

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public async Task<bool> CheckUserExistsAsync(string email)
        {
            var existingUser = await _uzivatelRepository.GetUserByEmailAsync(email);
            return existingUser != null;
        }

        public async Task<bool> IsPatientDataComplete(int userId)
        {
            var pacient = await _pacientRepository.GetPacientByUserDataId(userId);
            return pacient != null;
        }

        public async Task<PACIENT> GetPatientByUserId(int userId)
        {
            return await _pacientRepository.GetPacientByUserDataId(userId);
        }
    }
}