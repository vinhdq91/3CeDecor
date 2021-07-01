using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.ImageService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IImageService _imageService;
        public ImagesController(IWebHostEnvironment hostEnvironment, IImageService imageService)
        {
            this._hostEnvironment = hostEnvironment;
            this._imageService = imageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<ImageDto>> CreateImg([Bind("ImageId,Title,ImageFile")] ImageDto imageModel)
        {
            if (ModelState.IsValid)
            {
                //Save image to wwwroot/Resource/AllImages
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(imageModel.ImageFile.FileName);
                fileName = StringHandle.RemoveUnicode(fileName).Replace(" ", "_");
                string extension = Path.GetExtension(imageModel.ImageFile.FileName);
                imageModel.ImageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Resource/AllImages/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await imageModel.ImageFile.CopyToAsync(fileStream);
                }
                //Insert record
                ImageEntity imageEntity = new ImageEntity();
                await _imageService.AddImageAsync(imageEntity);
                return RedirectToAction(nameof(Index));
            }
            return View(imageModel);
        }

        //public async Task<IEnumerable<ImageDto>> GetImgs()
        //{
        //    return await _imageService.GetListAllImagesAsync();
        //}
    }
}