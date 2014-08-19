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
            this.menuGetAttatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuClearTrush = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAppExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSendMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRecieveMail = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuNewMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReplyMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFowerdMail = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDeleteMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTool = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetEnv = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressMail = new System.Windows.Forms.ToolStripProgressBar();
            this.buttonAttachList = new System.Windows.Forms.ToolStripDropDownButton();
            this.labelDate = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonSendMail = new System.Windows.Forms.ToolStripButton();
            this.buttonRecieveMail = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonNewMail = new System.Windows.Forms.ToolStripButton();
            this.buttonReplyMail = new System.Windows.Forms.ToolStripButton();
            this.buttonForwardMail = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonDeleteMail = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonHelp = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeMailBoxFolder = new System.Windows.Forms.TreeView();
            this.menuTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuContextClearTrush = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listMail = new System.Windows.Forms.ListView();
            this.columnFromTo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuContextReplyMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContextFowerdMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuContextDeleteMail = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAlreadyRead = new System.Windows.Forms.ToolStripMenuItem();
            this.menuNotReadYet = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuContextGetAttatch = new System.Windows.Forms.ToolStripMenuItem();
            this.textBody = new System.Windows.Forms.TextBox();
            this.browserBody = new System.Windows.Forms.WebBrowser();
            this.timerStatusTime = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timerAutoReceive = new System.Windows.Forms.Timer(this.components);
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
            this.menuTool,
            this.menuHelp});
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
            this.menuGetAttatch,
            this.toolStripSeparator6,
            this.menuClearTrush,
            this.toolStripSeparator5,
            this.menuAppExit});
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
            // menuGetAttatch
            // 
            this.menuGetAttatch.Name = "menuGetAttatch";
            this.menuGetAttatch.Size = new System.Drawing.Size(239, 22);
            this.menuGetAttatch.Text = "添付ファイルを取り出す(&G)...";
            this.menuGetAttatch.Click += new System.EventHandler(this.menuGetAttatch_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(236, 6);
            // 
            // menuClearTrush
            // 
            this.menuClearTrush.Name = "menuClearTrush";
            this.menuClearTrush.Size = new System.Drawing.Size(239, 22);
            this.menuClearTrush.Text = "ごみ箱を空にする(&Y)...";
            this.menuClearTrush.Click += new System.EventHandler(this.menuClearTrush_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(236, 6);
            // 
            // menuAppExit
            // 
            this.menuAppExit.Name = "menuAppExit";
            this.menuAppExit.Size = new System.Drawing.Size(239, 22);
            this.menuAppExit.Text = "アプリケーションの終了(&X)";
            this.menuAppExit.Click += new System.EventHandler(this.menuAppExit_Click);
            // 
            // menuMail
            // 
            this.menuMail.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSendMail,
            this.menuRecieveMail,
            this.toolStripMenuItem3,
            this.menuNewMail,
            this.menuReplyMail,
            this.menuFowerdMail,
            this.toolStripMenuItem4,
            this.menuDeleteMail});
            this.menuMail.Name = "menuMail";
            this.menuMail.Size = new System.Drawing.Size(76, 22);
            this.menuMail.Text = "メール(&M)";
            this.menuMail.DropDownOpening += new System.EventHandler(this.menuMail_DropDownOpening);
            // 
            // menuSendMail
            // 
            this.menuSendMail.Name = "menuSendMail";
            this.menuSendMail.Size = new System.Drawing.Size(167, 22);
            this.menuSendMail.Text = "送信(&S)";
            this.menuSendMail.Click += new System.EventHandler(this.menuSendMail_Click);
            // 
            // menuRecieveMail
            // 
            this.menuRecieveMail.Name = "menuRecieveMail";
            this.menuRecieveMail.Size = new System.Drawing.Size(167, 22);
            this.menuRecieveMail.Text = "受信(&M)";
            this.menuRecieveMail.Click += new System.EventHandler(this.menuRecieveMail_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(164, 6);
            // 
            // menuNewMail
            // 
            this.menuNewMail.Name = "menuNewMail";
            this.menuNewMail.Size = new System.Drawing.Size(167, 22);
            this.menuNewMail.Text = "新規作成(&N)";
            this.menuNewMail.Click += new System.EventHandler(this.menuNewMail_Click);
            // 
            // menuReplyMail
            // 
            this.menuReplyMail.Name = "menuReplyMail";
            this.menuReplyMail.Size = new System.Drawing.Size(167, 22);
            this.menuReplyMail.Text = "返信(&R)";
            this.menuReplyMail.Click += new System.EventHandler(this.menuReplyMail_Click);
            // 
            // menuFowerdMail
            // 
            this.menuFowerdMail.Name = "menuFowerdMail";
            this.menuFowerdMail.Size = new System.Drawing.Size(167, 22);
            this.menuFowerdMail.Text = "転送(&T)";
            this.menuFowerdMail.Click += new System.EventHandler(this.menuFowerdMail_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(164, 6);
            // 
            // menuDeleteMail
            // 
            this.menuDeleteMail.Name = "menuDeleteMail";
            this.menuDeleteMail.Size = new System.Drawing.Size(167, 22);
            this.menuDeleteMail.Text = "メールの削除(&D)";
            this.menuDeleteMail.Click += new System.EventHandler(this.menuDeleteMail_Click);
            // 
            // menuTool
            // 
            this.menuTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSetEnv});
            this.menuTool.Name = "menuTool";
            this.menuTool.Size = new System.Drawing.Size(74, 22);
            this.menuTool.Text = "ツール(&T)";
            // 
            // menuSetEnv
            // 
            this.menuSetEnv.Name = "menuSetEnv";
            this.menuSetEnv.Size = new System.Drawing.Size(152, 22);
            this.menuSetEnv.Text = "環境設定(&S)";
            this.menuSetEnv.Click += new System.EventHandler(this.menuSetEnv_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpOpen,
            this.toolStripMenuItem2,
            this.menuAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(75, 22);
            this.menuHelp.Text = "ヘルプ(&H)";
            // 
            // menuHelpOpen
            // 
            this.menuHelpOpen.Enabled = false;
            this.menuHelpOpen.Name = "menuHelpOpen";
            this.menuHelpOpen.Size = new System.Drawing.Size(178, 22);
            this.menuHelpOpen.Text = "目次(&H)";
            this.menuHelpOpen.Visible = false;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(175, 6);
            this.toolStripMenuItem2.Visible = false;
            // 
            // menuAbout
            // 
            this.menuAbout.Name = "menuAbout";
            this.menuAbout.Size = new System.Drawing.Size(178, 22);
            this.menuAbout.Text = "バージョン情報(&A)";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
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
            this.buttonSendMail,
            this.buttonRecieveMail,
            this.toolStripSeparator1,
            this.buttonNewMail,
            this.buttonReplyMail,
            this.buttonForwardMail,
            this.toolStripSeparator4,
            this.buttonDeleteMail,
            this.toolStripSeparator2,
            this.buttonHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 26);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonSendMail
            // 
            this.buttonSendMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonSendMail.Image")));
            this.buttonSendMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSendMail.Name = "buttonSendMail";
            this.buttonSendMail.Size = new System.Drawing.Size(52, 22);
            this.buttonSendMail.Text = "送信";
            this.buttonSendMail.Click += new System.EventHandler(this.menuSendMail_Click);
            // 
            // buttonRecieveMail
            // 
            this.buttonRecieveMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonRecieveMail.Image")));
            this.buttonRecieveMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRecieveMail.Name = "buttonRecieveMail";
            this.buttonRecieveMail.Size = new System.Drawing.Size(52, 22);
            this.buttonRecieveMail.Text = "受信";
            this.buttonRecieveMail.Click += new System.EventHandler(this.menuRecieveMail_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonNewMail
            // 
            this.buttonNewMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonNewMail.Image")));
            this.buttonNewMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNewMail.Name = "buttonNewMail";
            this.buttonNewMail.Size = new System.Drawing.Size(76, 22);
            this.buttonNewMail.Text = "新規作成";
            this.buttonNewMail.Click += new System.EventHandler(this.menuNewMail_Click);
            // 
            // buttonReplyMail
            // 
            this.buttonReplyMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonReplyMail.Image")));
            this.buttonReplyMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonReplyMail.Name = "buttonReplyMail";
            this.buttonReplyMail.Size = new System.Drawing.Size(52, 22);
            this.buttonReplyMail.Text = "返信";
            this.buttonReplyMail.Click += new System.EventHandler(this.menuReplyMail_Click);
            // 
            // buttonForwardMail
            // 
            this.buttonForwardMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonForwardMail.Image")));
            this.buttonForwardMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonForwardMail.Name = "buttonForwardMail";
            this.buttonForwardMail.Size = new System.Drawing.Size(52, 22);
            this.buttonForwardMail.Text = "転送";
            this.buttonForwardMail.Click += new System.EventHandler(this.menuFowerdMail_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonDeleteMail
            // 
            this.buttonDeleteMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonDeleteMail.Image")));
            this.buttonDeleteMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonDeleteMail.Name = "buttonDeleteMail";
            this.buttonDeleteMail.Size = new System.Drawing.Size(52, 22);
            this.buttonDeleteMail.Text = "削除";
            this.buttonDeleteMail.Click += new System.EventHandler(this.menuDeleteMail_Click);
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
            this.buttonHelp.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 51);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeMailBoxFolder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(784, 484);
            this.splitContainer1.SplitterDistance = 156;
            this.splitContainer1.TabIndex = 3;
            // 
            // treeMailBoxFolder
            // 
            this.treeMailBoxFolder.ContextMenuStrip = this.menuTreeView;
            this.treeMailBoxFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMailBoxFolder.ImageIndex = 0;
            this.treeMailBoxFolder.ImageList = this.imageList1;
            this.treeMailBoxFolder.Location = new System.Drawing.Point(0, 0);
            this.treeMailBoxFolder.Name = "treeMailBoxFolder";
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
            this.treeMailBoxFolder.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.treeMailBoxFolder.SelectedImageIndex = 0;
            this.treeMailBoxFolder.Size = new System.Drawing.Size(156, 484);
            this.treeMailBoxFolder.TabIndex = 0;
            this.treeMailBoxFolder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMailBoxFolder_AfterSelect);
            // 
            // menuTreeView
            // 
            this.menuTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuContextClearTrush});
            this.menuTreeView.Name = "contextMenuStrip2";
            this.menuTreeView.Size = new System.Drawing.Size(203, 26);
            this.menuTreeView.Opening += new System.ComponentModel.CancelEventHandler(this.menuTreeView_Opening);
            // 
            // menuContextClearTrush
            // 
            this.menuContextClearTrush.Name = "menuContextClearTrush";
            this.menuContextClearTrush.Size = new System.Drawing.Size(202, 22);
            this.menuContextClearTrush.Text = "ごみ箱を空にする(&Y)...";
            this.menuContextClearTrush.Click += new System.EventHandler(this.menuClearTrush_Click);
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
            this.splitContainer2.Panel1.Controls.Add(this.listMail);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBody);
            this.splitContainer2.Panel2.Controls.Add(this.browserBody);
            this.splitContainer2.Size = new System.Drawing.Size(624, 484);
            this.splitContainer2.SplitterDistance = 179;
            this.splitContainer2.TabIndex = 0;
            // 
            // listMail
            // 
            this.listMail.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFromTo,
            this.columnSubject,
            this.columnDate,
            this.columnSize});
            this.listMail.ContextMenuStrip = this.menuListView;
            this.listMail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMail.FullRowSelect = true;
            this.listMail.HideSelection = false;
            this.listMail.Location = new System.Drawing.Point(0, 0);
            this.listMail.Name = "listMail";
            this.listMail.Size = new System.Drawing.Size(624, 179);
            this.listMail.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listMail.TabIndex = 1;
            this.listMail.UseCompatibleStateImageBehavior = false;
            this.listMail.View = System.Windows.Forms.View.Details;
            this.listMail.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listMail_ColumnClick);
            this.listMail.Click += new System.EventHandler(this.listMail_Click);
            this.listMail.DoubleClick += new System.EventHandler(this.listMail_DoubleClick);
            // 
            // columnFromTo
            // 
            this.columnFromTo.Tag = "string";
            this.columnFromTo.Text = "名前";
            this.columnFromTo.Width = 174;
            // 
            // columnSubject
            // 
            this.columnSubject.Tag = "string";
            this.columnSubject.Text = "メールアドレス";
            this.columnSubject.Width = 188;
            // 
            // columnDate
            // 
            this.columnDate.Tag = "date";
            this.columnDate.Text = "最終データ更新日";
            this.columnDate.Width = 150;
            // 
            // columnSize
            // 
            this.columnSize.Tag = "num";
            this.columnSize.Text = "データサイズ";
            this.columnSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnSize.Width = 80;
            // 
            // menuListView
            // 
            this.menuListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuContextReplyMail,
            this.menuContextFowerdMail,
            this.menuContextDeleteMail,
            this.toolStripSeparator3,
            this.menuAlreadyRead,
            this.menuNotReadYet,
            this.toolStripMenuItem1,
            this.menuContextGetAttatch});
            this.menuListView.Name = "contextMenuStrip1";
            this.menuListView.Size = new System.Drawing.Size(228, 148);
            this.menuListView.Opening += new System.ComponentModel.CancelEventHandler(this.menuListView_Opening);
            // 
            // menuContextReplyMail
            // 
            this.menuContextReplyMail.Name = "menuContextReplyMail";
            this.menuContextReplyMail.Size = new System.Drawing.Size(227, 22);
            this.menuContextReplyMail.Text = "返信(&R)";
            this.menuContextReplyMail.Click += new System.EventHandler(this.menuReplyMail_Click);
            // 
            // menuContextFowerdMail
            // 
            this.menuContextFowerdMail.Name = "menuContextFowerdMail";
            this.menuContextFowerdMail.Size = new System.Drawing.Size(227, 22);
            this.menuContextFowerdMail.Text = "転送(&T)";
            this.menuContextFowerdMail.Click += new System.EventHandler(this.menuFowerdMail_Click);
            // 
            // menuContextDeleteMail
            // 
            this.menuContextDeleteMail.Name = "menuContextDeleteMail";
            this.menuContextDeleteMail.Size = new System.Drawing.Size(227, 22);
            this.menuContextDeleteMail.Text = "削除(&D)";
            this.menuContextDeleteMail.Click += new System.EventHandler(this.menuDeleteMail_Click);
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
            // menuContextGetAttatch
            // 
            this.menuContextGetAttatch.Name = "menuContextGetAttatch";
            this.menuContextGetAttatch.Size = new System.Drawing.Size(227, 22);
            this.menuContextGetAttatch.Text = "添付ファイルを取り出す(&G)";
            this.menuContextGetAttatch.Click += new System.EventHandler(this.menuGetAttatch_Click);
            // 
            // textBody
            // 
            this.textBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBody.Location = new System.Drawing.Point(0, 0);
            this.textBody.Multiline = true;
            this.textBody.Name = "textBody";
            this.textBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBody.Size = new System.Drawing.Size(624, 301);
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
            this.browserBody.Size = new System.Drawing.Size(624, 301);
            this.browserBody.TabIndex = 3;
            this.browserBody.TabStop = false;
            this.browserBody.Visible = false;
            this.browserBody.WebBrowserShortcutsEnabled = false;
            // 
            // timerStatusTime
            // 
            this.timerStatusTime.Enabled = true;
            this.timerStatusTime.Interval = 500;
            this.timerStatusTime.Tick += new System.EventHandler(this.timerStatusTime_Tick);
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
            // timerAutoReceive
            // 
            this.timerAutoReceive.Tick += new System.EventHandler(this.timerAutoReceive_Tick);
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
            this.menuTaskMailNew.Click += new System.EventHandler(this.menuNewMail_Click);
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
            this.menuTaskApplicationExit.Click += new System.EventHandler(this.menuAppExit_Click);
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ClientSizeChanged += new System.EventHandler(this.MainForm_ClientSizeChanged);
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
        private System.Windows.Forms.ToolStripMenuItem menuAppExit;
        private System.Windows.Forms.ToolStripMenuItem menuMail;
        private System.Windows.Forms.ToolStripMenuItem menuSendMail;
        private System.Windows.Forms.ToolStripMenuItem menuRecieveMail;
        private System.Windows.Forms.ToolStripMenuItem menuTool;
        private System.Windows.Forms.ToolStripMenuItem menuSetEnv;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuHelpOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem menuNewMail;
        private System.Windows.Forms.ToolStripMenuItem menuReplyMail;
        private System.Windows.Forms.ToolStripMenuItem menuFowerdMail;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteMail;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonSendMail;
        private System.Windows.Forms.ToolStripButton buttonRecieveMail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonNewMail;
        private System.Windows.Forms.ToolStripButton buttonReplyMail;
        private System.Windows.Forms.ToolStripButton buttonForwardMail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeMailBoxFolder;
        private System.Windows.Forms.ToolStripStatusLabel labelMessage;
        private System.Windows.Forms.ToolStripProgressBar progressMail;
        private System.Windows.Forms.ToolStripStatusLabel labelDate;
        private System.Windows.Forms.Timer timerStatusTime;
        private System.Windows.Forms.SplitContainer splitContainer2;
        public System.Windows.Forms.ListView listMail;
        private System.Windows.Forms.ColumnHeader columnFromTo;
        private System.Windows.Forms.ColumnHeader columnSubject;
        private System.Windows.Forms.ColumnHeader columnDate;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ToolStripButton buttonHelp;
        private System.Windows.Forms.ContextMenuStrip menuListView;
        private System.Windows.Forms.ToolStripMenuItem menuContextDeleteMail;
        private System.Windows.Forms.ToolStripMenuItem menuNotReadYet;
        public System.Windows.Forms.TextBox textBody;
        private System.Windows.Forms.ToolStripMenuItem menuContextReplyMail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton buttonDeleteMail;
        private System.Windows.Forms.ToolStripMenuItem menuContextFowerdMail;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuContextGetAttatch;
        private System.Windows.Forms.ToolStripMenuItem menuGetAttatch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem menuSaveMailFile;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem menuClearTrush;
        private System.Windows.Forms.ContextMenuStrip menuTreeView;
        private System.Windows.Forms.ToolStripMenuItem menuContextClearTrush;
        private System.Windows.Forms.ToolStripDropDownButton buttonAttachList;
        private System.Windows.Forms.Timer timerAutoReceive;
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

