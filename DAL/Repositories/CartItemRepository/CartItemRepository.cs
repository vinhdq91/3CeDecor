using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using DAL.Repositories.CartItemRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ProductRepository
{
    public class CartItemRepository : Repository<CartItemEntity>, ICartItemRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CartItemRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
