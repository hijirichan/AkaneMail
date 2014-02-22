using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net.Mail;
using System.Media;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using nMail;
using ACryptLib;
using System.Threading.Tasks;

namespace AkaneMail
{
    public partial class MainForm : Form
    {
        // メールを格納する配列
        public List<Mail>[] collectionMail = new List<Mail>[3];

        #region "constants"
        // メールを識別する定数
        public const int RECEIVE = 0;   // 受信メール
        public const int SEND = 1;      // 送信メール
        public const int DELETE = 2;    // 削除メール
        #endregion

        // ListViewItemSorterに指定するフィールド
        public ListViewItemComparer listViewItemSorter;

        // 選択された行を格納するフィールド
        private int currentRow;

        #region "flags"
        // データ読み込みエラー発生のときのフラグ
        public bool errorFlag = false;

        // データ変更が発生したのときのフラグ
        public bool dataDirtyFlag = false;

        // 添付ファイルが存在するかのフラグ
        public bool attachMenuFlag = false;

        // 添付付きメールの返信フラグ
        public bool attachMailReplay = false;

        // 添付付きメールの返信用文字列
        public string attachMailBody = "";

        // 未読メールか既読メールかの確認フラグ
        public bool checkNotYetReadMail = false;

        // nMailの致命的なエラーの確認フラグ(現状は64bitOSの32bit版DLL読み込みエラーのみ)
        public bool nMailError = false;
        #endregion

        // 環境保存用のクラスインスタンス
        private initClass initMail;

        // 環境設定保存のためのクラス
        public class initClass
        {
            public string m_fromName;           // ユーザ(差出人)の名前 
            public string m_mailAddress;        // ユーザのメールアドレス
            public string m_userName;           // ユーザ名
            public string m_passWord;           // POP3のパスワード
            public string m_popServer;          // POP3サーバ名
            public string m_smtpServer;         // SMTPサーバ名
            public int m_popPortNo;             // POP3のポート番号
            public int m_smtpPortNo;            // SMTPのポート番号
            public bool m_apopFlag;             // APOPのフラグ
            public bool m_deleteMail;           // POP受信時メール削除フラグ
            public bool m_popBeforeSMTP;        // POP before SMTPフラグ
            public bool m_popOverSSL;           // POP3 over SSL/TLSフラグ
            public bool m_smtpAuth;             // SMTP認証フラグ
            public bool m_autoMailFlag;         // メール自動受信フラグ
            public int m_getMailInterval;       // メール受信間隔
            public bool m_popSoundFlag;         // メール着信音フラグ
            public string m_popSoundName;       // メール着信音ファイル名
            public bool m_bodyIEShow;           // HTMLメール表示フラグ
            public bool m_minimizeTaskTray;     // 最小化時のタスクトレイフラグ
            public int m_windowLeft;            // ウィンドウの左上のLeft座標
            public int m_windowTop;             // ウィンドウの左上のTop座標
            public int m_windowWidth;           // ウィンドウの幅
            public int m_windowHeight;          // ウィンドウの高さ
            public FormWindowState m_windowStat;    // ウィンドウの状態
        }

        // デリゲートの宣言
        delegate void ProgressMailInitDlg(int value);
        delegate void ProgressMailUpdateDlg(int value);
        delegate void ProgressMailDisableDlg();
        delegate void EnableButtonDlg(int flag);
        delegate void UpdateViewDlg(int flag);
        delegate void FlashWindowOnDlg();

        // 点滅用 Win32API のインポート
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
        public static void FlashWindow(System.Windows.Forms.Form window)
        {
            FlashWindow(window.Handle, false);
        }

        /// <summary>
        /// ListViewの項目の並び替えに使用するクラス
        /// </summary>
        public class ListViewItemComparer : System.Collections.IComparer
        {
            /// <summary>
            /// 比較する方法
            /// </summary>
            public enum ComparerMode
            {
                String,
                Integer,
                DateTime
            };

            private int _column;

            /// <summary>
            /// 並び替えるListView列の番号
            /// </summary>
            public int Column
            {
                set
                {
                    if (_column == value) {
                        if (Order == SortOrder.Ascending)
                            Order = SortOrder.Descending;
                        else if (Order == SortOrder.Descending)
                            Order = SortOrder.Ascending;
                    }
                    _column = value;
                }
                get
                {
                    return _column;
                }
            }

            /// <summary>
            /// 昇順か降順か
            /// </summary>
            public SortOrder Order { get; set; }

            /// <summary>
            /// 並び替えの方法
            /// </summary>
            public ComparerMode Mode { get; private set; }

            /// <summary>
            /// 列ごとの並び替えの方法
            /// </summary>
            public ComparerMode[] ColumnModes { get; set; }

            /// <summary>
            /// ListViewItemComparerクラスのコンストラクタ
            /// </summary>
            /// <param name="col">並び替える列番号</param>
            /// <param name="ord">昇順か降順か</param>
            /// <param name="cmod">並び替えの方法</param>
            public ListViewItemComparer(int col, SortOrder ord, ComparerMode cmod)
            {
                _column = col;
                Order = ord;
                Mode = cmod;
            }

            public ListViewItemComparer()
            {
                _column = 0;
                Order = SortOrder.Ascending;
                Mode = ComparerMode.String;
            }

            // xがyより小さいときはマイナスの数、大きいときはプラスの数、
            // 同じときは0を返す
            public int Compare(object x, object y)
            {
                int result = 0;

                // ListViewItemの取得
                ListViewItem itemx = (ListViewItem)x;
                ListViewItem itemy = (ListViewItem)y;

                //並べ替えの方法を決定
                if (ColumnModes != null && ColumnModes.Length > _column)
                    Mode = ColumnModes[_column];

                // 並び替えの方法別に、xとyを比較する
                switch (Mode) {
                    case ComparerMode.String:
                        result = string.Compare(itemx.SubItems[_column].Text,
                            itemy.SubItems[_column].Text);
                        break;
                    case ComparerMode.Integer:
                        result = int.Parse(itemx.SubItems[_column].Text) -
                            int.Parse(itemy.SubItems[_column].Text);
                        break;
                    case ComparerMode.DateTime:
                        result = DateTime.Compare(
                            DateTime.Parse(itemx.SubItems[_column].Text),
                            DateTime.Parse(itemy.SubItems[_column].Text));
                        break;
                }

                // 降順の時は結果を+-逆にする
                if (Order == SortOrder.Descending)
                    result = -result;
                else if (Order == SortOrder.None)
                    result = 0;

                // 結果を返す
                return result;
            }
        }

        public MainForm()
        {
            // はじめは最小化した状態にしておく
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            InitializeComponent();

            // Appliction.Idleを登録する
            Application.Idle += new EventHandler(Application_Idle);

            // 配列を作成する
            collectionMail[RECEIVE] = new List<Mail>();
            collectionMail[SEND] = new List<Mail>();
            collectionMail[DELETE] = new List<Mail>();
        }

        /// <summary>
        /// ツリービューの更新
        /// </summary>
        public void UpdateTreeView()
        {
            // メールの件数を設定する
            treeView1.Nodes[0].Nodes[0].Text = "受信メール (" + collectionMail[RECEIVE].Count + ")";
            treeView1.Nodes[0].Nodes[1].Text = "送信メール (" + collectionMail[SEND].Count + ")";
            treeView1.Nodes[0].Nodes[2].Text = "ごみ箱 (" + collectionMail[DELETE].Count + ")";
        }

        /// <summary>
        /// リストビューの更新
        /// </summary>
        public void UpdateListView()
        {
            List<Mail> list = null;

            // リストビューの描画を止める
            listView1.BeginUpdate();

            // リストビューの内容をクリアする
            listView1.Items.Clear();

            if (listView1.Columns[0].Text == "差出人") {
                // 受信メールの場合
                list = collectionMail[RECEIVE];
            }
            else if (listView1.Columns[0].Text == "宛先") {
                // 送信メールの場合
                list = collectionMail[SEND];
            }
            else if (listView1.Columns[0].Text == "差出人または宛先") {
                // 削除メールの場合
                list = collectionMail[DELETE];
            }
            else if (listView1.Columns[0].Text == "名前") {
                // メールボックスのとき
                ListViewItem item = new ListViewItem(Mail.fromName);
                item.SubItems.Add(Mail.mailAddress);
                if (File.Exists(Application.StartupPath + @"\Mail.dat")) {
                    string mailDataDate = File.GetLastWriteTime(Application.StartupPath + @"\Mail.dat").ToShortDateString() + " " + File.GetLastWriteTime(Application.StartupPath + @"\Mail.dat").ToLongTimeString();
                    FileInfo fi = new FileInfo(Application.StartupPath + @"\Mail.dat");
                    item.SubItems.Add(mailDataDate);
                    item.SubItems.Add(fi.Length.ToString());
                }
                else {
                    item.SubItems.Add("データ未作成");
                    item.SubItems.Add("0");
                }
                listView1.Items.Add(item);
                listView1.EndUpdate();
                return;
            }

            var items = list.Select((mail, index) =>
            {
                ListViewItem item = new ListViewItem(mail.address);
                if (mail.subject != "") {
                    item.SubItems.Add(mail.subject);
                }
                else {
                    item.SubItems.Add("(no subject)");
                }

                // メールの受信(送信)日時とメールサイズをリストのサブアイテムに追加する
                item.SubItems.Add(mail.date);
                item.SubItems.Add(mail.size);

                // 各項目のタグに要素の番号を格納する
                item.Tag = index;
                item.Name = index.ToString();

                // 未読(未送信)の場合は、フォントを太字にする
                if (mail.notReadYet) {
                    item.Font = new Font(this.Font, FontStyle.Bold);
                }

                // 重要度が高い場合は、フォントを太字にする
                if (mail.priority == "urgent") {
                    item.ForeColor = Color.Tomato;
                }
                else if (mail.priority == "non-urgent") {
                    item.ForeColor = Color.LightBlue;
                }
                return item;
            });
            listView1.Items.AddRange(items.ToArray());
            listView1.EndUpdate();
        }

