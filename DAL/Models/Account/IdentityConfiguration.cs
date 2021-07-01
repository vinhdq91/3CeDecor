using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Account
{
    // Vinhdq create - Tạo configure set tài khoản admin default khi migrate db mới
    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        private const string adminId = "2301D884-221A-4E7D-B509-0113DCC043E2";

        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {

            builder.HasData(
                    new ApplicationRole
                    {
                        Id = adminId,
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    }
                );
        }
    }

    public class AdminConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        private const string adminId = "B22698B8-42A2-4115-9631-1C2D1E2AC5F8";

        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            var admin = new ApplicationUser
            {
                Id = adminId,
                IsAdminUser = true,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                PhoneNumber = "0868070921",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = new Guid().ToString("D"),
            };

            admin.PasswordHash = PassGenerate(admin);

            builder.HasData(admin);
        }

        public string PassGenerate(ApplicationUser user)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, "admin123456");
        }
    }

    public class UsersWithRolesConfig : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        private const string adminUserId = "B22698B8-42A2-4115-9631-1C2D1E2AC5F8";
        private const string adminRoleId = "2301D884-221A-4E7D-B509-0113DCC043E2";

        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            IdentityUserRole<string> iur = new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            };

            builder.HasData(iur);
        }
    }
}
