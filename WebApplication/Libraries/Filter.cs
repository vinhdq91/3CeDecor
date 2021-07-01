using DAL.Core.Enums;
using DAL.Core.Model;
using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Libraries
{
    public static class Filter
    {
        public static ProductListModel FilterProductSearch(
            ProductSearch objSearch, 
            PagingInfo paging, 
            List<ProductEntity> allProducts,
            List<ProductDto> productDtos)
        {
            ProductListModel productListModel = new ProductListModel();
            productListModel.Paging = paging;

            int categoryId = objSearch.CategoryId;
            string textSearch = objSearch.TextSearch == null ? string.Empty : objSearch.TextSearch;
            decimal priceMin = objSearch.PriceMin == null ? -1 : Convert.ToDecimal(objSearch.PriceMin);
            decimal priceMax = objSearch.PriceMax == null ? -1 : Convert.ToDecimal(objSearch.PriceMax);
            int pageIndex = objSearch.PageIndex == 0 ? 1 : objSearch.PageIndex;

            if (categoryId > 0)
            {
                if(priceMin != -1 && priceMax != -1)
                {
                    productListModel.ListProducts = GetOrderProducts(productDtos, paging.Sort)
                                                            .Where(x => x.Name.ToLower().Contains(textSearch.ToLower()))
                                                            .Where(x => x.PriceAfterSale >= priceMin && x.PriceAfterSale <= priceMax)
                                                            .Where(x => x.ProductCategoryIds.Any(x => x == categoryId))
                                                            .Skip((paging.PageIndex - 1) * paging.PageSize)
                                                            .Take(paging.PageSize).ToList();

                    productListModel.Paging.Count = allProducts.Where(x => x.Name.ToLower().Contains(textSearch.ToLower()))
                                                                .Where(x => x.PriceAfterSale >= priceMin && x.PriceAfterSale <= priceMax)
                                                                .Where(x => x.ProductProductCategories.Any(y => y.ProductCategoryId == categoryId))
                                                                .Count();
                }
                else
                {
                    productListModel.ListProducts = GetOrderProducts(productDtos, paging.Sort)
                                                            .Where(x => x.Name.ToLower().Contains(textSearch.ToLower()))
                                                            .Where(x => x.ProductCategoryIds.Any(x => x == categoryId))
                                                            .Skip((paging.PageIndex - 1) * paging.PageSize)
                                                            .Take(paging.PageSize).ToList();

                    productListModel.Paging.Count = allProducts.Where(x => x.Name.ToLower().Contains(textSearch.ToLower()))
                                                                .Where(x => x.ProductProductCategories.Any(x => x.ProductCategoryId == categoryId))
                                                                .Count();
                }
            }
            else
            {
                if(priceMin != -1 && priceMax != -1)
                {
                    productListModel.ListProducts = GetOrderProducts(productDtos, paging.Sort)
                                                            .Where(x => x.PriceAfterSale >= priceMin && x.PriceAfterSale <= priceMax)
                                                            .Where(x => x.Name.ToLower().Contains(textSearch.ToLower()))
                                                            .Skip((paging.PageIndex - 1) * paging.PageSize)
                                                            .Take(paging.PageSize).ToList();

                    productListModel.Paging.Count = allProducts.Where(x => x.PriceAfterSale >= priceMin && x.PriceAfterSale <= priceMax)
                                                                .Where(x => x.Name.ToLower().Contains(textSearch.ToLower())).Count();
                                                                
                }
                else
                {
                    productListModel.ListProducts = GetOrderProducts(productDtos, paging.Sort)
                                                            .Where(x => x.Name.ToLower().Contains(textSearch.ToLower()))
                                                            .Skip((paging.PageIndex - 1) * paging.PageSize)
                                                            .Take(paging.PageSize).ToList();

                    productListModel.Paging.Count = allProducts.Where(x => x.Name.ToLower().Contains(textSearch.ToLower())).Count();
                }
            }

            return productListModel;
        }

        public static List<ProductDto> GetOrderProducts(List<ProductDto> productDtos, int sort)
        {
            switch (sort)
            {
                case (int)SortProductEnum.PublishDateASC:
                    return productDtos.OrderBy(x => x.CreatedDate).ToList();
                case (int)SortProductEnum.PublishDateDESC:
                    return productDtos.OrderByDescending(x => x.CreatedDate).ToList();
                case (int)SortProductEnum.PriceASC:
                    return productDtos.OrderBy(x => x.Price).ToList();
                case (int)SortProductEnum.PriceDESC:
                    return productDtos.OrderByDescending(x => x.Price).ToList();
                default:
                    return productDtos;
            }
        }
    }
}
