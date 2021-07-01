using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Common;
using DAL.Core.Enums;
using DAL.Core.Model;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductCategoryService;
using DAL.UnitOfWorks.ProductProductCategoryService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.BuildLink;
using WebApplication.Libraries;

namespace WebApplication.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductProductCategoryService _productProductCategoryService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private readonly IBuildLinkSeo _buildLinkSeo;
        private IConfiguration _configuration;
        public ProductsController(
            IProductService productService,
            IProductCategoryService productCategoryService,
            IProductProductCategoryService productProductCategoryService,
            IWebHostEnvironment hostEnvironment,
            IImageService imageService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkProduct buildLinkProduct,
            IBuildLinkSeo buildLinkSeo,
            IConfiguration configuration
        )
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _productProductCategoryService = productProductCategoryService;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _buildLinkProduct = buildLinkProduct;
            _buildLinkSeo = buildLinkSeo;
            _configuration = configuration;
        }
        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> ProductList(int pageIndex = 1)
        {
            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsAsync();
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();

            var query = from product in listProducts
                        join image in listImages on product.Id equals image.ProductEntityId
                        select new { Product = product, Image = image };

            ProductListModel productListModel = new ProductListModel();
            List<ProductDto> productDtos = new List<ProductDto>();
            List<int> productIds = query.Select(x => x.Product.Id).Distinct().ToList();
            foreach (var item in productIds)
            {
                var products = query.ToList().FindAll(x => x.Product.Id == item);
                products[0].Product.UrlPath = _buildLinkProduct.BuildLinkDetail(products[0].Product.Name, products[0].Product.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(products[0].Product);
                if (productDto != null)
                {
                    productDtos.Add(productDto);
                }
            }

            IQueryable<ProductEntity> listProductsNoImg = listProducts.Where(x => !productIds.Any(y => y == x.Id));
            foreach (var item in listProductsNoImg)
            {
                item.UrlPath = _buildLinkProduct.BuildLinkDetail(item.Name, item.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(item);
                ImageDto imageDto = new ImageDto()
                {
                    ProductEntityId = item.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                productDto.ImageIds.Add(imageDto);
                productDtos.Add(productDto);
            }

            #region Sort
            int sort = 0;
            if (Request.Cookies["SortProductId"] != null)
            {
                sort = Convert.ToInt32(Request.Cookies["SortProductId"]);
            }
            #endregion

            #region Pagination
            PagingInfo paging = new PagingInfo();
            paging.PageSize = Convert.ToInt32(_configuration.GetSection("ProductPageSize").Value);
            paging.PageIndex = pageIndex;
            paging.LinkSite = _buildLinkProduct.GetCurrentURL(paging.PageIndex);
            paging.Count = productDtos.Count;
            paging.Sort = sort;
            productListModel.Paging = paging;
            #endregion

            #region Meta tags
            ViewBag.MetaTitle = "Danh sách sản phẩm của 3CE Decor";
            ViewBag.MetaDescription = SEO.AddMeta("description", "3Ce Decor là cửa hàng chuyên thiết kế và cung cấp các sản phẩm decor treo tường bằng chất liệu gỗ, mang đến cho căn nhà cũng như cửa hiệu của bạn một không gian đầy nghệ thuật và độc đáo.");
            #endregion

            productListModel.ListProducts = Filter.GetOrderProducts(productDtos, paging.Sort).Skip((paging.PageIndex - 1) * paging.PageSize)
                                                       .Take(paging.PageSize).ToList();

            ViewBag.TextSearch = "";
            ViewBag.CategoryId = (int)ProductCategoryEnum.All;
            var productCategories = await _productCategoryService.GetListAllProductCategoriesAsync();
            ViewBag.ListProductCategory = productCategories.ToList();
            return View(productListModel);
        }

        public async Task<ActionResult<IEnumerable<ProductDto>>> ProductSearch(ProductSearch objSearch)
        {
            int pageIndex = objSearch.PageIndex == 0 ? 1 : objSearch.PageIndex;

            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsAsync();
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();

            if (objSearch.CategoryId == 0)
            {
                var lstCategory = await _productCategoryService.GetListAllProductCategoriesAsync();
                objSearch.CategoryId = lstCategory.ToList().FirstOrDefault(x => x.UrlName == objSearch.CategoryName) != null ?
                                        lstCategory.ToList().FirstOrDefault(x => x.UrlName == objSearch.CategoryName).Id : 0;
            }

            var query = from product in listProducts
                        join image in listImages on product.Id equals image.ProductEntityId
                        select new { Product = product, Image = image };

            ProductListModel productListModel = new ProductListModel();
            List<ProductDto> productDtos = new List<ProductDto>();
            List<int> productIds = query.Select(x => x.Product.Id).Distinct().ToList();
            var productProductCategories = await _productProductCategoryService.GetListAllProductProductCategoriesAsync();
            foreach (var item in productIds)
            {
                var products = query.Where(x => x.Product.Id == item).ToList();
                products[0].Product.UrlPath = _buildLinkProduct.BuildLinkDetail(products[0].Product.Name, products[0].Product.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(products[0].Product);
                if (productDto != null)
                {
                    productDtos.Add(productDto);
                }

                var productProductCateIds = productProductCategories.Where(x => x.ProductId == item);
                productDto.ProductCategoryIds = productProductCateIds.Select(x => x.ProductCategoryId).ToList();
            }

            IQueryable<ProductEntity> listProductsNoImg = listProducts.Where(x => !productIds.Any(y => y == x.Id));
            foreach (var item in listProductsNoImg)
            {
                item.UrlPath = _buildLinkProduct.BuildLinkDetail(item.Name, item.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(item);
                ImageDto imageDto = new ImageDto()
                {
                    ProductEntityId = item.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                productDto.ImageIds.Add(imageDto);
                productDtos.Add(productDto);
            }

            #region Sort
            int sort = 0;
            if (Request.Cookies["SortProductId"] != null)
            {
                sort = Convert.ToInt32(Request.Cookies["SortProductId"]);
            }
            #endregion

            #region Pagination
            PagingInfo paging = new PagingInfo();
            paging.PageSize = Convert.ToInt32(_configuration.GetSection("ProductPageSize").Value);
            paging.PageIndex = pageIndex;
            paging.LinkSite = _buildLinkProduct.GetCurrentURL(paging.PageIndex);
            paging.Sort = sort;
            var allProducts = await _productService.GetListAllProductsAsync();
            await allProducts.Select(x => x.ProductProductCategories).LoadAsync();
            #endregion

            #region Meta tags
            string buildMetaSearch = objSearch.CategoryId > 0 ? "Sản phẩm " + objSearch.CategoryName : "Tất cả sản phẩm";
            buildMetaSearch += !string.IsNullOrEmpty(objSearch.TextSearch) ? ", Từ khóa: " + objSearch.TextSearch : "";
            buildMetaSearch += !string.IsNullOrEmpty(objSearch.PriceMin) && !string.IsNullOrEmpty(objSearch.PriceMax) ? ", Giá trong khoảng: " + objSearch.PriceMin + "-" + objSearch.PriceMax : "";
            ViewBag.MetaTitle = buildMetaSearch;
            ViewBag.MetaDescription = SEO.AddMeta("description", buildMetaSearch);
            #endregion

            productListModel = Filter.FilterProductSearch(objSearch, paging, allProducts.ToList(), productDtos);

            ViewBag.TextSearch = objSearch.TextSearch;
            ViewBag.CategoryId = objSearch.CategoryId;
            ViewBag.PriceMin = objSearch.PriceMin;
            ViewBag.PriceMax = objSearch.PriceMax;

            var lstProductCategory = await _productCategoryService.GetListAllProductCategoriesAsync();
            ViewBag.ListProductCategory = lstProductCategory.ToList();

            return View(productListModel);
        }

        [HttpGet]
        public async Task<ActionResult> ProductBoxSearch(int categoryId = 0, string textSearch = "")
        {
            var productCategoryList = await _productCategoryService.GetListAllProductCategoriesAsync();
            List<ProductCategoryDto> productCategoryDtos = _mapper.Map<List<ProductCategoryDto>>(productCategoryList.ToList());
            ProductSearch objSearch = new ProductSearch
            {
                CategoryId = categoryId,
                TextSearch = string.IsNullOrEmpty(textSearch) ? "" : textSearch.Replace("-", " "),
                ProductCategoryDtos = productCategoryDtos
            };
            return PartialView("_ProductBoxSearch", objSearch);
        }

        [HttpGet]
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<ProductDto>> ProductDetail(int id, string title = "")
        {
            ProductEntity product = await _productService.GetProductAsync(id);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

            product.ImageIds = lstImages.Where(x => x.ProductEntityId == product.Id).ToList();
            product.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;

            ProductDto productDto = _mapper.Map<ProductDto>(product);
            if (productDto.ImageIds.Count == 0)
            {
                ImageDto imageDto = new ImageDto()
                {
                    ProductEntityId = productDto.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                productDto.ImageIds.Add(imageDto);
            }

            #region Xử lý ProductCategoryInfo
            var productProductCategories = await _productProductCategoryService.GetListProductProductCategoriesAsync(id, null);
            if (productProductCategories != null)
            {
                var productCategories = await _productCategoryService.GetListAllProductCategoriesAsync();
                productCategories = productCategories.Where(x => productProductCategories.Any(y => y.ProductCategoryId == x.Id));
                foreach(var catItem in productCategories.ToList())
                {
                    ProductCategoryInfo catInfo = new ProductCategoryInfo() {
                        CategoryId = catItem.Id,
                        CategoryName = catItem.Name,
                        CategoryUrl = ("/product-search/" + StringHandle.RemoveUnicode(catItem.Name).Replace(" ", "-")).ToLower()
                    };
                    productDto.ProductCategoryInfos.Add(catInfo);
                }
            }
            #endregion

            #region Meta tags
            ViewBag.MetaTitle = product.MetaTitle;
            ViewBag.MetaDescription = SEO.AddMeta("description", product.MetaDescription);
            #endregion

            ViewBag.TextSearch = "";
            ViewBag.CategoryId = (int)ProductCategoryEnum.All;
            ViewBag.MetaFacebook = _buildLinkSeo.AddMetaFacebook(
                                                                product.Name, 
                                                                product.Description, 
                                                                product.UrlPath, 
                                                                product.UrlPath + "/Resource/AllImages/" + product.ImageIds[0].ImageName
                                                            );
            ViewBag.ProductDetailUrl = _buildLinkProduct.BuildLinkDetail(product.Name, product.Id);
            return View(productDto);
        }

        [HttpGet]
        public IActionResult BoxFilterByPrice(int categoryId = 0, string textSearch = "", string priceMin = null, string priceMax = null)
        {
            ProductSearch objSearch = new ProductSearch
            {
                CategoryId = categoryId,
                TextSearch = string.IsNullOrEmpty(textSearch) ? "" : textSearch.Replace("-", " "),
                PriceMin = priceMin == null ? Constant.PRICEMIN : priceMin,
                PriceMax = priceMax == null ? Constant.PRICEMAX : priceMax
            };
            return PartialView("_BoxFilterByPrice", objSearch);
        }

        [HttpPost]
        public async Task<IActionResult> BoxFilterByPrice(ProductSearch objSearch)
        {
            string urlResult = await _buildLinkProduct.BuildLinkProductSearch(objSearch);
            if (!string.IsNullOrEmpty(urlResult))
                return Redirect(urlResult);
            return null;
        }

        #region Box in list (Không dùng được viewcomponent vì lỗi path)
        public async Task<ActionResult<List<ProductDto>>> BoxLatestProducts(ProductSearch objSearch)
        {
            int numOfProducts = 3;
            IQueryable<ProductEntity> productEntities = await _productService.GetListAllProductsLatestAsync(numOfProducts);
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();

            var query = from product in productEntities
                        join image in listImages on product.Id equals image.ProductEntityId
                        select new { Product = product, Image = image };

            List<ProductDto> listProductDtos = new List<ProductDto>();
            List<int> productIds = query.Select(x => x.Product.Id).Distinct().ToList();
            foreach (var item in productIds)
            {
                var products = query.ToList().FindAll(x => x.Product.Id == item);
                products[0].Product.UrlPath = _buildLinkProduct.BuildLinkDetail(products[0].Product.Name, products[0].Product.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(products[0].Product);
                if (productDto != null)
                {
                    listProductDtos.Add(productDto);
                }
            }

            IQueryable<ProductEntity> listProductsNoImg = productEntities.Where(x => !productIds.Any(y => y == x.Id));
            foreach (var item in listProductsNoImg)
            {
                item.UrlPath = _buildLinkProduct.BuildLinkDetail(item.Name, item.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(item);
                ImageDto imageDto = new ImageDto()
                {
                    ProductEntityId = item.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                productDto.ImageIds.Add(imageDto);
                listProductDtos.Add(productDto);
            }

            // Lấy từ ProductList rồi co ảnh lại bằng css vì box này ảnh có kích thước nhỏ
            string domainName = "https://" + Request.Host.Value;
            ViewBag.PathBase = domainName + "/Resource/AllImages/";
            return View("_BoxLatestProducts", listProductDtos);
        }
        #endregion
    }
}