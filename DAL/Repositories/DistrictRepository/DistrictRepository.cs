using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.DistrictRepository
{
    public class DistrictRepository : Repository<DistrictEntity>, IDistrictRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public DistrictRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
