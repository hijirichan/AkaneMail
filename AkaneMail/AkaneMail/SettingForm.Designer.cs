namespace AkaneMail
{
    partial class SettingForm
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textUserAddress = new System.Windows.Forms.TextBox();
            this.textUserName = new System.Windows.Forms.TextBox();
            this.textPassword = new System.Windows.Forms.TextBox();
            this.textSmtpServer = new System.Windows.Forms.TextBox();
            this.textPopServer = new System.Windows.Forms.TextBox();
            this.checkApop = new System.Windows.Forms.CheckBox();
            this.checkDeleteMail = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCencel = new System.Windows.Forms.Button();
            this.checkPopBeforeSmtp = new System.Windows.Forms.CheckBox();
            this.textSmtpPortNo = new System.Windows.Forms.TextBox();
            this.textPopPortNo = new System.Windows.Forms.TextBox();
            this.textFromName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.checkAutoGetMail = new System.Windows.Forms.CheckBox();
            this.updownGetmailInterval = new System.Windows.Forms.NumericUpDown();
            this.labelIntervalRecieve = new System.Windows.Forms.Label();
            this.checkSoundPlay = new System.Windows.Forms.CheckBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textSoundFileName = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.checkBrowser = new System.Windows.Forms.CheckBox();
            this.checkMinimizeTaskTray = new System.Windows.Forms.CheckBox();
            this.checkPop3OverSSL = new System.Windows.Forms.CheckBox();
            this.checkSmtpAuth = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.updownGetmailInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "メールアドレス";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "ユーザ名";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "パスワード";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "SMTPサーバ/ポート番号";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 109);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(121, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "POP3サーバ/ポート番号";
            // 
            // textUserAddress
            // 
            this.textUserAddress.Location = new System.Drawing.Point(148, 31);
            this.textUserAddress.Name = "textUserAddress";
            this.textUserAddress.Size = new System.Drawing.Size(289, 19);
            this.textUserAddress.TabIndex = 3;
            // 
            // textUserName
            // 
            this.textUserName.Location = new System.Drawing.Point(148, 57);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(289, 19);
            this.textUserName.TabIndex = 5;
            // 
            // textPassword
            // 
            this.textPassword.Location = new System.Drawing.Point(148, 82);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(289, 19);
            this.textPassword.TabIndex = 7;
            // 
            // textSmtpServer
            // 
            this.textSmtpServer.Location = new System.Drawing.Point(149, 132);
            this.textSmtpServer.Name = "textSmtpServer";
            this.textSmtpServer.Size = new System.Drawing.Size(241, 19);
            this.textSmtpServer.TabIndex = 12;
            // 
            // textPopServer
            // 
            this.textPopServer.Location = new System.Drawing.Point(149, 106);
            this.textPopServer.Name = "textPopServer";
            this.textPopServer.Size = new System.Drawing.Size(241, 19);
            this.textPopServer.TabIndex = 9;
            // 
            // checkApop
            // 
            this.checkApop.AutoSize = true;
            this.checkApop.Location = new System.Drawing.Point(86, 157);
            this.checkApop.Name = "checkApop";
            this.checkApop.Size = new System.Drawing.Size(115, 16);
            this.checkApop.TabIndex = 14;
            this.checkApop.Text = "APOPを有効にする";
            this.checkApop.UseVisualStyleBackColor = true;
            // 
            // checkDeleteMail
            // 
            this.checkDeleteMail.AutoSize = true;
            this.checkDeleteMail.Location = new System.Drawing.Point(86, 179);
            this.checkDeleteMail.Name = "checkDeleteMail";
            this.checkDeleteMail.Size = new System.Drawing.Size(177, 16);
            this.checkDeleteMail.TabIndex = 15;
            this.checkDeleteMail.Text = "メール受信時にメールを削除する";
            this.checkDeleteMail.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(288, 365);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 24;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCencel
            // 
            this.buttonCencel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCencel.Location = new System.Drawing.Point(369, 365);
            this.buttonCencel.Name = "buttonCencel";
            this.buttonCencel.Size = new System.Drawing.Size(75, 23);
            this.buttonCencel.TabIndex = 25;
            this.buttonCencel.Text = "キャンセル";
            this.buttonCencel.UseVisualStyleBackColor = true;
            this.buttonCencel.Click += new System.EventHandler(this.buttonCencel_Click);
            // 
            // checkPopBeforeSmtp
            // 
            this.checkPopBeforeSmtp.AutoSize = true;
            this.checkPopBeforeSmtp.Location = new System.Drawing.Point(86, 201);
            this.checkPopBeforeSmtp.Name = "checkPopBeforeSmtp";
            this.checkPopBeforeSmtp.Size = new System.Drawing.Size(177, 16);
            this.checkPopBeforeSmtp.TabIndex = 16;
            this.checkPopBeforeSmtp.Text = "POP before SMTPを有効にする";
            this.checkPopBeforeSmtp.UseVisualStyleBackColor = true;
            // 
            // textSmtpPortNo
            // 
            this.textSmtpPortNo.Location = new System.Drawing.Point(396, 132);
            this.textSmtpPortNo.Name = "textSmtpPortNo";
            this.textSmtpPortNo.Size = new System.Drawing.Size(41, 19);
            this.textSmtpPortNo.TabIndex = 13;
            // 
            // textPopPortNo
            // 
            this.textPopPortNo.Location = new System.Drawing.Point(396, 107);
            this.textPopPortNo.Name = "textPopPortNo";
            this.textPopPortNo.Size = new System.Drawing.Size(41, 19);
            this.textPopPortNo.TabIndex = 10;
            // 
            // textFromName
            // 
            this.textFromName.Location = new System.Drawing.Point(147, 6);
            this.textFromName.Name = "textFromName";
            this.textFromName.Size = new System.Drawing.Size(290, 19);
            this.textFromName.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "名前";
            // 
            // checkAutoGetMail
            // 
            this.checkAutoGetMail.AutoSize = true;
            this.checkAutoGetMail.Location = new System.Drawing.Point(86, 267);
            this.checkAutoGetMail.Name = "checkAutoGetMail";
            this.checkAutoGetMail.Size = new System.Drawing.Size(133, 16);
            this.checkAutoGetMail.TabIndex = 17;
            this.checkAutoGetMail.Text = "自動受信を有効にする";
            this.checkAutoGetMail.UseVisualStyleBackColor = true;
            this.checkAutoGetMail.CheckedChanged += new System.EventHandler(this.checkAutGetMail_CheckedChanged);
            // 
            // updownGetmailInterval
            // 
            this.updownGetmailInterval.Enabled = false;
            this.updownGetmailInterval.Location = new System.Drawing.Point(225, 266);
            this.updownGetmailInterval.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.updownGetmailInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownGetmailInterval.Name = "updownGetmailInterval";
            this.updownGetmailInterval.Size = new System.Drawing.Size(38, 19);
            this.updownGetmailInterval.TabIndex = 18;
            this.updownGetmailInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelIntervalRecieve
            // 
            this.labelIntervalRecieve.AutoSize = true;
            this.labelIntervalRecieve.Enabled = false;
            this.labelIntervalRecieve.Location = new System.Drawing.Point(269, 268);
            this.labelIntervalRecieve.Name = "labelIntervalRecieve";
            this.labelIntervalRecieve.Size = new System.Drawing.Size(93, 12);
            this.labelIntervalRecieve.TabIndex = 19;
            this.labelIntervalRecieve.Text = "分間隔に受信する";
            // 
            // checkSoundPlay
            // 
            this.checkSoundPlay.AutoSize = true;
            this.checkSoundPlay.Location = new System.Drawing.Point(86, 292);
            this.checkSoundPlay.Name = "checkSoundPlay";
            this.checkSoundPlay.Size = new System.Drawing.Size(99, 16);
            this.checkSoundPlay.TabIndex = 20;
            this.checkSoundPlay.Text = "着信音を鳴らす";
            this.checkSoundPlay.UseVisualStyleBackColor = true;
            this.checkSoundPlay.CheckedChanged += new System.EventHandler(this.checkSoundPlay_CheckedChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Enabled = false;
            this.buttonBrowse.Location = new System.Drawing.Point(415, 288);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(22, 23);
            this.buttonBrowse.TabIndex = 22;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textSoundFileName
            // 
            this.textSoundFileName.Enabled = false;
            this.textSoundFileName.Location = new System.Drawing.Point(189, 290);
            this.textSoundFileName.Name = "textSoundFileName";
            this.textSoundFileName.Size = new System.Drawing.Size(220, 19);
            this.textSoundFileName.TabIndex = 21;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "WAVEファイル(*.wav)|*.wav|すべてのファイル(*.*)|*.*";
            // 
            // checkBrowser
            // 
            this.checkBrowser.AutoSize = true;
            this.checkBrowser.Location = new System.Drawing.Point(86, 314);
            this.checkBrowser.Name = "checkBrowser";
            this.checkBrowser.Size = new System.Drawing.Size(218, 16);
            this.checkBrowser.TabIndex = 23;
            this.checkBrowser.Text = "HTMLメールをIEコンポーネントで表示する";
            this.checkBrowser.UseVisualStyleBackColor = true;
            this.checkBrowser.CheckedChanged += new System.EventHandler(this.checkBrowser_CheckedChanged);
            // 
            // checkMinimizeTaskTray
            // 
            this.checkMinimizeTaskTray.AutoSize = true;
            this.checkMinimizeTaskTray.Location = new System.Drawing.Point(86, 336);
            this.checkMinimizeTaskTray.Name = "checkMinimizeTaskTray";
            this.checkMinimizeTaskTray.Size = new System.Drawing.Size(184, 16);
            this.checkMinimizeTaskTray.TabIndex = 26;
            this.checkMinimizeTaskTray.Text = "最小化時にタスクトレイに格納する";
            this.checkMinimizeTaskTray.UseVisualStyleBackColor = true;
            // 
            // checkPop3OverSSL
            // 
            this.checkPop3OverSSL.AutoSize = true;
            this.checkPop3OverSSL.Location = new System.Drawing.Point(86, 223);
            this.checkPop3OverSSL.Name = "checkPop3OverSSL";
            this.checkPop3OverSSL.Size = new System.Drawing.Size(189, 16);
            this.checkPop3OverSSL.TabIndex = 27;
            this.checkPop3OverSSL.Text = "POP3 over SSL/TLSを有効にする";
            this.checkPop3OverSSL.UseVisualStyleBackColor = true;
            // 
            // checkSmtpAuth
            // 
            this.checkSmtpAuth.AutoSize = true;
            this.checkSmtpAuth.Location = new System.Drawing.Point(86, 245);
            this.checkSmtpAuth.Name = "checkSmtpAuth";
            this.checkSmtpAuth.Size = new System.Drawing.Size(139, 16);
            this.checkSmtpAuth.TabIndex = 28;
            this.checkSmtpAuth.Text = "SMTP認証を有効にする";
            this.checkSmtpAuth.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 400);
            this.Controls.Add(this.checkSmtpAuth);
            this.Controls.Add(this.checkPop3OverSSL);
            this.Controls.Add(this.checkMinimizeTaskTray);
            this.Controls.Add(this.checkBrowser);
            this.Controls.Add(this.textSoundFileName);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.checkSoundPlay);
            this.Controls.Add(this.labelIntervalRecieve);
            this.Controls.Add(this.updownGetmailInterval);
            this.Controls.Add(this.checkAutoGetMail);
            this.Controls.Add(this.textFromName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textPopPortNo);
            this.Controls.Add(this.textSmtpPortNo);
            this.Controls.Add(this.checkPopBeforeSmtp);
            this.Controls.Add(this.buttonCencel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.checkDeleteMail);
            this.Controls.Add(this.checkApop);
            this.Controls.Add(this.textPopServer);
            this.Controls.Add(this.textSmtpServer);
            this.Controls.Add(this.textPassword);
            this.Controls.Add(this.textUserName);
            this.Controls.Add(this.textUserAddress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "環境設定";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.updownGetmailInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textUserAddress;
        private System.Windows.Forms.TextBox textUserName;
        private System.Windows.Forms.TextBox textPassword;
        private System.Windows.Forms.TextBox textSmtpServer;
        private System.Windows.Forms.TextBox textPopServer;
        private System.Windows.Forms.CheckBox checkApop;
        private System.Windows.Forms.CheckBox checkDeleteMail;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCencel;
        private System.Windows.Forms.CheckBox checkPopBeforeSmtp;
        private System.Windows.Forms.TextBox textSmtpPortNo;
        private System.Windows.Forms.TextBox textPopPortNo;
        private System.Windows.Forms.TextBox textFromName;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.CheckBox checkAutoGetMail;
        private System.Windows.Forms.NumericUpDown updownGetmailInterval;
        private System.Windows.Forms.Label labelIntervalRecieve;
        private System.Windows.Forms.CheckBox checkSoundPlay;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textSoundFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox checkBrowser;
        private System.Windows.Forms.CheckBox checkMinimizeTaskTray;
        private System.Windows.Forms.CheckBox checkPop3OverSSL;
        private System.Windows.Forms.CheckBox checkSmtpAuth;
    }
}