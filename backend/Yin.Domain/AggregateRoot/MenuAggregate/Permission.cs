using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.SeedWork;

namespace Yin.Domain.AggregateRoot.MenuAggregate
{
    /// <summary>
    /// 路由菜单表
    /// </summary>
    public class Permission : RecordableEntity
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
        /// 是否为按钮
        /// </summary>
        public bool IsButton { get; private set; }
        /// <summary>
        /// 是否为隐藏菜单
        /// </summary>
        public bool IsHide { get; private set; }
        /// <summary>
        /// 是否KeepAlive
        /// </summary>
        public bool IsKeepAlive { get; private set; }
        /// <summary>
        /// 按钮事件
        /// </summary>
        public string Func { get; private set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; private set; }
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
        /// 上一级菜单Id
        /// </summary>
        public Guid ParentId { get; private set; }
        /// <summary>
        /// 接口Id
        /// </summary>
        public Guid MoudleId { get; private set; }
    }
}
