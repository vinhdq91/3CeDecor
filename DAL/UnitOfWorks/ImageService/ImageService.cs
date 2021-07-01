using AutoMapper;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.Repositories.ImageRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWorks.ImageService
{
    public class ImageService: IImageService
    {
        private readonly IImageRepository _imageRepos;
        private readonly IMapper _mapper;
        public ImageService(IImageRepository imageRepos, IMapper mapper)
        {
            _imageRepos = imageRepos;
            _mapper = mapper;
        }

        public async Task<IQueryable<ImageEntity>> GetListAllImagesAsync()
        {
            IQueryable<ImageEntity> imageEntities = await _imageRepos.GetAllAsync();
            return imageEntities;
            //return _mapper.Map<IEnumerable<ImageDto>>(imageEntities);
        }

        public async Task<ImageEntity> GetImageAsync(int imageId)
        {
            ImageEntity imageEntity = await _imageRepos.GetAsync(imageId);
            return imageEntity;
        }

        public async Task UpdateImageAsync(ImageEntity imageEntity)
        {
            await _imageRepos.UpdateAsync(imageEntity);
        }

        public async Task<IQueryable<ImageEntity>> GetListImagesByProductIdAsync()
        {
            IQueryable<ImageEntity> imageEntities = await _imageRepos.GetAllAsync();
            return imageEntities;
            //return _mapper.Map<IEnumerable<ImageDto>>(imageEntities);
        }

        public async Task<ImageEntity> AddImageAsync(ImageEntity imageEntity)
        {
            return await _imageRepos.AddAsync(imageEntity);
        }
    }
}
