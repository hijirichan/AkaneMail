namespace AkaneMail
{
    partial class MessageLog
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
            if (disposing && (components != null)) {
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
            this.components = new System.ComponentModel.Container();
            this.listMessageLog = new System.Windows.Forms.ListView();
            this.MessageColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SendAtColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LogLevelColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BindingSourceMessageLog = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.BindingSourceMessageLog)).BeginInit();
            this.SuspendLayout();
            // 
            // listMessageLog
            // 
            this.listMessageLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LogLevelColumn,
            this.MessageColumn,
            this.SendAtColumn});
            this.listMessageLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listMessageLog.Location = new System.Drawing.Point(0, 0);
            this.listMessageLog.Name = "listMessageLog";
            this.listMessageLog.Size = new System.Drawing.Size(1018, 726);
            this.listMessageLog.TabIndex = 0;
            this.listMessageLog.UseCompatibleStateImageBehavior = false;
            this.listMessageLog.View = System.Windows.Forms.View.Details;
            // 
            // MessageColumn
            // 
            this.MessageColumn.DisplayIndex = 0;
            this.MessageColumn.Text = "メッセージ";
            // 
            // SendAtColumn
            // 
            this.SendAtColumn.Text = "日時";
            // 
            // LogLevelColumn
            // 
            this.LogLevelColumn.DisplayIndex = 1;
            this.LogLevelColumn.Text = "ログレベル";
            // 
            // MessageLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1018, 726);
            this.Controls.Add(this.listMessageLog);
            this.Name = "MessageLog";
            this.Text = "メッセージログ";
            ((System.ComponentModel.ISupportInitialize)(this.BindingSourceMessageLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listMessageLog;
        private System.Windows.Forms.ColumnHeader MessageColumn;
        private System.Windows.Forms.ColumnHeader LogLevelColumn;
        private System.Windows.Forms.ColumnHeader SendAtColumn;
        private System.Windows.Forms.BindingSource BindingSourceMessageLog;
    }
}