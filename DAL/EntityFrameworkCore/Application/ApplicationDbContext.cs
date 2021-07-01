using DAL.Models.Account;
using DAL.Models.Application;
using DAL.Models.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.EntityFrameworkCore.Application
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public string CurrentUserId { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductCategoryEntity> ProductCategories { get; set; }
        public DbSet<ProductProductCategory> ProductProductCategories { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderDetailEntity> OrderDetails { get; set; }
        public DbSet<OrderHistoryEntity> OrderHistories { get; set; }
        public DbSet<ArticleBlogEntity> ArticleBlogs { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<DiscountCodeEntity> DiscountCodes { get; set; }
        public DbSet<ProvinceEntity> Province { get; set; }
        public DbSet<DistrictEntity> District { get; set; }
        public DbSet<WardEntity> Ward { get; set; }
        public DbSet<StrategyEntity> Strategy { get; set; }
        public DbSet<StrategyProduct> StrategyProducts { get; set; }
        public DbSet<GalleryEntity> Gallery { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            const string priceDecimalType = "decimal(18,2)";

            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(250));
            //builder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(250));

            //builder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(250));

            // Apply tất cả các class implement IEntityTypeConfiguration trong file IdentityConfiguration.cs
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<CustomerEntity>().Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Entity<CustomerEntity>().HasIndex(c => c.Name);
            builder.Entity<CustomerEntity>().Property(c => c.Email).HasMaxLength(100);
            builder.Entity<CustomerEntity>().Property(c => c.PhoneNumber).IsUnicode(false).HasMaxLength(30);
            builder.Entity<CustomerEntity>().Property(c => c.DistrictCode).HasMaxLength(70);
            builder.Entity<CustomerEntity>().Property(c => c.ProvinceCode).HasMaxLength(70);
            builder.Entity<CustomerEntity>().Property(c => c.WardCode).HasMaxLength(70);
            builder.Entity<CustomerEntity>().Property(c => c.DistrictCode).HasMaxLength(6);
            builder.Entity<CustomerEntity>().ToTable($"App{nameof(this.Customers)}");

            builder.Entity<ProductCategoryEntity>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Entity<ProductCategoryEntity>().Property(p => p.Description).HasMaxLength(500);
            builder.Entity<ProductCategoryEntity>().ToTable($"App{nameof(this.ProductCategories)}");

            builder.Entity<ProductEntity>().Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Entity<ProductEntity>().HasIndex(p => p.Name);
            builder.Entity<ProductEntity>().Property(p => p.Description).HasMaxLength(5000);
            builder.Entity<ProductEntity>().ToTable($"App{nameof(this.Products)}");
            builder.Entity<ProductEntity>().Property(p => p.Price).HasColumnType(priceDecimalType);
            builder.Entity<ProductEntity>().Property(p => p.Discount).HasColumnType(priceDecimalType);

            // Quan hệ nhiều - nhiều giữa Product & ProductCategory
            builder.Entity<ProductProductCategory>().HasKey(sc => new { sc.ProductId, sc.ProductCategoryId });
            builder.Entity<ProductProductCategory>()
                    .HasOne<ProductEntity>(sc => sc.ProductEntity)
                    .WithMany(s => s.ProductProductCategories)
                    .HasForeignKey(sc => sc.ProductId);
            builder.Entity<ProductProductCategory>()
                .HasOne<ProductCategoryEntity>(sc => sc.ProductCategoryEntity)
                .WithMany(s => s.ProductProductCategories)
                .HasForeignKey(sc => sc.ProductCategoryId);


            builder.Entity<OrderEntity>().Property(o => o.Comments).HasMaxLength(500);
            builder.Entity<OrderEntity>().ToTable($"App{nameof(this.Orders)}");
            builder.Entity<OrderEntity>().Property(p => p.Discount).HasColumnType(priceDecimalType);

            builder.Entity<OrderDetailEntity>().ToTable($"App{nameof(this.OrderDetails)}");
            builder.Entity<OrderDetailEntity>().Property(p => p.UnitPrice).HasColumnType(priceDecimalType);
            builder.Entity<OrderDetailEntity>().Property(p => p.Discount).HasColumnType(priceDecimalType);

            builder.Entity<OrderHistoryEntity>().ToTable($"App{nameof(this.OrderHistories)}");

            builder.Entity<DiscountCodeEntity>().ToTable($"App{nameof(this.DiscountCodes)}");

            builder.Entity<ArticleBlogEntity>().Property(p => p.Title).IsRequired().HasMaxLength(250);

            builder.Entity<StrategyEntity>().HasIndex(s => s.Name).IsUnique(true);

            builder.Entity<StrategyProduct>().HasKey(sp => new { sp.ProductId, sp.StrategyId });
            builder.Entity<StrategyProduct>()
                    .HasOne<ProductEntity>(sc => sc.Product)
                    .WithMany(s => s.StrategyProducts)
                    .HasForeignKey(sc => sc.ProductId);
            builder.Entity<StrategyProduct>()
                    .HasOne<StrategyEntity>(sc => sc.Strategy)
                    .WithMany(s => s.StrategyProducts)
                    .HasForeignKey(sc => sc.StrategyId);

            builder.Entity<GalleryEntity>().ToTable(nameof(this.Gallery));
        }

        public override int SaveChanges()
        {
            UpdateAuditEntities();
            return base.SaveChanges();
        }


        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            UpdateAuditEntities();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }


        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }


        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateAuditEntities();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void UpdateAuditEntities()
        {
            var modifiedEntries = ChangeTracker.Entries()
                .Where(x => x.Entity is IAuditableEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entry in modifiedEntries)
            {
                var entity = (IAuditableEntity)entry.Entity;
                DateTime now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedDate = now;
                    entity.CreatedBy = CurrentUserId;
                }
                else
                {
                    base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                    base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
                }

                entity.UpdatedDate = now;
                entity.UpdatedBy = CurrentUserId;
            }
        }
    }
}
