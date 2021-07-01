using DAL.Dtos;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_LogoTestimonial")]
    public class _LogoTestimonial : ViewComponent
    {
        private readonly IProductService _productService;
        public _LogoTestimonial(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<ProductDto> listProducts1 = new List<ProductDto>();
            return View(listProducts1);
        }
    }
}
