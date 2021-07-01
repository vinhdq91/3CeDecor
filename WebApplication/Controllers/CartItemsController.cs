using AutoMapper;
using DAL.Common;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.Models.Account;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly IProductService _productService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        public CartItemsController(
            IProductService productService, 
            IImageService imageService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IConfiguration configuration
        )
        {
            _productService = productService;
            _imageService = imageService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _configuration = configuration;
        }
        public const string CartSessionKey = "3CECartSessionId";
        public IActionResult Index()
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            if (cart == null)
            {
                cart = new List<CartItemDto>();
            }
            ViewBag.Total = cart.Sum(item => (item.Product.Price - (item.Product.Price * item.Product.Discount / 100)) * item.Quantity);
            #region Meta tags
            ViewBag.MetaTitle = "Danh sách giỏ hàng của bạn.";
            ViewBag.MetaDescription = SEO.AddMeta("description", "Cập nhật giỏ hàng của bạn tại đây trước khi thanh toán.");
            #endregion
            return View(cart);
        }

        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            ProductEntity productEntity = await _productService.GetProductAsync(id);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

            productEntity.ImageIds = lstImages.Where(x => x.ProductEntityId == productEntity.Id).ToList();
            productEntity.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;
            ProductDto productDto = new ProductDto();         

            if (productEntity != null)
            {
                productDto = _mapper.Map<ProductDto>(productEntity);
                if (productDto.ImageIds == null || productDto.ImageIds.Count == 0)
                {
                    productDto.ImageIds = new List<ImageDto>();
                    ImageDto imageDto = new ImageDto()
                    {
                        ProductEntityId = productDto.Id,
                        ImageName = _configuration.GetSection("NoImageFile").Value,
                        Title = "No Image"
                    };
                    productDto.ImageIds.Add(imageDto);
                }
            }

            if (SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey) == null)
            {
                List<CartItemDto> cart = new List<CartItemDto>();
                cart.Add(new CartItemDto { Product = productDto, Quantity = quantity });
                SessionHelpers.SetObjectAsJson(HttpContext.Session, CartSessionKey, cart);
            }
            else
            {
                List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartItemDto { Product = productDto, Quantity = quantity });
                }
                SessionHelpers.SetObjectAsJson(HttpContext.Session, CartSessionKey, cart);
            }
            string currentPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;
            return Redirect(currentPath);
        }
        public IActionResult Remove(int id)
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelpers.SetObjectAsJson(HttpContext.Session, CartSessionKey, cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveCartHeaderItem(int id)
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelpers.SetObjectAsJson(HttpContext.Session, CartSessionKey, cart);
            string currentPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;                                  
            ViewBag.Total = cart.Sum(item => item.Product.Price * item.Quantity);
            return Redirect(currentPath);
        }

        public IActionResult ChangeQuantity(int productId, int quantity)
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            cart.Find(item => item.Product.Id == productId).Quantity = quantity;
            SessionHelpers.SetObjectAsJson(HttpContext.Session, CartSessionKey, cart);
            return RedirectToAction("Index");
        }

        private int isExist(int id)
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
