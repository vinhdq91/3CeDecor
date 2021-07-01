using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.DistrictService
{
    public interface IDistrictService
    {
        Task<IQueryable<DistrictEntity>> GetListAllDistrictsAsync();
        Task<IQueryable<DistrictEntity>> GetListAllDistrictsByProvinceAsync(string provinceCode);
        Task<DistrictEntity> GetDistrictAsync(int id);
        Task<DistrictEntity> GetDistrictAsync(string districtCode);
    }
}
