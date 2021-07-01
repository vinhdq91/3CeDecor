using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ProductRepository
{
    public interface IProductRepository : IRepository<ProductEntity>
    {
    }
}
