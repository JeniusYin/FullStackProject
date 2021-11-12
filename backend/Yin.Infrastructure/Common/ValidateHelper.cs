using System;
using System.Linq;

namespace Yin.Infrastructure.Common
{
    /// <summary>
    /// 验证类公共方法
    /// </summary>
    public class ValidateHelper
    {
        #region 纯数字验证
        /// <summary>
        /// 检查一个字符串是否是纯数字构成的，一般用于查询字符串参数的有效性验证。
        /// </summary>
        /// <param name="value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool IsNumeric(string value)
        {
            return QuickValidate("^[-]?[1-9]*[0-9]*$", value);
        }
        #endregion

        #region 数字（包括小数和整数）验证
        /// <summary>
        /// 判断是否是数字，包括小数和整数。
        /// </summary>
        /// <param name="value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool IsNumber(string value)
        {
            return QuickValidate("^(0|([1-9]+[0-9]*))(.[0-9]+)?$", value);
        }
        #endregion

        #region 手机号验证
        /// <summary>
        /// 判断输入的字符串是否是一个合法的手机号
        /// </summary>
        /// <param name="value">需验证的字符串</param>
        /// <returns></returns>
        public static bool IsMobilePhone(string value)
        {
            return QuickValidate("^1\\d{10}$", value);
        }
        #endregion


        #region 港澳通行证验证
        /// <summary>  
        /// 验证港澳通行证
        /// </summary>  
        /// <param name="input">待验证的字符串</param>  
        /// <returns>是否匹配</returns>  
        public static bool IsHKOrMacaoPassport(string input)
        {
            //20200320暂停港澳台通行证用户添加
            //string pattern = @"^[a-zA-Z0-9]{6,11}$";
            string pattern = @"^(([HhMm]{1}([0-9]{8}|[0-9]{10}))|([0-9]{8}))$";
            return QuickValidate(pattern, input);
        }

        #endregion
        #region 纯数字验证
        /// <summary>
        /// 身份证验证
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCard(string Id)
        {

            //身份证黑名单
            if (Id == "422325198412240060")
            {
                return false;
            }

            if (Id.Length == 18)
            {

                bool check = CheckIDCard18(Id);

                return check;

            }

            else if (Id.Length == 15)
            {

                bool check = CheckIDCard15(Id);

                return check;

            }

            else
            {

                return false;

            }

        }
        /// <summary>
        /// 18位身份证验证
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCard18(string Id)
        {

            long n = 0;

            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {

                return false;//数字验证  

            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(Id.Remove(2)) == -1)
            {

                return false;//省份验证  

            }

            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");

            DateTime time = new DateTime();

            if (DateTime.TryParse(birth, out time) == false)
            {

                return false;//生日验证  

            }

            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');

            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');

            char[] Ai = Id.Remove(17).ToCharArray();

            int sum = 0;

            for (int i = 0; i < 17; i++)
            {

                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());

            }

            int y = -1;

            Math.DivRem(sum, 11, out y);

            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {

                return false;//校验码验证  

            }

            return true;//符合GB11643-1999标准  

        }
        /// <summary>
        /// 15位身份证验证
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static bool CheckIDCard15(string Id)
        {

            long n = 0;

            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {

                return false;//数字验证  

            }

            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";

            if (address.IndexOf(Id.Remove(2)) == -1)
            {

                return false;//省份验证  

            }

            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");

            DateTime time = new DateTime();

            if (DateTime.TryParse(birth, out time) == false)
            {

                return false;//生日验证  

            }

            return true;//符合15位身份证标准  

        }

        #endregion

