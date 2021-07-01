using AutoMapper;
using DAL.Core.Enums;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.GalleryService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class GalleryAdminController : BaseController
    {
        private readonly IGalleryService _galleryService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GalleryAdminController(
            IGalleryService galleryService,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment
        )
        {
            _galleryService = galleryService;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ActionResult<List<GalleryDto>>> GalleryList()
        {
            IQueryable<GalleryEntity> galleryEntities = await _galleryService.GetListAllGalleryAsync(true);
            List<GalleryDto> listGalleryDtos = new List<GalleryDto>();
            if (galleryEntities != null)
            {
                foreach (GalleryEntity item in galleryEntities.ToList())
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

        #region Create
        [HttpGet]
        public IActionResult CreateGallery()
        {
            List<SelectListItem> listType = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryType.GalleryImage + "", Text = "Ảnh thư viện" },
                new SelectListItem(){ Value = (int)GalleryType.HomeSlide + "", Text = "Ảnh bìa trang chủ" },
                new SelectListItem(){ Value = (int)GalleryType.HomeVideo + "", Text = "Video trang chủ" }

            };

            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryStatus.Active + "", Text = "Mở khóa" },
                new SelectListItem(){ Value = (int)GalleryStatus.NotActive + "", Text = "Khóa" }

            };

            List<SelectListItem> listWidth = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryWidthEnum.Width1 + "", Text = (int)GalleryWidthEnum.Width1 + " px" }
            };

            List<SelectListItem> listHeight = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryHeightEnum.Height1 + "", Text = (int)GalleryHeightEnum.Height1 + " px" },
                new SelectListItem(){ Value = (int)GalleryHeightEnum.Height2 + "", Text = (int)GalleryHeightEnum.Height2 + " px" }
            };

            ViewBag.ListType = listType;
            ViewBag.ListStatus = listStatus;
            ViewBag.ListWidth = listWidth;
            ViewBag.ListHeight = listHeight;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGallery(GalleryDto galleryDto)
        {
            if (ModelState.IsValid)
            {
                // Xử lý lưu ảnh vào folder Gallery
                if (galleryDto.ImageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(galleryDto.ImageFile.FileName);
                    fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                    string extension = Path.GetExtension(galleryDto.ImageFile.FileName);
                    galleryDto.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Resource/Gallery/", fileName);
                    if (!Directory.Exists(wwwRootPath + "/Resource/Gallery"))
                    {
                        Directory.CreateDirectory(wwwRootPath + "/Resource/Gallery");
                    }
                    if (!Directory.Exists(path))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await galleryDto.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(galleryDto.ColorCode))
                {
                    galleryDto.ColorCode = "#fff";
                }
                GalleryEntity galleryEntity = _mapper.Map<GalleryEntity>(galleryDto);
                await _galleryService.AddGalleryAsync(galleryEntity);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("GalleryList");
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<ActionResult<GalleryDto>> EditGallery(int galleryId)
        {
            GalleryEntity galleryEntity = await _galleryService.GetGalleryAsync(galleryId);
            GalleryDto galleryDto = _mapper.Map<GalleryDto>(galleryEntity);
            galleryDto.ImageNameCurrent = galleryDto.ImageName;

            List<SelectListItem> listType = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryType.GalleryImage + "", Text = "Ảnh thư viện" },
                new SelectListItem(){ Value = (int)GalleryType.HomeSlide + "", Text = "Ảnh bìa trang chủ" },
                new SelectListItem(){ Value = (int)GalleryType.HomeVideo + "", Text = "Video trang chủ" }

            };

            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryStatus.Active + "", Text = "Mở khóa" },
                new SelectListItem(){ Value = (int)GalleryStatus.NotActive + "", Text = "Khóa" }

            };

            List<SelectListItem> listWidth = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryWidthEnum.Width1 + "", Text = (int)GalleryWidthEnum.Width1 + " px" }
            };

            List<SelectListItem> listHeight = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)GalleryHeightEnum.Height1 + "", Text = (int)GalleryHeightEnum.Height1 + " px" },
                new SelectListItem(){ Value = (int)GalleryHeightEnum.Height2 + "", Text = (int)GalleryHeightEnum.Height2 + " px" }
            };

            ViewBag.ListType = listType;
            ViewBag.ListStatus = listStatus;
            ViewBag.ListWidth = listWidth;
            ViewBag.ListHeight = listHeight;
            return View(galleryDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditGallery(GalleryDto galleryDto)
        {
            if (ModelState.IsValid)
            {
                if (galleryDto.ImageFile != null)
                {
                    // Save image to StrategyAvatars
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(galleryDto.ImageFile.FileName);
                    fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                    string extension = Path.GetExtension(galleryDto.ImageFile.FileName);
                    galleryDto.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Resource/Gallery/", fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await galleryDto.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    // Remove current image in Gallery
                    if (!string.IsNullOrEmpty(galleryDto.ImageNameCurrent))
                    {
                        string currentImagePath = Path.Combine(wwwRootPath + "/Resource/Gallery/", galleryDto.ImageNameCurrent);
                        currentImagePath = currentImagePath.Replace(@"/", @"\");
                        if (System.IO.File.Exists(currentImagePath))
                        {
                            System.IO.File.Delete(currentImagePath);
                        }
                    }
                }
                // Update Strategy
                GalleryEntity galleryInput = _mapper.Map<GalleryEntity>(galleryDto);
                await _galleryService.UpdateGalleryAsync(galleryInput);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("GalleryList");
        }
        #endregion
        public async Task<ActionResult> DeleteGallery(int galleryId)
        {
            if (galleryId > 0)
            {
                GalleryEntity galleryEntity = await _galleryService.GetGalleryAsync(galleryId);

                if (galleryEntity != null)
                {
                    if (!string.IsNullOrEmpty(galleryEntity.ImageName))
                    {
                        // Xóa ảnh khỏi Gallery
                        string currentImagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Resource/Gallery/", galleryEntity.ImageName);
                        currentImagePath = currentImagePath.Replace(@"/", @"\");
                        if (System.IO.File.Exists(currentImagePath))
                        {
                            System.IO.File.Delete(currentImagePath);
                        }
                    }
                }
                // Xóa strategy
                await _galleryService.DeleteGalleryAsync(galleryEntity);
            }
            return RedirectToAction("GalleryList");
        }
        public async Task<IActionResult> ChangeStatus(int galleryId)
        {
            GalleryEntity galleryEntity = await _galleryService.GetGalleryAsync(galleryId);
            if (galleryEntity.Status == (int)GalleryStatus.Active)
            {
                galleryEntity.Status = (int)GalleryStatus.NotActive;
            }
            else
            {
                galleryEntity.Status = (int)GalleryStatus.Active;
            }
            try
            {
                await _galleryService.UpdateGalleryAsync(galleryEntity);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return RedirectToAction("GalleryList");
        }
    }
}
