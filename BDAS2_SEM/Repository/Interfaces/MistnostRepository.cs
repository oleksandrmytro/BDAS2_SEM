using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public class MistnostRepository : IMistnostRepository
    {
        private string connectionString;

        public MistnostRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Реализация методов репозитория
        public Task<int> AddMistnost(MISTNOST mistnost)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMistnost(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<MISTNOST>> GetAllMistnosti()
        {
            throw new NotImplementedException();
        }

        public Task<MISTNOST> GetMistnostById(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMistnost(MISTNOST mistnost)
        {
            throw new NotImplementedException();
        }
    }
}
