using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AkaneMail
{
    // [検索]用、[置換]用の列挙型(フラグ用)
    public enum dialogMode
    {
        Find,
        Replace
    }

    public partial class findDialog : Form
    {
        // 処理対象となる TextBox のインスタンスを保持
        private TextBox _textBox;

        // [検索]用のダイアログボックスを表示するか、
        // [置換用のダイアログボックス]を表示するかのフラグ
        private dialogMode _mode;

        private int findStartIndex = 0;
        private int findCount = 0;
        private string dialogTitle = "";

        #region Constructor // コンストラクタ

        public findDialog()
        {
            InitializeComponent();
        }

        public findDialog(TextBox txtBox)
        {
            InitializeComponent();
            _textBox = txtBox;
        }

        public findDialog(dialogMode mode)
        {
            InitializeComponent();
            Mode = mode;
        }

        public findDialog(dialogMode mode, TextBox txtBox)
        {
            InitializeComponent();
            _textBox = txtBox;
            Mode = mode;
        }

        #endregion

        #region EventHandler // イベントハンドラ

        // [キャンセル]ボタンの処理
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        // [検索]ボタンの処理
        private void findButton_Click(object sender, EventArgs e)
        {
            replaceNextButton.Enabled = ExecFind();
        }

        // [置換]ボタンの処理
        private void replaceNextButton_Click(object sender, EventArgs e)
        {
            if (_textBox.SelectionLength > 0) _textBox.SelectedText = ReplaceTextBox.Text;
            replaceNextButton.Enabled = ExecFind();
        }

        // [すべて置換]ボタンの処理
        private void replaceAllButton_Click(object sender, EventArgs e)
        {
            while (ExecFind()) {
                if (_textBox.SelectionLength > 0) _textBox.SelectedText = ReplaceTextBox.Text;
            };

        }

        // 検索開始位置ラジオボタンの処理
        private void findPosition_Radio_CheckedChanged(object sender, EventArgs e)
        {
            // 対応するラジオボタンの常に反対の値を設定
            topPosRadio.Checked = (currentPosRadio.Checked != true);
        }

        #endregion

        #region Methods // メソッド

        // 検索処理
        private bool ExecFind()
        {
            // TextBox コントロール内の文字列
            string editString = _textBox.Text;

            // 検索を行う文字列
            string findString = findTextBox.Text;

            // 検索を行う文字列の長さ
            int findStringLength = findString.Length;

            // 検索開始位置の設定([先頭から]ラジオボタンが選択されている場合は 0 = 先頭 を設定)
            int findPoint = (topPosRadio.Checked) ? 0 : _textBox.SelectionStart;

            // 二回目以降の検索位置の指定
            findStartIndex = (findStartIndex == 0) ? findPoint : findStartIndex;

            // 検索処理
            findPoint = editString.IndexOf(findString, findStartIndex, (LgSmCheckBox.Checked) ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);

            // 検索する文字列が見つからなかった場合
            if (findPoint == -1) {
                const string MSGBOX_FINDED_STRING = "ドキュメントの最後まで検索しました。\nもう一度先頭から検索しますか?";
                const string MSGBOX_REPLACED_STRING = " 件置換しました";
                string msgboxString = (_mode == dialogMode.Find) ? MSGBOX_FINDED_STRING : findCount.ToString() + MSGBOX_REPLACED_STRING;
                string msbox_NothingWord = "\"" + findString + "\"は見つかりません。";
                if (findCount != 0) {
                    if (MessageBox.Show(this, msgboxString, dialogTitle,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        ResetFindPosition();
                    }
                    else {
                        this.Close();
                        this.Dispose();
                    }
                }
                else {   // 検索の開始位置が[現在位置から]の場合は先頭から検索しなおすかいちおう確認
                    if (currentPosRadio.Checked) {
                        string msgbox_string = "現在位置から" + MSGBOX_FINDED_STRING;
                        if (MessageBox.Show(this, msgbox_string, dialogTitle,
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                            ResetFindPosition();
                        }
                        else {
                            this.Close();
                            this.Dispose();
                        }
                    }
                    else {
                        MessageBox.Show(this, msbox_NothingWord, dialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                return false;
            }
            else {
                // 見つかった文字を選択
                _textBox.Select(findPoint, findStringLength);

                // TextBox が選択された文字列を表示するようキャレットを移動
                _textBox.ScrollToCaret();

                // [次を検索] ボタンをクリックされた際の検索開始位置を設定
                findStartIndex = findPoint + findStringLength;

                // 検索がヒットした回数をカウント
                findCount++;

                // テキストボックスにフォーカスをあてる
                _textBox.Focus();
                return true;
            }
        }

        // TextBox の検索位置をリセットする処理
        private void ResetFindPosition()
        {
            findStartIndex = 0;
            findCount = 0;
            _textBox.SelectionStart = 0;
            _textBox.Select(0, 0);
            _textBox.Focus();
        }
        #endregion

        #region property // プロパティ

        // 処理対象となる TextBox のインスタンス設定するためのプロパティ
        public TextBox textBox
        {
            get { return _textBox; }
            set { _textBox = value; }
        }

        // [検索]用のダイアログボックスを表示するか、
        // [置換用のダイアログボックス]を表示するかのプロパティ
        public dialogMode Mode
        {
            get { return _mode; }
            set
            {
                const string DIALOGTITLE_FIND = "検索";
                const string DIALOGTITLE_REPLACE = "置換";
                _mode = value;
                if (_mode == dialogMode.Find) {
                    dialogTitle = DIALOGTITLE_FIND;
                    ReplacePanel.Visible = false;
                }
                else {
                    dialogTitle = DIALOGTITLE_REPLACE;
                    ReplacePanel.Visible = true;
                }
                this.Text = dialogTitle;
            }
        }
        #endregion
    }
}
