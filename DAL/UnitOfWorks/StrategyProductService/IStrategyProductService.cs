using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.StrategyProductService
{
    public interface IStrategyProductService
    {
        Task<IQueryable<StrategyProduct>> GetListAllStrategyProductAsync();
        Task<IQueryable<StrategyProduct>> GetListProductIdByStrategyIdAsync(int strategyId);
        Task<IQueryable<StrategyProduct>> GetListStrategyIdByProductIdAsync(int productId);
        Task AddStrategyProductAsync(StrategyProduct strategyProduct);
        Task RemoveStrategyProductAsync(StrategyProduct strategyProduct);
    }
}
