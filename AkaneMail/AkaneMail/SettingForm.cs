using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AkaneMail
{
    public partial class SettingForm : Form
    {
        // フォーム表示フラグ
        public bool loadFromFlag = false;

        public SettingForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // アカウント情報を設定する
            AccountInfo.fromName = textFromName.Text;
            AccountInfo.mailAddress = textUserAddress.Text;
            AccountInfo.userName = textUserName.Text;
            AccountInfo.passWord = textPassword.Text;
            AccountInfo.smtpServer = textSmtpServer.Text;
            AccountInfo.smtpPortNumber = int.Parse(textSmtpPortNo.Text);
            AccountInfo.popServer = textPopServer.Text;
            AccountInfo.popPortNumber = int.Parse(textPopPortNo.Text);
            AccountInfo.apopFlag = checkApop.Checked;
            AccountInfo.deleteMail = checkDeleteMail.Checked;
            AccountInfo.popBeforeSMTP = checkPopBeforeSmtp.Checked;
            AccountInfo.popOverSSL = checkPop3OverSSL.Checked;
            AccountInfo.smtpAuth = checkSmtpAuth.Checked;
            AccountInfo.autoMailFlag = checkAutoGetMail.Checked;
            AccountInfo.getMailInterval = Int32.Parse(updownGetmailInterval.Value.ToString());
            AccountInfo.popSoundFlag = checkSoundPlay.Checked;
            AccountInfo.popSoundName = textSoundFileName.Text;
            AccountInfo.bodyIEShow = checkBrowser.Checked;
            AccountInfo.minimizeTaskTray = checkMinimizeTaskTray.Checked;

            this.Close();

        }

        private void buttonCencel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // フォームの表示準備中
            loadFromFlag = true;

            // アカウント情報を表示する
            if (AccountInfo.mailAddress != null && AccountInfo.userName != null && AccountInfo.passWord != null && AccountInfo.smtpServer != null && AccountInfo.popServer != null) {
                textFromName.Text = AccountInfo.fromName;
                textUserAddress.Text = AccountInfo.mailAddress;
                textUserName.Text = AccountInfo.userName;
                textPassword.Text = AccountInfo.passWord;
                textSmtpServer.Text = AccountInfo.smtpServer;
                textSmtpPortNo.Text = AccountInfo.smtpPortNumber.ToString();
                textPopServer.Text = AccountInfo.popServer;
                textPopPortNo.Text = AccountInfo.popPortNumber.ToString();
                checkApop.Checked = AccountInfo.apopFlag;
                checkDeleteMail.Checked = AccountInfo.deleteMail;
                checkPopBeforeSmtp.Checked = AccountInfo.popBeforeSMTP;
                checkPop3OverSSL.Checked = AccountInfo.popOverSSL;
                checkSmtpAuth.Checked = AccountInfo.smtpAuth;
                checkAutoGetMail.Checked = AccountInfo.autoMailFlag;
                checkMinimizeTaskTray.Checked = AccountInfo.minimizeTaskTray;

                // 旧バージョンのコンフィグ用対策
                if (AccountInfo.getMailInterval != 0) {
                    updownGetmailInterval.Value = AccountInfo.getMailInterval;
                }
                else {
                    updownGetmailInterval.Value = 10;
                }

                checkSoundPlay.Checked = AccountInfo.popSoundFlag;
                textSoundFileName.Text = AccountInfo.popSoundName;
                checkBrowser.Checked = AccountInfo.bodyIEShow;

                updownGetmailInterval.Enabled = checkAutoGetMail.Checked;
                labelIntervalRecieve.Enabled = checkAutoGetMail.Checked;

                textSoundFileName.Enabled = checkSoundPlay.Checked;
                buttonBrowse.Enabled = checkSoundPlay.Checked;
            }
            // フォーム読み込み完了
            loadFromFlag = false;
        }

        private void checkAutGetMail_CheckedChanged(object sender, EventArgs e)
        {
            updownGetmailInterval.Enabled = checkAutoGetMail.Checked;
            labelIntervalRecieve.Enabled = checkAutoGetMail.Checked;
        }

        private void checkSoundPlay_CheckedChanged(object sender, EventArgs e)
        {
            textSoundFileName.Enabled = checkSoundPlay.Checked;
            buttonBrowse.Enabled = checkSoundPlay.Checked;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            // ファイルを開くダイアログを表示する
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                if (openFileDialog1.FileName != "") {
                    textSoundFileName.Text = openFileDialog1.FileName;
                }
            }
        }

        private void checkBrowser_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBrowser.Checked && !loadFromFlag) {
                if (MessageBox.Show("この機能を有効にするとIEの機能でHTMLメールを表示します。\nウイルスを実行してしまう可能性がありますが、有効にしますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel) {
                    checkBrowser.Checked = false;
                }
            }
        }
    }
}