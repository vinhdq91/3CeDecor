using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.OrderHistoryRepository
{
    public class OrderHistoryRepository : Repository<OrderHistoryEntity>, IOrderHistoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public OrderHistoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
