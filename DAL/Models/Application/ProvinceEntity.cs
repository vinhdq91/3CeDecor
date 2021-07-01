using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Models.Application
{
    public class ProvinceEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        public string ProvinceCode { get; set; }

        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string ProvinceTypeName { get; set; }

        [ForeignKey("ProvinceCode")]
        public virtual List<DistrictEntity> DistrictEntities { get; set; }
    }
}
