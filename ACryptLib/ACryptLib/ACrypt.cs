using System;
using System.Collections.Generic;
using System.Text;

namespace ACryptLib
{
    public class ACrypt
    {
        /// <summary>
        /// 文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <param name="key">パスワード</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptString(string str, string key)
        {
            // 文字列をバイト型配列にする
            var bytesIn = System.Text.Encoding.UTF8.GetBytes(str);

            // DESCryptoServiceProviderオブジェクトの作成
            var des = new System.Security.Cryptography.DESCryptoServiceProvider();

            // 共有キーと初期化ベクタを決定

            // パスワードをバイト配列にする
            var bytesKey = System.Text.Encoding.UTF8.GetBytes(key);

            // 共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            // 暗号化されたデータを書き出すためのMemoryStream
            var msOut = new System.IO.MemoryStream();

            // DES暗号化オブジェクトの作成
            var desdecrypt = des.CreateEncryptor();

            // 書き込むためのCryptoStreamの作成
            var cryptStreem = new System.Security.Cryptography.CryptoStream(msOut, desdecrypt, System.Security.Cryptography.CryptoStreamMode.Write);

            // 書き込む
            cryptStreem.Write(bytesIn, 0, bytesIn.Length);
            cryptStreem.FlushFinalBlock();

            // 暗号化されたデータを取得
            byte[] bytesOut = msOut.ToArray();

            // 閉じる
            cryptStreem.Close();
            msOut.Close();

            // Base64で文字列に変更して結果を返す
            return System.Convert.ToBase64String(bytesOut);
        }

        /// <summary>
        /// パスワード文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptPasswordString(string str)
        {
            // パスワード暗号化用のパス文字列を使用して文字列を暗号化する。
            return EncryptString(str, "@K@nEmAiL_3DES_Key");
        }

        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        /// <param name="str">暗号化された文字列</param>
        /// <param name="key">パスワード</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptString(string str, string key)
        {
            //DESCryptoServiceProviderオブジェクトの作成
            var des = new System.Security.Cryptography.DESCryptoServiceProvider();

            // 共有キーと初期化ベクタを決定
            // パスワードをバイト配列にする
            var bytesKey = System.Text.Encoding.UTF8.GetBytes(key);

            // 共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            // Base64で文字列をバイト配列に戻す
            var bytesIn = System.Convert.FromBase64String(str);

            // 暗号化されたデータを読み込むためのMemoryStream
            var msIn = new System.IO.MemoryStream(bytesIn);

            // DES復号化オブジェクトの作成
            var desdecrypt = des.CreateDecryptor();

            // 読み込むためのCryptoStreamの作成
            var cryptStreem = new System.Security.Cryptography.CryptoStream(msIn, desdecrypt, System.Security.Cryptography.CryptoStreamMode.Read);

            // 復号化されたデータを取得するためのStreamReader
            var srOut = new System.IO.StreamReader(cryptStreem, System.Text.Encoding.UTF8);

            // 復号化されたデータを取得する
            var result = srOut.ReadToEnd();

            // 閉じる
            srOut.Close();
            cryptStreem.Close();
            msIn.Close();

            return result;
        }

        /// <summary>
        /// 暗号化されたパスワード文字列を復号化する
        /// </summary>
        /// <param name="str">暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptPasswordString(string str)
        {
            // パスワード暗号化用のパス文字列を使用して文字列を復号化する。
            return DecryptString(str, "@K@nEmAiL_3DES_Key");
        }

        /// <summary>
        /// 共有キー用に、バイト配列のサイズを変更する
        /// </summary>
        /// <param name="bytes">サイズを変更するバイト配列</param>
        /// <param name="newSize">バイト配列の新しい大きさ</param>
        /// <returns>サイズが変更されたバイト配列</returns>
        private static byte[] ResizeBytesArray(byte[] bytes, int newSize)
        {
            byte[] newBytes = new byte[newSize];

            if (bytes.Length <= newSize) {
                bytes.CopyTo(newBytes, 0);
            }
            else {
                for (int i = 0, p = 0; i < bytes.Length; i++, p = (p + 1) % newBytes.Length) {
                    newBytes[p] ^= bytes[i];
                }
            }
            return newBytes;
        }
    }
}
