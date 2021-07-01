using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.UnitOfWorks.ArticleBlogService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public class BuildLinkArticleBlog : IBuildLinkArticleBlog
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _domainName;
        private readonly string _pathUrl;
        private readonly IArticleBlogService _articleBlogService;
        public BuildLinkArticleBlog(
            IHttpContextAccessor contextAccessor,
            IArticleBlogService articleBlogService
        )
        {
            _contextAccessor = contextAccessor;
            _domainName = _contextAccessor.HttpContext.Request.Host.Value;
            _pathUrl = _contextAccessor.HttpContext.Request.Path;
            _articleBlogService = articleBlogService;
        }
        public string BuildLinkDetail(string title, int id)
        {
            string titleRemoveUnicode = StringHandle.RemoveUnicode(title);
            titleRemoveUnicode = Regex.Replace(titleRemoveUnicode, @"[^0-9a-zA-Z\s]+", "");
            if (!string.IsNullOrEmpty(titleRemoveUnicode))
            {
                titleRemoveUnicode = titleRemoveUnicode.ToLower().Replace(" ", "-");
            }
            string url = string.Format("https://{0}/article-blog/{1}/aid-{2}", _domainName, titleRemoveUnicode, id);
            return url;
        }

        public string BuildLinkArticleBlogSearch(ArticleBlogSearch objSearch)
        {
            string pathDomain = "https://" + _domainName;
            string url = string.Format("{0}/article-search/{1}", pathDomain, objSearch.TextSearch);
            return url;
        }
        public string GetCurrentURL(int? pageIndex)
        {
            string pathBaseUrl = "https://" + _domainName + _pathUrl;
            string url = "";
            if (pageIndex > 1)
                url = pathBaseUrl.Replace("/p" + pageIndex, "");
            else
                url = pathBaseUrl;
            return url;
        }
    }
}
