using DAL.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication.BuildLink
{
    public class BuildLinkSeo : IBuildLinkSeo
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        private readonly string _domainName;
        private readonly string _pathUrl;

        public BuildLinkSeo(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _domainName = _contextAccessor.HttpContext.Request.Host.Value;
            _pathUrl = _contextAccessor.HttpContext.Request.Path;
            _configuration = configuration;
        }
        public string AddMetaFacebook(string title, string description, string url, string image, string domainAndCropSize = "", int imgWidth = 620, int imgHeight = 324)
        {
            description = string.IsNullOrEmpty(description) ? " " : SEO.PlainText(description.Trim());
            title = string.IsNullOrEmpty(title) ? " " : SEO.PlainText(title);
            url = string.IsNullOrEmpty(url) ? _pathUrl : url;
            StringBuilder metaFormat = new StringBuilder();
            metaFormat.AppendFormat("<meta property=\"og:site_name\" content=\"{0}\" />", _configuration.GetSection("WebDomain").Value);
            metaFormat.AppendFormat("<meta property=\"og:title\" content=\"{0}\" />", title);
            metaFormat.AppendFormat("<meta property=\"og:description\" content=\"{0}\" />", description);
            metaFormat.AppendFormat("<meta property=\"og:url\" content=\"{0}\" />", url);
            if (!string.IsNullOrEmpty(image))
            {
                metaFormat.AppendFormat("<meta property=\"og:image\" content=\"{0}\" />", domainAndCropSize + image);
                metaFormat.Append("<meta property=\"og:image:type\" content=\"image/jpg\" />");
                metaFormat.AppendFormat("<meta property=\"og:image:width\" content=\"{0}\" />", imgWidth);
                metaFormat.AppendFormat("<meta property=\"og:image:height\" content=\"{0}\" />", imgHeight);
            }
            return metaFormat.ToString();
        }
    }
}
