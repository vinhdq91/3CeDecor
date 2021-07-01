using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using DAL.Core.Utilities;
using Microsoft.AspNetCore.Http;
using DAL.UnitOfWorks.ProductService;
using DAL.Dtos;
using System.Text.RegularExpressions;
using DAL.UnitOfWorks.ProductCategoryService;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public class BuildLinkProduct : IBuildLinkProduct
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _domainName;
        private readonly string _pathUrl;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;

        public BuildLinkProduct(IHttpContextAccessor contextAccessor, IProductService productService, IProductCategoryService productCategoryService)
        {
            _contextAccessor = contextAccessor;
            _domainName = _contextAccessor.HttpContext.Request.Host.Value;
            _pathUrl = _contextAccessor.HttpContext.Request.Path;
            _productService = productService;
            _productCategoryService = productCategoryService;
        }
        public string BuildLinkDetail(string title, int id)
        {
            string titleRemoveUnicode = StringHandle.RemoveUnicode(title);
            titleRemoveUnicode = Regex.Replace(titleRemoveUnicode, @"[^0-9a-zA-Z\s]+", "");
            if (!string.IsNullOrEmpty(titleRemoveUnicode))
            {
                titleRemoveUnicode = titleRemoveUnicode.ToLower().Replace(" ", "-");
            }
            string url = string.Format("https://{0}/product/{1}/pid-{2}", _domainName, titleRemoveUnicode, id);
            return url;
        }

        public string GetCurrentURL(int? pageIndex)
        {
            string pathBaseUrl = "https://" + _domainName + _pathUrl;
            string url = "";
            if (pageIndex > 1)
                url = pathBaseUrl.Replace("/p" + pageIndex, "");
            else
                url = pathBaseUrl;
            return url;
        }

        public async Task<string> BuildLinkProductSearch(ProductSearch objSearch)
        {
            objSearch.TextSearch = objSearch.TextSearch == null ? string.Empty : objSearch.TextSearch.Trim();
            string urlResult = string.Empty;
            string pathDomain = "https://" + _domainName;
            // Category
            var categoryEntity = await _productCategoryService.GetProductCategoryAsync(objSearch.CategoryId);
            string categoryUrlName = categoryEntity != null ? categoryEntity.UrlName : string.Empty;
            // Price
            if (!string.IsNullOrEmpty(objSearch.PriceMin) && !string.IsNullOrEmpty(objSearch.PriceMax))
            {
                string priceAlias = string.Format("{0}-{1}", objSearch.PriceMin, objSearch.PriceMax);
                if (!string.IsNullOrEmpty(objSearch.TextSearch)){
                    urlResult = string.Format("{0}/product-search/{1}/text-{2}/price-{3}", pathDomain, categoryUrlName, objSearch.TextSearch, priceAlias);
                }
                else
                {
                    urlResult = string.Format("{0}/product-search/{1}/price-{2}", pathDomain, categoryUrlName, priceAlias);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(objSearch.TextSearch))
                {
                    urlResult = string.Format("{0}/product-search/{1}/text-{2}", pathDomain, categoryUrlName, objSearch.TextSearch);
                }
                else
                {
                    urlResult = string.Format("{0}/product-search/{1}", pathDomain, categoryUrlName);
                }
            }
            return urlResult;
        }
    }
}
