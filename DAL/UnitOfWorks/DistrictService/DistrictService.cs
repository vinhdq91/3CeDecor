using AutoMapper;
using DAL.Models.Application;
using DAL.Repositories.DistrictRepository;
using DAL.Repositories.ProvinceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.DistrictService
{
    public class DistrictService : IDistrictService
    {
        private readonly IDistrictRepository _districtRepos;
        private readonly IMapper _mapper;
        public DistrictService(IDistrictRepository districtRepos, IMapper mapper)
        {
            _districtRepos = districtRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<DistrictEntity>> GetListAllDistrictsAsync()
        {
            return await _districtRepos.GetAllAsync();
        }

        public async Task<IQueryable<DistrictEntity>> GetListAllDistrictsByProvinceAsync(string provinceCode)
        {
            IQueryable<DistrictEntity> districtEntities = await _districtRepos.GetAllAsync();
            return districtEntities.Where(x => x.ProvinceCode.Equals(provinceCode));
        }

        public async Task<DistrictEntity> GetDistrictAsync(int id)
        {
            return await _districtRepos.GetAsync(id);
        }

        public async Task<DistrictEntity> GetDistrictAsync(string districtCode)
        {
            return await _districtRepos.GetSingleOrDefaultAsync(x => x.DistrictCode == districtCode);
        }
    }
}
