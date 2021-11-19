using System.ComponentModel;

namespace Yin.Infrastructure.Model
{
    public enum UserRoleType
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        [Description("普通用户")]
        Normal = 1,
        /// <summary>
        /// 后台用户
        /// </summary>
        [Description("后台用户")]
        Admin = 2,
        /// <summary>
        /// 系统服务
        /// </summary>
        [Description("系统服务")]
        System = 3,
    }
}
