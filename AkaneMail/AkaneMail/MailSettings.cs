using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace AkaneMail
{
    public class MailSettings
    {
        public string m_fromName = string.Empty;                        // ユーザ(差出人)の名前 
        public string m_mailAddress = string.Empty;                     // ユーザのメールアドレス
        public string m_userName = string.Empty;                        // ユーザ名
        public string m_passWord = string.Empty;                        // POP3のパスワード
        public string m_popServer = string.Empty;                       // POP3サーバ名
        public string m_smtpServer = string.Empty;                      // SMTPサーバ名
        public int m_popPortNo = 110;                                   // POP3のポート番号
        public int m_smtpPortNo = 25;                                   // SMTPのポート番号
        public bool m_apopFlag = false;                                 // APOPのフラグ
        public bool m_deleteMail = false;                               // POP受信時メール削除フラグ
        public bool m_popBeforeSMTP = false;                            // POP before SMTPフラグ
        public bool m_popOverSSL = false;                               // POP3 over SSL/TLSフラグ
        public bool m_smtpAuth = false;                                 // SMTP認証フラグ
        public bool m_autoMailFlag = false;                             // メール自動受信フラグ
        public int m_getMailInterval = 0;                               // メール受信間隔
        public bool m_popSoundFlag = false;                             // メール着信音フラグ
        public string m_popSoundName = string.Empty;                    // メール着信音ファイル名
        public bool m_bodyIEShow = false;                               // HTMLメール表示フラグ
        public bool m_minimizeTaskTray = false;                         // 最小化時のタスクトレイフラグ
        public int m_windowLeft = 0;                                    // ウィンドウの左上のLeft座標
        public int m_windowTop = 0;                                     // ウィンドウの左上のTop座標
        public int m_windowWidth = 0;                                   // ウィンドウの幅
        public int m_windowHeight = 0;                                  // ウィンドウの高さ
        public FormWindowState m_windowStat = FormWindowState.Normal;   // ウィンドウの状態
    }
}
