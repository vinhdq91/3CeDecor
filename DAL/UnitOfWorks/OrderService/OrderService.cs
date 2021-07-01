using AutoMapper;
using DAL.Models.Application;
using DAL.Repositories.OrderRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepos;
        private readonly IMapper _mapper;
        public OrderService(IOrderRepository orderRepos, IMapper mapper)
        {
            _orderRepos = orderRepos;
            _mapper = mapper;
        }
        public async Task AddOrderAsync(OrderEntity orderEntity)
        {
            await _orderRepos.AddAsync(orderEntity);
        }
        public async Task<IQueryable<OrderEntity>> GetListAllOrdersAsync()
        {
            IQueryable<OrderEntity> orderEntities = await _orderRepos.GetAllAsync();
            return orderEntities;
        }
        public async Task<OrderEntity> GetOrderAsync(int id)
        {
            OrderEntity orderEntity = await _orderRepos.GetSingleOrDefaultAsync(x => x.Id == id);
            return orderEntity;
        }
        public async Task RemoveAsync(OrderEntity orderEntity)
        {
            await _orderRepos.RemoveAsync(orderEntity);
        }

        public async Task UpdateAsync(OrderEntity orderEntity)
        {
            await _orderRepos.UpdateAsync(orderEntity);
        }
    }
}
