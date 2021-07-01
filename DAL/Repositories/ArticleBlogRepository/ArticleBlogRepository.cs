using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ArticleBlogRepository
{
    public class ArticleBlogRepository : Repository<ArticleBlogEntity>, IArticleBlogRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ArticleBlogRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
