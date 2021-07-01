using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Utility.Profile
{
    public class ApplicationProfile : AutoMapper.Profile
    {
        public ApplicationProfile()
        {
            CreateMap<CustomerEntity, CustomerDto>().ReverseMap();
            CreateMap<ProductEntity, ProductDto>().ReverseMap();
            CreateMap<ProductCategoryEntity, ProductCategoryDto>().ReverseMap();
            CreateMap<OrderEntity, OrderDto>().ReverseMap();
            CreateMap<OrderDetailEntity, OrderDetailDto>().ReverseMap();
            CreateMap<ArticleBlogEntity, ArticleBlogDto>().ReverseMap();
            CreateMap<ImageEntity, ImageDto>().ReverseMap();
            CreateMap<OrderHistoryEntity, OrderHistoryDto>().ReverseMap();
            CreateMap<DiscountCodeEntity, DiscountCodeDto>().ReverseMap();
            CreateMap<ProvinceEntity, ProvinceDto>().ReverseMap();
            CreateMap<DistrictEntity, DistrictDto>().ReverseMap();
            CreateMap<WardEntity, WardDto>().ReverseMap();
            CreateMap<StrategyEntity, StrategyDto>().ReverseMap();
            CreateMap<GalleryEntity, GalleryDto>().ReverseMap();
        }
    }
}
