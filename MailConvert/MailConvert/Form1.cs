using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using nMail;

namespace MailConvert
{
    public partial class Form1 : Form
    {
        // メールを格納する配列
        public ArrayList[] collectionMail = new ArrayList[3];

        // メールを識別する定数
        public const int RECEIVE = 0;   // 受信メール
        public const int SEND = 1;      // 送信メール
        public const int DELETE = 2;    // 削除メール

        public Form1()
        {
            InitializeComponent();

            // 配列を作成する
            collectionMail[RECEIVE] = new ArrayList();
            collectionMail[SEND] = new ArrayList();
            collectionMail[DELETE] = new ArrayList();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            DialogResult ret;

            ret = MessageBox.Show("変換を開始します。\nよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            try {
                // 確認ではいを選択したとき
                if (ret == DialogResult.Yes) {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                        if (radioConvTypeA.Checked) {
                            DataConvertA(openFileDialog1.FileName);
                        }
                        else {
                            DataConvertB(openFileDialog1.FileName);
                        }
                    }
                }
            }
            catch (Exception exp) {
                if (exp.Message.Contains("nMail")) {
                    MessageBox.Show("nMail.dllが変換ツールと同じ場所に存在しません。\nnMail.dllを配置しているフォルダで実行してください。", "Ak@Ne! Mail Convertor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else if (exp.Message.Contains("間違ったフォーマットのプログラムを読み込もうとしました。")) {
                    MessageBox.Show("64bit版OSで32bit版OS用のnMail.dllを使用して実行した場合\nこのエラーが表示されます。\n\nお手数をお掛け致しますが同梱のnMail.dllをnMail.dll.32、nMail.dll.64をnMail.dllに名前を変更して実行\nしてください。", "Ak@Ne! Mail Convertor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else {
                    MessageBox.Show("予期せぬエラーが発生しました。\n" + exp.Message, "Ak@Ne! Mail Convertor", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        /// <summary>
        /// 重要度取得
        /// </summary>
        /// <param name="header">ヘッダ</param>
        /// <returns>重要度(urgent/normal/non-urgent)</returns>
        private string GetPriority(string header)
        {
            string _priority = "normal";
            string priority = "";

            nMail.Attachment attach = new nMail.Attachment();

            // ヘッダにX-Priorityがあるとき
            if (header.Contains("X-Priority:")) {
                priority = attach.GetHeaderField("X-Priority:", header);

                if (priority == "1" || priority == "2") {
                    _priority = "urgent";
                }
                else if (priority == "3") {
                    _priority = "normal";
                }
                else if (priority == "4" || priority == "5") {
                    _priority = "non-urgent";
                }
            }
            else if (header.Contains("X-MsMail-Priotiry:")) {
                priority = attach.GetHeaderField("X-MsMail-Priotiry:", header);

                if (priority.ToLower() == "High") {
                    _priority = "urgent";
                }
                else if (priority.ToLower() == "Normal") {
                    _priority = "normal";
                }
                else if (priority.ToLower() == "low") {
                    _priority = "non-urgent";
                }
            }
            else if (header.Contains("Importance:")) {
                priority = attach.GetHeaderField("Importance:", header);

                if (priority.ToLower() == "high") {
                    _priority = "urgent";
                }
                else if (priority.ToLower() == "normal") {
                    _priority = "normal";
                }
                else if (priority.ToLower() == "low") {
                    _priority = "non-urgent";
                }
            }
            else if (header.Contains("Priority:")) {
                _priority = attach.GetHeaderField("Priority:", header);
            }

            return _priority;
        }

        // メールデータの変換を行う(1.01→1.20)
        public int DataConvertA(string file_name)
        {
            // ファイルのバックアップ
            string file_name_bak = file_name.Substring(0, file_name.Length - 3) + "bak";

            try {
                File.Copy(file_name, file_name_bak);
            }
            catch (Exception) {
                MessageBox.Show(file_name_bak + "が既に存在しています。\n変換済みか何らかのエラーで作られた可能性があります。", "変換エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 0;
            }

            // ファイルストリームを作成する
            FileStream stream_r = new FileStream(file_name, FileMode.Open);

            // ファイルストリームをストリームリーダに関連付ける
            StreamReader reader = new StreamReader(stream_r, Encoding.Default);

            // GetHederFieldとHeaderプロパティを使うためPop3クラスを作成する
            nMail.Pop3 pop = new nMail.Pop3();

            // データを読み出す
            for (int i = 0; i < collectionMail.Length; i++) {
                // メールの件数を読み出す
                int n = Int32.Parse(reader.ReadLine());

                // メールを取得する
                for (int j = 0; j < n; j++) {
                    // 送信メールのみ必要な項目
                    string address = reader.ReadLine();

                    // アドレスがx-akane-convert-mailまたは空値のとき
                    if (address.Contains("x-akane-convert-mail") == true) {
                        reader.Close();
                        stream_r.Close();
                        File.Delete(file_name_bak);
                        MessageBox.Show("このメールデータは1.10のメールデータに変換済みです。\n1.10から1.20の形式に変換するを選択して実行してください。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return 0;
                    }

                    string subject = reader.ReadLine();

                    // ヘッダを取得する
                    string header = "";
                    string hd = reader.ReadLine();

                    // ヘッダがurgent、normal、non-urgentのとき
                    if (hd.Contains("urgent") == true || hd.Contains("normal") == true || hd.Contains("non-urgent") == true) {
                        reader.Close();
                        stream_r.Close();
                        File.Delete(file_name_bak);
                        MessageBox.Show("このメールデータは変換済みです。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return 0;
                    }

                    // 区切り文字が来るまで文字列を連結する
                    while (hd != "\x03") {
                        header = header + hd + "\r\n";
                        hd = reader.ReadLine();
                    }

                    // ヘッダのサイズが1バイト以上の場合
                    if (header.Length > 0) {
                        // ヘッダープロパティにファイルから取得したヘッダを格納する
                        pop.Header = header;

                        // アドレスを取得する
                        pop.GetHeaderField("From:");
                        if (pop.Field != null) {
                            address = pop.Field;
                        }

                        // 件名を取得する
                        pop.GetHeaderField("Subject:");
                        if (pop.Field != null) {
                            subject = pop.Field;
                        }
                    }

                    // 本文を取得する
                    string body = "";
                    string b = reader.ReadLine();

                    bool err_parse = false;

                    // 区切り文字が来るまで文字列を連結する
                    while (b != "\x03") {
                        // 区切り文字が本文の後ろについてしまったとき
                        if (b.Contains("\x03") && b != "\x03") {
                            err_parse = true;
                            b = b.Replace("\x03", "");
                        }

                        body = body + b + "\r\n";

                        // 区切り文字が検出されたときは区切り文字を取り除いてループから抜ける
                        if (err_parse == true) {
                            break;
                        }

                        b = reader.ReadLine();
                    }

                    // 受信・送信日時を取得する
                    string date = reader.ReadLine();

                    // sizeの値が空値(送信メール)のとき
                    if (date == "未送信") {
                        // 文字列に現在時刻を格納する
                        date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
                    }

                    // メールサイズを取得する(送信メールは0を格納する)
                    string size = reader.ReadLine();

                    // sizeの値が空値(送信メール)のとき
                    if (size == "") {
                        // 文字列で0を格納する
                        size = "0";
                    }

                    // UIDLを取得する(送信メールは無視)
                    string uidl = reader.ReadLine();

                    // 添付ファイル名を取得する(受信メールは無視)
                    string attach = reader.ReadLine();

                    // 既読・未読フラグを取得する
                    bool notReadYet = (reader.ReadLine() == "True");

                    // メール格納配列に格納する
                    Mail mail = new Mail(address, header, subject, body, attach, date, size, uidl, notReadYet, "x-akane-convert-mail");
                    collectionMail[i].Add(mail);
                }
            }

            // ストリームリーダとファイルストリームを閉じる
            reader.Close();
            stream_r.Close();

            // データファイルを削除する
            File.Delete(file_name);

            // ファイルストリームを作成する
            FileStream stream_w = new FileStream(file_name, FileMode.Create);

            // ファイルストリームをストリームライタに関連付ける
            StreamWriter writer = new StreamWriter(stream_w, Encoding.UTF8);

            // メールの件数とデータを書き込む
            for (int i = 0; i < collectionMail.Length; i++) {
                writer.WriteLine(collectionMail[i].Count.ToString());
                foreach (Mail mail in collectionMail[i]) {
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

                    // CCアドレスを取得する
                    string cc = "";

                    // ヘッダが存在するとき
                    if (mail.header.Length > 0) {
                        // ヘッダープロパティにファイルから取得したヘッダを格納する
                        pop.Header = mail.header;

                        // ヘッダからCCアドレスを取得する
                        pop.GetHeaderField("Cc:");
                        if (pop.Field != null) {
                            cc = pop.Field;
                        }
                    }

                    // CCアドレスを書き込む
                    writer.WriteLine(cc);

                    // BCCを設定する(受信メールは無視)
                    string bcc = "";

                    // CCアドレスを書き込む
                    writer.WriteLine(bcc);

                    // 重要度を取得する
                    string priority = "normal";

                    // ヘッダが存在するとき
                    if (mail.header.Length > 0) {
                        // ヘッダから重要度を取得する
                        priority = GetPriority(mail.header);
                    }

                    // 重要度を書き込む
                    writer.WriteLine(priority);

                    // コンバート済みのステータスを書き込む
                    writer.WriteLine(mail.convert);
                }
            }

            // ストリームライタとファイルストリームを閉じる
            writer.Close();
            stream_w.Close();

            MessageBox.Show("変換が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return 0;
        }

        // メールデータの変換を行う(1.10→1.20)
        public int DataConvertB(string file_name)
        {
            // ファイルのバックアップ
            string file_name_bak = file_name.Substring(0, file_name.Length - 3) + "bak";

            try {
                File.Copy(file_name, file_name_bak);
            }
            catch (Exception) {
                MessageBox.Show(file_name_bak + "が既に存在しています。\n変換済みか何らかのエラーで作られた可能性があります。", "変換エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 0;
            }

            // ファイルストリームを作成する
            FileStream stream_r = new FileStream(file_name, FileMode.Open);

            // ファイルストリームをストリームリーダに関連付ける
            StreamReader reader = new StreamReader(stream_r, Encoding.Default);

            // GetHederFieldとHeaderプロパティを使うためPop3クラスを作成する
            nMail.Pop3 pop = new nMail.Pop3();

            // データを読み出す
            for (int i = 0; i < collectionMail.Length; i++) {
                // メールの件数を読み出す
                int n = Int32.Parse(reader.ReadLine());

                // メールを取得する
                for (int j = 0; j < n; j++) {
                    // 送信メールのみ必要な項目
                    string address = reader.ReadLine();
                    string subject = reader.ReadLine();

                    // ヘッダがurgent、normal、non-urgentのとき
                    if (subject.Contains("urgent") == true || subject.Contains("normal") == true || subject.Contains("non-urgent") == true) {
                        reader.Close();
                        stream_r.Close();
                        File.Delete(file_name_bak);
                        MessageBox.Show("このメールデータは変換済みです。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return 0;
                    }

                    // ヘッダを取得する
                    string header = "";
                    string hd = reader.ReadLine();

                    // 区切り文字が来るまで文字列を連結する
                    while (hd != "\x03") {
                        header = header + hd + "\r\n";
                        hd = reader.ReadLine();
                    }

                    // ヘッダのサイズが1バイト以上の場合
                    if (header.Length > 0) {
                        // ヘッダープロパティにファイルから取得したヘッダを格納する
                        pop.Header = header;

                        // アドレスを取得する
                        pop.GetHeaderField("From:");
                        if (pop.Field != null) {
                            address = pop.Field;
                        }

                        // 件名を取得する
                        pop.GetHeaderField("Subject:");
                        if (pop.Field != null) {
                            subject = pop.Field;
                        }
                    }

                    // 本文を取得する
                    string body = "";
                    string b = reader.ReadLine();

                    bool err_parse = false;

                    // 区切り文字が来るまで文字列を連結する
                    while (b != "\x03") {
                        // 区切り文字が本文の後ろについてしまったとき
                        if (b.Contains("\x03") && b != "\x03") {
                            err_parse = true;
                            b = b.Replace("\x03", "");
                        }

                        body = body + b + "\r\n";

                        // 区切り文字が検出されたときは区切り文字を取り除いてループから抜ける
                        if (err_parse == true) {
                            break;
                        }

                        b = reader.ReadLine();
                    }

                    // 受信・送信日時を取得する
                    string date = reader.ReadLine();

                    // sizeの値が空値(送信メール)のとき
                    if (date == "未送信") {
                        // 文字列に現在時刻を格納する
                        date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
                    }

                    // メールサイズを取得する(送信メールは0を格納する)
                    string size = reader.ReadLine();

                    // sizeの値が空値(送信メール)のとき
                    if (size == "") {
                        // 文字列で0を格納する
                        size = "0";
                    }

                    // UIDLを取得する(送信メールは無視)
                    string uidl = reader.ReadLine();

                    // 添付ファイル名を取得する(受信メールは無視)
                    string attach = reader.ReadLine();

                    // 既読・未読フラグを取得する
                    bool notReadYet = (reader.ReadLine() == "True");

                    // コンバートフラグを取得する
                    string convert = reader.ReadLine();

                    // メール格納配列に格納する
                    Mail mail = new Mail(address, header, subject, body, attach, date, size, uidl, notReadYet, convert);
                    collectionMail[i].Add(mail);
                }
            }

            // ストリームリーダとファイルストリームを閉じる
            reader.Close();
            stream_r.Close();

            // データファイルを削除する
            File.Delete(file_name);

            // ファイルストリームを作成する
            FileStream stream_w = new FileStream(file_name, FileMode.Create);

            // ファイルストリームをストリームライタに関連付ける
            StreamWriter writer = new StreamWriter(stream_w, Encoding.UTF8);

            // メールの件数とデータを書き込む
            for (int i = 0; i < collectionMail.Length; i++) {
                writer.WriteLine(collectionMail[i].Count.ToString());
                foreach (Mail mail in collectionMail[i]) {
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

                    // CCアドレスを設定する
                    string cc = "";

                    // ヘッダが存在するとき
                    if (mail.header.Length > 0) {
                        // ヘッダープロパティにファイルから取得したヘッダを格納する
                        pop.Header = mail.header;

                        // ヘッダからCCアドレスを取得する
                        pop.GetHeaderField("Cc:");
                        if (pop.Field != null) {
                            cc = pop.Field;
                        }
                    }

                    // CCアドレスを書き込む
                    writer.WriteLine(cc);

                    // BCCを設定する(受信メールは無視)
                    string bcc = "";

                    // CCアドレスを書き込む
                    writer.WriteLine(bcc);

                    // 重要度を取得する
                    string priority = "normal";

                    // ヘッダが存在するとき
                    if (mail.header.Length > 0) {
                        // ヘッダから重要度を取得する
                        priority = GetPriority(mail.header);
                    }

                    // 重要度を書き込む
                    writer.WriteLine(priority);

                    // コンバート済みのステータスを書き込む
                    writer.WriteLine(mail.convert);
                }
            }

            // ストリームライタとファイルストリームを閉じる
            writer.Close();
            stream_w.Close();

            MessageBox.Show("変換が完了しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return 0;
        }
    }
}