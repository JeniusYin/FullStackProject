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
    internal class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
    {
        public void Configure(EntityTypeBuilder<AdminUser> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Account).IsRequired().HasMaxLength(128);
            builder.Property(t => t.Password).IsRequired().HasMaxLength(128);
            builder.Property(t => t.RealName).IsRequired().HasMaxLength(128);

            builder.ToTable("AdminUser");
        }
    }
}
