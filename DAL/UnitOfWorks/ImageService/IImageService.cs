using DAL.Dtos;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ImageService
{
    public interface IImageService
    {
        Task<IQueryable<ImageEntity>> GetListAllImagesAsync();
        Task<ImageEntity> AddImageAsync(ImageEntity imageEntity);
        Task<ImageEntity> GetImageAsync(int imageId);
        Task UpdateImageAsync(ImageEntity imageEntity);
    }
}
