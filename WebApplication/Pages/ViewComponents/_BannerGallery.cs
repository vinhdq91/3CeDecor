using AutoMapper;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core.Enums;
using DAL.UnitOfWorks.GalleryService;
using System.IO;

namespace WebApplication.ViewComponents
{
    [ViewComponent(Name = "_BannerGallery")]
    public class _BannerGallery : ViewComponent
    {
        private readonly IGalleryService _galleryService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        public _BannerGallery(
            IGalleryService galleryService,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper
        )
        {
            _galleryService = galleryService;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            IQueryable<GalleryEntity> galleryEntities = await _galleryService.GetListAllGalleryAsync(false);
            List<GalleryDto> listGalleryDtos = new List<GalleryDto>();
            if (galleryEntities != null)
            {
                galleryEntities = galleryEntities.Where(x => x.GalleryType == (int)GalleryType.GalleryImage);
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

            List<GalleryDto> column1 = new List<GalleryDto>();
            List<GalleryDto> column2 = new List<GalleryDto>();
            List<GalleryDto> column3 = new List<GalleryDto>();

            column1 = listGalleryDtos.FindAll(x => x.Height == (int)GalleryHeightEnum.Height1).Take(1).ToList();
            column1.AddRange(listGalleryDtos.FindAll(x => x.Height == (int)GalleryHeightEnum.Height2).Take(1).ToList());
            column1 = column1.OrderBy(x => x.Height).ToList();

            listGalleryDtos = listGalleryDtos.Where(x => !column1.Any(y => y.Id == x.Id)).ToList();
            column2 = listGalleryDtos.FindAll(x => x.Height == (int)GalleryHeightEnum.Height1).Take(1).ToList();
            column2.AddRange(listGalleryDtos.FindAll(x => x.Height == (int)GalleryHeightEnum.Height2).Take(1).ToList());
            column2 = column2.OrderByDescending(x => x.Height).ToList();
            
            listGalleryDtos = listGalleryDtos.Where(x => !column2.Any(y => y.Id == x.Id)).ToList();
            column3 = listGalleryDtos.FindAll(x => x.Height == (int)GalleryHeightEnum.Height1).Take(1).ToList();
            column3.AddRange(listGalleryDtos.FindAll(x => x.Height == (int)GalleryHeightEnum.Height2).Take(1).ToList());
            column3 = column3.OrderBy(x => x.Height).ToList();

            ViewBag.ImageGalleryColumn1 = column1;
            ViewBag.ImageGalleryColumn2 = column2;
            ViewBag.ImageGalleryColumn3 = column3;
            return View();
        }
    }
}
