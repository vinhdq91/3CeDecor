using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class ImageEntity : AuditableEntity
    {
        [Key]
        public int ImageId { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        public string ImageName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }

        public int? ArticleBlogEntityId { get; set; }
        public int? CustomerEntityId { get; set; }
        public int? ProductEntityId { get; set; }
    }
}
