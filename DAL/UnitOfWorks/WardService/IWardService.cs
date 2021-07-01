using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.WardService
{
    public interface IWardService
    {
        Task<IQueryable<WardEntity>> GetListAllWardsAsync();
        Task<IQueryable<WardEntity>> GetListAllWardsByDistrictAsync(string districtCode);
        Task<WardEntity> GetWardAsync(int id);
        Task<WardEntity> GetWardAsync(string wardCode);
    }
}
