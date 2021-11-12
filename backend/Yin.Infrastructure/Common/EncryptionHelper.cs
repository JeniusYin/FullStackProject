using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Yin.Infrastructure.Common
{
    /// <summary>
    /// 加密相关类
    /// </summary>
    public static class EncryptionHelper
    {
        /// <summary>
        /// MD5加密（之前老密码 没有补全16进制,可能不满32位）
        /// </summary>
        /// <param name="str">待加密字符串</param>
        /// <param name="is32">是否补全32位</param>
        /// <returns></returns>
        public static string ToMd5([NotNull] string str, bool is32 = true)
        {
            using (MD5 md5 = MD5.Create())
            {
                var format = is32 ? "x2" : "x";
                var newBuffer = md5.ComputeHash(Encoding.Default.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                foreach (var t in newBuffer)
                {
                    sb.Append(t.ToString(format));
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 验证md5值
        /// 盐值为空，则不计算盐值(之前老密码没有验证)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="md5Str">md5加密后(有盐值为带盐加密，无盐则原字符串MD5)的字符串</param>
        /// <param name="salt">盐值</param>
        /// <returns></returns>
        public static bool CheckMd5Salt([NotNull] string str, [NotNull] string md5Str, string salt)
        {
            var is32 = md5Str.Length == 32;
            if (!string.IsNullOrEmpty(salt))
            {
                // 有salt 比较salt
                var strMd5 = ToMd5(str, is32).ToLower();
                return ToMd5($"{strMd5}{salt}").Equals(md5Str, StringComparison.CurrentCultureIgnoreCase);
            }
            else
            {
                // 没有salt 直接比较md5的内容
                return ToMd5(str, is32).Equals(md5Str, StringComparison.CurrentCultureIgnoreCase);
            }
        }

    }
}
