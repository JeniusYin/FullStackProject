using System;
using Yin.Infrastructure.GuidGenerator;

namespace Yin.Domain.SeedWork
{
    public class RecordableEntity : KeyEntity
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; protected set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Guid? Creator { get; protected set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; protected set; }

        /// <summary>
        /// 修改人
        /// </summary>
        public Guid? ModifiedBy { get; protected set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; protected set; }

        protected virtual void Init(Guid creator = default, Guid modifiedBy = default)
        {
            base.Id = GuidGenerateHelper.GenerateSequenceGuid();
            CreateTime = DateTime.Now;
            Creator = creator;
            ModifyTime = DateTime.Now;
            ModifiedBy = modifiedBy;
            IsDelete = false;
        }
    }
}
