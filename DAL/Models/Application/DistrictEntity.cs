using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class DistrictEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string DistrictCode { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string DistrictTypeName { get; set; }

        public string ProvinceCode { get; set; }
        public ProvinceEntity ProvinceEntity { get; set; }

        [ForeignKey("DistrictCode")]
        public virtual List<WardEntity> WardEntities { get; set; }
    }
}
