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

        /// <summary>
        /// メールデータの格納位置
        /// </summary>
        public int ListTag { get; set; }

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
            string addr = AccountInfo.FromAddress;
            string priority = mailPriority[comboPriority.Text];

            double attachSize = 0;
            // 添付ファイルがあるとき
            if (attaches != "") {
                attachSize = attaches.Split(',').Sum(f => new FileInfo(f).Length * 1.33);
            }

            // メールサイズの合計を取得する
            var formtexts = new[] { textAddress, textSubject, textBody, textCc, textBcc }
                .Select(t => t.Text);
            var moretext = new[] { addr, priority };
            int sizes = formtexts.Concat(moretext)
                .Sum(b => System.Text.Encoding.UTF8.GetBytes(b).Length);

            return (sizes + (long)attachSize).ToString();
        }

        public MailEditorForm()
        {
            Application.Idle += Application_Idle;

            InitializeComponent();
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MailEditorForm_Load(object sender, EventArgs e)
        {
            // 編集の場合はForm1から制御する
            if (!IsEdit) {
                comboPriority.SelectedIndex = 1;
            }

            IsDirty = false;
        }

        private void menuSendMail_Click(object sender, EventArgs e)
        {

            // アドレスまたは本文が未入力のとき
            if (textAddress.Text == "" || textBody.Text == "") {
                if (textAddress.Text == "") {
                    MessageBox.Show("宛先が入力されていません。", "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (textBody.Text == "") {
                    MessageBox.Show("本文が入力されていません。", "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // 件名がないときは件名に(無題)を設定する
            if (textSubject.Text == "") {
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
            MainForm.DirectSendMail(this.textAddress.Text, this.textCc.Text, this.textBcc.Text, this.textSubject.Text, this.textBody.Text, attaches, priority);
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            var sendMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attaches, date, size, "", false, "", this.textCc.Text, this.textBcc.Text, priority);
            // コレクションに追加する
            SendList.Add(sendMail);

            BeforeClosing(MainForm);
            
            this.Close();
        }
        
        private void CleanAttach()
        {
            if (buttonAttachList.DropDownItems.Count > 0) {
                foreach (var item in buttonAttachList.DropDownItems.Cast<ToolStripItem>()) {
                    // 存在しない添付ファイルをメニューから消す
                    if (item.Text.Contains("は削除されています。")) {
                        buttonAttachList.DropDownItems.Remove(item);
                    }
                }

                // メニューが空になった時は添付リストの表示を非表示にする
                buttonAttachList.Visible = buttonAttachList.DropDownItems.Count != 0;
            }
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

            form.dataDirtyFlag = true;

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
                if (textAddress.Text == "") {
                    MessageBox.Show("宛先が入力されていません。", "送信箱に入れる", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (textBody.Text == "") {
                    MessageBox.Show("本文が入力されていません。", "送信箱に入れる", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
 
            if (!IsEdit) {
                var size = GetMailSize(attaches);

                var newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attaches, date, size, "", true, "", this.textCc.Text, this.textBcc.Text, priority);
                SendList.Add(newMail);
            }
            else {
                // 選択したメールの内容を書き換える
                // 送信リストに入れている情報を書き換える
                var size = GetMailSize(attaches);

                SendList[ListTag].Subject = textSubject.Text;
                SendList[ListTag].Address = textAddress.Text;
                SendList[ListTag].Body = textBody.Text;
                SendList[ListTag].Attach = attaches;
                SendList[ListTag].Date = date;
                SendList[ListTag].Size = size;
                SendList[ListTag].NotReadYet = true;
                SendList[ListTag].Cc = textCc.Text;
                SendList[ListTag].Bcc = textBcc.Text;
                SendList[ListTag].Priority = priority;

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
            if (IsDirty) {
                string message, title;
                if (IsEdit) {
                    message = "送信メールの編集途中ですが、閉じてよろしいですか？\nウィンドウを閉じると編集前の内容に戻ります。";
                    title = "編集中";
                }
                else {
                    message = "メールの作成途中ですが、閉じてよろしいですか？\nウィンドウを閉じると作成中のメールは保存されません。";
                    title = "新規作成";
                }
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No) {
                    e.Cancel = true;
                    return;
                }
            }
            Application.Idle -= Application_Idle;
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (MessageBox.Show(e.ClickedItem.Text + "を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                // 選択した添付ファイルメニューを削除する
                buttonAttachList.DropDownItems.Remove(e.ClickedItem);
                // 添付ファイルの数が0になったらリストを閉じる
                buttonAttachList.Visible = buttonAttachList.DropDownItems.Count > 0;

                IsDirty = true;
                labelMessage.Text = "";
            }
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

                    var isctrlSelected = ctrl.SelectionLength > 0;
                    this.menuCut.Enabled = isctrlSelected;
                    this.menuCopy.Enabled = isctrlSelected;
                    this.menuDelete.Enabled = isctrlSelected;
                });

            // クリップボードの内容確認
            this.menuPaste.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                var isctrlSelected = ctrl.SelectionLength > 0;
                this.buttonCut.Enabled = isctrlSelected;
                this.buttonCopy.Enabled = isctrlSelected;
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

        public void Initialize(Mail mail, int tag)
        {
            Text = mail.Subject + " - Akane Mail";

            // 送信箱の配列をForm3に渡す
            //SendList = mailBox.Send.ToList();
            ListTag = tag;
            IsEdit = true;

            // 宛先、件名、本文をForm3に渡す
            textAddress.Text = mail.Address;
            textCc.Text = mail.Cc;
            textBcc.Text = mail.Bcc;
            textSubject.Text = mail.Subject;
            textBody.Text = mail.Body;

            // 重要度をForm3に渡す
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

            // 添付ファイルが付いているとき
            if (mail.Attaches.Length != 0)
            {
                // 添付リストメニューを表示
                buttonAttachList.Visible = true;
                // 添付ファイルの数だけメニューを追加する
                buttonAttachList.DropDownItems.AddRange(mail.GenerateMenuItem(true).ToArray());
            }
        }

    }
}