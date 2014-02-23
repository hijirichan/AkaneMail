using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using nMail;

namespace AkaneMail
{
    public class Mail
    {
        // インスタンスフィールド(メールの情報)
        public string address { get; set; }              // 差出人(宛先)アドレス
        public string header { get; set; }               // メールヘッダ
        public string subject { get; set; }              // メールの件名
        public string body { get; set; }                 // メール本文
        public string attach { get; set; }               // 添付ファイル
        public string date { get; set; }                 // 受信(送信)日時
        public string size { get; set; }                 // メールサイズ
        public string uidl { get; set; }                 // UIDL
        public bool notReadYet { get; set; }             // 未読・未送信フラグ
        public string cc { get; set; }                   // CCアドレス
        public string bcc { get; set; }                  // BCCアドレス
        public string priority { get; set; }             // 優先度(None/Low/Normal/High)
        public string convert { get; set; }              // バージョン識別用

        // コンストラクタ
        public Mail(string address, string header, string subject, string body, string attach, string date, string size, string uidl, bool notReadYet, string convert, string cc, string bcc, string priority)
        {
            this.address = address;
            this.header = header;
            this.subject = subject;
            this.body = body;
            this.attach = attach;
            this.date = date;
            this.size = size;
            this.uidl = uidl;
            this.notReadYet = notReadYet;
            this.cc = cc;
            this.bcc = bcc;
            this.priority = priority;
            this.convert = convert;
        }

        /// <summary>
        /// 文字コードを取得する
        /// </summary>
        /// <param name="mailHeader">メールヘッダ</param>
        /// <returns>文字コード</returns>
        public static string ParseEncoding(string mailHeader)
        {
            Pop3 pop = new Pop3();

            // メールヘッダから文字コード文字列を抜き出す
            string codeName = pop.GetHeaderField("Content-Type:", mailHeader);

            codeName = codeName.Replace("\"", "");
            string[] arrayName = codeName.Split('=');
            codeName = arrayName[1];

            return codeName;
        }

        /// <summary>
        /// 重要度取得
        /// </summary>
        /// <param name="header">ヘッダ</param>
        /// <returns>重要度(urgent/normal/non-urgent)</returns>
        public static string ParsePriority(string header)
        {
            string _priority = "normal";
            string priority = "";

            Pop3 pop = new Pop3();

            // ヘッダにX-Priorityがあるとき
            if(header.Contains("X-Priority:")){
                priority = pop.GetHeaderField("X-Priority:", header);

                if(priority == "1" || priority == "2"){
                    _priority = "urgent";
                }
                else if (priority == "3"){
                    _priority = "normal";
                }
                else if (priority == "4" || priority == "5"){
                    _priority = "non-urgent";
                }
            }
            else if(header.Contains("X-MsMail-Priotiry:")){
                priority = pop.GetHeaderField("X-MsMail-Priotiry:", header);

                if(priority.ToLower() == "High"){
                    _priority = "urgent";
                }
                else if(priority.ToLower() == "Normal"){
                    _priority = "normal";
                }
                else if (priority.ToLower() == "low"){
                    _priority = "non-urgent";
                }
            }
            else if(header.Contains("Importance:")){
                priority = pop.GetHeaderField("Importance:", header);

                if(priority.ToLower() == "high"){
                    _priority = "urgent";
                }
                else if(priority.ToLower() == "normal"){
                    _priority = "normal";
                }
                else if(priority.ToLower() == "low"){
                    _priority = "non-urgent";
                }
            }
            else if (header.Contains("Priority:")){
                priority = pop.GetHeaderField("Priority:", header);
                // 重要度の文字列の長さが0以上のときは取得した重要度を入れる
                if(priority.Length > 0){
                    _priority = priority;
                }
            }
            return _priority;
        }

        /// <summary>
        /// HTMLからタグを取り除く
        /// </summary>
        /// <param name="htmlBody">HTML本文</param>
        /// <param name="mailHeader">メールヘッダ</param>
        /// <returns>タグが取り除かれた文字列</returns>
        public static string HtmlToText(string htmlBody, string mailHeader)
        {
            string codeName;        // 文字コード名
            string bodyCodeName;    // HTMLの文字コード名   

            // メールヘッダの文字コードを取得する
            codeName = ParseEncoding(mailHeader);

            // metaタグから正規表現で文字コードを取り出す
            Regex regEnc = new Regex("<meta.*?charset=(?<encode>.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // regEncにマッチする文字列を検索
            Match m = regEnc.Match(htmlBody);

            // HTML本文の中でcharset=文字コード名がマッチしたとき
            if(m.Success == true){
                // HTMLの文字コード名を取得する
                bodyCodeName = m.Groups["encode"].Value;
                // メールヘッダの文字コードとHTMLの文字コードが同じとき
                if(codeName.ToLower() == bodyCodeName.ToLower()){
                    // HTMLの文字コードで変換する
                    Byte[] b = Encoding.GetEncoding(bodyCodeName).GetBytes(htmlBody);
                    htmlBody = Encoding.GetEncoding(bodyCodeName).GetString(b);
                }
                else{
                    // 文字コードが異なる場合はメールヘッダの文字コードで変換する
                    Byte[] b = Encoding.GetEncoding(codeName).GetBytes(htmlBody);
                    htmlBody = Encoding.GetEncoding(codeName).GetString(b);
                }
            }
            else{
                // htmlBody内にcharset指定が存在しないときは
                // メールヘッダの文字コードで変換する
                Byte[] b = Encoding.GetEncoding(codeName).GetBytes(htmlBody);
                htmlBody = Encoding.GetEncoding(codeName).GetString(b);
            }

            // 正規表現の設定(<script>, <noscript>)
            Regex re1 = new Regex("<(no)?script.*?script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 正規表現の設定(<style>)
            Regex re2 = new Regex("<style.*?style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 正規表現の設定(すべてのタグ)
            Regex re3 = new Regex("<.*?>", RegexOptions.Singleline);

            // タグを取り除く
            htmlBody = re1.Replace(htmlBody, "");
            htmlBody = re2.Replace(htmlBody, "");
            htmlBody = re3.Replace(htmlBody, "");

            // 変換できなかった特殊文字を個別置換
            htmlBody = htmlBody.Replace("&nbsp;", " ");
            htmlBody = htmlBody.Replace("&shy;", " ");
            htmlBody = htmlBody.Replace("&lt;", "<");
            htmlBody = htmlBody.Replace("&gt;", ">");
            htmlBody = htmlBody.Replace("&amp;", "&");
            htmlBody = htmlBody.Replace("&quot;", "\"");
            htmlBody = htmlBody.Replace("&copy;", "(c)");
            htmlBody = htmlBody.Replace("&reg;", "(R)");
            htmlBody = htmlBody.Replace("&trade;", "TM");
            htmlBody = htmlBody.Replace("\r\n\r\n\r\n\r\n", "");

            return htmlBody;
        }

    }
}
