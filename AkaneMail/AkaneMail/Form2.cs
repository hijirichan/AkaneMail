using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AkaneMail
{
    public partial class Form2 : Form
    {
        // フォーム表示フラグ
        public bool loadFromFlag = false;

        public Form2()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // アカウント情報を設定する
            Mail.fromName = textFromName.Text;
            Mail.mailAddress = textUserAddress.Text;
            Mail.userName = textUserName.Text;
            Mail.passWord = textPassword.Text;
            Mail.smtpServer = textSmtpServer.Text;
            Mail.smtpPortNumber = int.Parse(textSmtpPortNo.Text);
            Mail.popServer = textPopServer.Text;
            Mail.popPortNumber = int.Parse(textPopPortNo.Text);
            Mail.apopFlag = checkApop.Checked;
            Mail.deleteMail = checkDeleteMail.Checked;
            Mail.popBeforeSMTP = checkPopBeforeSmtp.Checked;
            Mail.popOverSSL = checkPop3OverSSL.Checked;
            Mail.autoMailFlag = checkAutGetMail.Checked;
            Mail.getMailInterval = Int32.Parse(updownGetmailInterval.Value.ToString());
            Mail.popSoundFlag = checkSoundPlay.Checked;
            Mail.popSoundName = textSoundFileName.Text;
            Mail.bodyIEShow = checkBrowser.Checked;
            Mail.minimizeTaskTray = checkMinimizeTaskTray.Checked;
            
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
            if(Mail.mailAddress != null && Mail.userName != null && Mail.passWord != null && Mail.smtpServer != null && Mail.popServer != null){
                textFromName.Text = Mail.fromName;
                textUserAddress.Text = Mail.mailAddress;
                textUserName.Text = Mail.userName;
                textPassword.Text = Mail.passWord;
                textSmtpServer.Text = Mail.smtpServer;
                textSmtpPortNo.Text = Mail.smtpPortNumber.ToString();
                textPopServer.Text = Mail.popServer;
                textPopPortNo.Text = Mail.popPortNumber.ToString();
                checkApop.Checked = Mail.apopFlag;
                checkDeleteMail.Checked = Mail.deleteMail;
                checkPopBeforeSmtp.Checked = Mail.popBeforeSMTP;
                checkPop3OverSSL.Checked = Mail.popOverSSL;
                checkAutGetMail.Checked = Mail.autoMailFlag;
                checkMinimizeTaskTray.Checked = Mail.minimizeTaskTray;

                // 旧バージョンのコンフィグ用対策
                if(Mail.getMailInterval != 0){
                    updownGetmailInterval.Value = Mail.getMailInterval;
                }
                else{
                    updownGetmailInterval.Value = 10;
                }

                checkSoundPlay.Checked = Mail.popSoundFlag;
                textSoundFileName.Text = Mail.popSoundName;
                checkBrowser.Checked = Mail.bodyIEShow;

                if(checkAutGetMail.Checked == true){
                    updownGetmailInterval.Enabled = true;
                    label7.Enabled = true;
                }
                else{
                    updownGetmailInterval.Enabled = false;
                    label7.Enabled = false;
                }

                if(checkSoundPlay.Checked == true){
                    textSoundFileName.Enabled = true;
                    buttonBrowse.Enabled = true;
                }
                else{
                    textSoundFileName.Enabled = false;
                    buttonBrowse.Enabled = false;
                }
            }
            // フォーム読み込み完了
            loadFromFlag = false;
        }

        private void checkAutGetMail_CheckedChanged(object sender, EventArgs e)
        {
            if(checkAutGetMail.Checked == true){
                updownGetmailInterval.Enabled = true;
                label7.Enabled = true;
            }
            else{
                updownGetmailInterval.Enabled = false;
                label7.Enabled = false;
            }
        }

        private void checkSoundPlay_CheckedChanged(object sender, EventArgs e)
        {
            if(checkSoundPlay.Checked == true){
                textSoundFileName.Enabled = true;
                buttonBrowse.Enabled = true;
            }
            else{
                textSoundFileName.Enabled = false;
                buttonBrowse.Enabled = false;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            // ファイルを開くダイアログを表示する
            if(openFileDialog1.ShowDialog() == DialogResult.OK){
                if(openFileDialog1.FileName != ""){
                    textSoundFileName.Text = openFileDialog1.FileName;
                }
            }
        }

        private void checkBrowser_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBrowser.Checked == true && loadFromFlag == false) {
                if(MessageBox.Show("この機能を有効にするとIEの機能でHTMLメールを表示します。\nウイルスを実行してしまう可能性がありますが、有効にしますか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel){
                    checkBrowser.Checked = false;
                }
            }
        }
    }
}