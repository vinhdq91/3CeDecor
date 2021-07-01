using AutoMapper;
using DAL.Core.Enums;
using DAL.Models.Application;
using DAL.Repositories.StrategyRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.StrategyService
{
    public class StrategyService : IStrategyService
    {
        private readonly IStrategyRepository _strategyRepos;
        private readonly IMapper _mapper;
        public StrategyService(IStrategyRepository strategyRepos, IMapper mapper)
        {
            _strategyRepos = strategyRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<StrategyEntity>> GetListAllStrategysAsync(bool isAdmin = false)
        {
            IQueryable<StrategyEntity> strategyEntities = await _strategyRepos.GetAllAsync();
            if (strategyEntities != null && !isAdmin)
            {
                strategyEntities = strategyEntities.Where(x => x.Status == (int)StrategyStatusEnum.Active);
            }
            return strategyEntities;
        }

        public async Task<StrategyEntity> GetStrategyAsync(int id)
        {
            StrategyEntity strategyEntity = await _strategyRepos.GetSingleOrDefaultAsync(x => x.Id == id);
            return strategyEntity;
        }

        public async Task AddStrategyAsync(StrategyEntity strategyEntity)
        {
            await _strategyRepos.AddAsync(strategyEntity);
        }

        public async Task RemoveAsync(StrategyEntity strategyEntity)
        {
            await _strategyRepos.RemoveAsync(strategyEntity);
        }

        public async Task UpdateStrategyAsync(StrategyEntity strategyEntity)
        {
            await _strategyRepos.UpdateAsync(strategyEntity);
        }
    }
}
