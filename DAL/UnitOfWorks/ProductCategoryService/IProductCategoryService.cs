using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProductCategoryService
{
    public interface IProductCategoryService
    {
        Task<IQueryable<ProductCategoryEntity>> GetListAllProductCategoriesAsync();

        Task<ProductCategoryEntity> GetProductCategoryAsync(int id);
        Task AddProductCategoryAsync(ProductCategoryEntity productCategoryEntity);
        Task UpdateProductCategoryAsync(ProductCategoryEntity productCategoryEntity);
        Task DeleteProductCategoryAsync(ProductCategoryEntity productCategoryEntity);
    }
}
