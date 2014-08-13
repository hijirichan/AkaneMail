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
    public class MailBox : IEnumerable<MailFolder>
    {
        private Dictionary<string, MailFolder> _folders = new Dictionary<string, MailFolder>();
        private MailFolder _send, _receive, _trash;

        public MailBox(MailFolder send, MailFolder receive, MailFolder trash)
        {
            _send = send;
            _receive = receive;
            _trash = trash;

            Add(_send);
            Add(_receive);
            Add(_trash);
        }

        public MailBox() : this(new MailFolder("Send", "送信メール"), new MailFolder("Receive", "受信メール"), new MailFolder("Trash", "ごみ箱")) { }

        /// <summary>
        /// 指定された名前の MailFolder を取得します。
        /// </summary>
        /// <param name="kind">取得する MailFolder の名前。</param>
        /// <returns>指定された名前の MailFolder。</returns>
        public MailFolder this[string folderName]
        {
            get
            {
                if (_folders.ContainsKey(folderName))
                    return _folders[folderName];
                throw new KeyNotFoundException("指定された MailFolder は存在しません。");
            }
        }

        public MailFolder Send { get { return _send; } }
        public MailFolder Receive { get { return _receive; } }
        public MailFolder Trash { get { return _trash; } }

        private void FolderNameChaged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            var folder = sender as MailFolder;
            if (e.PropertyName != "Name" || folder == null)
                return;
            if (_folders.ContainsKey(folder.Name))
                throw new ArgumentException("同名のフォルダが既に存在します。");

            // 名前が変更されたフォルダと参照が同じフォルダの名前を取り出して、それをリストに格納。
            var oldNames = _folders.Where(p => p.Value == folder).Select(p => p.Key);
            foreach (var oldName in oldNames) {
                _folders[oldName].PropertyChanged -= FolderNameChaged;
                _folders.Remove(oldName);
            }
            Add(folder);
        }

        /// <summary>
        /// 指定された名前の新しい MailFolder を追加します。
        /// </summary>
        /// <param name="folderName">新しい MailFolderの名前。</param>
        /// <exception cref="ArgumentNullException"><paramref name="folderName"/>が null です。</exception>
        /// <exception cref="ArgumentException">同じ名前の MailFolder が既に存在します。</exception>
        public void Add(string folderName)
        {
            if (folderName == null)
                throw new ArgumentNullException("folderNameがnullです。");
            if (_folders.ContainsKey(folderName))
                throw new ArgumentException("同じ名前のMailFolderが既に存在します。");

            var folder =  new MailFolder(folderName);
            _folders.Add(folder.Name, folder);

            folder.PropertyChanged += FolderNameChaged;
        }

        /// <summary>
        /// MailBox に指定された MailFolder を追加します。
        /// </summary>
        /// <param name="folder">追加する MailFolder。</param>
        /// <exception cref="ArgumentNullException"><paramref name="folder"/> が null です。</exception>
        /// <exception cref="ArgumentException">同じ名前の MailFolder が既に存在します。</exception>
        public void Add(MailFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException("folderがnullです。");
            if (_folders.ContainsKey(folder.Name))
                throw new ArgumentException("同じ名前のMailFolderが既に存在します。");
            _folders.Add(folder.Name, folder);

            folder.PropertyChanged += FolderNameChaged;
        }

        /// <summary>
        /// すべての MailFolder を反復処理する列挙子を返します。
        /// </summary>
        /// <returns>すべての MailFolder を反復処理する列挙子。</returns>
        public IEnumerator<MailFolder> GetEnumerator()
        {
            return _folders.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static object lockobj = new object();
        /// <summary>
        /// メールデータをファイルから読み込みます。
        /// </summary>
        public void MailDataLoad()
        {
            if (!File.Exists(Application.StartupPath + @"\Mail.dat")) return;

            // 予期せぬエラーの時にメールの本文が分かるようにするための変数
            var expSubject = "";

            // スレッドのロックをかける
            lock (lockobj) {
                try {
                    // ファイルストリームをストリームリーダに関連付ける
                    using (var reader = new StreamReader(Application.StartupPath + @"\Mail.dat", Encoding.UTF8)) {
                        var folders = new[] { _receive, _send, _trash };
                        // GetHederFieldとHeaderプロパティを使うためPop3クラスを作成する
                        using (var pop = new Pop3()) {
                            // データを読み出す
                            foreach (var folder in folders) {
                                var n = ReadMailCount(reader);

                                // メールを取得する
                                foreach (var _ in Enumerable.Range(0, n)) {
                                    // 送信メールのみ必要な項目
                                    string address = reader.ReadLine();
                                    string subject = reader.ReadLine();

                                    // 予期せぬエラーの時にメッセージボックスに表示する件名
                                    expSubject = subject;

                                    // ヘッダを取得する
                                    var header = ReadHeader(reader);
                                    // 本文を取得する
                                    var body = ReadBody(reader);
                                    // 受信・送信日時を取得する
                                    var date = reader.ReadLine();
                                    // メールサイズを取得する(送信メールは0byte扱い)
                                    var size = reader.ReadLine();
                                    // UIDLを取得する(送信メールは無視)
                                    var uidl = reader.ReadLine();
                                    // 添付ファイル名を取得する(受信メールは無視)
                                    var attach = reader.ReadLine();
                                    // 既読・未読フラグを取得する
                                    var notReadYet = (reader.ReadLine() == "True");
                                    // CCのアドレスを取得する
                                    var cc = reader.ReadLine();
                                    // BCCを取得する(受信メールは無視)
                                    var bcc = reader.ReadLine();
                                    // 重要度を取得する
                                    var priority = reader.ReadLine();
                                    // 旧ファイルを読み込んでいるとき
                                    if (priority != "urgent" && priority != "normal" && priority != "non-urgent") {
                                        var message = "Version 1.10以下のファイルを読み込もうとしています。\nメールデータ変換ツールで変換してから読み込んでください。";
                                        MessageBox.Show(message, "Akane Mail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                        throw new MailLoadException(message);
                                    }
                                    // 変換フラグを取得する(旧バージョンからのデータ移行)
                                    string convert = reader.ReadLine();

                                    // ヘッダーがあった場合はそちらを優先する
                                    if (header.Length > 0) {
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
                                        priority = MailPriority.Parse(header);
                                    }

                                    // メールをリストに追加
                                    var mail = new Mail(address, header, subject, body, attach, date, size, uidl, notReadYet, convert, cc, bcc, priority);
                                    folder.Add(mail);
                                }
                            }
                        }

                    }
                }
                catch (MailLoadException) {
                    throw;
                }
                catch (Exception exp) {
                    MessageBox.Show("予期しないエラーが発生しました。\n" + "件名:" + expSubject + "\n" + "エラー詳細 : \n" + exp.Message, "Akane Mail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    throw new MailLoadException("予期しないエラーが発生しました", exp);
                }

            }
        }

        private int ReadMailCount(StreamReader reader)
        {
            try {
                // メールの件数を読み出す
                return Int32.Parse(reader.ReadLine());
            }
            catch (Exception e) {
                var message = "メール件数とメールデータの数が一致していません。\n件数またはデータレコードをテキストエディタで修正してください。";
                MessageBox.Show(message, "Akane Mail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                throw new MailLoadException(message, e);
            }
        }

        private string ReadBody(StreamReader reader)
        {
            var body = new StringBuilder();
            var b = reader.ReadLine();
            
            while (b != "\x03") {
                // 区切り文字が検出されたときは区切り文字を取り除いてループから抜ける
                if (b.Contains("\x03")) {
                    body.Append(b.Replace("\x03", "") + "\r\n");
                    break;
                }

                body.Append(b + "\r\n");

                b = reader.ReadLine();
            }
            return body.ToString();
        }

        private string ReadHeader(StreamReader reader)
        {
            // ヘッダを取得する
            var header = new StringBuilder();
            string h = reader.ReadLine();

            // 区切り文字が来るまで文字列を連結する
            while (h != "\x03") {
                header.Append(h + "\r\n");
                h = reader.ReadLine();
            }
            return header.ToString();
        }

        /// <summary>
        /// メールデータの保存
        /// </summary>
        public void MailDataSave()
        {
            var lockobj = new object();
            lock (lockobj) {
                try {
                    // ファイルストリームをストリームライタに関連付ける
                    using (var writer = new StreamWriter(Application.StartupPath + @"\Mail.dat", false, Encoding.UTF8)) {
                        var folders = new [] { _receive, _send, _trash };
                        // メールの件数とデータを書き込む
                        foreach (var folder in folders) {
                            writer.WriteLine(folder.Count);
                            foreach (var mail in folder) {
                                writer.WriteLine(mail.Address);
                                writer.WriteLine(mail.Subject);
                                writer.Write(mail.Header);
                                writer.WriteLine("\x03");
                                writer.Write(mail.Body);
                                writer.WriteLine("\x03");
                                writer.WriteLine(mail.Date);
                                writer.WriteLine(mail.Size);
                                writer.WriteLine(mail.Uidl);
                                writer.WriteLine(mail.Attach);
                                writer.WriteLine(mail.NotReadYet.ToString());
                                writer.WriteLine(mail.Cc);
                                writer.WriteLine(mail.Bcc);
                                writer.WriteLine(mail.Priority);
                                writer.WriteLine(mail.Convert);
                            }
                        }
                    }
                }
                catch (Exception exp) {
                    MessageBox.Show("予期しないエラーが発生しました。\n" + exp.Message, "Akane Mail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    throw new MailSaveException("予期しないエラーが発生しました。", exp);
                }
            }
        }

        /// <summary>
        /// メールを削除します。
        /// </summary>
        public void MoveToTrash(string name, IEnumerable<ListViewItem> items)
        {
            var mails = (from item in items
                       let i = (int)item.Tag
                       select this[name][i]).ToList();
            foreach (var mail in mails) {
                Trash.Add(mail);
                this[name].Remove(mail);
            }
        }
        
        /// <summary>
        /// メールを完全に削除します。
        /// </summary>
        /// <param name="items"></param>
        public void TrashCompletely(IEnumerable<ListViewItem> items)
        {
            var mails = (from item in items
                        let i = (int)item.Tag
                        select Trash[i]).ToList();
            foreach (var mail in mails) {
                Trash.Remove(mail);
            }
        }

        public MailFolder GetSelectedMailFolder(string text)
        {
            switch (text) {
                case "差出人":
                    return Receive;
                case "宛先":
                    return Send;
                case "差出人または宛先":
                    return Trash;
                default:
                    return null;
            }
        }

    }
}
