using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ProvinceRepository
{
    public class ProvinceRepository : Repository<ProvinceEntity>, IProvinceRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ProvinceRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
