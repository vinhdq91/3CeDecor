using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ArticleBlogService
{
    public interface IArticleBlogService
    {
        Task<IQueryable<ArticleBlogEntity>> GetListAllArticlesAsync();

        Task<ArticleBlogEntity> GetArticleAsync(int id);
        Task AddArticleAsync(ArticleBlogEntity articleBlogEntity);
        Task UpdateArticleAsync(ArticleBlogEntity articleEntity);
        Task RemoveAsync(ArticleBlogEntity articleEntity);
        List<ArticleBlogDto> GetOrderArticles(List<ArticleBlogDto> articleDtos, int sort);
    }
}
