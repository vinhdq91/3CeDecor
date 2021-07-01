using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class WardEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string WardCode { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string WardTypeName { get; set; }

        public string DistrictCode { get; set; }

        [StringLength(60)]
        public string ProvinceCode { get; set; } 
        public DistrictEntity DistrictEntity { get; set; }
    }
}
