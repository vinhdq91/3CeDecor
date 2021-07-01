using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.CarItemService
{
    public interface ICarItemService
    {
        Task<CartItemEntity> AddToCart(int id);
        string GetCartId();
        IQueryable<CartItemEntity> GetAllCartItems();
    }
}
