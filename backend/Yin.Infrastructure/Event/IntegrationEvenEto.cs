using System;

namespace Yin.Infrastructure.Event
{
    /// <summary>
    /// cap 消息 体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IntegrationEvenEto<T>
    {
        ///<summary>
        /// 消息 id
        ///</summary>
        public Guid Id { get; set; }
        /// <summary>
        ///  创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public T Message { get; set; }
        /// <summary>
        ///  topic
        /// </summary>
        public string Topic { get; set; }
    }
}
