namespace AkaneMail
{
    partial class FindDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.findTextBox = new System.Windows.Forms.TextBox();
            this.findButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.LgSmCheckBox = new System.Windows.Forms.CheckBox();
            this.findCourseGroupBox = new System.Windows.Forms.GroupBox();
            this.currentPosRadio = new System.Windows.Forms.RadioButton();
            this.topPosRadio = new System.Windows.Forms.RadioButton();
            this.replaceAllButton = new System.Windows.Forms.Button();
            this.replaceNextButton = new System.Windows.Forms.Button();
            this.ReplaceTextBox = new System.Windows.Forms.TextBox();
            this.ReplaceLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.findCourseGroupBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "検索する文字列(&N):";
            // 
            // findTextBox
            // 
            this.findTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.findTextBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.findTextBox.Location = new System.Drawing.Point(176, 3);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new System.Drawing.Size(542, 24);
            this.findTextBox.TabIndex = 1;
            // 
            // findButton
            // 
            this.findButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.findButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.findButton.Location = new System.Drawing.Point(724, 3);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(127, 24);
            this.findButton.TabIndex = 3;
            this.findButton.Text = "次を検索(&F)";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(724, 123);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(127, 24);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "キャンセル";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // LgSmCheckBox
            // 
            this.LgSmCheckBox.AutoSize = true;
            this.LgSmCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LgSmCheckBox.Location = new System.Drawing.Point(176, 123);
            this.LgSmCheckBox.Name = "LgSmCheckBox";
            this.LgSmCheckBox.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.LgSmCheckBox.Size = new System.Drawing.Size(222, 21);
            this.LgSmCheckBox.TabIndex = 4;
            this.LgSmCheckBox.Text = "大文字小文字を区別する";
            this.LgSmCheckBox.UseVisualStyleBackColor = true;
            // 
            // findCourseGroupBox
            // 
            this.findCourseGroupBox.AutoSize = true;
            this.findCourseGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.findCourseGroupBox.Controls.Add(this.currentPosRadio);
            this.findCourseGroupBox.Controls.Add(this.topPosRadio);
            this.findCourseGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.findCourseGroupBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.findCourseGroupBox.Location = new System.Drawing.Point(176, 63);
            this.findCourseGroupBox.Name = "findCourseGroupBox";
            this.findCourseGroupBox.Size = new System.Drawing.Size(542, 54);
            this.findCourseGroupBox.TabIndex = 5;
            this.findCourseGroupBox.TabStop = false;
            this.findCourseGroupBox.Text = "検索する方法";
            // 
            // currentPosRadio
            // 
            this.currentPosRadio.AutoSize = true;
            this.currentPosRadio.Dock = System.Windows.Forms.DockStyle.Left;
            this.currentPosRadio.Location = new System.Drawing.Point(124, 20);
            this.currentPosRadio.MinimumSize = new System.Drawing.Size(0, 20);
            this.currentPosRadio.Name = "currentPosRadio";
            this.currentPosRadio.Size = new System.Drawing.Size(144, 31);
            this.currentPosRadio.TabIndex = 6;
            this.currentPosRadio.Text = "現在位置から(&D)";
            this.currentPosRadio.UseVisualStyleBackColor = true;
            this.currentPosRadio.CheckedChanged += new System.EventHandler(this.findPosition_Radio_CheckedChanged);
            // 
            // topPosRadio
            // 
            this.topPosRadio.AutoSize = true;
            this.topPosRadio.Checked = true;
            this.topPosRadio.Dock = System.Windows.Forms.DockStyle.Left;
            this.topPosRadio.Location = new System.Drawing.Point(3, 20);
            this.topPosRadio.MinimumSize = new System.Drawing.Size(0, 20);
            this.topPosRadio.Name = "topPosRadio";
            this.topPosRadio.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.topPosRadio.Size = new System.Drawing.Size(121, 31);
            this.topPosRadio.TabIndex = 0;
            this.topPosRadio.TabStop = true;
            this.topPosRadio.Text = "先頭から(&T)";
            this.topPosRadio.UseVisualStyleBackColor = true;
            this.topPosRadio.CheckedChanged += new System.EventHandler(this.findPosition_Radio_CheckedChanged);
            // 
            // replaceAllButton
            // 
            this.replaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceAllButton.Location = new System.Drawing.Point(724, 63);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(127, 26);
            this.replaceAllButton.TabIndex = 4;
            this.replaceAllButton.Text = "すべて置換(&A)";
            this.replaceAllButton.UseVisualStyleBackColor = true;
            this.replaceAllButton.Click += new System.EventHandler(this.replaceAllButton_Click);
            // 
            // replaceNextButton
            // 
            this.replaceNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceNextButton.Enabled = false;
            this.replaceNextButton.Location = new System.Drawing.Point(724, 33);
            this.replaceNextButton.Name = "replaceNextButton";
            this.replaceNextButton.Size = new System.Drawing.Size(127, 24);
            this.replaceNextButton.TabIndex = 10;
            this.replaceNextButton.Text = "置換して次に(&R)";
            this.replaceNextButton.UseVisualStyleBackColor = true;
            this.replaceNextButton.Click += new System.EventHandler(this.replaceNextButton_Click);
            // 
            // ReplaceTextBox
            // 
            this.ReplaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceTextBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReplaceTextBox.Location = new System.Drawing.Point(176, 33);
            this.ReplaceTextBox.Name = "ReplaceTextBox";
            this.ReplaceTextBox.Size = new System.Drawing.Size(542, 24);
            this.ReplaceTextBox.TabIndex = 2;
            // 
            // ReplaceLabel
            // 
            this.ReplaceLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ReplaceLabel.AutoSize = true;
            this.ReplaceLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReplaceLabel.Location = new System.Drawing.Point(3, 30);
            this.ReplaceLabel.Name = "ReplaceLabel";
            this.ReplaceLabel.Size = new System.Drawing.Size(167, 30);
            this.ReplaceLabel.TabIndex = 12;
            this.ReplaceLabel.Text = "置換後の文字列(&P):";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.replaceAllButton, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.findCourseGroupBox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ReplaceLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.ReplaceTextBox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.findTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.findButton, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.replaceNextButton, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.LgSmCheckBox, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(854, 150);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // findDialog
            // 
            this.AcceptButton = this.findButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(854, 150);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "findDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.findCourseGroupBox.ResumeLayout(false);
            this.findCourseGroupBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox findTextBox;
        private System.Windows.Forms.Button findButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox LgSmCheckBox;
        private System.Windows.Forms.GroupBox findCourseGroupBox;
        private System.Windows.Forms.RadioButton currentPosRadio;
        private System.Windows.Forms.RadioButton topPosRadio;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.Button replaceNextButton;
        private System.Windows.Forms.TextBox ReplaceTextBox;
        private System.Windows.Forms.Label ReplaceLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}