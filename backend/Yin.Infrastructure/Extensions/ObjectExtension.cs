using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yin.Infrastructure.Extensions
{
    /// <summary>
    ///  ObjectExtension 
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 判断是否为null "string"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool NotNullAndEmpty(this string str)
        {
            return !string.IsNullOrWhiteSpace(str) && !"string".Equals(str, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 文字转大小写
        /// </summary>
        /// <param name="s"></param>
        /// <param name="resolve"></param>
        /// <returns></returns>
        public static string PropResolve(this string s, PropNameResolve resolve)
        {
            switch (resolve)
            {
                case PropNameResolve.CamelCase:
                    return s.ToCamelCase();
                case PropNameResolve.AllLower:
                    return s.ToLower();
                case PropNameResolve.AllUpper:
                    return s.ToUpper();
                default: return s;
            }
        }
        public static string ToCamelCase(this string s)
        {
            if (IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // if the next character is a space, which is not considered uppercase 
                    // (otherwise we wouldn't be here...)
                    // we want to ensure that the following:
                    // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                    // The code was written in such a way that the first word in uppercase
                    // ends when if finds an uppercase letter followed by a lowercase letter.
                    // now a ' ' (space, (char)32) is considered not upper
                    // but in that case we still want our current character to become lowercase
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = ToLower(chars[i]);
                    }

                    break;
                }

                chars[i] = ToLower(chars[i]);
            }

            return new string(chars);
        }
        public static char ToLower(char c)
        {
#if HAVE_CHAR_TO_STRING_WITH_CULTURE
            c = char.ToLower(c, CultureInfo.InvariantCulture);
#else
            c = char.ToLowerInvariant(c);
#endif
            return c;
        }
        /// <summary>
        /// string.IsNullOrEmpty
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }
        /// <summary>
        /// 生成请求参数
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GenerateRequestPar(this Dictionary<string, string> dic)
        {
            var query = new StringBuilder();
            // 将参数字符串拼接后 进行 sha1 加密
            foreach (var kv in dic)
            {
                string pKey = kv.Key;
                string pValue = kv.Value;
                query.Append($"{pKey}={pValue}&");
            }

            string result = query.ToString().Substring(0, query.ToString().Length - 1);
            return result;
        }

        /// <summary>
        /// 对象转字典
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ObjectToMap(this object obj)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理
                    if (m.Invoke(obj, new object[] { }) != null)
                    {
                        var value = m.Invoke(obj, new object[] { }).ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            map.Add(p.Name.PropResolve(PropNameResolve.CamelCase), value); // 向字典添加元素
                        }
                    }
                }
            }
            return map;
        }

        public static Dictionary<string, object> ObjectToObjMap(this object obj, params string[] ignoreAttr)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();

            Type t = obj.GetType(); // 获取对象对应的类， 对应的类型

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

            foreach (PropertyInfo p in pi)
            {
                if (ignoreAttr?.Contains(p.Name.ToLower()) == true)
                    continue;
                MethodInfo m = p.GetGetMethod();

                if (m != null && m.IsPublic)
                {
                    // 进行判NULL处理
                    if (m.Invoke(obj, new object[] { }) != null)
                    {
                        var value = m.Invoke(obj, new object[] { });
                        if (value != null)
                        {
                            map.Add(p.Name.ToCamelCase().PropResolve(PropNameResolve.CamelCase), value); // 向字典添加元素
                        }
                    }
                }
            }
            return map;
        }
        /// <summary>
        /// 把list分成小list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<List<T>> SubList<T>(this List<T> list, int size = 100)
        {
            int batch = Convert.ToInt32(Math.Ceiling(list.Count * 1.0 / size));
            var result = new List<List<T>>();
            Enumerable.Range(0, batch).ToList().ForEach(t =>
             {
                 result.Add(list.Skip(t * size).Take(size).ToList());
             });
            return result;
        }
    }
    /// <summary>
    /// 首字母格式enum
    /// </summary>
    public enum PropNameResolve
    {
        /// <summary>
        /// 首字母小写
        /// </summary>
        [Description("首字母小写")]
        CamelCase = 1,
        /// <summary>
        /// 全部大写
        /// </summary>
        [Description("全部大写")]
        AllUpper = 2,
        /// <summary>
        /// 全部小写
        /// </summary>
        [Description("全部小写")]
        AllLower = 3,
    }

    /// <summary>
    ///字典排序
    /// </summary>
    public class ComparerString : IComparer<string>
    {
        /// <summary>
        /// https://www.cnblogs.com/similar/p/6739293.html
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}

