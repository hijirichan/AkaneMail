using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using nMail;

namespace AkaneMail
{
  /// <summary>
  /// 送信BOX、受信BOX、削除済みBOXをもつメールボックスを表します。
  /// </summary>
  public class MailBox : IEnumerable<Mail>
  {
    private Dictionary<MailKind, List<Mail>> _mails = new Dictionary<MailKind,List<Mail>>();

    /// <summary>
    /// 指定された種類のメールを格納するコレクションを取得します。
    /// </summary>
    /// <param name="kind">取得するメールの種類。</param>
    /// <returns>指定されたの種類のメールを格納するコレクション。</returns>
    public IList<Mail> this[MailKind kind]
    {
      get
      {
        // 指定された種類のメールを格納するListがまだ作られていなかった場合、空のListを返す。
        if (!_mails.ContainsKey(kind))
          return new List<Mail>();
        return _mails[kind].AsReadOnly();
      }
    }

    /// <summary>
    /// メールを、指定された種類で追加します。
    /// </summary>
    /// <param name="item">追加するメール。</param>
    /// <param name="kind">メールの種類。</param>
    /// <exception cref="ArgumentNullException"><paramref name="item"/>が null です。</exception>
    public void Add(Mail item, MailKind kind)
    {
      if (!_mails.ContainsKey(kind))
        _mails[kind] = new List<Mail>();
      _mails[kind].Add(item);
    }

