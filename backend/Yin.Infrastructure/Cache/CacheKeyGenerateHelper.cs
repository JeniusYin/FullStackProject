using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Yin.Infrastructure.Cache
{
    public class CacheKeyGenerateHelper
    {
        public static string CacheKeyGenerate(string template, object parameters)
        {
            if (string.IsNullOrEmpty(template))
                throw new ArgumentNullException(nameof(template), "redis cache keyTemple is null");
            var regCollection = Regex.Matches(template, @"\{(.+?)\}");
            PropertyInfo[] propertyInfos = null;
            if (parameters == null)
            {
                if (regCollection.Count != 0)
                    throw new ArgumentException("模板参数个数不匹配，请检查");
            }
            else
            {
                propertyInfos = parameters.GetType().GetProperties();
                if (regCollection.Count != propertyInfos.Length)
                    throw new ArgumentException("模板参数个数不匹配，请检查");
            }
            foreach (Match reg in regCollection)
            {
                if (reg.Success)
                {
                    var replaceName = reg.Groups[0].Value;
                    var propName = reg.Groups[1].Value;
                    if (replaceName.Contains("."))
                    {
                        // 复杂参数
                        throw new NotImplementedException("暂不支持复杂参数模板");
                    }
                    else
                    {
                        // 简单参数 直接替换
                        var propertyInfo = propertyInfos.FirstOrDefault(t =>
                            t.Name.Equals(propName, StringComparison.CurrentCultureIgnoreCase));
                        if (propertyInfo == null)
                            throw new ArgumentException("模板参数名称不匹配，请检查", nameof(propName));
                        var value = propertyInfo.GetValue(parameters);
                        if (value is string)
                        {
                            template = template.Replace(replaceName, value.ToString());
                        }
                        else
                        {
                            if (value is IEnumerable enumerable)
                            {
                                throw new NotImplementedException("暂不支持IEnumerable且非string数据");
                            }
                            else
                            {
                                template = template.Replace(replaceName, value.ToString());
                            }
                        }
                    }
                }
            }

            return template;
        }
    }
}
