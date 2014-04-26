using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
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
            var utf8 = Encoding.UTF8;
            var bytesIn = utf8.GetBytes(str);
            var bytesKey = utf8.GetBytes(key);
            
            var des = new DESCryptoServiceProvider();
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            using (var msOut = new MemoryStream()) {
                var desdecrypt = des.CreateEncryptor();
                using (var cryptStreem = new CryptoStream(msOut, desdecrypt, CryptoStreamMode.Write)) {
                    cryptStreem.Write(bytesIn, 0, bytesIn.Length);
                    cryptStreem.FlushFinalBlock();
                    return Convert.ToBase64String(msOut.ToArray());
                }
            }
        }

        /// <summary>
        /// パスワード文字列を暗号化する
        /// </summary>
        /// <param name="str">暗号化する文字列</param>
        /// <returns>暗号化された文字列</returns>
        public static string EncryptPasswordString(string str)
        {
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
            var bytesKey = Encoding.UTF8.GetBytes(key);

            var des = new DESCryptoServiceProvider();
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            var bytesIn = Convert.FromBase64String(str);
            using (var msIn = new MemoryStream(bytesIn)) {
                var desdecrypt = des.CreateDecryptor();
                using (var cryptStreem = new CryptoStream(msIn, desdecrypt, CryptoStreamMode.Read)) {
                    var srOut = new StreamReader(cryptStreem, Encoding.UTF8);
                    return srOut.ReadToEnd();
                }
                
            }
        }

        /// <summary>
        /// 暗号化されたパスワード文字列を復号化する
        /// </summary>
        /// <param name="str">暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public static string DecryptPasswordString(string str)
        {
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
            var newBytes = new byte[newSize];
            foreach (var i in Enumerable.Range(0, bytes.Length)) {
                newBytes[i % newBytes.Length] ^= bytes[i];
            }
            return newBytes;
        }
    }
}
