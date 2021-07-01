using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class ImageDto : AuditableEntity
    {
        public int ImageId { get; set; }
        public string ImageName { get; set; }
        public string Title { get; set; }

        public IFormFile ImageFile { get; set; }

        public int? ArticleBlogEntityId { get; set; }
        public int? CustomerEntityId { get; set; }
        public int? ProductEntityId { get; set; }
    }
}
