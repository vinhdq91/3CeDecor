using AutoMapper;
using DAL.Core.Enums;
using DAL.Models.Application;
using DAL.Repositories.DiscountCodeRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.DiscountCodeService
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly IDiscountCodeRepository _discountCodeRepos;
        private readonly IMapper _mapper;
        public DiscountCodeService(IDiscountCodeRepository discountCodeRepos, IMapper mapper)
        {
            _discountCodeRepos = discountCodeRepos;
            _mapper = mapper;
        }

        public async Task<DiscountCodeEntity> CheckDiscountCodeAsync(string discountCode)
        {
            IQueryable<DiscountCodeEntity> discountCodeEntities = await _discountCodeRepos.GetAllAsync();
            IQueryable<DiscountCodeEntity> discountCodeOutput;
            DiscountCodeEntity discountCodeEntity = null;
            if (discountCodeEntities != null)
            {
                discountCodeOutput = discountCodeEntities.Where(x => x.DiscountCodeName == discountCode && x.Status == (int)DiscountCodeStatus.Active);
                bool isEnable = discountCodeOutput != null
                                && discountCodeOutput.Count() == 1
                                && DateTime.Compare(discountCodeOutput.ToList()[0].ExpireDate, DateTime.Now) > 0;
                if (isEnable)
                    discountCodeEntity = discountCodeOutput.ToList()[0];
            }
            return discountCodeEntity;
        }

        public async Task<IQueryable<DiscountCodeEntity>> GetListAllDiscountCodeAsync()
        {
            IQueryable<DiscountCodeEntity> DiscountCodeEntities = await _discountCodeRepos.GetAllAsync();
            return DiscountCodeEntities;
        }

        public async Task<DiscountCodeEntity> GetDiscountCodeAsync(int discountCodeId)
        {
            return await _discountCodeRepos.GetSingleOrDefaultAsync(x => x.Id == discountCodeId);
        }

        public async Task AddDiscountCodeAsync(DiscountCodeEntity discountCodeEntity)
        {
            await _discountCodeRepos.AddAsync(discountCodeEntity);
        }

        public async Task UpdateDiscountCodeAsync(DiscountCodeEntity discountCodeEntity)
        {
            await _discountCodeRepos.UpdateAsync(discountCodeEntity);
        }

        public async Task DeleteDiscountCodeAsync(DiscountCodeEntity discountCodeEntity)
        {
            await _discountCodeRepos.RemoveAsync(discountCodeEntity);
        }
    }
}
