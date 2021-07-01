using AutoMapper;
using DAL.Models.Application;
using DAL.Repositories.WardRepository;
using DAL.Repositories.ProvinceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.WardService
{
    public class WardService : IWardService
    {
        private readonly IWardRepository _wardRepos;
        private readonly IMapper _mapper;
        public WardService(IWardRepository wardRepos, IMapper mapper)
        {
            _wardRepos = wardRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<WardEntity>> GetListAllWardsAsync()
        {
            return await _wardRepos.GetAllAsync();
        }

        public async Task<IQueryable<WardEntity>> GetListAllWardsByDistrictAsync(string districtCode)
        {
            IQueryable<WardEntity> WardEntities = await _wardRepos.GetAllAsync();
            return WardEntities.Where(x => x.DistrictCode.Equals(districtCode));
        }

        public async Task<WardEntity> GetWardAsync(int id)
        {
            return await _wardRepos.GetAsync(id);
        }

        public async Task<WardEntity> GetWardAsync(string wardCode)
        {
            return await _wardRepos.GetSingleOrDefaultAsync(x => x.WardCode == wardCode);
        }
    }
}
