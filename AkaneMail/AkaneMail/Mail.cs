using System;
using System.Collections.Generic;
using System.Text;

namespace AkaneMail
{
    public class Mail
    {
        // 静的フィールド(メールアカウント情報)
        public static string fromName;          // ユーザ(差出人)の名前
        public static string mailAddress;       // ユーザのメールアドレス
        public static string userName;          // ユーザ名
        public static string passWord;          // POP3のパスワード
        public static string popServer;         // POP3サーバ名
        public static string smtpServer;        // SMTPサーバ名
        public static int popPortNumber;        // POP3のポート番号
        public static int smtpPortNumber;       // SMTPのポート番号
        public static bool apopFlag;            // APOPのフラグ
        public static bool deleteMail;          // POP受信時メール削除フラグ
        public static bool popBeforeSMTP;       // POP before SMTPフラグ
        public static bool popOverSSL;          // POP3 over SSL/TLSフラグ
        public static bool smtpAuth;            // SMTP認証フラグ
        public static bool autoMailFlag;        // メール自動受信フラグ
        public static int getMailInterval;      // メール取得間隔
        public static bool popSoundFlag;        // メール着信音フラグ
        public static string popSoundName;      // メール着信音ファイル名
        public static bool bodyIEShow;          // HTMLメール表示フラグ
        public static bool minimizeTaskTray;    // 最小化時のタスクトレイフラグ

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
        public string cc;                   // CCアドレス
        public string bcc;                  // BCCアドレス
        public string priority;             // 優先度(None/Low/Normal/High)
        public string convert;              // バージョン識別用

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

    }
}
