using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProductService
{
    public interface IProductService
    {
        Task<IQueryable<ProductEntity>> GetListAllProductsAsync(bool isAdmin = false);
        Task<IQueryable<ProductEntity>> GetListAllProductsByTypeAsync(int productType, bool isAdmin = false);

        Task<ProductEntity> GetProductAsync(int id);
        Task<IQueryable<ProductEntity>> GetListAllProductsLatestAsync(int numOfProducts);

        Task AddProductAsync(ProductEntity productEntity);
        Task RemoveAsync(ProductEntity productEntity);
        Task UpdateProductNoImageAsync(ProductEntity productEntity);
        Task UpdateProductAsync(ProductEntity productEntity);
    }
}
