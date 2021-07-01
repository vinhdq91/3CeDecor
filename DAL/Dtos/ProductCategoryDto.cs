using DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class ProductCategoryDto : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string UrlName { get; set; }

        public List<int> ProductIds { get; set; }
        public List<ProductDto> ProductDtos { get; set; }
    }
}
