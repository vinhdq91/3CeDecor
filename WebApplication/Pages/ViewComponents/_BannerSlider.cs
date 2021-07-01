using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.GalleryService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_BannerSlider")]
    public class _BannerSlider : ViewComponent
    {
        private readonly IGalleryService _galleryService;
        private readonly IMapper _mapper;
        public _BannerSlider(
            IGalleryService galleryService,
            IMapper mapper
        )
        {
            _galleryService = galleryService;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<GalleryEntity> galleryEntities = await _galleryService.GetListAllGalleryAsync(false);
            List<GalleryDto> listGalleryDtos = new List<GalleryDto>();
            if (galleryEntities != null)
            {
                galleryEntities = galleryEntities.Where(x => x.GalleryType == (int)GalleryType.HomeSlide || x.GalleryType == (int)GalleryType.HomeVideo);
                foreach (GalleryEntity item in galleryEntities)
                {
                    GalleryDto galleryDto = _mapper.Map<GalleryDto>(item);
                    if (galleryDto != null)
                    {
                        if (!string.IsNullOrEmpty(galleryDto.ImageName))
                        {
                            galleryDto.ImagePath = Path.Combine("/Resource/Gallery/", galleryDto.ImageName);
                        }
                        listGalleryDtos.Add(galleryDto);
                    }
                }
            }
            return View(listGalleryDtos);
        }
    }
}
