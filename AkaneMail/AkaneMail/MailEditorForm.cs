﻿using System;
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
        public string attachName;
        FindDialog findDlg = null;  // 検索ダイアログのインスタンスを格納

        /// <summary>
        /// 親フォームクラス
        /// </summary>
        public MainForm MainForm { set; get; }

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
        public string GetMailSize()
        {
            string addr = AccountInfo.FromAddress;
            string priority = mailPriority[comboPriority.Text];

            double attachSize = 0;
            // 添付ファイルがあるとき
            if (attachName != "") {
                attachSize = attachName.Split(',').Sum(f => new FileInfo(f).Length * 1.33);
            }

            // メールサイズの合計を取得する
            var formtexts = new[] { textAddress, textSubject, textBody, textCc, textBcc }
                .Select(t => t.Text).ToArray();
            var moretext = new[] { addr, priority };
            int sizes = formtexts.Concat(moretext)
                .Sum(b => System.Text.Encoding.UTF8.GetBytes(b).Length);

            return (sizes + (long)attachSize).ToString();
        }

        public MailEditorForm()
        {
            // Appliction.Idleを登録する
            Application.Idle += new EventHandler(Application_Idle);

            InitializeComponent();
        }

        private void MailEditorForm_Resize(object sender, EventArgs e)
        {
            textAddress.Width = this.Width - 85;
            textSubject.Width = this.Width - 85;
            textCc.Width = this.Width - 85;
            textBcc.Width = this.Width - 85;
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MailEditorForm_Load(object sender, EventArgs e)
        {
            // 添付ファイル名を空値に設定
            attachName = "";

            // 新規作成のとき(編集の場合はForm1から制御する)
            if (!IsEdit) {
                // 重要度をNormal(普通)に設定する
                comboPriority.SelectedIndex = 1;
            }

            // isDirtyをfalseにする
            IsDirty = false;
        }

        private void menuSendMail_Click(object sender, EventArgs e)
        {
            string size = "";
            string priority = "";

            // アドレスまたは本文が未入力のとき
            if (textAddress.Text == "" || textBody.Text == "") {
                if (textAddress.Text == "") {
                    // アドレス未入力エラーメッセージを表示する
                    MessageBox.Show("宛先が入力されていません。", "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (textBody.Text == "") {
                    // 本文未入力エラーメッセージを表示する
                    MessageBox.Show("本文が入力されていません。", "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // 件名がないときは件名に(無題)を設定する
            if (textSubject.Text == "") {
                textSubject.Text = "(無題)";
            }

            // 優先度の設定をする
            priority = mailPriority[comboPriority.Text];

            // 文面の末尾が\r\nでないときは\r\nを付加する
            if (!textBody.Text.EndsWith("\r\n")) {
                textBody.Text += "\r\n";
            }

            CleanAttach();

            attachName = GetAttaches();

            // 送信メールサイズを取得する
            size = GetMailSize();

            // 直接送信
            MainForm.DirectSendMail(this.textAddress.Text, this.textCc.Text, this.textBcc.Text, this.textSubject.Text, this.textBody.Text, attachName, priority);
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            var sendMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, this.attachName, date, size, "", false, "", this.textCc.Text, this.textBcc.Text, priority);
            // コレクションに追加する
            SendList.Add(sendMail);

            BeforeClosing(MainForm);
            
            this.Close();
        }

        private void CleanAttach()
        {
            if (buttonAttachList.DropDownItems.Count > 0) {
                foreach (var item in buttonAttachList.DropDownItems.Cast<ToolStripItem>()) {
                    // 添付ファイルが存在しないとき
                    if (item.Text.Contains("は削除されています。")) {
                        // そのメニューを削除する
                        buttonAttachList.DropDownItems.Remove(item);
                    }
                }

                // メニューが空になった時は添付リストの表示を非表示にする
                buttonAttachList.Visible = buttonAttachList.DropDownItems.Count != 0;
            }
        }

        private string GetAttaches() {
            // 削除アイテムチェック後に添付ファイルが1個以上ある場合
            var items = buttonAttachList.DropDownItems
                .Cast<ToolStripItem>()
                .Select(i => i.Text);
            return string.Join(",", items);
        }
       
        /// <summary>
        /// このフォームを閉じる前の引継ぎをします。
        /// </summary>
        /// <param name="form"></param>
        private void BeforeClosing(MainForm form)
        {
            // ListViewItemSorterを解除する
            form.listMail.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            form.UpdateTreeView();
            form.UpdateListView();

            // ListViewItemSorterを指定する
            form.listMail.ListViewItemSorter = form.listViewItemSorter;

            // 編集モードをfalseにする
            IsEdit = false;
            IsDirty = false;

            // データ変更フラグをtrueにする
            form.dataDirtyFlag = true;
        }

        private void menuSetAttachFile_Click(object sender, EventArgs e)
        {
            Icon appIcon;

            // ファイルを開くダイアログを表示する
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                if (openFileDialog1.FileName != "") {
                    buttonAttachList.Visible = true;
                    labelMessage.Text = openFileDialog1.FileName + "をメールに添付しました。";
                    appIcon = System.Drawing.Icon.ExtractAssociatedIcon(openFileDialog1.FileName);
                    buttonAttachList.DropDownItems.Add(openFileDialog1.FileName, appIcon.ToBitmap());
                    // isDirtyをtrueにする
                    IsDirty = true;
                }
            }
        }

        private void DoInActiveTextBox(Action<TextBox> action)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if (ctrl is TextBox) {
                action(ctrl as TextBox);
            }
        }

        private void menuUndo_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.CanUndo) { ctrl.Undo(); }
            });
        }

        private void menuCut_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength > 0) { ctrl.Cut(); }
            });
        }

        private void menuCopy_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength > 0) { ctrl.Copy(); }
            });
        }

        private void menuPaste_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (Clipboard.ContainsData(DataFormats.Text)) { ctrl.Paste(); }
            });
        }

        private void menuSelectAll_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength == ctrl.Text.Length) {
                    // テキストボックスの文字列全選択を解除する
                    ctrl.SelectionLength = 0;
                }
                else {
                    // それ以外のときはテキストの前選択をおこなう
                    ctrl.SelectAll();
                }
            });
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength > 0) { ctrl.SelectedText = ""; }
            });
        }

        private void menuSendMailBox_Click(object sender, EventArgs e)
        {
            // アドレスまたは本文が未入力のとき
            if (textAddress.Text == "" || textBody.Text == "") {
                if (textAddress.Text == "") {
                    // アドレス未入力エラーメッセージを表示する
                    MessageBox.Show("宛先が入力されていません。", "送信箱に入れる", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (textBody.Text == "") {
                    // 本文未入力エラーメッセージを表示する
                    MessageBox.Show("本文が入力されていません。", "送信箱に入れる", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            // 件名がないときは件名に(無題)を設定する
            if (textSubject.Text == "") {
                textSubject.Text = "(無題)";
            }

            // 優先度の設定をする
            var priority = mailPriority[comboPriority.Text];

            // 文面の末尾が\r\nでないときは\r\nを付加する
            if (!textBody.Text.EndsWith("\r\n")) {
                textBody.Text += "\r\n";
            }

            CleanAttach();

            attachName = GetAttaches();

            // 未送信メールは作成日時を格納するようにする(未送信という文字列だと日付ソートでエラーになる)
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
 
            // 編集フラグがOffのとき
            if (!IsEdit) {
                // 送信メールサイズを取得する
                var size = GetMailSize();

                // Form1からのコレクションに追加してリスト更新する
                var newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attachName, date, size, "", true, "", this.textCc.Text, this.textBcc.Text, priority);
                SendList.Add(newMail);
            }
            else {
                // 選択したメールの内容を書き換える
                // 送信リストに入れている情報を書き換える
                var size = GetMailSize();
                // 優先度の設定をする
                SendList[ListTag].Subject = textSubject.Text;
                SendList[ListTag].Address = textAddress.Text;
                SendList[ListTag].Body = textBody.Text;
                SendList[ListTag].Attach = attachName;
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
            // バージョン情報を表示する
            AboutForm AboutForm = new AboutForm();
            AboutForm.ShowDialog();
        }

        private void TextEdited(object sender, EventArgs e)
        {
            // isDirtyをtrueにする
            IsDirty = true;
        }

        private void MailEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // isDirtyフラグがtrueのとき
            if (IsDirty) {
                string message = "", title = "";
                if (IsEdit) {
                    message = "送信メールの編集途中ですが、閉じてよろしいですか？\nウィンドウを閉じると編集前の内容に戻ります。";
                    title = "編集中";
                }
                else {
                    message = "メールの作成途中ですが、閉じてよろしいですか？\nウィンドウを閉じると作成中のメールは保存されません。";
                    title = "新規作成";
                }
                if (MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No) {
                    // ウィンドウを閉じるのをキャンセル
                    e.Cancel = true;
                }
            }
            // Appliction.Idleを削除する
            Application.Idle -= new EventHandler(Application_Idle);
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // 添付するファイルパスをを削除するのかを確認
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
                string[] drags = e.Data.GetData(DataFormats.FileDrop) as string[];

                foreach (string d in drags) {
                    if (!System.IO.File.Exists(d)) {
                        // ファイル以外であればイベント・ハンドラを抜ける
                        return;
                    }
                }
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MailEditorForm_DragDrop(object sender, DragEventArgs e)
        {
            Icon appIcon;

            // 添付ファイルが1個以上ある場合はそのメニューを削除する
            if (buttonAttachList.DropDownItems.Count >= 1) {
                for (int cnt = 0; cnt < buttonAttachList.DropDownItems.Count; cnt++) {
                    buttonAttachList.DropDownItems.RemoveAt(cnt);
                }
            }

            buttonAttachList.Visible = true;

            // ドラッグ＆ドロップされたファイルを添付ファイルリストに追加する
            var files = e.Data.GetData(DataFormats.FileDrop) as string[];

            foreach (string fname in files) {
                appIcon = System.Drawing.Icon.ExtractAssociatedIcon(fname);
                buttonAttachList.DropDownItems.Add(fname, appIcon.ToBitmap());
            }

            // isDirtyをtrueにする
            IsDirty = true;
        }

    }
}