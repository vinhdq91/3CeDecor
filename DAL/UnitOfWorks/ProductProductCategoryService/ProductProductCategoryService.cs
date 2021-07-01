using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.Repositories.ProductCategoryRepository;
using DAL.Repositories.ProductProductCategoryRepository;
using DAL.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProductProductCategoryService
{
    public class ProductProductCategoryService : IProductProductCategoryService
    {
        private readonly IProductProductCategoryRepository _productProductCategoryRepos;
        private readonly IMapper _mapper;
        public ProductProductCategoryService(IProductProductCategoryRepository productProductCategoryRepos, IMapper mapper)
        {
            _productProductCategoryRepos = productProductCategoryRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<ProductProductCategory>> GetListAllProductProductCategoriesAsync()
        {
            IQueryable<ProductProductCategory> productCategoryEntities = await _productProductCategoryRepos.GetAllAsync();
            return productCategoryEntities;
        }

        public async Task<IQueryable<ProductProductCategory>> GetListProductProductCategoriesAsync(int? productId, int? productCategoryId)
        {
            IQueryable<ProductProductCategory> productCategoryEntities = await _productProductCategoryRepos.GetAllAsync();
            productCategoryEntities = productCategoryEntities.Where(x => productId == null || x.ProductId == productId)
                                                             .Where(x => productCategoryId == null || x.ProductCategoryId == productCategoryId);
            return productCategoryEntities;
        }

        public async Task<ProductProductCategory> GetProductProductCategoryAsync(int productId)
        {
            ProductProductCategory productCategoryEntity = await _productProductCategoryRepos.GetSingleOrDefaultAsync(x => x.ProductId == productId);
            return productCategoryEntity;
        }

        public async Task AddProductProductCategoryAsync(ProductProductCategory productProductCategory)
        {
            await _productProductCategoryRepos.AddAsync(productProductCategory);
        }

        public async Task UpdateProductProductCategoryAsync(ProductProductCategory productProductCategory)
        {
            await _productProductCategoryRepos.UpdateAsync(productProductCategory);
        }

        public async Task DeleteProductProductCategoryAsync(ProductProductCategory productProductCategory)
        {
            await _productProductCategoryRepos.RemoveAsync(productProductCategory);
        }
    }
}
