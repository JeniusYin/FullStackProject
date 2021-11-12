using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yin.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 异步saveChange
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        /// <summary>
        /// 发送领域事件 保存到数据库
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
