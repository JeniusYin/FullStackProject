using DotNetCore.CAP;
using System.Threading.Tasks;

namespace Yin.Application.EventBus
{
    /// <summary>
    /// 发送领域事件（不暴露）
    /// </summary>
    public interface IInMemoryEventBus
    {
        bool IsEmpty();
        /// <summary>
        /// 领域事件使用（会包含在一个 事务中发送）
        /// 在事务中使用
        /// </summary>
        /// <param name="capPublisher"></param>
        /// <returns></returns>
        Task PublishEvent(ICapPublisher capPublisher);
    }
}
