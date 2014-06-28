namespace AkaneMail
{
    partial class SplashScreen
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
            if (disposing && (components != null)) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.labelProgress = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelProgress
            // 
            this.labelProgress.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelProgress.BackColor = System.Drawing.Color.Transparent;
            this.labelProgress.Location = new System.Drawing.Point(0, 344);
            this.labelProgress.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelProgress.Name = "labelProgress";
            this.labelProgress.Size = new System.Drawing.Size(648, 15);
            this.labelProgress.TabIndex = 0;
            this.labelProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labelVersion
            // 
            this.labelVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelVersion.AutoSize = true;
            this.labelVersion.BackColor = System.Drawing.Color.Transparent;
            this.labelVersion.Location = new System.Drawing.Point(544, 309);
            this.labelVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(90, 15);
            this.labelVersion.TabIndex = 1;
            this.labelVersion.Text = "Version 1.2.1";
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(653, 412);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SplashScreen";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelProgress;
        private System.Windows.Forms.Label labelVersion;
    }
}