        private static object lockobj = new object();

        /// <summary>
        /// メールデータの読み込み
        /// </summary>
        private void MailDataLoad()
        {
            // 予期せぬエラーの時にメールの本文が分かるようにするための変数
            string expSubject = "";
            int n = 0;

            // スレッドのロックをかける
            lock (lockobj) {
                if (File.Exists(Application.StartupPath + @"\Mail.dat")) {
                    try {
                        // ファイルストリームをストリームリーダに関連付ける
                        using (var reader = new StreamReader(Application.StartupPath + @"\Mail.dat", Encoding.UTF8)) {
                            // GetHederFieldとHeaderプロパティを使うためPop3クラスを作成する
                            using (var pop = new Pop3()) {
                                // データを読み出す
                                foreach (var mailList in collectionMail) {
                                    try {
                                        // メールの件数を読み出す
                                        n = Int32.Parse(reader.ReadLine());
                                    }
                                    catch (Exception) {
                                        // エラーフラグをtrueに変更する
                                        errorFlag = true;

                                        MessageBox.Show("メール件数とメールデータの数が一致していません。\n件数またはデータレコードをテキストエディタで修正してください。", "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                        return;
                                    }

                                    // メールを取得する
                                    for (int j = 0; j < n; j++) {
                                        // 送信メールのみ必要な項目
                                        string address = reader.ReadLine();
                                        string subject = reader.ReadLine();

                                        // 予期せぬエラーの時にメッセージボックスに表示する件名
                                        expSubject = subject;

                                        // ヘッダを取得する
                                        string header = "";
                                        string hd = reader.ReadLine();

                                        // 区切り文字が来るまで文字列を連結する
                                        while (hd != "\x03") {
                                            header += hd + "\r\n";
                                            hd = reader.ReadLine();
                                        }

                                        // 本文を取得する
                                        string body = "";
                                        string b = reader.ReadLine();

                                        // エラー文字区切りの時対策
                                        bool err_parse = false;

                                        // 区切り文字が来るまで文字列を連結する
                                        while (b != "\x03") {
                                            // 区切り文字が本文の後ろについてしまったとき
                                            if (b.Contains("\x03") && b != "\x03") {
                                                // 区切り文字を取り除く
                                                err_parse = true;
                                                b = b.Replace("\x03", "");
                                            }

                                            body += b + "\r\n";

                                            // 区切り文字が検出されたときは区切り文字を取り除いてループから抜ける
                                            if (err_parse) {
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
                                        if (priority != "urgent" && priority != "normal" && priority != "non-urgent") {

                                            // エラーフラグをtrueに変更する
                                            errorFlag = true;

                                            MessageBox.Show("Version 1.10以下のファイルを読み込もうとしています。\nメールデータ変換ツールで変換してから読み込んでください。", "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                            return;
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
                                            priority = Mail.ParsePriority(header);
                                        }

                                        // メール格納配列に格納する
                                        var mail = new Mail(address, header, subject, body, attach, date, size, uidl, notReadYet, convert, cc, bcc, priority);
                                        mailList.Add(mail);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exp) {
                        MessageBox.Show("予期しないエラーが発生しました。\n" + "件名:" + expSubject + "\n" + "エラー詳細 : \n" + exp.Message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        /// <summary>
        /// メールデータの保存
        /// </summary>
        private void MailDataSave()
        {
            lock (lockobj) {
                try {
                    // ファイルストリームをストリームライタに関連付ける
                    using (var writer = new StreamWriter(Application.StartupPath + @"\Mail.dat", false, Encoding.UTF8)) {
                        // メールの件数とデータを書き込む
                        foreach (var mails in collectionMail) {
                            writer.WriteLine(mails.Count);
                            foreach (var mail in mails) {
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
                catch (Exception exp) {
                    MessageBox.Show("予期しないエラーが発生しました。\n" + exp.Message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        /// <summary>
        /// メール送信・受信用プログレスバーの初期化
        /// </summary>
        private void ProgressMailInit(int value)
        {
            // プログレスバーを表示して最大値を未受信メール件数に設定する
            progressMail.Visible = true;
            progressMail.Minimum = 0;
            progressMail.Maximum = value;
        }

        /// <summary>
        /// メール送信・受信件数の更新
        /// </summary>
        private void ProgressMailUpdate(int value)
        {
            // メールの受信件数を更新する
            progressMail.Value = value;
        }

        /// <summary>
        /// メール送信・受信用プログレスバーの非表示
        /// </summary>
        private void ProgressMailDisable()
        {
            // プログレスバーを非表示にする
            progressMail.Visible = false;
            progressMail.Value = 0;
            progressMail.Minimum = 0;
            progressMail.Maximum = 0;
        }

        /// <summary>
        /// FlashWindow()の実行
        /// </summary>
        private void FlashWindowOn()
        {
            // 画面をフラッシュさせる
            FlashWindow(this.Handle, false);
        }

        /// <summary>
        /// 送受信メニューとツールボタンの更新
        /// </summary>
        private void EnableButton(int flag)
        {
            // メール受信のメニューとツールボタンを有効化する
            menuMailRecieve.Enabled = true;
            buttonMailRecieve.Enabled = true;

            // フラグの値が1のときメール送信のメニューとツールボタンを有効化する
            menuMailSend.Enabled = flag == 1;
            buttonMailSend.Enabled = flag == 1;
        }

        /// <summary>
        /// メール送受信後のTreeView、ListViewの更新
        /// </summary>
        private void UpdateView(int flag)
        {
            // フラグの値が0のとき
            if (flag == 0) {
                // テキストボックスを空値にする
                this.textBody.Text = "";
                // IEコンポが表示されているいとき

                if (this.browserBody.Visible) {
                    // IEコンポを閉じてテキストボックスを表示させる
                    this.browserBody.Visible = false;
                    this.textBody.Visible = true;
                }
            }

            // ListViewItemSorterを解除する
            listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // フラグの値が0のとき
            if (flag == 0) {
                // 受信or送信日時の降順で並べる
                listViewItemSorter.Column = 2;
                listViewItemSorter.Order = SortOrder.Descending;
            }

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;

            // フラグの値が0のとき
            if (flag == 0) {
                // 受信メールのとき
                if (listView1.Columns[0].Text == "差出人") {
                    // ListViewの1行目にフォーカスを当て直す
                    listView1.Items[0].Selected = true;
                    listView1.Items[0].Focused = true;
                    listView1.SelectedItems[0].EnsureVisible();
                    listView1.Select();
                    listView1.Focus();
                }
            }
        }

        /// <summary>
        /// 設定ファイルからアプリケーション設定を読み出す
        /// </summary>
        public void LoadSettings()
        {
            // 環境設定保存クラスを作成する
            initMail = new initClass();

            // アカウント情報(初期値)を設定する
            Mail.fromName = "";
            Mail.mailAddress = "";
            Mail.userName = "";
            Mail.passWord = "";
            Mail.smtpServer = "";
            Mail.popServer = "";
            Mail.popPortNumber = 110;
            Mail.smtpPortNumber = 25;
            Mail.apopFlag = false;
            Mail.deleteMail = false;
            Mail.popBeforeSMTP = false;
            Mail.popOverSSL = false;
            Mail.smtpAuth = false;
            Mail.autoMailFlag = false;
            Mail.getMailInterval = 10;
            Mail.popSoundName = "";
            Mail.bodyIEShow = false;
            Mail.minimizeTaskTray = false;

            // 環境設定ファイルが存在する場合は環境設定情報を読み込んでアカウント情報に設定する
            if (File.Exists(Application.StartupPath + @"\AkaneMail.xml")) {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(initClass));
                using (var fs = new FileStream(Application.StartupPath + @"\AkaneMail.xml", FileMode.Open)) {
                    initMail = (initClass)serializer.Deserialize(fs);
                }

                // アカウント情報
                Mail.fromName = initMail.m_fromName;
                Mail.mailAddress = initMail.m_mailAddress;
                Mail.userName = initMail.m_userName;
                Mail.passWord = Decrypt(initMail.m_passWord);

                // 接続情報
                Mail.smtpServer = initMail.m_smtpServer;
                Mail.popServer = initMail.m_popServer;
                Mail.smtpPortNumber = initMail.m_smtpPortNo;
                Mail.popPortNumber = initMail.m_popPortNo;
                Mail.apopFlag = initMail.m_apopFlag;
                Mail.deleteMail = initMail.m_deleteMail;
                Mail.popBeforeSMTP = initMail.m_popBeforeSMTP;
                Mail.popOverSSL = initMail.m_popOverSSL;
                Mail.smtpAuth = initMail.m_smtpAuth;

                // 自動受信設定
                Mail.autoMailFlag = initMail.m_autoMailFlag;
                Mail.getMailInterval = initMail.m_getMailInterval;

                // 通知設定
                Mail.popSoundFlag = initMail.m_popSoundFlag;
                Mail.popSoundName = initMail.m_popSoundName;
                Mail.bodyIEShow = initMail.m_bodyIEShow;
                Mail.minimizeTaskTray = initMail.m_minimizeTaskTray;

                // 画面の表示が通常のとき 
                if (initMail.m_windowStat == FormWindowState.Normal) {
                    // 過去のバージョンから環境設定ファイルを流用した初期起動以外はこの中に入る
                    if (initMail.m_windowLeft != 0 && initMail.m_windowTop != 0 && initMail.m_windowWidth != 0 && initMail.m_windowHeight != 0) {
                        this.Left = initMail.m_windowLeft;
                        this.Top = initMail.m_windowTop;
                        this.Width = initMail.m_windowWidth;
                        this.Height = initMail.m_windowHeight;
                    }
                }
                this.WindowState = initMail.m_windowStat;
            }
        }

        /// <summary>
        /// アプリケーション設定を設定ファイルに書き出す
        /// </summary>
        public void SaveSettings()
        {
            initMail = new initClass()
            {
                // アカウント情報
                m_fromName = Mail.fromName,
                m_mailAddress = Mail.mailAddress,
                m_userName = Mail.userName,
                m_passWord = Encrypt(Mail.passWord),

                // 接続情報
                m_smtpServer = Mail.smtpServer,
                m_popServer = Mail.popServer,
                m_smtpPortNo = Mail.smtpPortNumber,
                m_popPortNo = Mail.popPortNumber,
                m_apopFlag = Mail.apopFlag,
                m_deleteMail = Mail.deleteMail,
                m_popBeforeSMTP = Mail.popBeforeSMTP,
                m_popOverSSL = Mail.popOverSSL,
                m_smtpAuth = Mail.smtpAuth,

                // 自動受信設定
                m_autoMailFlag = Mail.autoMailFlag,
                m_getMailInterval = Mail.getMailInterval,

                // 通知設定
                m_popSoundFlag = Mail.popSoundFlag,
                m_popSoundName = Mail.popSoundName,
                m_bodyIEShow = Mail.bodyIEShow,
                m_minimizeTaskTray = Mail.minimizeTaskTray,

                // ウィンドウ設定
                m_windowLeft = this.Left,
                m_windowTop = this.Top,
                m_windowWidth = this.Width,
                m_windowHeight = this.Height,
                m_windowStat = this.WindowState
            };

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(initClass));

            using (var fs = new FileStream(Application.StartupPath + @"\AkaneMail.xml", FileMode.Create)) {
                serializer.Serialize(fs, initMail);
            }
        }

        /// <summary>文字列を暗号化します</summary>
        /// <remarks>例外発生時は旧バージョンと同じ動作をします</remarks>
        private string Encrypt(string password)
        {
            try {
                return ACrypt.EncryptPasswordString(password);
            }
            catch (Exception) {
                return password;
            }
        }

        /// <summary>文字列を複合化します</summary>
        /// <remarks>例外発生時は旧バージョンと同じ動作をします</remarks>
        private string Decrypt(string password)
        {
            try {
                return ACrypt.DecryptPasswordString(password);
            }
            catch (Exception) {
                return password;
            }
        }

        /// <summary>
        /// 指定されたメールを開く
        /// </summary>
        /// <param name="mail">メール</param>
        private void OpenMail(Mail mail)
        {
            Icon appIcon;
            bool htmlMail = false;
            bool base64Mail = false;

            // 添付ファイルメニューに登録されている要素を破棄する
            buttonAttachList.DropDownItems.Clear();
            buttonAttachList.Visible = false;
            attachMenuFlag = false;
            attachMailReplay = false;
            attachMailBody = "";

            // 添付ファイルクラスのインスタンスを作成する
            nMail.Attachment attach = new nMail.Attachment();

            // Contents-Typeがtext/htmlのメールか確認するフラグを取得する
            htmlMail = attach.GetHeaderField("Content-Type:", mail.header).Contains("text/html");

            // 未読メールの場合
            checkNotYetReadMail = mail.notReadYet;

            // 保存パスはプログラム直下に作成したtmpに設定する
            attach.Path = Application.StartupPath + @"\tmp";

            // 添付ファイルが存在するかを確認する
            int id = attach.GetId(mail.header);

            // 添付ファイルが存在する場合(存在しない場合は-1が返る)
            // もしくは HTML メールの場合
            if (id != nMail.Attachment.NoAttachmentFile || htmlMail) {
                try {
                    // 旧バージョンからの変換データではないとき
                    if (mail.convert == "") {
                        // HTML/Base64のデコードを有効にする
                        Options.EnableDecodeBody();
                    }
                    else {
                        // HTML/Base64のデコードを無効にする
                        Options.DisableDecodeBodyText();
                    }
                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(mail.header, mail.body);

                    // 添付ファイルを取り外す
                    attach.Save();
                }
                catch (Exception ex) {
                    // エラーメッセージを表示する
                    labelMessage.Text = String.Format("エラー メッセージ:{0:s}", ex.Message);
                    return;
                }

                // 添付返信フラグをtrueにする
                attachMailReplay = true;

                // IE コンポーネントを使用かつ HTML パートを保存したファイルがある場合
                if (Mail.bodyIEShow && attach.HtmlFile != "") {
                    // 本文表示用のテキストボックスの表示を非表示にしてHTML表示用のWebBrowserを表示する
                    this.textBody.Visible = false;
                    this.browserBody.Visible = true;

                    // Contents-Typeがtext/htmlでないとき(テキストとHTMLパートが存在する添付メール)
                    if (!htmlMail) {
                        // テキストパートを返信文に格納する
                        attachMailBody = attach.Body;
                    }
                    else {
                        // 本文にHTMLタグが直書きされているタイプのHTMLメールのとき
                        // 展開したHTMLファイルをストリーム読み込みしてテキストを返信用の変数に格納する
                        using (var sr = new StreamReader(Application.StartupPath + @"\tmp\" + attach.HtmlFile, Encoding.Default)) {
                            string htmlBody = sr.ReadToEnd();

                            // HTMLからタグを取り除いた本文を返信文に格納する
                            attachMailBody = Mail.HtmlToText(htmlBody, mail.header);
                        }
                    }

                    // 添付ファイル保存フォルダに展開されたHTMLファイルをWebBrowserで表示する
                    browserBody.AllowNavigation = true;
                    browserBody.Navigate(attach.Path + @"\" + attach.HtmlFile);
                }
                else {
                    // 添付ファイルを外した本文をテキストボックスに表示する
                    this.browserBody.Visible = false;
                    this.textBody.Visible = true;
                    // IE コンポーネントを使用せず、HTML メールで HTML パートを保存したファイルがある場合
                    if (htmlMail && !Mail.bodyIEShow && attach.HtmlFile != "") {
                        // 本文にHTMLタグが直書きされているタイプのHTMLメールのとき
                        // 展開したHTMLファイルをストリーム読み込みしてテキストボックスに表示する
                        using (var sr = new StreamReader(Application.StartupPath + @"\tmp\" + attach.HtmlFile, Encoding.Default)) {
                            string htmlBody = sr.ReadToEnd();

                            // HTMLからタグを取り除く
                            htmlBody = Mail.HtmlToText(htmlBody, mail.header);

                            attachMailBody = htmlBody;
                            this.textBody.Text = htmlBody;
                        }
                    }
                    else if (attach.Body != "") {
                        // デコードした本文の行末が\n\nではないとき
                        if (!attach.Body.Contains("\n\n")) {
                            attachMailBody = attach.Body;
                            this.textBody.Text = attach.Body;
                        }
                        else {
                            attach.Body.Replace("\n\n", "\r\n");
                            attachMailBody = attach.Body.Replace("\n", "\r\n");
                            this.textBody.Text = attachMailBody;
                        }
                    }
                    else {
                        this.textBody.Text = mail.body;
                    }
                }
                // 添付ファイルを外した本文が空値以外の場合
                // 添付ファイル名リストがnull以外のとき
                if (attach.FileNameList != null) {
                    // IE コンポーネントありで、添付ファイルが HTML パートを保存したファイルのみの場合はメニューを表示しない
                    if (!Mail.bodyIEShow || attach.HtmlFile == "" || attach.FileNameList.Length > 1) {
                        buttonAttachList.Visible = true;
                        attachMenuFlag = true;
                        // メニューに添付ファイルの名前を追加する
                        // IE コンポーネントありで、添付ファイルが HTML パートを保存したファイルはメニューに表示しない
                        foreach (var attachFile in attach.FileNameList.Where(a => a != attach.HtmlFile)) {
                            appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.StartupPath + @"\tmp\" + attachFile);
                            buttonAttachList.DropDownItems.Add(attachFile, appIcon.ToBitmap());
                        }
                    }
                }
            }
            else {
                // 添付ファイルが存在しない通常のメールまたは
                // 送信済みメールのときは本文をテキストボックスに表示する
                this.browserBody.Visible = false;
                this.textBody.Visible = true;

                // 添付ファイルリストが""でないとき
                if (mail.attach != "") {
                    buttonAttachList.Visible = true;

                    // 添付ファイルリストを分割して一覧にする
                    string[] attachFileNameList = mail.attach.Split(',');

                    for (int i = 0; i < attachFileNameList.Length; i++) {
                        var attachFile = attachFileNameList[i];
                        if (File.Exists(attachFile)) {
                            appIcon = System.Drawing.Icon.ExtractAssociatedIcon(attachFile);
                            buttonAttachList.DropDownItems.Add(attachFile, appIcon.ToBitmap());
                        }
                        else {
                            buttonAttachList.DropDownItems.Add(attachFile + "は削除されています。");
                            buttonAttachList.DropDownItems[i].Enabled = false;
                        }
                    }
                }

                // Contents-TypeがBase64のメールの場合
                base64Mail = attach.GetDecodeHeaderField("Content-Transfer-Encoding:", mail.header).Contains("base64");

                // Base64の文章が添付されている場合
                if (base64Mail) {
                    // 文章をデコードする
                    Options.EnableDecodeBody();

                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(mail.header, mail.body);

                    // 添付ファイルを取り外す
                    attach.Save();

                    if (!attach.Body.Contains("\n\n")) {
                        attachMailBody = attach.Body;
                        this.textBody.Text = attach.Body;
                    }
                    else {
                        attach.Body.Replace("\n\n", "\r\n");
                        attachMailBody = attach.Body.Replace("\n", "\r\n");
                        this.textBody.Text = attachMailBody;
                    }
                }
                else {
                    // ISO-2022-JPでかつquoted-printableがある場合(nMail.dllが対応するまでの暫定処理)
                    if (attach.GetHeaderField("Content-Type:", mail.header).ToLower().Contains("iso-2022-jp") && attach.GetHeaderField("Content-Transfer-Encoding:", mail.header).Contains("quoted-printable")) {
                        // 文章をデコードする
                        Options.EnableDecodeBody();

                        // ヘッダと本文付きの文字列を添付クラスに追加する
                        attach.Add(mail.header, mail.body);

                        // 添付ファイルを取り外す
                        attach.Save();

                        if (!attach.Body.Contains("\n\n")) {
                            attachMailBody = attach.Body;
                            this.textBody.Text = attach.Body;
                        }
                        else {
                            attach.Body.Replace("\n\n", "\r\n");
                            attachMailBody = attach.Body.Replace("\n", "\r\n");
                            this.textBody.Text = attachMailBody;
                        }
                    }
                    else if (attach.GetHeaderField("X-NMAIL-BODY-UTF8:", mail.header).Contains("8bit")) {
                        // Unicode化されたUTF-8文字列をデコードする
                        // 1件のメールサイズの大きさのbyte型配列を確保
                        var bs = mail.body.Select(c => (byte)c).ToArray();

                        // GetStringでバイト型配列をUTF-8の配列にエンコードする
                        attachMailBody = Encoding.UTF8.GetString(bs);
                        this.textBody.Text = attachMailBody;
                    }
                    else {
                        // テキストボックスに出力する文字コードをJISに変更する
                        byte[] b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.body);
                        string strBody = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                        // 本文をテキストとして表示する
                        this.textBody.Text = strBody;
                    }
                }
            }
        }

        /// <summary>
        /// 返信メールを作成
        /// </summary>
        /// <param name="mail"></param>
        private void CreateReturnMail(Mail mail)
        {
            MailEditorForm NewMailForm = new MailEditorForm();

            // 親フォームをForm1に設定する
            NewMailForm.MainForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.SendList = collectionMail[SEND];

            // 返信のための宛先・件名を設定する
            NewMailForm.textAddress.Text = mail.address;
            NewMailForm.textSubject.Text = "Re:" + mail.subject;

            // 添付なしメールのときはbodyを渡す
            if (!attachMailReplay) {
                // UTF-8でエンコードされたメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + attachMailBody;
                }
                else {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + mail.body;
                }
            }
            else {
                // 添付付きメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + attachMailBody;
                }
                else {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + mail.body;
                }
            }

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

        /// <summary>
        /// 転送メールを作成
        /// </summary>
        /// <param name="mail">メール</param>
        private void CreateFowerdMail(Mail mail)
        {
            string strFrom = "";
            string strTo = "";
            string strDate = "";
            string strSubject = "";

            Icon appIcon;
            MailEditorForm NewMailForm = new MailEditorForm();

            // 親フォームをForm1に設定する
            NewMailForm.MainForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.SendList = collectionMail[SEND];

            // 転送のために件名を設定する(件名は空白にする)
            NewMailForm.textAddress.Text = "";
            NewMailForm.textSubject.Text = "Fw:" + mail.subject;

            nMail.Attachment atch = new nMail.Attachment();

            // メールヘッダが存在するとき
            if (mail.header != "") {
                strFrom = atch.GetHeaderField("From:", mail.header);
                strTo = atch.GetHeaderField("To:", mail.header);
                strDate = atch.GetHeaderField("Date:", mail.header);
                strSubject = atch.GetHeaderField("Subject:", mail.header);
            }
            else {
                strFrom = Mail.mailAddress;
                strTo = mail.address;
                strDate = mail.date;
                strSubject = mail.subject;
            }

            string fwHeader = "----------------------- Original Message -----------------------\r\n";
            fwHeader = fwHeader + " From: " + strFrom + "\r\n To: " + strTo + "\r\n Date: " + strDate;
            fwHeader = fwHeader + "\r\n Subject:" + strSubject + "\r\n----\r\n\r\n";

            // 添付なしメールのときはbodyを渡す
            if (!attachMailReplay) {
                // NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + mail.body;
                // UTF-8でエンコードされたメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + attachMailBody;
                }
                else {
                    // JISコードなどのメールは通常のbodyを渡す
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + mail.body;
                }
            }
            else {
                // 添付付きメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + attachMailBody;
                }
                else {
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + mail.body;
                }
            }

            // 送信メールで添付ファイルがあるとき
            if (mail.attach != "") {
                // 添付リストメニューを表示
                NewMailForm.buttonAttachList.Visible = true;
                // 添付ファイルリストを分割して一覧にする
                NewMailForm.attachFileNameList = mail.attach.Split(',');
                // 添付ファイルの数だけメニューを追加する
                foreach (var attachFile in NewMailForm.attachFileNameList) {
                    appIcon = System.Drawing.Icon.ExtractAssociatedIcon(attachFile);
                    NewMailForm.buttonAttachList.DropDownItems.Add(attachFile, appIcon.ToBitmap());
                }
            }
            else if (this.buttonAttachList.Visible) {
                // 受信メールで添付ファイルがあるとき
                // 添付リストメニューを表示
                NewMailForm.buttonAttachList.Visible = true;

                // 添付ファイルの数だけメニューを追加する
                foreach (var attachFile in this.buttonAttachList.DropDownItems.Cast<ToolStripItem>().Select(i => i.Text)) {
                    // 添付ファイルが存在するかを確認してから添付する
                    if (File.Exists(Application.StartupPath + @"\tmp\" + attachFile)) {
                        appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.StartupPath + @"\tmp\" + attachFile);
                        NewMailForm.buttonAttachList.DropDownItems.Add(Application.StartupPath + @"\tmp\" + attachFile, appIcon.ToBitmap());
                    }
                }
            }

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

        /// <summary>
        /// メールの編集
        /// </summary>
        /// <param name="mail">メール</param>
        /// <param name="item">リストアイテム</param>
        private void EditMail(Mail mail, ListViewItem item)
        {
            Icon appIcon;

            // 1番目のカラムの表示が差出人か差出人または宛先のとき
            if (listView1.Columns[0].Text == "差出人" || listView1.Columns[0].Text == "差出人または宛先") {
                mail.notReadYet = false;

                ReforcusListView(listView1);

                // データ変更フラグをtrueにする
                dataDirtyFlag = true;
            }
            else if (listView1.Columns[0].Text == "宛先") {
                // 1番目のカラムが宛先のときは編集画面を表示する
                MailEditorForm EditMailForm = new MailEditorForm();

                // 親フォームをForm1に設定する
                EditMailForm.MainForm = this;

                // 親フォームにタイトルを設定する
                EditMailForm.Text = mail.subject + " - Ak@Ne!";

                // 送信箱の配列をForm3に渡す
                EditMailForm.SendList = collectionMail[SEND];
                EditMailForm.ListTag = (int)item.Tag;
                EditMailForm.IsEdit = true;

                // 宛先、件名、本文をForm3に渡す
                EditMailForm.textAddress.Text = mail.address;
                EditMailForm.textCc.Text = mail.cc;
                EditMailForm.textBcc.Text = mail.bcc;
                EditMailForm.textSubject.Text = mail.subject;
                EditMailForm.textBody.Text = mail.body;

                // 重要度をForm3に渡す
                if (mail.priority == "urgent") {
                    EditMailForm.comboPriority.SelectedIndex = 0;
                }
                else if (mail.priority == "normal") {
                    EditMailForm.comboPriority.SelectedIndex = 1;
                }
                else {
                    EditMailForm.comboPriority.SelectedIndex = 2;
                }

                // 添付ファイルが付いているとき
                if (mail.attach != "") {
                    // 添付リストメニューを表示
                    EditMailForm.buttonAttachList.Visible = true;
                    // 添付ファイルリストを分割して一覧にする
                    EditMailForm.attachFileNameList = mail.attach.Split(',');
                    // 添付ファイルの数だけメニューを追加する
                    foreach (var attachFile in EditMailForm.attachFileNameList) {
                        if (File.Exists(attachFile)) {
                            appIcon = System.Drawing.Icon.ExtractAssociatedIcon(attachFile);
                            EditMailForm.buttonAttachList.DropDownItems.Add(attachFile, appIcon.ToBitmap());
                        }
                        else {
                            EditMailForm.buttonAttachList.DropDownItems.Add(attachFile + "は削除されています。");
                        }
                    }
                }

                // メール編集フォームを表示する
                EditMailForm.Show();
            }
        }

        /// <summary>
        /// メールを削除
        /// </summary>
        private void DeleteMail()
        {
            int nSel = listView1.SelectedItems[0].Index;

            if (listView1.Columns[0].Text == "差出人") {
                // 受信メールのとき
                // 選択アイテムの数を取得
                int nLen = listView1.SelectedItems.Count;

                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                var nIndices = new int[nLen];

                nIndices = Enumerable.Range(0, nLen).Select(i => listView1.SelectedItems[i].Name).Select(t => int.Parse(t)).OrderBy(i => i).ToArray();

                // キーの並べ替え
                List<Mail> sList = collectionMail[RECEIVE];
                List<Mail> dList = collectionMail[DELETE];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        dList.Add(mail);
                        sList.Remove(mail);
                    }
                    nLen--;
                }
            }
            else if (listView1.Columns[0].Text == "宛先") {
                // 送信メールのとき
                // 選択アイテムの数を取得
                int nLen = listView1.SelectedItems.Count;

                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                var nIndices = new int[nLen];

                nIndices = Enumerable.Range(0, nLen).Select(i => listView1.SelectedItems[i].Name).Select(t => int.Parse(t)).OrderBy(i => i).ToArray();

                // キーの並べ替え
                List<Mail> sList = collectionMail[SEND];
                List<Mail> dList = collectionMail[DELETE];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        dList.Add(mail);
                        sList.Remove(mail);
                    }
                    nLen--;
                }
            }
            else if (listView1.Columns[0].Text == "差出人または宛先") {
                // 削除メールのとき
                if (MessageBox.Show("選択されたメールは完全に削除されます。\nよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                    // 選択アイテムの数を取得
                    int nLen = listView1.SelectedItems.Count;

                    if (nLen == 0)
                        return;

                    // 選択アイテムのキーを取得
                    var nIndices = Enumerable.Range(0, nLen).Select(n => int.Parse(listView1.SelectedItems[n].Name)).ToArray();

                    // キーの並べ替え
                    Array.Sort(nIndices);
                    List<Mail> dList = collectionMail[DELETE];

                    while (nLen > 0) {
                        // 選択アイテムのキーから 選択アイテム群の位置を取得
                        int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                        ListViewItem item = listView1.SelectedItems[nIndex];

                        // 元リストからメールアイテムを取得
                        Mail mail = dList[nIndices[nLen - 1]];

                        if (item.SubItems[1].Text == mail.subject) {
                            dList.Remove(mail);
                        }
                        nLen--;
                    }
                }
            }

            ClearInput();

            // リストが空になったのを判定するフラグ
            bool isListEmpty = false;

            // 選択しているリスト位置が現在のリスト件数以上のとき
            if (listView1.Items.Count <= nSel) {
                // 選択しているリスト位置が0でないとき
                if (nSel != 0) {
                    nSel = listView1.Items.Count - 1;
                }
                else {
                    isListEmpty = true;
                }
            }
            else {
                // 選択しているリスト位置が0でないとき
                if (nSel != 0)
                    nSel--;
            }

            // リストが空でないとき
            if (!isListEmpty) {
                // フォーカスをnSelの行に当てる
                listView1.Items[nSel].Selected = true;
                listView1.Items[nSel].Focused = true;
                listView1.SelectedItems[0].EnsureVisible();
                listView1.Select();
                listView1.Focus();
            }
            // データ変更フラグをtrueにする
            dataDirtyFlag = true;
        }

        /// <summary>
        /// POP3サーバからメールを受信する
        /// </summary>
        private void RecieveMail()
        {
            int mailCount = 0;              // 未受信メール件数

            ProgressMailInitDlg progressMailInit = ProgressMailInit;
            ProgressMailUpdateDlg progressMailUpdate = ProgressMailUpdate;
            UpdateViewDlg updateView = UpdateView;
            FlashWindowOnDlg flashWindow = FlashWindowOn;
            EnableButtonDlg enableButton = EnableButton;

            try {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール受信中";

                // POP3のセッションを作成する
                using (var pop = new nMail.Pop3()) {
                    // POP3への接続タイムアウト設定をする
                    Options.EnableConnectTimeout();

                    // APOPを使用するときに使うフラグ
                    pop.APop = Mail.apopFlag;

                    // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                    if (Mail.popOverSSL) {
                        pop.SSL = nMail.Pop3.SSL3;
                        pop.Connect(Mail.popServer, Mail.popPortNumber);
                    }
                    else {
                        // POP3へ接続する
                        pop.Connect(Mail.popServer, Mail.popPortNumber);
                    }

                    // POP3への認証処理を行う
                    pop.Authenticate(Mail.userName, Mail.passWord);


                    // 未受信のメールが何件あるかチェックする
                    var countMail = new Task<int>(() =>
                    {
                        var uidls = Enumerable.Range(1, pop.Count).Select(i => { pop.GetUidl(i); return pop.Uidl; });
                        var locals = collectionMail[RECEIVE].Union(collectionMail[DELETE]);
                        var unreadMails = from u in uidls
                                          join l in locals on u equals l.uidl
                                          select l;
                        return unreadMails.Count();
                    });
                    countMail.Start();

                    // POP3サーバ上に1件以上のメールが存在するとき
                    if (pop.Count > 0) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = pop.Count + "件のメッセージがサーバ上にあります。";
                    }
                    else {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "新着のメッセージはありませんでした。";

                        // メール受信のメニューとツールボタンを有効化する
                        Invoke(enableButton, 1);
                        return;
                    }

                    var receivedCount = countMail.Result;
                    // 受信済みメールカウントがPOP3サーバ上にあるメール件数と同じとき
                    if (receivedCount == pop.Count) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "新着のメッセージはありませんでした。";

                        // プログレスバーを非表示に戻す
                        Invoke(new ProgressMailDisableDlg(ProgressMailDisable));

                        // メール受信のメニューとツールボタンを有効化する
                        Invoke(enableButton, 1);

                        return;
                    }

                    // プログレスバーを表示して最大値を未受信メール件数に設定する
                    int mailCountMax = pop.Count - receivedCount;
                    Invoke(progressMailInit, mailCountMax);

                    // 未受信のメールを取得するためカウントを1増加させる
                    receivedCount++;

                    // 取得したメールをコレクションに追加する
                    for (int no = receivedCount; no <= pop.Count; no++) {
                        // 受信中件数を表示
                        labelMessage.Text = no + "件目のメールを受信しています。";

                        // メールのUIDLを取得する
                        pop.GetUidl(no);

                        // HTML/Base64のデコードを無効にする
                        Options.DisableDecodeBodyText();

                        // メールの情報を取得する
                        pop.GetMail(no);

                        // メールの情報を格納する
                        Mail mail = new Mail(pop.From, pop.Header, pop.Subject, pop.Body, pop.FileName, pop.DateString, pop.Size.ToString(), pop.Uidl, true, "", pop.GetDecodeHeaderField("Cc:"), "", Mail.ParsePriority(pop.Header));
                        collectionMail[RECEIVE].Add(mail);

                        // 受信メールの数を増加する
                        mailCount++;

                        // メール受信時にPOP3サーバ上のメール削除のチェックがある時はPOP3サーバからメールを削除する
                        if (Mail.deleteMail) {
                            pop.Delete(no);
                        }

                        // メールの受信件数を更新する
                        Invoke(progressMailUpdate, mailCount);

                        // スレッドを1秒間待機させる
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                // プログレスバーを非表示に戻す
                Invoke(new ProgressMailDisableDlg(ProgressMailDisable));

                // メール受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 1);

                // 未受信メールが1件以上の場合
                if (mailCount >= 1) {
                    // メール着信音の設定をしている場合
                    if (Mail.popSoundFlag && Mail.popSoundName != "") {
                        SoundPlayer sndPlay = new SoundPlayer(Mail.popSoundName);
                        sndPlay.Play();
                    }

                    // ウィンドウが最小化でタスクトレイに格納されていて何分間隔かで受信をするとき
                    if (this.WindowState == FormWindowState.Minimized && Mail.minimizeTaskTray && Mail.autoMailFlag) {
                        notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                        notifyIcon1.BalloonTipTitle = "新着メール";
                        notifyIcon1.BalloonTipText = mailCount + "件の新着メールを受信しました。";
                        notifyIcon1.ShowBalloonTip(300);
                    }
                    else {
                        // 画面をフラッシュさせる
                        Invoke(flashWindow);

                        // ステータスバーに状況表示する
                        labelMessage.Text = mailCount + "件の新着メールを受信しました。";
                    }

                    // データ変更フラグをtrueにする
                    dataDirtyFlag = true;
                }
                else {
                    // ステータスバーに状況表示する
                    labelMessage.Text = "新着のメッセージはありませんでした。";

                    // メール受信のメニューとツールボタンを有効化する
                    Invoke(enableButton, 1);

                    return;
                }
            }
            catch (nMail.nMailException nex) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                // メール受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 1);

                return;
            }
            catch (Exception exp) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーメッセージ:" + exp.Message;

                // メール受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 1);

                return;
            }

            // TreeViewとListViewの更新を行う
            Invoke(updateView, 0);

        }

        /// <summary>
        /// メールを送信する
        /// </summary>
        private void SendMail()
        {
            ProgressMailInitDlg progressMailInit = ProgressMailInit;
            ProgressMailUpdateDlg progressMailUpdate = ProgressMailUpdate;
            UpdateViewDlg updateView = UpdateView;
            EnableButtonDlg enableButton = EnableButton;

            int max_no = 0;
            int send_no = 0;

            // 送信可能なメールの数を確認する
            max_no = collectionMail[SEND].Count(m => m.notReadYet);

            // 送信可能なメールが存在しないとき
            if (max_no == 0) {
                // メール送信・受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 1);
                return;
            }

            try {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信中";

                // プログレスバーを表示して最大値を未送信メール件数に設定する
                Invoke(progressMailInit, max_no);

                // POP before SMTPが有効の場合
                if (Mail.popBeforeSMTP) {
                    try {
                        // POP3のセッションを作成する
                        using (var pop = new nMail.Pop3()) {
                            // POP3への接続タイムアウト設定をする
                            Options.EnableConnectTimeout();

                            // APOPを使用するときに使うフラグ
                            pop.APop = Mail.apopFlag;

                            // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                            if (Mail.popOverSSL) {
                                pop.SSL = nMail.Pop3.SSL3;
                                pop.Connect(Mail.popServer, Mail.popPortNumber);
                            }
                            else {
                                // POP3へ接続する
                                pop.Connect(Mail.popServer, Mail.popPortNumber);
                            }

                            // POP3への認証処理を行う
                            pop.Authenticate(Mail.userName, Mail.passWord);
                        }
                    }
                    catch (nMail.nMailException nex) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                        // メール送信・受信のメニューとツールボタンを有効化する
                        Invoke(enableButton, 1);

                        return;
                    }
                    catch (Exception exp) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "エラーメッセージ:" + exp.Message;

                        // メール送信・受信のメニューとツールボタンを有効化する
                        Invoke(enableButton, 1);

                        return;
                    }
                }

                // SMTPのセッションを作成する
                using (var smtp = new nMail.Smtp(Mail.smtpServer)) {
                    smtp.Port = Mail.smtpPortNumber;

                    // SMTP認証フラグが有効の時はSMTP認証を行う
                    if (Mail.smtpAuth) {
                        // SMTPサーバに接続
                        smtp.Connect();

                        // SMTP認証を行う
                        smtp.Authenticate(Mail.userName, Mail.passWord, Smtp.AuthPlain | Smtp.AuthCramMd5);
                    }

                    foreach (var mail in collectionMail[SEND]) {
                        if (mail.notReadYet) {
                            // CCが存在するとき
                            if (mail.cc != "") {
                                // CCの宛先を設定する
                                smtp.Cc = mail.cc;
                            }

                            // BCCが存在するとき
                            if (mail.bcc != "") {
                                // BCCの宛先を設定する
                                smtp.Bcc = mail.bcc;
                            }

                            // 添付ファイルを指定している場合
                            if (mail.attach != "") {
                                smtp.FileName = mail.attach;
                            }

                            // 追加ヘッダをつける
                            smtp.Header = "\r\nPriority: " + mail.priority + "\r\nX-Mailer: Akane Mail Version " + Application.ProductVersion;

                            // 差出人のアドレスを編集する
                            string fromAddress = Mail.FromAddress;

                            // 送信する
                            smtp.SendMail(mail.address, fromAddress, mail.subject, mail.body);

                            // 送信日時を設定する
                            mail.date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

                            // 送信済みに変更する
                            mail.notReadYet = false;

                            // メールの送信件数を更新する
                            send_no++;
                            Invoke(progressMailUpdate, send_no);

                            // スレッドを1秒間待機させる
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }

                // プログレスバーを非表示に戻す
                Invoke(new ProgressMailDisableDlg(ProgressMailDisable));

                // ボタンとメニューを有効化する
                Invoke(enableButton, 1);

                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信完了";
            }
            catch (nMail.nMailException nex) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                // メール送信・受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 1);

                return;
            }
            catch (Exception exp) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーメッセージ:" + exp.Message;

                // メール送信・受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 1);

                return;
            }

            // TreeViewとListViewの更新を行う
            Invoke(updateView, 1);

        }

        /// <summary>
        /// 直接メール送信
        /// </summary>
        /// <param name="address">宛先</param>
        /// <param name="cc">CCのアドレス</param>
        /// <param name="bcc">BCCのアドレス</param>
        /// <param name="subject">件名</param>
        /// <param name="body">本文</param>
        /// <param name="attach">添付メールリスト</param>
        /// <param name="priority">重要度</param>
        /// <returns>なし</returns>
        public void DirectSendMail(string address, string cc, string bcc, string subject, string body, string attach, string priority)
        {
            try {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信中";

                // POP before SMTPが有効の場合
                if (Mail.popBeforeSMTP) {
                    try {
                        // POP3のセッションを作成する
                        using (var pop = new Pop3()) {
                            // POP3への接続タイムアウト設定をする
                            Options.EnableConnectTimeout();

                            // APOPを使用するときに使うフラグ
                            pop.APop = Mail.apopFlag;

                            // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                            if (Mail.popOverSSL) {
                                pop.SSL = Pop3.SSL3;
                                pop.Connect(Mail.popServer, Mail.popPortNumber);
                            }
                            else {
                                // POP3へ接続する
                                pop.Connect(Mail.popServer, Mail.popPortNumber);
                            }

                            // POP3への認証処理を行う
                            pop.Authenticate(Mail.userName, Mail.passWord);
                        }
                    }
                    catch (nMail.nMailException nex) {
                        labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;
                        return;
                    }
                    catch (Exception exp) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "エラーメッセージ:" + exp.Message;
                        return;
                    }
                }

                // SMTPのセッションを作成する
                using (var smtp = new Smtp(Mail.smtpServer)) {
                    smtp.Port = Mail.smtpPortNumber;

                    // SMTP認証フラグが有効の時はSMTP認証を行う
                    if (Mail.smtpAuth) {
                        // SMTPサーバに接続
                        smtp.Connect();
                        // SMTP認証を行う
                        smtp.Authenticate(Mail.userName, Mail.passWord, Smtp.AuthPlain | Smtp.AuthCramMd5);
                    }

                    // CCが存在するとき
                    if (cc != "") {
                        // CCの宛先を設定する
                        smtp.Cc = cc;
                    }

                    // BCCが存在するとき
                    if (bcc != "") {
                        // BCCの宛先を設定する
                        smtp.Bcc = bcc;
                    }

                    // 添付ファイルを指定している場合
                    if (attach != "") {
                        smtp.FileName = attach;
                    }

                    // 追加ヘッダをつける
                    smtp.Header = "\r\nPriority: " + priority + "\r\nX-Mailer: Akane Mail Version " + Application.ProductVersion;

                    // 差出人のアドレスを編集する
                    string fromAddress = Mail.FromAddress;

                    // 送信する
                    smtp.SendMail(address, fromAddress, subject, body);
                }

                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信完了";
            }
            catch (nMail.nMailException nex) {
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;
                return;
            }
            catch (Exception exp) {
                labelMessage.Text = "エラーメッセージ:" + exp.Message;
                return;
            }
        }

        /// <summary>
        /// 添付ファイル付きメールの展開
        /// </summary>
        /// <param name="mail"></param>
        private void GetAttachMail(Mail mail)
        {
            // 添付ファイル保存対象フォルダを選択する
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                try {
                    // 添付ファイルクラスを作成する
                    nMail.Attachment attach = new nMail.Attachment();

                    // 保存パスを設定する
                    attach.Path = folderBrowserDialog1.SelectedPath;

                    // 添付ファイル展開用のテンポラリファイルを作成する
                    string tmpFileName = Path.GetTempFileName();
                    using (var writer = new StreamWriter(tmpFileName)) {
                        writer.Write(mail.header);
                        writer.Write("\r\n");
                        writer.Write(mail.body);
                    }

                    // テンポラリファイルを開いて添付ファイルを開く
                    using (var reader = new StreamReader(tmpFileName)) {
                        string header = reader.ReadToEnd();
                        // ヘッダと本文付きの文字列を添付クラスに追加する
                        attach.Add(header);
                    }
                    // 添付ファイルを保存する
                    attach.Save();

                    MessageBox.Show(attach.Path + "に添付ファイル" + attach.FileName + "を保存しました。", "添付ファイルの取り出し", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (nMailException nex) {
                    string message = "";
                    switch (nex.ErrorCode) {
                        case nMail.Attachment.ErrorFileOpen:
                            message = "添付ファイルがオープンできません。";
                            break;
                        case nMail.Attachment.ErrorInvalidNo:
                            message = "分割されたメールの順番が正しくないか、該当しないファイルが入っています。";
                            break;
                        case nMail.Attachment.ErrorPartial:
                            message = "分割されたメールが全て揃っていません";
                            break;
                        default:
                            break;
                    }
                    MessageBox.Show(message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                catch (Exception ex) {
                    MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        /// <summary>
        /// メールファイルの保存
        /// </summary>
        /// <param name="mail">メール</param>
        /// <param name="FileToSave">保存ファイル名</param>
        private void SaveMailFile(Mail mail, string FileToSave)
        {
            string fileBody = "";
            string fileHeader = "";

            // ヘッダから文字コードを取得する(添付付きは取得できない)
            string enc = Mail.ParseEncoding(mail.header);

            // 出力する文字コードがUTF-8ではないとき
            if (enc.ToLower().Contains("iso-") || enc.ToLower().Contains("shift_") || enc.ToLower().Contains("euc") || enc.ToLower().Contains("windows")) {
                // 出力するヘッダをUTF-8から各文字コードに変換する
                Byte[] b = Encoding.GetEncoding(enc).GetBytes(mail.header);
                fileHeader = Encoding.GetEncoding(enc).GetString(b);

                // 出力する本文をUTF-8から各文字コードに変換する
                b = Encoding.GetEncoding(enc).GetBytes(mail.body);
                fileBody = Encoding.GetEncoding(enc).GetString(b);
            }
            else if (enc.ToLower().Contains("utf-8") || mail.header.Contains("X-NMAIL-BODY-UTF8: 8bit")) {
                // text/plainまたはmultipart/alternativeでUTF-8でエンコードされたメールのとき
                // nMail.dllはUTF-8エンコードのメールを8bit単位に分解してUncode(16bit)扱いで格納している。
                // これはUnicodeで文字列を受け取る関数内で生のUTF-8の文字列を受け取っておかしなことに
                // なるのを防ぐための意図で行われている。
                // これをデコードするにはバイト型で格納し、UTF-8でデコードし直せば文字化けのような文字列を
                // 可読化することができる。

                // Byte型構造体に変換する
                var bs = mail.body.Select(s => (byte)s).ToArray();

                // GetStringでバイト型配列をUTF-8の配列にエンコードする
                fileBody = Encoding.UTF8.GetString(bs);

                // fileHeaderにヘッダを格納する
                fileHeader = mail.header;

                // ファイル出力フラグにUTF-8を設定する
                enc = "utf-8";
            }
            else {
                // ここに落ちてくるのは基本的に添付ファイルのみ
                Byte[] b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.header);
                fileHeader = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.body);
                fileBody = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                // 文字コードをJISに設定する
                enc = "iso-2022-jp";
            }

            // ファイル書き込み用のエンコーディングを取得する
            Encoding writeEnc = Encoding.GetEncoding(enc);

            using (var writer = new StreamWriter(FileToSave, false, writeEnc)) {
                // 受信メール(ヘッダが存在する)のとき
                if (mail.header.Length > 0) {
                    writer.Write(fileHeader);
                    writer.Write("\r\n");
                }
                else {
                    // 送信メールのときはヘッダの代わりに送り先と件名を出力
                    writer.WriteLine("To: " + mail.address);
                    writer.WriteLine("Subject: " + mail.subject);
                    writer.Write("\r\n");
                }
                writer.Write(fileBody);
            }
        }

        /// <summary>
        ///  選択されたメールを取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="columnText">選択されているカラムの文字列</param>
        /// <returns></returns>
        private Mail GetSelectedMail(object index, string columnText)
        {
            switch (columnText) {
                case "差出人":
                    return collectionMail[RECEIVE][(int)index];
                case "宛先":
                    return collectionMail[SEND][(int)index];
                case "差出人または宛先":
                    return collectionMail[DELETE][(int)index];
                default:
                    throw new ArgumentException(columnText + "は有効な値ではありません。");
            }
        }

        /// <summary>
        /// リストビューのフォーカスをリセットします。
        /// </summary>
        /// <param name="listView">対象のリストビュー</param>
        private void ReforcusListView(ListView listView)
        {
            // ListViewItemSorterを解除する
            listView.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;

            // フォーカスを当て直す
            listView.Items[currentRow].Selected = true;
            listView.SelectedItems[0].EnsureVisible();
            listView.Select();
            listView.Focus();
        }

        /// <summary>
        /// 入力欄をクリアします。
        /// </summary>
        private void ClearInput()
        {
            // IEコンポが表示されていないとき
            if (!this.browserBody.Visible) {
                // テキストボックスを空値にする
                this.textBody.Text = "";
            }
            else {
                // IEコンポを閉じてテキストボックスを表示させる
                this.browserBody.Visible = false;
                this.textBody.Text = "";
                this.textBody.Visible = true;
            }

            // 添付リストが表示されているとき
            if (buttonAttachList.Visible) {
                buttonAttachList.DropDownItems.Clear();
                buttonAttachList.Visible = false;
            }

            // ListViewItemSorterを解除する
            listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;
        }

        /// <summary>
        /// リストビューのカラム説明を設定
        /// </summary>
        /// <param name="col1">1カラム目の設定</param>
        /// <param name="col2">2カラム目の設定</param>
        /// <param name="col3">3カラム目の設定</param>
        /// <param name="col4">4カラム目の設定</param>
        private void SetListViewColumns(string col1, string col2, string col3, string col4)
        {
            listView1.Columns[0].Text = col1;
            listView1.Columns[1].Text = col2;
            listView1.Columns[2].Text = col3;
            listView1.Columns[3].Text = col4;
        }

        #region "Event Listeners"
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // ノードに付けたタグからリストビューのカラムを変更
            switch (e.Node.Tag.ToString()) {
                case "MailBoxRoot":
                    // メールボックスが選択された場合
                    SetListViewColumns("名前", "メールアドレス", "最終データ更新日", "データサイズ");
                    labelMessage.Text = "メールボックス";
                    listView1.ContextMenuStrip = null;
                    break;
                case "ReceiveMailBox":
                    // 受信メールが選択された場合
                    SetListViewColumns("差出人", "件名", "受信日時", "サイズ");
                    labelMessage.Text = "受信メール";
                    listView1.ContextMenuStrip = menuListView;
                    break;
                case "SendMailBox":
                    // 送信メールが選択された場合
                    SetListViewColumns("宛先", "件名", "送信日時", "サイズ"); ;
                    labelMessage.Text = "送信メール";
                    listView1.ContextMenuStrip = menuListView;
                    break;
                case "DeleteMailBox":
                    // ごみ箱が選択された場合
                    SetListViewColumns("差出人または宛先", "件名", "受信日時または送信日時", "サイズ");
                    labelMessage.Text = "ごみ箱";
                    listView1.ContextMenuStrip = menuListView;
                    break;
                default:
                    break;
            }

            // 画面設定をクリアする。
            ClearInput();
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuToolSetEnv_Click(object sender, EventArgs e)
        {
            SettingForm Form2 = new SettingForm();

            timer2.Enabled = false;

            DialogResult ret = Form2.ShowDialog();

            if (ret == DialogResult.OK) {
                if (Form2.checkAutoGetMail.Checked) {
                    timer2.Interval = Mail.getMailInterval * 60000;
                    timer2.Enabled = true;
                }
                else {
                    timer2.Enabled = false;
                }
            }
            // ListViewItemSorterを解除する
            listView1.ListViewItemSorter = null;

            // リストビューを更新する
            UpdateListView();

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // 現在の時刻を表示する
            labelDate.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
        }

        private void menuMailNew_Click(object sender, EventArgs e)
        {
            MailEditorForm NewMailForm = new MailEditorForm();

            // 親フォームをForm1に設定する
            NewMailForm.MainForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.SendList = collectionMail[SEND];

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

        private void menuMailSend_Click(object sender, EventArgs e)
        {
            // メール送信・受信のメニューとツールボタンを無効化する
            menuMailRecieve.Enabled = false;
            buttonMailRecieve.Enabled = false;
            buttonMailRecieve.Enabled = false;
            buttonMailSend.Enabled = false;

            // Threadオブジェクトを作成する
            System.Threading.Thread ts = new System.Threading.Thread(new System.Threading.ThreadStart(SendMail));

            // メール送信スレッドを開始する
            ts.Start();
        }

        private void menuMailRecieve_Click(object sender, EventArgs e)
        {
            // メール受信のメニューとツールボタンを無効化する
            menuMailRecieve.Enabled = false;
            buttonMailRecieve.Enabled = false;

            // Threadオブジェクトを作成する
            System.Threading.Thread tr = new System.Threading.Thread(new System.Threading.ThreadStart(RecieveMail));

            // メール受信スレッドを開始する
            tr.Start();
        }

        private void menuMailDelete_Click(object sender, EventArgs e)
        {
            // メールを削除
            DeleteMail();
        }

        private Dictionary<string, int> mailbox = new Dictionary<string, int> 
        {
            { "差出人", RECEIVE }, 
            { "宛先", SEND }, 
            { "差出人または宛先", DELETE } 
        };

        /// <summary>
        /// 未読メールを既読にする
        /// </summary>
        private void menuAlreadyRead_Click(object sender, EventArgs e)
        {
            var sList = collectionMail[mailbox[listView1.Columns[0].Text]];

            // 選択アイテムの数を取得
            int nLen = listView1.SelectedItems.Count;

            // 選択アイテムの数が0のとき
            if (nLen == 0)
                return;

            // 選択アイテムのキーを取得
            var nIndices = Enumerable.Range(0, nLen).Select(i => listView1.SelectedItems[i].Name).Select(t => int.Parse(t)).OrderBy(i => i).ToArray();

            while (nLen > 0) {
                nLen--;
                // 選択アイテムのキーから 選択アイテム群の位置を取得
                int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen].ToString());
                ListViewItem item = listView1.SelectedItems[nIndex];

                // 元リストからメールアイテムを取得
                Mail mail = sList[nIndices[nLen]];

                sList[nIndices[nLen]].notReadYet = !(item.SubItems[1].Text == mail.subject);
            }

            ReforcusListView(listView1);

            // データ変更フラグをtrueにする
            dataDirtyFlag = true;
        }

        /// <summary>
        /// 既読メールを未読にする
        /// </summary>
        private void menuNotReadYet_Click(object sender, EventArgs e)
        {
            var sList = collectionMail[mailbox[listView1.Columns[0].Text]];

            // 選択アイテムの数を取得
            int nLen = listView1.SelectedItems.Count;

            // 選択アイテムの数が0のとき
            if (nLen == 0)
                return;

            // 選択アイテムのキーを取得
            var nIndices = Enumerable.Range(0, nLen).Select(i => listView1.SelectedItems[i].Name).Select(t => int.Parse(t)).OrderBy(i => i).ToArray();

            while (nLen > 0) {
                nLen--;
                // 選択アイテムのキーから 選択アイテム群の位置を取得
                int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen].ToString());
                ListViewItem item = listView1.SelectedItems[nIndex];

                // 元リストからメールアイテムを取得
                Mail mail = sList[nIndices[nLen]];

                sList[nIndices[nLen]].notReadYet = item.SubItems[1].Text == mail.subject;
            }

            ReforcusListView(listView1);

            // データ変更フラグをtrueにする
            dataDirtyFlag = true;
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // 選択された行を取得する
            currentRow = e.ItemIndex;
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // メールボックスのときは反応しない
            if (listView1.Columns[0].Text == "名前") {
                return;
            }

            mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // メールの編集
            EditMail(mail, item);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ファイル展開用のテンポラリフォルダの削除
            if (Directory.Exists(Application.StartupPath + @"\tmp")) {
                Directory.Delete(Application.StartupPath + @"\tmp", true);
            }

            // データエラーフラグがfalseでデータ変更フラグがtrueのとき
            if (!errorFlag && dataDirtyFlag) {
                // データファイルを削除する
                if (File.Exists(Application.StartupPath + @"\Mail.dat")) {
                    File.Delete(Application.StartupPath + @"\Mail.dat");
                }

                // Threadオブジェクトを作成する
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(MailDataSave));

                // スレッドを開始する
                t.Start();

                // スレッドが終了するまで待機
                t.Join();
            }

            // 環境設定の書き出し
            SaveSettings();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // スプラッシュ・スクリーンの表示開始
            SplashScreen splash = new SplashScreen();
            splash.ProgressMsg = "メールクライアントの初期化中です";
            if (File.Exists(@"akanemail.png")) {
                try {
                    splash.BackgroundImage = Image.FromFile(@"akanemail.png");
                }
                catch {
                    // 読み込めないときは通常画像を表示するため処理なし
                }
            }
            splash.Show();
            splash.Refresh();

            // 最大化の時スプラッシュスクリーンよりも先にフォームが出ることがあるので
            // それを防ぐために一時的にフォームを非表示にする。
            this.Hide();

            // 環境設定の読み込み
            LoadSettings();

            try {
                // WinSockの初期化処理
                nMail.Winsock.Initialize();
            }
            catch (Exception exp) {
                // 64bit版OSで同梱の32bit版OS用のnMail.dllを使用して起動したときはエラーになるため差し替えのお願いメッセージを表示する
                if (exp.Message.Contains("間違ったフォーマットのプログラムを読み込もうとしました。")) {
                    MessageBox.Show("64bit版OSで32bit版OS用のnMail.dllを使用して実行した場合\nこのエラーが表示されます。\n\nお手数をお掛け致しますが同梱のnMail.dllをnMail.dll.32、nMail.dll.64をnMail.dllに名前を変更してAk@Ne!を起動\nしてください。", "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // 致命的なnMail.dllのエラーフラグをOn
                    nMailError = true;
                    Application.Exit();
                    return;
                }
            }

            // nMailのHTML添付ファイルの展開オプションをONにする
            Options.EnableSaveHtmlFile();

            // ファイル展開用のテンポラリフォルダの作成
            if (!Directory.Exists(Application.StartupPath + @"\tmp")) {
                Directory.CreateDirectory(Application.StartupPath + @"\tmp");
            }

            // Threadオブジェクトを作成する
            var t = new System.Threading.Thread(new System.Threading.ThreadStart(MailDataLoad));

            splash.ProgressMsg = "メールデータの読み込み作業中です";

            // スレッドを開始する
            t.Start();

            // スレッドが終了するまで待機
            t.Join();

            // メール自動受信が設定されている場合はタイマーを起動する
            if (Mail.autoMailFlag) {
                // 取得間隔*60000(60000ミリ秒=1分)をタイマー実行間隔に設定する
                timer2.Interval = Mail.getMailInterval * 60000;
                timer2.Enabled = true;
            }

            // ツリービューを展開する
            treeView1.ExpandAll();

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemComparerの作成と設定
            // 受信or送信日時の降順で並べる
            listViewItemSorter = new ListViewItemComparer() { Column = 2, Order = SortOrder.Descending };
            listViewItemSorter.ColumnModes = new ListViewItemComparer.ComparerMode[] { ListViewItemComparer.ComparerMode.String, ListViewItemComparer.ComparerMode.String, ListViewItemComparer.ComparerMode.DateTime, ListViewItemComparer.ComparerMode.Integer };

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;

            // スプラッシュ・スクリーンの表示終了
            splash.Close();
            if (!splash.IsDisposed)
                splash.Dispose();

            // 一時的に非表示にした画面を表示させる
            if (!(Mail.minimizeTaskTray && WindowState == FormWindowState.Minimized)) {
                ShowInTaskbar = true;
                this.Show();
            }
            // メインとなるフォームをアクティブに戻す
            this.Activate();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Appliction.Idleを削除する
            Application.Idle -= new EventHandler(Application_Idle);

            // 致命的なnMail.dllエラーがないとき
            if (!nMailError) {
                // WinSockを開放する
                nMail.Winsock.Done();
            }
        }

        private void menuMailReturnMail_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listView1.SelectedItems.Count == 0) {
                return;
            }

            // 表示機能はシンプルなものに変わる
            mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // 返信メールを作成する
            CreateReturnMail(mail);
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // メールボックスのときは反応しない
            if (listView1.Columns[0].Text == "名前") {
                return;
            }

            // リストビューで選択したメールデータを取得
            mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // メールを開く
            OpenMail(mail);
        }

        private void menuFileGetAttatch_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listView1.SelectedItems.Count == 0) {
                return;
            }

            // 送信メール以外も展開できるように変更
            mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // 添付ファイルを展開する
            GetAttachMail(mail);
        }

        private void menuSaveMailFile_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            // ListViewから選択した1行の情報をitemに格納する
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listView1.SelectedItems.Count == 0) {
                return;
            }

            // どの項目でも保存できるように変更
            mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // ファイル名にメールの件名を入れる
            saveFileDialog1.FileName = mail.subject;

            // 名前を付けて保存
            if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                if (saveFileDialog1.FileName != "") {
                    try {
                        SaveMailFile(mail, saveFileDialog1.FileName);
                    }
                    catch (Exception ex) {
                        MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private void menuFileClearTrush_Click(object sender, EventArgs e)
        {
            // ごみ箱の配列を空っぽにする
            if (MessageBox.Show("ごみ箱の中身をすべて削除します。\nよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                collectionMail[DELETE].Clear();

                ClearInput();

                // データ変更フラグをtrueにする
                dataDirtyFlag = true;
            }
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            // バージョン情報を表示する
            AboutForm AboutForm = new AboutForm();
            AboutForm.ShowDialog();
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];

            var mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // ファイルを開くかの確認をする
            if (MessageBox.Show(e.ClickedItem.Text + "を開きますか？\nファイルによってはウイルスの可能性もあるため\n注意してファイルを開いてください。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                // 受信されたメールのとき
                if (mail.attach.Length == 0) {
                    System.Diagnostics.Process.Start(Application.StartupPath + @"\tmp\" + e.ClickedItem.Text);
                }
                else {
                    // 送信メールのとき
                    System.Diagnostics.Process.Start(e.ClickedItem.Text);
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // メールボックスのときはソートしない
            if (listView1.Columns[0].Text == "名前")
                return;

            // クリックされた列を設定
            listViewItemSorter.Column = e.Column;

            // 並び替える
            listView1.Sort();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            menuMailRecieve_Click(sender, e);
        }

        private void browserBody_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            browserBody.AllowNavigation = false;
        }

        private void Form1_ClientSizeChanged(object sender, EventArgs e)
        {
            // 最小化時にタスクトレイに格納フラグがtrueでウィンドウが最小化されたとき
            if (this.WindowState == FormWindowState.Minimized && Mail.minimizeTaskTray) {
                // フォームが最小化の状態であればフォームを非表示にする   
                this.Hide();
                // トレイリストのアイコンを表示する   
                notifyIcon1.Visible = true;
            }
        }

        private void menuTaskRestoreWindow_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            // フォームを表示する   
            this.Visible = true;

            // 現在の状態が最小化の状態であれば通常の状態に戻す   
            if (this.WindowState == FormWindowState.Minimized) {
                this.WindowState = FormWindowState.Normal;
            }

            // フォームをアクティブにする   
            this.Activate();
        }

        private void menuFile_DropDownOpening(object sender, EventArgs e)
        {
            // メールの選択件が1かつメールボックスのとき
            var condition = listView1.SelectedItems.Count == 1 && listView1.Columns[0].Text != "名前";
            menuSaveMailFile.Enabled = condition;
            menuFileGetAttatch.Enabled = condition;
            menuFileGetAttatch.Enabled = condition && attachMenuFlag;

            // 削除メールが0件の場合
            menuFileClearTrush.Enabled = collectionMail[DELETE].Count != 0;
        }

        private void menuMail_DropDownOpening(object sender, EventArgs e)
        {

            if (listView1.Columns[0].Text == "名前") {
                menuMailDelete.Enabled = false;
                menuMailReturnMail.Enabled = false;
                menuMailFowerdMail.Enabled = false;
            }
            else {
                menuMailDelete.Enabled = listView1.SelectedItems.Count > 0;
                menuMailReturnMail.Enabled = listView1.SelectedItems.Count == 1;
                menuMailFowerdMail.Enabled = listView1.SelectedItems.Count == 1;
            }
        }

        private void menuListView_Opening(object sender, CancelEventArgs e)
        {
            // メールの選択件が1かつメールボックスのとき
            var condition = listView1.Columns[0].Text != "名前" && listView1.SelectedItems.Count == 1;
            menuReturnMail.Enabled = condition;
            menuFowerdMail.Enabled = condition;
            menuGetAttach.Enabled = condition && attachMenuFlag;

            // メールの選択数が0またはメールボックスのとき
            if (listView1.SelectedItems.Count == 0 || listView1.Columns[0].Text == "名前") {
                menuDelete.Enabled = false;
            }
            else if (listView1.SelectedItems.Count == 1) {
                // メールが1件選択されたとき
                menuDelete.Enabled = true;
            }

            // メールが既読で、メールボックス以外で何かが選択されているとき
            menuNotReadYet.Enabled = listView1.SelectedItems.Count > 0 && listView1.Columns[0].Text != "名前";
            menuAlreadyRead.Enabled = listView1.SelectedItems.Count > 0 && listView1.Columns[0].Text != "名前";

            // 送信メールを選択したとき
            if (listView1.Columns[0].Text == "宛先") {
                menuAlreadyRead.Text = "送信済にする(&K)";
                menuNotReadYet.Text = "未送信にする(&U)";
            }
            else {
                menuAlreadyRead.Text = "既読にする(&K)";
                menuNotReadYet.Text = "未読にする(&U)";
            }
        }

        private void menuTreeView_Opening(object sender, CancelEventArgs e)
        {
            menuClearTrush.Enabled = collectionMail[DELETE].Count != 0;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            // ボタンの有効状態を変更
            if (listView1.Columns[0].Text == "名前") {
                buttonMailDelete.Enabled = false;
                buttonReturnMail.Enabled = false;
                buttonForwardMail.Enabled = false;

            }
            else {
                buttonMailDelete.Enabled = listView1.SelectedItems.Count > 0;
                buttonReturnMail.Enabled = listView1.SelectedItems.Count == 1;
                buttonForwardMail.Enabled = listView1.SelectedItems.Count == 1;
            }

            // データ読み込みエラーを起こしたとき
            if (errorFlag) {
                this.Hide();
                this.Close();
            }
        }

        private void menuMailFowerdMail_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listView1.SelectedItems.Count == 0) {
                return;
            }

            // 表示機能はシンプルなものに変わる
            mail = GetSelectedMail(item.Tag, listView1.Columns[0].Text);

            // 転送メールの作成
            CreateFowerdMail(mail);
        }

        #endregion
    }
}