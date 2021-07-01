using DAL.Core.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;

namespace WebApplication.Controllers
{
    public class PagingController : Controller
    {
        public ActionResult Index(PagingInfo pagingInfo)
        {
            string strParameter = string.Empty;
            string strchar = "?";
            int pageNum = (int)Math.Ceiling((double)pagingInfo.Count / pagingInfo.PageSize);
            if (pageNum * pagingInfo.PageSize < pagingInfo.Count)
            {
                pageNum++;
            }
            //if (pagingInfo.Sort > (int)ArrangeId.All)
            //{
                pagingInfo.LinkSite = pagingInfo.LinkSite == null ? "" : pagingInfo.LinkSite.Replace("?sort=" + pagingInfo.Sort, "");
                strParameter = string.Concat(strchar, "sort=", pagingInfo.Sort);
                strchar = "&";
            //}

            // Check request has image
            if (pagingInfo.HasImage == 1)
                strParameter += string.Concat(strchar, "image=", pagingInfo.HasImage);

            pagingInfo.LinkSite = pagingInfo.LinkSite.TrimEnd('/') + "/";
            const string buildLink = "<li><a href='{0}{1}{2}' {3}>{4}</a> </li>";
            const string active = " class=\"active\" ";
            const string first_page = "class='first-page'";
            const string prev_page = "class='prev-page'";
            const string next_page = "class='next-page'";
            const string last_page = "class='last-page'";
            const string strTrang = "p";
            string currentPage = pagingInfo.PageIndex.ToString();
            StringBuilder htmlPage = new StringBuilder();
            int iCurrentPage = 0;
            if (pagingInfo.PageIndex <= 0) iCurrentPage = 1;
            else iCurrentPage = pagingInfo.PageIndex;
            if (pageNum <= 5)
            {
                if (pageNum != 1)
                {
                    for (int i = 1; i <= pageNum; i++)
                    {
                        if (i == 1)
                        {
                            htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite.TrimEnd('/'), string.Empty, strParameter, i == pagingInfo.PageIndex ? active : string.Empty, i);
                        }
                        else
                        {
                            htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + i, strParameter, i == pagingInfo.PageIndex ? active : string.Empty, i);
                        }
                    }
                }
            }
            else
            {
                if (iCurrentPage > 1)
                {
                    htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite.TrimEnd('/'), string.Empty, strParameter, first_page, "<i class='fa fa-angle-double-left'></i>");
                }
                else
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        if (i == 1)
                        {
                            htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite.TrimEnd('/'), "", strParameter, i == pagingInfo.PageIndex ? active : string.Empty, i);
                        }
                        else
                        {
                            htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + i, strParameter, i == pagingInfo.PageIndex ? active : string.Empty, i);
                        }
                    }
                    htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + 2, strParameter, next_page, "<i class='fa fa-angle-right'></i>");
                }
                if (iCurrentPage > 1 && iCurrentPage < pageNum)
                {
                    if (iCurrentPage > 2)
                    {
                        if (iCurrentPage - 1 == 1)
                        {
                            htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite.TrimEnd('/'), string.Empty, strParameter, prev_page, "<i class='fa fa-angle-left'></i>");
                        }
                        else
                        {
                            htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + (iCurrentPage - 1), strParameter, prev_page, "<i class='fa fa-angle-left'></i>");
                        }
                    }
                    for (int i = (iCurrentPage - 2); i <= (iCurrentPage + 2 < pageNum ? iCurrentPage + 2 : pageNum); i++)
                    {
                        if (i > 0)
                        {
                            if (i == 1)
                            {
                                htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite.TrimEnd('/'), "", strParameter, i == pagingInfo.PageIndex ? active : string.Empty, i);
                            }
                            else
                            {
                                htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + i, strParameter, i == pagingInfo.PageIndex ? active : string.Empty, i);
                            }
                        }
                    }
                    if (iCurrentPage <= pageNum - 2)
                    {
                        htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + (iCurrentPage + 1), strParameter, next_page, "<i class='fa fa-angle-right'></i>");
                    }
                }
                else if (iCurrentPage > 5 && iCurrentPage == pageNum)
                {
                    htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + (iCurrentPage - 1), strParameter, prev_page, "<i class='fa fa-angle-left'></i>");
                }

                int intCurrentPage = 0;
                int.TryParse(currentPage, out intCurrentPage);
                if (intCurrentPage == 0) intCurrentPage = 1;
                if (intCurrentPage < pageNum)
                {
                    htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + pageNum, strParameter, last_page, "<i class='fa fa-angle-double-right'></i>");
                }
                else
                {
                    if (pageNum - 4 == 1)
                    {
                        htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite.TrimEnd('/'), string.Empty, strParameter, prev_page, "<i class='fa fa-angle-left'></i>");
                    }
                    int j = 4;
                    for (int i = pageNum; i >= pageNum - 4; i--)
                    {
                        htmlPage.AppendFormat(buildLink, pagingInfo.LinkSite, strTrang + (pageNum - j), strParameter, j == 0 ? active : string.Empty, pageNum - j);
                        j--;
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<ul class='paging'>{0}</ul>", htmlPage);
            return PartialView("_Paging", sb.ToString());
        }
    }
}