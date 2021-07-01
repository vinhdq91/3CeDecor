using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Models.Application
{
    public class ProductCategoryEntity: AuditableEntity
    {
        public int Id { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(510)]
        public string Description { get; set; }

        [StringLength(60)]
        public string MetaTitle { get; set; }

        [StringLength(510)]
        public string MetaDescription { get; set; }

        [StringLength(60)]
        public string UrlName { get; set; }

        public virtual List<ProductProductCategory> ProductProductCategories { get; set; }
    }
}
