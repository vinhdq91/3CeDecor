using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class GalleryDto
    {
        public int Id { get; set; }
        public string ImageName { get; set; }
        public string ImageNameCurrent { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int GalleryType { get; set; }
        public string UrlForward { get; set; }
        public string LinkVideoEmbed { get; set; }
        public int Status { get; set; }

        public string SlideName { get; set; }
        public string SlideTitle { get; set; }
        public string SlideContent { get; set; }
        public string ColorCode { get; set; }

        public string ImagePath { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}
