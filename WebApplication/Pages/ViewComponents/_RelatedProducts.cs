using AutoMapper;
using DAL.Common;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductProductCategoryService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BuildLink;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_RelatedProducts")]
    public class _RelatedProducts : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IProductProductCategoryService _productProductCategoryService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private IConfiguration _configuration;
        private int maxProducts = Constant.RELATEDPRODUCTMAX;

        public _RelatedProducts(
            IProductService productService,
            IProductProductCategoryService productProductCategoryService,
            IWebHostEnvironment hostEnvironment,
            IImageService imageService,
            IMapper mapper,
            IBuildLinkProduct buildLinkProduct,
            IConfiguration configuration
        )
        {
            _productService = productService;
            _productProductCategoryService = productProductCategoryService;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _mapper = mapper;
            _buildLinkProduct = buildLinkProduct;
            _configuration = configuration;
        }

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            IQueryable<ProductEntity> productEntities = await _productService.GetListAllProductsAsync();
            await productEntities.Select(x => x.ProductProductCategories).LoadAsync();
            List<ProductEntity> listProductEntity = productEntities.ToList();

            // Lọc các product liên quan
            List<ProductEntity> listRelated = await FilterForRelatedProducts(productId, listProductEntity);

            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();
            var query = from product in listRelated
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

            IEnumerable<ProductEntity> listProductsNoImg = listRelated.Where(x => !productIds.Any(y => y == x.Id));
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

            // Lấy ảnh từ ProductList luôn vì cùng kích cỡ
            ViewBag.PathBase = "~/Resource/AllImages/";
            return View(listProductDtos);
        }

        protected async Task<List<ProductEntity>> FilterForRelatedProducts(int productId, List<ProductEntity> listProductEntity)
        {
            List<ProductEntity> listRelated = new List<ProductEntity>();
            ProductEntity productInput = await _productService.GetProductAsync(productId);

            var productProductCategoryCats = await _productProductCategoryService.GetListProductProductCategoriesAsync(productId, null);
            productInput.ProductProductCategories = productProductCategoryCats.ToList();

            // Lọc theo category
            listRelated = listProductEntity.FindAll(x => x.ProductProductCategories.Any(y => productInput.ProductProductCategories.Any(z => z.ProductCategoryId == y.ProductCategoryId)));

            // Lọc theo price (Lấy các product có giá trên dưới 30%)
            decimal rangePriceMin = productInput.PriceAfterSale - (productInput.PriceAfterSale * 30 / 100);
            decimal rangePriceMax = productInput.PriceAfterSale + (productInput.PriceAfterSale * 30 / 100);
            List<ProductEntity> listFilterByPrice = listProductEntity.FindAll(x => x.PriceAfterSale >= rangePriceMin && x.PriceAfterSale >= rangePriceMax);

            // Cộng dồn vào listRelated
            if (listFilterByPrice != null && listFilterByPrice.Count > 0)
            {
                foreach (ProductEntity item in listFilterByPrice)
                {
                    if (!listRelated.Any(x => x.Id == item.Id))
                    {
                        listRelated.Add(item);
                    }
                }
            }

            // Bỏ product đầu vào khỏi list (tránh trùng lặp)
            if (listRelated.Any(x => x.Id == productInput.Id))
            {
                listRelated.RemoveAt(listRelated.FindIndex(x => x.Id == productInput.Id));
            }

            // Sắp xếp theo ngày tạo mới nhất
            listRelated = listRelated.OrderByDescending(x => x.CreatedDate).ToList();

            // Lấy ra số lượng tối đa nếu lớn hơn
            if (listRelated.Count > maxProducts)
            {
                listRelated = listRelated.Take(maxProducts).ToList();
            }
            return listRelated;
        }
    }
}
