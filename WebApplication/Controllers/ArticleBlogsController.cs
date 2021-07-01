using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using DAL.Core.Model;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ArticleBlogService;
using DAL.UnitOfWorks.ImageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication.BuildLink;
using DAL.Common;

namespace WebApplication.Controllers
{
    public class ArticleBlogsController : Controller
    {
        private readonly IArticleBlogService _articleService;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkArticleBlog _buildLinkArticle;
        private IConfiguration _configuration;
        public ArticleBlogsController(
            IArticleBlogService articleService,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IImageService imageService,
            IBuildLinkArticleBlog buildLinkArticle,
            IConfiguration configuration
            )
        {
            _articleService = articleService;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _imageService = imageService;
            _buildLinkArticle = buildLinkArticle;
            _configuration = configuration;
        }

        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<IEnumerable<ArticleBlogDto>>> ArticleBlogList(int pageIndex = 1)
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

            IQueryable<ArticleBlogEntity> listArticlesNoImg = listArticles.Where(x => !articleIds.Any(y => y == x.Id));
            foreach (var item in listArticlesNoImg.ToList())
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

            ArticleBlogListModel articleListModel = new ArticleBlogListModel();

            #region Pagination
            PagingInfo paging = new PagingInfo();
            paging.PageSize = Convert.ToInt32(_configuration.GetSection("ArticlePageSize").Value);
            paging.PageIndex = pageIndex;
            paging.LinkSite = _buildLinkArticle.GetCurrentURL(paging.PageIndex);
            paging.Count = articleDtos.Count;
            //paging.Sort = sort;
            articleListModel.Paging = paging;
            #endregion

            #region Meta tags
            ViewBag.MetaTitle = "Danh sách các bài viết trên 3CE Decor";
            ViewBag.MetaDescription = SEO.AddMeta("description", "Tìm hiểu thêm về nghệ thuật trang trí qua các bài viết của 3CE Decor sưu tầm và biên tập.");
            #endregion

            articleListModel.ListArticles = _articleService.GetOrderArticles(articleDtos, paging.Sort).Skip((paging.PageIndex - 1) * paging.PageSize)
                                                       .Take(paging.PageSize).ToList();
            ViewBag.TextSearch = "";
            return View(articleListModel);
        }

        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<IEnumerable<ArticleBlogDto>>> ArticleBlogSearch(string textSearch, int pageIndex = 1)
        {
            if (string.IsNullOrWhiteSpace(textSearch))
            {
                return RedirectToAction("ArticleBlogList");
            }
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

            IQueryable<ArticleBlogEntity> listArticlesNoImg = listArticles.Where(x => !articleIds.Any(y => y == x.Id));
            foreach (var item in listArticlesNoImg.ToList())
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

            ArticleBlogListModel articleListModel = new ArticleBlogListModel();

            textSearch = textSearch == null ? string.Empty : textSearch;

            #region Pagination
            PagingInfo paging = new PagingInfo();
            paging.PageSize = Convert.ToInt32(_configuration.GetSection("ArticlePageSize").Value);
            paging.PageIndex = pageIndex;
            paging.LinkSite = _buildLinkArticle.GetCurrentURL(paging.PageIndex);
            var allArticles = await _articleService.GetListAllArticlesAsync();
            paging.Count = allArticles.Where(x => x.Title.ToLower().Contains(textSearch.ToLower())).ToList().Count();
            //paging.Sort = sort;
            articleListModel.Paging = paging;
            #endregion

            #region Meta tags
            ViewBag.MetaTitle = !string.IsNullOrEmpty(textSearch) ? "Tìm kiếm bài viết theo từ khỏa: " + textSearch : "Danh sách bài viết trên 3CE Decor";
            ViewBag.MetaDescription = SEO.AddMeta("description", !string.IsNullOrEmpty(textSearch) ? "Tìm kiếm bài viết theo từ khỏa: " + textSearch : "Tìm hiểu thêm về nghệ thuật trang trí qua các bài viết của 3CE Decor sưu tầm và biên tập.");
            #endregion

            articleListModel.ListArticles = _articleService.GetOrderArticles(articleDtos, paging.Sort)
                                                        .Where(x => x.Title.ToLower().Contains(textSearch.ToLower()))
                                                        .Skip((paging.PageIndex - 1) * paging.PageSize)
                                                        .Take(paging.PageSize).ToList();
           
            ViewBag.TextSearch = textSearch;
            return View(articleListModel);
        }

        public ActionResult ArticleBlogBoxSearch(string textSearch = "")
        {
            ArticleBlogSearch objSearch = new ArticleBlogSearch
            {
                TextSearch = string.IsNullOrEmpty(textSearch) ? "" : textSearch.Replace("-", " ")
            };
            return PartialView("_ArticleBlogBoxSearch", objSearch);
        }

        [ResponseCache(VaryByHeader = "User-Agent", Duration = 86400)]
        public async Task<ActionResult<ArticleBlogDto>> ArticleBlogDetail(int id, string title = "")
        {
            ArticleBlogEntity article = await _articleService.GetArticleAsync(id);
            IQueryable<ImageEntity> lstImages = await _imageService.GetListAllImagesAsync();

            article.ImageIds = lstImages.Where(x => x.ArticleBlogEntityId == article.Id).ToList();
            article.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;

            ArticleBlogDto articleDto = _mapper.Map<ArticleBlogDto>(article);
            if (articleDto.ImageIds.Count == 0)
            {
                ImageDto imageDto = new ImageDto()
                {
                    ArticleBlogEntityId = articleDto.Id,
                    ImageName = _configuration.GetSection("NoImageFile").Value,
                    Title = "No Image"
                };
                articleDto.ImageIds.Add(imageDto);
            }

            #region Meta tags
            ViewBag.MetaTitle = article.MetaTitle;
            ViewBag.MetaDescription = article.MetaDescription;
            #endregion

            ViewBag.TextSearch = "";
            ViewBag.BlogDetailUrl = _buildLinkArticle.BuildLinkDetail(article.Title, article.Id);
            return View(articleDto);
        }

        public async Task<ActionResult<List<ArticleBlogDto>>> RecentPost()
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
            articleDtos = articleDtos.Count >= 5 ? articleDtos.Take(5).ToList() : articleDtos;            
            return View("_RecentPost", articleDtos);
        }
    }
}