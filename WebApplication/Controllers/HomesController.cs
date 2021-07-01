using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core.Utilities;
using DAL.Models.Application;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Mvc;
using DAL.Common;

namespace WebApplication.Controllers
{
    public class HomesController : Controller
    {
        private readonly IProductService _productService;
        public const string CartSessionKey = "3CECartSessionId";
        public HomesController(IProductService productService)
        {
            _productService = productService;
        }

        [ResponseCache(VaryByHeader = "User-Agent", Duration = 1800)]
        public IActionResult Index()
        {
            ViewBag.MetaTitle = "3Ce Decor, nơi bạn thỏa sức sáng tạo cho căn phòng của mình";
            ViewBag.MetaDescription = SEO.AddMeta("description", "3Ce Decor là cửa hàng chuyên thiết kế và cung cấp các sản phẩm decor treo tường bằng chất liệu gỗ, mang đến cho căn nhà cũng như cửa hiệu của bạn một không gian đầy nghệ thuật và độc đáo.");
            return View();
        }

        public IActionResult Header()
        {
            return View("~/Pages/PartialViews/_Header.cshtml");
        }

        public IActionResult CartHeader()
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            if (cart == null)
            {
                cart = new List<CartItemDto>();
            }
            ViewBag.Total = cart.Sum(item => (item.Product.Price - (item.Product.Price * item.Product.Discount / 100)) * item.Quantity);
            return PartialView("~/Views/Homes/_CartHeader.cshtml", cart);
        }
    }
}
