using DAL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public interface IBuildLinkArticleBlog
    {
        public string BuildLinkDetail(string title, int id);
        public string GetCurrentURL(int? pageIndex);
        public string BuildLinkArticleBlogSearch(ArticleBlogSearch objSearch);
    }
}
