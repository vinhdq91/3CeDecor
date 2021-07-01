using AutoMapper;
using DAL.Core.Utilities;
using DAL.Models.Application;
using DAL.Repositories.CartItemRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.CarItemService
{
    public class CarItemService : ICarItemService
    {
        private readonly ICartItemRepository _cartItemRepos;
        private readonly IMapper _mapper;
        public const string CartSessionKey = "3CECartSessionId";
        public CarItemService(ICartItemRepository cartItemRepos, IMapper mapper)
        {
            _cartItemRepos = cartItemRepos;
            _mapper = mapper;
        }
        public Task<CartItemEntity> AddToCart(int id)
        {
            return null;
        }

        public IQueryable<CartItemEntity> GetAllCartItems()
        {
            throw new NotImplementedException();
        }

        public string GetCartId()
        {
            throw new NotImplementedException();
        }
    }
}
