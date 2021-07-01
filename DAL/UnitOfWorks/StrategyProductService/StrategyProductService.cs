using AutoMapper;
using DAL.Models.Application;
using DAL.Repositories.StrategyProductRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.StrategyProductService
{
    public class StrategyProductService : IStrategyProductService
    {
        private readonly IStrategyProductRepository _straProRepos;
        private readonly IMapper _mapper;

        public StrategyProductService(
            IStrategyProductRepository straProRepos,
            IMapper mapper
        )
        {
            _straProRepos = straProRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<StrategyProduct>> GetListAllStrategyProductAsync()
        {
            IQueryable<StrategyProduct> strategyProducts = await _straProRepos.GetAllAsync();
            return strategyProducts;
        }

        public async Task<IQueryable<StrategyProduct>> GetListProductIdByStrategyIdAsync(int strategyId)
        {
            IQueryable<StrategyProduct> strategyProducts = await _straProRepos.GetAllAsync();
            strategyProducts = strategyProducts.Where(x => x.StrategyId == strategyId);
            return strategyProducts;
        }

        public async Task<IQueryable<StrategyProduct>> GetListStrategyIdByProductIdAsync(int productId)
        {
            IQueryable<StrategyProduct> strategyProducts = await _straProRepos.GetAllAsync();
            strategyProducts = strategyProducts.Where(x => x.ProductId == productId);
            return strategyProducts;
        }

        public async Task AddStrategyProductAsync(StrategyProduct strategyProduct)
        {
            await _straProRepos.AddAsync(strategyProduct);
        }
        public async Task RemoveStrategyProductAsync(StrategyProduct strategyProduct)
        {
            await _straProRepos.RemoveAsync(strategyProduct);
        }
    }
}
