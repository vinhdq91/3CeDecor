using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.OrderDetailRepository
{
    public class OrderDetailRepository: Repository<OrderDetailEntity>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public OrderDetailRepository(ApplicationDbContext applicationDbContext): base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
