using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.StrategyRepository
{
    public class StrategyRepository : Repository<StrategyEntity>, IStrategyRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public StrategyRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
