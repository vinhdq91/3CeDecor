using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Areas.Admin.CommonClasses;
using WebApplication.BuildLink;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize]
    public class ProductAdminController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductProductCategoryService _productProductCategoryService;
        private IConfiguration _configuration;

        public ProductAdminController(
            IProductService productService,
            IWebHostEnvironment hostEnvironment,
            IImageService imageService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkProduct buildLinkProduct,
            IProductCategoryService productCategoryService,
            IProductProductCategoryService productProductCategoryService,
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
            _productProductCategoryService = productProductCategoryService;
        }


        #region ProductList & Detail
        // GET: Admin/ProductAdmin
        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> ProductList()
        {
            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsAsync(true);
            //IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            //IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();

            //var query = from product in listProducts
            //            join image in listImages on product.Id equals image.ProductEntityId
            //            join productCategory in listProductCategories on product.ProductProductCategories equals productCategory.Id
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

            IEnumerable<ProductEntity> listProductsNoImg = listProducts.Where(x => !productIds.Any(y => y == x.Id));
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

            productDtos = productDtos.OrderByDescending(x => x.Id).ToList();
            return View(productDtos);
        }

        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<ProductDto>> Detail(int productId)
        {
            ProductEntity productEntity = await _productService.GetProductAsync(productId);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();
            IQueryable<ProductCategoryEntity> lstCategories = await _productCategoryService.GetListAllProductCategoriesAsync();

            productEntity.ImageIds = lstImages.Where(x => x.ProductEntityId == productId).ToList();
            productEntity.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;

            ProductDto productDto = _mapper.Map<ProductDto>(productEntity);
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
            var productProductCategories = await _productProductCategoryService.GetListProductProductCategoriesAsync(productId, null);
            if (productProductCategories != null)
            {
                var productCategories = await _productCategoryService.GetListAllProductCategoriesAsync();
                productCategories = productCategories.Where(x => productProductCategories.Any(y => y.ProductCategoryId == x.Id));
                foreach (var catItem in productCategories.ToList())
                {
                    ProductCategoryInfo catInfo = new ProductCategoryInfo()
                    {
                        CategoryId = catItem.Id,
                        CategoryName = catItem.Name,
                    };
                    productDto.ProductCategoryInfos.Add(catInfo);
                }
            }
            #endregion

            return View(productDto);
        }
        #endregion

        #region Create
        // GET: Admin/ProductAdmin/Create
        public async Task<ActionResult> Create()
        {
            ProductDto productDto = new ProductDto();

            // Categories
            List<SelectListItem> listCat = new List<SelectListItem>();
            IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();
            foreach (var item in listProductCategories.ToList())
            {
                SelectListItem listCatitem = new SelectListItem() { Value = item.Id + "", Text = item.Name + "" };
                listCat.Add(listCatitem);
            }

            // Status
            List<SelectListItem> listStatus = new List<SelectListItem>();
            SelectListItem statusItem1 = new SelectListItem() { Value = (int)ProductStatus.Active + "", Text = "Xuất bản" };
            SelectListItem statusItem2 = new SelectListItem() { Value = (int)ProductStatus.UnActive + "", Text = "Chưa Xuất bản" };
            listStatus.Add(statusItem1);
            listStatus.Add(statusItem2);

            // Size
            List<SelectListItem> listSize = new List<SelectListItem>();
            SelectListItem sizeItem1 = new SelectListItem() { Value = "1", Text = "Nhỏ" };
            SelectListItem sizeItem2 = new SelectListItem() { Value = "2", Text = "Vừa" };
            SelectListItem sizeItem3 = new SelectListItem() { Value = "3", Text = "Lớn" };
            listSize.Add(sizeItem1);
            listSize.Add(sizeItem2);
            listSize.Add(sizeItem3);

            // Product Topic
            List<SelectListItem> listTopic = new List<SelectListItem>();
            SelectListItem topicItem1 = new SelectListItem() { Value = (int)ProductTypeEnum.Normal + "", Text = "Không chọn" };
            SelectListItem topicItem2 = new SelectListItem() { Value = (int)ProductTypeEnum.Featured + "", Text = "Sản phẩm nổi bật" };
            SelectListItem topicItem3 = new SelectListItem() { Value = (int)ProductTypeEnum.BestSelling + "", Text = "Bán chạy nhất" };
            SelectListItem topicItem4 = new SelectListItem() { Value = (int)ProductTypeEnum.TopRate + "", Text = "Top được yêu thích" };
            SelectListItem topicItem5 = new SelectListItem() { Value = (int)ProductTypeEnum.TopNew + "", Text = "Sản phẩm mới đang hot" };
            listTopic.Add(topicItem1);
            listTopic.Add(topicItem2);
            listTopic.Add(topicItem3);
            listTopic.Add(topicItem4);
            listTopic.Add(topicItem5);

            ViewBag.ListCategories = listCat;
            ViewBag.ListStatus = listStatus;
            ViewBag.ListSize = listSize;
            ViewBag.ListTopic = listTopic;
            return View(productDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                #region Xử lý lưu ảnh vào wwwroot
                if (productDto.ImageIds == null)
                    productDto.ImageIds = new List<ImageDto>();
                foreach (ImageDto item in productDto.ImageIds)
                {
                    //Save image to wwwroot/Resource/AllImages
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(item.ImageFile.FileName);
                    fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                    string extension = Path.GetExtension(item.ImageFile.FileName);
                    item.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Resource/AllImages/", fileName);
                    if (!Directory.Exists(path))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await item.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    // Setup foreign key of Image Table
                    item.ArticleBlogEntityId = null;
                    item.CustomerEntityId = null;
                }
                #endregion

                //// Resize & Lưu ảnh vào topic (nếu chọn)
                //if (productDto.ProductType > 0)
                //{
                //    ProductCommonClasses.SaveImageToTopic(productDto.ProductType, productDto.ImageIds);
                //}
                //// Lưu ảnh vào Product List
                //ProductCommonClasses.SaveImageToTopic((int)ProductTypeEnum.ProductList, productDto.ImageIds);

                //productDto.Description = Common.RemoveHTMLTag(productDto.Description);

                ProductEntity productEntity = _mapper.Map<ProductEntity>(productDto);
                productEntity.ProductProductCategories = new List<ProductProductCategory>();
                foreach (var catIdItem in productDto.ProductCategoryIds)
                {
                    ProductProductCategory item = new ProductProductCategory()
                    {
                        ProductCategoryId = catIdItem
                    };
                    productEntity.ProductProductCategories.Add(item);
                }
                await _productService.AddProductAsync(productEntity);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
                return RedirectToAction("Create");
            }
            return RedirectToAction("ProductList");
        }
        #endregion

        #region Edit
        // GET: Admin/ProductAdmin/Edit/5
        public async Task<ActionResult> Edit(int productId)
        {
            ProductEntity productEntity = await _productService.GetProductAsync(productId);
            var productProductCategories = await _productProductCategoryService.GetListProductProductCategoriesAsync(productId, null);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

            productEntity.ImageIds = lstImages.Where(x => x.ProductEntityId == productEntity.Id).ToList();
            productEntity.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;

            ProductDto productDto = _mapper.Map<ProductDto>(productEntity);
            productDto.ProductCurrentType = productDto.ProductType;
            
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

            productDto.ProductCategoryIds = new List<int>();
            foreach (var productProductCatItem in productProductCategories.ToList())
            {
                productDto.ProductCategoryIds.Add(productProductCatItem.ProductCategoryId);
            }

            List<SelectListItem> listCat = new List<SelectListItem>();
            IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();
            foreach (var item in listProductCategories.ToList())
            {
                SelectListItem listCatitem = new SelectListItem() { Value = item.Id + "", Text = item.Name + "" };
                listCat.Add(listCatitem);
            }

            // Status
            List<SelectListItem> listStatus = new List<SelectListItem>();
            SelectListItem statusItem1 = new SelectListItem() { Value = (int)ProductStatus.Active + "", Text = "Xuất bản" };
            SelectListItem statusItem2 = new SelectListItem() { Value = (int)ProductStatus.UnActive + "", Text = "Chưa Xuất bản" };
            listStatus.Add(statusItem1);
            listStatus.Add(statusItem2);

            // Size
            List<SelectListItem> listSize = new List<SelectListItem>();
            SelectListItem sizeItem1 = new SelectListItem() { Value = "1", Text = "Nhỏ" };
            SelectListItem sizeItem2 = new SelectListItem() { Value = "2", Text = "Vừa" };
            SelectListItem sizeItem3 = new SelectListItem() { Value = "3", Text = "Lớn" };
            listSize.Add(sizeItem1);
            listSize.Add(sizeItem2);
            listSize.Add(sizeItem3);

            // Product Topic
            List<SelectListItem> listTopic = new List<SelectListItem>();
            SelectListItem topicItem1 = new SelectListItem() { Value = (int)ProductTypeEnum.Normal + "", Text = "Không chọn" };
            SelectListItem topicItem2 = new SelectListItem() { Value = (int)ProductTypeEnum.Featured + "", Text = "Sản phẩm nổi bật" };
            SelectListItem topicItem3 = new SelectListItem() { Value = (int)ProductTypeEnum.BestSelling + "", Text = "Sản phẩm bán chạy" };
            SelectListItem topicItem4 = new SelectListItem() { Value = (int)ProductTypeEnum.TopRate + "", Text = "Sản phẩm được bình chọn" };
            SelectListItem topicItem5 = new SelectListItem() { Value = (int)ProductTypeEnum.TopNew + "", Text = "Sản phẩm mới nổi bật" };
            listTopic.Add(topicItem1);
            listTopic.Add(topicItem2);
            listTopic.Add(topicItem3);
            listTopic.Add(topicItem4);
            listTopic.Add(topicItem5);

            ViewBag.ListCategories = listCat;
            ViewBag.ListStatus = listStatus;
            ViewBag.ListSize = listSize;
            ViewBag.ListTopic = listTopic;
            return View(productDto);
        }

        // POST: Admin/ProductAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                #region Xử lý lưu ảnh vào wwwroot
                if (productDto.ImageIds == null)
                    productDto.ImageIds = new List<ImageDto>();
                foreach (ImageDto item in productDto.ImageIds)
                {
                    if (item.ImageFile != null)
                    {
                        //Save image to wwwroot/Resource/AllImages
                        string fileName = Path.GetFileNameWithoutExtension(item.ImageFile.FileName);
                        fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                        string extension = Path.GetExtension(item.ImageFile.FileName);
                        item.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Resource/AllImages/", fileName);
                        if (!System.IO.File.Exists(path))
                        {
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                await item.ImageFile.CopyToAsync(fileStream);
                            }
                        }

                        // Check thay ảnh cũ hay thêm ảnh mới
                        if (item.ImageId > 0)
                        {
                            // Xóa ảnh hiện tại trong AllImages và ProductList
                            ImageEntity imageEntity = await _imageService.GetImageAsync(item.ImageId);
                            string currentImagePath = Path.Combine(wwwRootPath + "/Resource/AllImages/", imageEntity.ImageName);
                            //string currentImagePathInProductList = Path.Combine(wwwRootPath + "/Resource/ProductList/", imageEntity.ImageName);
                            currentImagePath = currentImagePath.Replace(@"/", @"\");
                            //currentImagePathInProductList = currentImagePathInProductList.Replace(@"/", @"\");
                            if (System.IO.File.Exists(currentImagePath))
                            {
                                System.IO.File.Delete(currentImagePath);
                            }
                            //if (System.IO.File.Exists(currentImagePathInProductList))
                            //{
                            //    System.IO.File.Delete(currentImagePathInProductList);
                            //}

                            // Update to db ImageEntities
                            imageEntity.ImageName = item.ImageName;
                            imageEntity.UpdatedDate = DateTime.Now;
                            await _imageService.UpdateImageAsync(imageEntity);
                        }
                        else
                        {
                            // Add to db ImageEntities
                            ImageEntity imageEntity = _mapper.Map<ImageEntity>(item);
                            imageEntity.CreatedDate = DateTime.Now;
                            // Setup foreign key of Image Table
                            imageEntity.ProductEntityId = productDto.Id;
                            imageEntity.ArticleBlogEntityId = null;
                            imageEntity.CustomerEntityId = null;
                            await _imageService.AddImageAsync(imageEntity);
                        }
                    }

                    // Xóa ảnh hiện tại trong topic folder
                    //if (!string.IsNullOrEmpty(item.ImageName))
                    //{
                    //    int topicCurrentId = productDto.ProductCurrentType;  // Lấy ProductType hiện tại
                    //    string folderPath = _hostEnvironment.WebRootPath + "/Resource/" + Enum.GetName(typeof(ProductTypeEnum), topicCurrentId);
                    //    string currentImageTopicPath = Path.Combine(folderPath + "/", item.ImageName);
                    //    currentImageTopicPath = currentImageTopicPath.Replace(@"/", @"\");
                    //    if (System.IO.File.Exists(currentImageTopicPath))
                    //    {
                    //        System.IO.File.Delete(currentImageTopicPath);
                    //    }
                    //}
                }
                #endregion

                // Resize & Lưu ảnh vào product list 
                //ProductCommonClasses.SaveImageToTopic((int)ProductTypeEnum.ProductList, productDto.ImageIds);

                // Resize & Lưu ảnh vào topic (nếu chọn)
                //if (productDto.ProductType > 0)
                //{
                //    ProductCommonClasses.SaveImageToTopic(productDto.ProductType, productDto.ImageIds);
                //}

                // Update to db Product
                productDto.UpdatedDate = DateTime.Now;
                ProductEntity productEntity = _mapper.Map<ProductEntity>(productDto);

                #region Update ProductProductCategories
                var productProductCats = await _productProductCategoryService.GetListProductProductCategoriesAsync(productEntity.Id, null);
                var deleteProductProductCats = productProductCats.Where(x => !productDto.ProductCategoryIds.Any(y => y == x.ProductCategoryId)).ToList();
                foreach (var catItem in deleteProductProductCats)
                {
                    await _productProductCategoryService.DeleteProductProductCategoryAsync(catItem);
                }

                var newProductProductCats = productDto.ProductCategoryIds.Where(x => !productProductCats.Any(y => y.ProductCategoryId == x)).ToList();
                foreach (var catIdItem in newProductProductCats)
                {
                    var item = new ProductProductCategory { ProductId = productEntity.Id, ProductCategoryId = catIdItem };
                    await _productProductCategoryService.AddProductProductCategoryAsync(item);
                }
                #endregion

                await _productService.UpdateProductAsync(productEntity);
            }
            else
            {
                return RedirectToAction("Edit");
            }
            return RedirectToAction("ProductList");
        }
        #endregion

        //// GET: Admin/ProductAdmin/Delete/5
        public async Task<ActionResult> Delete(int[] lstProductIds)
        {
            if (lstProductIds.Length > 0)
            {
                foreach (int item in lstProductIds)
                {
                    ProductEntity productEntity = await _productService.GetProductAsync(item);
                    
                    if (productEntity != null)
                    {
                        IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();
                        productEntity.ImageIds = lstImages.Where(x => x.ProductEntityId == item).ToList();
                        if (productEntity.ImageIds != null)
                        {
                            foreach (ImageEntity imgItem in productEntity.ImageIds)
                            {
                                if (!string.IsNullOrEmpty(imgItem.ImageName))
                                {
                                    // Xóa ảnh khỏi AllImages và ProductList
                                    string currentImagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Resource/AllImages/", imgItem.ImageName);
                                    //string currentImagePathInProductList = Path.Combine(_hostEnvironment.WebRootPath + "/Resource/ProductList/", imgItem.ImageName);
                                    currentImagePath = currentImagePath.Replace(@"/", @"\");
                                    //currentImagePathInProductList = currentImagePathInProductList.Replace(@"/", @"\");
                                    if (System.IO.File.Exists(currentImagePath))
                                    {
                                        System.IO.File.Delete(currentImagePath);
                                    }
                                    //if (System.IO.File.Exists(currentImagePathInProductList))
                                    //{
                                    //    System.IO.File.Delete(currentImagePathInProductList);
                                    //}

                                    // Xóa ảnh khỏi topic folder (nếu có)
                                    //if (productEntity.ProductType > 0)
                                    //{
                                    //    string folderPath = _hostEnvironment.WebRootPath + "/Resource/" + Enum.GetName(typeof(ProductTypeEnum), productEntity.ProductType);
                                    //    string currentImageTopicPath = Path.Combine(folderPath + "/", imgItem.ImageName);
                                    //    currentImageTopicPath = currentImageTopicPath.Replace(@"/", @"\");
                                    //    if (System.IO.File.Exists(currentImageTopicPath))
                                    //    {
                                    //        System.IO.File.Delete(currentImageTopicPath);
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                    // Xóa product
                    await _productService.RemoveAsync(productEntity);
                }
            }
            return RedirectToAction("ProductList");
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
