using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AkaneMail
{
    static class ContentTransferEncodingExtensions
    {
        public static string ToQuotedPrintable(this byte[] convertee)
        {
            return string.Join("", convertee.Select(c =>
                    (c == '\r' || c == '\n' || c == '=' || c < 33 || c > 126) ?
                        "=" + ((int)c).ToString("X2") : ((char)c).ToString()));
        }

        public static byte[] FromQuotedPrintable(this string convertee)
        {
            var prepared = convertee.Replace("=\r\n", "");
            var matches = Regex.Matches(prepared, "(=([0-9A-Fa-f]{2}))|(.)", RegexOptions.Singleline);
            return matches.Cast<Match>().Select(m => m.Groups[2].Success ?
                Convert.ToByte(m.Groups[2].Value, 16) : (byte)m.Groups[3].Value[0]).ToArray();
        }

        public static string ToQuotedPrintable(this string convertee, string encoding = "UTF-8")
        {
            return convertee.ToQuotedPrintable(Encoding.GetEncoding(encoding));
        }

        public static string ToQuotedPrintable(this string convertee, Encoding encoding)
        {
            return ToQuotedPrintable(encoding.GetBytes(convertee));
        }

        public static string StringFromQuotedPrintable(this string convertee, string encoding = "UTF-8")
        {
            return convertee.StringFromQuotedPrintable(Encoding.GetEncoding(encoding));
        }

        public static string StringFromQuotedPrintable(this string convertee, Encoding encoding)
        {
            return encoding.GetString(FromQuotedPrintable(convertee));
        }

        public static string ToBase64(this byte[] convertee)
        {
            return Convert.ToBase64String(convertee);
        }

        public static byte[] FromBase64(this string convertee)
        {
            return Convert.FromBase64String(convertee);
        }

        public static string ToBase64(this string convertee, string encoding = "UTF-8")
        {
            return convertee.ToBase64(Encoding.GetEncoding(encoding));
        }

        public static string ToBase64(this string convertee, Encoding encoding)
        {
            return ToBase64(encoding.GetBytes(convertee));
        }
        public static string StringFromBase64(this string convertee, string encoding = "UTF-8")
        {
            return convertee.StringFromBase64(Encoding.GetEncoding(encoding));
        }
        public static string StringFromBase64(this string convertee, Encoding encoding)
        {
            return encoding.GetString(FromBase64(convertee));
        }
    }
}
