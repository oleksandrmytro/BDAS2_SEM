﻿using BDAS2_SEM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Repository.Interfaces
{
    public interface IStatusRepository
    {
        Task<int> AddStatus(STATUS status);
        Task UpdateStatus(STATUS status);
        Task<STATUS> GetStatusById(int id);
        Task<IEnumerable<STATUS>> GetAllStatuses();
        Task DeleteStatus(int id);
    }
}
