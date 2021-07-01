using AutoMapper;
using DAL.Core.Enums;
using DAL.Core.Utilities;
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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StrategyAdminController : BaseController
    {
        private readonly IProductService _productService;
        private readonly IStrategyService _strategyService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IImageService _imageService;
        private readonly IStrategyProductService _straProService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private IConfiguration _configuration;

        public StrategyAdminController(
            IProductService productService,
            IProductCategoryService productCategoryService,
            IStrategyService strategyService,
            IImageService imageService,
        IStrategyProductService straProService,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration
        )
        {
            _productService = productService;
            _productCategoryService = productCategoryService;
            _strategyService = strategyService;
            _imageService = imageService;
            _straProService = straProService;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public async Task<ActionResult<List<StrategyDto>>> StrategyList()
        {
            IQueryable<StrategyEntity> strategyEntities = await _strategyService.GetListAllStrategysAsync(true);

            List<StrategyDto> listStrategyDto = new List<StrategyDto>();
            if (strategyEntities != null)
            {
                foreach (StrategyEntity strategyitem in strategyEntities.ToList())
                {
                    StrategyDto strategyDto = _mapper.Map<StrategyDto>(strategyitem);
                    if (strategyDto != null)
                    {
                        if (!string.IsNullOrEmpty(strategyDto.ImageName))
                        {
                            strategyDto.ImagePath = Path.Combine("/Resource/StrategyAvatars/", strategyDto.ImageName);
                        }
                        listStrategyDto.Add(strategyDto);
                    }
                }
            }

            return View(listStrategyDto);
        }

        #region Add Strategy
        [HttpGet]
        public IActionResult CreateStrategy()
        {
            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem() { Value = (int)StrategyStatusEnum.Active + "", Text = "Xuất bản" },
                new SelectListItem() { Value = (int)StrategyStatusEnum.UnActive + "", Text = "Chưa Xuất bản" },
                new SelectListItem() { Value = (int)StrategyStatusEnum.OutDate + "", Text = "Hết hạn" }
            };
            ViewBag.ListStatus = listStatus;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStrategy(StrategyDto strategyDto)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu ảnh vào thư mục StrategyAvatar
                if (strategyDto.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(strategyDto.ImageFile.FileName);
                    fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                    string extension = Path.GetExtension(strategyDto.ImageFile.FileName);
                    strategyDto.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Resource/StrategyAvatars/", fileName);
                    if (!Directory.Exists(wwwRootPath + "/Resource/StrategyAvatars"))
                    {
                        Directory.CreateDirectory(wwwRootPath + "/Resource/StrategyAvatars");
                    }
                    if (!Directory.Exists(path))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await strategyDto.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                }
                StrategyEntity strategyEntity = _mapper.Map<StrategyEntity>(strategyDto);
                await _strategyService.AddStrategyAsync(strategyEntity);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("StrategyList");
        }
        #endregion

        #region Edit Strategy
        [HttpGet]
        public async Task<ActionResult<List<StrategyDto>>> EditStrategy(int strategyId)
        {
            StrategyEntity strategyEntity = await _strategyService.GetStrategyAsync(strategyId);
            IQueryable<StrategyProduct> strategyProducts = await _straProService.GetListAllStrategyProductAsync();
            IQueryable<ProductEntity> productEntities = await _productService.GetListAllProductsAsync(true);
            IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            var query = (from straPro in strategyProducts
                                     join product in productEntities on straPro.ProductId equals product.Id
                                     join image in listImages on product.Id equals image.ProductEntityId
                                     //join productCategory in listProductCategories on product.ProductCategoryId equals productCategory.Id
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

            // List Status
            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem() { Value = (int)StrategyStatusEnum.Active + "", Text = "Xuất bản" },
                new SelectListItem() { Value = (int)StrategyStatusEnum.UnActive + "", Text = "Chưa Xuất bản" },
                new SelectListItem() { Value = (int)StrategyStatusEnum.OutDate + "", Text = "Hết hạn" }
            };
            ViewBag.ListStatus = listStatus;
            ViewBag.ListProductDtos = listProductDtos;
            return View(strategyDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditStrategy(StrategyDto strategyDto)
        {
            if (ModelState.IsValid)
            {
                if (strategyDto.ImageFile != null)
                {
                    // Save image to StrategyAvatars
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(strategyDto.ImageFile.FileName);
                    fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                    string extension = Path.GetExtension(strategyDto.ImageFile.FileName);
                    strategyDto.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Resource/StrategyAvatars/", fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await strategyDto.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    // Remove current image in StrategyAvatars
                    //StrategyEntity strategyEntity = await _strategyService.GetStrategyAsync(strategyDto.Id);
                    if (!string.IsNullOrEmpty(strategyDto.ImageNameCurrent))
                    {
                        string currentImagePath = Path.Combine(wwwRootPath + "/Resource/StrategyAvatars/", strategyDto.ImageNameCurrent);
                        currentImagePath = currentImagePath.Replace(@"/", @"\");
                        if (System.IO.File.Exists(currentImagePath))
                        {
                            System.IO.File.Delete(currentImagePath);
                        }
                    }
                }
                // Update Strategy
                StrategyEntity strategyInput = _mapper.Map<StrategyEntity>(strategyDto);
                await _strategyService.UpdateStrategyAsync(strategyInput);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("StrategyList");
        }
        #endregion

        #region Update Product For Strategy
        public async Task<ActionResult<List<ProductDto>>> GetProductListForPopUpStrategy(int strategyId)
        {
            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsAsync(false);
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            IQueryable<ProductCategoryEntity> listProductCategories = await _productCategoryService.GetListAllProductCategoriesAsync();

            var query = from product in listProducts
                        join image in listImages on product.Id equals image.ProductEntityId
                        //join productCategory in listProductCategories on product.ProductCategoryId equals productCategory.Id
                        select new { Product = product, Image = image };

            List<ProductDto> productDtos = new List<ProductDto>();
            List<int> productIds = query.Select(x => x.Product.Id).Distinct().ToList();
            foreach (var item in productIds)
            {
                var products = query.ToList().FindAll(x => x.Product.Id == item);
                ProductDto productDto = _mapper.Map<ProductDto>(products[0].Product);
                if (productDto != null)
                {
                    productDtos.Add(productDto);
                }
            }

            IQueryable<ProductEntity> listProductsNoImg = listProducts.Where(x => !productIds.Any(y => y == x.Id));
            foreach (var item in listProductsNoImg.ToList())
            {
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

            // Loại bỏ các product đã có trong strategy
            IQueryable<StrategyProduct> strategyProducts = await _straProService.GetListProductIdByStrategyIdAsync(strategyId);
            productDtos = productDtos.Where(x => !strategyProducts.Any(y => y.ProductId == x.Id)).ToList();

            return View("GetProductListInPopUp", productDtos);
        }

        public async Task<IActionResult> AddProductListToStrategy(int[] lstProductIds, int strategyId)
        {
            try
            {
                if (lstProductIds.Length > 0)
                {
                    foreach (int item in lstProductIds)
                    {
                        // Add product to strategy
                        StrategyProduct strategyProduct = new StrategyProduct()
                        {
                            ProductId = item,
                            StrategyId = strategyId
                        };
                        await _straProService.AddStrategyProductAsync(strategyProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return RedirectToAction("StrategyList");
        }

        public async Task<IActionResult> RemoveFromStrategy(int[] lstProductIds, int strategyId)
        {
            try
            {
                if (lstProductIds.Length > 0)
                {
                    foreach (int item in lstProductIds)
                    {
                        // Remove product from strategy
                        StrategyProduct strategyProduct = new StrategyProduct()
                        {
                            ProductId = item,
                            StrategyId = strategyId
                        };
                        await _straProService.RemoveStrategyProductAsync(strategyProduct);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return RedirectToAction("EditStrategy", new { strategyId = strategyId });
        }
        #endregion

        public async Task<ActionResult> DeleteStrategy(int[] lstStrategyIds)
        {
            if (lstStrategyIds.Length > 0)
            {
                foreach (int item in lstStrategyIds)
                {
                    StrategyEntity strategyEntity = await _strategyService.GetStrategyAsync(item);

                    if (strategyEntity != null)
                    {
                        if (!string.IsNullOrEmpty(strategyEntity.ImageName))
                        {
                            // Xóa ảnh khỏi StrategyAvatars
                            string currentImagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Resource/StrategyAvatars/", strategyEntity.ImageName);
                            currentImagePath = currentImagePath.Replace(@"/", @"\");
                            if (System.IO.File.Exists(currentImagePath))
                            {
                                System.IO.File.Delete(currentImagePath);
                            }
                        }
                    }
                    // Xóa strategy
                    await _strategyService.RemoveAsync(strategyEntity);
                }
            }
            return RedirectToAction("StrategyList");
        }
        public async Task<JsonResult> ChangeStatus(int strategyId)
        {
            StrategyEntity strategyEntity = await _strategyService.GetStrategyAsync(strategyId);
            if (strategyEntity.Status == (int)StrategyStatusEnum.Active)
            {
                strategyEntity.Status = (int)StrategyStatusEnum.UnActive;
            }
            else
            {
                strategyEntity.Status = (int)StrategyStatusEnum.Active;
            }
            try
            {
                await _strategyService.UpdateStrategyAsync(strategyEntity);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return Json(null, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}
