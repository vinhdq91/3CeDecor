using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.ImageRepository
{
    public class ImageRepository: Repository<ImageEntity>, IImageRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public ImageRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

    }
}
