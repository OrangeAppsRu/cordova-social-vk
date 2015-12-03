using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VK.WindowsPhone.SDK.Util
{
    static class StrUtil
    {
        public static string ForUI(this string backendTextString)
        {
            if (string.IsNullOrEmpty(backendTextString))
            {
                return "";
            }

            string res = backendTextString;

            res = res.Replace("\r\n", "\n");

            res = res.Replace("\n", "\r\n");

            res = WebUtility.HtmlDecode(res);

            return res.Trim();
        }

        public static string MakeIntoOneLine(this string str)
        {
            if (str == null)
                return "";

            str = str.Replace(Environment.NewLine, " ");
            str = str.Replace("\n", " ");
            return str;
        }

        public static string GetCommaSeparated(this List<string> ids)
        {
            StringBuilder sb = new StringBuilder();

            int count = ids.Count;

            for (int i = 0; i < count; i++)
            {
                sb = sb.Append(ids[i]);
                if (i != count - 1)
                {
                    sb = sb.Append(",");
                }
            }

            return sb.ToString();
        }

    }
}
