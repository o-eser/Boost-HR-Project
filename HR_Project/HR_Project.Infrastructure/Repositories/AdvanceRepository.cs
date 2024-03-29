﻿using HR_Project.Domain.Entities.Concrete;
using HR_Project.Domain.Repositories;
using HR_Project.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR_Project.Infrastructure.Repositories
{
    public class AdvanceRepository : BaseRepository<Advance>, IAdvanceRepository
    {
        public AdvanceRepository(AppDbContext context) : base(context)
        {
        }
    }
}
