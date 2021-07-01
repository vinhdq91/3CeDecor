using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.DiscountCodeRepository
{
    public class DiscountCodeRepository : Repository<DiscountCodeEntity>, IDiscountCodeRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public DiscountCodeRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
