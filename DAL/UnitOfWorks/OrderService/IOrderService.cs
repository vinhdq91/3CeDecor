using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.OrderService
{
    public interface IOrderService
    {
        Task AddOrderAsync(OrderEntity orderEntity);
        Task<IQueryable<OrderEntity>> GetListAllOrdersAsync();
        Task<OrderEntity> GetOrderAsync(int id);
        Task RemoveAsync(OrderEntity orderEntity);
        Task UpdateAsync(OrderEntity orderEntity);
    }
}