        #region 邮箱验证
        /// <summary>
        /// 验证是否是有效邮箱地址
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsEmail(string strln)
        {
            return QuickValidate(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", strln);
        }
        #endregion

        /// <summary>
        /// 快速验证一个字符串是否符合指定的正则表达式。
        /// </summary>
        /// <param name="express">正则表达式的内容。</param>
        /// <param name="value">需验证的字符串。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool QuickValidate(string express, string value)
        {
            var myRegex = new System.Text.RegularExpressions.Regex(express);
            return value.Length != 0 && myRegex.IsMatch(value);
        }

        #region 姓名验证（验证汉字）

        /// <summary>
		/// 姓名验证（验证汉字）
		/// </summary>
		/// <param name="strInput">待验证的字符串</param>
		/// <returns>是中文返回true,否则false</returns>
		public static bool IsCN(string strInput)
        {
            // return QuickValidate("^[\u4e00-\u9fa5]{2,10}$",strInput);
            return JudegeCN(strInput, 2, 10);
        }

        /// <summary>
        /// CodePoint转UTF-16编码（两字节或者四字节）
        /// </summary>
        /// <param name="CodePoint"></param>
        /// <returns></returns>
        private static string UnicodeToUTF_16(int CodePoint)
        {
            //如果CodePoint不在Unicode字符集有效编码范围
            if (CodePoint < 0 || CodePoint > 0x10FFFF)
            {
                throw new ArgumentException("CodePoint不在有效范围内！，有效范围为0x0-0x10FFFF");
            }

            //如果是在BMP-基本多语言平面范围内0x0-0xFFFF（共65536个CodePoint）则按CodePoint进行两字节编码
            if (CodePoint <= 0xFFFF)
            {
                return string.Format(@"\u{0:x4}", CodePoint);
            }
            else//如果是在辅助平面范围内0x10000-0x10FFFF(共65536*16个CodePoint)则按如下规则进行编码，此时是四字节编码
            {
                CodePoint -= 0x10000;
                //高10位
                int high_10bit = CodePoint / (0x400) + 0xD800;
                //低10位
                int low_10bit = CodePoint % (0x400) + 0xDC00;

                return string.Format(@"\u{0:x4}\u{1:x4}", high_10bit, low_10bit);
            }

        }

        /// <summary>
        /// 判断是否是汉子
        /// </summary>
        /// <param name="text"></param>
        /// <param name="minLength"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool JudegeCN(string text, int minLength, int maxLength)
        {
            if (!IsIn(text.Length, minLength, maxLength)) return false;
            int i = 0;
            while (i < text.Count())
            {
                int unicodeIndex = text[i];
                //如果是两字节编码
                if ((unicodeIndex >= 0 && unicodeIndex <= 0xD7FF) || (unicodeIndex >= 0xE000 && unicodeIndex <= 0xFFFF))
                {
                    //两字节编码汉子CodePoint范围为4E00-9FA5 9FA6-9FEF 3400-4DB5
                    if ((unicodeIndex >= 0x4E00 && unicodeIndex <= 0x9FEF) || (unicodeIndex >= 0x3400 && unicodeIndex <= 0x4DB5))
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
                else //如果是四字节编码
                {
                    //汉字CodePoint范围20000 - 2A6D6 2A700 - 2B734 2B740 - 2B81D 2B820 - 2CEA1 2CEB0 - 2EBE0
                    int codePoint = UTF_16ToUnicode(text.Substring(i, 2));
                    if (IsIn(codePoint, 0x20000, 0x2A6D6) || IsIn(codePoint, 0x2A700, 0x2B734) || IsIn(codePoint, 0x2B740, 0x2B81D) || IsIn(codePoint, 0x2B820, 0x2CEA1) || IsIn(codePoint, 0x2CEB0, 0x2EBE0))
                    {
                        i = i + 2;
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private static bool IsIn(int target, int start, int end)
        {
            return target >= start && target <= end ? true : false;
        }
        /// <summary>
        /// 获得四字节UTF-16编码的CodePoint
        /// </summary>
        /// <param name="codeStr"></param>
        /// <returns></returns>
        private static int UTF_16ToUnicode(string codeStr)
        {
            int highByte = (codeStr[0] - 0xD800) * 0x400;
            int lowByte = (codeStr[1] - 0xDC00);
            return highByte + lowByte + 0x10000;
        }

        #endregion
    }
}
