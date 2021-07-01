using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProductProductCategoryService
{
    public interface IProductProductCategoryService
    {
        Task<IQueryable<ProductProductCategory>> GetListAllProductProductCategoriesAsync();
        Task<IQueryable<ProductProductCategory>> GetListProductProductCategoriesAsync(int? productId, int? productCategoryId);

        Task<ProductProductCategory> GetProductProductCategoryAsync(int id);

        Task AddProductProductCategoryAsync(ProductProductCategory productProductCategory);
        Task UpdateProductProductCategoryAsync(ProductProductCategory productProductCategory);
        Task DeleteProductProductCategoryAsync(ProductProductCategory productProductCategory);
    }
}
