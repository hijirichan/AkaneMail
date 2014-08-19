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
using System.Threading;

namespace AkaneMail
{
    public partial class MainForm : Form
    {
        private static readonly string DataRootPath = Application.StartupPath;

        private static readonly string TempFileRoot = DataRootPath + @"\tmp";

        private static readonly string SettingFilePath = DataRootPath + @"\AkaneMail.xml";

        private static readonly string MailDataPath = DataRootPath + @"\Mail.dat";

        // メールを格納する配列
        MailBox mailBox;

        // ListViewItemSorterに指定するフィールド
        public ListViewItemComparer listViewItemSorter = ListViewItemComparer.Default;

        // データ変更が発生したのときのフラグ
        public bool dataModified;

        // 環境保存用のクラスインスタンス
        private MailSettings MailSetting;

        // 点滅用 Win32API のインポート
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
        public static void FlashWindow(Form window)
        {
            FlashWindow(window.Handle, false);
        }

        public MainForm()
        {
            // はじめは最小化した状態にしておく
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;

            InitializeComponent();

            Application.Idle += Application_Idle;

            mailBox = new MailBox();
        }

        /// <summary>
        /// ツリービューの更新
        /// </summary>
        public void UpdateTreeView()
        {
            // メールの件数を設定する
            treeMailBoxFolder.Nodes[0].Nodes[0].Text = mailBox.Receive.ToString();
            treeMailBoxFolder.Nodes[0].Nodes[1].Text = mailBox.Send.ToString();
            treeMailBoxFolder.Nodes[0].Nodes[2].Text = mailBox.Trash.ToString();
        }

        /// <summary>
        /// リストビューの更新
        /// </summary>
        public void UpdateListView()
        {
            if (AccountSelected()) {
                // メールボックスのとき
                InitializeMailBox();
                return;
            }

            listMail.BeginUpdate();
            listMail.Items.Clear();
            var folder = mailBox.GetSelectedMailFolder(listMail.Columns[0].Text);
            listMail.Items.AddRange(folder.Select(CreateMailItem).ToArray());
            listMail.EndUpdate();
        }

        private bool AccountSelected()
        {
            return listMail.Columns[0].Text == "名前";
        }

        private void InitializeMailBox()
        {
            listMail.Items.Clear();
            var item = new ListViewItem(AccountInfo.fromName);
            item.SubItems.Add(AccountInfo.mailAddress);
            var fi = new FileInfo(MailDataPath);
            if (fi.Exists) {
                var mailDataDate = fi.LastWriteTime.ToString("yy/MM/dd hh:mm:ss");
                item.SubItems.AddRange(new[] { mailDataDate, fi.Length.ToString() });
            }
            else {
                item.SubItems.AddRange(new[] { "データ未作成", "0" });
            }
            listMail.Items.Add(item);
        }

