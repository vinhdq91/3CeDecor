using DAL.EntityFrameworkCore.Application;
using DAL.Models.Application;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repositories.GalleryRepository
{
    public class GalleryRepository : Repository<GalleryEntity>, IGalleryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public GalleryRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
    }
}
