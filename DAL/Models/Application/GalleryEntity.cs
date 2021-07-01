using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class GalleryEntity
    {
        public int Id { get; set; }

        [StringLength(60)]
        public string ImageName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập chiều dài")]
        public int Width { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập chiều cao")]
        public int Height { get; set; }

        public int GalleryType { get; set; }

        [StringLength(110)]
        public string UrlForward { get; set; }

        [StringLength(110)]
        public string LinkVideoEmbed { get; set; }
        public int Status { get; set; }

        [StringLength(110)]
        public string SlideName { get; set; }

        [StringLength(110)]
        public string SlideTitle { get; set; }

        [StringLength(110)]
        public string SlideContent { get; set; }

        [StringLength(20)]
        public string ColorCode { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
