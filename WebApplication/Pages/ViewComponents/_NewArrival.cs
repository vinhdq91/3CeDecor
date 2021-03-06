using AutoMapper;
using DAL.Core.Enums;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BuildLink;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_NewArrival")]
    public class _NewArrival : ViewComponent
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private IConfiguration _configuration;
        private readonly int MaxProductDisplay = 12;
        public _NewArrival(
            IProductService productService,
            IImageService imageService,
            IMapper mapper,
            IBuildLinkProduct buildLinkProduct,
            IConfiguration configuration
        )
        {
            _productService = productService;
            _imageService = imageService;
            _mapper = mapper;
            _buildLinkProduct = buildLinkProduct;
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<ProductEntity> listProducts = await _productService.GetListAllProductsByTypeAsync((int)ProductTypeEnum.TopNew);
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();

            var query = from product in listProducts
                        join image in listImages on product.Id equals image.ProductEntityId
                        select new { Product = product, Image = image };

            List<ProductDto> listProductDtos = new List<ProductDto>();
            List<int> productIds = query.Select(x => x.Product.Id).Distinct().ToList();
            foreach (var item in productIds)
            {
                var products = query.ToList().FindAll(x => x.Product.Id == item);
                products[0].Product.UrlPath = _buildLinkProduct.BuildLinkDetail(products[0].Product.Name, products[0].Product.Id);
                ProductDto productDto = _mapper.Map<ProductDto>(products[0].Product);
                if (productDto != null && listProductDtos.Count < MaxProductDisplay)
                {
                    listProductDtos.Add(productDto);
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
                if (productDto != null && listProductDtos.Count < MaxProductDisplay)
                {
                    listProductDtos.Add(productDto);
                }
            }
            ViewBag.PathBase = "~/Resource/AllImages/";
            return View(listProductDtos);
        }
    }
}
