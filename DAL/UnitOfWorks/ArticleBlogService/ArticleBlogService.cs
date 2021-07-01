using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.Repositories.ArticleBlogRepository;
using DAL.Repositories.ProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ArticleBlogService
{
    public class ArticleBlogService : IArticleBlogService
    {
        private readonly IArticleBlogRepository _articleRepos;
        private readonly IMapper _mapper;
        public ArticleBlogService(IArticleBlogRepository articleRepos, IMapper mapper)
        {
            _articleRepos = articleRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<ArticleBlogEntity>> GetListAllArticlesAsync()
        {
            IQueryable<ArticleBlogEntity> articleEntities = await _articleRepos.GetAllAsync();
            return articleEntities;
        }

        public async Task AddArticleAsync(ArticleBlogEntity articleBlogEntity)
        {
            await _articleRepos.AddAsync(articleBlogEntity);
        }

        public async Task<ArticleBlogEntity> GetArticleAsync(int id)
        {
            ArticleBlogEntity articleEntity = await _articleRepos.GetSingleOrDefaultAsync(x => x.Id == id);
            return articleEntity;
        }
        public async Task UpdateArticleAsync(ArticleBlogEntity articleEntity)
        {
            await _articleRepos.UpdateAsync(articleEntity);
        }

        public async Task RemoveAsync(ArticleBlogEntity articleEntity)
        {
            await _articleRepos.RemoveAsync(articleEntity);
        }

        public List<ArticleBlogDto> GetOrderArticles(List<ArticleBlogDto> articleDtos, int sort)
        {
            switch (sort)
            {
                case (int)SortProductEnum.PublishDateASC:
                    return articleDtos.OrderBy(x => x.CreatedDate).ToList();
                case (int)SortProductEnum.PublishDateDESC:
                    return articleDtos.OrderByDescending(x => x.CreatedDate).ToList();
                default:
                    return articleDtos.OrderBy(x => x.CreatedDate).ToList();
            }
        }
    }
}
