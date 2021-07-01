using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ProductCategoryService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.BuildLink;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductCategoryAdminController : BaseController
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private readonly IProductCategoryService _productCategoryService;
        private IConfiguration _configuration;

        public ProductCategoryAdminController(
            IWebHostEnvironment hostEnvironment,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkProduct buildLinkProduct,
            IProductCategoryService productCategoryService,
            IConfiguration configuration
        )
        {
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _buildLinkProduct = buildLinkProduct;
            _productCategoryService = productCategoryService;
            _configuration = configuration;
        }
        public async Task<ActionResult<List<ProductCategoryDto>>> ProductCategoryList()
        {
            IQueryable<ProductCategoryEntity> listCateEntity = await _productCategoryService.GetListAllProductCategoriesAsync();
            List<ProductCategoryDto> listCateDto = new List<ProductCategoryDto>();

            foreach (ProductCategoryEntity item in listCateEntity.ToList())
            {
                ProductCategoryDto cateDto = _mapper.Map<ProductCategoryDto>(item);
                listCateDto.Add(cateDto);
            }
            return View(listCateDto);
        }

        #region Create
        public ActionResult<ProductCategoryDto> Create()
        {
            ProductCategoryDto productCategoryDto = new ProductCategoryDto();

            return View(productCategoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductCategoryDto productCategoryDto)
        {
            if (ModelState.IsValid)
            {
                productCategoryDto.UrlName = ConvertNameToUrlName(productCategoryDto.Name);
                ProductCategoryEntity productCategoryEntity = _mapper.Map<ProductCategoryEntity>(productCategoryDto);
                await _productCategoryService.AddProductCategoryAsync(productCategoryEntity);
            }
            return RedirectToAction("ProductCategoryList");
        }
        #endregion

        #region Edit
        public async Task<ActionResult<ProductCategoryDto>> Edit(int productCategoryId)
        {
            ProductCategoryEntity productCategoryEntity = await _productCategoryService.GetProductCategoryAsync(productCategoryId);
            ProductCategoryDto productCategoryDto = _mapper.Map<ProductCategoryDto>(productCategoryEntity);

            return View(productCategoryDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductCategoryDto productCategoryDto)
        {
            if (ModelState.IsValid)
            {
                productCategoryDto.UrlName = ConvertNameToUrlName(productCategoryDto.Name);
                ProductCategoryEntity productCategoryEntity = _mapper.Map<ProductCategoryEntity>(productCategoryDto);
                await _productCategoryService.UpdateProductCategoryAsync(productCategoryEntity);
            }
            return RedirectToAction("ProductCategoryList");
        }
        #endregion

        #region Delete
        public async Task<ActionResult> Delete(int productCategoryId)
        {
            ProductCategoryEntity productCategoryEntity = await _productCategoryService.GetProductCategoryAsync(productCategoryId);
            await _productCategoryService.DeleteProductCategoryAsync(productCategoryEntity);

            return RedirectToAction("ProductCategoryList");
        }
        #endregion

        private string ConvertNameToUrlName(string text)
        {
            if (text == null)
                text = string.Empty;

            text = text.Trim().ToLower();
            // Xóa multiple spaces
            text = string.Join(" ", text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            string[] arr1 = new string[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ",
                                            "đ",
                                            "é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
                                            "í","ì","ỉ","ĩ","ị",
                                            "ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
                                            "ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
                                            "ý","ỳ","ỷ","ỹ","ỵ",};
                                                    string[] arr2 = new string[] { "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a", "a",
                                            "d",
                                            "e","e","e","e","e","e","e","e","e","e","e",
                                            "i","i","i","i","i",
                                            "o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
                                            "u","u","u","u","u","u","u","u","u","u","u",
                                            "y","y","y","y","y",};
            for (int i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }

            text = text.Replace(",", "").Replace(".", "").Replace(" ", "-");
            return text;
        }
    }
}