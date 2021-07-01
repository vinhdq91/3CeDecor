using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.StrategyService
{
    public interface IStrategyService
    {
        Task<IQueryable<StrategyEntity>> GetListAllStrategysAsync(bool isAdmin = false);
        Task<StrategyEntity> GetStrategyAsync(int id);
        Task AddStrategyAsync(StrategyEntity strategyEntity);
        Task RemoveAsync(StrategyEntity strategyEntity);
        Task UpdateStrategyAsync(StrategyEntity strategyEntity);
    }
}
