using AutoMapper;
using DAL.Models.Application;
using DAL.Repositories.ProvinceRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ProvinceService
{
    public class ProvinceService : IProvinceService
    {
        private readonly IProvinceRepository _provinceRepos;
        private readonly IMapper _mapper;
        public ProvinceService(IProvinceRepository provinceRepos, IMapper mapper)
        {
            _provinceRepos = provinceRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<ProvinceEntity>> GetListAllProvincesAsync()
        {
            return await _provinceRepos.GetAllAsync();
        }

        public async Task<ProvinceEntity> GetProvinceAsync(int id)
        {
            return await _provinceRepos.GetAsync(id);
        }
        public async Task<ProvinceEntity> GetProvinceAsync(string provinceCode)
        {
            return await _provinceRepos.GetSingleOrDefaultAsync(x => x.ProvinceCode == provinceCode);
        }
    }
}
