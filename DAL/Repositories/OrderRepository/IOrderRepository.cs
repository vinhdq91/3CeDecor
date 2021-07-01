using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.OrderRepository
{
    public interface IOrderRepository : IRepository<OrderEntity>
    {

    }
}
