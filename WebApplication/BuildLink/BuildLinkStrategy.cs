using DAL.Core.Utilities;
using DAL.UnitOfWorks.StrategyService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public class BuildLinkStrategy : IBuildLinkStrategy
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _domainName;
        private readonly string _pathUrl;
        private readonly IStrategyService _strategyService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BuildLinkStrategy(
            IHttpContextAccessor contextAccessor,
            IStrategyService strategyService,
            IWebHostEnvironment hostEnvironment
        )
        {
            _contextAccessor = contextAccessor;
            _domainName = _contextAccessor.HttpContext.Request.Host.Value;
            _pathUrl = _contextAccessor.HttpContext.Request.Path;
            _strategyService = strategyService;
            _hostEnvironment = hostEnvironment;
        }

        public string BuildLinkDetail(string strategyName, int strategyId)
        {
            string titleRemoveUnicode = StringHandle.RemoveUnicode(strategyName);
            titleRemoveUnicode = Regex.Replace(titleRemoveUnicode, @"[^0-9a-zA-Z\s]+", "");
            if (!string.IsNullOrEmpty(titleRemoveUnicode))
            {
                titleRemoveUnicode = titleRemoveUnicode.ToLower().Replace(" ", "-");
            }
            string url = string.Format("https://{0}/strategy-{1}/sid-{2}", _domainName, titleRemoveUnicode, strategyId);
            return url;
        }

        public string BuildLinkImage(string imageName)
        {
            if (!string.IsNullOrEmpty(imageName))
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string imagePath = Path.Combine("Resource/StrategyAvatars/", imageName);
                return imagePath;
            }
            return string.Empty;
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
