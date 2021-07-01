using AutoMapper;
using DAL.Repositories.OrderHistoryRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.UnitOfWorks.OrderHistoryService
{
    public class OrderHistoryService : IOrderHistoryService
    {
        private readonly IOrderHistoryRepository _orderHistoryRepos;
        private readonly IMapper _mapper;
        public OrderHistoryService(IOrderHistoryRepository orderHistoryRepos, IMapper mapper)
        {
            _orderHistoryRepos = orderHistoryRepos;
            _mapper = mapper;
        }
    }
}
