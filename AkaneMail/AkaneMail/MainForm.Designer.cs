namespace AkaneMail
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("受信メール (0)", 1, 1);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("送信メール (0)", 2, 2);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("ごみ箱 (0)", 3, 3);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("メールボックス", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSaveMailFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileGetAttatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileClearTrush = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMailSend = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMailRecieve = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMailNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMailReturnMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMailFowerdMail = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuMailDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.オプションOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuToolSetEnv = new System.Windows.Forms.ToolStripMenuItem();
            this.ヘルプHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressMail = new System.Windows.Forms.ToolStripProgressBar();
            this.buttonAttachList = new System.Windows.Forms.ToolStripDropDownButton();
            this.labelDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonMailSend = new System.Windows.Forms.ToolStripButton();
            this.buttonMailRecieve = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonMailNew = new System.Windows.Forms.ToolStripButton();
            this.buttonReturnMail = new System.Windows.Forms.ToolStripButton();
            this.buttonForwardMail = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonMailDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonHelp = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuClearTrush = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnFromTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuReturnMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFowerdMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAlreadyRead = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNotReadYet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuGetAttach = new System.Windows.Forms.ToolStripMenuItem();
            this.textBody = new System.Windows.Forms.TextBox();
            this.browserBody = new System.Windows.Forms.WebBrowser();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuTaskRestoreWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.menuTaskMailNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.menuTaskApplicationExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.menuTreeView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.menuListView.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuMail,
            this.オプションOToolStripMenuItem,
            this.ヘルプHToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 26);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSaveMailFile,
            this.menuFileGetAttatch,
            this.toolStripSeparator6,
            this.menuFileClearTrush,
            this.toolStripSeparator5,
            this.menuFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(85, 22);
            this.menuFile.Text = "ファイル(&F)";
            this.menuFile.DropDownOpening += new System.EventHandler(this.menuFile_DropDownOpening);
            // 
            // menuSaveMailFile
            // 
            this.menuSaveMailFile.Name = "menuSaveMailFile";
            this.menuSaveMailFile.Size = new System.Drawing.Size(239, 22);
            this.menuSaveMailFile.Text = "名前を付けて保存(&A)...";
            this.menuSaveMailFile.Click += new System.EventHandler(this.menuSaveMailFile_Click);
            // 
            // menuFileGetAttatch
            // 
            this.menuFileGetAttatch.Name = "menuFileGetAttatch";
            this.menuFileGetAttatch.Size = new System.Drawing.Size(239, 22);
            this.menuFileGetAttatch.Text = "添付ファイルを取り出す(&G)...";
            this.menuFileGetAttatch.Click += new System.EventHandler(this.menuFileGetAttatch_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(236, 6);
            // 
            // menuFileClearTrush
            // 
            this.menuFileClearTrush.Name = "menuFileClearTrush";
            this.menuFileClearTrush.Size = new System.Drawing.Size(239, 22);
            this.menuFileClearTrush.Text = "ごみ箱を空にする(&Y)...";
            this.menuFileClearTrush.Click += new System.EventHandler(this.menuFileClearTrush_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(236, 6);
            // 
            // menuFileExit
            // 
            this.menuFileExit.Name = "menuFileExit";
            this.menuFileExit.Size = new System.Drawing.Size(239, 22);
            this.menuFileExit.Text = "アプリケーションの終了(&X)";
            this.menuFileExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // menuMail
            // 
            this.menuMail.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMailSend,
            this.menuMailRecieve,
            this.toolStripMenuItem3,
            this.menuMailNew,
            this.menuMailReturnMail,
            this.menuMailFowerdMail,
            this.toolStripMenuItem4,
            this.menuMailDelete});
            this.menuMail.Name = "menuMail";
            this.menuMail.Size = new System.Drawing.Size(76, 22);
            this.menuMail.Text = "メール(&M)";
            this.menuMail.DropDownOpening += new System.EventHandler(this.menuMail_DropDownOpening);
            // 
            // menuMailSend
            // 
            this.menuMailSend.Name = "menuMailSend";
            this.menuMailSend.Size = new System.Drawing.Size(167, 22);
            this.menuMailSend.Text = "送信(&S)";
            this.menuMailSend.Click += new System.EventHandler(this.menuMailSend_Click);
            // 
            // menuMailRecieve
            // 
            this.menuMailRecieve.Name = "menuMailRecieve";
            this.menuMailRecieve.Size = new System.Drawing.Size(167, 22);
            this.menuMailRecieve.Text = "受信(&M)";
            this.menuMailRecieve.Click += new System.EventHandler(this.menuMailRecieve_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(164, 6);
            // 
            // menuMailNew
            // 
            this.menuMailNew.Name = "menuMailNew";
            this.menuMailNew.Size = new System.Drawing.Size(167, 22);
            this.menuMailNew.Text = "新規作成(&N)";
            this.menuMailNew.Click += new System.EventHandler(this.menuMailNew_Click);
            // 
            // menuMailReturnMail
            // 
            this.menuMailReturnMail.Name = "menuMailReturnMail";
            this.menuMailReturnMail.Size = new System.Drawing.Size(167, 22);
            this.menuMailReturnMail.Text = "返信(&R)";
            this.menuMailReturnMail.Click += new System.EventHandler(this.menuMailReturnMail_Click);
            // 
            // menuMailFowerdMail
            // 
            this.menuMailFowerdMail.Name = "menuMailFowerdMail";
            this.menuMailFowerdMail.Size = new System.Drawing.Size(167, 22);
            this.menuMailFowerdMail.Text = "転送(&T)";
            this.menuMailFowerdMail.Click += new System.EventHandler(this.menuMailFowerdMail_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(164, 6);
            // 
            // menuMailDelete
            // 
            this.menuMailDelete.Name = "menuMailDelete";
            this.menuMailDelete.Size = new System.Drawing.Size(167, 22);
            this.menuMailDelete.Text = "メールの削除(&D)";
            this.menuMailDelete.Click += new System.EventHandler(this.menuMailDelete_Click);
            // 
            // オプションOToolStripMenuItem
            // 
            this.オプションOToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolSetEnv});
            this.オプションOToolStripMenuItem.Name = "オプションOToolStripMenuItem";
            this.オプションOToolStripMenuItem.Size = new System.Drawing.Size(74, 22);
            this.オプションOToolStripMenuItem.Text = "ツール(&T)";
            // 
            // menuToolSetEnv
            // 
            this.menuToolSetEnv.Name = "menuToolSetEnv";
            this.menuToolSetEnv.Size = new System.Drawing.Size(142, 22);
            this.menuToolSetEnv.Text = "環境設定(&S)";
            this.menuToolSetEnv.Click += new System.EventHandler(this.menuToolSetEnv_Click);
            // 
            // ヘルプHToolStripMenuItem
            // 
            this.ヘルプHToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelp,
            this.toolStripMenuItem2,
            this.menuHelpAbout});
            this.ヘルプHToolStripMenuItem.Name = "ヘルプHToolStripMenuItem";
            this.ヘルプHToolStripMenuItem.Size = new System.Drawing.Size(75, 22);
            this.ヘルプHToolStripMenuItem.Text = "ヘルプ(&H)";
            // 
            // menuHelp
            // 
            this.menuHelp.Enabled = false;
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(178, 22);
            this.menuHelp.Text = "目次(&H)";
            this.menuHelp.Visible = false;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(175, 6);
            this.toolStripMenuItem2.Visible = false;
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(178, 22);
            this.menuHelpAbout.Text = "バージョン情報(&A)";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelMessage,
            this.progressMail,
            this.buttonAttachList,
            this.labelDate});
            this.statusStrip1.Location = new System.Drawing.Point(0, 535);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 27);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelMessage
            // 
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(697, 22);
            this.labelMessage.Spring = true;
            this.labelMessage.Text = "現在のステータス";
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressMail
            // 
            this.progressMail.Name = "progressMail";
            this.progressMail.Size = new System.Drawing.Size(100, 21);
            this.progressMail.Visible = false;
            // 
            // buttonAttachList
            // 
            this.buttonAttachList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAttachList.Image = ((System.Drawing.Image)(resources.GetObject("buttonAttachList.Image")));
            this.buttonAttachList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAttachList.Name = "buttonAttachList";
            this.buttonAttachList.Size = new System.Drawing.Size(29, 25);
            this.buttonAttachList.ToolTipText = "添付ファイル";
            this.buttonAttachList.Visible = false;
            this.buttonAttachList.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.buttonAttachList_DropDownItemClicked);
            // 
            // labelDate
            // 
            this.labelDate.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(72, 22);
            this.labelDate.Text = "現在の時刻";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonMailSend,
            this.buttonMailRecieve,
            this.toolStripSeparator1,
            this.buttonMailNew,
            this.buttonReturnMail,
            this.buttonForwardMail,
            this.toolStripSeparator4,
            this.buttonMailDelete,
            this.toolStripSeparator2,
            this.buttonHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 26);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonMailSend
            // 
            this.buttonMailSend.Image = ((System.Drawing.Image)(resources.GetObject("buttonMailSend.Image")));
            this.buttonMailSend.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMailSend.Name = "buttonMailSend";
            this.buttonMailSend.Size = new System.Drawing.Size(52, 22);
            this.buttonMailSend.Text = "送信";
            this.buttonMailSend.Click += new System.EventHandler(this.menuMailSend_Click);
            // 
            // buttonMailRecieve
            // 
            this.buttonMailRecieve.Image = ((System.Drawing.Image)(resources.GetObject("buttonMailRecieve.Image")));
            this.buttonMailRecieve.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMailRecieve.Name = "buttonMailRecieve";
            this.buttonMailRecieve.Size = new System.Drawing.Size(52, 22);
            this.buttonMailRecieve.Text = "受信";
            this.buttonMailRecieve.Click += new System.EventHandler(this.menuMailRecieve_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonMailNew
            // 
            this.buttonMailNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonMailNew.Image")));
            this.buttonMailNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMailNew.Name = "buttonMailNew";
            this.buttonMailNew.Size = new System.Drawing.Size(76, 22);
            this.buttonMailNew.Text = "新規作成";
            this.buttonMailNew.Click += new System.EventHandler(this.menuMailNew_Click);
            // 
            // buttonReturnMail
            // 
            this.buttonReturnMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonReturnMail.Image")));
            this.buttonReturnMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReturnMail.Name = "buttonReturnMail";
            this.buttonReturnMail.Size = new System.Drawing.Size(52, 22);
            this.buttonReturnMail.Text = "返信";
            this.buttonReturnMail.Click += new System.EventHandler(this.menuMailReturnMail_Click);
            // 
            // buttonForwardMail
            // 
            this.buttonForwardMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonForwardMail.Image")));
            this.buttonForwardMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonForwardMail.Name = "buttonForwardMail";
            this.buttonForwardMail.Size = new System.Drawing.Size(52, 22);
            this.buttonForwardMail.Text = "転送";
            this.buttonForwardMail.Click += new System.EventHandler(this.menuMailFowerdMail_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonMailDelete
            // 
            this.buttonMailDelete.Image = ((System.Drawing.Image)(resources.GetObject("buttonMailDelete.Image")));
            this.buttonMailDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonMailDelete.Name = "buttonMailDelete";
            this.buttonMailDelete.Size = new System.Drawing.Size(52, 22);
            this.buttonMailDelete.Text = "削除";
            this.buttonMailDelete.Click += new System.EventHandler(this.menuMailDelete_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonHelp
            // 
            this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
            this.buttonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(64, 22);
            this.buttonHelp.Text = "ヘルプ";
            this.buttonHelp.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 51);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(784, 484);
            this.splitContainer1.SplitterDistance = 159;
            this.splitContainer1.TabIndex = 3;
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.menuTreeView;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "nodeReceive";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Tag = "ReceiveMailBox";
            treeNode1.Text = "受信メール (0)";
            treeNode2.ImageIndex = 2;
            treeNode2.Name = "nodeSend";
            treeNode2.SelectedImageIndex = 2;
            treeNode2.Tag = "SendMailBox";
            treeNode2.Text = "送信メール (0)";
            treeNode3.ImageIndex = 3;
            treeNode3.Name = "nodeDelete";
            treeNode3.SelectedImageIndex = 3;
            treeNode3.Tag = "DeleteMailBox";
            treeNode3.Text = "ごみ箱 (0)";
            treeNode4.Name = "rootMail";
            treeNode4.Tag = "MailBoxRoot";
            treeNode4.Text = "メールボックス";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(159, 484);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // menuTreeView
            // 
            this.menuTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClearTrush});
            this.menuTreeView.Name = "contextMenuStrip2";
            this.menuTreeView.Size = new System.Drawing.Size(203, 26);
            this.menuTreeView.Opening += new System.ComponentModel.CancelEventHandler(this.menuTreeView_Opening);
            // 
            // menuClearTrush
            // 
            this.menuClearTrush.Name = "menuClearTrush";
            this.menuClearTrush.Size = new System.Drawing.Size(202, 22);
            this.menuClearTrush.Text = "ごみ箱を空にする(&Y)...";
            this.menuClearTrush.Click += new System.EventHandler(this.menuFileClearTrush_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "box_normal_r.gif");
            this.imageList1.Images.SetKeyName(1, "box_receive_r.gif");
            this.imageList1.Images.SetKeyName(2, "box_send_r.gif");
            this.imageList1.Images.SetKeyName(3, "trash_r.gif");
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listView1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBody);
            this.splitContainer2.Panel2.Controls.Add(this.browserBody);
            this.splitContainer2.Size = new System.Drawing.Size(621, 484);
            this.splitContainer2.SplitterDistance = 183;
            this.splitContainer2.TabIndex = 0;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFromTo,
            this.columnSubject,
            this.columnDate,
            this.columnSize});
            this.listView1.ContextMenuStrip = this.menuListView;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(621, 183);
            this.listView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            this.listView1.Click += new System.EventHandler(this.listView1_Click);
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            // 
            // columnFromTo
            // 
            this.columnFromTo.Tag = "string";
            this.columnFromTo.Text = "差出人";
            this.columnFromTo.Width = 174;
            // 
            // columnSubject
            // 
            this.columnSubject.Tag = "string";
            this.columnSubject.Text = "件名";
            this.columnSubject.Width = 188;
            // 
            // columnDate
            // 
            this.columnDate.Tag = "date";
            this.columnDate.Text = "受信時刻";
            this.columnDate.Width = 150;
            // 
            // columnSize
            // 
            this.columnSize.Tag = "num";
            this.columnSize.Text = "サイズ";
            this.columnSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnSize.Width = 80;
            // 
            // menuListView
            // 
            this.menuListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuReturnMail,
            this.menuFowerdMail,
            this.menuDelete,
            this.toolStripSeparator3,
            this.menuAlreadyRead,
            this.menuNotReadYet,
            this.toolStripMenuItem1,
            this.menuGetAttach});
            this.menuListView.Name = "contextMenuStrip1";
            this.menuListView.Size = new System.Drawing.Size(228, 170);
            this.menuListView.Opening += new System.ComponentModel.CancelEventHandler(this.menuListView_Opening);
            // 
            // menuReturnMail
            // 
            this.menuReturnMail.Name = "menuReturnMail";
            this.menuReturnMail.Size = new System.Drawing.Size(227, 22);
            this.menuReturnMail.Text = "返信(&R)";
            this.menuReturnMail.Click += new System.EventHandler(this.menuMailReturnMail_Click);
            // 
            // menuFowerdMail
            // 
            this.menuFowerdMail.Name = "menuFowerdMail";
            this.menuFowerdMail.Size = new System.Drawing.Size(227, 22);
            this.menuFowerdMail.Text = "転送(&T)";
            this.menuFowerdMail.Click += new System.EventHandler(this.menuMailFowerdMail_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.Size = new System.Drawing.Size(227, 22);
            this.menuDelete.Text = "削除(&D)";
            this.menuDelete.Click += new System.EventHandler(this.menuMailDelete_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(224, 6);
            // 
            // menuAlreadyRead
            // 
            this.menuAlreadyRead.Name = "menuAlreadyRead";
            this.menuAlreadyRead.Size = new System.Drawing.Size(227, 22);
            this.menuAlreadyRead.Text = "既読にする(&K)";
            this.menuAlreadyRead.Click += new System.EventHandler(this.menuAlreadyRead_Click);
            // 
            // menuNotReadYet
            // 
            this.menuNotReadYet.Name = "menuNotReadYet";
            this.menuNotReadYet.Size = new System.Drawing.Size(227, 22);
            this.menuNotReadYet.Text = "未読にする(&U)";
            this.menuNotReadYet.Click += new System.EventHandler(this.menuNotReadYet_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(224, 6);
            // 
            // menuGetAttach
            // 
            this.menuGetAttach.Name = "menuGetAttach";
            this.menuGetAttach.Size = new System.Drawing.Size(227, 22);
            this.menuGetAttach.Text = "添付ファイルを取り出す(&G)";
            this.menuGetAttach.Click += new System.EventHandler(this.menuFileGetAttatch_Click);
            // 
            // textBody
            // 
            this.textBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBody.Location = new System.Drawing.Point(0, 0);
            this.textBody.Multiline = true;
            this.textBody.Name = "textBody";
            this.textBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBody.Size = new System.Drawing.Size(621, 297);
            this.textBody.TabIndex = 2;
            // 
            // browserBody
            // 
            this.browserBody.AllowNavigation = false;
            this.browserBody.AllowWebBrowserDrop = false;
            this.browserBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserBody.Location = new System.Drawing.Point(0, 0);
            this.browserBody.MinimumSize = new System.Drawing.Size(20, 20);
            this.browserBody.Name = "browserBody";
            this.browserBody.Size = new System.Drawing.Size(621, 297);
            this.browserBody.TabIndex = 3;
            this.browserBody.TabStop = false;
            this.browserBody.Visible = false;
            this.browserBody.WebBrowserShortcutsEnabled = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "添付ファイルを保存するフォルダを選択";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "eml";
            this.saveFileDialog1.Filter = "メール(*.eml)|*.eml|テキスト ドキュメント(*.txt)|*.txt";
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip3;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Akane Mail";
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuTaskRestoreWindow,
            this.toolStripSeparator7,
            this.menuTaskMailNew,
            this.toolStripSeparator8,
            this.menuTaskApplicationExit});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(227, 82);
            // 
            // menuTaskRestoreWindow
            // 
            this.menuTaskRestoreWindow.Name = "menuTaskRestoreWindow";
            this.menuTaskRestoreWindow.Size = new System.Drawing.Size(226, 22);
            this.menuTaskRestoreWindow.Text = "元のサイズに戻す(&R)";
            this.menuTaskRestoreWindow.Click += new System.EventHandler(this.menuTaskRestoreWindow_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(223, 6);
            // 
            // menuTaskMailNew
            // 
            this.menuTaskMailNew.Name = "menuTaskMailNew";
            this.menuTaskMailNew.Size = new System.Drawing.Size(226, 22);
            this.menuTaskMailNew.Text = "新規作成(&N)";
            this.menuTaskMailNew.Click += new System.EventHandler(this.menuMailNew_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(223, 6);
            // 
            // menuTaskApplicationExit
            // 
            this.menuTaskApplicationExit.Name = "menuTaskApplicationExit";
            this.menuTaskApplicationExit.Size = new System.Drawing.Size(226, 22);
            this.menuTaskApplicationExit.Text = "アプリケーションの終了(&X)";
            this.menuTaskApplicationExit.Click += new System.EventHandler(this.menuFileExit_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Akane Mail";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ClientSizeChanged += new System.EventHandler(this.Form1_ClientSizeChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.menuTreeView.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.menuListView.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuMail;
        private System.Windows.Forms.ToolStripMenuItem menuMailSend;
        private System.Windows.Forms.ToolStripMenuItem menuMailRecieve;
        private System.Windows.Forms.ToolStripMenuItem オプションOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuToolSetEnv;
        private System.Windows.Forms.ToolStripMenuItem ヘルプHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem menuMailNew;
        private System.Windows.Forms.ToolStripMenuItem menuMailReturnMail;
        private System.Windows.Forms.ToolStripMenuItem menuMailFowerdMail;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem menuMailDelete;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonMailSend;
        private System.Windows.Forms.ToolStripButton buttonMailRecieve;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonMailNew;
        private System.Windows.Forms.ToolStripButton buttonReturnMail;
        private System.Windows.Forms.ToolStripButton buttonForwardMail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ToolStripStatusLabel labelMessage;
        private System.Windows.Forms.ToolStripProgressBar progressMail;
        private System.Windows.Forms.ToolStripStatusLabel labelDate;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnFromTo;
        private System.Windows.Forms.ColumnHeader columnSubject;
        private System.Windows.Forms.ColumnHeader columnDate;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ToolStripButton buttonHelp;
        private System.Windows.Forms.ContextMenuStrip menuListView;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripMenuItem menuNotReadYet;
        public System.Windows.Forms.TextBox textBody;
        private System.Windows.Forms.ToolStripMenuItem menuReturnMail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton buttonMailDelete;
        private System.Windows.Forms.ToolStripMenuItem menuFowerdMail;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuGetAttach;
        private System.Windows.Forms.ToolStripMenuItem menuFileGetAttatch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem menuSaveMailFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem menuFileClearTrush;
        private System.Windows.Forms.ContextMenuStrip menuTreeView;
        private System.Windows.Forms.ToolStripMenuItem menuClearTrush;
        private System.Windows.Forms.ToolStripDropDownButton buttonAttachList;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.WebBrowser browserBody;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem menuTaskRestoreWindow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem menuTaskMailNew;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem menuTaskApplicationExit;
        private System.Windows.Forms.ToolStripMenuItem menuAlreadyRead;
    }
}

