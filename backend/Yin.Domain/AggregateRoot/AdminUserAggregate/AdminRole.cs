using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yin.Domain.SeedWork;

namespace Yin.Domain.AggregateRoot.AdminUserAggregate
{
    public class AdminRole : RecordableEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; private set; }
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

        public AdminRole(string name, int sort, string description)
        {
            base.Init();
            Name = name;
            Enabled = true;
            Sort = sort;
            Description = description;
        }
    }
}
