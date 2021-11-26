using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.SeedWork;

namespace Yin.Domain.AggregateRoot.AdminUserAggregate
{
    public class AdminUserRole : KeyEntity
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// 角色Id
        /// </summary>
        public Guid RoleId { get; set; }

        public AdminUserRole(Guid userId, Guid roleId)
        {
            base.Init();
            UserId = userId;
            RoleId = roleId;
        }
    }
}
