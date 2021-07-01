using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.OrderDetailService
{
    public interface IOrderDetailService
    {
        void AddOrderDetailAsync(OrderDetailEntity orderEntity);
        Task<IQueryable<OrderDetailEntity>> GetListAllOrderDetailByOrderIdAsync(int orderId);
    }
}
