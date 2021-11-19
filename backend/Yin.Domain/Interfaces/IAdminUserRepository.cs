using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.AggregateRoot.AdminUserAggregate;

namespace Yin.Domain.Interfaces
{
    public interface IAdminUserRepository : IRepository<AdminUser>
    {
    }
}
