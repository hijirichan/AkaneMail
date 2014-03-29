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
        MailBox mailBox;

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
        private MailSettings MailSetting;

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

            mailBox = new MailBox();
        }

        /// <summary>
        /// ツリービューの更新
        /// </summary>
        public void UpdateTreeView()
        {
            // メールの件数を設定する
            treeMailBoxFolder.Nodes[0].Nodes[0].Text = "受信メール (" + mailBox.Receive.Count + ")";
            treeMailBoxFolder.Nodes[0].Nodes[1].Text = "送信メール (" + mailBox.Send.Count + ")";
            treeMailBoxFolder.Nodes[0].Nodes[2].Text = "ごみ箱 (" + mailBox.Trash.Count + ")";
        }

        /// <summary>
        /// リストビューの更新
        /// </summary>
        public void UpdateListView()
        {

            MailFolder folder = null;

            // リストビューの描画を止める
            listMail.BeginUpdate();

            // リストビューの内容をクリアする
            listMail.Items.Clear();

            if (listMail.Columns[0].Text == "差出人") {
                // 受信メールの場合
                folder = mailBox.Receive;
            }
            else if (listMail.Columns[0].Text == "宛先") {
                // 送信メールの場合
                folder = mailBox.Send;
            }
            else if (listMail.Columns[0].Text == "差出人または宛先") {
                // 削除メールの場合
                folder = mailBox.Trash;
            }
            else if (listMail.Columns[0].Text == "名前") {
                // メールボックスのとき
                ListViewItem item = new ListViewItem(AccountInfo.fromName);
                item.SubItems.Add(AccountInfo.mailAddress);
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
                listMail.Items.Add(item);
                listMail.EndUpdate();
                return;
            }

            folder.Select(CreateMailItem).Select(listMail.Items.Add).ToList();
            listMail.EndUpdate();
        }

        private ListViewItem CreateMailItem(Mail mail, int index)
        {
            ListViewItem item = new ListViewItem(mail.Address);
            if (mail.Subject != "") {
                item.SubItems.Add(mail.Subject);
            }
            else {
                item.SubItems.Add("(no subject)");
            }

            // メールの受信(送信)日時とメールサイズをリストのサブアイテムに追加する
            item.SubItems.Add(mail.Date);
            item.SubItems.Add(mail.Size);

            // 各項目のタグに要素の番号を格納する
            item.Tag = index;
            item.Name = index.ToString();

            // 未読(未送信)の場合は、フォントを太字にする
            if (mail.NotReadYet) {
                item.Font = new Font(this.Font, FontStyle.Bold);
            }

            // 重要度が高い場合は、フォントを太字にする
            if (mail.Priority == MailPriority.Urgent) {
                item.ForeColor = Color.Tomato;
            }
            else if (mail.Priority == MailPriority.NonUrgent) {
                item.ForeColor = Color.LightBlue;
            }
            return item;
        }

        private static object lockobj = new object();
        #region UI更新
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
        private void EnableButton(bool enable)
        {
            // メール受信のメニューとツールボタンを有効化する
            menuRecieveMail.Enabled = true;
            buttonRecieveMail.Enabled = true;

            // メール送信のメニューとツールボタンが有効か設定する
            menuSendMail.Enabled = enable;
            buttonSendMail.Enabled = enable;
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
            listMail.ListViewItemSorter = null;

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
            listMail.ListViewItemSorter = listViewItemSorter;

            // フラグの値が0のとき
            if (flag == 0) {
                // 受信メールのとき
                if (listMail.Columns[0].Text == "差出人") {
                    // ListViewの1行目にフォーカスを当て直す
                    listMail.Items[0].Selected = true;
                    listMail.Items[0].Focused = true;
                    listMail.SelectedItems[0].EnsureVisible();
                    listMail.Select();
                    listMail.Focus();
                }
            }
        }

        private void Invoke(Action invokeAction)
        {
            this.Invoke((Delegate)invokeAction);
        }

        private void Invoke<T>(Action<T> action, T param)
        {
            this.Invoke((Delegate)action, param);
        }
        #endregion

        /// <summary>
        /// 設定ファイルからアプリケーション設定を読み出す
        /// </summary>
        public void LoadSettings()
        {
            // 環境設定保存クラスを作成する
            MailSetting = new MailSettings();

            // アカウント情報(初期値)を設定する
            AccountInfo.fromName = "";
            AccountInfo.mailAddress = "";
            AccountInfo.userName = "";
            AccountInfo.passWord = "";
            AccountInfo.smtpServer = "";
            AccountInfo.popServer = "";
            AccountInfo.popPortNumber = 110;
            AccountInfo.smtpPortNumber = 25;
            AccountInfo.apopFlag = false;
            AccountInfo.deleteMail = false;
            AccountInfo.popBeforeSMTP = false;
            AccountInfo.popOverSSL = false;
            AccountInfo.smtpAuth = false;
            AccountInfo.autoMailFlag = false;
            AccountInfo.getMailInterval = 10;
            AccountInfo.popSoundName = "";
            AccountInfo.bodyIEShow = false;
            AccountInfo.minimizeTaskTray = false;

            // 環境設定ファイルが存在する場合は環境設定情報を読み込んでアカウント情報に設定する
            if (File.Exists(Application.StartupPath + @"\AkaneMail.xml")) {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MailSettings));
                using (var fs = new FileStream(Application.StartupPath + @"\AkaneMail.xml", FileMode.Open)) {
                    MailSetting = (MailSettings)serializer.Deserialize(fs);
                }

                // アカウント情報
                AccountInfo.fromName = MailSetting.m_fromName;
                AccountInfo.mailAddress = MailSetting.m_mailAddress;
                AccountInfo.userName = MailSetting.m_userName;
                AccountInfo.passWord = Decrypt(MailSetting.m_passWord);

                // 接続情報
                AccountInfo.smtpServer = MailSetting.m_smtpServer;
                AccountInfo.popServer = MailSetting.m_popServer;
                AccountInfo.smtpPortNumber = MailSetting.m_smtpPortNo;
                AccountInfo.popPortNumber = MailSetting.m_popPortNo;
                AccountInfo.apopFlag = MailSetting.m_apopFlag;
                AccountInfo.deleteMail = MailSetting.m_deleteMail;
                AccountInfo.popBeforeSMTP = MailSetting.m_popBeforeSMTP;
                AccountInfo.popOverSSL = MailSetting.m_popOverSSL;
                AccountInfo.smtpAuth = MailSetting.m_smtpAuth;

                // 自動受信設定
                AccountInfo.autoMailFlag = MailSetting.m_autoMailFlag;
                AccountInfo.getMailInterval = MailSetting.m_getMailInterval;

                // 通知設定
                AccountInfo.popSoundFlag = MailSetting.m_popSoundFlag;
                AccountInfo.popSoundName = MailSetting.m_popSoundName;
                AccountInfo.bodyIEShow = MailSetting.m_bodyIEShow;
                AccountInfo.minimizeTaskTray = MailSetting.m_minimizeTaskTray;

                // 画面の表示が通常のとき 
                if (MailSetting.m_windowStat == FormWindowState.Normal) {
                    // 過去のバージョンから環境設定ファイルを流用した初期起動以外はこの中に入る
                    if (MailSetting.m_windowLeft != 0 && MailSetting.m_windowTop != 0 && MailSetting.m_windowWidth != 0 && MailSetting.m_windowHeight != 0) {
                        this.Left = MailSetting.m_windowLeft;
                        this.Top = MailSetting.m_windowTop;
                        this.Width = MailSetting.m_windowWidth;
                        this.Height = MailSetting.m_windowHeight;
                    }
                }
                this.WindowState = MailSetting.m_windowStat;
            }
        }

        /// <summary>
        /// アプリケーション設定を設定ファイルに書き出す
        /// </summary>
        public void SaveSettings()
        {
            MailSetting = new MailSettings()
            {
                // アカウント情報
                m_fromName = AccountInfo.fromName,
                m_mailAddress = AccountInfo.mailAddress,
                m_userName = AccountInfo.userName,
                m_passWord = Encrypt(AccountInfo.passWord),

                // 接続情報
                m_smtpServer = AccountInfo.smtpServer,
                m_popServer = AccountInfo.popServer,
                m_smtpPortNo = AccountInfo.smtpPortNumber,
                m_popPortNo = AccountInfo.popPortNumber,
                m_apopFlag = AccountInfo.apopFlag,
                m_deleteMail = AccountInfo.deleteMail,
                m_popBeforeSMTP = AccountInfo.popBeforeSMTP,
                m_popOverSSL = AccountInfo.popOverSSL,
                m_smtpAuth = AccountInfo.smtpAuth,

                // 自動受信設定
                m_autoMailFlag = AccountInfo.autoMailFlag,
                m_getMailInterval = AccountInfo.getMailInterval,

                // 通知設定
                m_popSoundFlag = AccountInfo.popSoundFlag,
                m_popSoundName = AccountInfo.popSoundName,
                m_bodyIEShow = AccountInfo.bodyIEShow,
                m_minimizeTaskTray = AccountInfo.minimizeTaskTray,

                // ウィンドウ設定
                m_windowLeft = this.Left,
                m_windowTop = this.Top,
                m_windowWidth = this.Width,
                m_windowHeight = this.Height,
                m_windowStat = this.WindowState
            };

            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(MailSettings));

            using (var fs = new FileStream(Application.StartupPath + @"\AkaneMail.xml", FileMode.Create)) {
                serializer.Serialize(fs, MailSetting);
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

        /// <summary> 添付ファイルメニューに登録されている要素を破棄する </summary>
        private void ClearAttachMenu()
        {
            buttonAttachList.DropDownItems.Clear();
            buttonAttachList.Visible = false;
            attachMenuFlag = false;
            attachMailReplay = false;
            attachMailBody = "";
        }



        /// <summary>
        /// 指定されたメールを開く
        /// </summary>
        /// <param name="mail">メール</param>
        private void OpenMail(Mail mail)
        {

            ClearAttachMenu();

            // 添付ファイルクラスのインスタンスを作成する
            var attach = new nMail.Attachment();

            // 未読メールの場合
            checkNotYetReadMail = mail.NotReadYet;

            // 保存パスはプログラム直下に作成したtmpに設定する
            attach.Path = Application.StartupPath + @"\tmp";

            // Contents-Typeがtext/htmlのメールか確認するフラグを取得する
            var isHtmlMail = attach.GetHeaderField("Content-Type:", mail.Header).Contains("text/html");
            // 添付ファイルが存在するかを確認する
            var exists = attach.GetId(mail.Header) != nMail.Attachment.NoAttachmentFile;

            // 添付ファイルが存在する場合(存在しない場合は-1が返る)
            // もしくは HTML メールの場合
            if (exists || isHtmlMail) {
                try {
                    // 旧バージョンからの変換データではないとき
                    if (mail.Convert == "") {
                        // HTML/Base64のデコードを有効にする
                        Options.EnableDecodeBody();
                    }
                    else {
                        // HTML/Base64のデコードを無効にする
                        Options.DisableDecodeBodyText();
                    }
                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(mail.Header, mail.Body);

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
                if (AccountInfo.bodyIEShow && attach.HtmlFile != "") {
                    // 本文表示用のテキストボックスの表示を非表示にしてHTML表示用のWebBrowserを表示する
                    this.textBody.Visible = false;
                    this.browserBody.Visible = true;

                    // Contents-Typeがtext/htmlでないとき(テキストとHTMLパートが存在する添付メール)
                    if (!isHtmlMail) {
                        // テキストパートを返信文に格納する
                        attachMailBody = attach.Body;
                    }
                    else {
                        // 本文にHTMLタグが直書きされているタイプのHTMLメールのとき
                        // 展開したHTMLファイルをストリーム読み込みしてテキストを返信用の変数に格納する
                        using (var sr = new StreamReader(Application.StartupPath + @"\tmp\" + attach.HtmlFile, Encoding.Default)) {
                            string htmlBody = sr.ReadToEnd();

                            // HTMLからタグを取り除いた本文を返信文に格納する
                            attachMailBody = Mail.HtmlToText(htmlBody, mail.Header);
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
                    if (isHtmlMail && !AccountInfo.bodyIEShow && attach.HtmlFile != "") {
                        // 本文にHTMLタグが直書きされているタイプのHTMLメールのとき
                        // 展開したHTMLファイルをストリーム読み込みしてテキストボックスに表示する
                        using (var sr = new StreamReader(Application.StartupPath + @"\tmp\" + attach.HtmlFile, Encoding.Default)) {
                            string htmlBody = sr.ReadToEnd();

                            // HTMLからタグを取り除く
                            htmlBody = Mail.HtmlToText(htmlBody, mail.Header);

                            attachMailBody = htmlBody;
                            this.textBody.Text = htmlBody;
                        }
                    }
                    else if (attach.Body != "") {
                        var text = BreakLine(attach.Body);
                        attachMailBody = text;
                        this.textBody.Text = text;
                        }
                        else {
                        this.textBody.Text = mail.Body;
                        }
                    }
                if (attach.FileNameList != null) {
                    // IE コンポーネントありで、添付ファイルが HTML パートを保存したファイルのみの場合はメニューを表示しない
                    if (!AccountInfo.bodyIEShow || attach.HtmlFile == "" || attach.FileNameList.Length > 1) {
                        buttonAttachList.Visible = true;
                        attachMenuFlag = true;
                        // IE コンポーネントありで、添付ファイルが HTML パートを保存したファイルはメニューに表示しない
                        // foreach (var attachFile in attach.FileNameList.Where(a => a != attach.HtmlFile)) {
                        buttonAttachList.DropDownItems.AddRange(GenerateMenuItem(attach.Path + "\\", attach.FileNameList).ToArray());
                        }
                    }
                }
            else {
                // 添付ファイルが存在しない通常のメールまたは
                // 送信済みメールのときは本文をテキストボックスに表示する
                this.browserBody.Visible = false;
                this.textBody.Visible = true;

                // 添付ファイルリストが""でないとき
                if (mail.Attaches.Length != 0) {
                    buttonAttachList.Visible = true;

                    buttonAttachList.DropDownItems.AddRange(GenerateMenuItem(mail.Attaches).ToArray());
                }

                var base64Mail = attach.GetDecodeHeaderField("Content-Transfer-Encoding:", mail.Header).Contains("base64");

                // Base64の文章が添付されている場合
                if (base64Mail) {
                    // 文章をデコードする
                    Options.EnableDecodeBody();

                    // ヘッダと本文付きの文字列を添付クラスに追加する
                    attach.Add(mail.Header, mail.Body);

                    // 添付ファイルを取り外す
                    attach.Save();

                    var text = BreakLine(attach.Body);
                    attachMailBody = text;
                    this.textBody.Text = text;
                }
                else {
                    // ISO-2022-JPでかつquoted-printableがある場合(nMail.dllが対応するまでの暫定処理)
                    if (attach.GetHeaderField("Content-Type:", mail.Header).ToLower().Contains("iso-2022-jp") && attach.GetHeaderField("Content-Transfer-Encoding:", mail.Header).Contains("quoted-printable")) {
                        // 文章をデコードする
                        Options.EnableDecodeBody();

                        // ヘッダと本文付きの文字列を添付クラスに追加する
                        attach.Add(mail.Header, mail.Body);

                        // 添付ファイルを取り外す
                        attach.Save();

                        var text = BreakLine(attach.Body);
                        attachMailBody = text;
                        this.textBody.Text = text;
                    }
                    else if (attach.GetHeaderField("X-NMAIL-BODY-UTF8:", mail.Header).Contains("8bit")) {
                        // Unicode化されたUTF-8文字列をデコードする
                        // 1件のメールサイズの大きさのbyte型配列を確保
                        var bs = mail.Body.Select(c => (byte)c).ToArray();

                        // GetStringでバイト型配列をUTF-8の配列にエンコードする
                        attachMailBody = Encoding.UTF8.GetString(bs);
                        this.textBody.Text = attachMailBody;
                    }
                    else {
                        // テキストボックスに出力する文字コードをJISに変更する
                        byte[] b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.Body);
                        string strBody = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                        // 本文をテキストとして表示する
                        this.textBody.Text = strBody;
                    }
                }
            }
        }

        private string BreakLine(string text)
        {
            if (text.Contains("\n\n")) {
                text = text.Replace("\n\n", "\r\n").Replace("\n", "\r\n");
            }
            return text;
        }

        private IEnumerable<ToolStripItem> GenerateMenuItem(string rootPath, IEnumerable<string> attaches, bool enableWhenRemoved = false)
        {
            foreach (var attachFile in attaches)
            {
                if (File.Exists(rootPath + attachFile))
                {
                    yield return new ToolStripMenuItem
                    {
                        Text = attachFile,
                        Image = System.Drawing.Icon.ExtractAssociatedIcon(rootPath+attachFile).ToBitmap()
                    };
                }
                else
                {
                    yield return new ToolStripMenuItem
                    {
                        Text = attachFile + "は削除されています。",
                        Enabled = enableWhenRemoved
                    };
                }
            }
        }
        private IEnumerable<ToolStripItem> GenerateMenuItem(IEnumerable<string> attaches, bool enableWhenRemoved = false)
        {
            return GenerateMenuItem("", attaches, enableWhenRemoved);
        }

        private string EncodeBody(Mail mail, nMail.Attachment attach)
        {
            // ISO-2022-JPでかつquoted-printableがある場合(nMail.dllが対応するまでの暫定処理)
            if (attach.GetHeaderField("Content-Type:", mail.Header).ToLower().Contains("iso-2022-jp") && attach.GetHeaderField("Content-Transfer-Encoding:", mail.Header).Contains("quoted-printable")) {
                // 文章をデコードする
                Options.EnableDecodeBody();

                // ヘッダと本文付きの文字列を添付クラスに追加する
                attach.Add(mail.Header, mail.Body);

                // 添付ファイルを取り外す
                attach.Save();

                return BreakLine(attach.Body);
            }
            else if (attach.GetHeaderField("X-NMAIL-BODY-UTF8:", mail.Header).Contains("8bit")) {
                // Unicode化されたUTF-8文字列をデコードする
                // 1件のメールサイズの大きさのbyte型配列を確保
                var bs = mail.Body.Select(c => (byte)c).ToArray();

                // GetStringでバイト型配列をUTF-8の配列にエンコードする
                return Encoding.UTF8.GetString(bs);
                }
            else {
                // テキストボックス .に出力する文字コードをJISに変更する
                byte[] b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.Body);
                return Encoding.GetEncoding("iso-2022-jp").GetString(b);
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
            NewMailForm.SendList = mailBox.Send.ToList();

            // 返信のための宛先・件名を設定する
            NewMailForm.textAddress.Text = mail.Address;
            NewMailForm.textSubject.Text = "Re:" + mail.Subject;

            // 添付なしメールのときはbodyを渡す
            if (!attachMailReplay) {
                // UTF-8でエンコードされたメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.Address + " was wrote ---\r\n\r\n" + attachMailBody;
                }
                else {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.Address + " was wrote ---\r\n\r\n" + mail.Body;
                }
            }
            else {
                // 添付付きメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.Address + " was wrote ---\r\n\r\n" + attachMailBody;
                }
                else {
                    NewMailForm.textBody.Text = "\r\n\r\n---" + mail.Address + " was wrote ---\r\n\r\n" + mail.Body;
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
            MailEditorForm NewMailForm = new MailEditorForm();

            // 親フォームをForm1に設定する
            NewMailForm.MainForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.SendList = mailBox.Send.ToList();

            // 転送のために件名を設定する(件名は空白にする)
            NewMailForm.textAddress.Text = "";
            NewMailForm.textSubject.Text = "Fw:" + mail.Subject;

            NewMailForm.textBody.Text = BuildForwardingBody(mail);

            // 送信メールで添付ファイルがあるとき
            if (mail.Attaches.Length != 0) {
                // 添付リストメニューを表示
                NewMailForm.buttonAttachList.Visible = true;
                // 添付ファイルの数だけメニューを追加する
                NewMailForm.buttonAttachList.DropDownItems.AddRange(GenerateMenuItem(mail.Attaches).ToArray());
                }
            else if (this.buttonAttachList.Visible) {
                // 受信メールで添付ファイルがあるとき
                // 添付リストメニューを表示
                NewMailForm.buttonAttachList.Visible = true;

                // 添付ファイルの数だけメニューを追加する
                var attaches = this.buttonAttachList.DropDownItems.Cast<ToolStripItem>()
                    .Select(i => Application.StartupPath + @"\tmp\" + i.Text);
                NewMailForm.buttonAttachList.DropDownItems.AddRange(GenerateMenuItem(attaches).ToArray());
                    }

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

        private string BuildForwardingBody(Mail mail)
        {
            var from = "";
            var to = "";
            var sentAt = "";
            var subject = "";

            var atch = new nMail.Attachment();

            // メールヘッダが存在するとき
            if (mail.Header != "") {
                from = atch.GetHeaderField("From:", mail.Header);
                to = atch.GetHeaderField("To:", mail.Header);
                sentAt = atch.GetHeaderField("Date:", mail.Header);
                subject = atch.GetHeaderField("Subject:", mail.Header);
            }
            else {
                from = AccountInfo.mailAddress;
                to = mail.Address;
                sentAt = mail.Date;
                subject = mail.Subject;
            }

            var builder = new StringBuilder("\r\n\r\n");
            builder
                .AppendLine("--- Forwarded by " + AccountInfo.mailAddress + " ---")
                .AppendLine("----------------------- Original Message -----------------------")
                .AppendLine("From: " + from)
                .AppendLine("To: " + to)
                .AppendLine("Date: " + sentAt)
                .AppendLine(" Subject:" + subject)
                .AppendLine("----\r\n");

            // 添付なしメールのときはbodyを渡す
            if (!attachMailReplay) {
                // NewMailForm.textBody.Text = "\r\n\r\n--- Forwarded by " + Mail.mailAddress + " ---\r\n" + fwHeader + mail.body;
                // UTF-8でエンコードされたメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    return builder.Append(attachMailBody).ToString();
                }
                else {
                    // JISコードなどのメールは通常のbodyを渡す
                    return builder.Append(mail.Body).ToString();
                }
            }
            else {
                // 添付付きメールのときはattachMailBodyを渡す
                if (attachMailBody != "") {
                    return builder.Append(attachMailBody).ToString();
                }
                else {
                    return builder.Append(mail.Body).ToString();
                }
            }
        }

        private void InitializeMailEditorForm(Mail mail, int tag, MainForm mainForm)
        {
            // 1番目のカラムが宛先のときは編集画面を表示する
            MailEditorForm EditMailForm = new MailEditorForm();

            // 親フォームをForm1に設定する
            EditMailForm.MainForm = mainForm;

            // 親フォームにタイトルを設定する
            EditMailForm.Text = mail.Subject + " - Akane Mail";

            // 送信箱の配列をForm3に渡す
            EditMailForm.SendList = mailBox.Send.ToList();
            EditMailForm.ListTag = tag;
            EditMailForm.IsEdit = true;

            // 宛先、件名、本文をForm3に渡す
            EditMailForm.textAddress.Text = mail.Address;
            EditMailForm.textCc.Text = mail.Cc;
            EditMailForm.textBcc.Text = mail.Bcc;
            EditMailForm.textSubject.Text = mail.Subject;
            EditMailForm.textBody.Text = mail.Body;

            // 重要度をForm3に渡す
            if (mail.Priority == MailPriority.Urgent) {
                EditMailForm.comboPriority.SelectedIndex = 0;
            }
            else if (mail.Priority == MailPriority.Normal) {
                EditMailForm.comboPriority.SelectedIndex = 1;
            }
            else {
                EditMailForm.comboPriority.SelectedIndex = 2;
            }

            // 添付ファイルが付いているとき
            if (mail.Attaches.Length != 0) {
                // 添付リストメニューを表示
                EditMailForm.buttonAttachList.Visible = true;
                // 添付ファイルの数だけメニューを追加する
                EditMailForm.buttonAttachList.DropDownItems.AddRange(GenerateMenuItem(mail.Attaches, true).ToArray());
            }

            // メール編集フォームを表示する
            EditMailForm.Show();
        }

        /// <summary>
        /// メールの編集
        /// </summary>
        /// <param name="mail">メール</param>
        /// <param name="item">リストアイテム</param>
        private void EditMail(Mail mail, ListViewItem item)
        {
            // 1番目のカラムの表示が差出人か差出人または宛先のとき
            if (listMail.Columns[0].Text == "差出人" || listMail.Columns[0].Text == "差出人または宛先") {
                mail.NotReadYet = false;

                ReforcusListView(listMail);

                // データ変更フラグをtrueにする
                dataDirtyFlag = true;
            }
            else if (listMail.Columns[0].Text == "宛先") {
                InitializeMailEditorForm(mail, (int)item.Tag, this);
            }
        }

        /// <summary>
        /// メールを削除
        /// </summary>
        private void DeleteMail()
        {
            int nSel = listMail.SelectedItems[0].Index;

            if (listMail.Columns[0].Text == "差出人") {
                // 受信メールのとき
                // 選択アイテムの数を取得
                Trash(mailBox.Receive);
            }
            else if (listMail.Columns[0].Text == "宛先") {
                // 送信メールのとき
                Trash(mailBox.Send);
            }
            else if (listMail.Columns[0].Text == "差出人または宛先") {
                if (MessageBox.Show("選択されたメールは完全に削除されます。\nよろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                    // 削除メールのとき
                    TrashCompletely();
                }
            }

            ClearInput();

            var after = Math.Min(nSel, listMail.Items.Count) - 1;

            // リストが空でないとき
            if (after >= 0) {
                // フォーカスをnSelの行に当てる
                listMail.Items[after].Selected = true;
                listMail.Items[after].Focused = true;
                listMail.SelectedItems[0].EnsureVisible();
                listMail.Select();
                listMail.Focus();
            }
            // データ変更フラグをtrueにする
            dataDirtyFlag = true;
        }

        public void Trash(MailFolder fromFolder)
        {
            // 受信メールのとき
            // 選択アイテムの数を取得
            int nLen = listMail.SelectedItems.Count;

            if (nLen == 0)
                return;

            // 選択アイテムのキーを取得
            var nIndices = Enumerable.Range(0, nLen)
                .Select(i => listMail.SelectedItems[i].Name)
                .Select(t => int.Parse(t))
                .OrderBy(i => i)
                .ToArray();

            // キーの並べ替え
            while (nLen > 0) {
                // 選択アイテムのキーから 選択アイテム群の位置を取得
                var nIndex = listMail.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                var item = listMail.SelectedItems[nIndex];

                // 元リストからメールアイテムを取得
                var mail = fromFolder[nIndices[nLen - 1]];

                if (item.SubItems[1].Text == mail.Subject) {
                    mailBox.Trash.Add(mail);
                    fromFolder.Remove(mail);
                }
                nLen--;
            }
        }

        public void TrashCompletely()
        {
            // 選択アイテムの数を取得
            int nLen = listMail.SelectedItems.Count;

            if (nLen == 0)
                return;

            // 選択アイテムのキーを取得
            var nIndices = Enumerable.Range(0, nLen).Select(n => int.Parse(listMail.SelectedItems[n].Name)).ToArray();

            // キーの並べ替え
            Array.Sort(nIndices);
            var dList = mailBox.Trash;

            while (nLen > 0) {
                // 選択アイテムのキーから 選択アイテム群の位置を取得
                int nIndex = listMail.SelectedItems.IndexOfKey(nIndices[nLen - 1].ToString());
                ListViewItem item = listMail.SelectedItems[nIndex];

                // 元リストからメールアイテムを取得
                Mail mail = dList[nIndices[nLen - 1]];

                if (item.SubItems[1].Text == mail.Subject) {
                    dList.Remove(mail);
                }
                nLen--;
            }
        }

        private IEnumerable<string> QueryUnreadMailUids(nMail.Pop3 pop, IEnumerable<Mail> locals)
        {
            foreach (var i in Enumerable.Range(1, pop.Count))
            {
                pop.GetUidl(i);
                var uidl = pop.Uidl;
                if (locals.Any(m => m.Uidl == uidl))
                {
                    yield return uidl;
                }
            }
        }

        /// <summary>
        /// POP3サーバからメールを受信する
        private void RecieveMail()
        {
            int mailCount = 0;              // 未受信メール件数

            try {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール受信中";

                // POP3のセッションを作成する;
                using (var pop = new nMail.Pop3()) {
                    // POP3への接続タイムアウト設定をする
                    Options.EnableConnectTimeout();

                    // APOPを使用するときに使うフラグ
                    pop.APop = AccountInfo.apopFlag;

                    // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                    if (AccountInfo.popOverSSL) {
                        pop.SSL = nMail.Pop3.SSL3;
                        pop.Connect(AccountInfo.popServer, AccountInfo.popPortNumber);
                    }
                    else {
                        // POP3へ接続する
                        pop.Connect(AccountInfo.popServer, AccountInfo.popPortNumber);
                    }

                    // POP3への認証処理を行う
                    pop.Authenticate(AccountInfo.userName, AccountInfo.passWord);


                    // 未受信のメールが何件あるかチェックする
                    var countMail = new Task<int>(() =>
                    {
                        var locals = mailBox.Receive.Union(mailBox.Trash);
                        return QueryUnreadMailUids(pop, locals).Count();
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
                        Invoke(EnableButton, true);
                        return;
                    }

                    var receivedCount = countMail.Result;
                    // 受信済みメールカウントがPOP3サーバ上にあるメール件数と同じとき
                    if (receivedCount == pop.Count) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "新着のメッセージはありませんでした。";

                        // プログレスバーを非表示に戻す
                        Invoke(ProgressMailDisable);

                        // メール受信のメニューとツールボタンを有効化する
                        Invoke(EnableButton, true);

                        return;
                    }

                    // プログレスバーを表示して最大値を未受信メール件数に設定する
                    int mailCountMax = pop.Count - receivedCount;
                    Invoke(ProgressMailInit, mailCountMax);

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
                        Mail mail = new Mail(pop.From, pop.Header, pop.Subject, pop.Body, pop.FileName, pop.DateString, pop.Size.ToString(), pop.Uidl, true, "", pop.GetDecodeHeaderField("Cc:"), "", MailPriority.Parse(pop.Header));
                        mailBox.Receive.Add(mail);

                        // 受信メールの数を増加する
                        mailCount++;

                        // メール受信時にPOP3サーバ上のメール削除のチェックがある時はPOP3サーバからメールを削除する
                        if (AccountInfo.deleteMail) {
                            pop.Delete(no);
                        }

                        // メールの受信件数を更新する
                        Invoke(ProgressMailUpdate, mailCount);

                        // スレッドを1秒間待機させる
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                // プログレスバーを非表示に戻す
                Invoke(ProgressMailDisable);

                // メール受信のメニューとツールボタンを有効化する
                Invoke(EnableButton, true);

                // 未受信メールが1件以上の場合
                if (mailCount >= 1) {
                    // メール着信音の設定をしている場合
                    if (AccountInfo.popSoundFlag && AccountInfo.popSoundName != "") {
                        SoundPlayer sndPlay = new SoundPlayer(AccountInfo.popSoundName);
                        sndPlay.Play();
                    }

                    // ウィンドウが最小化でタスクトレイに格納されていて何分間隔かで受信をするとき
                    if (this.WindowState == FormWindowState.Minimized && AccountInfo.minimizeTaskTray && AccountInfo.autoMailFlag) {
                        notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                        notifyIcon1.BalloonTipTitle = "新着メール";
                        notifyIcon1.BalloonTipText = mailCount + "件の新着メールを受信しました。";
                        notifyIcon1.ShowBalloonTip(300);
                    }
                    else {
                        // 画面をフラッシュさせる
                        Invoke(FlashWindow, this);

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
                    Invoke(EnableButton, true);

                    return;
                }
            }
            catch (nMail.nMailException nex) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                // メール受信のメニューとツールボタンを有効化する
                Invoke(EnableButton, true);

                return;
            }
            catch (Exception exp) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーメッセージ:" + exp.Message;

                // メール受信のメニューとツールボタンを有効化する
                Invoke(EnableButton, true);

                return;
            }

            // TreeViewとListViewの更新を行う
            Invoke(UpdateView, 0);

        }

        /// <summary>
        /// メールを送信する
        /// </summary>
        private void SendMail()
        {

            // 送信可能なメールの数を確認する
            var max_no = mailBox.Send.Count(m => m.NotReadYet);

            // 送信可能なメールが存在しないとき
            if (max_no == 0) {
                Invoke(EnableButton, true);
                return;
            }

            try {
                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信中";

                // プログレスバーを表示して最大値を未送信メール件数に設定する
                Invoke(ProgressMailInit, max_no);

                // POP before SMTPが有効の場合
                if (AccountInfo.popBeforeSMTP) {
                    try {
                        using (var pop = new nMail.Pop3()) {
                            Options.EnableConnectTimeout();

                            pop.APop = AccountInfo.apopFlag;

                            if (AccountInfo.popOverSSL) {
                                pop.SSL = nMail.Pop3.SSL3;
                            }
                            pop.Connect(AccountInfo.popServer, AccountInfo.popPortNumber);
                            pop.Authenticate(AccountInfo.userName, AccountInfo.passWord);
                        }
                    }
                    catch (nMail.nMailException nex) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                        // メール送信・受信のメニューとツールボタンを有効化する
                        Invoke(EnableButton, true);

                        return;
                    }
                    catch (Exception exp) {
                        // ステータスバーに状況表示する
                        labelMessage.Text = "エラーメッセージ:" + exp.Message;

                        // メール送信・受信のメニューとツールボタンを有効化する
                        Invoke(EnableButton, true);

                        return;
                    }
                }

                // SMTPのセッションを作成する
                using (var smtp = new nMail.Smtp(AccountInfo.smtpServer)) {
                    smtp.Port = AccountInfo.smtpPortNumber;

                    // SMTP認証フラグが有効の時はSMTP認証を行う
                    if (AccountInfo.smtpAuth) {
                        // SMTPサーバに接続
                        smtp.Connect();

                        // SMTP認証を行う
                        smtp.Authenticate(AccountInfo.userName, AccountInfo.passWord, Smtp.AuthPlain | Smtp.AuthCramMd5);
                    }
                    var send_no = 0;
                    foreach (var mail in mailBox.Send) {
                        if (mail.NotReadYet) {
                            // CCが存在するとき
                            if (mail.Cc != "") {
                                // CCの宛先を設定する
                                smtp.Cc = mail.Cc;
                            }

                            // BCCが存在するとき
                            if (mail.Bcc != "") {
                                // BCCの宛先を設定する
                                smtp.Bcc = mail.Bcc;
                            }

                            // 添付ファイルを指定している場合
                            if (mail.Attach != "") {
                                smtp.FileName = mail.Attach;
                            }

                            // 追加ヘッダをつける
                            smtp.Header = "\r\nPriority: " + mail.Priority + "\r\nX-Mailer: Akane Mail Version " + Application.ProductVersion;

                            // 差出人のアドレスを編集する
                            string fromAddress = AccountInfo.FromAddress;

                            // 送信する
                            smtp.SendMail(mail.Address, fromAddress, mail.Subject, mail.Body);

                            // 送信日時を設定する
                            mail.Date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

                            // 送信済みに変更する
                            mail.NotReadYet = false;

                            // メールの送信件数を更新する
                            send_no++;
                            Invoke(ProgressMailUpdate, send_no);

                            // スレッドを1秒間待機させる
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }

                // プログレスバーを非表示に戻す
                Invoke(ProgressMailDisable);

                // ボタンとメニューを有効化する
                Invoke(EnableButton, true);

                // ステータスバーに状況表示する
                labelMessage.Text = "メール送信完了";
            }
            catch (nMail.nMailException nex) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーNo:" + nex.ErrorCode + " エラーメッセージ:" + nex.Message;

                // メール送信・受信のメニューとツールボタンを有効化する
                Invoke(EnableButton, true);

                return;
            }
            catch (Exception exp) {
                // ステータスバーに状況表示する
                labelMessage.Text = "エラーメッセージ:" + exp.Message;

                // メール送信・受信のメニューとツールボタンを有効化する
                Invoke(EnableButton, true);

                return;
            }

            // TreeViewとListViewの更新を行う
            Invoke(UpdateView, 1);

        }

        /// <summary>
        /// 直接メール送信
        /// </summary>
        /// <param name="Address">宛先</param>
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
                if (AccountInfo.popBeforeSMTP) {
                    try {
                        // POP3のセッションを作成する
                        using (var pop = new Pop3()) {
                            // POP3への接続タイムアウト設定をする
                            Options.EnableConnectTimeout();

                            // APOPを使用するときに使うフラグ
                            pop.APop = AccountInfo.apopFlag;

                            // POP3 over SSL/TLSフラグが有効のときはSSLを使用する
                            if (AccountInfo.popOverSSL) {
                                pop.SSL = Pop3.SSL3;
                                pop.Connect(AccountInfo.popServer, AccountInfo.popPortNumber);
                            }
                            else {
                                // POP3へ接続する
                                pop.Connect(AccountInfo.popServer, AccountInfo.popPortNumber);
                            }

                            // POP3への認証処理を行う
                            pop.Authenticate(AccountInfo.userName, AccountInfo.passWord);
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
                using (var smtp = new Smtp(AccountInfo.smtpServer)) {
                    smtp.Port = AccountInfo.smtpPortNumber;

                    // SMTP認証フラグが有効の時はSMTP認証を行う
                    if (AccountInfo.smtpAuth) {
                        // SMTPサーバに接続
                        smtp.Connect();
                        // SMTP認証を行う
                        smtp.Authenticate(AccountInfo.userName, AccountInfo.passWord, Smtp.AuthPlain | Smtp.AuthCramMd5);
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
                    string fromAddress = AccountInfo.FromAddress;

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
                        writer.Write(mail.Header);
                        writer.Write("\r\n");
                        writer.Write(mail.Body);
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
            string enc = Mail.ParseEncoding(mail.Header);

            // 出力する文字コードがUTF-8ではないとき
            if (enc.ToLower().Contains("iso-") || enc.ToLower().Contains("shift_") || enc.ToLower().Contains("euc") || enc.ToLower().Contains("windows")) {
                // 出力するヘッダをUTF-8から各文字コードに変換する
                Byte[] b = Encoding.GetEncoding(enc).GetBytes(mail.Header);
                fileHeader = Encoding.GetEncoding(enc).GetString(b);

                // 出力する本文をUTF-8から各文字コードに変換する
                b = Encoding.GetEncoding(enc).GetBytes(mail.Body);
                fileBody = Encoding.GetEncoding(enc).GetString(b);
            }
            else if (enc.ToLower().Contains("utf-8") || mail.Header.Contains("X-NMAIL-BODY-UTF8: 8bit")) {
                // text/plainまたはmultipart/alternativeでUTF-8でエンコードされたメールのとき
                // nMail.dllはUTF-8エンコードのメールを8bit単位に分解してUncode(16bit)扱いで格納している。
                // これはUnicodeで文字列を受け取る関数内で生のUTF-8の文字列を受け取っておかしなことに
                // なるのを防ぐための意図で行われている。
                // これをデコードするにはバイト型で格納し、UTF-8でデコードし直せば文字化けのような文字列を
                // 可読化することができる。

                // Byte型構造体に変換する
                var bs = mail.Body.Select(s => (byte)s).ToArray();

                // GetStringでバイト型配列をUTF-8の配列にエンコードする
                fileBody = Encoding.UTF8.GetString(bs);

                // fileHeaderにヘッダを格納する
                fileHeader = mail.Header;

                // ファイル出力フラグにUTF-8を設定する
                enc = "utf-8";
            }
            else {
                // ここに落ちてくるのは基本的に添付ファイルのみ
                Byte[] b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.Header);
                fileHeader = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.Body);
                fileBody = Encoding.GetEncoding("iso-2022-jp").GetString(b);

                // 文字コードをJISに設定する
                enc = "iso-2022-jp";
            }

            // ファイル書き込み用のエンコーディングを取得する
            Encoding writeEnc = Encoding.GetEncoding(enc);

            using (var writer = new StreamWriter(FileToSave, false, writeEnc)) {
                // 受信メール(ヘッダが存在する)のとき
                if (mail.Header.Length > 0) {
                    writer.Write(fileHeader);
                    writer.Write("\r\n");
                }
                else {
                    // 送信メールのときはヘッダの代わりに送り先と件名を出力
                    writer.WriteLine("To: " + mail.Address);
                    writer.WriteLine("Subject: " + mail.Subject);
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
                    return mailBox.Receive[(int)index];
                case "宛先":
                    return mailBox.Send[(int)index];
                case "差出人または宛先":
                    return mailBox.Trash[(int)index];
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
            listMail.ListViewItemSorter = listViewItemSorter;

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
            listMail.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemSorterを指定する
            listMail.ListViewItemSorter = listViewItemSorter;
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
            listMail.Columns[0].Text = col1;
            listMail.Columns[1].Text = col2;
            listMail.Columns[2].Text = col3;
            listMail.Columns[3].Text = col4;
        }

        #region "Event Listeners"
        private void treeMailBoxFolder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // ノードに付けたタグからリストビューのカラムを変更
            switch (e.Node.Tag.ToString()) {
                case "MailBoxRoot":
                    // メールボックスが選択された場合
                    SetListViewColumns("名前", "メールアドレス", "最終データ更新日", "データサイズ");
                    labelMessage.Text = "メールボックス";
                    listMail.ContextMenuStrip = null;
                    break;
                case "ReceiveMailBox":
                    // 受信メールが選択された場合
                    SetListViewColumns("差出人", "件名", "受信日時", "サイズ");
                    labelMessage.Text = "受信メール";
                    listMail.ContextMenuStrip = menuListView;
                    break;
                case "SendMailBox":
                    // 送信メールが選択された場合
                    SetListViewColumns("宛先", "件名", "送信日時", "サイズ"); ;
                    labelMessage.Text = "送信メール";
                    listMail.ContextMenuStrip = menuListView;
                    break;
                case "DeleteMailBox":
                    // ごみ箱が選択された場合
                    SetListViewColumns("差出人または宛先", "件名", "受信日時または送信日時", "サイズ");
                    labelMessage.Text = "ごみ箱";
                    listMail.ContextMenuStrip = menuListView;
                    break;
                default:
                    break;
            }

            // 画面設定をクリアする。
            ClearInput();
        }

        private void menuAppExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuSetEnv_Click(object sender, EventArgs e)
        {
            SettingForm Form2 = new SettingForm();

            timerAutoReceive.Enabled = false;

            DialogResult ret = Form2.ShowDialog();

            if (ret == DialogResult.OK) {
                if (Form2.checkAutoGetMail.Checked) {
                    timerAutoReceive.Interval = AccountInfo.getMailInterval * 60000;
                    timerAutoReceive.Enabled = true;
                }
                else {
                    timerAutoReceive.Enabled = false;
                }
            }
            // ListViewItemSorterを解除する
            listMail.ListViewItemSorter = null;

            // リストビューを更新する
            UpdateListView();

            // ListViewItemSorterを指定する
            listMail.ListViewItemSorter = listViewItemSorter;
        }

        private void timerStatusTime_Tick(object sender, EventArgs e)
        {
            // 現在の時刻を表示する
            labelDate.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
        }

        private void menuNewMail_Click(object sender, EventArgs e)
        {
            MailEditorForm NewMailForm = new MailEditorForm();

            // 親フォームをForm1に設定する
            NewMailForm.MainForm = this;

            // 送信箱の配列をForm3に渡す
            NewMailForm.SendList = mailBox.Send.ToList();

            // メール新規作成フォームを表示する
            NewMailForm.Show();
        }

        private void menuSendMail_Click(object sender, EventArgs e)
        {
            // メール送信・受信のメニューとツールボタンを無効化する
            menuRecieveMail.Enabled = false;
            buttonRecieveMail.Enabled = false;
            buttonRecieveMail.Enabled = false;
            buttonSendMail.Enabled = false;

            // Threadオブジェクトを作成する
            System.Threading.Thread ts = new System.Threading.Thread(new System.Threading.ThreadStart(SendMail));

            // メール送信スレッドを開始する
            ts.Start();
        }

        private void menuRecieveMail_Click(object sender, EventArgs e)
        {
            // メール受信のメニューとツールボタンを無効化する
            menuRecieveMail.Enabled = false;
            buttonRecieveMail.Enabled = false;

            // Threadオブジェクトを作成する
            System.Threading.Thread tr = new System.Threading.Thread(new System.Threading.ThreadStart(RecieveMail));

            // メール受信スレッドを開始する
            tr.Start();
        }

        private void menuDeleteMail_Click(object sender, EventArgs e)
        {
            // メールを削除
            DeleteMail();
        }

        // 現在リストビューに表示されているメールボックスを取得。
        private MailFolder GetShowingMailFolder()
        {
            switch (listMail.Columns[0].Text) {
                case "差出人":
                    return mailBox.Receive;
                case "宛先":
                    return mailBox.Send;
                case "差出人または宛先":
                    return mailBox.Trash;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 未読メールを既読にする
        /// </summary>
        private void menuAlreadyRead_Click(object sender, EventArgs e)
        {
            var sList = GetShowingMailFolder();

            // 選択アイテムの数を取得
            int nLen = listMail.SelectedItems.Count;

            // 選択アイテムの数が0のとき
            if (nLen == 0)
                return;

            // 選択アイテムのキーを取得
            var nIndices = Enumerable.Range(0, nLen).Select(i => listMail.SelectedItems[i].Name).Select(t => int.Parse(t)).OrderBy(i => i).ToArray();

            while (nLen > 0) {
                nLen--;
                // 選択アイテムのキーから 選択アイテム群の位置を取得
                int nIndex = listMail.SelectedItems.IndexOfKey(nIndices[nLen].ToString());
                ListViewItem item = listMail.SelectedItems[nIndex];

                // 元リストからメールアイテムを取得
                Mail mail = sList[nIndices[nLen]];

                sList[nIndices[nLen]].NotReadYet = !(item.SubItems[1].Text == mail.Subject);
            }

            ReforcusListView(listMail);

            // データ変更フラグをtrueにする
            dataDirtyFlag = true;
        }

        /// <summary>
        /// 既読メールを未読にする
        /// </summary>
        private void menuNotReadYet_Click(object sender, EventArgs e)
        {
            var sList = GetShowingMailFolder();

            // 選択アイテムの数を取得
            int nLen = listMail.SelectedItems.Count;

            // 選択アイテムの数が0のとき
            if (nLen == 0)
                return;

            // 選択アイテムのキーを取得
            var nIndices = Enumerable.Range(0, nLen).Select(i => listMail.SelectedItems[i].Name).Select(t => int.Parse(t)).OrderBy(i => i).ToArray();

            while (nLen > 0) {
                nLen--;
                // 選択アイテムのキーから 選択アイテム群の位置を取得
                int nIndex = listMail.SelectedItems.IndexOfKey(nIndices[nLen].ToString());
                ListViewItem item = listMail.SelectedItems[nIndex];

                // 元リストからメールアイテムを取得
                Mail mail = sList[nIndices[nLen]];

                sList[nIndices[nLen]].NotReadYet = item.SubItems[1].Text == mail.Subject;
            }

            ReforcusListView(listMail);

            // データ変更フラグをtrueにする
            dataDirtyFlag = true;
        }

        private void listMail_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // 選択された行を取得する
            currentRow = e.ItemIndex;
        }

        private void listMail_DoubleClick(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listMail.SelectedItems[0];

            // メールボックスのときは反応しない
            if (listMail.Columns[0].Text == "名前") {
                return;
            }

            mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // メールの編集
            EditMail(mail, item);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ファイル展開用のテンポラリフォルダの削除
            if (Directory.Exists(Application.StartupPath + @"\tmp")) {
                Directory.Delete(Application.StartupPath + @"\tmp", true);
            }

            if (!errorFlag && dataDirtyFlag) {
                // データファイルを削除する
                if (File.Exists(Application.StartupPath + @"\Mail.dat")) {
                    File.Delete(Application.StartupPath + @"\Mail.dat");
                }

                // Threadオブジェクトを作成する
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(mailBox.MailDataSave));

                // スレッドを開始する
                t.Start();

                // スレッドが終了するまで待機
                t.Join();
            }

            // 環境設定の書き出し
            SaveSettings();

        }

        private void MainForm_Load(object sender, EventArgs e)
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
                    MessageBox.Show("64bit版OSで32bit版OS用のnMail.dllを使用して実行した場合\nこのエラーが表示されます。\n\nお手数をお掛け致しますが同梱のnMail.dllをnMail.dll.32、nMail.dll.64をnMail.dllに名前を変更してAkane Mailを起動\nしてください。", "Akane Mail", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try {
                // Threadオブジェクトを作成する
                var t = new System.Threading.Thread(new System.Threading.ThreadStart(mailBox.MailDataLoad));

                splash.ProgressMsg = "メールデータの読み込み作業中です";

                // スレッドを開始する
                t.Start();

                // スレッドが終了するまで待機
                t.Join();
            }
            catch (MailLoadException) {
                errorFlag = true;
            }

            // メール自動受信が設定されている場合はタイマーを起動する
            if (AccountInfo.autoMailFlag) {
                // 取得間隔*60000(60000ミリ秒=1分)をタイマー実行間隔に設定する
                timerAutoReceive.Interval = AccountInfo.getMailInterval * 60000;
                timerAutoReceive.Enabled = true;
            }

            // ツリービューとリストビューの表示を更新する
            UpdateTreeView();
            UpdateListView();

            // ListViewItemComparerの作成と設定
            // 受信or送信日時の降順で並べる
            listViewItemSorter = new ListViewItemComparer() { Column = 2, Order = SortOrder.Descending };
            listViewItemSorter.ColumnModes = new ListViewItemComparer.ComparerMode[] { ListViewItemComparer.ComparerMode.String, ListViewItemComparer.ComparerMode.String, ListViewItemComparer.ComparerMode.DateTime, ListViewItemComparer.ComparerMode.Integer };

            // ListViewItemSorterを指定する
            listMail.ListViewItemSorter = listViewItemSorter;

            // スプラッシュ・スクリーンの表示終了
            splash.Close();
            if (!splash.IsDisposed)
                splash.Dispose();

            // 一時的に非表示にした画面を表示させる
            if (!(AccountInfo.minimizeTaskTray && WindowState == FormWindowState.Minimized)) {
                ShowInTaskbar = true;
                this.Show();
            }

            // ツリービューを展開する
            treeMailBoxFolder.ExpandAll();

            // メインとなるフォームをアクティブに戻す
            this.Activate();

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Appliction.Idleを削除する
            Application.Idle -= Application_Idle;

            // 致命的なnMail.dllエラーがないとき
            if (!nMailError) {
                // WinSockを開放する
                nMail.Winsock.Done();
            }
        }

        private void menuReplyMail_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listMail.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listMail.SelectedItems.Count == 0) {
                return;
            }

            // 表示機能はシンプルなものに変わる
            mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // 返信メールを作成する
            CreateReturnMail(mail);
        }

        private void listMail_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listMail.SelectedItems[0];

            // メールボックスのときは反応しない
            if (listMail.Columns[0].Text == "名前") {
                return;
            }

            // リストビューで選択したメールデータを取得
            mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // メールを開く
            OpenMail(mail);
        }

        private void menuGetAttatch_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listMail.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listMail.SelectedItems.Count == 0) {
                return;
            }

            // 送信メール以外も展開できるように変更
            mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // 添付ファイルを展開する
            GetAttachMail(mail);
        }

        private void menuSaveMailFile_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            // ListViewから選択した1行の情報をitemに格納する
            ListViewItem item = listMail.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listMail.SelectedItems.Count == 0) {
                return;
            }

            // どの項目でも保存できるように変更
            mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // ファイル名にメールの件名を入れる
            saveFileDialog1.FileName = mail.Subject;

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

        private void menuClearTrush_Click(object sender, EventArgs e)
        {
            // ごみ箱の配列を空っぽにする
            if (MessageBox.Show("ごみ箱の中身をすべて削除します。\nよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                mailBox.Trash.Clear();

                ClearInput();

                // データ変更フラグをtrueにする
                dataDirtyFlag = true;
            }
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            // バージョン情報を表示する
            AboutForm AboutForm = new AboutForm();
            AboutForm.ShowDialog();
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ListViewItem item = listMail.SelectedItems[0];

            var mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // ファイルを開くかの確認をする
            if (MessageBox.Show(e.ClickedItem.Text + "を開きますか？\nファイルによってはウイルスの可能性もあるため\n注意してファイルを開いてください。", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) {
                // 受信されたメールのとき
                if (mail.Attach.Length == 0) {
                    System.Diagnostics.Process.Start(Application.StartupPath + @"\tmp\" + e.ClickedItem.Text);
                }
                else {
                    // 送信メールのとき
                    System.Diagnostics.Process.Start(e.ClickedItem.Text);
                }
            }
        }

        private void listMail_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // メールボックスのときはソートしない
            if (listMail.Columns[0].Text == "名前")
                return;

            // クリックされた列を設定
            listViewItemSorter.Column = e.Column;

            // 並び替える
            listMail.Sort();
        }

        private void timerAutoReceive_Tick(object sender, EventArgs e)
        {
            menuRecieveMail_Click(sender, e);
        }

        private void browserBody_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            browserBody.AllowNavigation = false;
        }

        private void MainForm_ClientSizeChanged(object sender, EventArgs e)
        {
            // 最小化時にタスクトレイに格納フラグがtrueでウィンドウが最小化されたとき
            if (this.WindowState == FormWindowState.Minimized && AccountInfo.minimizeTaskTray) {
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
            var condition = listMail.SelectedItems.Count == 1 && listMail.Columns[0].Text != "名前";
            menuSaveMailFile.Enabled = condition;
            menuGetAttatch.Enabled = condition;
            menuGetAttatch.Enabled = condition && attachMenuFlag;

            // 削除メールが0件の場合
            menuClearTrush.Enabled = mailBox.Trash.Count != 0;
        }

        private void menuMail_DropDownOpening(object sender, EventArgs e)
        {

            if (listMail.Columns[0].Text == "名前") {
                menuDeleteMail.Enabled = false;
                menuReplyMail.Enabled = false;
                menuFowerdMail.Enabled = false;
            }
            else {
                menuDeleteMail.Enabled = listMail.SelectedItems.Count > 0;
                menuReplyMail.Enabled = listMail.SelectedItems.Count == 1;
                menuFowerdMail.Enabled = listMail.SelectedItems.Count == 1;
            }
        }

        private void menuListView_Opening(object sender, CancelEventArgs e)
        {
            // メールの選択件が1かつメールボックスのとき
            var condition = listMail.Columns[0].Text != "名前" && listMail.SelectedItems.Count == 1;
            menuContextReplyMail.Enabled = condition;
            menuContextFowerdMail.Enabled = condition;
            menuContextGetAttatch.Enabled = condition && attachMenuFlag;

            // メールの選択数が0またはメールボックスのとき
            if (listMail.SelectedItems.Count == 0 || listMail.Columns[0].Text == "名前") {
                menuContextDeleteMail.Enabled = false;
            }
            else if (listMail.SelectedItems.Count == 1) {
                // メールが1件選択されたとき
                menuContextDeleteMail.Enabled = true;
            }

            // メールが既読で、メールボックス以外で何かが選択されているとき
            menuNotReadYet.Enabled = listMail.SelectedItems.Count > 0 && listMail.Columns[0].Text != "名前";
            menuAlreadyRead.Enabled = listMail.SelectedItems.Count > 0 && listMail.Columns[0].Text != "名前";

            // 送信メールを選択したとき
            if (listMail.Columns[0].Text == "宛先") {
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
            menuContextClearTrush.Enabled = mailBox.Trash.Count != 0;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            // ボタンの有効状態を変更
            if (listMail.Columns[0].Text == "名前") {
                buttonDeleteMail.Enabled = false;
                buttonReplyMail.Enabled = false;
                buttonForwardMail.Enabled = false;

            }
            else {
                buttonDeleteMail.Enabled = listMail.SelectedItems.Count > 0;
                buttonReplyMail.Enabled = listMail.SelectedItems.Count == 1;
                buttonForwardMail.Enabled = listMail.SelectedItems.Count == 1;
            }

            // データ読み込みエラーを起こしたとき
            if (errorFlag) {
                this.Hide();
                this.Close();
            }
        }

        private void menuFowerdMail_Click(object sender, EventArgs e)
        {
            Mail mail = null;
            ListViewItem item = listMail.SelectedItems[0];

            // 選択アイテムが0のときは反応にしない
            if (listMail.SelectedItems.Count == 0) {
                return;
            }

            // 表示機能はシンプルなものに変わる
            mail = GetSelectedMail(item.Tag, listMail.Columns[0].Text);

            // 転送メールの作成
            CreateFowerdMail(mail);
        }

        #endregion
    }
}