        private ListViewItem CreateMailItem(Mail mail, int index)
        {
            var item = new ListViewItem(mail.Address)
            {
                Tag = index,
                Name = index.ToString(),
                ForeColor = MailPriority.GetPriorityColor(mail)
            };

            if (mail.Subject != "") {
                item.SubItems.Add(mail.Subject);
            }
            else {
                item.SubItems.Add("(no subject)");
            }

            // メールの受信(送信)日時とメールサイズをリストのサブアイテムに追加する
            item.SubItems.Add(mail.Date);
            item.SubItems.Add(mail.Size);

            // 未読(未送信)の場合は、フォントを太字にする
            if (mail.NotReadYet) {
                item.Font = new Font(this.Font, FontStyle.Bold);
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
            progressMail.Value = value;
        }

        /// <summary>
        /// メール送信・受信用プログレスバーの非表示
        /// </summary>
        private void HideProgressMail()
        {
            progressMail.Visible = false;
            progressMail.Value = 0;
            progressMail.Minimum = 0;
            progressMail.Maximum = 0;
        }

        /// <summary>
        /// 送受信メニューとツールボタンの更新
        /// </summary>
        private void EnableButton()
        {
            menuRecieveMail.Enabled = true;
            buttonRecieveMail.Enabled = true;

            menuSendMail.Enabled = true;
            buttonSendMail.Enabled = true;
        }

        /// <summary>
        /// メール送受信後のTreeView、ListViewの更新
        /// </summary>
        private void UpdateView(ListViewItemComparer sorter = null)
        {
            listMail.ListViewItemSorter = null;

            UpdateTreeView();
            UpdateListView();

            listMail.ListViewItemSorter = sorter ?? listViewItemSorter;
        }

        private void UpdateViewFully()
        {
            // 本文ペインをリセットする
            this.textBody.Text = "";
            if (this.browserBody.Visible) {
                this.browserBody.Visible = false;
                this.textBody.Visible = true;
            }

            UpdateView(ListViewItemComparer.Default);

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

        private void SetMessage(string message)
        {
            labelMessage.Text = message;
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
            AccountInfo.Reset();
            // 環境設定ファイルが存在する場合は環境設定情報を読み込んでアカウント情報に設定する
            if (File.Exists(SettingFilePath)) {
                MailSetting = MailSettings.Load(SettingFilePath);

                #region アカウント情報
                AccountInfo.fromName = MailSetting.m_fromName;
                AccountInfo.mailAddress = MailSetting.m_mailAddress;
                AccountInfo.userName = MailSetting.m_userName;
                AccountInfo.passWord = MailSettings.Decrypt(MailSetting.m_passWord);
                #endregion

                #region 接続情報
                AccountInfo.smtpServer = MailSetting.m_smtpServer;
                AccountInfo.popServer = MailSetting.m_popServer;
                AccountInfo.smtpPortNumber = MailSetting.m_smtpPortNo;
                AccountInfo.popPortNumber = MailSetting.m_popPortNo;
                AccountInfo.apopFlag = MailSetting.m_apopFlag;
                AccountInfo.deleteMail = MailSetting.m_deleteMail;
                AccountInfo.popBeforeSMTP = MailSetting.m_popBeforeSMTP;
                AccountInfo.popOverSSL = MailSetting.m_popOverSSL;
                AccountInfo.smtpAuth = MailSetting.m_smtpAuth;
                #endregion

                #region 自動受信設定
                AccountInfo.autoMailFlag = MailSetting.m_autoMailFlag;
                AccountInfo.getMailInterval = MailSetting.m_getMailInterval;
                #endregion

                #region 通知設定
                AccountInfo.popSoundFlag = MailSetting.m_popSoundFlag;
                AccountInfo.popSoundName = MailSetting.m_popSoundName;
                AccountInfo.bodyIEShow = MailSetting.m_bodyIEShow;
                AccountInfo.minimizeTaskTray = MailSetting.m_minimizeTaskTray;
                #endregion

                #region 画面設定
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
                #endregion
            }
        }

        /// <summary>
        /// デコード機能を使用するかを設定
        /// </summary>
        /// <param name="convert"></param>
        private void ChangeConvertMode(string convert)
        {
            // 変換フラグがない時はHTML/Base64のデコードを有効にする
            if (string.IsNullOrWhiteSpace(convert)) {
                Options.EnableDecodeBody();
            }
            else {
                Options.DisableDecodeBodyText();
            }
        }

        /// <summary>
        /// 添付ファイルを抽出
        /// </summary>
        /// <param name="attach">Attachmentインスタンス</param>
        /// <param name="mail">メール</param>
        /// <returns>添付ファイル抽出後のメール</returns>
        private nMail.Attachment ExtractAttachFile(nMail.Attachment attach, Mail mail)
        {
            var AttachMail = attach;

            try {
                AttachMail.Add(mail.Header, mail.Body);
                AttachMail.Save();
            }
            catch (Exception ex) {
                labelMessage.Text = String.Format("エラー メッセージ:{0:s}", ex.Message);
                return null;
            }

            return AttachMail;
        }

        /// <summary>
        /// デコードされていないメッセージを展開
        /// </summary>
        /// <param name="mail">デコード前のメッセージ</param>
        /// <returns>デコード後のメッセージ</returns>
        private nMail.Attachment ExtractMessage(Mail mail)
        {
            Options.EnableDecodeBody();

            var attach = new nMail.Attachment();

            attach.Path = Application.StartupPath + @"\tmp";

            // Contents-Typeがtext/htmlのメールか確認するフラグを取得する
            var isHtmlMail = attach.GetHeaderField("Content-Type:", mail.Header).Contains("text/html");
            var hasAttachments = attach.GetId(mail.Header) != nMail.Attachment.NoAttachmentFile;

            var DecodeMessage = ExtractAttachFile(attach, mail);

            if (DecodeMessage == null) return null;

            return DecodeMessage;

        }

        /// <summary>
        /// 指定されたメールを開く
        /// </summary>
        /// <param name="mail">メール</param>
        private void OpenMail(Mail mail)
        {
            var attach = new nMail.Attachment { Path = TempFileRoot };
            try {
                ChangeConvertMode(mail.Convert);
                attach.Add(mail.Header, mail.Body);
                attach.Save();
            }
            catch {
                throw;
            }

            // 表示状態の決定
            var showInBrowser = AccountInfo.bodyIEShow && attach.HtmlFile != "";
            this.browserBody.Visible = showInBrowser;
            this.textBody.Visible = !this.browserBody.Visible;
            try {
                textBody.Text = ReadPlainText(attach, mail);
            }
            catch (Exception ex) {
                labelMessage.Text = String.Format("エラー メッセージ:{0:s}", ex.Message);
                return;
            }

            if (showInBrowser) {
                browserBody.AllowNavigation = true;
                browserBody.Navigate(attach.Path + @"\" + attach.HtmlFile);
            }

            //添付ファイルリストの表示決定
            buttonAttachList.DropDownItems.Clear();
            var paths = GetAttachmentFilePaths(attach, mail);
            buttonAttachList.Visible = paths.Any();
            buttonAttachList.DropDownItems.AddRange(NmailAttachEx.GenerateMenuItem("", paths).ToArray());
        }

        private string ReadPlainText(nMail.Attachment attach, Mail mail)
        {
            if (attach.FileNameList != null || attach.HtmlFile != "") {
                return ReadHtmlText(attach, mail);
            }
            else {
                return DecodeBody(attach, mail);
            }
        }

        private IEnumerable<string> GetAttachmentFilePaths(nMail.Attachment attach, Mail mail)
        {
            if (!AccountInfo.bodyIEShow || attach.HtmlFile == "" || (attach.FileNameList ?? new string[] { }).Length > 1) {
                return (attach.FileNameList ?? Enumerable.Empty<string>()).Select(p => Path.Combine(attach.Path, p));
            }
            else {
                return mail.Attachments;
            }
        }

        private string ReadHtmlText(nMail.Attachment attach, Mail mail)
        {
            if (attach.GetHeaderField("Content-Type:", mail.Header).Contains("text/html")) {
                using (var sr = new StreamReader(TempFileRoot + "\\" + attach.HtmlFile, Encoding.Default)) {
                    return Mail.HtmlToText(sr.ReadToEnd(), mail.Header);
                }
            }
            else {
                return string.IsNullOrWhiteSpace(attach.Body) ? mail.Body : BreakLine(attach.Body);
            }
        }

        private string DecodeBody(nMail.Attachment attach, Mail mail)
        {
            var contentType = attach.GetHeaderField("Content-Type:", mail.Header).ToLower();
            var qp = attach.GetHeaderField("Content-Transfer-Encoding:", mail.Header).Contains("quoted-printable");
            var base64 = attach.GetDecodeHeaderField("Content-Transfer-Encoding:", mail.Header).Contains("base64");
            if ((contentType.Contains("iso-2022-jp") && qp) || base64) {
                return AttachmentDecode(attach, mail);
            }
            else if (attach.GetHeaderField("X-NMAIL-BODY-UTF8:", mail.Header).Contains("8bit")) {
                var bs = mail.Body.Select(c => (byte)c).ToArray();
                return Encoding.UTF8.GetString(bs);
            }
            else {
                if (attach.Body.Length > 0) { return attach.Body; }
                var b = Encoding.GetEncoding("iso-2022-jp").GetBytes(mail.Body);
                return Encoding.GetEncoding("iso-2022-jp").GetString(b);
            }
        }

        private string AttachmentDecode(nMail.Attachment attach, Mail mail)
        {
            Options.EnableDecodeBody();

            attach.Add(mail.Header, mail.Body);
            attach.Save();

            return BreakLine(attach.Body);
        }

        private string BreakLine(string text)
        {
            if (text.Contains("\n\n")) {
                text = text.Replace("\n\n", "\r\n").Replace("\n", "\r\n");
            }
            return text;
        }

        /// <summary>
        /// 返信メールを作成
        /// </summary>
        /// <param name="mail"></param>
        private void CreateReturnMail(Mail mail)
        {
            var NewMailForm = new MailEditorForm { Owner = this };

            NewMailForm.SendList = mailBox.Send.ToList();

            NewMailForm.textAddress.Text = mail.Address;
            NewMailForm.textSubject.Text = "Re:" + mail.Subject;

            NewMailForm.textBody.Text = "\r\n\r\n---" + mail.Address + " was wrote ---\r\n\r\n" + textBody.Text;

            NewMailForm.Show();
        }

        /// <summary>
        /// 転送メールを作成
        /// </summary>
        /// <param name="mail">メール</param>
        private void CreateFowerdMail(Mail mail)
        {
            var NewMailForm = new MailEditorForm { Owner = this };

            NewMailForm.SendList = mailBox.Send.ToList();

            // 転送のために件名を設定する(件名は空白にする)
            NewMailForm.textSubject.Text = "Fw:" + mail.Subject;

            NewMailForm.textBody.Text = BuildForwardingBody(mail);

            // 送信メールで添付ファイルがあるとき
            if (mail.Attachments.Length != 0) {
                NewMailForm.buttonAttachList.Visible = true;
                NewMailForm.buttonAttachList.DropDownItems.AddRange(mail.GenerateMenuItem().ToArray());
            }
            else if (this.buttonAttachList.Visible) {
                // 受信メールで添付ファイルがあるとき
                NewMailForm.buttonAttachList.Visible = true;

                var attaches = this.buttonAttachList.DropDownItems.Cast<ToolStripItem>().Select(i => i.Text);
                NewMailForm.buttonAttachList.DropDownItems.AddRange(NmailAttachEx.GenerateMenuItem(TempFileRoot + "\\", attaches).ToArray());
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

            var builder = new StringBuilder("\r\n\r\n")
                .AppendLine("--- Forwarded by " + AccountInfo.mailAddress + " ---")
                .AppendLine("----------------------- Original Message -----------------------")
                .AppendLine("From: " + from)
                .AppendLine("To: " + to)
                .AppendLine("Date: " + sentAt)
                .AppendLine(" Subject:" + subject)
                .AppendLine("----\r\n")
                .Append(textBody.Text);

            return builder.ToString();
        }

        private void InitializeMailEditorForm(Mail mail, MainForm mainForm)
        {
            MailEditorForm EditMailForm = new MailEditorForm
            {
                Owner = mainForm,
                Text = mail.Subject + " - Akane Mail",
                Mail = mail
            };

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

            if (mail.Attachments.Length != 0) {
                EditMailForm.buttonAttachList.Visible = true;
                EditMailForm.buttonAttachList.DropDownItems.AddRange(mail.GenerateMenuItem(true).ToArray());
            }

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
                dataModified = true;
            }
            else if (listMail.Columns[0].Text == "宛先") {
                InitializeMailEditorForm(mail, this);
            }
        }

        /// <summary>
        /// メールを削除
        /// </summary>
        private void DeleteMail()
        {
            var firstSelected = listMail.SelectedItems[0].Index;
            var items = listMail.SelectedItems.Cast<ListViewItem>();

            if (listMail.Columns[0].Text == "差出人") {
                // 受信メールのとき
                mailBox.MoveToTrash("Receive", items);
            }
            else if (listMail.Columns[0].Text == "宛先") {
                // 送信メールのとき
                mailBox.MoveToTrash("Send", items);
            }
            else if (listMail.Columns[0].Text == "差出人または宛先") {
                if (MessageBox.Show(MainFormMessages.Check.TrashComplete, "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK) {
                    // 削除メールのとき
                    mailBox.TrashCompletely(items);
                }
            }

            ClearInput();

            var after = Math.Min(firstSelected, listMail.Items.Count) - 1;

            // リストが空でないとき
            if (after >= 0) {
                // フォーカスをnSelの行に当てる
                listMail.Items[after].Selected = true;
                listMail.Items[after].Focused = true;
                listMail.SelectedItems[0].EnsureVisible();
                listMail.Select();
                listMail.Focus();
            }
            dataModified = true;
        }

        private IEnumerable<int> QueryRetreivingMailIds(nMail.Pop3 pop, IEnumerable<Mail> locals)
        {
            // 古い順に通し番号が振られるので、新しい順に見て手元のメールでUIDがヒットするまでTakeする
            var latestMail = locals.AsParallel().OrderBy(d => DateTime.Parse(d.Date)).LastOrDefault();
            if (latestMail == null) return Enumerable.Range(1, pop.Count);
            var latestUid = latestMail.Uidl;
            pop.GetUidl(nMail.Pop3.UidlAll);
            return pop.Uidl.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { ' ' }))
                .Reverse()
                .TakeWhile(s => s[1] != latestUid)
                .Select(s => int.Parse(s[0]));
        }

        /// <summary>
        /// POP3サーバからメールを受信する
        /// </summary>
        private void RecieveMail()
        {
            try {
                Invoke(SetMessage, MainFormMessages.Notification.MailReceiving);

                using (var pop = new nMail.Pop3()) {
                    Options.EnableConnectTimeout();
                    pop.APop = AccountInfo.apopFlag;

                    if (AccountInfo.popOverSSL) {
                        pop.SSL = nMail.Pop3.SSL3;
                    }
                    pop.Connect(AccountInfo.popServer, AccountInfo.popPortNumber);
                    pop.Authenticate(AccountInfo.userName, AccountInfo.passWord);

                    var receivingMailIds = CheckReceivingMails(pop);
                    // 受信していないメールがあったとき
                    if (receivingMailIds.Any()) {
                        // プログレスバーを表示(受信件数/未受信件数)
                        Invoke(ProgressMailInit, receivingMailIds.Count());
                        // HTML/Base64のデコードを無効にする
                        Options.DisableDecodeBodyText();
                        // ヘッダ・本文のデコードを無効にする(メール送受信コンポーネント変更の前段階)
                        // Options.DisableDecodeHeader();
                        // Options.DisableDecodeBodyAll();
                        Receive(pop, receivingMailIds);
                    }
                    Invoke(NotifyReceive, receivingMailIds.Count());
                    Invoke(UpdateViewFully);
                }
            }
            catch (nMail.nMailException ex) {
                Invoke(SetMessage, MainFormMessages.Error.GeneralErrorMessage(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex) {
                Invoke(SetMessage, MainFormMessages.Error.GeneralErrorMessage(ex.Message));
            }
            finally {
                Invoke(HideProgressMail);
                Invoke(EnableButton);
            }

        }

        private IEnumerable<int> CheckReceivingMails(Pop3 pop)
        {
            var countMail = Task.Run(() =>
            {
                var locals = mailBox.Receive.Union(mailBox.Trash);
                return QueryRetreivingMailIds(pop, locals);
            });

            if (pop.Count == 0) return new int[] { };

            Invoke(SetMessage, pop.Count + "件のメッセージがサーバ上にあります。");

            return countMail.Result;
        }

        private void Receive(Pop3 pop, IEnumerable<int> counts)
        {
            foreach (var no in counts.Select((num, i) => new { num, i })) {
                Invoke(SetMessage, (no.i + 1).ToString() + "件目のメールを受信しています。");
                pop.GetUidl(no.num);
                pop.GetMail(no.num);

                mailBox.Receive.Add(new Mail(pop, true, ""));

                if (AccountInfo.deleteMail) { pop.Delete(no.num); }

                Invoke(ProgressMailUpdate, no.i + 1);
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            }
        }

        private void NotifyReceive(int mailCount)
        {
            if (mailCount > 0) {
                // 通知音の再生(設定してあれば)
                if (AccountInfo.popSoundFlag && !string.IsNullOrWhiteSpace(AccountInfo.popSoundName)) {
                    using (var p = new SoundPlayer(AccountInfo.popSoundName)) { p.Play(); }
                }

                notifyIcon1.BalloonTipText = MainFormMessages.Notification.NewMailReceived(mailCount);
                // 通知の表示(タスクトレイに入っていて自動受信したとき)
                if (this.WindowState == FormWindowState.Minimized && AccountInfo.minimizeTaskTray && AccountInfo.autoMailFlag) {
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.BalloonTipTitle = MainFormMessages.Notification.NewMail;
                    notifyIcon1.ShowBalloonTip(300);
                }
                else {
                    Invoke(FlashWindow, this);
                }
                dataModified = true;
                Invoke(SetMessage, MainFormMessages.Notification.NewMailReceived(mailCount));
            }
            else {
                Invoke(SetMessage, MainFormMessages.Notification.AllReceived);
            }
        }

        private void Preauthenticate()
        {
            if (!AccountInfo.popBeforeSMTP) return;
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

        #region SendMail
        /// <summary>
        /// メールを送信する
        /// </summary>
        private void SendMail()
        {
            var draftMails = mailBox.Send.Where(m => m.NotReadYet);
            if (!draftMails.Any()) {
                Invoke(EnableButton);
                return;
            }

            SendMail(smtp =>
            {
                foreach (var mail in draftMails.Select((Mail, Index) => new { Index, Mail })) {
                    SendSingleMail(smtp, mail.Mail);
                    Invoke(ProgressMailUpdate, mail.Index + 1);
                    Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                }
                Invoke(HideProgressMail);
            });

            Invoke(() => UpdateView());
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
        public void DirectSendMail(string address, string cc, string bcc, string subject, string body, string attach, string priority)
        {
            SendMail(smtp => SendSingleMail(smtp, address, cc, bcc, subject, body, attach, priority));
        }

        /// <summary>
        /// Mailクラスの表すメールを送信します。
        /// </summary>
        /// <param name="smtp"></param>
        /// <param name="mail"></param>
        private void SendSingleMail(Smtp smtp, Mail mail)
        {
            SendSingleMail(smtp, mail.Address, mail.Cc, mail.Bcc, mail.Subject, mail.Body, mail.Attach, mail.Priority);

            mail.Date = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            mail.NotReadYet = false;
        }

        /// <summary>
        /// 送信先や本文などを指定してメールを送信します。
        /// </summary>
        /// <param name="smtp"></param>
        /// <param name="address"></param>
        /// <param name="cc"></param>
        /// <param name="bcc"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attach"></param>
        /// <param name="priority"></param>
        private void SendSingleMail(Smtp smtp, string address, string cc, string bcc, string subject, string body, string attach, string priority)
        {
            if (cc != "") { smtp.Cc = cc; }
            if (bcc != "") { smtp.Bcc = bcc; }
            if (attach != "") { smtp.FileName = attach; }
            smtp.Header = string.Format("\r\nPriority: {0}\r\nX-Mailer: Akane Mail Version {1}", priority, Application.ProductVersion);
            smtp.SendMail(address, AccountInfo.FromAddress, subject, body);
        }

        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <param name="sendMailAciton">SMTPクライアントを引数に取る送信操作</param>
        private void SendMail(Action<Smtp> sendMailAciton)
        {
            try {
                Invoke(SetMessage, MainFormMessages.Notification.MailSending);
                Preauthenticate();
                using (var smtp = new Smtp(AccountInfo.smtpServer)) {
                    smtp.Port = AccountInfo.smtpPortNumber;
                    if (AccountInfo.smtpAuth) {
                        smtp.Connect();
                        smtp.Authenticate(AccountInfo.userName, AccountInfo.passWord, Smtp.AuthPlain | Smtp.AuthCramMd5);
                    }
                    sendMailAciton(smtp);
                }
                Invoke(SetMessage, MainFormMessages.Notification.MailSent);
            }
            catch (nMail.nMailException ex) {
                Invoke(SetMessage, MainFormMessages.Error.GeneralErrorMessage(ex.Message, ex.ErrorCode));
            }
            catch (Exception ex) {
                Invoke(SetMessage, MainFormMessages.Error.GeneralErrorMessage(ex.Message));
            }
        }
        #endregion

        /// <summary>
        /// 添付ファイル付きメールの展開
        /// </summary>
        /// <param name="mail"></param>
        private void GetAttachMail(Mail mail)
        {
            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK) return;
            try {
                var attach = new nMail.Attachment();

                attach.Path = folderBrowserDialog1.SelectedPath;

                var tmpFileName = Path.GetTempFileName();
                using (var writer = new StreamWriter(tmpFileName)) {
                    writer.Write(mail.Header);
                    writer.Write("\r\n");
                    writer.Write(mail.Body);
                }

                using (var reader = new StreamReader(tmpFileName)) {
                    attach.Add(reader.ReadToEnd());
                }
                attach.Save();

                MessageBox.Show(MainFormMessages.Notification.InternalSaved(attach.Path, attach.FileName), "添付ファイルの取り出し", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(MainFormMessages.Error.GeneralErrorMessage(ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
        }

        /// <summary>
        /// メールファイルの保存
        /// </summary>
        /// <param name="mail">メール</param>
        /// <param name="FileToSave">保存ファイル名</param>
        private void SaveMailFile(Mail mail, string FileToSave)
        {
            // これたぶんバイト列をそのままファイルに突っ込んでいけば幸せになれる気がする
            string fileBody = "";
            string fileHeader = "";

            var encoding = DecideEncoding(mail);
            if (encoding.BodyName == Encoding.UTF8.BodyName) {
                // text/plainまたはmultipart/alternativeでUTF-8でエンコードされたメールのとき
                // nMailの仕様上、UTF-8の文字列が時々化けるのでいったんバイト列にしてからデコードし直す
                var bs = mail.Body.Select(s => (byte)s).ToArray();

                fileBody = Encoding.UTF8.GetString(bs);
                fileHeader = mail.Header;
            }
            else {
                var b = encoding.GetBytes(mail.Header);
                fileHeader = encoding.GetString(b);

                b = encoding.GetBytes(mail.Body);
                fileBody = encoding.GetString(b);
            }

            using (var writer = new StreamWriter(FileToSave, false, encoding)) {
                // 受信メール(ヘッダが存在する)のとき
                if (mail.Header.Length > 0) {
                    writer.Write(fileHeader);
                }
                else {
                    // 送信メールのときはヘッダの代わりに送り先と件名を出力
                    writer.WriteLine("To: " + mail.Address);
                    writer.WriteLine("Subject: " + mail.Subject);
                }
                writer.Write("\r\n");

                writer.Write(fileBody);
            }
        }

        private Encoding DecideEncoding(Mail mail)
        {
            // ヘッダーから文字コードを取得する
            string enc = Mail.ParseEncoding(mail.Header).ToLower();
            if (new[] { "iso-", "shift_", "euc", "windows", "utf-8" }.Any(enc.Contains)) {
                return Encoding.GetEncoding(enc);
            }
            else if (mail.Header.Contains("X-NMAIL-BODY-UTF8: 8bit")) {
                return Encoding.UTF8;
            }
            else {
                // 添付ファイルがだいたいここに来る
                return Encoding.GetEncoding("iso-2022-jp");
            }
        }


        /// <summary>
        ///  選択されたメールを取得します。
        /// </summary>
        /// <param name="index">インデックス</param>
        /// <param name="columnText">選択されているカラムの文字列</param>
        /// <returns></returns>
        private Mail GetSelectedMail(object index)
        {
            return GetShowingMailFolder()[(int)index];
        }

        /// <summary>
        /// リストビューのフォーカスをリセットします。
        /// </summary>
        /// <param name="listView">対象のリストビュー</param>
        private void ReforcusListView(ListView listView)
        {
            var row = listView.SelectedIndices[0];
            UpdateView();

            listView.Items[row].Selected = true;
            listView.SelectedItems[0].EnsureVisible();
            listView.Select();
            listView.Focus();
        }

        /// <summary>
        /// 入力欄をクリアします。
        /// </summary>
        private void ClearInput()
        {
            // 本文ペインのリセット
            this.textBody.Text = "";
            if (this.browserBody.Visible) {
                this.browserBody.Visible = false;
                this.textBody.Visible = true;
            }

            // 添付リストのリセット
            if (buttonAttachList.Visible) {
                buttonAttachList.DropDownItems.Clear();
                buttonAttachList.Visible = false;
            }

            // その他のペインのリセット
            UpdateView();
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
                    Invoke(SetMessage, "メールボックス");
                    listMail.ContextMenuStrip = null;
                    break;
                case "ReceiveMailBox":
                    // 受信メールが選択された場合
                    SetListViewColumns("差出人", "件名", "受信日時", "サイズ");
                    Invoke(SetMessage, "受信メール");
                    listMail.ContextMenuStrip = menuListView;
                    break;
                case "SendMailBox":
                    // 送信メールが選択された場合
                    SetListViewColumns("宛先", "件名", "送信日時", "サイズ"); ;
                    Invoke(SetMessage, "送信メール");
                    listMail.ContextMenuStrip = menuListView;
                    break;
                case "DeleteMailBox":
                    // ごみ箱が選択された場合
                    SetListViewColumns("差出人または宛先", "件名", "受信日時または送信日時", "サイズ");
                    Invoke(SetMessage, "ごみ箱");
                    listMail.ContextMenuStrip = menuListView;
                    break;
                default:
                    break;
            }

            ClearInput();
        }

        private void menuAppExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuSetEnv_Click(object sender, EventArgs e)
        {
            timerAutoReceive.Enabled = false;

            var ret = new SettingForm().ShowDialog();

            if (ret == DialogResult.OK) {
                SetTimer(AccountInfo.autoMailFlag, AccountInfo.getMailInterval);
            }

            listMail.ListViewItemSorter = null;
            UpdateListView();
            listMail.ListViewItemSorter = listViewItemSorter;
        }

        private void timerStatusTime_Tick(object sender, EventArgs e)
        {
            labelDate.Text = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
        }

        private void menuNewMail_Click(object sender, EventArgs e)
        {
            var newMailForm = new MailEditorForm
            {
                Owner = this,
                // TODO たぶんメール渡すだけでいい
                SendList = mailBox.Send.ToList()
            };

            newMailForm.Show();
        }

        private async void menuSendMail_Click(object sender, EventArgs e)
        {
            menuRecieveMail.Enabled = false;
            buttonRecieveMail.Enabled = false;
            buttonRecieveMail.Enabled = false;
            buttonSendMail.Enabled = false;

            await Task.Run(() => SendMail());
        }

        private async void menuRecieveMail_Click(object sender, EventArgs e)
        {
            menuRecieveMail.Enabled = false;
            buttonRecieveMail.Enabled = false;

            await Task.Run(() => RecieveMail());
        }

        private void menuDeleteMail_Click(object sender, EventArgs e)
        {
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
            ChangeSelectedMailReadStatus(false);
        }

        /// <summary>
        /// 既読メールを未読にする
        /// </summary>
        private void menuNotReadYet_Click(object sender, EventArgs e)
        {
            ChangeSelectedMailReadStatus(true);
        }

        private void ChangeSelectedMailReadStatus(bool unread)
        {
            var sList = GetShowingMailFolder();

            var items = listMail.SelectedItems;
            if (items.Count == 0) return;

            items.Cast<ListViewItem>()
                .Select(t => int.Parse(t.Name))
                .Select(i => sList[i])
                .ToList()
                .ForEach(m => m.NotReadYet = unread);

            ReforcusListView(listMail);

            dataModified = true;
        }

        private void listMail_DoubleClick(object sender, EventArgs e)
        {
            // メールボックスのときは反応しない
            if (AccountSelected()) return;
            var item = listMail.SelectedItems[0];
            var mail = GetSelectedMail(item.Tag);

            EditMail(mail, item);
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Directory.Exists(TempFileRoot)) {
                Directory.Delete(TempFileRoot, true);
            }

            if (dataModified) {
                if (File.Exists(MailDataPath)) {
                    File.Delete(MailDataPath);
                }

                await Task.Run(() => mailBox.MailDataSave());
            }

            MailSetting.Save(SettingFilePath);
        }

        private void CheckSocket()
        {
            try {
                nMail.Winsock.Initialize();
            }
            catch (Exception exp) {
                // 64bit環境で32bit用のnMail.dllを使用して起動したときはエラーになる
                if (exp.Message.Contains("間違ったフォーマットのプログラムを読み込もうとしました。")) {
                    MessageBox.Show(MainFormMessages.Error.Needx64nMail, MainFormMessages.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Idle -= Application_Idle;
                    Application.Exit();
                }
            }
        }

        private void SetTimer(bool isEnabled, int intervalMinutes)
        {
            // 60,000(msec)
            timerAutoReceive.Interval = intervalMinutes * 60000;
            timerAutoReceive.Enabled = isEnabled;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // スプラッシュスクリーンよりも先にフォームが出ることがあるらしい
            this.Hide();
            var splash = new SplashScreen();
            splash.Initialize();

            var load = Task.Run(() => LoadSettings());
            CheckSocket();
            Options.EnableSaveHtmlFile();

            // ファイル展開用のテンポラリフォルダの作成
            if (!Directory.Exists(TempFileRoot)) {
                Directory.CreateDirectory(TempFileRoot);
            }
            await load;
            try {
                var t = Task.Run(() => mailBox.MailDataLoad());
                splash.ProgressMesssage = MainFormMessages.Notification.MailLoading;
                await t;
            }
            catch (MailLoadException ex) {
                Invoke(() =>
                {
                    MessageBox.Show("読み込みエラーが発生しました。\n メッセージ:" + ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    dataModified = false;
                    Application.Exit();
                });
            }

            SetTimer(AccountInfo.autoMailFlag, AccountInfo.getMailInterval);

            splash.Dispose();

            if (!(AccountInfo.minimizeTaskTray && WindowState == FormWindowState.Minimized)) {
                ShowInTaskbar = true;
                this.Show();
            }

            listMail.ListViewItemSorter = ListViewItemComparer.Default;
            // このタイミングで初期化が走るらしい
            treeMailBoxFolder.ExpandAll();

            this.Activate();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Idle -= Application_Idle;
            nMail.Winsock.Done();
        }

        private void MenuAction(Func<bool> CancelCondition, Action<Mail> action)
        {
            if (CancelCondition()) return;
            action(GetSelectedMail(listMail.SelectedItems[0].Tag));
        }

        private void menuReplyMail_Click(object sender, EventArgs e)
        {
            // 表示機能はシンプルなものに変わる
            MenuAction(() => listMail.SelectedItems.Count == 0, CreateReturnMail);
        }

        private void listMail_Click(object sender, EventArgs e)
        {
            MenuAction(AccountSelected, OpenMail);
        }

        private void menuGetAttatch_Click(object sender, EventArgs e)
        {
            // 送信メール以外も展開できるように変更
            MenuAction(() => listMail.SelectedItems.Count == 0, GetAttachMail);
        }

        private void menuSaveMailFile_Click(object sender, EventArgs e)
        {
            MenuAction(() => listMail.SelectedItems.Count == 0, mail =>
            {
                // ファイル名にメールの件名を入れる
                saveFileDialog1.FileName = mail.Subject;

                // 名前を付けて保存
                if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
                if (string.IsNullOrWhiteSpace(saveFileDialog1.FileName)) return;
                try {
                    SaveMailFile(mail, saveFileDialog1.FileName);
                }
                catch (Exception ex) {
                    MessageBox.Show(MainFormMessages.Error.GeneralErrorMessage(ex.Message), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            });
        }

        private void menuClearTrush_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(MainFormMessages.Check.ClearTrash, "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            mailBox.Trash.Clear();
            ClearInput();
            dataModified = true;
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            AboutForm AboutForm = new AboutForm();
            AboutForm.ShowDialog();
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // ファイルを開くかの確認をする
            if (MessageBox.Show(MainFormMessages.Check.OpenUnsafeFile(e.ClickedItem.Text), "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes) return;

            var mail = GetSelectedMail(listMail.SelectedItems[0].Tag);
            // 受信されたメールのとき
            if (mail.Attach.Length == 0) {
                System.Diagnostics.Process.Start(TempFileRoot + "\\" + e.ClickedItem.Text);
            }
            else {
                // 送信メールのとき
                System.Diagnostics.Process.Start(e.ClickedItem.Text);
            }
        }


        private void listMail_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (AccountSelected()) return;

            listViewItemSorter.Column = e.Column;
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
            if (this.WindowState == FormWindowState.Minimized && AccountInfo.minimizeTaskTray) {
                // 最初化時に通知領域にしか表示しない
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void menuTaskRestoreWindow_Click(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;

            if (this.WindowState == FormWindowState.Minimized) {
                this.WindowState = FormWindowState.Normal;
            }

            this.Activate();
        }

        private void menuFile_DropDownOpening(object sender, EventArgs e)
        {
            // メールボックスのメールを1件選んでいるとき
            var condition = listMail.SelectedItems.Count == 1 && !AccountSelected();
            menuSaveMailFile.Enabled = condition;
            menuGetAttatch.Enabled = condition;

            menuClearTrush.Enabled = mailBox.Trash.Count != 0;
        }

        private void menuMail_DropDownOpening(object sender, EventArgs e)
        {
            if (AccountSelected()) {
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
            // メール返信・転送・添付ファイル保存の有効/無効化
            var condition = !AccountSelected() && listMail.SelectedItems.Count == 1;
            menuContextReplyMail.Enabled = condition;
            menuContextFowerdMail.Enabled = condition;
            menuContextGetAttatch.Enabled = condition;

            // メールの未既読操作、削除の有効/無効化
            var mailCondition = listMail.SelectedItems.Count > 0 && !AccountSelected();
            menuContextDeleteMail.Enabled = mailCondition;
            menuNotReadYet.Enabled = mailCondition;
            menuAlreadyRead.Enabled = mailCondition;

            // ラベルの変更
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
            if (AccountSelected()) {
                buttonDeleteMail.Enabled = false;
                buttonReplyMail.Enabled = false;
                buttonForwardMail.Enabled = false;
            }
            else {
                buttonDeleteMail.Enabled = listMail.SelectedItems.Count > 0;
                buttonReplyMail.Enabled = listMail.SelectedItems.Count == 1;
                buttonForwardMail.Enabled = listMail.SelectedItems.Count == 1;
            }
        }

        private void menuFowerdMail_Click(object sender, EventArgs e)
        {
            MenuAction(() => listMail.SelectedItems.Count == 0, CreateFowerdMail);
        }
        #endregion
    }
}