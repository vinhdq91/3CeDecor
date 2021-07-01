using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductCategoryService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Areas.Admin.CommonClasses;
using WebApplication.BuildLink;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TopicAdminController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private readonly IProductCategoryService _productCategoryService;
        private IConfiguration _configuration;

        public TopicAdminController(
            IProductService productService,
            IWebHostEnvironment hostEnvironment,
            IImageService imageService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkProduct buildLinkProduct,
            IProductCategoryService productCategoryService,
            IConfiguration configuration
        )
        {
            _productService = productService;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _buildLinkProduct = buildLinkProduct;
            _productCategoryService = productCategoryService;
            _configuration = configuration;
        }
        public async Task<ActionResult<List<ProductDto>>> TopicList(int topicId)
        {
            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsByTypeAsync(topicId, true);
            //IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            //IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();

            //var query = from product in listProducts
            //            join image in listImages on product.Id equals image.ProductEntityId
            //            join productCategory in listProductCategories on product.ProductCategoryId equals productCategory.Id
            //            select new { Product = product, Image = image };
            await listProducts.Select(e => e.ImageIds).LoadAsync();
            await listProducts.Select(e => e.ProductProductCategories).LoadAsync();

            ProductListModel productListModel = new ProductListModel();
            List<ProductDto> productDtos = new List<ProductDto>();
            List<int> productIds = listProducts.Select(x => x.Id).Distinct().ToList();
            foreach (var item in listProducts.ToList())
            {
                //var products = query.ToList().FindAll(x => x.Product.Id == item);
                item.UrlPath = _buildLinkProduct.BuildLinkDetail(item.Name, item.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(item);
                if (productDto != null)
                {
                    productDtos.Add(productDto);
                }
            }

            IQueryable<ProductEntity> listProductsNoImg = listProducts.Where(x => !productIds.Any(y => y == x.Id));
            foreach (var item in listProductsNoImg.ToList())
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

            productDtos = productDtos.OrderByDescending(x => x.Id).ToList();

            ViewBag.TopicId = topicId;
            return View(productDtos);
        }

        public async Task<ActionResult<List<ProductDto>>> GetProductListNormalType(){
            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsByTypeAsync((int)ProductTypeEnum.Normal, false);
            //IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            //IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();

            //var query = from product in listProducts
            //            join image in listImages on product.Id equals image.ProductEntityId
            //            join productCategory in listProductCategories on product.ProductCategoryId equals productCategory.Id
            //            select new { Product = product, Image = image };
            await listProducts.Select(e => e.ImageIds).LoadAsync();
            await listProducts.Select(e => e.ProductProductCategories).LoadAsync();

            List<ProductDto> productDtos = new List<ProductDto>();
            List<int> productIds = listProducts.Select(x => x.Id).Distinct().ToList();
            foreach (var item in listProducts.ToList())
            {
                //var products = query.ToList().FindAll(x => x.Product.Id == item);
                item.UrlPath = _buildLinkProduct.BuildLinkDetail(item.Name, item.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(item);
                if (productDto != null)
                {
                    productDtos.Add(productDto);
                }
            }

            IQueryable<ProductEntity> listProductsNoImg = listProducts.Where(x => !productIds.Any(y => y == x.Id));
            foreach (var item in listProductsNoImg.ToList())
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

            productDtos = productDtos.OrderByDescending(x => x.Id).ToList();

            return View(productDtos);
        }

        public async Task<JsonResult> UpdateTopicOfProductList(int[] lstProductIds, int topicId)
        {
            try {
                if (lstProductIds.Length > 0)
                {
                    foreach (int item in lstProductIds)
                    {
                        // Update productType
                        ProductEntity productEntity = await _productService.GetProductAsync(item);
                        productEntity.ProductType = topicId;
                        await _productService.UpdateProductNoImageAsync(productEntity);

                        // Save images to topic folder
                        IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();
                        productEntity.ImageIds = lstImages.Where(x => x.ProductEntityId == item).ToList();
                        ProductDto productDto = _mapper.Map<ProductDto>(productEntity);
                        ProductCommonClasses.SaveImageToTopic(topicId, productDto.ImageIds);
                    }
                }
            }
            catch(Exception ex)
            {
                return Json(ex.Message);
            }
            
            return Json(null, new Newtonsoft.Json.JsonSerializerSettings());
        }
        public async Task<ActionResult> RemoveFromTopic(int[] lstProductIds, int topicId)
        {
            if (lstProductIds.Length > 0)
            {
                foreach (int item in lstProductIds)
                {
                    ProductEntity productEntity = await _productService.GetProductAsync(item);
                    IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

                    productEntity.ImageIds = lstImages.ToList().FindAll(x => x.ProductEntityId == productEntity.Id);

                    // Xóa ảnh khỏi thư mục topic tương ứng
                    string folderPath = _hostEnvironment.WebRootPath + "/Resource/" + Enum.GetName(typeof(ProductTypeEnum), topicId);
                    foreach (ImageEntity imageItem in productEntity.ImageIds)
                    {
                        string currentImagePath = Path.Combine(folderPath + "/", imageItem.ImageName);
                        currentImagePath = currentImagePath.Replace(@"/", @"\");
                        if (System.IO.File.Exists(currentImagePath))
                        {
                            System.IO.File.Delete(currentImagePath);
                        }
                    }

                    if (productEntity != null)
                    {
                        productEntity.ProductType = (int)ProductTypeEnum.Normal;
                        await _productService.UpdateProductNoImageAsync(productEntity);
                    }
                }
            }
            return RedirectToAction("TopicList", new { topicId = topicId });
        }
        public async Task<JsonResult> ChangeStatus(int productId)
        {
            ProductEntity productEntity = await _productService.GetProductAsync(productId);
            if (productEntity.Status == (int)ProductStatus.Active)
            {
                productEntity.Status = (int)ProductStatus.UnActive;
            }
            else
            {
                productEntity.Status = (int)ProductStatus.Active;
            }
            try
            {
                await _productService.UpdateProductAsync(productEntity);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return Json(null, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}