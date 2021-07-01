using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.OrderRepository
{
    public class OrderRepository: Repository<OrderEntity>, IOrderRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public OrderRepository(ApplicationDbContext applicationDbContext): base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
