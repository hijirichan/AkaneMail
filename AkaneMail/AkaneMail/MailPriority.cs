using nMail;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    /// <summary>メールの優先度を表します。</summary>
    static class MailPriority
    {
        /// <summary>通常の優先度を表します。</summary>
        public static readonly string Normal = "normal";
        /// <summary>高い優先度を表します。</summary>
        public static readonly string Urgent = "urgent";
        /// <summary>高くない優先度を表します。</summary>
        public static readonly string NonUrgent = "non-urgent";

        /// <summary>
        /// 重要度取得
        /// </summary>
        /// <param name="header">ヘッダ</param>
        /// <returns>重要度(urgent/normal/non-urgent)</returns>
        public static string Parse(string header)
        {
            string _priority = "normal";
            string priority = "";

            var pop = new Pop3();

            // ヘッダにX-Priorityがあるとき
            if (header.Contains("X-Priority:")) {
                priority = pop.GetHeaderField("X-Priority:", header);

                if (priority == "1" || priority == "2") {
                    _priority = Urgent;
                }
                else if (priority == "3") {
                    _priority = Normal;
                }
                else if (priority == "4" || priority == "5") {
                    _priority = NonUrgent;
                }
            }
            else if (header.Contains("X-MsMail-Priotiry:")) {
                priority = pop.GetHeaderField("X-MsMail-Priotiry:", header);

                if (priority.ToLower() == "High") {
                    _priority = Urgent;
                }
                else if (priority.ToLower() == "Normal") {
                    _priority = Normal;
                }
                else if (priority.ToLower() == "low") {
                    _priority = NonUrgent;
                }
            }
            else if (header.Contains("Importance:")) {
                priority = pop.GetHeaderField("Importance:", header);

                if (priority.ToLower() == "high") {
                    _priority = Urgent;
                }
                else if (priority.ToLower() == "normal") {
                    _priority = Normal;
                }
                else if (priority.ToLower() == "low") {
                    _priority = NonUrgent;
                }
            }
            else if (header.Contains("Priority:")) {
                priority = pop.GetHeaderField("Priority:", header);
                // 重要度の文字列の長さが0以上のときは取得した重要度を入れる
                if (priority.Length > 0) {
                    _priority = priority;
                }
            }
            return _priority;
        }
    }
}
