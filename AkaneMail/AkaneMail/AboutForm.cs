using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AkaneMail
{
    public partial class AboutForm : Form
    {
        // アドレスを直書きから変数に変更
        string strHomeUrl = "http://www.angel-teatime.com/";

        public AboutForm()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            labelNmailVersion.Text = "nMail.dll Version " + nMail.Options.Version;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //リンク先に移動したことにする
            linkLabel1.LinkVisited = true;

            //ブラウザで開く
            System.Diagnostics.Process.Start(strHomeUrl);
        }
    }
}