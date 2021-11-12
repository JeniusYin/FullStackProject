using System.ComponentModel;

namespace Yin.Infrastructure.Model
{
    /// <summary>
    /// 通用返回类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseType<T>
    {
        /// <summary>
        ///  http 状态码
        ///  成功 200 
        ///  失败 100 系列
        /// </summary>
        [Description("状态码")]
        public int Code { get; set; } = 99;
        /// <summary>
        /// 描述信息
        /// </summary>
        [Description("描述信息")]
        public string Msg { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        [Description("返回数据")]
        public T Data { get; set; }

        /// <summary>
        /// ok
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        public ResponseType<T> Ok(T data, string msg = "")
        {
            Code = 200;
            Data = data;
            if (!string.IsNullOrEmpty(msg))
                Msg = msg;
            return this;
        }
    }
    public class ResponseType
    {
        /// <summary>
        ///  http 状态码
        ///  成功 200 
        ///  失败 100 系列
        /// </summary>
        [Description("状态码")]
        public int Code { get; set; } = 99;
        /// <summary>
        /// 描述信息
        /// </summary>
        [Description("描述信息")]
        public string Msg { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        [Description("返回数据")]
        public object Data { get; set; }

    }
}
