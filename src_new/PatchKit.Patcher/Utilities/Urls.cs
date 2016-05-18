using System;
using System.Text;

namespace PatchKit.Patcher.Utilities
{
    class Urls
    {
        public static string CombineRelative(string contextPath, params object[] args)
        {
            if (args.Length % 2 != 0)
            {
                throw new ArgumentException("Invalid number of params. Should be even.");
            }

            var sb = new StringBuilder();
            sb.Append(contextPath);

            if (args.Length > 0)
            {
                sb.Append("?");

                for (int i = 0; i < args.Length; i += 2)
                {
                    if (i > 0)
                    {
                        sb.Append("&");
                    }

                    string key = args[i].ToString();
                    string value = args[i + 1].ToString();
                    sb.Append(UrlEncode(key));
                    sb.Append("=");
                    sb.Append(UrlEncode(value));
                }
            }

            return sb.ToString();
        }

        private static string UrlEncode(string key)
        {
            return Uri.EscapeUriString(key);
        }
    }
}
