using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.DiscountCodeService
{
    public interface IDiscountCodeService
    {
        Task<DiscountCodeEntity> CheckDiscountCodeAsync(string discountCode);
        Task<IQueryable<DiscountCodeEntity>> GetListAllDiscountCodeAsync();
        Task<DiscountCodeEntity> GetDiscountCodeAsync(int discountCodeId);
        Task AddDiscountCodeAsync(DiscountCodeEntity discountCodeEntity);
        Task UpdateDiscountCodeAsync(DiscountCodeEntity discountCodeEntity);
        Task DeleteDiscountCodeAsync(DiscountCodeEntity discountCodeEntity);
    }
}
