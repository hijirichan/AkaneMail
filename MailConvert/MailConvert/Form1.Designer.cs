namespace MailConvert
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.buttonClose = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.radioConvTypeA = new System.Windows.Forms.RadioButton();
            this.radioConvTypeB = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "このツールはAk@Ne! 1.0.1および1.1.0のメールデータを1.2.0以降で\r\n使用可能なメールデータに変換するためのツールです。";
            // 
            // buttonConvert
            // 
            this.buttonConvert.Location = new System.Drawing.Point(51, 84);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(153, 23);
            this.buttonConvert.TabIndex = 1;
            this.buttonConvert.Text = "メールデータを変換(&C)";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(210, 84);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "閉じる(&X)";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "メールデータ(*.dat)|*.dat|すべてのファイル(*.*)|*.*";
            // 
            // radioConvTypeA
            // 
            this.radioConvTypeA.AutoSize = true;
            this.radioConvTypeA.Checked = true;
            this.radioConvTypeA.Location = new System.Drawing.Point(51, 40);
            this.radioConvTypeA.Name = "radioConvTypeA";
            this.radioConvTypeA.Size = new System.Drawing.Size(167, 16);
            this.radioConvTypeA.TabIndex = 3;
            this.radioConvTypeA.TabStop = true;
            this.radioConvTypeA.Text = "1.01から1.20の形式に変換する";
            this.radioConvTypeA.UseVisualStyleBackColor = true;
            // 
            // radioConvTypeB
            // 
            this.radioConvTypeB.AutoSize = true;
            this.radioConvTypeB.Location = new System.Drawing.Point(51, 62);
            this.radioConvTypeB.Name = "radioConvTypeB";
            this.radioConvTypeB.Size = new System.Drawing.Size(167, 16);
            this.radioConvTypeB.TabIndex = 4;
            this.radioConvTypeB.TabStop = true;
            this.radioConvTypeB.Text = "1.10から1.20の形式に変換する";
            this.radioConvTypeB.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 119);
            this.Controls.Add(this.radioConvTypeB);
            this.Controls.Add(this.radioConvTypeA);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ak@Ne! Mail Converter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RadioButton radioConvTypeA;
        private System.Windows.Forms.RadioButton radioConvTypeB;
    }
}

