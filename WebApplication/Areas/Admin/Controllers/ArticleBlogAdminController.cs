using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Core.Enums;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ArticleBlogService;
using DAL.UnitOfWorks.ImageService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using WebApplication.BuildLink;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticleBlogAdminController : BaseController
    {
        private readonly IArticleBlogService _articleService;
        private readonly IImageService _imageService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkArticleBlog _buildLinkArticle;
        private IConfiguration _configuration;

        public ArticleBlogAdminController (
            IArticleBlogService articleService,
            IWebHostEnvironment hostEnvironment,
            IImageService imageService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkArticleBlog buildLinkArticle,
            IConfiguration configuration
        )
        {
            _articleService = articleService;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _buildLinkArticle = buildLinkArticle;
            _configuration = configuration;
        }
        #region List & Detail
        public async Task<ActionResult<ArticleBlogDto>> ArticleBlogList()
        {
            IQueryable<ArticleBlogEntity> listArticles = await _articleService.GetListAllArticlesAsync();
            IQueryable<ImageEntity> listImages = await _imageService.GetListAllImagesAsync();

            var query = from article in listArticles
                        join image in listImages on article.Id equals image.ArticleBlogEntityId
                        select new { Article = article, Image = image };

            List<ArticleBlogDto> articleDtos = new List<ArticleBlogDto>();
            List<int> articleIds = query.Select(x => x.Article.Id).Distinct().ToList();
            foreach (int item in articleIds)
            {
                var articles = query.ToList().FindAll(x => x.Article.Id == item);
                articles[0].Article.UrlPath = _buildLinkArticle.BuildLinkDetail(articles[0].Article.Title, articles[0].Article.Id);
                ArticleBlogDto articleDto = _mapper.Map<ArticleBlogDto>(articles[0].Article);
                if (articleDto != null)
                {
                    articleDtos.Add(articleDto);
                }
            }

            IEnumerable<ArticleBlogEntity> listArticlesNoImg = listArticles.Where(x => !articleIds.Any(y => y == x.Id));
            foreach (var item in listArticlesNoImg)
            {
                item.UrlPath = _buildLinkArticle.BuildLinkDetail(item.Title, item.Id);
                ArticleBlogDto articleDto = _mapper.Map<ArticleBlogDto>(item);
                ImageDto imageDto = new ImageDto()
                {
                    ArticleBlogEntityId = articleDto.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                articleDto.ImageIds.Add(imageDto);
                articleDtos.Add(articleDto);
            }
            articleDtos = articleDtos.OrderByDescending(x => x.Id).ToList();
            return View(articleDtos);
        }

        public async Task<ActionResult<ArticleBlogDto>> Detail(int articleBlogId)
        {
            ArticleBlogEntity articleBlogEntity = await _articleService.GetArticleAsync(articleBlogId);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

            articleBlogEntity.ImageIds = lstImages.ToList().FindAll(x => x.ArticleBlogEntityId == articleBlogId);
            articleBlogEntity.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;

            ArticleBlogDto articleBlogDto = _mapper.Map<ArticleBlogDto>(articleBlogEntity);
            if (articleBlogDto.ImageIds.Count == 0)
            {
                ImageDto imageDto = new ImageDto()
                {
                    ArticleBlogEntityId = articleBlogDto.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                articleBlogDto.ImageIds.Add(imageDto);
            }

            return View(articleBlogDto);
        } 
        #endregion

        #region Create
        public ActionResult<ArticleBlogDto> Create()
        {
            ArticleBlogDto articleBlogDto = new ArticleBlogDto();

            // Status
            List<SelectListItem> listStatus = new List<SelectListItem>();
            SelectListItem statusItem1 = new SelectListItem() { Value = (int)ArticleBlogStatus.Active + "", Text = "Xuất bản" };
            SelectListItem statusItem2 = new SelectListItem() { Value = (int)ArticleBlogStatus.UnActive + "", Text = "Chưa Xuất bản" };
            listStatus.Add(statusItem1);
            listStatus.Add(statusItem2);

            ViewBag.ListStatus = listStatus;
            return View(articleBlogDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ArticleBlogDto articleBlogDto)
        {
            if (ModelState.IsValid)
            {
                #region Xử lý lưu ảnh vào wwwroot
                if (articleBlogDto.ImageIds == null)
                    articleBlogDto.ImageIds = new List<ImageDto>();
                foreach (ImageDto item in articleBlogDto.ImageIds)
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
                    item.CustomerEntityId = null;
                    item.ProductEntityId = null;
                }
                #endregion

                ArticleBlogEntity articleBlogEntity = _mapper.Map<ArticleBlogEntity>(articleBlogDto);
                await _articleService.AddArticleAsync(articleBlogEntity);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("ArticleBlogList");
        }
        #endregion

        #region Edit
        public async Task<ActionResult<ArticleBlogDto>> Edit(int articleBlogId)
        {
            ArticleBlogEntity articleBlogEntity = await _articleService.GetArticleAsync(articleBlogId);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

            articleBlogEntity.ImageIds = lstImages.ToList().FindAll(x => x.ArticleBlogEntityId == articleBlogEntity.Id);
            articleBlogEntity.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;

            ArticleBlogDto articleBlogtDto = _mapper.Map<ArticleBlogDto>(articleBlogEntity);
            if (articleBlogtDto.ImageIds.Count == 0)
            {
                ImageDto imageDto = new ImageDto()
                {
                    ArticleBlogEntityId = articleBlogtDto.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                articleBlogtDto.ImageIds.Add(imageDto);
            }

            // Status
            List<SelectListItem> listStatus = new List<SelectListItem>();
            SelectListItem statusItem1 = new SelectListItem() { Value = (int)ProductStatus.Active + "", Text = "Xuất bản" };
            SelectListItem statusItem2 = new SelectListItem() { Value = (int)ProductStatus.UnActive + "", Text = "Chưa Xuất bản" };
            listStatus.Add(statusItem1);
            listStatus.Add(statusItem2);

            ViewBag.ListStatus = listStatus;
            return View(articleBlogtDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ArticleBlogDto articleBlogDto)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;

                #region Xử lý lưu ảnh vào wwwroot
                if (articleBlogDto.ImageIds == null)
                    articleBlogDto.ImageIds = new List<ImageDto>();
                foreach (ImageDto item in articleBlogDto.ImageIds)
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
                            // Xóa ảnh hiện tại
                            ImageEntity imageEntity = await _imageService.GetImageAsync(item.ImageId);
                            string currentImagePath = Path.Combine(wwwRootPath + "/Resource/AllImages/", imageEntity.ImageName);
                            currentImagePath = currentImagePath.Replace(@"/", @"\");
                            if (System.IO.File.Exists(currentImagePath))
                            {
                                System.IO.File.Delete(currentImagePath);
                            }

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
                            imageEntity.ArticleBlogEntityId = articleBlogDto.Id;
                            imageEntity.ProductEntityId = null;
                            imageEntity.CustomerEntityId = null;
                            await _imageService.AddImageAsync(imageEntity);
                        }
                    }
                }
                #endregion

                // Update to db Product
                articleBlogDto.UpdatedDate = DateTime.Now;
                ArticleBlogEntity articleBlogEntity = _mapper.Map<ArticleBlogEntity>(articleBlogDto);
                await _articleService.UpdateArticleAsync(articleBlogEntity);
            }
            return RedirectToAction("ArticleBlogList");
        }
        #endregion

        public async Task<ActionResult> Delete(int articleBlogId)
        {
            ArticleBlogEntity articleBlogEntity = await _articleService.GetArticleAsync(articleBlogId);
            if (articleBlogEntity != null)
            {
                IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();
                articleBlogEntity.ImageIds = lstImages.Where(x => x.ArticleBlogEntityId == articleBlogEntity.Id).ToList();
                if (articleBlogEntity.ImageIds != null)
                {
                    foreach (ImageEntity imgItem in articleBlogEntity.ImageIds)
                    {
                        if (!string.IsNullOrEmpty(imgItem.ImageName))
                        {
                            // Xóa ảnh khỏi AllImages và ArticleBlogList
                            string currentImagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Resource/AllImages/", imgItem.ImageName);
                            //string currentImagePathInProductList = Path.Combine(_hostEnvironment.WebRootPath + "/Resource/ProductList/", imgItem.ImageName);
                            currentImagePath = currentImagePath.Replace(@"/", @"\");
                            //currentImagePathInProductList = currentImagePathInProductList.Replace(@"/", @"\");
                            if (System.IO.File.Exists(currentImagePath))
                            {
                                System.IO.File.Delete(currentImagePath);
                            }
                        }
                    }
                }
            }
            await _articleService.RemoveAsync(articleBlogEntity);
            return RedirectToAction("ArticleBlogList");
        }

        public async Task<ActionResult> ChangeStatus(int articleBlogId)
        {
            ArticleBlogEntity articleBlogEntity = await _articleService.GetArticleAsync(articleBlogId);
            if (articleBlogEntity.Status == (int)ArticleBlogStatus.Active)
            {
                articleBlogEntity.Status = (int)ArticleBlogStatus.UnActive;
            }
            else
            {
                articleBlogEntity.Status = (int)ProductStatus.Active;
            }

            await _articleService.UpdateArticleAsync(articleBlogEntity);
            return RedirectToAction("ArticleBlogList");
        }
    }
}