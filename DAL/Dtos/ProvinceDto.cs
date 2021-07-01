using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class ProvinceDto
    {
        public string ProvinceCode { get; set; }
        public string Name { get; set; }
        public string ProviceTypeName { get; set; }

        public virtual List<DistrictDto> DistrictDtos { get; set; }
    }
}
