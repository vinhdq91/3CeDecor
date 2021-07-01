using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class DistrictDto
    {
        public string DistrictCode { get; set; }
        public string Name { get; set; }
        public string DistrictTypeName { get; set; }

        public string ProvinceCode { get; set; }
        public ProvinceDto ProvinceDto { get; set; }
        public virtual List<WardDto> WardDtos { get; set; }
    }
}
