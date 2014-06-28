using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkaneMail
{
    public class MainFormMessages
    {
        /// <summary>
        /// 製品名です。更新するときはアセンブリのProductNameを書き換えます。
        /// </summary>
        internal static readonly string ProductName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductName;

        internal static class FilePaths
        {
            internal static readonly string SettinFile = Application.StartupPath + @"\AkaneMail.xml";
            internal static readonly string TemporalyFolder = Application.StartupPath + @"\tmp";
            internal static readonly string MailData = Application.StartupPath + @"\Mail.dat";
        }

        internal static class Error
        {
            #region 固定エラーメッセージ
            /// <summary>nMailの読み込みでランタイムエラーが起きたときのメッセージです</summary>
            internal static readonly string Needx64nMail =
@"64bit版OSで32bit版OS用のnMail.dllを使用して実行した場合このエラーが表示されます。
お手数をお掛け致しますが同梱のnMail.dllをnMail.dll.32、nMail.dll.64をnMail.dllに名前を変更してAkane Mailを起動してください。";
            internal static readonly string InvalidContent = "分割されたメールの順番が正しくないか、該当しないファイルが入っています。";
            internal static readonly string LackContent = "分割されたメールが全て揃っていません";
            internal static readonly string LoadFailed = "添付ファイルがオープンできません。";
            #endregion

            #region 変数ありエラーメッセージ
            /// <summary>基本的なエラーメッセージです。</summary>
            /// <param name="message">エラーメッセージの内容</param>
            internal static string GeneralErrorMessage(string message)
            {
                return "エラーメッセージ:" + message;
            }

            internal static string GeneralErrorMessage(string message, int errorCode)
            {
                return string.Format("エラーNo:{0}  エラーメッセージ:{1}", errorCode, message);
            }
            #endregion
        }
        
        public static class Check
        {
            #region 固定確認メッセージ
            internal static readonly string ClearTrash =
@"ごみ箱の中身をすべて削除します。
よろしいですか？";
            internal static readonly string TrashComplete =
@"選択されたメールは完全に削除されます。
よろしいですか？";
            #endregion

            #region 変数有り確認メッセージ
            /// <summary>
            /// 安全でないファイルを開こうとするときの確認メッセージです。
            /// </summary>
            /// <param name="fileName"></param>
            /// <returns></returns>
            internal static string OpenUnsafeFile(string fileName)
            {
                return
fileName + @"を開きますか？
ファイルによってはウイルスの可能性もあるため、
注意してファイルを開いてください。";
            }
            #endregion
        }

        public static class Notification
        {
            #region 固定通知メッセージ
            internal static readonly string MailLoading = "メールデータの読み込み作業中です";
            internal static readonly string MailSending = "メール送信中";
            internal static readonly string MailSent = "メール送信完了";
            internal static readonly string MailReceiving = "メール受信中";
            internal static readonly string NewMail = "新着メール";
            internal static readonly string AllReceived = "新着のメッセージはありませんでした。";
            #endregion

            #region 変数あり通知メッセージ
            internal static string NewMailReceived(int count)
            {
               return count + "件の新着メールを受信しました。";
            }


            internal static string InternalSaved(string path, string name)
            {
                return string.Format("{0}に添付ファイル{1}を保存しました。", path, name);
            }
            #endregion

        }
    }
}
