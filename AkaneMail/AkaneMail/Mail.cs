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
        public string Address { get; set; }              // 差出人(宛先)アドレス
        public string Header { get; set; }               // メールヘッダ
        public string Subject { get; set; }              // メールの件名
        public string Body { get; set; }                 // メール本文
        public string Attach { get; set; }               // 添付ファイル
        public string Date { get; set; }                 // 受信(送信)日時
        public string Size { get; set; }                 // メールサイズ
        public string Uidl { get; set; }                 // UIDL
        public bool NotReadYet { get; set; }             // 未読・未送信フラグ
        public string Cc { get; set; }                   // CCアドレス
        public string Bcc { get; set; }                  // BCCアドレス
        public string Priority { get; set; }             // 優先度(None/Low/Normal/High)
        public string Convert { get; set; }              // バージョン識別用

        public string[] Attaches { get { return Attach.Split(','); } }

        // コンストラクタ
        public Mail(string address, string header, string subject, string body, string attach, string date, string size, string uidl, bool notReadYet, string convert, string cc, string bcc, string priority)
        {
            this.Address = address;
            this.Header = header;
            this.Subject = subject;
            this.Body = body;
            this.Attach = attach;
            this.Date = date;
            this.Size = size;
            this.Uidl = uidl;
            this.NotReadYet = notReadYet;
            this.Cc = cc;
            this.Bcc = bcc;
            this.Priority = priority;
            this.Convert = convert;
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
            var arrayName = codeName.Split('=');
            codeName = arrayName[1];

            return codeName;
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
            var regEnc = new Regex("<meta.*?charset=(?<encode>.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // regEncにマッチする文字列を検索
            var m = regEnc.Match(htmlBody);

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
                var b = Encoding.GetEncoding(codeName).GetBytes(htmlBody);
                htmlBody = Encoding.GetEncoding(codeName).GetString(b);
            }

            // 正規表現の設定(<script>, <noscript>)
            var re1 = new Regex("<(no)?script.*?script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 正規表現の設定(<style>)
            var re2 = new Regex("<style.*?style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 正規表現の設定(すべてのタグ)
            var re3 = new Regex("<.*?>", RegexOptions.Singleline);

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
