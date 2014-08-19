using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using nMail;
using System.Windows.Forms;
using System.IO;

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

        public string[] Attachments { get { return Attach.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); } }

        // コンストラクタ
        //TODO 引数減らす
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

        public Mail(Pop3 pop, bool unread, string covert)
        {
            Address = pop.From;
            Header = pop.Header;
            Subject = pop.Subject;
            Body = pop.Body;
            Attach = pop.FileName;
            Date = pop.DateString;
            Size = pop.Size.ToString();
            Uidl = pop.Uidl;
            Cc = pop.GetDecodeHeaderField("Cc:");
            Bcc = "";
            Convert = "";
            Priority = MailPriority.Parse(pop.Header);
            NotReadYet = unread;
        }

        public bool Update(string address = null, string header = null, string subject = null, string body = null, string attach = null, string date = null, string size = null, string uidl = null, bool? read = null, string convert = null, string cc = null, string bcc = null, string priority = null)
        {
            this.Address = address ?? Address;
            this.Header = header ?? Header;
            this.Subject = subject ?? Subject;
            this.Body = body ?? Body;
            this.Attach = attach ?? Attach;
            this.Date = date ?? Date;
            this.Size = size ?? Size;
            this.Uidl = uidl ?? Uidl;
            this.NotReadYet = read ?? NotReadYet;
            this.Cc = cc ?? Cc;
            this.Bcc = bcc ?? Bcc;
            this.Priority = priority ?? Priority;
            this.Convert = convert ?? Convert;
            return true;
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

        private static Encoding DetectEncoding(string htmlBody, string mailHeader)
        {
            var codeName = ParseEncoding(mailHeader);
       
            var regEnc = new Regex("<meta.*?charset=(?<encode>.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var m = regEnc.Match(htmlBody);

            if (m.Success) {
                var bodyCodeName = m.Groups["encode"].Value;
                //var encoding = codeName.ToLower() == bodyCodeName.ToLower() ? bodyCodeName : codeName;
                // 同じことのような…
                var encoding = codeName;
                return Encoding.GetEncoding(encoding);
            }
            else {
                return Encoding.GetEncoding(codeName);
            }
        }

        /// <summary>
        /// HTMLからタグを取り除く
        /// </summary>
        /// <param name="htmlBody">HTML本文</param>
        /// <param name="mailHeader">メールヘッダ</param>
        /// <returns>タグが取り除かれた文字列</returns>
        public static string HtmlToText(string htmlBody, string mailHeader)
        {
            var encode = DetectEncoding(htmlBody, mailHeader);

            htmlBody = htmlBody.SafeRencode(encode);

            var script = new Regex("<(no)?script.*?script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var style = new Regex("<style.*?style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var tags = new Regex("<.*?>", RegexOptions.Singleline);

            // タグを取り除く
            htmlBody = script.Replace(htmlBody, "");
            htmlBody = style.Replace(htmlBody, "");
            htmlBody = tags.Replace(htmlBody, "");

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

        /// <summary>
        /// 選択したメールが送信メールか調べる
        /// </summary>
        public bool IsMailToSend()
        {
            return this.Header.Length == 0;
        }

        public IEnumerable<ToolStripItem> GenerateMenuItem(bool enableWhenRemoved = false)
        {
            return NmailAttachEx.GenerateMenuItem("", this.Attachments, enableWhenRemoved);
        }

    }

    public static class NmailAttachEx
    {
        public static IEnumerable<ToolStripItem> GenerateMenuItem(this nMail.Attachment attach, bool enableWhenRemoved = false)
        {
            return GenerateMenuItem(attach.Path + "\\", attach.FileNameList as IEnumerable<string>, enableWhenRemoved);
        }

        public static IEnumerable<ToolStripItem> GenerateMenuItem(string rootPath, IEnumerable<string> attaches, bool enableWhenRemoved = false)
        {
            foreach (var attachFile in attaches)
            {
                if (File.Exists(rootPath + attachFile))
                {
                    yield return new ToolStripMenuItem
                    {
                        Text = attachFile,
                        Image = System.Drawing.Icon.ExtractAssociatedIcon(rootPath + attachFile).ToBitmap()
                    };
                }
                else
                {
                    yield return new ToolStripMenuItem
                    {
                        Text = attachFile + "は削除されています。",
                        Enabled = enableWhenRemoved
                    };
                }
            }
        }
    }

    internal static class ByteArrayExtender
    {
        private static string DecodeString(this byte[] bytes, Encoding encode)
        {
            return encode.GetString(bytes);
        }

        private static byte[] EncodeString(this string input, Encoding encode)
        {
            return encode.GetBytes(input);
        }

        public static string SafeRencode(this string input, Encoding encode)
        {
            return input.EncodeString(encode).DecodeString(encode);
        }
    }
}
