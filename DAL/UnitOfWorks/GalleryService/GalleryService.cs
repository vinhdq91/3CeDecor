using AutoMapper;
using DAL.Core.Enums;
using DAL.Models.Application;
using DAL.Repositories.GalleryRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.GalleryService
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _galleryRepos;
        private readonly IMapper _mapper;
        public GalleryService(IGalleryRepository galleryRepos, IMapper mapper)
        {
            _galleryRepos = galleryRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<GalleryEntity>> GetListAllGalleryAsync(bool isAdmin = false)
        {
            IQueryable<GalleryEntity> galleryEntities = await _galleryRepos.GetAllAsync();
            if (!isAdmin)
            {
                galleryEntities = galleryEntities.Where(x => x.Status == (int)GalleryStatus.Active);
            }
            return galleryEntities;
        }

        public async Task<GalleryEntity> GetGalleryAsync(int galleryId)
        {
            return await _galleryRepos.GetSingleOrDefaultAsync(x => x.Id == galleryId);
        }

        public async Task AddGalleryAsync(GalleryEntity galleryEntity)
        {
            await _galleryRepos.AddAsync(galleryEntity);
        }

        public async Task UpdateGalleryAsync(GalleryEntity galleryEntity)
        {
            await _galleryRepos.UpdateAsync(galleryEntity);
        }

        public async Task DeleteGalleryAsync (GalleryEntity galleryEntity)
        {
            await _galleryRepos.RemoveAsync(galleryEntity);
        }
    }
}
