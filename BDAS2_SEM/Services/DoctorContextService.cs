﻿using BDAS2_SEM.Model;
using BDAS2_SEM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDAS2_SEM.Services
{
    public class DoctorContextService : IDoctorContextService
    {
        public ZAMESTNANEC CurrentDoctor { get; set; }
    }
}
