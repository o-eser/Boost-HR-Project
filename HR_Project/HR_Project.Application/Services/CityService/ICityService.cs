﻿using HR_Project.Common.Models.DTOs;
using HR_Project.Common.Models.VMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Project.Application.Services.CityService
{
    public interface ICityService
    {
        Task<List<CityDTO>> GetCities();
    }
}
