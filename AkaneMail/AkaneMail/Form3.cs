using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace AkaneMail
{
    public partial class Form3 : Form
    {
        public string attachName;
        public string[] attachFileNameList;
        findDialog findDlg = null;  // 検索ダイアログのインスタンスを格納

        private List<Mail> _sList = null;
        private bool _isDirty = false;
        private bool _isEdit = false;
        private int _listTag = 0;

        /// <summary>
        /// 親フォームクラス
        /// </summary>
        public Form1 pForm { set; get; }

        /// <summary>
        /// 送信箱の配列
        /// </summary>
        public List<Mail> sList { get; set; }

        /// <summary>
        /// テキスト変更フラグ
        /// </summary>
        public bool isDirty{ get; set; }

        /// <summary>
        /// 編集モードフラグ
        /// </summary>
        public bool isEdit { get; set; }

        /// <summary>
        /// メールデータの格納位置
        /// </summary>
        public int listTag { get; set; }

        /// <summary>
        /// 送信箱に格納するときのメールサイズ取得
        /// </summary>>
        /// <returns>メールサイズの文字列</returns>
        public string GetMailSize()
        {
            // メールサイズを格納する変数
            long _GetMailSize = 0;
            double attachSize = 0;
            string priority = "";

            string addr = Mail.fromName + " <" + Mail.mailAddress + ">";
            int addr_size = System.Text.Encoding.UTF8.GetBytes(addr).Length;
            int to_size = System.Text.Encoding.UTF8.GetBytes(textAddress.Text).Length;
            int sub_size = System.Text.Encoding.UTF8.GetBytes(textSubject.Text).Length;
            int body_size = System.Text.Encoding.UTF8.GetBytes(textBody.Text).Length;
            int cc_size = System.Text.Encoding.UTF8.GetBytes(textCc.Text).Length;
            int bcc_size = System.Text.Encoding.UTF8.GetBytes(textBcc.Text).Length;

            // 優先度の設定をする
            if(comboPriority.Text == "高い"){
                priority = "Priority: urgent";
            }
            else if(comboPriority.Text == "普通"){
                priority = "Priority: normal";
            }
            else{
                priority = "Priority: non-urgent";
            }

            int priority_size = System.Text.Encoding.UTF8.GetBytes(priority).Length;

            // 添付ファイルがあるとき
            if(attachName != ""){
                // 添付ファイルリストを分割して一覧にする
                string[] attachFileNameList = attachName.Split(',');
                for(int no = 0; no < attachFileNameList.Length; no++){
                    FileInfo fl = new FileInfo(attachFileNameList[no]);
                    attachSize = attachSize + (fl.Length * 1.33);
                }
            }

            // 合計を取得する
            _GetMailSize = addr_size + to_size + cc_size + bcc_size + priority_size + sub_size + body_size + (long)attachSize;

            return _GetMailSize.ToString();
        }

        public Form3()
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
            if(isEdit == false){
                // 重要度をNormal(普通)に設定する
                comboPriority.SelectedIndex = 1;
            }

            // isDirtyをfalseにする
            isDirty = false;
        }

        private void menuFileDirectSend_Click(object sender, EventArgs e)
        {
            string size = "";
            string attachList = "";
            string priority = "";

            // アドレスまたは本文が未入力のとき
            if(textAddress.Text == "" || textBody.Text == ""){
                if(textAddress.Text == ""){
                    // アドレス未入力エラーメッセージを表示する
                    MessageBox.Show("宛先が入力されていません。", "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(textBody.Text == ""){
                    // 本文未入力エラーメッセージを表示する
                    MessageBox.Show("本文が入力されていません。", "直接送信", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }
            
            // 件名がないときは件名に(無題)を設定する
            if(textSubject.Text == ""){
                textSubject.Text = "(無題)";
            }

            // 優先度の設定をする
            if(comboPriority.Text == "高い"){
                priority = "urgent";
            }
            else if(comboPriority.Text == "普通"){
                priority = "normal";
            }
            else{
                priority = "non-urgent";
            }

            // 文面の末尾が\r\nでないときは\r\nを付加する
            if(!textBody.Text.EndsWith("\r\n")){
                textBody.Text = textBody.Text + "\r\n";
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
                if (buttonAttachList.DropDownItems.Count == 0) {
                    buttonAttachList.Visible = false;
                }
            }

            // 削除アイテムチェック後に添付ファイルが1個以上ある場合
            if(buttonAttachList.DropDownItems.Count >= 1){
                for(int cnt = 0; cnt < buttonAttachList.DropDownItems.Count; cnt++){
                    // 添付ファイルが1個の場合(添付ファイルが複数ある場合の１回目)
                    if(cnt == 0){
                        attachList = buttonAttachList.DropDownItems[cnt].Text;
                    }
                    else{
                        // 2個以上の添付ファイルがある場合、カンマ区切りで
                        attachList = attachList + "," + buttonAttachList.DropDownItems[cnt].Text;
                    }
                }
                // 添付ファイル名のリストを変数に渡す
                attachName = attachList;
            }

            // 送信メールサイズを取得する
            size = GetMailSize();

            // 直接送信
            pForm.DirectSendMail(this.textAddress.Text, this.textCc.Text, this.textBcc.Text, this.textSubject.Text, this.textBody.Text, attachName, priority);
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            // コレクションに追加する
            Mail newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, this.attachName, date, size, "", false, "", this.textCc.Text, this.textBcc.Text, priority);
            sList.Add(newMail);

            // ListViewItemSorterを解除する
            pForm.listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            pForm.UpdateTreeView();
            pForm.UpdateListView();

            // ListViewItemSorterを指定する
            pForm.listView1.ListViewItemSorter = pForm.listViewItemSorter;

            // 編集モードをfalseにする
            isEdit = false;
            isDirty = false;
            
            // データ変更フラグをtrueにする
            pForm.dataDirtyFlag = true;

            this.Close(); 
        }

        private void menuFileAttach_Click(object sender, EventArgs e)
        {
            Icon appIcon;

            // ファイルを開くダイアログを表示する
            if(openFileDialog1.ShowDialog() == DialogResult.OK){
                if(openFileDialog1.FileName != ""){
                    buttonAttachList.Visible = true;
                    labelMessage.Text = openFileDialog1.FileName + "をメールに添付しました。";
                    appIcon = System.Drawing.Icon.ExtractAssociatedIcon(openFileDialog1.FileName);
                    buttonAttachList.DropDownItems.Add(openFileDialog1.FileName, appIcon.ToBitmap());
                    // isDirtyをtrueにする
                    isDirty = true;
                }
            }
        }

        private void menuEditUndo_Click(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer){
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(((TextBox)ctrl).CanUndo){
                        ((TextBox)ctrl).Undo();
                    }
                }
            }
        }

        private void menuEditCut_Click(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer){
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(((TextBox)ctrl).SelectionLength > 0){
                        ((TextBox)ctrl).Cut();
                    }
                }
            }
        }

        private void menuEditCopy_Click(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer){
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(((TextBox)ctrl).SelectionLength > 0){
                        ((TextBox)ctrl).Copy();
                    }
                }
            }
        }

        private void menuEditPaste_Click(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer){
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(Clipboard.ContainsData(DataFormats.Text)){
                        ((TextBox)ctrl).Paste();
                    }
                }
            }
        }

        private void menuEditAllSelect_Click(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer){
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(((TextBox)ctrl).SelectionLength == ((TextBox)ctrl).Text.Length){
                        // テキストボックスの文字列全選択を解除する
                        ((TextBox)ctrl).SelectionLength = 0;
                    }
                    else{
                        // それ以外のときはテキストの前選択をおこなう
                        ((TextBox)ctrl).SelectAll();
                    }
                }
            }
        }

        private void menuEditDelete_Click(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer){
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(((TextBox)ctrl).SelectionLength > 0){
                        ((TextBox)ctrl).SelectedText = "";
                    }
                }
            }
        }

        private void menuFileSend_Click(object sender, EventArgs e)
        {
            string size = "";
            string attachList = "";
            string priority = "";

            // アドレスまたは本文が未入力のとき
            if(textAddress.Text == "" || textBody.Text == ""){
                if(textAddress.Text == ""){
                    // アドレス未入力エラーメッセージを表示する
                    MessageBox.Show("宛先が入力されていません。", "送信箱に入れる", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if(textBody.Text == ""){
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
            if(comboPriority.Text == "高い"){
                priority = "urgent";
            }
            else if (comboPriority.Text == "普通"){
                priority = "normal";
            }
            else{
                priority = "non-urgent";
            }

            // 文面の末尾が\r\nでないときは\r\nを付加する
            if (!textBody.Text.EndsWith("\r\n")) {
                textBody.Text = textBody.Text + "\r\n";
            }

            // 添付ファイルが1個以上ある場合
            if (buttonAttachList.DropDownItems.Count >= 1) {
                for (int cnt = 0; cnt < buttonAttachList.DropDownItems.Count; cnt++) {
                    // 添付ファイルが存在しないとき
                    if(buttonAttachList.DropDownItems[cnt].Text.Contains("は削除されています。")){
                        // そのメニューを削除する
                        buttonAttachList.DropDownItems.RemoveAt(cnt);
                    }
                }

                // メニューが空になった時は添付リストの表示を非表示にする
                if (buttonAttachList.DropDownItems.Count == 0) {
                    buttonAttachList.Visible = false;
                }
            }

            // 削除アイテムチェック後に添付ファイルが1個以上ある場合
            if (buttonAttachList.DropDownItems.Count >= 1) {
                for (int cnt = 0; cnt < buttonAttachList.DropDownItems.Count; cnt++) {
                    // 添付ファイルが１個の場合(添付ファイルが複数ある場合の１回目)
                    if (cnt == 0) {
                        attachList = buttonAttachList.DropDownItems[cnt].Text;
                    } else {
                        // 2個以上の添付ファイルがある場合、カンマ区切りで
                        attachList = attachList + "," + buttonAttachList.DropDownItems[cnt].Text;
                    }
                }
                // 添付ファイル名のリストを変数に渡す
                attachName = attachList;
            }

            // 未送信メールは作成日時を格納するようにする(未送信という文字列だと日付ソートでエラーになる)
            string date = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            // 編集フラグがOffのとき
            if(isEdit == false){
                // 送信メールサイズを取得する
                size = GetMailSize();

                // Form1からのコレクションに追加してリスト更新する
                Mail newMail = new Mail(this.textAddress.Text, "", this.textSubject.Text, this.textBody.Text, attachName, date, size, "", true, "", this.textCc.Text, this.textBcc.Text, priority);
                sList.Add(newMail);
            }
            else{
                // 選択したメールの内容を書き換える
                // 送信リストに入れている情報を書き換える
                size = GetMailSize();
                ((Mail)sList[listTag]).subject = textSubject.Text;
                ((Mail)sList[listTag]).address = textAddress.Text;
                ((Mail)sList[listTag]).body = textBody.Text;
                ((Mail)sList[listTag]).attach = attachName;
                ((Mail)sList[listTag]).date = date;
                ((Mail)sList[listTag]).size = size;
                ((Mail)sList[listTag]).notReadYet = true;
                ((Mail)sList[listTag]).cc = textCc.Text;
                ((Mail)sList[listTag]).bcc = textBcc.Text;
                ((Mail)sList[listTag]).priority = priority;

                // Becky!と同じように更新後はテキストも変更
                pForm.textBody.Text = textBody.Text;
            }

            // ListViewItemSorterを解除する
            pForm.listView1.ListViewItemSorter = null;

            // ツリービューとリストビューの表示を更新する
            pForm.UpdateTreeView();
            pForm.UpdateListView();

            // ListViewItemSorterを指定する
            pForm.listView1.ListViewItemSorter = pForm.listViewItemSorter;

            // 編集モードをfalseにする
            isEdit = false;
            isDirty = false;

            // データ変更フラグをtrueにする
            pForm.dataDirtyFlag = true;

            this.Close();
        }

        private void menuHelpAbout_Click(object sender, EventArgs e)
        {
            // バージョン情報を表示する
            Form4 AboutForm = new Form4();
            AboutForm.ShowDialog();
        }

        private void textAddress_TextChanged(object sender, EventArgs e)
        {
            // isDirtyをtrueにする
            isDirty = true;
        }

        private void textSubject_TextChanged(object sender, EventArgs e)
        {
            // isDirtyをtrueにする
            isDirty = true;
        }

        private void textBody_TextChanged(object sender, EventArgs e)
        {
            // isDirtyをtrueにする
            isDirty = true;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            // isDirtyフラグがtrueのとき
            if(isDirty == true){
                if(isEdit == false){
                    if (MessageBox.Show("メールの作成途中ですが、閉じてよろしいですか？\nウィンドウを閉じると作成中のメールは保存されません。", "新規作成", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                        // ウィンドウを閉じるのをキャンセル
                        e.Cancel = true;
                    }
                }
                else{
                    if(MessageBox.Show("送信メールの編集途中ですが、閉じてよろしいですか？\nウィンドウを閉じると編集前の内容に戻ります。", "編集中", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No){
                        // ウィンドウを閉じるのをキャンセル
                        e.Cancel = true;
                    }
                }
            }
            // Appliction.Idleを削除する
            Application.Idle -= new EventHandler(Application_Idle);
        }

        private void buttonAttachList_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // 添付するファイルパスをを削除するのかを確認
            if(MessageBox.Show(e.ClickedItem.Text + "を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes){
                // 選択した添付ファイルメニューを削除する
                buttonAttachList.DropDownItems.Remove(e.ClickedItem);
                // 添付ファイルの数が0になったらリストを閉じる
                if(buttonAttachList.DropDownItems.Count == 0){
                    buttonAttachList.Visible = false;
                }
                isDirty = true;
                labelMessage.Text = "";
            }
        }

        private void menuEditFind_Click(object sender, EventArgs e)
        {
            // 二重起動を防止
            if(findDlg == null || findDlg.IsDisposed){
                // 検索ダイアログボックス用フォームのインスタンスを生成
                findDlg = new findDialog(dialogMode.Find, textBody);
                // 検索ダイアログボックスを表示
                findDlg.Show(this);
            }
        }

        private void menuEditReplace_Click(object sender, EventArgs e)
        {
            // 二重起動を防止
            if(findDlg == null || findDlg.IsDisposed){
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
                    if (((TextBox)ctrl).Name == "textBody") {
                        // 検索と置換メニューを有効にする
                        menuEditFind.Enabled = true;
                        menuEditReplace.Enabled = true;
                    } else {
                        // それ以外のときは検索と置換メニューを無効にする
                        menuEditFind.Enabled = false;
                        menuEditReplace.Enabled = false;
                    }

                    if (((TextBox)ctrl).Text.Length > 0) {
                        this.menuEditAllSelect.Enabled = true;
                    } else {
                        this.menuEditAllSelect.Enabled = false;
                    }

                    if (((TextBox)ctrl).SelectionLength > 0) {
                        this.menuEditCut.Enabled = true;
                        this.menuEditCopy.Enabled = true;
                        this.menuEditDelete.Enabled = true;
                    } else {
                        this.menuEditCut.Enabled = false;
                        this.menuEditCopy.Enabled = false;
                        this.menuEditDelete.Enabled = false;
                    }
                }
            }

            // クリップボードの内容確認
            if (Clipboard.ContainsData(DataFormats.Text)) {
                this.menuEditPaste.Enabled = true;
            } else {
                this.menuEditPaste.Enabled = false;
            }
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            Control ctrl = this.ActiveControl;

            // Spliterコントロール配下のコントロールを取得する
            if(ctrl is SplitContainer) {
                ctrl = (ctrl as SplitContainer).ActiveControl;
                if(ctrl is TextBox){
                    if(((TextBox)ctrl).SelectionLength > 0){
                        this.buttonCut.Enabled = true;
                        this.buttonCopy.Enabled = true;
                    }
                    else{
                        this.buttonCut.Enabled = false;
                        this.buttonCopy.Enabled = false;
                    }
                }
            }

            if(Clipboard.ContainsData(DataFormats.Text)){
                this.buttonPaste.Enabled = true;
            }
            else{
                this.buttonPaste.Enabled = false;
            }
        }

        private void Form3_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop)){
                // ドラッグ中のファイルやディレクトリの取得
                string[] drags = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string d in drags){
                    if(!System.IO.File.Exists(d)){
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
            if(buttonAttachList.DropDownItems.Count >= 1){
                for(int cnt = 0; cnt < buttonAttachList.DropDownItems.Count; cnt++){
                    buttonAttachList.DropDownItems.RemoveAt(cnt);
                }
            }

            buttonAttachList.Visible = true;

            // ドラッグ＆ドロップされたファイルを添付ファイルリストに追加する
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string fname in files){
                appIcon = System.Drawing.Icon.ExtractAssociatedIcon(fname);
                buttonAttachList.DropDownItems.Add(fname, appIcon.ToBitmap());
            }

            // isDirtyをtrueにする
            isDirty = true;
        }

    }
}