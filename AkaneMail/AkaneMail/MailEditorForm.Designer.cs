namespace AkaneMail
{
    partial class MailEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MailEditorForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSendMail = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSendMailBox = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSetAttachFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFind = new System.Windows.Forms.ToolStripMenuItem();
            this.menuReplace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelpIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonSendMail = new System.Windows.Forms.ToolStripButton();
            this.buttonSendMailBox = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonSetAttachFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonCut = new System.Windows.Forms.ToolStripButton();
            this.buttonCopy = new System.Windows.Forms.ToolStripButton();
            this.buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonHelp = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonAttachList = new System.Windows.Forms.ToolStripDropDownButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.comboPriority = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textSubject = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textCc = new System.Windows.Forms.TextBox();
            this.textAddress = new System.Windows.Forms.TextBox();
            this.textBcc = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBody = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1229, 27);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSendMail,
            this.menuSendMailBox,
            this.toolStripMenuItem1,
            this.menuSetAttachFile,
            this.toolStripMenuItem4,
            this.menuClose});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(86, 23);
            this.menuFile.Text = "ファイル(&F)";
            // 
            // menuSendMail
            // 
            this.menuSendMail.Name = "menuSendMail";
            this.menuSendMail.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSendMail.Size = new System.Drawing.Size(247, 24);
            this.menuSendMail.Text = "送信(&S)";
            this.menuSendMail.Click += new System.EventHandler(this.menuSendMail_Click);
            // 
            // menuSendMailBox
            // 
            this.menuSendMailBox.Name = "menuSendMailBox";
            this.menuSendMailBox.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.menuSendMailBox.Size = new System.Drawing.Size(247, 24);
            this.menuSendMailBox.Text = "送信箱に保存(&B)";
            this.menuSendMailBox.Click += new System.EventHandler(this.menuSendMailBox_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(244, 6);
            // 
            // menuSetAttachFile
            // 
            this.menuSetAttachFile.Name = "menuSetAttachFile";
            this.menuSetAttachFile.Size = new System.Drawing.Size(247, 24);
            this.menuSetAttachFile.Text = "ファイルの添付(&A)...";
            this.menuSetAttachFile.Click += new System.EventHandler(this.menuSetAttachFile_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(244, 6);
            // 
            // menuClose
            // 
            this.menuClose.Name = "menuClose";
            this.menuClose.Size = new System.Drawing.Size(247, 24);
            this.menuClose.Text = "閉じる(&C)";
            this.menuClose.Click += new System.EventHandler(this.menuClose_Click);
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuUndo,
            this.toolStripMenuItem2,
            this.menuCut,
            this.menuCopy,
            this.menuPaste,
            this.menuDelete,
            this.toolStripSeparator5,
            this.menuFind,
            this.menuReplace,
            this.toolStripMenuItem3,
            this.menuSelectAll});
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(74, 23);
            this.menuEdit.Text = "編集(&E)";
            this.menuEdit.DropDownOpening += new System.EventHandler(this.menuEdit_DropDownOpening);
            // 
            // menuUndo
            // 
            this.menuUndo.Name = "menuUndo";
            this.menuUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.menuUndo.Size = new System.Drawing.Size(226, 24);
            this.menuUndo.Text = "元に戻す(&U)";
            this.menuUndo.Click += new System.EventHandler(this.menuUndo_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(223, 6);
            // 
            // menuCut
            // 
            this.menuCut.Name = "menuCut";
            this.menuCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.menuCut.Size = new System.Drawing.Size(226, 24);
            this.menuCut.Text = "切り取り(&T)";
            this.menuCut.Click += new System.EventHandler(this.menuCut_Click);
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopy.Size = new System.Drawing.Size(226, 24);
            this.menuCopy.Text = "コピー(&C)";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuPaste
            // 
            this.menuPaste.Name = "menuPaste";
            this.menuPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.menuPaste.Size = new System.Drawing.Size(226, 24);
            this.menuPaste.Text = "貼り付け(&P)";
            this.menuPaste.Click += new System.EventHandler(this.menuPaste_Click);
            // 
            // menuDelete
            // 
            this.menuDelete.Name = "menuDelete";
            this.menuDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.menuDelete.Size = new System.Drawing.Size(226, 24);
            this.menuDelete.Text = "削除(&D)";
            this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(223, 6);
            // 
            // menuFind
            // 
            this.menuFind.Name = "menuFind";
            this.menuFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.menuFind.Size = new System.Drawing.Size(226, 24);
            this.menuFind.Text = "検索(&F)...";
            this.menuFind.Click += new System.EventHandler(this.menuFind_Click);
            // 
            // menuReplace
            // 
            this.menuReplace.Name = "menuReplace";
            this.menuReplace.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.menuReplace.Size = new System.Drawing.Size(226, 24);
            this.menuReplace.Text = "置換(&R)...";
            this.menuReplace.Click += new System.EventHandler(this.menuReplace_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(223, 6);
            // 
            // menuSelectAll
            // 
            this.menuSelectAll.Name = "menuSelectAll";
            this.menuSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.menuSelectAll.Size = new System.Drawing.Size(226, 24);
            this.menuSelectAll.Text = "すべて選択(&A)";
            this.menuSelectAll.Click += new System.EventHandler(this.menuSelectAll_Click);
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuHelpIndex,
            this.toolStripSeparator4,
            this.menuHelpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(81, 23);
            this.menuHelp.Text = "ヘルプ(&H)";
            // 
            // menuHelpIndex
            // 
            this.menuHelpIndex.Enabled = false;
            this.menuHelpIndex.Name = "menuHelpIndex";
            this.menuHelpIndex.Size = new System.Drawing.Size(188, 24);
            this.menuHelpIndex.Text = "目次(&H)";
            this.menuHelpIndex.Visible = false;
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(185, 6);
            this.toolStripSeparator4.Visible = false;
            // 
            // menuHelpAbout
            // 
            this.menuHelpAbout.Name = "menuHelpAbout";
            this.menuHelpAbout.Size = new System.Drawing.Size(188, 24);
            this.menuHelpAbout.Text = "バージョン情報(&A)";
            this.menuHelpAbout.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSendMail,
            this.buttonSendMailBox,
            this.toolStripSeparator1,
            this.buttonSetAttachFile,
            this.toolStripSeparator2,
            this.buttonCut,
            this.buttonCopy,
            this.buttonPaste,
            this.toolStripSeparator3,
            this.buttonHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 27);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1229, 26);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonSendMail
            // 
            this.buttonSendMail.Image = ((System.Drawing.Image)(resources.GetObject("buttonSendMail.Image")));
            this.buttonSendMail.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSendMail.Name = "buttonSendMail";
            this.buttonSendMail.Size = new System.Drawing.Size(59, 23);
            this.buttonSendMail.Text = "送信";
            this.buttonSendMail.Click += new System.EventHandler(this.menuSendMail_Click);
            // 
            // buttonSendMailBox
            // 
            this.buttonSendMailBox.Image = ((System.Drawing.Image)(resources.GetObject("buttonSendMailBox.Image")));
            this.buttonSendMailBox.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSendMailBox.Name = "buttonSendMailBox";
            this.buttonSendMailBox.Size = new System.Drawing.Size(116, 23);
            this.buttonSendMailBox.Text = "送信箱に保存";
            this.buttonSendMailBox.Click += new System.EventHandler(this.menuSendMailBox_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 26);
            // 
            // buttonSetAttachFile
            // 
            this.buttonSetAttachFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonSetAttachFile.Image")));
            this.buttonSetAttachFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSetAttachFile.Name = "buttonSetAttachFile";
            this.buttonSetAttachFile.Size = new System.Drawing.Size(113, 23);
            this.buttonSetAttachFile.Text = "ファイルの添付";
            this.buttonSetAttachFile.Click += new System.EventHandler(this.menuSetAttachFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 26);
            // 
            // buttonCut
            // 
            this.buttonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonCut.Enabled = false;
            this.buttonCut.Image = ((System.Drawing.Image)(resources.GetObject("buttonCut.Image")));
            this.buttonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCut.Name = "buttonCut";
            this.buttonCut.Size = new System.Drawing.Size(23, 23);
            this.buttonCut.Text = "切り取り";
            this.buttonCut.Click += new System.EventHandler(this.menuCut_Click);
            // 
            // buttonCopy
            // 
            this.buttonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonCopy.Enabled = false;
            this.buttonCopy.Image = ((System.Drawing.Image)(resources.GetObject("buttonCopy.Image")));
            this.buttonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(23, 23);
            this.buttonCopy.Text = "コピー";
            this.buttonCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // buttonPaste
            // 
            this.buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPaste.Enabled = false;
            this.buttonPaste.Image = ((System.Drawing.Image)(resources.GetObject("buttonPaste.Image")));
            this.buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(23, 23);
            this.buttonPaste.Text = "貼り付け";
            this.buttonPaste.Click += new System.EventHandler(this.menuPaste_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 26);
            // 
            // buttonHelp
            // 
            this.buttonHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonHelp.Image = ((System.Drawing.Image)(resources.GetObject("buttonHelp.Image")));
            this.buttonHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(23, 23);
            this.buttonHelp.Text = "ヘルプ";
            this.buttonHelp.Click += new System.EventHandler(this.menuHelpAbout_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelMessage,
            this.buttonAttachList});
            this.statusStrip1.Location = new System.Drawing.Point(0, 680);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(3, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1229, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelMessage
            // 
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(1207, 17);
            this.labelMessage.Spring = true;
            this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonAttachList
            // 
            this.buttonAttachList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAttachList.Image = ((System.Drawing.Image)(resources.GetObject("buttonAttachList.Image")));
            this.buttonAttachList.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAttachList.Name = "buttonAttachList";
            this.buttonAttachList.Size = new System.Drawing.Size(29, 20);
            this.buttonAttachList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAttachList.Visible = false;
            this.buttonAttachList.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.buttonAttachList_DropDownItemClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.comboPriority, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textSubject, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textCc, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textAddress, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBcc, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 53);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1229, 131);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // comboPriority
            // 
            this.comboPriority.FormattingEnabled = true;
            this.comboPriority.Items.AddRange(new object[] {
            "高い",
            "普通",
            "低い"});
            this.comboPriority.Location = new System.Drawing.Point(64, 106);
            this.comboPriority.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.comboPriority.Name = "comboPriority";
            this.comboPriority.Size = new System.Drawing.Size(113, 23);
            this.comboPriority.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(4, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "宛先";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(4, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "CC";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textSubject
            // 
            this.textSubject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSubject.Location = new System.Drawing.Point(64, 80);
            this.textSubject.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textSubject.Name = "textSubject";
            this.textSubject.Size = new System.Drawing.Size(1161, 22);
            this.textSubject.TabIndex = 7;
            this.textSubject.TextChanged += new System.EventHandler(this.TextEdited);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(4, 52);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 26);
            this.label4.TabIndex = 4;
            this.label4.Text = "BCC";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textCc
            // 
            this.textCc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textCc.Location = new System.Drawing.Point(64, 28);
            this.textCc.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textCc.Name = "textCc";
            this.textCc.Size = new System.Drawing.Size(1161, 22);
            this.textCc.TabIndex = 3;
            // 
            // textAddress
            // 
            this.textAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textAddress.Location = new System.Drawing.Point(64, 2);
            this.textAddress.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textAddress.Name = "textAddress";
            this.textAddress.Size = new System.Drawing.Size(1161, 22);
            this.textAddress.TabIndex = 1;
            this.textAddress.TextChanged += new System.EventHandler(this.TextEdited);
            // 
            // textBcc
            // 
            this.textBcc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBcc.Location = new System.Drawing.Point(64, 54);
            this.textBcc.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textBcc.Name = "textBcc";
            this.textBcc.Size = new System.Drawing.Size(1161, 22);
            this.textBcc.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(4, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 26);
            this.label2.TabIndex = 6;
            this.label2.Text = "件名";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(4, 104);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 27);
            this.label5.TabIndex = 8;
            this.label5.Text = "重要度";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBody
            // 
            this.textBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBody.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBody.Location = new System.Drawing.Point(0, 184);
            this.textBody.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.textBody.Multiline = true;
            this.textBody.Name = "textBody";
            this.textBody.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBody.Size = new System.Drawing.Size(1229, 496);
            this.textBody.TabIndex = 0;
            this.textBody.TextChanged += new System.EventHandler(this.TextEdited);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "すべてのファイル(*.*)|*.*";
            // 
            // MailEditorForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1229, 702);
            this.Controls.Add(this.textBody);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "MailEditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "新規作成 - Akane Mail";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MailEditorForm_FormClosing);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MailEditorForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MailEditorForm_DragEnter);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuSendMailBox;
        private System.Windows.Forms.ToolStripMenuItem menuSendMail;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem menuClose;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuUndo;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem menuCut;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuPaste;
        private System.Windows.Forms.ToolStripMenuItem menuDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem menuSelectAll;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuSetAttachFile;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem menuHelpIndex;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonSendMailBox;
        private System.Windows.Forms.ToolStripButton buttonSendMail;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonSetAttachFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton buttonCut;
        public System.Windows.Forms.ToolStripButton buttonCopy;
        public System.Windows.Forms.ToolStripButton buttonPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton buttonHelp;
        private System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.TextBox textAddress;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox textBody;
        public System.Windows.Forms.TextBox textSubject;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripStatusLabel labelMessage;
        public System.Windows.Forms.ToolStripDropDownButton buttonAttachList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuHelpAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuFind;
        private System.Windows.Forms.ToolStripMenuItem menuReplace;
        public System.Windows.Forms.ComboBox comboPriority;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox textBcc;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox textCc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}