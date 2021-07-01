using DAL.Dtos;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_CommitRuleBox")]
    public class _CommitRuleBox : ViewComponent
    {
        private readonly IProductService _productService;
        public _CommitRuleBox(IProductService productService)
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
