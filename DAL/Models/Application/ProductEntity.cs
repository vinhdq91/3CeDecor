using DAL.Core.Enums;
using DAL.Models.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class ProductEntity : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(125)]
        public string Name { get; set; }
        public int? Size { get; set; }

        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$")]
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public decimal PriceAfterSale
        {
            get
            {
                return Price - (Price * Discount / 100);
            }
        }
        public int Status { get; set; }

        [StringLength(510)]
        public string Description { get; set; }
        public int ProductType { get; set; }

        [StringLength(110)]
        public string UrlPath { get; set; }
        public int NumberInStock { get; set; }

        [StringLength(110)]
        public string MetaTitle { get; set; }

        [StringLength(510)]
        public string MetaDescription { get; set; }


        [ForeignKey("ProductEntityId")]
        public virtual List<OrderDetailEntity> OrderDetails { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một loại sản phẩm")]
        public virtual List<ProductProductCategory> ProductProductCategories { get; set; }
        public virtual List<ImageEntity> ImageIds { get; set; }
        public virtual List<StrategyProduct> StrategyProducts { get; set; }
    }
}
