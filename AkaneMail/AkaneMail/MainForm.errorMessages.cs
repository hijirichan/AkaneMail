using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkaneMail
{
    public static class MainFormMessages
    {
        /// <summary>
        /// 製品名です。更新するときはアセンブリのProductNameを書き換えます。
        /// </summary>
        internal static readonly string ProductName = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).ProductName;
        public static class Error
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
            #endregion
        }

        public static class Warning
        {

        }

        public static class Notification
        {

        }
    }
}
