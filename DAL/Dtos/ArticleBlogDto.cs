using DAL.Core.Model;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class ArticleBlogDto : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Sapo { get; set; }
        public string Content { get; set; }
        public string Authors { get; set; }
        public string UrlPath { get; set; }
        public string SourceUrl { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public int Status { get; set; }

        public virtual List<ImageDto> ImageIds { get; set; }
    }

    public class ArticleBlogListModel
    {
        public string TitleList { get; set; }
        public List<ArticleBlogDto> ListArticles { get; set; }
        public PagingInfo Paging { get; set; }
        public string MenuActive { get; set; }
        public string TextSEO { get; set; }
        public string LinkSearchSuggest { get; set; }

        public ArticleBlogListModel()
        {
            ListArticles = new List<ArticleBlogDto>();
        }
    }

    public class ArticleBlogSearch
    {
        public int CategoryId { get; set; }
        public int TagId { get; set; }
        public int TagType { get; set; }
        public string TextSearch { get; set; }
        public int Price { get; set; }
        public int OrderBy { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
