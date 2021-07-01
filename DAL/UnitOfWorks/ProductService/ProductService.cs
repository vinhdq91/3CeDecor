using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.Repositories.ProductProductCategoryRepository;
using DAL.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProductService
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _productRepos;
        private readonly IMapper _mapper;
        public ProductService(IProductRepository productRepos, IMapper mapper)
        {
            _productRepos = productRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<ProductEntity>> GetListAllProductsAsync(bool isAdmin = false)
        {
            IQueryable<ProductEntity> productEntities = await _productRepos.GetAllAsync();
            if (productEntities != null && !isAdmin)
            {
                productEntities = productEntities.Where(x => x.Status == (int)ProductStatus.Active);
            }
            return productEntities;
        }

        public async Task<IQueryable<ProductEntity>> GetListAllProductsByTypeAsync(int productType, bool isAdmin = false)
        {
            IQueryable<ProductEntity> productEntities = await _productRepos.GetAllAsync();
            if (productEntities != null)
            {
                productEntities = productEntities.Where(x => x.ProductType == productType);
                if (!isAdmin)
                {
                    productEntities = productEntities.Where(x => x.Status == (int)ProductStatus.Active);
                }
            }
            return productEntities;
        }

        public async Task<IQueryable<ProductEntity>> GetListAllProductsLatestAsync(int numOfProducts)
        {
            IQueryable<ProductEntity> productEntities = await _productRepos.GetAllAsync();
            if (productEntities != null)
            {
                productEntities = productEntities.Where(x => x.Status == (int)ProductStatus.Active)
                                            .OrderByDescending(x => x.CreatedDate)
                                            .Take(numOfProducts);
            }
            return productEntities;
        }

        public async Task<ProductEntity> GetProductAsync(int id)
        {
            ProductEntity productEntity = await _productRepos.GetSingleOrDefaultAsync(x => x.Id == id);
            return productEntity;
        }

        public async Task AddProductAsync(ProductEntity productEntity)
        {
            await _productRepos.AddAsync(productEntity);
        }

        public async Task RemoveAsync(ProductEntity productEntity)
        {
            await _productRepos.RemoveAsync(productEntity);
        }
        
        public async Task UpdateProductNoImageAsync(ProductEntity productEntity)
        {
            await _productRepos.UpdateNoImageAsync(productEntity);
        }
        
        public async Task UpdateProductAsync(ProductEntity productEntity)
        {
            await _productRepos.UpdateAsync(productEntity);
        }
    }
}
