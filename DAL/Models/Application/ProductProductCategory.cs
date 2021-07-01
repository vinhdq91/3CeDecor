using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Application
{
    public class ProductProductCategory
    {
        public int ProductId { get; set; }
        public ProductEntity ProductEntity { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategoryEntity ProductCategoryEntity { get; set; }
    }
}
