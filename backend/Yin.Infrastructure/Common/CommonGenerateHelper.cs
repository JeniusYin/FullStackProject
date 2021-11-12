using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static Yin.Infrastructure.Common.EncryptionHelper;

namespace Yin.Infrastructure.Common
{
    /// <summary>
    /// 通用生成类
    /// </summary>
    public class CommonGenerateHelper
    {
        /// <summary>
        /// 生成N位随机数 
        /// </summary>
        /// <param name="n">N位随机数</param>
        /// <returns>生成的N位随机数</returns>
        public static string RandCode(int n)
        {
            var arrChar = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var num = new StringBuilder();
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            foreach (var i in Enumerable.Range(0, n))
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }
            return num.ToString();
        }
        /// <summary>
        /// 邮箱显示
        /// 大于6位邮箱 前三位 **** 后三位@
        /// 小于6位邮箱 前两位 **** 后两位
        /// 小于4位邮箱 前一位 **** 后一位
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public static string MailShowHide(string mail)
        {
            if (string.IsNullOrEmpty(mail))
            {
                return mail;
            }
            var index = mail.LastIndexOf("@", StringComparison.Ordinal);
            var head = mail.Substring(0, index);
            var foot = mail.Substring(index);
            var showMail = mail;
            if (index < 0)
            {
                return showMail;
            }
            if (index > 6)
            {
                var replaceStr = head.Substring(3, index - 6);
                showMail = $"{head.Replace(replaceStr, "****")}{foot}";
            }
            else if (index == 6)
            {
                showMail = mail.Insert(3, "****");
            }
            else if (index > 4)
            {
                var replaceStr = head.Substring(2, index - 4);
                showMail = $"{head.Replace(replaceStr, "****")}{foot}";
            }
            else if (index == 4)
            {
                showMail = mail.Insert(2, "****");
            }
            else if (index > 2)
            {
                var replaceStr = head.Substring(1, index - 2);
                showMail = $"{head.Replace(replaceStr, "****")}{foot}";
            }
            else if (index == 2)
            {
                showMail = mail.Insert(1, "****");
            }
            return showMail;
        }
        /// <summary>
        /// 手机号显示
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static string PhoneNumberShowHide(string phoneNumber)
        {
            if (!ValidateHelper.IsMobilePhone(phoneNumber))
            {
                return phoneNumber;
            }

            var header = phoneNumber.Substring(0, 3);
            var foot = phoneNumber.Substring(7, 4);
            return $"{header}****{foot}";
        }

        /// <summary>
        /// 24 小时 显示
        /// 几小时前
        /// 几分钟前
        /// 超过24小时 显示 实际日期
        /// </summary>
        /// <param name="time"></param>
        /// <param name="now">默认为 DateTime.Now</param>
        /// <returns></returns>
        public static string DateTime24Show(DateTime time, DateTime? now = null)
        {
            now ??= DateTime.Now;
            var timeSpan = now.Value - time;
            return timeSpan.TotalDays > 1 ? time.ToString("yyyy/MM/dd") :
                timeSpan.TotalHours > 1 ? $"{Math.Floor(timeSpan.TotalHours)}小时前" :
                timeSpan.TotalMinutes > 1 ? $"{Math.Floor(timeSpan.TotalMinutes)}分钟前" :
            $"{Math.Floor(timeSpan.TotalSeconds)}秒前";
        }
    }
}
