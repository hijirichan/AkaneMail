using System;
using System.Collections.Generic;
using System.Text;

namespace MailConvert
{
    public class Mail
    {
        // インスタンスフィールド(メールの情報)
        public string address;              // 差出人(宛先)アドレス
        public string header;               // メールヘッダ
        public string subject;              // メールの件名
        public string body;                 // メール本文
        public string attach;               // 添付ファイル
        public string date;                 // 受信(送信)日時
        public string size;                 // メールサイズ
        public string uidl;                 // UIDL
        public bool notReadYet;             // 未読・未送信フラグ
        public string convert;              // 1.01コンバートフラグ

        // コンストラクタ
        public Mail(string address, string header, string subject, string body, string attach, string date, string size, string uidl, bool notReadYet, string convert)
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
            this.convert = convert;
        }
    }
}
