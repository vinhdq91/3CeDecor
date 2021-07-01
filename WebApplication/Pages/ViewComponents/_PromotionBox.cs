using AutoMapper;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ProductService;
using DAL.UnitOfWorks.StrategyService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.BuildLink;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_PromotionBox")]
    public class _PromotionBox : ViewComponent
    {
        private readonly IStrategyService _strategyService;
        private readonly IBuildLinkStrategy _buildLinkStrategy;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public _PromotionBox(
            IStrategyService strategyService,
            IBuildLinkStrategy buildLinkStrategy,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper,
            IConfiguration configuration
        )
        {
            _strategyService = strategyService;
            _buildLinkStrategy = buildLinkStrategy;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<StrategyEntity> strategyEntities = await _strategyService.GetListAllStrategysAsync(false);
            List<StrategyDto> listStrategyDtos = new List<StrategyDto>();
            if (strategyEntities != null)
            {
                foreach (StrategyEntity item in strategyEntities)
                {
                    StrategyDto strategyDto = _mapper.Map<StrategyDto>(item);
                    if(strategyDto != null)
                    {
                        strategyDto.UrlPath = _buildLinkStrategy.BuildLinkDetail(strategyDto.Name, strategyDto.Id);
                        strategyDto.ImagePath = _buildLinkStrategy.BuildLinkImage(strategyDto.ImageName);
                        listStrategyDtos.Add(strategyDto);
                    }
                }
            }

            listStrategyDtos = listStrategyDtos.OrderByDescending(x => x.CreatedDate).Take(3).ToList();

            return View(listStrategyDtos);
        }
    }
}
