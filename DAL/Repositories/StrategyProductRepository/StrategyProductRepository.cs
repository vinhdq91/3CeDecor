using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.StrategyProductRepository
{
    public class StrategyProductRepository : Repository<StrategyProduct>, IStrategyProductRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public StrategyProductRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
