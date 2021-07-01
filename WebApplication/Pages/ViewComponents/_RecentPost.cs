using AutoMapper;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ArticleBlogService;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BuildLink;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_RecentPost")]
    public class _RecentPost : ViewComponent
    {
        private readonly IArticleBlogService _articleService;
        private readonly IBuildLinkArticleBlog _buildLinkArticle;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;
        public _RecentPost(
            IArticleBlogService articleService,
            IBuildLinkArticleBlog buildLinkArticle,
            IImageService imageService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _articleService = articleService;
            _buildLinkArticle = buildLinkArticle;
            _imageService = imageService;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync()
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
            return View(articleDtos);
        }
    }
}
