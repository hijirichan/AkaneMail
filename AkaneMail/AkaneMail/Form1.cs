using System;
using System.Collections.Generic;
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

namespace AkaneMail
{
    public partial class Form1 : Form
    {
        // メールを格納する配列
        public List<Mail>[] collectionMail = new List<Mail>[3];

        // メールを識別する定数
        public const int RECEIVE = 0;   // 受信メール
        public const int SEND = 1;      // 送信メール
        public const int DELETE = 2;    // 削除メール

        // ListViewItemSorterに指定するフィールド
        public ListViewItemComparer listViewItemSorter;

        // 選択された行を格納するフィールド
        private int currentRow;

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
            private SortOrder _order;
            private ComparerMode _mode;
            private ComparerMode[] _columnModes;

            /// <summary>
            /// 並び替えるListView列の番号
            /// </summary>
            public int Column
            {
                set
                {
                    if (_column == value) {
                        if (_order == SortOrder.Ascending)
                            _order = SortOrder.Descending;
                        else if (_order == SortOrder.Descending)
                            _order = SortOrder.Ascending;
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
            public SortOrder Order
            {
                set { _order = value; }
                get { return _order; }
            }

            /// <summary>
            /// 並び替えの方法
            /// </summary>
            public ComparerMode Mode
            {
                set { _mode = value; }
                get { return _mode; }
            }

            /// <summary>
            /// 列ごとの並び替えの方法
            /// </summary>
            public ComparerMode[] ColumnModes
            {
                set { _columnModes = value; }
            }

            /// <summary>
            /// ListViewItemComparerクラスのコンストラクタ
            /// </summary>
            /// <param name="col">並び替える列番号</param>
            /// <param name="ord">昇順か降順か</param>
            /// <param name="cmod">並び替えの方法</param>
            public ListViewItemComparer(int col, SortOrder ord, ComparerMode cmod)
            {
                _column = col;
                _order = ord;
                _mode = cmod;
            }

            public ListViewItemComparer()
            {
                _column = 0;
                _order = SortOrder.Ascending;
                _mode = ComparerMode.String;
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
                if (_columnModes != null && _columnModes.Length > _column)
                    _mode = _columnModes[_column];

                // 並び替えの方法別に、xとyを比較する
                switch (_mode) {
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
                if (_order == SortOrder.Descending)
                    result = -result;
                else if (_order == SortOrder.None)
                    result = 0;

                // 結果を返す
                return result;
            }
        }

        public Form1()
        {
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
            int i = 0;

            // リストビューの描画を止める
            listView1.BeginUpdate();

            // リストビューの内容をクリアする
            listView1.Items.Clear();

            if(listView1.Columns[0].Text == "差出人"){
                // 受信メールの場合
                list = collectionMail[RECEIVE];
            }
            else if(listView1.Columns[0].Text == "宛先"){
                // 送信メールの場合
                list = collectionMail[SEND];
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                // 削除メールの場合
                list = collectionMail[DELETE];
            }
            else if(listView1.Columns[0].Text == "名前"){
                // メールボックスのとき
                ListViewItem item = new ListViewItem(Mail.fromName);
                item.SubItems.Add(Mail.mailAddress);
                if(File.Exists(Application.StartupPath + @"\Mail.dat")){
                    string mailDataDate = File.GetLastWriteTime(Application.StartupPath + @"\Mail.dat").ToShortDateString() + " " + File.GetLastWriteTime(Application.StartupPath + @"\Mail.dat").ToLongTimeString();
                    FileInfo fi = new FileInfo(Application.StartupPath + @"\Mail.dat");
                    item.SubItems.Add(mailDataDate);
                    item.SubItems.Add(fi.Length.ToString());
                }
                else{
                    item.SubItems.Add("データ未作成");
                    item.SubItems.Add("0");
                }
                listView1.Items.Add(item);
                listView1.EndUpdate();
                return;
            }

            foreach (Mail mail in list){
                ListViewItem item = new ListViewItem(mail.address);
                if(mail.subject != ""){
                    item.SubItems.Add(mail.subject);
                }
                else{
                    item.SubItems.Add("(no subject)");
                }
                item.SubItems.Add(mail.date);
                item.SubItems.Add(mail.size);

                // 各項目のタグに要素の番号を格納する
                item.Tag = i;
                item.Name = i.ToString();

                // 未読(未送信)の場合は、フォントを太字にする
                if(mail.notReadYet == true){
                    item.Font = new Font(this.Font, FontStyle.Bold);
                }

                // 重要度が高い場合は、フォントを太字にする
                if(mail.priority == "urgent"){
                    item.ForeColor = Color.Tomato;
                }
                else if(mail.priority == "non-urgent"){
                    item.ForeColor = Color.LightBlue;
                }

                i++;

                listView1.Items.Add(item);
            }
            listView1.EndUpdate();
        }

        /// <summary>
        /// メールデータの読み込み
        /// </summary>
        private void MailDataLoad()
        {
            // 予期せぬエラーの時にメールの本文が分かるようにするための変数
            string expSubject = "";
            int n = 0;

            // スレッドのロックをかける
            System.Threading.Monitor.Enter(this);

            if(File.Exists(Application.StartupPath + @"\Mail.dat") == true){
                try {
                    // ファイルストリームを作成する
                    FileStream stream = new FileStream(Application.StartupPath + @"\Mail.dat", FileMode.Open);

                    // ファイルストリームをストリームリーダに関連付ける
                    // StreamReader reader = new StreamReader(stream, Encoding.Default);
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);

                    // GetHederFieldとHeaderプロパティを使うためPop3クラスを作成する
                    nMail.Pop3 pop = new nMail.Pop3();

                    // データを読み出す
                    for (int i = 0; i < collectionMail.Length; i++) {
                        try{
                            // メールの件数を読み出す
                            n = Int32.Parse(reader.ReadLine());
                        }
                        catch(Exception){
                            // 中身を編集したときに起こりうるエラー
                            // ストリームリーダとファイルストリームを閉じる
                            reader.Close();
                            stream.Close();

                            // エラーフラグをtrueに変更する
                            errorFlag = true;

                            MessageBox.Show("メール件数とメールデータの数が一致していません。\n件数またはデータレコードをテキストエディタで修正してください。", "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                            // スレッドのロックを解放
                            System.Threading.Monitor.Exit(this);

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
                                header = header + hd + "\r\n";
                                hd = reader.ReadLine();
                            }

                            // ヘッダのサイズが1バイト以上の場合
                            if (header.Length > 0) {
                                // ヘッダープロパティにファイルから取得したヘッダを格納する
                                pop.Header = header;

                                // アドレスを取得する
                                //pop.GetHeaderField("From:");
                                pop.GetDecodeHeaderField("From:");
                                if (pop.Field != null) {
                                    address = pop.Field;
                                }

                                // 件名を取得する
                                //pop.GetHeaderField("Subject:");
                                pop.GetDecodeHeaderField("Subject:");
                                if (pop.Field != null) {
                                    subject = pop.Field;
                                }
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

                                body = body + b + "\r\n";

                                // 区切り文字が検出されたときは区切り文字を取り除いてループから抜ける
                                if (err_parse == true) {
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

                            // ヘッダが存在するとき
                            if(header.Length > 0){
                                // ヘッダープロパティにファイルから取得したヘッダを格納する
                                pop.Header = header;

                                // ヘッダからCCアドレスを取得する
                                //pop.GetHeaderField("Cc:");
                                pop.GetDecodeHeaderField("Cc:");
                                if (pop.Field != null) {
                                    cc = pop.Field;
                                }
                            }

                            // BCCを取得する(受信メールは無視)
                            string bcc = reader.ReadLine();

                            // 重要度を取得する
                            string priority = reader.ReadLine();

                            // 旧ファイルを読み込んでいるとき
                            if (priority != "urgent" && priority != "normal" && priority != "non-urgent") {
                                // ストリームリーダとファイルストリームを閉じる
                                reader.Close();
                                stream.Close();

                                // エラーフラグをtrueに変更する
                                errorFlag = true;

                                MessageBox.Show("Version 1.10以下のファイルを読み込もうとしています。\nメールデータ変換ツールで変換してから読み込んでください。", "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                                // スレッドのロックを解放
                                System.Threading.Monitor.Exit(this);

                                return;
                            }

                            // ヘッダが存在するとき
                            if(header.Length > 0){
                                // ヘッダから重要度を取得する
                                priority = GetPriority(header);
                            }

                            // 変換フラグを取得する(旧バージョンからのデータ移行)
                            string convert = reader.ReadLine();

                            // メール格納配列に格納する
                            Mail mail = new Mail(address, header, subject, body, attach, date, size, uidl, notReadYet, convert, cc, bcc, priority);
                            collectionMail[i].Add(mail);
                        }
                    }

                    // ストリームリーダとファイルストリームを閉じる
                    reader.Close();
                    stream.Close();

                    // nMail.Pop3を解放
                    pop.Dispose();
                }
                catch (Exception exp) {
                    MessageBox.Show("予期しないエラーが発生しました。\n" + "件名:" + expSubject + "\n" + "エラー詳細 : \n" + exp.Message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }

            // スレッドのロックを解放
            System.Threading.Monitor.Exit(this);
        }

        /// <summary>
        /// メールデータの保存
        /// </summary>
        private void MailDataSave()
        {
            // スレッドのロックをかける
            System.Threading.Monitor.Enter(this);

            try {
                // ファイルストリームを作成する
                FileStream stream = new FileStream(Application.StartupPath + @"\Mail.dat", FileMode.Create);

                // ファイルストリームをストリームライタに関連付ける
                // StreamWriter writer = new StreamWriter(stream, Encoding.Default);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);

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
                        writer.WriteLine(mail.cc);
                        writer.WriteLine(mail.bcc);
                        writer.WriteLine(mail.priority);
                        writer.WriteLine(mail.convert);
                    }
                }

                // ストリームライタとファイルストリームを閉じる
                writer.Close();
                stream.Close();
            }
            catch (Exception exp) {
                MessageBox.Show("予期しないエラーが発生しました。\n" + exp.Message, "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            // スレッドのロックを解放
            System.Threading.Monitor.Exit(this);
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

            // フラグの値が1のとき
            if(flag == 1){
                // メール送信のメニューとツールボタンを有効化する
                menuMailSend.Enabled = true;
                buttonMailSend.Enabled = true;
            }
        }

        /// <summary>
        /// メール送受信後のTreeView、ListViewの更新
        /// </summary>
        private void UpdateView(int flag)
        {
            // フラグの値が0のとき
            if(flag == 0){
                // IEコンポが表示されていないとき
                if(this.browserBody.Visible == false){
                    // テキストボックスを空値にする
                    this.textBody.Text = "";
                }
                else{
                    // IEコンポを閉じてテキストボックスを表示させる
                    this.browserBody.Visible = false;
                    this.textBody.Text = "";
                    this.textBody.Visible = true;
                }
            }

            // ListViewItemSorterを解除する
            listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // フラグの値が0のとき
            if(flag == 0){
                // 受信or送信日時の降順で並べる
                listViewItemSorter.Column = 2;
                listViewItemSorter.Order = SortOrder.Descending;
            }

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;

            // フラグの値が0のとき
            if(flag == 0){
                // 受信メールのとき
                if(listView1.Columns[0].Text == "差出人"){
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
        /// POP3サーバからメールを受信する
        /// </summary>
        private void RecieveMail()
        {
            int receivedCount = 0;          // 受信済みメール件数
            int mailCount = 0;              // 未受信メール件数

            ProgressMailInitDlg progressMailInit = new ProgressMailInitDlg(ProgressMailInit);
            ProgressMailUpdateDlg progressMailUpdate = new ProgressMailUpdateDlg(ProgressMailUpdate);
            UpdateViewDlg updateView = new UpdateViewDlg(UpdateView);
            FlashWindowOnDlg flashWindow = new FlashWindowOnDlg(FlashWindowOn);
            EnableButtonDlg enableButton = new EnableButtonDlg(EnableButton);

            try {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール受信中";

                // POP3のセッションを作成する
                nMail.Pop3 pop = new nMail.Pop3();

                // POP3への接続タイムアウト設定をする
                nMail.Options.EnableConnectTimeout();

                // APOPを使用するときに使うフラグ
                pop.APop = Mail.apopFlag;

                // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                if(Mail.popOverSSL == true){
                    pop.SSL = nMail.Pop3.SSL3;
                    pop.Connect(Mail.popServer, nMail.Pop3.StandardSslPortNo);
                }else{
                    // POP3へ接続する
                    pop.Connect(Mail.popServer, Mail.popPortNumber);
                }

                // POP3への認証処理を行う
                pop.Authenticate(Mail.userName, Mail.passWord);

                // POP3サーバ上に1件以上のメールが存在するとき
                if (pop.Count >= 1) {
                    // ステータスバーに状況表示する
                    labelMessage.Text = pop.Count + "件のメッセージがサーバ上にあります。";
                } else {
                    // ステータスバーに状況表示する
                    labelMessage.Text = "新着のメッセージはありませんでした。";

                    // POP3から切断する
                    pop.Close();

                    // メール受信のメニューとツールボタンを有効化する
                    Invoke(enableButton, 0);

                    return;
                }

                // 未受信のメールが何件あるかチェックする
                for (int no = 1; no <= pop.Count; no++) {
                    // メールのUIDLを取得する
                    pop.GetUidl(no);

                    // UIDLを文字列として格納
                    string uidl = pop.Uidl;

                    // 受信メールの配列に該当のUIDLがないかチェックする
                    for (int i = 0; i < collectionMail[RECEIVE].Count; i++) {
                        // UIDLが受信済みメールの配列に存在した場合
                        if (uidl == ((Mail)collectionMail[RECEIVE][i]).uidl) {
                            // 受信済みメールカウントを1増加させる
                            receivedCount++;
                        }
                    }

                    // ゴミ箱に削除されたメールの配列に該当のUIDLがないかチェックする
                    for (int i = 0; i < collectionMail[DELETE].Count; i++) {
                        // UIDLがゴミ箱に削除されたメールの配列に存在した場合
                        if (uidl == ((Mail)collectionMail[DELETE][i]).uidl) {
                            // 受信済みメールカウントを1増加させる
                            receivedCount++;
                        }
                    }
                }

                // 受信済みメールカウントがPOP3サーバ上にあるメール件数と同じとき
                if (receivedCount == pop.Count) {
                    // ステータスバーに状況表示する
                    labelMessage.Text = "新着のメッセージはありませんでした。";

                    // POP3から切断する
                    pop.Close();

                    // プログレスバーを非表示に戻す
                    Invoke(new ProgressMailDisableDlg(ProgressMailDisable));

                    // メール受信のメニューとツールボタンを有効化する
                    Invoke(enableButton, 0);

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

                    // ヘッダのデコードを無効にする(実験)
                    //nMail.Options.DisableDecodeHeader();

                    // HTML/Base64のデコードを無効にする
                    nMail.Options.DisableDecodeBodyText();

                    // メールの情報を取得する
                    pop.GetMail(no);

                    // メールの情報を格納する
                    //Mail mail = new Mail(pop.From, pop.Header, pop.Subject, pop.Body, pop.FileName, pop.DateString, pop.Size.ToString(), pop.Uidl, true, "", pop.GetHeaderField("Cc:"), "", GetPriority(pop.Header));
                    Mail mail = new Mail(pop.From, pop.Header, pop.Subject, pop.Body, pop.FileName, pop.DateString, pop.Size.ToString(), pop.Uidl, true, "", pop.GetDecodeHeaderField("Cc:"), "", GetPriority(pop.Header));
                    collectionMail[RECEIVE].Add(mail);

                    // 受信メールの数を増加する
                    mailCount++;

                    // メール受信時にPOP3サーバ上のメール削除のチェックがある時はPOP3サーバからメールを削除する
                    if (Mail.deleteMail == true) {
                        pop.Delete(no);
                    }

                    // メールの受信件数を更新する
                    Invoke(progressMailUpdate, mailCount);

                    // スレッドを1秒間待機させる
                    System.Threading.Thread.Sleep(1000);
                }

                // プログレスバーを非表示に戻す
                Invoke(new ProgressMailDisableDlg(ProgressMailDisable));

                // POP3から切断する
                pop.Close();

                // メール受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 0);

                // 未受信メールが1件以上の場合
                if (mailCount >= 1) {
                    // メール着信音の設定をしている場合
                    if (Mail.popSoundFlag == true && Mail.popSoundName != "") {
                        SoundPlayer sndPlay = new SoundPlayer(Mail.popSoundName);
                        sndPlay.Play();
                    }

                    // ウィンドウが最小化でタスクトレイに格納されていて何分間隔かで受信をするとき
                    if (this.WindowState == FormWindowState.Minimized && Mail.minimizeTaskTray == true && Mail.autoMailFlag == true) {
                        notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                        notifyIcon1.BalloonTipTitle = "新着メール";
                        notifyIcon1.BalloonTipText = mailCount + "件の新着メールを受信しました。";
                        notifyIcon1.ShowBalloonTip(300);
                    } else {
                        // 画面をフラッシュさせる
                        Invoke(flashWindow);

                        // ステータスバーに状況表示する
                        labelMessage.Text = mailCount + "件の新着メールを受信しました。";
                    }

                    // データ変更フラグをtrueにする
                    dataDirtyFlag = true;
                } else {
                    // ステータスバーに状況表示する
                    labelMessage.Text = "新着のメッセージはありませんでした。";

                    // メール受信のメニューとツールボタンを有効化する
                    Invoke(enableButton, 0);

                    return;
                }
            }
            catch (nMail.nMailException nex) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                // メール受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 0);

                return;
            }
            catch (Exception exp) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーメッセージ:" + exp.Message;

                // メール受信のメニューとツールボタンを有効化する
                Invoke(enableButton, 0);

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
            ProgressMailInitDlg progressMailInit = new ProgressMailInitDlg(ProgressMailInit);
            ProgressMailUpdateDlg progressMailUpdate = new ProgressMailUpdateDlg(ProgressMailUpdate);
            UpdateViewDlg updateView = new UpdateViewDlg(UpdateView);
            EnableButtonDlg enableButton = new EnableButtonDlg(EnableButton);

            int max_no = 0;
            int send_no = 0;

            // 送信可能なメールの数を確認する
            foreach (Mail mail in collectionMail[SEND]) {
                if (mail.notReadYet == true) {
                    max_no++;
                }
            }

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
                if (Mail.popBeforeSMTP == true) {
                    try {
                        // POP3のセッションを作成する
                        nMail.Pop3 pop = new nMail.Pop3();

                        // POP3への接続タイムアウト設定をする
                        nMail.Options.EnableConnectTimeout();

                        // APOPを使用するときに使うフラグ
                        pop.APop = Mail.apopFlag;

                        // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                        if(Mail.popOverSSL == true){
                            pop.SSL = nMail.Pop3.SSL3;
                            pop.Connect(Mail.popServer, nMail.Pop3.StandardSslPortNo);
                        }else{
                            // POP3へ接続する
                            pop.Connect(Mail.popServer, Mail.popPortNumber);
                        }

                        // POP3への認証処理を行う
                        pop.Authenticate(Mail.userName, Mail.passWord);

                        // 何もせずに切断する
                        pop.Close();
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
                nMail.Smtp smtp = new nMail.Smtp(Mail.smtpServer);
                smtp.Port = Mail.smtpPortNumber;

                // SMTP認証フラグが有効の時はSMTP認証を行う
                if(Mail.smtpAuth == true){
                    // SMTPサーバに接続
                    smtp.Connect();
                    // SMTP認証を行う
                    smtp.Authenticate(Mail.userName, Mail.passWord, nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
                }

                foreach (Mail mail in collectionMail[SEND]) {
                    if (mail.notReadYet == true) {
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
                        smtp.Header = "\r\nPriority: " + mail.priority + "\r\nX-Mailer: Akane 32bit Windows Mailer Version " + Application.ProductVersion;

                        // 差出人のアドレスを編集する
                        string fromAddress = Mail.fromName + " <" + Mail.mailAddress + ">";

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

                // SMTPから切断する
                smtp.Close();

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
        /// 文字コードを取得する
        /// </summary>
        /// <param name="mailHeader">メールヘッダ</param>
        /// <returns>文字コード</returns>
        public string GetEncodingString(string mailHeader)
        {
            nMail.Attachment attach = new nMail.Attachment();

            // メールヘッダから文字コード文字列を抜き出す
            string codeName = attach.GetHeaderField("Content-Type:", mailHeader);

            codeName = codeName.Replace("\"","");
            string[] arrayName = codeName.Split('=');
            codeName = arrayName[1];

            return codeName;
        }

        /// <summary>
        /// HTMLからタグを取り除く
        /// </summary>
        /// <param name="htmlBody">HTML本文</param>
        /// <param name="mailHeader">メールヘッダ</param>
        /// <returns>タグが取り除かれた文字列</returns>
        public string HtmlToText(string htmlBody, string mailHeader)
        {
            string codeName;        // 文字コード名
            string bodyCodeName;    // HTMLの文字コード名   

            // メールヘッダの文字コードを取得する
            codeName = GetEncodingString(mailHeader);

            // metaタグから正規表現で文字コードを取り出す
            Regex regEnc = new Regex("<meta.*?charset=(?<encode>.*?)\".*?>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // regEncにマッチする文字列を検索
            Match m = regEnc.Match(htmlBody);

            // HTML本文の中でcharset=文字コード名がマッチしたとき
            if (m.Success == true) {
                // HTMLの文字コード名を取得する
                bodyCodeName = m.Groups["encode"].Value;
                // メールヘッダの文字コードとHTMLの文字コードが同じとき
                if (codeName.ToLower() == bodyCodeName.ToLower()) {
                    // HTMLの文字コードで変換する
                    Byte[] b = Encoding.GetEncoding(bodyCodeName).GetBytes(htmlBody);
                    htmlBody = Encoding.GetEncoding(bodyCodeName).GetString(b);
                } else {
                    // 文字コードが異なる場合はメールヘッダの文字コードで変換する
                    Byte[] b = Encoding.GetEncoding(codeName).GetBytes(htmlBody);
                    htmlBody = Encoding.GetEncoding(codeName).GetString(b);
                }
            } else{
                // htmlBody内にcharset指定が存在しないときは
                // メールヘッダの文字コードで変換する
                Byte[] b = Encoding.GetEncoding(codeName).GetBytes(htmlBody);
                htmlBody = Encoding.GetEncoding(codeName).GetString(b);
            }

            // 正規表現の設定(<script>, <noscript>)
            Regex re1 = new Regex("<(no)?script.*?script>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 正規表現の設定(<style>)
            Regex re2 = new Regex("<style.*?style>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            // 正規表現の設定(すべてのタグ)
            Regex re3 = new Regex("<.*?>", RegexOptions.Singleline);

            // タグを取り除く
            htmlBody = re1.Replace(htmlBody, "");
            htmlBody = re2.Replace(htmlBody, "");
            htmlBody = re3.Replace(htmlBody, "");

            // 変換できなかった特殊文字を個別置換
            htmlBody = htmlBody.Replace("&nbsp;", " ");
            htmlBody = htmlBody.Replace("&shy;", " ");
            htmlBody = htmlBody.Replace("&lt;", "<");
            htmlBody = htmlBody.Replace("&gt;", ">");
            htmlBody = htmlBody.Replace("&amp;", "&");
            htmlBody = htmlBody.Replace("&quot;", "\"");
            htmlBody = htmlBody.Replace("&copy;", "(c)");
            htmlBody = htmlBody.Replace("&reg;", "(R)");
            htmlBody = htmlBody.Replace("&trade;", "TM");
            htmlBody = htmlBody.Replace("\r\n\r\n\r\n\r\n", "");

            return htmlBody;
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
            try
            {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信中";

                // POP before SMTPが有効の場合
                if(Mail.popBeforeSMTP == true){
                    try {
                        // POP3のセッションを作成する
                        nMail.Pop3 pop = new nMail.Pop3();

                        // POP3への接続タイムアウト設定をする
                        nMail.Options.EnableConnectTimeout();

                        // APOPを使用するときに使うフラグ
                        pop.APop = Mail.apopFlag;

                        // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                        if(Mail.popOverSSL == true){
                            pop.SSL = nMail.Pop3.SSL3;
                            pop.Connect(Mail.popServer, nMail.Pop3.StandardSslPortNo);
                        }else{
                            // POP3へ接続する
                            pop.Connect(Mail.popServer, Mail.popPortNumber);
                        }

                        // POP3への認証処理を行う
                        pop.Authenticate(Mail.userName, Mail.passWord);

                        // 何もせずに切断する
                        pop.Close();
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
                nMail.Smtp smtp = new nMail.Smtp(Mail.smtpServer);
                smtp.Port = Mail.smtpPortNumber;

                // SMTP認証フラグが有効の時はSMTP認証を行う
                if(Mail.smtpAuth == true){
                    // SMTPサーバに接続
                    smtp.Connect();
                    // SMTP認証を行う
                    smtp.Authenticate(Mail.userName, Mail.passWord, nMail.Smtp.AuthPlain | nMail.Smtp.AuthCramMd5);
                }

                // CCが存在するとき
                if(cc != ""){
                    // CCの宛先を設定する
                    smtp.Cc = cc;
                }

                // BCCが存在するとき
                if(bcc != ""){
                    // BCCの宛先を設定する
                    smtp.Bcc = bcc;
                }

                // 添付ファイルを指定している場合
                if(attach != ""){
                    smtp.FileName = attach;
                }

                // 追加ヘッダをつける
                smtp.Header = "\r\nPriority: " + priority + "\r\nX-Mailer: Akane 32bit Windows Mailer Version " + Application.ProductVersion;

                // 差出人のアドレスを編集する
                string fromAddress = Mail.fromName + " <" + Mail.mailAddress + ">";

                // 送信する
                smtp.SendMail(address, fromAddress, subject, body);
                
                // SMTPから切断する
                smtp.Close();
                
                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信完了";
            }
            catch (nMail.nMailException nex)
            {
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;
                return;
            }
            catch (Exception exp)
            {
                labelMessage.Text = "エラーメッセージ:" + exp.Message;
                return;
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
            if(header.Contains("X-Priority:")){
                priority = attach.GetHeaderField("X-Priority:", header);

                if(priority == "1" || priority == "2"){
                    _priority = "urgent";
                }
                else if(priority == "3"){
                    _priority = "normal";
                }
                else if(priority == "4" || priority == "5"){
                    _priority = "non-urgent";
                }
            }
            else if(header.Contains("X-MsMail-Priotiry:")){
                priority = attach.GetHeaderField("X-MsMail-Priotiry:", header);

                if(priority.ToLower() == "High"){
                    _priority = "urgent";
                }
                else if(priority.ToLower() == "Normal"){
                    _priority = "normal";
                }
                else if(priority.ToLower() == "low"){
                    _priority = "non-urgent";
                }
            }
            else if(header.Contains("Importance:")){
                priority = attach.GetHeaderField("Importance:", header);

                if(priority.ToLower() == "high"){
                    _priority = "urgent";
                }
                else if(priority.ToLower() == "normal"){
                    _priority = "normal";
                }
                else if(priority.ToLower() == "low"){
                    _priority = "non-urgent";
                }
            }
            else if(header.Contains("Priority:")){
                _priority = attach.GetHeaderField("Priority:", header);

                // 重要度が空値の時はnormalを入れる
                if(_priority.Length == 0){
                    _priority = "normal";
                }
            }

            return _priority;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if(e.Node.Index == 0){
                // 受信メールが選択された場合
                if(e.Node.Text == "メールボックス"){
                    listView1.Columns[0].Text = "名前";
                    listView1.Columns[1].Text = "メールアドレス";
                    listView1.Columns[2].Text = "最終データ更新日";
                    listView1.Columns[3].Text = "サイズ";
                    labelMessage.Text = "メールボックス";
                }
                else{
                    listView1.Columns[0].Text = "差出人";
                    listView1.Columns[1].Text = "件名";
                    listView1.Columns[2].Text = "受信日時";
                    listView1.Columns[3].Text = "サイズ";
                    labelMessage.Text = "受信メール";
                }
            }
            else if(e.Node.Index == 1){
                // 受信メールが選択された場合
                listView1.Columns[0].Text = "宛先";
                listView1.Columns[1].Text = "件名";
                listView1.Columns[2].Text = "送信日時";
                listView1.Columns[3].Text = "サイズ";
                labelMessage.Text = "送信メール";
            }
            else if(e.Node.Index == 2){
                // 受信メールが選択された場合
                listView1.Columns[0].Text = "差出人または宛先";
                listView1.Columns[1].Text = "件名";
                listView1.Columns[2].Text = "受信日時または送信日時";
                listView1.Columns[3].Text = "サイズ";
                labelMessage.Text = "ごみ箱";
            }

            // IEコンポが表示されていないとき
            if(this.browserBody.Visible == false){
                // テキストボックスを空値にする
                this.textBody.Text = "";
            }
            else{
                // IEコンポを閉じてテキストボックスを表示させる
                this.browserBody.Visible = false;
                this.textBody.Text = "";
                this.textBody.Visible = true;
            }

            // 添付リストが表示されているとき
            if(buttonAttachList.Visible == true){
                buttonAttachList.DropDownItems.Clear();
                buttonAttachList.Visible = false;
            }

            // ListViewItemSorterを解除する
            listView1.ListViewItemSorter = null;

            // リストビューを更新する
            UpdateListView();

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;
        }

        private void menuFileExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuToolSetEnv_Click(object sender, EventArgs e)
        {
            Form2 Form2 = new Form2();

            if(timer2.Enabled == true){
                timer2.Enabled = false;
            }
            
            DialogResult ret = Form2.ShowDialog();

            if(ret == DialogResult.OK){
                if(Form2.checkAutoGetMail.Checked == true){
                    timer2.Interval = Mail.getMailInterval * 60000;
                    timer2.Enabled = true;
                }
                else{
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
            Form3 NewMailForm = new Form3();

            // 親フォームをForm1に設定する
            NewMailForm.pForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.sList = collectionMail[SEND];

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
            int nSel = listView1.SelectedItems[0].Index;

            if (listView1.Columns[0].Text == "差出人") {
                // 受信メールのとき
                // 選択アイテムの数を取得
                int nLen = listView1.SelectedItems.Count;

                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                int[] nIndices = new int[nLen];

                for (int n = 0; n < nLen; n++) {
                    nIndices[n] = int.Parse(listView1.SelectedItems[n].Name);
                }

                // キーの並べ替え
                Array.Sort(nIndices);
                List<Mail> sList = collectionMail[RECEIVE];
                List<Mail> dList = collectionMail[DELETE];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = (Mail)sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        dList.Add(mail);
                        sList.Remove(mail);
                    }
                    nLen--;
                }
            } else if (listView1.Columns[0].Text == "宛先") {
                // 送信メールのとき
                // 選択アイテムの数を取得
                int nLen = listView1.SelectedItems.Count;

                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                int[] nIndices = new int[nLen];

                for (int n = 0; n < nLen; n++) {
                    nIndices[n] = int.Parse(listView1.SelectedItems[n].Name);
                }

                // キーの並べ替え
                Array.Sort(nIndices);
                List<Mail> sList = collectionMail[SEND];
                List<Mail> dList = collectionMail[DELETE];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = (Mail)sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        dList.Add(mail);
                        sList.Remove(mail);
                    }
                    nLen--;
                }
            } else if (listView1.Columns[0].Text == "差出人または宛先") {
                // 削除メールのとき
                if (MessageBox.Show("選択されたメールは完全に削除されます。\nよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                    // 選択アイテムの数を取得
                    int nLen = listView1.SelectedItems.Count;

                    if (nLen == 0)
                        return;

                    // 選択アイテムのキーを取得
                    int[] nIndices = new int[nLen];

                    for (int n = 0; n < nLen; n++) {
                        nIndices[n] = int.Parse(listView1.SelectedItems[n].Name);
                    }

                    // キーの並べ替え
                    Array.Sort(nIndices);
                    List<Mail> dList = collectionMail[DELETE];

                    while (nLen > 0) {
                        // 選択アイテムのキーから 選択アイテム群の位置を取得
                        int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                        ListViewItem item = listView1.SelectedItems[nIndex];

                        // 元リストからメールアイテムを取得
                        Mail mail = (Mail)dList[nIndices[nLen - 1]];

                        if (item.SubItems[1].Text == mail.subject) {
                            dList.Remove(mail);
                        }
                        nLen--;
                    }
                }
            }

            // IEコンポが表示されていないとき
            if (this.browserBody.Visible == false) {
                // テキストボックスを空値にする
                this.textBody.Text = "";
            } else {
                // IEコンポを閉じてテキストボックスを表示させる
                this.browserBody.Visible = false;
                this.textBody.Text = "";
                this.textBody.Visible = true;
            }

            // 添付リストが表示されているとき
            if (buttonAttachList.Visible == true) {
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

            // リストが空になったのを判定するフラグ
            bool bEmptyList = false;

            // 選択しているリスト位置が現在のリスト件数以上のとき
            if (listView1.Items.Count <= nSel) {
                // 選択しているリスト位置が0でないとき
                if (nSel != 0) {
                    nSel = listView1.Items.Count - 1;
                } else {
                    bEmptyList = true;
                }
            } else {
                // 選択しているリスト位置が0でないとき
                if (nSel != 0)
                    nSel = nSel - 1;
            }

            // リストが空でないとき
            if (bEmptyList == false) {
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

        private void menuNotReadYet_Click(object sender, EventArgs e)
        {
            // 受信メールのとき
            if(listView1.Columns[0].Text == "差出人"){
                // 選択アイテムの数を取得
                int nLen = listView1.SelectedItems.Count;

                // 選択アイテムの数が0のとき
                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                int[] nIndices = new int[nLen];

                for (int n = 0; n < nLen; n++) {
                    nIndices[n] = int.Parse(listView1.SelectedItems[n].Name);
                }

                // キーの並べ替え
                Array.Sort(nIndices);
                List<Mail> sList = collectionMail[RECEIVE];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = (Mail)sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        ((Mail)sList[nIndices[nLen - 1]]).notReadYet = true;
                    }
                    nLen--;
                }
            }
            else if(listView1.Columns[0].Text == "宛先"){
                // 送信メールのとき
                int nLen = listView1.SelectedItems.Count;

                // 選択アイテムの数が0のとき
                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                int[] nIndices = new int[nLen];

                for (int n = 0; n < nLen; n++) {
                    nIndices[n] = int.Parse(listView1.SelectedItems[n].Name);
                }

                // キーの並べ替え
                Array.Sort(nIndices);
                List<Mail> sList = collectionMail[SEND];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = (Mail)sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        ((Mail)sList[nIndices[nLen - 1]]).notReadYet = true;
                    }
                    nLen--;
                }
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                // 削除メールのとき
                int nLen = listView1.SelectedItems.Count;

                // 選択アイテムの数が0のとき
                if (nLen == 0)
                    return;

                // 選択アイテムのキーを取得
                int[] nIndices = new int[nLen];

                for (int n = 0; n < nLen; n++) {
                    nIndices[n] = int.Parse(listView1.SelectedItems[n].Name);
                }

                // キーの並べ替え
                Array.Sort(nIndices);
                List<Mail> sList = collectionMail[DELETE];

                while (nLen > 0) {
                    // 選択アイテムのキーから 選択アイテム群の位置を取得
                    int nIndex = listView1.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                    ListViewItem item = listView1.SelectedItems[nIndex];

                    // 元リストからメールアイテムを取得
                    Mail mail = (Mail)sList[nIndices[nLen - 1]];

                    if (item.SubItems[1].Text == mail.subject) {
                        ((Mail)sList[nIndices[nLen - 1]]).notReadYet = true;
                    }
                    nLen--;
                }
            }

            // ListViewItemSorterを解除する
            listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;

            // フォーカスを当て直す
            listView1.Items[currentRow].Selected = true;
            listView1.Items[currentRow].Focused = true;
            listView1.SelectedItems[0].EnsureVisible();
            listView1.Select();
            listView1.Focus();

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
            Icon appIcon;
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // メールボックスのときは反応しない
            if(listView1.Columns[0].Text == "名前"){
                return;
            }

            if(listView1.Columns[0].Text == "差出人"){
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            }
            else if (listView1.Columns[0].Text == "宛先"){
                mail = (Mail)collectionMail[SEND][(int)item.Tag];
            }
            else if (listView1.Columns[0].Text == "差出人または宛先"){
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

            // 1番目のカラムの表示が差出人か差出人または宛先のとき
            if(listView1.Columns[0].Text == "差出人" || listView1.Columns[0].Text == "差出人または宛先"){
                mail.notReadYet = false;

                // ListViewItemSorterを解除する
                listView1.ListViewItemSorter = null;

                // ツリービューとリストビューの表示を更新する
                UpdateTreeView();
                UpdateListView();

                // ListViewItemSorterを指定する
                listView1.ListViewItemSorter = listViewItemSorter;

                // フォーカスを当て直す
                listView1.Items[currentRow].Selected = true;
                listView1.Items[currentRow].Focused = true;
                listView1.SelectedItems[0].EnsureVisible();
                listView1.Select();
                listView1.Focus();

                // データ変更フラグをtrueにする
                dataDirtyFlag = true;
            }
            else if(listView1.Columns[0].Text == "宛先"){
                // 1番目のカラムが宛先のときは編集画面を表示する
                Form3 EditMailForm = new Form3();

                // 親フォームをForm1に設定する
                EditMailForm.pForm = this;

                // 親フォームにタイトルを設定する
                EditMailForm.Text = mail.subject + " - Ak@Ne!";

                // 送信箱の配列をForm3に渡す
                EditMailForm.sList = collectionMail[SEND];
                EditMailForm.listTag = (int)item.Tag;
                EditMailForm.isEdit = true;

                // 宛先、件名、本文をForm3に渡す
                EditMailForm.textAddress.Text = mail.address;
                EditMailForm.textCc.Text = mail.cc;
                EditMailForm.textBcc.Text = mail.bcc;
                EditMailForm.textSubject.Text = mail.subject;
                EditMailForm.textBody.Text = mail.body;

                // 重要度をForm3に渡す
                if(mail.priority == "urgent"){
                    EditMailForm.comboPriority.SelectedIndex = 0;
                }
                else if(mail.priority == "normal"){
                    EditMailForm.comboPriority.SelectedIndex = 1;
                }
                else{
                    EditMailForm.comboPriority.SelectedIndex = 2;
                }

                // 添付ファイルが付いているとき
                if(mail.attach != ""){
                    // 添付リストメニューを表示
                    EditMailForm.buttonAttachList.Visible = true;
                    // 添付ファイルリストを分割して一覧にする
                    EditMailForm.attachFileNameList = mail.attach.Split(',');
                    // 添付ファイルの数だけメニューを追加する
                    for (int no = 0; no < EditMailForm.attachFileNameList.Length; no++) {
                        if (File.Exists(EditMailForm.attachFileNameList[no])) {
                            appIcon = System.Drawing.Icon.ExtractAssociatedIcon(EditMailForm.attachFileNameList[no]);
                            EditMailForm.buttonAttachList.DropDownItems.Add(EditMailForm.attachFileNameList[no], appIcon.ToBitmap());
                        }
                        else{
                            EditMailForm.buttonAttachList.DropDownItems.Add(EditMailForm.attachFileNameList[no] + "は削除されています。");
                        }
                    }
                }

                // メール編集フォームを表示する
                EditMailForm.Show();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ファイル展開用のテンポラリフォルダの削除
            if(Directory.Exists(Application.StartupPath + @"\tmp") == true){
                Directory.Delete(Application.StartupPath + @"\tmp", true);
            }

            // データエラーフラグがfalseでデータ変更フラグがtrueのとき
            if (errorFlag == false && dataDirtyFlag == true) {
                // データファイルを削除する
                if(File.Exists(Application.StartupPath + @"\Mail.dat") == true){
                    File.Delete(Application.StartupPath + @"\Mail.dat");
                }

                // Threadオブジェクトを作成する
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(MailDataSave));

                // スレッドを開始する
                t.Start();

                // スレッドが終了するまで待機
                t.Join();
            }

            // 環境設定保存クラスを作成する
            initMail = new initClass();

            // 環境設定ファイルへアカウント情報を保存する
            initMail.m_fromName = Mail.fromName;
            initMail.m_mailAddress = Mail.mailAddress;
            initMail.m_userName = Mail.userName;
            // パスワードの暗号化を行う
            try {
                initMail.m_passWord = ACrypt.EncryptPasswordString(Mail.passWord);
            }
            catch (Exception) {
                // 例外発生時は旧バージョンと同じ動作
                initMail.m_passWord = Mail.passWord;
            }
            initMail.m_smtpServer = Mail.smtpServer;
            initMail.m_popServer = Mail.popServer;
            initMail.m_smtpPortNo = Mail.smtpPortNumber;
            initMail.m_popPortNo = Mail.popPortNumber;
            initMail.m_apopFlag = Mail.apopFlag;
            initMail.m_deleteMail = Mail.deleteMail;
            initMail.m_popBeforeSMTP = Mail.popBeforeSMTP;
            initMail.m_popOverSSL = Mail.popOverSSL;
            initMail.m_smtpAuth = Mail.smtpAuth;
            initMail.m_autoMailFlag = Mail.autoMailFlag;
            initMail.m_getMailInterval = Mail.getMailInterval;
            initMail.m_popSoundFlag = Mail.popSoundFlag;
            initMail.m_popSoundName = Mail.popSoundName;
            initMail.m_bodyIEShow = Mail.bodyIEShow;
            initMail.m_minimizeTaskTray = Mail.minimizeTaskTray;
            initMail.m_windowLeft = this.Left;
            initMail.m_windowTop = this.Top;
            initMail.m_windowWidth = this.Width;
            initMail.m_windowHeight = this.Height;
            initMail.m_windowStat = this.WindowState;

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(initClass));
            FileStream fs = new FileStream(Application.StartupPath + @"\AkaneMail.xml", FileMode.Create);
            serializer.Serialize(fs, initMail);
            fs.Close();
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

            // 最大化の時スプラッシュスクリーンよりも先にフォームが出ることがあるので
            // それを防ぐために一時的にフォームを非表示にする。
            this.Hide();

            // 環境設定ファイルが存在する場合は環境設定情報を読み込んでアカウント情報に設定する
            if(File.Exists(Application.StartupPath + @"\AkaneMail.xml")){
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(initClass));
                FileStream fs = new FileStream(Application.StartupPath + @"\AkaneMail.xml", FileMode.Open);
                initMail = (initClass)serializer.Deserialize(fs);
                fs.Close();
                Mail.fromName = initMail.m_fromName;
                Mail.mailAddress = initMail.m_mailAddress;
                Mail.userName = initMail.m_userName;

                // パスワードの復号化を行う
                try{
                    Mail.passWord = ACrypt.DecryptPasswordString(initMail.m_passWord);
                }
                catch(Exception){
                    // 新規インストール以外の初回起動時には必ずここに入る
                    Mail.passWord = initMail.m_passWord;
                }

                Mail.smtpServer = initMail.m_smtpServer;
                Mail.popServer = initMail.m_popServer;
                Mail.smtpPortNumber = initMail.m_smtpPortNo;
                Mail.popPortNumber = initMail.m_popPortNo;
                Mail.apopFlag = initMail.m_apopFlag;
                Mail.deleteMail = initMail.m_deleteMail;
                Mail.popBeforeSMTP = initMail.m_popBeforeSMTP;
                Mail.popOverSSL = initMail.m_popOverSSL;
                Mail.smtpAuth = initMail.m_smtpAuth;
                Mail.autoMailFlag = initMail.m_autoMailFlag;
                Mail.getMailInterval = initMail.m_getMailInterval;
                Mail.popSoundFlag = initMail.m_popSoundFlag;
                Mail.popSoundName = initMail.m_popSoundName;
                Mail.bodyIEShow = initMail.m_bodyIEShow;
                Mail.minimizeTaskTray = initMail.m_minimizeTaskTray;

                // 画面の表示が通常のとき 
                if(initMail.m_windowStat == FormWindowState.Normal){
                    // 過去のバージョンから環境設定ファイルを流用した初期起動以外はこの中に入る
                    if(initMail.m_windowLeft != 0 && initMail.m_windowTop != 0 && initMail.m_windowWidth != 0 && initMail.m_windowHeight != 0){
                        this.Left = initMail.m_windowLeft;
                        this.Top = initMail.m_windowTop;
                        this.Width = initMail.m_windowWidth;
                        this.Height = initMail.m_windowHeight;
                    }
                }
                else{
                    // 最大化または最小化の時はウィンドウ状態を設定する
                    this.WindowState = initMail.m_windowStat;
                }
            }

            try
            {
                // WinSockの初期化処理
                nMail.Winsock.Initialize();
            }
            catch(Exception exp){
                // 64bit版OSで同梱の32bit版OS用のnMail.dllを使用して起動したときはエラーになるため差し替えのお願いメッセージを表示する
                if(exp.Message.Contains("間違ったフォーマットのプログラムを読み込もうとしました。")){
                    MessageBox.Show("64bit版OSで32bit版OS用のnMail.dllを使用して実行した場合\nこのエラーが表示されます。\n\nお手数をお掛け致しますが同梱のnMail.dllをnMail.dll.32、nMail.dll.64をnMail.dllに名前を変更してAk@Ne!を起動\nしてください。", "Ak@Ne!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // 致命的なnMail.dllのエラーフラグをOn
                    nMailError = true;
                    Application.Exit();
                    return;
                }
            }

            // nMailのHTML添付ファイルの展開オプションをONにする
            nMail.Options.EnableSaveHtmlFile();

            // ファイル展開用のテンポラリフォルダの作成
            if(Directory.Exists(Application.StartupPath + @"\tmp") == false){
                Directory.CreateDirectory(Application.StartupPath + @"\tmp");
            }

            // Threadオブジェクトを作成する
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(MailDataLoad));

            splash.ProgressMsg = "メールデータの読み込み作業中です";

            // スレッドを開始する
            t.Start();

            // スレッドが終了するまで待機
            t.Join();

            // メール自動受信が設定されている場合はタイマーを起動する
            if(Mail.autoMailFlag == true){
                // 取得間隔*60000(60000秒=1分)をタイマー実行間隔に設定する
                timer2.Interval = Mail.getMailInterval * 60000;
                timer2.Enabled = true;
            }

            // ツリービューを展開する
            treeView1.ExpandAll();

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemComparerの作成と設定
            listViewItemSorter = new ListViewItemComparer();
            listViewItemSorter.ColumnModes = new ListViewItemComparer.ComparerMode[] { ListViewItemComparer.ComparerMode.String, ListViewItemComparer.ComparerMode.String, ListViewItemComparer.ComparerMode.DateTime, ListViewItemComparer.ComparerMode.Integer };

            // 受信or送信日時の降順で並べる
            listViewItemSorter.Column = 2;
            listViewItemSorter.Order = SortOrder.Descending;

            // ListViewItemSorterを指定する
            listView1.ListViewItemSorter = listViewItemSorter;

            // スプラッシュ・スクリーンの表示終了
            splash.Close();
            splash.Dispose();

            // 一時的に非表示にした画面を表示させる
            this.Show();

            // メインとなるフォームをアクティブに戻す
            this.Activate();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Appliction.Idleを削除する
            Application.Idle -= new EventHandler(Application_Idle);

            // 致命的なnMail.dllエラーがないとき
            if(nMailError != true){
                // WinSockを開放する
                nMail.Winsock.Done();
            }
        }

        private void menuMailReturnMail_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            Form3 NewMailForm = new Form3();
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listView1.SelectedItems.Count == 0) {
                return;
            }

            // 表示機能はシンプルなものに変わる
            if(listView1.Columns[0].Text == "差出人"){
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "宛先"){
                mail = (Mail)collectionMail[SEND][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

            // 親フォームをForm1に設定する
            NewMailForm.pForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.sList = collectionMail[SEND];

            // 返信のための宛先・件名を設定する
            NewMailForm.textAddress.Text = mail.address;
            NewMailForm.textSubject.Text = "Re:" + mail.subject;

            // 添付なしメールのときはbodyを渡す
            if(!attachMailReplay){
                //NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + mail.body;
                // UTF-8でエンコードされたメールのときはattachMailBodyを渡す
                if(attachMailBody != ""){
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + attachMailBody;
                }
                else{
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + mail.body;
                }
            }
            else{
                // 添付付きメールのときはattachMailBodyを渡す
                if(attachMailBody != ""){
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + attachMailBody;
                }
                else{
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.address + " was wrote ---\r\n\r\n" + mail.body;
                }
            }

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            Icon appIcon;
            bool htmlMail = false;
            bool base64Mail = false;
            ListViewItem item = listView1.SelectedItems[0];

            // メールボックスのときは反応しない
            if(listView1.Columns[0].Text == "名前"){
                return;
            }

            // リストの1番目のカラムが"差出人"のとき
            if(listView1.Columns[0].Text == "差出人"){
                // 受信メールの配列からデータを取得する
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "宛先"){
                // 1番目のカラムが"宛先"のとき送信メールの配列からデータを取得する
                mail = (Mail)collectionMail[SEND][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                // 1番目のカラムが"差出人または宛先"のとき削除メールの配列からデータを取得する
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

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
            if(mail.notReadYet == true){
                checkNotYetReadMail = true;
            }
            else{
                checkNotYetReadMail = false;
            }

            // 保存パスはプログラム直下に作成したtmpに設定する
            attach.Path = Application.StartupPath + @"\tmp";

            // 添付ファイルが存在するかを確認する
            int id = attach.GetId(mail.header);

            // 添付ファイルが存在する場合(存在しない場合は-1が返る)
            // もしくは HTML メールの場合
            if(id != nMail.Attachment.NoAttachmentFile || htmlMail){
                try{
                    // 旧バージョンからの変換データではないとき
                    if(mail.convert == ""){
                        // HTML/Base64のデコードを有効にする
                        nMail.Options.EnableDecodeBody();
                    }
                    else{
                        // HTML/Base64のデコードを無効にする
                        nMail.Options.DisableDecodeBodyText();
                    }
                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(mail.header, mail.body);

                    // 添付ファイルを取り外す
                    attach.Save();
                }
                catch(Exception ex){
                    // エラーメッセージを表示する
                    labelMessage.Text = String.Format("エラー メッセージ:{0:s}", ex.Message);
                    return;
                }

                // 添付返信フラグをtrueにする
                attachMailReplay = true;

                // IE コンポーネントを使用かつ HTML パートを保存したファイルがある場合
                if(Mail.bodyIEShow && attach.HtmlFile != ""){
                    // 本文表示用のテキストボックスの表示を非表示にしてHTML表示用のWebBrowserを表示する
                    this.textBody.Visible = false;
                    this.browserBody.Visible = true;

                    // Contents-Typeがtext/htmlでないとき(テキストとHTMLパートが存在する添付メール)
                    if(!htmlMail){
                        // テキストパートを返信文に格納する
                        attachMailBody = attach.Body;
                    }
                    else{
                        // 本文にHTMLタグが直書きされているタイプのHTMLメールのとき
                        // 展開したHTMLファイルをストリーム読み込みしてテキストを返信用の変数に格納する
                        FileStream fs = new FileStream(Application.StartupPath + @"\tmp\" + attach.HtmlFile, FileMode.Open);
                        StreamReader sr = new StreamReader(fs, Encoding.Default);
                        string htmlBody = sr.ReadToEnd();
                        sr.Close();
                        fs.Close();

                        // HTMLからタグを取り除いた本文を返信文に格納する
                        // attachMailBody = HtmlToText(mail.body, mail.header);
                        attachMailBody = HtmlToText(htmlBody, mail.header);
                    }

                    // 添付ファイル保存フォルダに展開されたHTMLファイルをWebBrowserで表示する
                    browserBody.AllowNavigation = true;
                    browserBody.Navigate(attach.Path + @"\" + attach.HtmlFile);
                }
                else{
                    // 添付ファイルを外した本文をテキストボックスに表示する
                    this.browserBody.Visible = false;
                    this.textBody.Visible = true;
                    // IE コンポーネントを使用せず、HTML メールで HTML パートを保存したファイルがある場合
                    if(htmlMail && !Mail.bodyIEShow && attach.HtmlFile != ""){
                        // 本文にHTMLタグが直書きされているタイプのHTMLメールのとき
                        // 展開したHTMLファイルをストリーム読み込みしてテキストボックスに表示する
                        FileStream fs = new FileStream(Application.StartupPath + @"\tmp\" + attach.HtmlFile, FileMode.Open);
                        StreamReader sr = new StreamReader(fs, Encoding.Default);
                        string htmlBody = sr.ReadToEnd();
                        sr.Close();
                        fs.Close();

                        // HTMLからタグを取り除く
                        htmlBody = HtmlToText(htmlBody, mail.header);

                        attachMailBody = htmlBody;
                        this.textBody.Text = htmlBody;
                    }
                    else if(attach.Body != ""){
                        // デコードした本文の行末が\n\nではないとき
                        if(!attach.Body.Contains("\n\n")){
                            attachMailBody = attach.Body;
                            this.textBody.Text = attach.Body;
                        }
                        else{
                            attach.Body.Replace("\n\n", "\r\n");
                            attachMailBody = attach.Body.Replace("\n", "\r\n");
                            this.textBody.Text = attachMailBody;
                        }
                    }
                    else{
                        this.textBody.Text = mail.body;
                    }
                }
                // 添付ファイルを外した本文が空値以外の場合
                // 添付ファイル名リストがnull以外のとき
                if(attach.FileNameList != null){
                    // IE コンポーネントありで、添付ファイルが HTML パートを保存したファイルのみの場合はメニューを表示しない
                    if(!Mail.bodyIEShow || attach.HtmlFile == "" || attach.FileNameList.Length > 1){
                        buttonAttachList.Visible = true;
                        attachMenuFlag = true;
                        // メニューに添付ファイルの名前を追加する
                        for(int no = 0; no < attach.FileNameList.Length; no++){
                            // IE コンポーネントありで、添付ファイルが HTML パートを保存したファイルはメニューに表示しない
                            if(!Mail.bodyIEShow || attach.FileNameList[no] != attach.HtmlFile){
                                appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.StartupPath + @"\tmp\" + attach.FileNameList[no]);
                                buttonAttachList.DropDownItems.Add(attach.FileNameList[no], appIcon.ToBitmap());
                            }
                        }
                    }
                }
            }
            else{
                // 添付ファイルが存在しない通常のメールまたは
                // 送信済みメールのときは本文をテキストボックスに表示する
                this.browserBody.Visible = false;
                this.textBody.Visible = true;

                // 添付ファイルリストが""でないとき
                if(mail.attach != ""){
                    buttonAttachList.Visible = true;

                    // 添付ファイルリストを分割して一覧にする
                    string[] attachFileNameList = mail.attach.Split(',');

                    for(int no = 0; no < attachFileNameList.Length; no++){
                        if(File.Exists(attachFileNameList[no])){
                            appIcon = System.Drawing.Icon.ExtractAssociatedIcon(attachFileNameList[no]);
                            buttonAttachList.DropDownItems.Add(attachFileNameList[no], appIcon.ToBitmap());
                        }
                        else{
                            buttonAttachList.DropDownItems.Add(attachFileNameList[no] + "は削除されています。");
                            buttonAttachList.DropDownItems[no].Enabled = false;
                        }
                    }
                }

                // Contents-TypeがBase64のメールの場合
                base64Mail = attach.GetDecodeHeaderField("Content-Transfer-Encoding:", mail.header).Contains("base64");

                // Base64の文章が添付されている場合
                if(base64Mail == true){
                    // 文章をデコードする
                    nMail.Options.EnableDecodeBody();

                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(mail.header, mail.body);

                    // 添付ファイルを取り外す
                    attach.Save();

                    if(!attach.Body.Contains("\n\n")){
                        attachMailBody = attach.Body;
                        this.textBody.Text = attach.Body;
                    }
                    else{
                        attach.Body.Replace("\n\n", "\r\n");
                        attachMailBody = attach.Body.Replace("\n", "\r\n");
                        this.textBody.Text = attachMailBody;
                    }
                }
                else{
                    // ISO-2022-JPでかつquoted-printableがある場合(nMail.dllが対応するまでの暫定処理)
                    if(attach.GetHeaderField("Content-Type:", mail.header).ToLower().Contains("iso-2022-jp") && attach.GetHeaderField("Content-Transfer-Encoding:", mail.header).Contains("quoted-printable")){
                        // 文章をデコードする
                        nMail.Options.EnableDecodeBody();

                        // ヘッダと本文付きの文字列を添付クラスに追加する
                        attach.Add(mail.header, mail.body);

                        // 添付ファイルを取り外す
                        attach.Save();

                        if(!attach.Body.Contains("\n\n")){
                            attachMailBody = attach.Body;
                            this.textBody.Text = attach.Body;
                        }
                        else{
                            attach.Body.Replace("\n\n", "\r\n");
                            attachMailBody = attach.Body.Replace("\n", "\r\n");
                            this.textBody.Text = attachMailBody;
                        }
                    }
                    else if(attach.GetHeaderField("X-NMAIL-BODY-UTF8:", mail.header).Contains("8bit")){
                        // Unicode化されたUTF-8文字列をデコードする
                        // 1件のメールサイズの大きさのbyte型配列を確保
                        byte[] bs = new byte[mail.body.Length];

                        // 配列にメール本文を1文字ずつ格納する
                        for(int i = 0; i < mail.body.Length; i++){
                            bs[i] = (byte)mail.body[i];
                        }

                        // GetStringでバイト型配列をUTF-8の配列にエンコードする
                        attachMailBody = Encoding.UTF8.GetString(bs);
                        this.textBody.Text = attachMailBody;
                    }
                    else{
                        // テキストボックスに出力する文字コードをJISに変更する
                        byte[] b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.body);
                        string strBody = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                        // 本文をテキストとして表示する
                        // this.textBody.Text = mail.body;
                        this.textBody.Text = strBody;
                    }
                }
            }
        }

        private void menuFileGetAttatch_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if(listView1.SelectedItems.Count == 0){
                return;
            }

            // 送信メール以外も展開できるように変更
            if(listView1.Columns[0].Text == "差出人"){
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

            // 添付ファイル保存対象フォルダを選択する
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK){
                try{
                    // 添付ファイルクラスを作成する
                    nMail.Attachment attach = new nMail.Attachment();

                    // 保存パスを設定する
                    attach.Path = folderBrowserDialog1.SelectedPath;

                    // 添付ファイル展開用のテンポラリファイルを作成する
                    string tmpFileName = Path.GetTempFileName();
                    StreamWriter writer = new StreamWriter(tmpFileName);
                    writer.Write(mail.header);
                    writer.Write("\r\n");
                    writer.Write(mail.body);
                    writer.Close();

                    // テンポラリファイルを開いて添付ファイルを開く
                    StreamReader reader = new StreamReader(tmpFileName);
                    string header = reader.ReadToEnd();
                    reader.Close();

                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(header);

                    // 添付ファイルを保存する
                    attach.Save();

                    MessageBox.Show(attach.Path + "に添付ファイル" + attach.FileName + "を保存しました。", "添付ファイルの取り出し", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (nMailException nex){
                    if(nex.ErrorCode == nMail.Attachment.ErrorFileOpen){
                        MessageBox.Show("添付ファイルがオープンできません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else if (nex.ErrorCode == nMail.Attachment.ErrorInvalidNo){
                        MessageBox.Show("分割されたメールの順番が正しくないか、該当しないファイルが入っています。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else if (nex.ErrorCode == nMail.Attachment.ErrorPartial){
                        MessageBox.Show("分割されたメールが全て揃っていません", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
                catch (Exception ex){
                    MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void menuSaveMailFile_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            string fileBody = "";
            string fileHeader = "";

            // ListViewから選択した1行の情報をitemに格納する
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if(listView1.SelectedItems.Count == 0){
                return;
            }

            // どの項目でも保存できるように変更
            if(listView1.Columns[0].Text == "差出人"){
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "宛先"){
                mail = (Mail)collectionMail[SEND][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

            // ファイル名にメールの件名を入れる
            saveFileDialog1.FileName = mail.subject;

            // 名前を付けて保存
            if(saveFileDialog1.ShowDialog() == DialogResult.OK){
                if(saveFileDialog1.FileName != ""){
                    try{
                        // ヘッダから文字コードを取得する(添付付きは取得できない)
                        string enc = GetEncodingString(mail.header);
                        
                        // 出力する文字コードがUTF-8ではないとき
                        if(enc.ToLower().Contains("iso-") || enc.ToLower().Contains("shift_") || enc.ToLower().Contains("euc") || enc.ToLower().Contains("windows")){
                            // 出力するヘッダをUTF-8から各文字コードに変換する
                            Byte[] b = Encoding.GetEncoding(enc).GetBytes(mail.header);
                            fileHeader = Encoding.GetEncoding(enc).GetString(b);

                            // 出力する本文をUTF-8から各文字コードに変換する
                            b = Encoding.GetEncoding(enc).GetBytes(mail.body);
                            fileBody = Encoding.GetEncoding(enc).GetString(b);
                        }
                        else if(enc.ToLower().Contains("utf-8") || mail.header.Contains("X-NMAIL-BODY-UTF8: 8bit")){
                            // text/plainまたはmultipart/alternativeでUTF-8でエンコードされたメールのとき
                            // nMail.dllはUTF-8エンコードのメールを8bit単位に分解してUncode(16bit)扱いで格納している。
                            // これはUnicodeで文字列を受け取る関数内で生のUTF-8の文字列を受け取っておかしなことに
                            // なるのを防ぐための意図で行われている。
                            // これをデコードするにはバイト型で格納し、UTF-8でデコードし直せば文字化けのような文字列を
                            // 可読化することができる。

                            // 1件のメールサイズの大きさのbyte型配列を確保
                            byte[] bs = new byte[mail.body.Length];

                            // 配列にメール本文を1文字ずつ格納する
                            for(int i=0; i < mail.body.Length; i++){
                                bs[i] = (byte)mail.body[i];
                            }

                            // GetStringでバイト型配列をUTF-8の配列にエンコードする
                            fileBody = Encoding.UTF8.GetString(bs);

                            // fileHeaderにヘッダを格納する
                            fileHeader = mail.header;

                            // ファイル出力フラグにUTF-8を設定する
                            enc = "utf-8";
                        }else{
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

                        StreamWriter writer = new StreamWriter(saveFileDialog1.FileName, false, writeEnc);

                        // 受信メール(ヘッダが存在する)のとき
                        if(mail.header.Length > 0){
                            writer.Write(fileHeader);
                            writer.Write("\r\n");
                        }
                        else{
                            // 送信メールのときはヘッダの代わりに送り先と件名を出力
                            writer.WriteLine("To: " + mail.address);
                            writer.WriteLine("Subject: " + mail.subject);
                            writer.Write("\r\n");
                        }
                        writer.Write(fileBody);
                        writer.Close();
                    }
                    catch(Exception ex){
                        MessageBox.Show(String.Format("エラー メッセージ:{0:s}", ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private void menuFileClearTrush_Click(object sender, EventArgs e)
        {
            // ごみ箱の配列を空っぽにする
            if (MessageBox.Show("ごみ箱の中身をすべて削除します。\nよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                for (int i = collectionMail[DELETE].Count - 1; i >= 0; i--) {
                    collectionMail[DELETE].RemoveAt(i);
                }

                // IEコンポが表示されていないとき
                if (this.browserBody.Visible == false) {
                    // テキストボックスを空値にする
                    this.textBody.Text = "";
                } else {
                    // IEコンポを閉じてテキストボックスを表示させる
                    this.browserBody.Visible = false;
                    this.textBody.Text = "";
                    this.textBody.Visible = true;
                }

                // 添付リストが表示されているとき
                if (buttonAttachList.Visible == true) {
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

                // データ変更フラグをtrueにする
                dataDirtyFlag = true;
            }
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            // バージョン情報を表示する
            Form4 AboutForm = new Form4();
            AboutForm.ShowDialog();
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listView1.SelectedItems[0];

            // 選択している場所によって添付を開く動作が変わるため追加
            if(listView1.Columns[0].Text == "差出人"){
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "宛先"){
                mail = (Mail)collectionMail[SEND][(int)item.Tag];
            }
            else if(listView1.Columns[0].Text == "差出人または宛先"){
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

            // ファイルを開くかの確認をする
            if(MessageBox.Show(e.ClickedItem.Text + "を開きますか？\nファイルによってはウイルスの可能性もあるため\n注意してファイルを開いてください。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes){
                // 受信されたメールのとき
                if(mail.attach.Length == 0){
                    System.Diagnostics.Process.Start(Application.StartupPath + @"\tmp\" + e.ClickedItem.Text);
                }
                else{
                    // 送信メールのとき
                    System.Diagnostics.Process.Start(e.ClickedItem.Text);
                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // メールボックスのときはソートしない
            if(listView1.Columns[0].Text == "名前")
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
            if(this.WindowState == System.Windows.Forms.FormWindowState.Minimized && Mail.minimizeTaskTray == true){
                // フォームが最小化の状態であればフォームを非表示にする   
                this.Hide();
                // トレイリストのアイコンを表示する   
                notifyIcon1.Visible = true;
            }
        }

        private void menuTaskRestoreWindow_Click(object sender, EventArgs e)
        {
            // フォームを表示する   
            this.Visible = true;

            // 現在の状態が最小化の状態であれば通常の状態に戻す   
            if(this.WindowState == FormWindowState.Minimized){
                this.WindowState = FormWindowState.Normal;
            }

            // フォームをアクティブにする   
            this.Activate();
        }

        private void menuFile_DropDownOpening(object sender, EventArgs e)
        {
            // メールの選択数が0またはメールボックスのとき
            if (listView1.SelectedItems.Count == 0 || listView1.Columns[0].Text == "名前") {
                menuSaveMailFile.Enabled = false;
                menuFileGetAttatch.Enabled = false;
            } else if (listView1.SelectedItems.Count == 1) {
                // メールが1件選択されたとき
                menuSaveMailFile.Enabled = true;
                // 添付ファイルメニューが開いているとき
                if (attachMenuFlag == true) {
                    menuFileGetAttatch.Enabled = true;
                } else {
                    menuFileGetAttatch.Enabled = false;
                }
            } else {
                // メールが複数件選択されたとき
                menuSaveMailFile.Enabled = false;
                menuFileGetAttatch.Enabled = false;
            }

            // 削除メールが0件の場合
            if (collectionMail[DELETE].Count == 0) {
                menuFileClearTrush.Enabled = false;
            } else {
                menuFileClearTrush.Enabled = true;
            }
        }

        private void menuMail_DropDownOpening(object sender, EventArgs e)
        {
            // メールの選択数が0またはメールボックスのとき
            if (listView1.SelectedItems.Count == 0 || listView1.Columns[0].Text == "名前") {
                menuMailDelete.Enabled = false;
                menuMailReturnMail.Enabled = false;
                menuMailFowerdMail.Enabled = false;
            } else if (listView1.SelectedItems.Count == 1) {
                // メールが1件選択されたとき
                menuMailDelete.Enabled = true;
                menuMailReturnMail.Enabled = true;
                menuMailFowerdMail.Enabled = true;
            } else {
                // メールが複数件選択されたとき
                menuMailDelete.Enabled = true;
                menuMailReturnMail.Enabled = false;
                menuMailFowerdMail.Enabled = false;
            }
        }

        private void menuListView_Opening(object sender, CancelEventArgs e)
        {
            // メールの選択数が0またはメールボックスのとき
            if (listView1.SelectedItems.Count == 0 || listView1.Columns[0].Text == "名前") {
                menuReturnMail.Enabled = false;
                menuDelete.Enabled = false;
                menuGetAttach.Enabled = false;
                menuFowerdMail.Enabled = false;
            } else if (listView1.SelectedItems.Count == 1) {
                // メールが1件選択されたとき
                menuReturnMail.Enabled = true;
                menuDelete.Enabled = true;
                menuFowerdMail.Enabled = true;
                // 添付ファイルメニューが開いているとき
                if (attachMenuFlag == true) {
                    menuGetAttach.Enabled = true;
                } else {
                    menuGetAttach.Enabled = false;
                }
            } else {
                // メールが複数件選択されたとき
                menuReturnMail.Enabled = false;
                menuGetAttach.Enabled = false;
                menuFowerdMail.Enabled = false;
            }

            // 未読メールのとき
            if (checkNotYetReadMail == true) {
                menuNotReadYet.Enabled = false;
            } else {
                // メールの選択数が0またはメールボックスのとき
                if (listView1.SelectedItems.Count == 0 || listView1.Columns[0].Text == "名前") {
                    menuNotReadYet.Enabled = false;
                } else {
                    menuNotReadYet.Enabled = true;
                }
            }

            // 送信メールを選択したとき
            if (listView1.Columns[0].Text == "宛先") {
                menuNotReadYet.Text = "未送信にする(&N)";
            } else {
                menuNotReadYet.Text = "未読にする(&N)";
            }
        }

        private void menuTreeView_Opening(object sender, CancelEventArgs e)
        {
            // 削除メールが0件の場合
            if (collectionMail[DELETE].Count == 0) {
                menuClearTrush.Enabled = false;
            } else {
                menuClearTrush.Enabled = true;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            // メールの選択数が0またはメールボックスのとき
            if (listView1.SelectedItems.Count == 0 || listView1.Columns[0].Text == "名前") {
                buttonMailDelete.Enabled = false;
                buttonReturnMail.Enabled = false;
                buttonForwardMail.Enabled = false;
            } else if (listView1.SelectedItems.Count == 1) {
                // メールが1件選択されたとき
                buttonMailDelete.Enabled = true;
                buttonReturnMail.Enabled = true;
                buttonForwardMail.Enabled = true;
            } else {
                // メールが複数件選択されたとき
                buttonMailDelete.Enabled = true;
                buttonReturnMail.Enabled = false;
                buttonForwardMail.Enabled = false;
            }

            // データ読み込みエラーを起こしたとき
            if (errorFlag == true) {
                this.Hide();
                this.Close();
            }
        }

        private void menuMailFowerdMail_Click(object sender, EventArgs e)
        {
            string strFrom = "";
            string strTo = "";
            string strDate = "";
            string strSubject = "";

            Icon appIcon;
            Mail mail = null;
            Form3 NewMailForm = new Form3();
            ListViewItem item = listView1.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listView1.SelectedItems.Count == 0) {
                return;
            }

            // 表示機能はシンプルなものに変わる
            if (listView1.Columns[0].Text == "差出人") {
                mail = (Mail)collectionMail[RECEIVE][(int)item.Tag];
            } else if (listView1.Columns[0].Text == "宛先") {
                mail = (Mail)collectionMail[SEND][(int)item.Tag];
            } else if (listView1.Columns[0].Text == "差出人または宛先") {
                mail = (Mail)collectionMail[DELETE][(int)item.Tag];
            }

            // 親フォームをForm1に設定する
            NewMailForm.pForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.sList = collectionMail[SEND];

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
            } else {
                strFrom = Mail.mailAddress;
                strTo = mail.address;
                strDate = mail.date;
                strSubject = mail.subject;
            }

            string fwHeader = "----------------------- Original Message -----------------------\r\n";
            fwHeader = fwHeader + " From: " + strFrom + "\r\n To: " + strTo + "\r\n Date: " + strDate;
            fwHeader = fwHeader + "\r\n Subject:" + strSubject + "\r\n----\r\n\r\n";

            // 添付なしメールのときはbodyを渡す
            if (!attachMailReplay){
                // NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + mail.body;
                // UTF-8でエンコードされたメールのときはattachMailBodyを渡す
                if(attachMailBody != ""){
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + attachMailBody;
                }
                else{
                    // JISコードなどのメールは通常のbodyを渡す
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + mail.body;
                }
            }
            else{
                // 添付付きメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + attachMailBody;
                } else {
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
                for (int no = 0; no < NewMailForm.attachFileNameList.Length; no++) {
                    appIcon = System.Drawing.Icon.ExtractAssociatedIcon(NewMailForm.attachFileNameList[no]);
                    NewMailForm.buttonAttachList.DropDownItems.Add(NewMailForm.attachFileNameList[no], appIcon.ToBitmap());
                }
            } else if (this.buttonAttachList.Visible == true) {
                // 受信メールで添付ファイルがあるとき
                // 添付リストメニューを表示
                NewMailForm.buttonAttachList.Visible = true;
                                
                // 添付ファイルの数だけメニューを追加する
                for (int no = 0; no < this.buttonAttachList.DropDownItems.Count; no++) {
                    // 添付ファイルが存在するかを確認してから添付する
                    if (File.Exists(Application.StartupPath + @"\tmp\" + this.buttonAttachList.DropDownItems[no].Text)) {
                        appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.StartupPath + @"\tmp\" + this.buttonAttachList.DropDownItems[no].Text);
                        NewMailForm.buttonAttachList.DropDownItems.Add(Application.StartupPath + @"\tmp\" + this.buttonAttachList.DropDownItems[no].Text, appIcon.ToBitmap());
                    }
                }
            }

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

    }
}