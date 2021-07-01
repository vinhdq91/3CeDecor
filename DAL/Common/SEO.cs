using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Common
{
    public static class SEO
    {
        public static string AddTitle(string title)
        {
            //return string.Format("{0} | {1}", title, Const.Webconfig.SiteName);
            return title;
        }
        public static string AddMeta(string metaName, string content)
        {
            const string metaFormat = "<meta name=\"{0}\" content=\"{1}\" />";
            return string.IsNullOrEmpty(content) ? string.Empty : string.Format(metaFormat, metaName, content);
        }
        //public static string AddMetaFacebook(string title, string type, string description, string url, string image, string domainAndCropSize = "", int imgWidth = 620, int imgHeight = 324)
        //{
        //    description = description.Trim() == string.Empty ? " " : PlainText(description);
        //    title = title.Trim() == string.Empty ? " " : PlainText(title);
        //    type = type.Trim() == string.Empty ? " " : type;
        //    url = url.Trim() == string.Empty ? System.Web.HttpContext.Current.Request.RawUrl : url;
        //    StringBuilder metaFormat = new StringBuilder();
        //    metaFormat.AppendFormat("<meta property=\"og:site_name\" content=\"{0}\" />", Const.Webconfig.SiteName);
        //    metaFormat.AppendFormat("<meta property=\"og:title\" content=\"{0}\" />", title);
        //    metaFormat.AppendFormat("<meta property=\"og:type\" content=\"{0}\" />", type);
        //    metaFormat.AppendFormat("<meta property=\"og:description\" content=\"{0}\" />", description);
        //    metaFormat.AppendFormat("<meta property=\"og:url\" content=\"{0}\" />", url);
        //    if (!string.IsNullOrEmpty(image))
        //    {
        //        metaFormat.AppendFormat("<meta property=\"og:image\" content=\"{0}\" />", domainAndCropSize + image);
        //        metaFormat.Append("<meta property=\"og:image:type\" content=\"image/jpg\" />");
        //        metaFormat.AppendFormat("<meta property=\"og:image:width\" content=\"{0}\" />", imgWidth);
        //        metaFormat.AppendFormat("<meta property=\"og:image:height\" content=\"{0}\" />", imgHeight);
        //    }
        //    return metaFormat.ToString();
        //}

        public static string MetaCanonical(string link)
        {
            if (String.IsNullOrEmpty(link)) return string.Empty;
            return string.Format("<link rel=\"canonical\" href=\"{0}\" />", link);
        }
        public static string MetaAlternate(string link)
        {
            if (String.IsNullOrEmpty(link)) return string.Empty;
            return string.Format("<link rel=\"alternate\" href=\"{0}\" media=\"only screen and (max-width: 640px)\" /><link rel=\"alternate\" href=\"{0}\" media=\"handheld\" /><link rel=\"alternate\" hreflang=\"en-ph\" href=\"{0}\"/>", link);
        }

        public static string RemoveHTMLTag(string htmlString)
        {
            string pattern = @"(<[^>]+>)";
            string text = System.Text.RegularExpressions.Regex.Replace(htmlString, pattern, string.Empty);
            return text;
        }
        public static string PreProcessSearchString(string searchString)
        {
            string output = searchString.Replace("'", " ").Replace("\"\"", " ").Replace(">", " ").Replace("<", " ").Replace(",", " ").Replace("(", " ").Replace(")", " ").Replace("\"", " ");
            output = System.Text.RegularExpressions.Regex.Replace(output, "[ ]+", "+");
            return output.Trim();
        }

        public static string PlainText(string input)
        {
            if (!string.IsNullOrEmpty(input))
                return RemoveHTMLTag(input).Replace("\"", string.Empty).Replace("'", string.Empty).Trim();
            return string.Empty;
        }

        //public static string MetaPagination(int totalCount, int pageIndex, int pageSize, string link)
        //{
        //    string strMeta = string.Empty;
        //    int pageNum = (int)Math.Ceiling((double)totalCount / pageSize);
        //    if (pageIndex > 1)
        //        strMeta = string.Format("<link rel=\"prev\" href=\"{0}{1}/p{2}\" />", Webconfig.BaseUrlNoSlash, link, (pageIndex - 1));
        //    if (pageIndex < pageNum)
        //        strMeta = string.Format("{0}<link rel=\"next\" href=\"{1}{2}/p{3}\" />", strMeta, Webconfig.BaseUrlNoSlash, link, (pageIndex + 1));
        //    return strMeta;
        //} 
    }

    public class FacebookPixel
    {
        public const string AutosList = "<script>fbq('track', 'ViewContent', {content_type: 'tin_rao',});</script>";
        public const string AutosDetail = "<script>fbq('track', 'ViewContent', {content_type: 'tin_rao > chi_tiet',});</script>";
        public const string AutosPost = "<script>fbq('track', 'ViewContent', {content_type: 'dang_tin',});</script>";
        public const string Article = "<script>fbq('track', 'ViewContent', {content_type: 'tin_bai',});</script>";
    }
}
