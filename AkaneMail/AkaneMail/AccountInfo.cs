using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkaneMail
{
    public class AccountInfo
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

        // 静的フィールド(動作環境情報)
        public static bool autoMailFlag;        // メール自動受信フラグ
        public static int getMailInterval;      // メール取得間隔
        public static bool popSoundFlag;        // メール着信音フラグ
        public static string popSoundName;      // メール着信音ファイル名
        public static bool bodyIEShow;          // HTMLメール表示フラグ
        public static bool minimizeTaskTray;    // 最小化時のタスクトレイフラグ

        /// <summary>
        /// 差出人の名前とメールアドレスの組み合わせを取得します。
        /// </summary>
        public static string FromAddress
        {
            get { return fromName + "<" + mailAddress + ">"; }
        }

        /// <summary>
        ///  アカウント情報(初期値)を設定します。
        /// </summary>
        public static void Reset()
        {
            AccountInfo.fromName = "";
            AccountInfo.mailAddress = "";
            AccountInfo.userName = "";
            AccountInfo.passWord = "";
            AccountInfo.smtpServer = "";
            AccountInfo.popServer = "";
            AccountInfo.popPortNumber = 110;
            AccountInfo.smtpPortNumber = 25;
            AccountInfo.apopFlag = false;
            AccountInfo.deleteMail = false;
            AccountInfo.popBeforeSMTP = false;
            AccountInfo.popOverSSL = false;
            AccountInfo.smtpAuth = false;
            AccountInfo.autoMailFlag = false;
            AccountInfo.getMailInterval = 10;
            AccountInfo.popSoundName = "";
            AccountInfo.bodyIEShow = false;
            AccountInfo.minimizeTaskTray = false;
        }
    }
}
