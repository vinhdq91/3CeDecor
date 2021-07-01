using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.Repositories.ProductCategoryRepository;
using DAL.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProductCategoryService
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoryRepos;
        private readonly IMapper _mapper;
        public ProductCategoryService(IProductCategoryRepository productCategoryRepos, IMapper mapper)
        {
            _productCategoryRepos = productCategoryRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<ProductCategoryEntity>> GetListAllProductCategoriesAsync()
        {
            IQueryable<ProductCategoryEntity> productCategoryEntities = await _productCategoryRepos.GetAllAsync();
            return productCategoryEntities;
        }

        public async Task<ProductCategoryEntity> GetProductCategoryAsync(int id)
        {
            ProductCategoryEntity productCategoryEntity = await _productCategoryRepos.GetSingleOrDefaultAsync(x => x.Id == id);
            return productCategoryEntity;
        }

        public async Task AddProductCategoryAsync(ProductCategoryEntity productCategoryEntity)
        {
            await _productCategoryRepos.AddAsync(productCategoryEntity);
        }

        public async Task UpdateProductCategoryAsync(ProductCategoryEntity productCategoryEntity)
        {
            await _productCategoryRepos.UpdateAsync(productCategoryEntity);
        }

        public async Task DeleteProductCategoryAsync(ProductCategoryEntity productCategoryEntity)
        {
            await _productCategoryRepos.RemoveAsync(productCategoryEntity);
        }
    }
}
