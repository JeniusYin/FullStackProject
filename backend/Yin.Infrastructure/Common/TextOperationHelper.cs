using System.Collections.Generic;
using System.Linq;

namespace Yin.Infrastructure.Common
{
    public class TextOperationHelper
    {
        /// <summary>
        /// 对html 进行转义
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string ReplaceHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return html;
            return HtmlReplace.Aggregate(html,
                (temp, keyValue) =>
                    temp.Replace(keyValue.Key, keyValue.Value));
        }

        private static readonly IReadOnlyDictionary<string, string> HtmlReplace = new Dictionary<string, string>()
        {
            {"<","&lt;"},
            {">","&gt;"}
        };
    }
}
