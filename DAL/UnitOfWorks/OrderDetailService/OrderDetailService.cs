using AutoMapper;
using DAL.Models.Application;
using DAL.Repositories.OrderDetailRepository;
using DAL.Repositories.OrderRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.OrderDetailService
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _orderDetailRepos;
        private readonly IMapper _mapper;
        public OrderDetailService(IOrderDetailRepository orderDetailRepos, IMapper mapper)
        {
            _orderDetailRepos = orderDetailRepos;
            _mapper = mapper;
        }
        public void AddOrderDetailAsync(OrderDetailEntity orderEntity)
        {
            _orderDetailRepos.AddAsync(orderEntity);
        }

        public async Task<IQueryable<OrderDetailEntity>> GetListAllOrderDetailByOrderIdAsync(int orderId)
        {
            IQueryable<OrderDetailEntity> orderDetailEntities = await _orderDetailRepos.GetAllAsync();
            orderDetailEntities = orderDetailEntities.Where(x => x.OrderEntityId == orderId);
            return orderDetailEntities;
        }
    }
}
