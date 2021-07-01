using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProvinceService
{
    public interface IProvinceService
    {
        Task<IQueryable<ProvinceEntity>> GetListAllProvincesAsync();
        Task<ProvinceEntity> GetProvinceAsync(int id);
        Task<ProvinceEntity> GetProvinceAsync(string provinceCode);
    }
}
