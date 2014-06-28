using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace AkaneMail
{
    public partial class MailEditorForm : Form
    {
        FindDialog findDlg;

        /// <summary>
        /// 親フォームクラス
        /// </summary>
        public MainForm MainForm { get { return Owner as MainForm; } }


        public Mail Mail { get; set; }

        /// <summary>
        /// 送信箱の配列
        /// </summary>
        public List<Mail> SendList { get; set; }

        /// <summary>
        /// テキスト変更フラグ
        /// </summary>
        public bool IsDirty { get; set; }

        /// <summary>
        /// 編集モードフラグ
        /// </summary>
        public bool IsEdit { get; set; }

        private readonly Dictionary<string, string> mailPriority = new Dictionary<string, string>()
        {
            { "高い", "urgent" },
            { "普通", "normal" },
            { "低い", "non-urgent" }
        };

        /// <summary>
        /// 送信箱に格納するときのメールサイズ取得
        /// </summary>
        /// <returns>メールサイズの文字列</returns>
        public string GetMailSize(string attaches)
        {
            double attachSize = 0;
            // 添付ファイルがあるとき
            if (attaches != "") {
                attachSize = attaches.Split(',').Sum(f => new FileInfo(f).Length * 1.33);
            }

            // メールサイズの合計を取得する
            var allString = string.Concat(
                AccountInfo.FromAddress, mailPriority[comboPriority.Text],
                textAddress.Text, textSubject.Text, textBody.Text, textCc.Text, textBcc.Text);

            return (Encoding.UTF8.GetByteCount(allString) + (long)attachSize).ToString();
        }

        public MailEditorForm()
        {
            Application.Idle += Application_Idle;

            InitializeComponent();
            comboPriority.SelectedIndex = 1;
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuSendMail_Click(object sender, EventArgs e)
        {
            // アドレスまたは本文が未入力のとき
            if (textAddress.Text == "" || textBody.Text == "") {
                var message = "";
                if (textAddress.Text == "") {
                    message += "宛先が入力されていません。\n";
                }
                if (textBody.Text == "") {
                    message += "本文が入力されていません。\n";
                }
                MessageBox.Show(message, "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 件名がないときは件名に(無題)を設定する
            if (string.IsNullOrWhiteSpace(textSubject.Text)) {
                textSubject.Text = "(無題)";
            }

            var priority = mailPriority[comboPriority.Text];

            // 文面の末尾が\r\nで終わるようにする
            if (!textBody.Text.EndsWith("\r\n")) {
                textBody.Text += "\r\n";
            }

            CleanAttach();

            var attaches = GetAttaches();

            var size = GetMailSize(attaches);

            // 直接送信
            var date = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
            var sendMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attaches, date, size, "", false, "", this.textCc.Text, this.textBcc.Text, priority);

            MainForm.DirectSendMail(this.textAddress.Text, this.textCc.Text, this.textBcc.Text, this.textSubject.Text, this.textBody.Text, attaches, priority);

            // コレクションに追加する
            SendList.Add(sendMail);

            BeforeClosing(MainForm);
            
            this.Close();
        }

        private void CleanAttach()
        {
            foreach (var item in buttonAttachList.DropDownItems.Cast<ToolStripItem>().Where(i => i.Text.Contains("は削除されています。"))) {
                buttonAttachList.DropDownItems.Remove(item);
            }

            // メニューが空になった時は添付リストの表示を非表示にする
            buttonAttachList.Visible = buttonAttachList.DropDownItems.Count != 0;

        }

        private string GetAttaches() {
            var items = buttonAttachList.DropDownItems
                .Cast<ToolStripItem>()
                .Select(i => i.Text);
            return string.Join(",", items);
        }
       
        /// <summary>
        /// このフォームを閉じる前の引継ぎをします。多分このメソッドがあるべき場所はここではない
        /// </summary>
        /// <param name="form"></param>
        private void BeforeClosing(MainForm form)
        {
            form.listMail.ListViewItemSorter = null;

            form.UpdateTreeView();
            form.UpdateListView();

            form.listMail.ListViewItemSorter = form.listViewItemSorter;

            form.dataModified = true;

            // インスタンスを使いまわしているのでなければこの2行はいらない
            IsEdit = false;
            IsDirty = false;
        }

        private void menuSetAttachFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                if (openFileDialog1.FileName != "") {
                    buttonAttachList.Visible = true;
                    labelMessage.Text = openFileDialog1.FileName + "をメールに添付しました。";
                    var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(openFileDialog1.FileName);
                    buttonAttachList.DropDownItems.Add(openFileDialog1.FileName, appIcon.ToBitmap());
                    IsDirty = true;
                }
            }
        }

        private void DoInActiveTextBox(Action<TextBox> action)
        {
            var ctrl = this.ActiveControl;
            if (ctrl is TextBox) {
                action(ctrl as TextBox);
            }
        }

        private void DoInActiveTextBox(Predicate<TextBox> condition, Action<TextBox> action)
        {
            var ctrl = this.ActiveControl;
            if (ctrl is TextBox && condition(ctrl as TextBox)) {
                action(ctrl as TextBox);
            }
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(c => c.CanUndo, ctrl => ctrl.Undo());
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(c => c.SelectionLength > 0, ctrl => ctrl.Cut());
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(c => c.SelectionLength > 0, ctrl => ctrl.Copy());
        }

        private void menuPaste_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(_ => Clipboard.ContainsData(DataFormats.Text), ctrl => ctrl.Paste());
        }

        private void menuSelectAll_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                // 全選択→解除、それ以外は全選択
                if (ctrl.SelectionLength == ctrl.Text.Length) {
                    ctrl.SelectionLength = 0;
                }
                else {
                    ctrl.SelectAll();
                }
            });
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(c => c.SelectionLength > 0, ctrl => ctrl.SelectedText = "");
        }

        private void menuSendMailBox_Click(object sender, EventArgs e)
        {
            if (textAddress.Text == "" || textBody.Text == "") {
                var message = "";
                if (textAddress.Text == "") { message += "宛先が入力されていません。\n"; }
                else if (textBody.Text == "") { message += "本文が入力されていません。\n"; }
                MessageBox.Show(message, "送信箱に入れる", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(textSubject.Text)) {
                textSubject.Text = "(無題)";
            }

            var priority = mailPriority[comboPriority.Text];

            if (!textBody.Text.EndsWith("\r\n")) {
                textBody.Text += "\r\n";
            }

            CleanAttach();

            var attaches = GetAttaches();

            // 未送信という文字列だと日付ソートでエラーになる
            var date = DateTime.Now.ToString("yy/MM/dd hh:mm:ss");
            var size = GetMailSize(attaches);
 
            if (!IsEdit) {
                var newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attaches, date, size, "", true, "", this.textCc.Text, this.textBcc.Text, priority);
                SendList.Add(newMail);
            }
            else {
                Mail.Update(textAddress.Text, null, textSubject.Text, textBody.Text, attaches, date, size, null, true, textCc.Text, null, textBcc.Text, priority);

                // Becky!と同じように更新後はテキストも変更
                MainForm.textBody.Text = textBody.Text;
            }

            BeforeClosing(MainForm);

            this.Close();
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            AboutForm AboutForm = new AboutForm();
            AboutForm.ShowDialog();
        }

        private void TextEdited(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        private void MailEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDirty) {
                Application.Idle -= Application_Idle;
                return;
            }

            string message = "送信メールの編集途中ですが、閉じてよろしいですか？\nウィンドウを閉じると", title;
            if (IsEdit) {
                message += "編集前の内容に戻ります。";
                title = "編集中";
            }
            else {
                message += "作成中のメールは保存されません。";
                title = "新規作成";
            }
            e.Cancel = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No;
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (MessageBox.Show(e.ClickedItem.Text + "を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            // 選択した添付ファイルメニューを削除する
            buttonAttachList.DropDownItems.Remove(e.ClickedItem);
            // 添付ファイルの数が0になったらリストを閉じる
            buttonAttachList.Visible = buttonAttachList.DropDownItems.Count > 0;

            IsDirty = true;
            labelMessage.Text = "";
        }

        private void menuFind_Click(object sender, EventArgs e)
        {
            SuppressMultiDialogs(findDlg, DialogMode.Find, textBody);
        }

        private void menuReplace_Click(object sender, EventArgs e)
        {
            SuppressMultiDialogs(findDlg, DialogMode.Replace, textBody); 
        }

        /// <summary>
        /// ダイアログの二重起動を防ぎます。
        /// </summary>
        /// <param name="dialog">表示させるダイアログのインスタンス。</param>
        /// <param name="dialogMode">表示させるモード。</param>
        /// <param name="textBox">対象のTextBox</param>
        private void SuppressMultiDialogs(FindDialog dialog, DialogMode dialogMode, TextBox textBox)
        {
            if (dialog == null || dialog.IsDisposed) {
                dialog = new FindDialog(dialogMode, textBox);
                dialog.Show(this);
            }
        }

        private void menuEdit_DropDownOpening(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                this.menuUndo.Enabled = ctrl.CanUndo;

                // 検索対象は本文入力のみ
                var istestBody = ctrl.Name == "textBody";
                menuFind.Enabled = istestBody;
                menuReplace.Enabled = istestBody;

                this.menuSelectAll.Enabled = ctrl.Text.Length > 0;

                var selected = ctrl.SelectionLength > 0;
                this.menuCut.Enabled = selected;
                this.menuCopy.Enabled = selected;
                this.menuDelete.Enabled = selected;
            });

            this.menuPaste.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                var selected = ctrl.SelectionLength > 0;
                this.buttonCut.Enabled = selected;
                this.buttonCopy.Enabled = selected;
            });

            this.buttonPaste.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        private void MailEditorForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // ドラッグ中のファイルやディレクトリの取得
                var drags = e.Data.GetData(DataFormats.FileDrop) as string[];

                if (drags.All(f => File.Exists(f))) {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void MailEditorForm_DragDrop(object sender, DragEventArgs e)
        {
            if (buttonAttachList.DropDownItems.Count > 0) {
                buttonAttachList.DropDownItems.Clear();
            }

            buttonAttachList.Visible = true;

            // ドラッグ＆ドロップされたファイルを添付ファイルリストに追加する
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            foreach (var fname in files) {
                var appIcon = Icon.ExtractAssociatedIcon(fname);
                buttonAttachList.DropDownItems.Add(fname, appIcon.ToBitmap());
            }

            IsDirty = true;
        }

        public void Initialize(Mail mail)
        {
            IsEdit = mail == null;

            this.Mail = mail ?? new Mail("", "", null, "", "", "", "", "", false, "", "", "", MailPriority.Normal);

            Text = (mail.Subject ?? "新規作成") + " - Akane Mail";
            textAddress.Text = mail.Address;
            textCc.Text = mail.Cc;
            textBcc.Text = mail.Bcc;
            textSubject.Text = mail.Subject;
            textBody.Text = mail.Body;

            if (mail.Priority == MailPriority.Urgent)
            {
                comboPriority.SelectedIndex = 0;
            }
            else if (mail.Priority == MailPriority.Normal)
            {
                comboPriority.SelectedIndex = 1;
            }
            else
            {
                comboPriority.SelectedIndex = 2;
            }

            if (mail.Attachments.Length != 0)
            {
                buttonAttachList.Visible = true;
                buttonAttachList.DropDownItems.AddRange(mail.GenerateMenuItem(true).ToArray());
            }
        }

        private void Initialize()
        {
            Text = "新規作成 - " + MainFormMessages.ProductName;
        }

    }
}