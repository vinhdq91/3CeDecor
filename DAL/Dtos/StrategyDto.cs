using DAL.Models;
using DAL.Models.Application;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class StrategyDto : AuditableEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? ExpireDate { get; set; }
        public string Description { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public int Status { get; set; }
        public string ImageName { get; set; }
        public string ImageNameCurrent { get; set; }  // Lưu Image Name hiện tại để không phải track entity
        public IFormFile ImageFile { get; set; }
        public string UrlPath { get; set; }
        public string ImagePath { get; set; }

        public virtual List<StrategyProduct> StrategyProducts { get; set; }
    }
}
