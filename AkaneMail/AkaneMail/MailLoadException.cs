using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AkaneMail
{
  /// <summary>
  /// メールの読み込みに失敗した時にスローされる例外。
  /// </summary>
  public class MailLoadException : Exception
  {
    /// <summary>
    /// MailLoadException クラスの新しいインスタンスを初期化します。
    /// </summary>
    public MailLoadException() : base() { }

    /// <summary>
    /// 指定したエラー メッセージを使用して、MailLoadException クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">例外の原因を説明するメッセージ。</param>
    public MailLoadException(string message) : base(message) { }

    /// <summary>
    /// 指定したエラー メッセージと、この例外の原因である内部例外への参照を使用して、MailLoadException クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">例外の原因を説明するメッセージ。</param>
    /// <param name="innerException">現在の例外の原因である例外。内部例外が指定されていない場合は null。</param>
    public MailLoadException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// シリアル化したデータを使用して、MailLoadException クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="info">スローされている例外に関するシリアル化済みオブジェクト データを保持している SerializationInfo。</param>
    /// <param name="contest">転送元または転送先に関するコンテキスト情報を含んでいる StreamingContext。</param>
    public MailLoadException(SerializationInfo info, StreamingContext contest) : base(info, contest) { }
  }
}
