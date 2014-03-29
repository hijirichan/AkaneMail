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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.updownGetmailInterval)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 37);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 36, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "メールアドレス";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 67);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 36, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "ユーザ名";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 97);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 36, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "パスワード";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 157);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 36, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(155, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "SMTPサーバ/ポート番号";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 127);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 36, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(154, 15);
            this.label5.TabIndex = 8;
            this.label5.Text = "POP3サーバ/ポート番号";
            // 
            // textUserAddress
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textUserAddress, 2);
            this.textUserAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textUserAddress.Location = new System.Drawing.Point(199, 34);
            this.textUserAddress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textUserAddress.Name = "textUserAddress";
            this.textUserAddress.Size = new System.Drawing.Size(387, 22);
            this.textUserAddress.TabIndex = 3;
            // 
            // textUserName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textUserName, 2);
            this.textUserName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textUserName.Location = new System.Drawing.Point(199, 64);
            this.textUserName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textUserName.Name = "textUserName";
            this.textUserName.Size = new System.Drawing.Size(387, 22);
            this.textUserName.TabIndex = 5;
            // 
            // textPassword
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textPassword, 2);
            this.textPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textPassword.Location = new System.Drawing.Point(199, 94);
            this.textPassword.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textPassword.Name = "textPassword";
            this.textPassword.PasswordChar = '*';
            this.textPassword.Size = new System.Drawing.Size(387, 22);
            this.textPassword.TabIndex = 7;
            // 
            // textSmtpServer
            // 
            this.textSmtpServer.Location = new System.Drawing.Point(199, 154);
            this.textSmtpServer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textSmtpServer.Name = "textSmtpServer";
            this.textSmtpServer.Size = new System.Drawing.Size(320, 22);
            this.textSmtpServer.TabIndex = 12;
            // 
            // textPopServer
            // 
            this.textPopServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textPopServer.Location = new System.Drawing.Point(199, 124);
            this.textPopServer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textPopServer.Name = "textPopServer";
            this.textPopServer.Size = new System.Drawing.Size(320, 22);
            this.textPopServer.TabIndex = 9;
            // 
            // checkApop
            // 
            this.checkApop.AutoSize = true;
            this.checkApop.Location = new System.Drawing.Point(124, 8);
            this.checkApop.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkApop.Name = "checkApop";
            this.checkApop.Size = new System.Drawing.Size(144, 19);
            this.checkApop.TabIndex = 14;
            this.checkApop.Text = "APOPを有効にする";
            this.checkApop.UseVisualStyleBackColor = true;
            // 
            // checkDeleteMail
            // 
            this.checkDeleteMail.AutoSize = true;
            this.checkDeleteMail.Location = new System.Drawing.Point(124, 35);
            this.checkDeleteMail.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkDeleteMail.Name = "checkDeleteMail";
            this.checkDeleteMail.Size = new System.Drawing.Size(219, 19);
            this.checkDeleteMail.TabIndex = 15;
            this.checkDeleteMail.Text = "メール受信時にメールを削除する";
            this.checkDeleteMail.UseVisualStyleBackColor = true;
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(378, 4);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(100, 29);
            this.buttonOK.TabIndex = 24;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCencel
            // 
            this.buttonCencel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCencel.Location = new System.Drawing.Point(486, 4);
            this.buttonCencel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonCencel.Name = "buttonCencel";
            this.buttonCencel.Size = new System.Drawing.Size(100, 29);
            this.buttonCencel.TabIndex = 25;
            this.buttonCencel.Text = "キャンセル";
            this.buttonCencel.UseVisualStyleBackColor = true;
            this.buttonCencel.Click += new System.EventHandler(this.buttonCencel_Click);
            // 
            // checkPopBeforeSmtp
            // 
            this.checkPopBeforeSmtp.AutoSize = true;
            this.checkPopBeforeSmtp.Location = new System.Drawing.Point(124, 62);
            this.checkPopBeforeSmtp.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkPopBeforeSmtp.Name = "checkPopBeforeSmtp";
            this.checkPopBeforeSmtp.Size = new System.Drawing.Size(224, 19);
            this.checkPopBeforeSmtp.TabIndex = 16;
            this.checkPopBeforeSmtp.Text = "POP before SMTPを有効にする";
            this.checkPopBeforeSmtp.UseVisualStyleBackColor = true;
            // 
            // textSmtpPortNo
            // 
            this.textSmtpPortNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.textSmtpPortNo.Location = new System.Drawing.Point(530, 154);
            this.textSmtpPortNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textSmtpPortNo.Name = "textSmtpPortNo";
            this.textSmtpPortNo.Size = new System.Drawing.Size(56, 22);
            this.textSmtpPortNo.TabIndex = 13;
            // 
            // textPopPortNo
            // 
            this.textPopPortNo.Dock = System.Windows.Forms.DockStyle.Right;
            this.textPopPortNo.Location = new System.Drawing.Point(530, 124);
            this.textPopPortNo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textPopPortNo.Name = "textPopPortNo";
            this.textPopPortNo.Size = new System.Drawing.Size(56, 22);
            this.textPopPortNo.TabIndex = 10;
            // 
            // textFromName
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.textFromName, 2);
            this.textFromName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textFromName.Location = new System.Drawing.Point(199, 4);
            this.textFromName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textFromName.Name = "textFromName";
            this.textFromName.Size = new System.Drawing.Size(387, 22);
            this.textFromName.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(4, 7);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 36, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "名前";
            // 
            // checkAutoGetMail
            // 
            this.checkAutoGetMail.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkAutoGetMail.AutoSize = true;
            this.checkAutoGetMail.Location = new System.Drawing.Point(4, 4);
            this.checkAutoGetMail.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkAutoGetMail.Name = "checkAutoGetMail";
            this.checkAutoGetMail.Size = new System.Drawing.Size(166, 19);
            this.checkAutoGetMail.TabIndex = 17;
            this.checkAutoGetMail.Text = "自動受信を有効にする";
            this.checkAutoGetMail.UseVisualStyleBackColor = true;
            this.checkAutoGetMail.CheckedChanged += new System.EventHandler(this.checkAutGetMail_CheckedChanged);
            // 
            // updownGetmailInterval
            // 
            this.updownGetmailInterval.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.updownGetmailInterval.Enabled = false;
            this.updownGetmailInterval.Location = new System.Drawing.Point(174, 2);
            this.updownGetmailInterval.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
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
            this.updownGetmailInterval.Size = new System.Drawing.Size(51, 22);
            this.updownGetmailInterval.TabIndex = 18;
            this.updownGetmailInterval.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labelIntervalRecieve
            // 
            this.labelIntervalRecieve.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelIntervalRecieve.AutoSize = true;
            this.labelIntervalRecieve.Enabled = false;
            this.labelIntervalRecieve.Location = new System.Drawing.Point(233, 6);
            this.labelIntervalRecieve.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelIntervalRecieve.Name = "labelIntervalRecieve";
            this.labelIntervalRecieve.Size = new System.Drawing.Size(118, 15);
            this.labelIntervalRecieve.TabIndex = 19;
            this.labelIntervalRecieve.Text = "分間隔に受信する";
            // 
            // checkSoundPlay
            // 
            this.checkSoundPlay.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.checkSoundPlay.AutoSize = true;
            this.checkSoundPlay.Location = new System.Drawing.Point(4, 4);
            this.checkSoundPlay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkSoundPlay.Name = "checkSoundPlay";
            this.checkSoundPlay.Size = new System.Drawing.Size(123, 19);
            this.checkSoundPlay.TabIndex = 20;
            this.checkSoundPlay.Text = "着信音を鳴らす";
            this.checkSoundPlay.UseVisualStyleBackColor = true;
            this.checkSoundPlay.CheckedChanged += new System.EventHandler(this.checkSoundPlay_CheckedChanged);
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonBrowse.Enabled = false;
            this.buttonBrowse.Location = new System.Drawing.Point(435, 0);
            this.buttonBrowse.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(28, 28);
            this.buttonBrowse.TabIndex = 22;
            this.buttonBrowse.Text = "...";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // textSoundFileName
            // 
            this.textSoundFileName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.textSoundFileName.Enabled = false;
            this.textSoundFileName.Location = new System.Drawing.Point(131, 3);
            this.textSoundFileName.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.textSoundFileName.Name = "textSoundFileName";
            this.textSoundFileName.Size = new System.Drawing.Size(296, 22);
            this.textSoundFileName.TabIndex = 21;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "WAVEファイル(*.wav)|*.wav|すべてのファイル(*.*)|*.*";
            // 
            // checkBrowser
            // 
            this.checkBrowser.AutoSize = true;
            this.checkBrowser.Location = new System.Drawing.Point(124, 198);
            this.checkBrowser.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBrowser.Name = "checkBrowser";
            this.checkBrowser.Size = new System.Drawing.Size(269, 19);
            this.checkBrowser.TabIndex = 23;
            this.checkBrowser.Text = "HTMLメールをIEコンポーネントで表示する";
            this.checkBrowser.UseVisualStyleBackColor = true;
            this.checkBrowser.CheckedChanged += new System.EventHandler(this.checkBrowser_CheckedChanged);
            // 
            // checkMinimizeTaskTray
            // 
            this.checkMinimizeTaskTray.AutoSize = true;
            this.checkMinimizeTaskTray.Location = new System.Drawing.Point(124, 225);
            this.checkMinimizeTaskTray.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkMinimizeTaskTray.Name = "checkMinimizeTaskTray";
            this.checkMinimizeTaskTray.Size = new System.Drawing.Size(231, 19);
            this.checkMinimizeTaskTray.TabIndex = 26;
            this.checkMinimizeTaskTray.Text = "最小化時にタスクトレイに格納する";
            this.checkMinimizeTaskTray.UseVisualStyleBackColor = true;
            // 
            // checkPop3OverSSL
            // 
            this.checkPop3OverSSL.AutoSize = true;
            this.checkPop3OverSSL.Location = new System.Drawing.Point(124, 89);
            this.checkPop3OverSSL.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkPop3OverSSL.Name = "checkPop3OverSSL";
            this.checkPop3OverSSL.Size = new System.Drawing.Size(241, 19);
            this.checkPop3OverSSL.TabIndex = 27;
            this.checkPop3OverSSL.Text = "POP3 over SSL/TLSを有効にする";
            this.checkPop3OverSSL.UseVisualStyleBackColor = true;
            // 
            // checkSmtpAuth
            // 
            this.checkSmtpAuth.AutoSize = true;
            this.checkSmtpAuth.Location = new System.Drawing.Point(124, 116);
            this.checkSmtpAuth.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkSmtpAuth.Name = "checkSmtpAuth";
            this.checkSmtpAuth.Size = new System.Drawing.Size(174, 19);
            this.checkSmtpAuth.TabIndex = 28;
            this.checkSmtpAuth.Text = "SMTP認証を有効にする";
            this.checkSmtpAuth.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textFromName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textUserAddress, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textUserName, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.textPassword, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.textPopServer, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.textPopPortNo, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textSmtpServer, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.textSmtpPortNo, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(590, 180);
            this.tableLayoutPanel1.TabIndex = 29;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.checkAutoGetMail);
            this.flowLayoutPanel1.Controls.Add(this.updownGetmailInterval);
            this.flowLayoutPanel1.Controls.Add(this.labelIntervalRecieve);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(120, 139);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(355, 27);
            this.flowLayoutPanel1.TabIndex = 30;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.checkSoundPlay);
            this.flowLayoutPanel2.Controls.Add(this.textSoundFileName);
            this.flowLayoutPanel2.Controls.Add(this.buttonBrowse);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(120, 166);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(467, 28);
            this.flowLayoutPanel2.TabIndex = 31;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.checkApop);
            this.flowLayoutPanel3.Controls.Add(this.checkDeleteMail);
            this.flowLayoutPanel3.Controls.Add(this.checkPopBeforeSmtp);
            this.flowLayoutPanel3.Controls.Add(this.checkPop3OverSSL);
            this.flowLayoutPanel3.Controls.Add(this.checkSmtpAuth);
            this.flowLayoutPanel3.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel3.Controls.Add(this.flowLayoutPanel2);
            this.flowLayoutPanel3.Controls.Add(this.checkBrowser);
            this.flowLayoutPanel3.Controls.Add(this.checkMinimizeTaskTray);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(12, 192);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(12);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(120, 4, 0, 0);
            this.flowLayoutPanel3.Size = new System.Drawing.Size(590, 296);
            this.flowLayoutPanel3.TabIndex = 32;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this.buttonCencel);
            this.flowLayoutPanel4.Controls.Add(this.buttonOK);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(12, 451);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(590, 37);
            this.flowLayoutPanel4.TabIndex = 33;
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 500);
            this.Controls.Add(this.flowLayoutPanel4);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.Padding = new System.Windows.Forms.Padding(12);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "環境設定";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.updownGetmailInterval)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
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
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
    }
}