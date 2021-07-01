using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class ArticleBlogEntity : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(260)]
        public string Title { get; set; }

        [StringLength(260)]
        public string Sapo { get; set; }

        public string Content { get; set; }

        [StringLength(30)]
        public string Authors { get; set; }

        [StringLength(110)]
        public string UrlPath { get; set; }

        [StringLength(210)]
        public string SourceUrl { get; set; }

        [StringLength(260)]
        public string MetaTitle { get; set; }

        [StringLength(510)]
        public string MetaDescription { get; set; }
        public int Status { get; set; }

        public virtual List<ImageEntity> ImageIds { get; set; }
    }
}
