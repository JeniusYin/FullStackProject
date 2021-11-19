using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.AggregateRoot.AdminUserAggregate;
using Yin.Domain.Interfaces;
using Yin.EntityFrameworkCore.Context;

namespace Yin.EntityFrameworkCore.Repositories
{
    public class AdminUserRepository : BaseRepository<AdminUser>, IAdminUserRepository
    {
        public AdminUserRepository(MyDbContext context) : base(context)
        {
        }
    }
}
