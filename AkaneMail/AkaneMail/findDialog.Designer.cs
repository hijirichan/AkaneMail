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
            this.ReplacePanel = new System.Windows.Forms.Panel();
            this.replaceAllButton = new System.Windows.Forms.Button();
            this.replaceNextButton = new System.Windows.Forms.Button();
            this.ReplaceTextBox = new System.Windows.Forms.TextBox();
            this.ReplaceLabel = new System.Windows.Forms.Label();
            this.findCourseGroupBox.SuspendLayout();
            this.ReplacePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "検索する文字列(&N):";
            // 
            // findTextBox
            // 
            this.findTextBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.findTextBox.Location = new System.Drawing.Point(139, 7);
            this.findTextBox.Name = "findTextBox";
            this.findTextBox.Size = new System.Drawing.Size(253, 20);
            this.findTextBox.TabIndex = 1;
            // 
            // findButton
            // 
            this.findButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.findButton.Location = new System.Drawing.Point(398, 4);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(108, 26);
            this.findButton.TabIndex = 3;
            this.findButton.Text = "次を検索(&F)";
            this.findButton.UseVisualStyleBackColor = true;
            this.findButton.Click += new System.EventHandler(this.findButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(398, 100);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(108, 26);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "キャンセル";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // LgSmCheckBox
            // 
            this.LgSmCheckBox.AutoSize = true;
            this.LgSmCheckBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LgSmCheckBox.Location = new System.Drawing.Point(7, 100);
            this.LgSmCheckBox.Name = "LgSmCheckBox";
            this.LgSmCheckBox.Size = new System.Drawing.Size(161, 17);
            this.LgSmCheckBox.TabIndex = 4;
            this.LgSmCheckBox.Text = "大文字小文字を区別する";
            this.LgSmCheckBox.UseVisualStyleBackColor = true;
            // 
            // findCourseGroupBox
            // 
            this.findCourseGroupBox.Controls.Add(this.currentPosRadio);
            this.findCourseGroupBox.Controls.Add(this.topPosRadio);
            this.findCourseGroupBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.findCourseGroupBox.Location = new System.Drawing.Point(196, 46);
            this.findCourseGroupBox.Name = "findCourseGroupBox";
            this.findCourseGroupBox.Size = new System.Drawing.Size(225, 48);
            this.findCourseGroupBox.TabIndex = 5;
            this.findCourseGroupBox.TabStop = false;
            this.findCourseGroupBox.Text = "検索する方法";
            // 
            // currentPosRadio
            // 
            this.currentPosRadio.AutoSize = true;
            this.currentPosRadio.Location = new System.Drawing.Point(109, 21);
            this.currentPosRadio.Name = "currentPosRadio";
            this.currentPosRadio.Size = new System.Drawing.Size(113, 17);
            this.currentPosRadio.TabIndex = 6;
            this.currentPosRadio.Text = "現在位置から(&D)";
            this.currentPosRadio.UseVisualStyleBackColor = true;
            this.currentPosRadio.CheckedChanged += new System.EventHandler(this.findPosition_Radio_CheckedChanged);
            // 
            // topPosRadio
            // 
            this.topPosRadio.AutoSize = true;
            this.topPosRadio.Checked = true;
            this.topPosRadio.Location = new System.Drawing.Point(15, 21);
            this.topPosRadio.Name = "topPosRadio";
            this.topPosRadio.Size = new System.Drawing.Size(87, 17);
            this.topPosRadio.TabIndex = 0;
            this.topPosRadio.TabStop = true;
            this.topPosRadio.Text = "先頭から(&T)";
            this.topPosRadio.UseVisualStyleBackColor = true;
            this.topPosRadio.CheckedChanged += new System.EventHandler(this.findPosition_Radio_CheckedChanged);
            // 
            // ReplacePanel
            // 
            this.ReplacePanel.Controls.Add(this.replaceAllButton);
            this.ReplacePanel.Controls.Add(this.replaceNextButton);
            this.ReplacePanel.Controls.Add(this.ReplaceTextBox);
            this.ReplacePanel.Controls.Add(this.ReplaceLabel);
            this.ReplacePanel.Location = new System.Drawing.Point(0, 35);
            this.ReplacePanel.Name = "ReplacePanel";
            this.ReplacePanel.Size = new System.Drawing.Size(507, 60);
            this.ReplacePanel.TabIndex = 7;
            // 
            // replaceAllButton
            // 
            this.replaceAllButton.Location = new System.Drawing.Point(397, 34);
            this.replaceAllButton.Name = "replaceAllButton";
            this.replaceAllButton.Size = new System.Drawing.Size(108, 26);
            this.replaceAllButton.TabIndex = 4;
            this.replaceAllButton.Text = "すべて置換(&A)";
            this.replaceAllButton.UseVisualStyleBackColor = true;
            this.replaceAllButton.Click += new System.EventHandler(this.replaceAllButton_Click);
            // 
            // replaceNextButton
            // 
            this.replaceNextButton.Enabled = false;
            this.replaceNextButton.Location = new System.Drawing.Point(397, -1);
            this.replaceNextButton.Name = "replaceNextButton";
            this.replaceNextButton.Size = new System.Drawing.Size(108, 26);
            this.replaceNextButton.TabIndex = 10;
            this.replaceNextButton.Text = "置換して次に(&R)";
            this.replaceNextButton.UseVisualStyleBackColor = true;
            this.replaceNextButton.Click += new System.EventHandler(this.replaceNextButton_Click);
            // 
            // ReplaceTextBox
            // 
            this.ReplaceTextBox.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReplaceTextBox.Location = new System.Drawing.Point(141, 3);
            this.ReplaceTextBox.Name = "ReplaceTextBox";
            this.ReplaceTextBox.Size = new System.Drawing.Size(251, 20);
            this.ReplaceTextBox.TabIndex = 2;
            // 
            // ReplaceLabel
            // 
            this.ReplaceLabel.AutoSize = true;
            this.ReplaceLabel.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReplaceLabel.Location = new System.Drawing.Point(3, 9);
            this.ReplaceLabel.Name = "ReplaceLabel";
            this.ReplaceLabel.Size = new System.Drawing.Size(131, 15);
            this.ReplaceLabel.TabIndex = 12;
            this.ReplaceLabel.Text = "置換後の文字列(&P):";
            // 
            // findDialog
            // 
            this.AcceptButton = this.findButton;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(512, 131);
            this.Controls.Add(this.ReplacePanel);
            this.Controls.Add(this.findCourseGroupBox);
            this.Controls.Add(this.LgSmCheckBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.findButton);
            this.Controls.Add(this.findTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "findDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.findCourseGroupBox.ResumeLayout(false);
            this.findCourseGroupBox.PerformLayout();
            this.ReplacePanel.ResumeLayout(false);
            this.ReplacePanel.PerformLayout();
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
        private System.Windows.Forms.Panel ReplacePanel;
        private System.Windows.Forms.Button replaceAllButton;
        private System.Windows.Forms.Button replaceNextButton;
        private System.Windows.Forms.TextBox ReplaceTextBox;
        private System.Windows.Forms.Label ReplaceLabel;
    }
}