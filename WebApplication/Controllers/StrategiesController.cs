using AutoMapper;
using DAL.Common;
using DAL.Core.Enums;
using DAL.Core.Model;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductCategoryService;
using DAL.UnitOfWorks.ProductService;
using DAL.UnitOfWorks.StrategyProductService;
using DAL.UnitOfWorks.StrategyService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BuildLink;
using WebApplication.Libraries;

namespace WebApplication.Controllers
{
    public class StrategiesController : Controller
    {
        private readonly IStrategyService _strategyService;
        private readonly IStrategyProductService _straProService;
        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private IConfiguration _configuration;
        public StrategiesController (
            IStrategyService strategyService,
            IStrategyProductService straProService,
            IProductService productService,
            IProductCategoryService productCategoryService,
            IWebHostEnvironment hostEnvironment,
            IImageService imageService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkProduct buildLinkProduct,
            IConfiguration configuration
        )
        {
            _strategyService = strategyService;
            _straProService = straProService;
            _productService = productService;
            _productCategoryService = productCategoryService;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _buildLinkProduct = buildLinkProduct;
            _configuration = configuration;
        }

        public async Task<ActionResult<ProductListModel>> StrategyDetail(string strategyName, int strategyId, int pageIndex = 1)
        {
            StrategyEntity strategyEntity = await _strategyService.GetStrategyAsync(strategyId);
            IQueryable<StrategyProduct> strategyProducts = await _straProService.GetListAllStrategyProductAsync();
            IQueryable<ProductEntity> productEntities = await _productService.GetListAllProductsAsync(true);
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            var query = (from straPro in strategyProducts
                         join product in productEntities on straPro.ProductId equals product.Id
                         join image in listImages on product.Id equals image.ProductEntityId
                         where straPro.StrategyId == strategyEntity.Id
                         select new { Product = product, Image = image });

            StrategyDto strategyDto = _mapper.Map<StrategyDto>(strategyEntity);
            strategyDto.ImageNameCurrent = !string.IsNullOrEmpty(strategyDto.ImageName) ? strategyDto.ImageName : string.Empty;

            List<ProductDto> listProductDtos = new List<ProductDto>();
            List<int> productIds = query.Select(x => x.Product.Id).Distinct().ToList();
            foreach (var item in productIds)
            {
                var products = query.ToList().FindAll(x => x.Product.Id == item);
                ProductDto productDto = _mapper.Map<ProductDto>(products[0].Product);
                if (productDto != null)
                {
                    listProductDtos.Add(productDto);
                }
            }
            //IEnumerable<ProductEntity> listProductsNoImg = lstProductEntity.Where(x => !productIds.Any(y => y == x.Id));
            //foreach (var item in listProductsNoImg)
            //{
            //    ProductDto productDto = _mapper.Map<ProductDto>(item);
            //    ImageDto imageDto = new ImageDto()
            //    {
            //        ProductEntityId = item.Id,
            //        ImageName = _configuration.GetSection("NoImageFile").Value,
            //        Title = "No Image"
            //    };
            //    productDto.ImageIds.Add(imageDto);
            //    listProductDtos.Add(productDto);
            //}

            ProductListModel productListModel = new ProductListModel();
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
            paging.Count = listProductDtos.Count;
            paging.Sort = sort;
            productListModel.Paging = paging;
            #endregion

            #region Meta tags
            ViewBag.MetaTitle = strategyEntity.MetaTitle;
            ViewBag.MetaDescription = SEO.AddMeta("description", strategyEntity.MetaDescription);
            #endregion

            productListModel.ListProducts = Filter.GetOrderProducts(listProductDtos, paging.Sort).Skip((paging.PageIndex - 1) * paging.PageSize)
                                                       .Take(paging.PageSize).ToList();

            ViewBag.TextSearch = "";
            ViewBag.CategoryId = (int)ProductCategoryEnum.All;

            var lstProductCategory = await _productCategoryService.GetListAllProductCategoriesAsync();
            ViewBag.ListProductCategory = lstProductCategory.ToList();
            return View(productListModel);
        }
    }
}
