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
            //文字列をバイト型配列にする
            byte[] bytesIn = System.Text.Encoding.UTF8.GetBytes(str);

            //DESCryptoServiceProviderオブジェクトの作成
            System.Security.Cryptography.DESCryptoServiceProvider des =
                new System.Security.Cryptography.DESCryptoServiceProvider();

            //共有キーと初期化ベクタを決定
            //パスワードをバイト配列にする
            byte[] bytesKey = System.Text.Encoding.UTF8.GetBytes(key);
            //共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            //暗号化されたデータを書き出すためのMemoryStream
            System.IO.MemoryStream msOut = new System.IO.MemoryStream();
            //DES暗号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform desdecrypt =
                des.CreateEncryptor();
            //書き込むためのCryptoStreamの作成
            System.Security.Cryptography.CryptoStream cryptStreem =
                new System.Security.Cryptography.CryptoStream(msOut,
                desdecrypt,
                System.Security.Cryptography.CryptoStreamMode.Write);
            //書き込む
            cryptStreem.Write(bytesIn, 0, bytesIn.Length);
            cryptStreem.FlushFinalBlock();
            //暗号化されたデータを取得
            byte[] bytesOut = msOut.ToArray();

            //閉じる
            cryptStreem.Close();
            msOut.Close();

            //Base64で文字列に変更して結果を返す
            return System.Convert.ToBase64String(bytesOut);
        }

        /// <summary>
        /// パスワード文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <param name="key">パスワード</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptPasswordString(string str)
        {
            // パスワード暗号化用のパス文字列
            string cryptPassString = "@K@nEmAiL_3DES_Key";

            //文字列をバイト型配列にする
            byte[] bytesIn = System.Text.Encoding.UTF8.GetBytes(str);

            //DESCryptoServiceProviderオブジェクトの作成
            System.Security.Cryptography.DESCryptoServiceProvider des =
                new System.Security.Cryptography.DESCryptoServiceProvider();

            //共有キーと初期化ベクタを決定
            //パスワードをバイト配列にする
            byte[] bytesKey = System.Text.Encoding.UTF8.GetBytes(cryptPassString);
            //共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            //暗号化されたデータを書き出すためのMemoryStream
            System.IO.MemoryStream msOut = new System.IO.MemoryStream();
            //DES暗号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform desdecrypt =
                des.CreateEncryptor();
            //書き込むためのCryptoStreamの作成
            System.Security.Cryptography.CryptoStream cryptStreem =
                new System.Security.Cryptography.CryptoStream(msOut,
                desdecrypt,
                System.Security.Cryptography.CryptoStreamMode.Write);
            //書き込む
            cryptStreem.Write(bytesIn, 0, bytesIn.Length);
            cryptStreem.FlushFinalBlock();
            //暗号化されたデータを取得
            byte[] bytesOut = msOut.ToArray();

            //閉じる
            cryptStreem.Close();
            msOut.Close();

            //Base64で文字列に変更して結果を返す
            return System.Convert.ToBase64String(bytesOut);
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
            System.Security.Cryptography.DESCryptoServiceProvider des =
                new System.Security.Cryptography.DESCryptoServiceProvider();

            //共有キーと初期化ベクタを決定
            //パスワードをバイト配列にする
            byte[] bytesKey = System.Text.Encoding.UTF8.GetBytes(key);
            //共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            //Base64で文字列をバイト配列に戻す
            byte[] bytesIn = System.Convert.FromBase64String(str);
            //暗号化されたデータを読み込むためのMemoryStream
            System.IO.MemoryStream msIn =
                new System.IO.MemoryStream(bytesIn);
            //DES復号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform desdecrypt =
                des.CreateDecryptor();
            //読み込むためのCryptoStreamの作成
            System.Security.Cryptography.CryptoStream cryptStreem =
                new System.Security.Cryptography.CryptoStream(msIn,
                desdecrypt,
                System.Security.Cryptography.CryptoStreamMode.Read);

            //復号化されたデータを取得するためのStreamReader
            System.IO.StreamReader srOut =
                new System.IO.StreamReader(cryptStreem,
                System.Text.Encoding.UTF8);
            //復号化されたデータを取得する
            string result = srOut.ReadToEnd();

            //閉じる
            srOut.Close();
            cryptStreem.Close();
            msIn.Close();

            return result;
        }

        /// <summary>
        /// 暗号化されたパスワード文字列を復号化する
        /// </summary>
        /// <param name="str">暗号化された文字列</param>
        /// <param name="key">パスワード</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptPasswordString(string str)
        {
            // パスワード暗号化用のパス文字列
            string cryptPassString = "@K@nEmAiL_3DES_Key";

            //DESCryptoServiceProviderオブジェクトの作成
            System.Security.Cryptography.DESCryptoServiceProvider des =
                new System.Security.Cryptography.DESCryptoServiceProvider();

            //共有キーと初期化ベクタを決定
            //パスワードをバイト配列にする
            byte[] bytesKey = System.Text.Encoding.UTF8.GetBytes(cryptPassString);
            //共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            //Base64で文字列をバイト配列に戻す
            byte[] bytesIn = System.Convert.FromBase64String(str);
            //暗号化されたデータを読み込むためのMemoryStream
            System.IO.MemoryStream msIn =
                new System.IO.MemoryStream(bytesIn);
            //DES復号化オブジェクトの作成
            System.Security.Cryptography.ICryptoTransform desdecrypt =
                des.CreateDecryptor();
            //読み込むためのCryptoStreamの作成
            System.Security.Cryptography.CryptoStream cryptStreem =
                new System.Security.Cryptography.CryptoStream(msIn,
                desdecrypt,
                System.Security.Cryptography.CryptoStreamMode.Read);

            //復号化されたデータを取得するためのStreamReader
            System.IO.StreamReader srOut =
                new System.IO.StreamReader(cryptStreem,
                System.Text.Encoding.UTF8);
            //復号化されたデータを取得する
            string result = srOut.ReadToEnd();

            //閉じる
            srOut.Close();
            cryptStreem.Close();
            msIn.Close();

            return result;
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
                for (int i = 0; i < bytes.Length; i++)
                    newBytes[i] = bytes[i];
            } else {
                int pos = 0;
                for (int i = 0; i < bytes.Length; i++) {
                    newBytes[pos++] ^= bytes[i];
                    if (pos >= newBytes.Length)
                        pos = 0;
                }
            }
            return newBytes;
        }
    }
}
