using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.SeedWork;

namespace Yin.Domain.AggregateRoot.MenuAggregate
{
    /// <summary>
    /// 接口API地址信息表
    /// </summary>
    public class Moudles : RecordableEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; private set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string Area { get; private set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string Controller { get; private set; }
        /// <summary>
        /// Action名称
        /// </summary>
        public string Action { get; private set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; private set; }
        /// <summary>
        /// 是否为右侧菜单
        /// </summary>
        public bool IsMenu { get; private set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; private set; }
        /// <summary>
        /// 排序值
        /// </summary>
        public int Sort { get; private set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 父Id
        /// </summary>
        public Guid ParentId { get; private set; }
    }
}
