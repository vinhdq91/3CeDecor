using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.GalleryService
{
    public interface IGalleryService
    {
        Task<IQueryable<GalleryEntity>> GetListAllGalleryAsync(bool isAdmin = false);
        Task<GalleryEntity> GetGalleryAsync(int galleryId);
        Task AddGalleryAsync(GalleryEntity galleryEntity);
        Task UpdateGalleryAsync(GalleryEntity galleryEntity);
        Task DeleteGalleryAsync(GalleryEntity galleryEntity);
    }
}
