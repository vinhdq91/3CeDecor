using DAL.Core.Enums;
using DAL.Core.Model;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Dtos
{
    public class ProductDto : AuditableEntity
    {
        public ProductDto()
        {
            ProductCategoryInfos = new List<ProductCategoryInfo>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Size { get; set; }
        public string SizeName
        {
            get
            {
                if (Size == 1)
                    return "Nhỏ";
                if (Size == 2)
                    return "Vừa";
                if (Size == 3)
                    return "Lớn";
                return "Vừa";
            }
        }
        public decimal Price { get; set; }
        public int Discount { get; set; }

        [NotMapped]
        public decimal PriceAfterSale
        {
            get
            {
                return Price - (Price * Discount / 100);
            }
        }
        public int Status { get; set; }
        public string Description { get; set; }

        // Trường này để lưu giá trị hiện tại của productType, trong trường hợp update ảnh mới cho product bị lỗi
        public int ProductCurrentType { get; set; }
        public int ProductType { get; set; }
        public string ProductTypeName
        {
            get
            {
                if (ProductType == (int)ProductTypeEnum.Normal)
                    return "";
                if (ProductType == (int)ProductTypeEnum.Featured)
                    return "Sản phẩm nổi bật";
                if (ProductType == (int)ProductTypeEnum.BestSelling)
                    return "Sản phẩm bán chạy ";
                if (ProductType == (int)ProductTypeEnum.TopRate)
                    return "Sản phẩm được bình chọn";
                if (ProductType == (int)ProductTypeEnum.TopNew)
                    return "Sản phẩm mới nổi bật";
                if (ProductType == (int)ProductTypeEnum.Recent)
                    return "Sản phẩm mới nhất";
                return "";
            }
        }
        public string UrlPath { get; set; }
        public int NumberInStock { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public int ProductCategoryId { get; set; }

        public string ProductCategoryName {
            get
            {
                if (ProductCategoryId == (int)ProductCategoryEnum.HomeDecor)
                    return "Trang trí nhà cửa";
                if (ProductCategoryId == (int)ProductCategoryEnum.RestaurantDecor)
                    return "Trang trí nhà hàng, khách sạn";
                if (ProductCategoryId == (int)ProductCategoryEnum.KidToys)
                    return "Đồ chơi trẻ em";
                return "";
            }
        }
        public string ProductCategoryUrl
        {
            get
            {
                if (ProductCategoryId == (int)ProductCategoryEnum.HomeDecor)
                    return "/product-search/nha-cua";
                if (ProductCategoryId == (int)ProductCategoryEnum.RestaurantDecor)
                    return "/product-search/nha-hang-khach-san";
                if (ProductCategoryId == (int)ProductCategoryEnum.KidToys)
                    return "/product-search/do-choi-tre-em";
                return "";
            }
        }

        [Required(ErrorMessage = "Vui lòng chọn ít nhất một loại sản phẩm")]
        public List<int> ProductCategoryIds { get; set; }
        public List<ProductCategoryInfo> ProductCategoryInfos { get; set; }
        public ProductCategoryDto ProductCategory { get; set; }
        public virtual List<OrderDetailDto> OrderDetails { get; set; }
        public virtual List<ImageDto> ImageIds { get; set; }
    }

    public class ProductCategoryInfo {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryUrl { get; set; }
    }

    public class ProductListModel
    {
        public string TitleList { get; set; }
        public List<ProductDto> ListProducts { get; set; }
        public PagingInfo Paging { get; set; }
        public string MenuActive { get; set; }
        public string TextSEO { get; set; }
        public string LinkSearchSuggest { get; set; }

        public ProductListModel()
        {
            ListProducts = new List<ProductDto>();
        }
    }

    public class ProductSearch
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TagId { get; set; }
        public int TagType { get; set; }
        public string TextSearch { get; set; }
        public int Price { get; set; }
        public string PriceMin { get; set; }
        public string PriceMax { get; set; }
        public int OrderBy { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<ProductCategoryDto> ProductCategoryDtos { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
