using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ProductCategoryRepository
{
    public class ProductCategoryRepository: Repository<ProductCategoryEntity>, IProductCategoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ProductCategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
