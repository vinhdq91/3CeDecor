using DAL.EntityFrameworkCore.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DAL.Models.Application;

namespace DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, new()
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly DbSet<TEntity> _entities;
        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _entities = _applicationDbContext.Set<TEntity>();
        }

        public async virtual Task<IQueryable<TEntity>> GetAllAsync()
        {
            IQueryable<TEntity> listAllEntity = null;
            try
            {
                listAllEntity = _entities.AsQueryable();
            }
            catch(Exception ex)
            {
                return null;
            }
            return listAllEntity;
        }

        public async virtual Task<TEntity> GetAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async virtual Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entities.SingleOrDefaultAsync(predicate);
        }

        public async virtual Task<TEntity> AddAsync(TEntity entity)       
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} entity must not be null");
            }

            try
            {
                await _applicationDbContext.AddAsync(entity);
                await SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be saved");
            }
        }

        // Dùng cho việc update các property thường, không update image
        public async virtual Task<TEntity> UpdateNoImageAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateNoImageAsync)} entity must not be null");
            }

            try
            {
                _applicationDbContext.Update(entity);
                await SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated");
            }
        }

        public async virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateAsync)} entity must not be null");
            }

            try
            {
                // Xử lý update: Loại bỏ track foreign key imageId bằng AsNoTracking trước khi Update
                if (entity.GetType() == typeof(ProductEntity))
                {
                    ProductEntity productInput = entity as ProductEntity;
                    ProductEntity productOutput = await _applicationDbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == productInput.Id);
                    productOutput = productInput;
                    // Không update bảng ImageEntities, set null
                    productOutput.ImageIds = null;
                    _applicationDbContext.Update(productOutput);
                }

                else if (entity.GetType() == typeof(ArticleBlogEntity))
                {
                    ArticleBlogEntity articleInput = entity as ArticleBlogEntity;
                    ArticleBlogEntity articleOutput = await _applicationDbContext.ArticleBlogs.AsNoTracking().FirstOrDefaultAsync(x => x.Id == articleInput.Id);
                    articleOutput = articleInput;
                    // Không update bảng ImageEntities, set null
                    articleOutput.ImageIds = null;
                    _applicationDbContext.Update(articleOutput);
                }

                else if (entity.GetType() == typeof(CustomerEntity))
                {
                    CustomerEntity customerInput = entity as CustomerEntity;
                    CustomerEntity customerOutput = await _applicationDbContext.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == customerInput.Id);
                    customerOutput = customerInput;
                    // Không update bảng ImageEntities, set null
                    customerOutput.ImageIds = null;
                    _applicationDbContext.Update(customerOutput);
                }

                else
                {
                    _applicationDbContext.Update(entity);
                }
                await SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be updated");
            }
        }

        public async virtual Task<TEntity> RemoveAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(RemoveAsync)} entity must not be null");
            }

            try
            {
                _applicationDbContext.Remove(entity);
                await SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception($"{nameof(entity)} could not be removed");
            }
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.Where(predicate);
        }

        public virtual int Count()
        {
            return _entities.Count();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _applicationDbContext.SaveChangesAsync();
        }
    }
}