    /// <summary>
    /// すべてのメールを反復処理する列挙子を返します。
    /// </summary>
    /// <returns>すべてのメールを反復処理する列挙子。</returns>
    public IEnumerator<Mail> GetEnumerator()
    {
      var unitedMails = _mails.Values.Aggregate((a, b) =>
      {
        a.AddRange(b);
        return a;
      });
      return unitedMails.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    // TODO: 長くて且つ複雑なので、もっと簡潔に書きなおす。
    // TODO: このメソッドがこのクラスにあることが正しくない可能性がある。
    //       どのクラスに書くのか、またメソッド名が適切か検討を要する。
    /// <summary>
    /// メールデータをファイルから読み込みます。
    /// </summary>
    public void MailDataLoad()
    {
      // 予期せぬエラーの時にメールの本文が分かるようにするための変数
      string expSubject = "";
      int n = 0;
      var lockobj = new object();

      // スレッドのロックをかける
      lock (lockobj)
      {
        if (File.Exists(Application.StartupPath + @"\Mail.dat"))
        {
          try
          {
            // ファイルストリームをストリームリーダに関連付ける
            using (var reader = new StreamReader(Application.StartupPath + @"\Mail.dat", Encoding.UTF8))
            {
              var kinds = new MailKind[] { MailKind.Send, MailKind.Receive, MailKind.Delete };
              // GetHederFieldとHeaderプロパティを使うためPop3クラスを作成する
              using (var pop = new Pop3())
              {
                // データを読み出す
                foreach (var kind in kinds)
                {
                  try
                  {
                    // メールの件数を読み出す
                    n = Int32.Parse(reader.ReadLine());
                  }
                  catch (Exception e)
                  {
                    var message = "メール件数とメールデータの数が一致していません。\n件数またはデータレコードをテキストエディタで修正してください。";
                    MessageBox.Show(message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    throw new MailLoadException(message, e);
                  }

                  // メールを取得する
                  for (int j = 0; j < n; j++)
                  {
                    // 送信メールのみ必要な項目
                    string address = reader.ReadLine();
                    string subject = reader.ReadLine();

                    // 予期せぬエラーの時にメッセージボックスに表示する件名
                    expSubject = subject;

                    // ヘッダを取得する
                    string header = "";
                    string hd = reader.ReadLine();

                    // 区切り文字が来るまで文字列を連結する
                    while (hd != "\x03")
                    {
                      header += hd + "\r\n";
                      hd = reader.ReadLine();
                    }

                    // 本文を取得する
                    string body = "";
                    string b = reader.ReadLine();

                    // エラー文字区切りの時対策
                    bool err_parse = false;

                    // 区切り文字が来るまで文字列を連結する
                    while (b != "\x03")
                    {
                      // 区切り文字が本文の後ろについてしまったとき
                      if (b.Contains("\x03") && b != "\x03")
                      {
                        // 区切り文字を取り除く
                        err_parse = true;
                        b = b.Replace("\x03", "");
                      }

                      body += b + "\r\n";

                      // 区切り文字が検出されたときは区切り文字を取り除いてループから抜ける
                      if (err_parse)
                      {
                        break;
                      }

                      b = reader.ReadLine();
                    }

                    // 受信・送信日時を取得する
                    string date = reader.ReadLine();

                    // メールサイズを取得する(送信メールは0byte扱い)
                    string size = reader.ReadLine();

                    // UIDLを取得する(送信メールは無視)
                    string uidl = reader.ReadLine();

                    // 添付ファイル名を取得する(受信メールは無視)
                    string attach = reader.ReadLine();

                    // 既読・未読フラグを取得する
                    bool notReadYet = (reader.ReadLine() == "True");

                    // CCのアドレスを取得する
                    string cc = reader.ReadLine();

                    // BCCを取得する(受信メールは無視)
                    string bcc = reader.ReadLine();

                    // 重要度を取得する
                    string priority = reader.ReadLine();

                    // 旧ファイルを読み込んでいるとき
                    if (priority != "urgent" && priority != "normal" && priority != "non-urgent")
                    {
                      var message = "Version 1.10以下のファイルを読み込もうとしています。\nメールデータ変換ツールで変換してから読み込んでください。";
                      MessageBox.Show(message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                      throw new MailLoadException(message);
                    }

                    // 変換フラグを取得する(旧バージョンからのデータ移行)
                    string convert = reader.ReadLine();

                    // ヘッダーがあった場合はそちらを優先する
                    if (header.Length > 0)
                    {
                      // ヘッダープロパティにファイルから取得したヘッダを格納する
                      pop.Header = header;

                      // アドレスを取得する
                      pop.GetDecodeHeaderField("From:");
                      address = pop.Field ?? address;

                      // 件名を取得する
                      pop.GetDecodeHeaderField("Subject:");
                      subject = pop.Field ?? subject;

                      // ヘッダからCCアドレスを取得する
                      pop.GetDecodeHeaderField("Cc:");
                      cc = pop.Field ?? cc;

                      // ヘッダから重要度を取得する
                      priority = Mail.ParsePriority(header);
                    }

                    // メール格納配列に格納する
                    var mail = new Mail(address, header, subject, body, attach, date, size, uidl, notReadYet, convert, cc, bcc, priority);
                    Add(mail, kind);
                  }
                }
              }
            }
          }
          catch (MailLoadException)
          {
            throw;
          }
          catch (Exception exp)
          {
            MessageBox.Show("予期しないエラーが発生しました。\n" + "件名:" + expSubject + "\n" + "エラー詳細 : \n" + exp.Message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            throw new MailLoadException("予期しないエラーが発生しました", exp);
          }
        }
      }
    }

    /// <summary>
    /// メールデータの保存
    /// </summary>
    public void MailDataSave()
    {
      var lockobj = new object();
      lock (lockobj)
      {
        try
        {
          // ファイルストリームをストリームライタに関連付ける
          using (var writer = new StreamWriter(Application.StartupPath + @"\Mail.dat", false, Encoding.UTF8))
          {
            var mailArray = new IList<Mail>[] { this[MailKind.Send], this[MailKind.Receive], this[MailKind.Delete] };
            // メールの件数とデータを書き込む
            foreach (var mails in mailArray)
            {
              writer.WriteLine(mails.Count);
              foreach (var mail in mails)
              {
                writer.WriteLine(mail.address);
                writer.WriteLine(mail.subject);
                writer.Write(mail.header);
                writer.WriteLine("\x03");
                writer.Write(mail.body);
                writer.WriteLine("\x03");
                writer.WriteLine(mail.date);
                writer.WriteLine(mail.size);
                writer.WriteLine(mail.uidl);
                writer.WriteLine(mail.attach);
                writer.WriteLine(mail.notReadYet.ToString());
                writer.WriteLine(mail.cc);
                writer.WriteLine(mail.bcc);
                writer.WriteLine(mail.priority);
                writer.WriteLine(mail.convert);
              }
            }
          }
        }
        catch (Exception exp)
        {
          MessageBox.Show("予期しないエラーが発生しました。\n" + exp.Message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
          throw new MailSaveException("予期しないエラーが発生しました。", exp);
        }
      }
    }
  }
}
