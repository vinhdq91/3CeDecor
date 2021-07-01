using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.WardRepository
{
    public class WardRepository : Repository<WardEntity>, IWardRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public WardRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
