using nMail;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    /// <summary>メールの優先度を表します。</summary>
    public static class MailPriority
    {
        /// <summary>通常の優先度を表します。</summary>
        public static readonly string Normal = "normal";
        /// <summary>高い優先度を表します。</summary>
        public static readonly string Urgent = "urgent";
        /// <summary>高くない優先度を表します。</summary>
        public static readonly string NonUrgent = "non-urgent";

        private static string[] priorityArray = new[] { "", Urgent, Urgent, Normal, NonUrgent, NonUrgent };

        /// <summary>
        /// 重要度取得
        /// </summary>
        /// <param name="header">ヘッダ</param>
        /// <returns>重要度(urgent/normal/non-urgent)</returns>
        public static string Parse(string header)
        {
            string _priority = Normal;
            string priority = "";

            var pop = new Pop3();

            if (header.Contains("X-Priority:")) {
                priority = pop.GetHeaderField("X-Priority:", header);
                var i = 0;
                if (int.TryParse(priority, out i)) {
                    _priority = priorityArray[i];
                }
            }
            else if (header.TryGetHeaders(pop, out priority, "X-MsMail-Priotiry", "Importance")) {
                if (priority == "high") {
                    _priority = Urgent;
                }
                else if (priority == "normal") {
                    _priority = Normal;
                }
                else if (priority == "low") {
                    _priority = NonUrgent;
                }
            }
            else if (header.Contains("Priority:")) {
                priority = pop.GetHeaderField("Priority:", header);
                // 重要度の文字列の長さが0以上のときは取得した重要度を入れる
                if (priority.Length > 0) {
                    _priority = priority.ToLower();
                }
            }
            return _priority;
        }

        public static Color GetPriorityColor(Mail mail)
        {
            if (mail.Priority == Urgent) {
                return Color.Tomato;
            }
            else if (mail.Priority == NonUrgent) {
                return Color.LightBlue;
            }
            else {
                return Color.Black;
            }
        }

    }

    internal static class Pop3Extender
    {

        private static string GetHeader(this string str, Pop3 pop, string h)
        {
            if (str == default(string)) return null;
            else return pop.GetHeaderField(h, str + ":").ToLower();
        }
        internal static bool TryGetHeaders(this string header, Pop3 pop, out string value, params string[] contents)
        {
            value = contents.FirstOrDefault(header.Contains).GetHeader(pop, header);
            return value != null;
        }
    }
}
