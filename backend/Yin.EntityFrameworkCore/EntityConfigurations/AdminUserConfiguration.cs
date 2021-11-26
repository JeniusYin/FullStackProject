using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.AggregateRoot.AdminUserAggregate;

namespace Yin.EntityFrameworkCore.EntityConfigurations
{
    public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Account).IsRequired().HasMaxLength(128);
            builder.Property(t => t.Password).IsRequired().HasMaxLength(128);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(128);
            builder.ToTable("AdminUser");
        }
    }

    public class AdminRoleConfiguration : IEntityTypeConfiguration<AdminRole>
    {
        public void Configure(EntityTypeBuilder<AdminRole> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(128);
            builder.Property(t => t.Description).HasMaxLength(1024);

            builder.ToTable("AdminRole");
        }
    }

    public class AdminUserRoleConfiguration : IEntityTypeConfiguration<AdminUserRole>
    {
        public void Configure(EntityTypeBuilder<AdminUserRole> builder)
        {
            builder.HasKey(t => t.Id);

            builder.ToTable("AdminUserRole");
        }
    }
}
