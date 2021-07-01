using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class StrategyEntity : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        [DisplayName("Image Name")]
        [StringLength(50)]
        public string Name { get; set; }
        public DateTime? ExpireDate { get; set; }

        [StringLength(510)]
        public string Description { get; set; }

        [StringLength(60)]
        public string MetaTitle { get; set; }

        [StringLength(510)]
        public string MetaDescription { get; set; }
        public int Status { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Image Name")]
        [StringLength(110)]
        public string ImageName { get; set; }

        [NotMapped]
        [DisplayName("Upload File")]
        public IFormFile ImageFile { get; set; }

        public virtual List<StrategyProduct> StrategyProducts { get; set; }
    }
}
