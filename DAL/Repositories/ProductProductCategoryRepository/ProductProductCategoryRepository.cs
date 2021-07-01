using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ProductProductCategoryRepository
{
    public class ProductProductCategoryRepository : Repository<ProductProductCategory>, IProductProductCategoryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ProductProductCategoryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
