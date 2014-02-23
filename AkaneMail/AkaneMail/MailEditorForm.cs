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
        public string attachName;
        public string[] attachFileNameList;
        findDialog findDlg = null;  // 検索ダイアログのインスタンスを格納

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
            var formtexts = new[] { textAddress, textSubject, textBody, textCc, textBcc }.Select(t => t.Text).ToArray();
            var moretext = new[] { addr, priority };
            int sizes = formtexts.Concat(moretext).Sum(b => System.Text.Encoding.UTF8.GetBytes(b).Length);

            return (sizes + (long)attachSize).ToString();
        }

        public MailEditorForm()
        {
            // Appliction.Idleを登録する
            Application.Idle += new EventHandler(Application_Idle);

            InitializeComponent();
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            textAddress.Width = this.Width - 85;
            textSubject.Width = this.Width - 85;
            textCc.Width = this.Width - 85;
            textBcc.Width = this.Width - 85;
        }

        private void menuFileClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // 添付ファイル名を空値に設定
            attachName = "";

            // 新規作成のとき(編集の場合はForm1から制御する)
            if (IsEdit == false) {
                // 重要度をNormal(普通)に設定する
                comboPriority.SelectedIndex = 1;
            }

            // isDirtyをfalseにする
            IsDirty = false;
        }

        private void menuFileDirectSend_Click(object sender, EventArgs e)
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

            // 添付ファイルが1個以上ある場合
            if (buttonAttachList.DropDownItems.Count > 0) {
                var blanks = Enumerable.Range(0, buttonAttachList.DropDownItems.Count).
                    Where(i => buttonAttachList.DropDownItems[i].Text.Contains("は削除されています。")).ToArray();
                Array.ForEach(blanks, i => buttonAttachList.DropDownItems.RemoveAt(i));

                // メニューが空になった時は添付リストの表示を非表示にする
                buttonAttachList.Visible = buttonAttachList.DropDownItems.Count == 0;
            }

            // 削除アイテムチェック後に添付ファイルが1個以上ある場合
            if (buttonAttachList.DropDownItems.Count > 0) {
                var attaches = Enumerable.Range(0, buttonAttachList.DropDownItems.Count).Select(i => buttonAttachList.DropDownItems[i].Text);
                // 添付ファイル名のリストを変数に渡す
                attachName = string.Join(",", attaches.ToArray());
            }

            // 送信メールサイズを取得する
            size = GetMailSize();

            // 直接送信
            MainForm.DirectSendMail(this.textAddress.Text, this.textCc.Text, this.textBcc.Text, this.textSubject.Text, this.textBody.Text, attachName, priority);
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            // コレクションに追加する
            Mail newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, this.attachName, date, size, "", false, "", this.textCc.Text, this.textBcc.Text, priority);
            SendList.Add(newMail);

            // ListViewItemSorterを解除する
            MainForm.listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            MainForm.UpdateTreeView();
            MainForm.UpdateListView();

            // ListViewItemSorterを指定する
            MainForm.listView1.ListViewItemSorter = MainForm.listViewItemSorter;

            // 編集モードをfalseにする
            IsEdit = false;
            IsDirty = false;

            // データ変更フラグをtrueにする
            MainForm.dataDirtyFlag = true;

            this.Close();
        }

        private void menuFileAttach_Click(object sender, EventArgs e)
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
            if (ctrl is SplitContainer) {
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if (ctrl is TextBox) {
                    action((TextBox)ctrl);
                }
            }
        }

        private void menuEditUndo_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.CanUndo) { ctrl.Undo(); }
            });
        }

        private void menuEditCut_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength > 0) { ctrl.Cut(); }
            });
        }

        private void menuEditCopy_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength > 0) { ctrl.Copy(); }
            });
        }

        private void menuEditPaste_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (Clipboard.ContainsData(DataFormats.Text)) { ctrl.Paste(); }
            });
        }

        private void menuEditAllSelect_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (((TextBox)ctrl).SelectionLength == ((TextBox)ctrl).Text.Length) {
                    // テキストボックスの文字列全選択を解除する
                    ((TextBox)ctrl).SelectionLength = 0;
                }
                else {
                    // それ以外のときはテキストの前選択をおこなう
                    ((TextBox)ctrl).SelectAll();
                }
            });
        }

        private void menuEditDelete_Click(object sender, EventArgs e)
        {
            DoInActiveTextBox(ctrl =>
            {
                if (ctrl.SelectionLength > 0) { ctrl.SelectedText = ""; }
            });
        }

        private void menuFileSend_Click(object sender, EventArgs e)
        {
            string size = "";
            string priority = "";

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
            priority = mailPriority[comboPriority.Text];

            // 文面の末尾が\r\nでないときは\r\nを付加する
            if (!textBody.Text.EndsWith("\r\n")) {
                textBody.Text += "\r\n";
            }

            // 添付ファイルが1個以上ある場合
            if (buttonAttachList.DropDownItems.Count >= 1) {
                for (int cnt = 0; cnt < buttonAttachList.DropDownItems.Count; cnt++) {
                    // 添付ファイルが存在しないとき
                    if (buttonAttachList.DropDownItems[cnt].Text.Contains("は削除されています。")) {
                        // そのメニューを削除する
                        buttonAttachList.DropDownItems.RemoveAt(cnt);
                    }
                }

                // メニューが空になった時は添付リストの表示を非表示にする
                buttonAttachList.Visible = buttonAttachList.DropDownItems.Count != 0;
            }

            // 削除アイテムチェック後に添付ファイルが1個以上ある場合
            if (buttonAttachList.DropDownItems.Count > 0) {
                var attachList = string.Join(",", buttonAttachList.DropDownItems.Cast<ToolStripItem>().Select(i => i.Text).ToArray());
                // 添付ファイル名のリストを変数に渡す
                attachName = attachList;
            }

            // 未送信メールは作成日時を格納するようにする(未送信という文字列だと日付ソートでエラーになる)
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            // 編集フラグがOffのとき
            if (!IsEdit) {
                // 送信メールサイズを取得する
                size = GetMailSize();

                // Form1からのコレクションに追加してリスト更新する
                Mail newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attachName, date, size, "", true, "", this.textCc.Text, this.textBcc.Text, priority);
                SendList.Add(newMail);
            }
            else {
                // 選択したメールの内容を書き換える
                // 送信リストに入れている情報を書き換える
                size = GetMailSize();
                SendList[ListTag].subject = textSubject.Text;
                SendList[ListTag].address = textAddress.Text;
                SendList[ListTag].body = textBody.Text;
                SendList[ListTag].attach = attachName;
                SendList[ListTag].date = date;
                SendList[ListTag].size = size;
                SendList[ListTag].notReadYet = true;
                SendList[ListTag].cc = textCc.Text;
                SendList[ListTag].bcc = textBcc.Text;
                SendList[ListTag].priority = priority;

                // Becky!と同じように更新後はテキストも変更
                MainForm.textBody.Text = textBody.Text;
            }

            // ListViewItemSorterを解除する
            MainForm.listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            MainForm.UpdateTreeView();
            MainForm.UpdateListView();

            // ListViewItemSorterを指定する
            MainForm.listView1.ListViewItemSorter = MainForm.listViewItemSorter;

            // 編集モードをfalseにする
            IsEdit = false;
            IsDirty = false;

            // データ変更フラグをtrueにする
            MainForm.dataDirtyFlag = true;

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

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
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

        private void menuEditFind_Click(object sender, EventArgs e)
        {
            // 二重起動を防止
            if (findDlg == null || findDlg.IsDisposed) {
                // 検索ダイアログボックス用フォームのインスタンスを生成
                findDlg = new findDialog(dialogMode.Find, textBody);
                // 検索ダイアログボックスを表示
                findDlg.Show(this);
            }
        }

        private void menuEditReplace_Click(object sender, EventArgs e)
        {
            // 二重起動を防止
            if (findDlg == null || findDlg.IsDisposed) {
                // 置換ダイアログボックス用フォームのインスタンスを生成
                findDlg = new findDialog(dialogMode.Replace, textBody);
                // 置換ダイアログボックスを表示
                findDlg.Show(this);
            }
        }

        private void menuEdit_DropDownOpening(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if (ctrl is SplitContainer) {
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if (ctrl is TextBox) {
                    this.menuEditUndo.Enabled = ((TextBox)ctrl).CanUndo;

                    // 検索対象は本文入力のみ
                    var istestBody = ((TextBox)ctrl).Name == "textBody";
                    menuEditFind.Enabled = istestBody;
                    menuEditReplace.Enabled = istestBody;

                    this.menuEditAllSelect.Enabled = ((TextBox)ctrl).Text.Length > 0;

                    var isctrlSelected = ((TextBox)ctrl).SelectionLength > 0;
                    this.menuEditCut.Enabled = isctrlSelected;
                    this.menuEditCopy.Enabled =isctrlSelected;
                    this.menuEditDelete.Enabled = isctrlSelected;
                }
            }

            // クリップボードの内容確認
            this.menuEditPaste.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if (ctrl is SplitContainer) {
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if (ctrl is TextBox) {
                    var isctrlSelected = ((TextBox)ctrl).SelectionLength > 0;
                    this.buttonCut.Enabled = isctrlSelected;
                    this.buttonCopy.Enabled = isctrlSelected;
                }
            }

            this.buttonPaste.Enabled = Clipboard.ContainsData(DataFormats.Text);
        }

        private void Form3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string d in drags) {
                    if (!System.IO.File.Exists(d)) {
                        // ファイル以外であればイベント・ハンドラを抜ける
                        return;
                    }
                }
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void Form3_DragDrop(object sender, DragEventArgs e)
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
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string fname in files) {
                appIcon = System.Drawing.Icon.ExtractAssociatedIcon(fname);
                buttonAttachList.DropDownItems.Add(fname, appIcon.ToBitmap());
            }

            // isDirtyをtrueにする
            IsDirty = true;
        }

    }
}