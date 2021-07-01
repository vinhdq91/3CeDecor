using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Dtos
{
    public class WardDto
    {
        public string WardCode { get; set; }
        public string Name { get; set; }
        public string WardTypeName { get; set; }

        public string DistrictCode { get; set; }
        public string ProvinceCode { get; set; }
        public DistrictDto DistrictDto { get; set; }
    }
}
